# SWIFolder Reverse Report

## 1. Scope and conclusion

This folder is not a single easy-to-decompile `.NET` app. It is a native Windows application package centered on a `PE32` executable plus several native DLLs, external text configs, image resources, CSV parameter maps, and a SQL Server backup.

What can be extracted reliably in the current pass:

- program module split and responsibilities
- exported native interfaces between the EXE and DLLs
- PLC communication interface shape
- machine setup rules stored in config and CSV files
- job/template data grammar from Quick Pick sample library
- database table and business entity clues from the `.bak`
- the exact logic of the small `.NET` helper `ifjFileHandler.exe`

What cannot be fully restored without deeper native reverse engineering:

- complete C/C++ source for `SWIFolder.exe`
- internal class names and full control flow inside `SWIFolder.exe` / `FoldingMachineDll.dll`
- the exact implementation of fold-sequence cost calculation and search heuristics

## 2. Binary inventory

Core binaries identified in this folder:

| File | Type | Notes |
| --- | --- | --- |
| `SWIFolder.exe` | native `PE32` | main application shell, version `4.18.0.4` |
| `FoldingMachineDll.dll` | native `PE32` | fold-sequence and model-parameter algorithm DLL, version `4.17.0.13` |
| `EthernetIP.dll` | native `PE32` | PLC tag read/write wrapper over EtherNet/IP |
| `ifjFileHandler.exe` | managed `.NET` `PE32` | small launcher / single-instance wakeup helper, version `1.0.0.0` |
| `FoldingMachineGUI_*.dll` | native `PE32` | language resource DLLs for `en-AU`, `fr`, `ru`, `zh-CHS` |
| `sfml-graphics-2.dll`, `sfml-system-2.dll`, `sfml-window-2.dll` | native `PE32` | graphics/UI support |
| `Vds176.dll` | native `PE32` | third-party dependency |

Important non-binary files:

- `config.txt`
- `config - Full.txt`
- `config - OfficeMode.txt`
- `SWIFolder.conf`
- `IPAddress.txt`
- `anglemaptables.txt`
- `Folder data values.csv`
- `SWIFolder data values.csv`
- `library_quickpick.txt`
- `DB\SWI-metric.bak`

## 3. High-level architecture

### 3.1 Main shell: `SWIFolder.exe`

The main EXE imports these key DLL interfaces:

- from `FoldingMachineDll.dll`
  - `generate_fold_sequence`
  - `read_model_parameters`
  - `write_model_parameters`
  - `write_fold_preferences`
  - `xy_to_polar`
  - `polar_to_xy`
- from `EthernetIP.dll`
  - `InitialiseEthernetIP`
  - `ShutDownEthernetIP`
  - `RequestPLCTagRead(...)`
  - `RequestPLCTagWrite(...)`
  - `GetLastEthernetIPError`

Other imported subsystems show the app responsibilities directly:

- `ODBC32.dll`: SQL database jobs/orders/customer data
- `WINSPOOL.DRV`: label/report printing
- `COMDLG32.dll`: file open dialogs
- `ADVAPI32.dll`: registry-backed settings
- `SHELL32.dll`: launching/opening files
- `d3d9.dll`, `d3dx9_27.dll`, `sfml-*.dll`: 2D/3D rendering and UI graphics

### 3.2 Algorithm DLL: `FoldingMachineDll.dll`

Exported symbols strongly suggest that this DLL owns the geometry and fold-planning logic:

- `generate_fold_sequence`
- `read_model_parameters`
- `write_model_parameters`
- `write_fold_preferences`
- `xy_to_polar`
- `polar_to_xy`
- global vectors `PossibleActionList` and `PossibleFoldList`

This is the clearest evidence that the EXE is mostly UI/orchestration, while actual folding sequence generation is delegated to this DLL.

### 3.3 PLC comms DLL: `EthernetIP.dll`

This DLL exposes typed tag accessors:

- `RequestPLCTagRead` overloads for `char/int/float/double/long/bool` style payloads
- `RequestPLCTagWrite` overloads for the same
- `GetBitFromWord`
- `SetIPAddress`
- `InitialiseEthernetIP`
- `ShutDownEthernetIP`

Error strings confirm it talks directly to PLC tags:

- `IOI could not be deciphered or Tag in PLC does not exist`
- `Other status code i havent allowed for ->`
- `Extended Status -`

This means the HMI is not just loading files; it is acting as a live machine-side operator interface over EtherNet/IP.

## 4. Exact helper-program logic: `ifjFileHandler.exe`

This is the only managed program in the package, and its entry logic is recoverable.

Recovered behavior of `ifjFileHandler.Program.Main(string[] args)`:

1. Call `Process.GetProcessesByName("SWI Folder Interface")`.
2. If a process already exists:
   - if at least one CLI argument is present, write it to registry:
     - `HKEY_LOCAL_MACHINE\SOFTWARE\SWI Engineering\SWI Folder Interface`
     - value name: `Filename`
   - send Windows message `41251` to the existing main window
   - call `SetForegroundWindow` on that window
3. If no process exists:
   - start:
     - `C:\Program Files\SWI Engineering\SWI Folder Interface\SWI Folder Interface.exe`
   - working directory:
     - `C:\Program Files\SWI Engineering\SWI Folder Interface\`
   - forward the first CLI argument as startup argument if one exists

Interpretation:

- this helper is a file-association / single-instance bridge
- it passes an external filename into the running main app via registry + custom window message
- it is not part of the folding algorithm

Note:

- the helper expects `SWI Folder Interface.exe`, but this package contains `SWIFolder.exe`
- that suggests either an installed-name vs package-name difference, or a later binary rename

## 5. Feature domains confirmed from strings, images, and config

The package clearly contains these machine and workflow domains:

- Auto mode
- Semi-auto mode
- Manual mode
- Setup mode
- Library / Quick Pick
- Top apron / bottom apron motion
- Clamp control
- Backgauge control
- Slitter control
- Tapered profile handling
- Squash folds
- Swage
- Duplex / simplex machine modes
- Rollformer job entry
- PDF flashing report generation
- Label printing
- Barcode scanning
- Multi-operator support
- Micronet integration switch
- SQL database job/order/customer workflow

This is also reflected by image/resource names such as:

- `Auto1`, `Auto2`, `Manual`, `SetupMode1`, `SetupMode2`
- `BackgaugePedal`, `ClampPedal`, `StartPedal`
- `GeneratePDF`, `MaterialEdit`, `GaugeEdit`
- `SendToMicronet`
- `Simplex`, `Duplex`, `SlitMode`

## 6. Config-driven machine rules

The program behavior is heavily parameterized outside the EXE.

### 6.1 `SWIFolder.conf`

This looks like the active machine/site override file. Notable confirmed rules:

- working directory:
  - `C:\Jing Gong Flashings\TestSave\`
- current language:
  - `en-AU`
- angle display:
  - `AngleDisplayPressBrake 1`
- machine limits:
  - `MaximumAutoFolds 100`
  - `MaximumSlitterOperations 12`
  - `MaximumSemiAutoOperations 20`
  - `MaximumFoldAngle 145`
  - `MaxGirthWarning 1220`
  - `MaxLengthWarning 8200`
- angle compensation:
  - `AngleMappingTable 8`
  - `PLC Angle map entries 15`
  - `Angle Decimal Places 1`
- operator / workflow options:
  - `Learn and use operators choice of fold sequences: 1`
  - `Reset start fold: 2`
  - `Clear Semi-auto sequence with new job: 0`
- machine hardware feature flags:
  - `Loading tables installed 1`
  - `Loading tables enabled 0`
  - `PLC has MiscControlWord2 1`
  - `Servo valve installed 0`
  - `Use material thickness 0`

### 6.2 SQL and job-management rules

Confirmed from config:

- `SQLDatabaseEnabled 1`
- `SQLDatabaseUserID "SWI"`
- `SQLDatabasePassword "SWIeng1neering"`
- `SQLDatabaseDSN "SWI"`
- `SQLDatabaseName "SWI"`
- `SQLServerVersion 14.00`
- `MachineID: SWIFolder`
- `JobEditMode 0`
- `UseOrders 0`
- `OnBarcodeScanLoadJobsWithSameShapeID 1`
- `MergeCurrentFoldingProfileWithNewJob 1`
- `CountingOfCompletedJobs 2`
- `ManualCompleteJobButton 0`

Business meaning:

- the app can work with a SQL-backed job database
- it supports barcode-driven job lookup
- it can merge a new job into the current folding profile
- completed-job counting is configurable and currently disabled in this machine profile

### 6.3 Office mode template

`config - OfficeMode.txt` confirms the software also supports an off-machine office workflow.

Notable differences in that template:

- `OfficeMode 1`
- `WordForJobName "Order:"`
- `WordForJobItem "Order item:"`
- address fields visible
- on-screen keyboard disabled
- `ShowNewJobDialog 1`
- `WorkingDirectory "C:\temp\"`
- `TemplatesDirectory "C:\ProfTest\"`

Interpretation:

- there is a dual personality:
  - machine HMI mode
  - office/order-entry/template-prep mode

## 7. PLC parameter map recovered from CSV

The strongest rule extraction comes from `SWIFolder data values.csv` and `Folder data values.csv`.

These files map machine setup values to `SetupWord[DINT[100]]` indexes and PLC tag names.

### 7.1 Parameter groups by index range

- `SetupWord[0..15]`: top/bottom slide gain, max speed, creep speed, home speed, home offsets
- `SetupWord[19..32]`: angle limits, angle calibration, apron up/down gains and speeds, manual apron speed
- `SetupWord[40..45]`: backgauge calibration, forward/reverse RPM, encoder P223/P71, taper version flag
- `SetupWord[48..58]`: clamp pressures, clamp calibration, creep thresholds, clamp speeds, half-clamp limits
- `SetupWord[60..61]`: hydraulic pump ESTOP speed and idle speed
- `SetupWord[67..83]`: slitter safe speed, forward/reverse delays, creep/fast/slow speeds, operator and pedal rules, imperial flag
- `SetupWord[84..99]`: top apron gain-change point, up/down gains/speeds, lube counters/time, operator count, splash timeout

### 7.2 Confirmed rule semantics

- gain parameters use integer transport for reals:
  - `SetupWord is integer = real value *100`
- some values are explicit machine booleans packed as integers:
  - `B_TaperVersion`
  - `B_SlitterHoldToRun`
  - `B_ApronHomeAlways`
  - `B_StopOn2ndOpertorFault`
  - `B_Enable2ndSlitterFootswitch`
  - `B_ImperialMode`
  - `B_Enable2ndLockFootswitchFor2Operators`
- operator count is PLC-configurable:
  - `NoOfOperators`

This is a major result: a large share of actual machine behavior is driven by structured PLC parameter words, not buried exclusively in native code.

## 8. Angle compensation / springback tables

`anglemaptables.txt` contains named material-thickness profiles such as:

- `SWI 11-08-2022`
- `BHP0.53`
- `BHP0.6`
- `BHP0.8`
- `lvban0.8`
- `lvb0.8`

Each section contains:

- `Material`
- `Thickness`
- `AngleRange10 ... AngleRange150`
- `Angle2Range10 ... Angle2Range150`

Interpretation:

- this is a discrete angle compensation table keyed by material / thickness
- it likely feeds springback compensation or target-angle correction
- because the active config contains `AngleMappingTable 8`, the software almost certainly selects one compensation profile family at runtime

## 9. Job/template grammar extracted from Quick Pick library

`library_quickpick.txt` exposes the internal serialized job/profile format.

Recovered fields include:

- `Name`
- `Sheetlength`
- `Ends`
- repeated fold records:
  - `Offset`
  - `Angle`
  - `Type`
  - `Radius`
  - `ClampHt`
- `SequenceOverride`
- `ReqstFoldPref`
- `MaxGrip`
- `SemiAutoList4`
- `Width`
- `Thickness`
- `Material`
- `Rotation`
- `ReverseColours`
- `Customer`
- `Materialname`
- `Colour`
- `JobWidth`
- `CustomTxt1`
- `CustomTxt2`
- `Tapering`
- `TaperOfs`
- `TaperOffStart`
- `TaperOffEnd`
- `TaperPrincipalAxis`
- `TaperWidth`
- `TaperSplits`
- `JobID`
- `JobLength`
- `JobQuantity`

This is enough to state the internal job model approximately:

- one job/profile contains a flat-sheet length
- a fold list is represented as ordered offset-angle operations
- job data can carry customer/order metadata
- semi-auto step recipes are serialized alongside the profile
- tapering is a first-class feature, not a post-process hack

## 10. Database clues from `DB\SWI-metric.bak`

The `.bak` file is rich in SQL Server metadata and string references. Recovered business entities include:

- `Jobs`
- `JobsArchived`
- `JobsDeleted`
- `Orders`
- `OrdersArchived`
- `Customer`
- `Material`
- `Colours`
- `StatsMachine`
- `StatsMaterial`
- `FolderPLCmessages`
- `MarxmanPLCmessages`
- `UsedJobIDs`

Recovered field/entity clues include:

- `WorkOrder`
- `SalesOrder`
- `RecoilJobID`
- `MaterialDiscountL2`
- `MaterialDiscountL3`
- `AlternativeColours`
- `QtyFolded`
- `GetNextJobID`
- `AddNewJob`

Interpretation:

- the database stores both active and archived jobs/orders
- it likely stores customer/material/color master data
- PLC messages can be localized or database-driven
- machine statistics and used job IDs are tracked

## 11. Network and machine binding

Confirmed machine-side binding data:

- `IPAddress.txt`
  - `10.1.1.10`
- `SWIFolder.conf`
  - `MachineID: SWIFolder`

Combined with `EthernetIP.dll`, this is strong evidence that the package is intended to communicate with a live PLC at that IP or a configured equivalent.

## 12. Most important logic recovered so far

The current pass supports these concrete conclusions:

1. The app is a folding-machine HMI/job-preparation system, not just a drawing viewer.
2. Machine execution is driven by a mix of:
   - native UI/orchestration code
   - folding algorithm DLL
   - EtherNet/IP PLC tag access
   - external config and PLC setup-word tables
3. Fold-sequence generation is centralized in `FoldingMachineDll.dll`.
4. Angle compensation is table-driven by material/thickness data.
5. Many machine behaviors are explicit configuration rules, including:
   - fold limits
   - slitter timings
   - clamp pressures
   - backgauge encoder parameters
   - taper geometry
   - operator-count logic
   - office/machine mode switching
6. The package contains a SQL-backed job/order/customer workflow with barcode and printing support.
7. The `.NET` helper exists only to pass a filename into a running instance and foreground the main app.

## 13. Recommended next reverse-engineering steps

If a deeper pass is needed, the best next order is:

1. Load `FoldingMachineDll.dll` in Ghidra/IDA first.
   - focus on `generate_fold_sequence`
   - recover the fold/action structures and scoring logic
2. Then inspect `SWIFolder.exe`.
   - identify where config keys and `SetupWord[]` data are loaded
   - identify where `EthernetIP.dll` tag reads/writes are called
3. Parse the SQL backup with SQL Server tools.
   - extract table schema and stored procedures
4. If needed, capture runtime traffic.
   - watch registry key `HKLM\SOFTWARE\SWI Engineering\SWI Folder Interface`
   - watch EtherNet/IP tag names requested by the EXE

## 14. Folding-sequence algorithm deep dive

This section focuses specifically on `FoldingMachineDll.dll` and the exported function:

- `generate_fold_sequence`
- entry RVA: `0x000157F0`
- return RVA: `0x00018F6D`
- approximate function length: `0x377D`

### 14.1 Entry-stage setup

At function entry, `generate_fold_sequence` performs a consistent setup sequence:

- stores a float argument into global `0x1003B0D8`
  - this is later used in score math and matches the role of `GripMaximisingCoefficient`
- stores a float argument into global `0x1003B0F0`
  - this is also used in piecewise score math
- copies three integers from one pointer argument into `0x1003B0DC[0..2]`
- stores a byte/boolean argument into `0x100ABE16`
- stores a float timeout argument into global double `0x100ABE30`
  - this is the search time limit used later for early exit
- initializes a large local candidate pool via `0x100043D0`
- initializes a large global candidate pool at `0x100745F8` via `0x100043D0`
- clears/normalizes the two output vectors through helper calls `0x1001A880` and `0x1001AA30`

This confirms the function is not a simple calculator. It sets up a full search session with:

- shared search globals
- a local working candidate pool
- output containers
- time/budget limits

### 14.2 Two core structure families

The disassembly shows two very different structure sizes being managed.

#### A. Large candidate/action pool objects

Helpers:

- accessor: `0x1001A6F0`
- count: `0x1001A6B0`
- push/pop helpers: `0x1001A730`, `0x1001A7B0`, `0x1001A800`

Observed properties:

- element stride: `0x37710`
- internal reset function: `0x100043D0`
- `0x100043D0` initializes:
  - `count` at offset `+0`
  - two mode/flag bytes at `+4`, `+5`
  - `1001` subentries of size `0xE0` starting at offset `+0x108`
  - cumulative score/time field at `+0x36CF0`

Interpretation:

- this is the action-candidate pool type used during search
- it can hold up to `1001` action records per pool snapshot
- it stores both:
  - candidate action entries
  - cumulative ranking / score metadata

#### B. Fold-state / sequence-state objects

Helpers:

- copy helpers: `0x10019570`, `0x10019A10`
- nested vector accessor: `0x1001A5B0`
- vector assignment/growth: `0x1001A420`, `0x1001A2C0`

Observed properties:

- nested vector element stride: `0x13C`
- outer structure contains:
  - a `0x80`-byte header block
  - a count at `+0x80`
  - a nested vector object at `+0x84`
  - many additional floats/ints/flags after `+0x9C`
  - geometry and state arrays at offsets like:
    - `+0xC8`
    - `+0x108`
    - `+0x148`
    - `+0x188`
    - `+0x1D8`
    - `+0x228`
    - `+0x728`
    - `+0xC2C`

Interpretation:

- this is the current fold-state / sequence-state object that gets copied before recursion
- it is much richer than a single fold record
- it appears to bundle:
  - current fold order
  - per-step geometry
  - side/orientation flags
  - cached values used by the planner

### 14.3 Recursive search worker

The real search worker is the internal function at:

- `0x1000D4F0`

Evidence that this is the recursive search core:

- it recursively calls itself multiple times:
  - `0x100120EB`
  - `0x100121FE`
  - `0x10012300`
  - `0x10012CD3`
  - `0x10012D9A`
  - `0x10012E96`
  - `0x10012FF5`
  - `0x10013223`
  - `0x10013312`
  - `0x10013415`
- before recursive calls, it clones the current fold-state via `0x10019570`
- it repeatedly accesses nested fold entries through `0x1001A5B0`

This is a classic search / backtracking shape:

- copy current state
- mutate one branch
- recurse
- return with success/failure

### 14.4 Pruning and early exit rules

The worker maintains at least three important global search values:

- `0x100744CC`
  - search node / recursion counter
- `0x1003B0CC`
  - configured maximum permutation budget
  - default observed value from globals: `500000`
- `0x100ABE30`
  - timeout limit

Observed pruning logic in `0x1000D4F0`:

- read clock/tick via `0x10021E4F`
- compare elapsed time against `0x100ABE30`
- if time limit is exceeded and a result already exists in global pool `0x100745F8`, return early
- compare node counter `0x100744CC` against `0x1003B0CC`
- if the search has already found results and exceeded permutation budget, return early
- compare again against approximately `2 * MaximumPermutations`
- hard-stop if even the expanded budget is exceeded

Interpretation:

- this is not exhaustive search without limits
- it is a bounded search with two stop modes:
  - time budget
  - permutation budget
- once at least one valid candidate exists, the search is willing to stop early

### 14.5 Geometry-path helpers

Several internal helpers operate on polyline-like point lists.

#### `0x1000A070`

This function iterates points with stride `0x14` and repeatedly calls:

- `0x10009800`
- `0x100098F0`

It reads point data from offsets:

- `+0x1C`
- `+0x20`

Interpretation:

- this is a coordinate-transform helper
- it likely converts or reprojects profile points between geometric spaces
- it appears to apply an angle/offset transform to the current shape

#### `0x1000A110`

This helper:

- scales one coordinate component for every point
- toggles bit `2` in a mode field at offset `+0x18`

Interpretation:

- this looks like an orientation/mirroring or side-flip transform

#### `0x1000B100`

This helper:

- walks consecutive points
- computes segment lengths using squared deltas and `sqrt`
- accumulates distance until reaching a threshold

Interpretation:

- this is a “distance along profile” helper
- it likely finds a reachable grip/clamp/contact point on the folded outline

#### `0x1000B5A0`

This helper performs a similar segment walk, but with much more bookkeeping and output-field writes.

Interpretation:

- this appears to be the heavier candidate-evaluation version of the same geometry process
- it likely produces a full action record, not just a geometric point

### 14.6 Feasibility checks against machine geometry

The function at:

- `0x1000C3F0`

is one of the clearest machine-feasibility validators.

Evidence:

- it sets a failure/result code output
- it writes failure flag values
- it uses multiple model globals:
  - `0x1003B078`
  - `0x1003B07C`
  - `0x1003B080`
  - `0x1003B084`
  - `0x1003B094`
- it checks orientation bytes from the current state
- it can immediately fail with result code `9`

Interpretation:

- this function evaluates whether a candidate action is mechanically legal
- it uses current machine model geometry and current fold-side/orientation state
- it is part of the hard constraint layer, not just soft scoring

### 14.7 Grip-related hard constraints

Two global values line up closely with config terminology:

- `0x1003B0D4 = 19.5`
  - matches `Min Grip Position`
- `0x100ABE2C`
  - used as an additive offset
  - behaves like `Push-Grip Offset`

Evidence:

- around `0x100100A8`, the code evaluates:
  - `-(Min Grip Position + Push-Grip Offset)`
- if the current candidate violates this boundary, branch flags are set and the search path can be rejected or altered
- `0x1000ED40` and nearby logic repeatedly reuses `0x100ABE2C` while deciding edge cases near profile ends

Interpretation:

- grip reach is enforced as a hard feasibility rule
- the algorithm does not merely prefer better grip positions; it blocks invalid ones

### 14.8 GripMaximisingCoefficient is a scoring weight, not a hard gate

The strongest evidence for a real ranking score is in:

- `0x1000A923 .. 0x1000AB03`

Observed behavior:

- it maintains a cumulative floating score in local `qword [ebp-0x3C]`
- it repeatedly adds penalty/bonus terms involving:
  - `0x1003B0D8`
  - `0x1003B0F0`
- in some branches it uses roughly:
  - `coefficient^2 / constant`
- in others it uses:
  - `coefficient / constant`
  - or a piecewise expression derived from `0x1003B0F0`
- the resulting cumulative score is stored into action records at offsets like `+0x1B0`

Interpretation:

- `GripMaximisingCoefficient` directly affects candidate ranking
- larger values increase the cost impact of grip-related considerations
- this parameter biases the planner toward grip-favorable sequences rather than merely rejecting impossible ones

### 14.9 Result selection and output handoff

At the end of the main function, the planner compares candidate pool scores and preserves the best one.

Important addresses:

- `0x100ABE44`
- `0x100ABE5C`
- `0x100745F8`
- `0x100745F0`
- `0x100ABE90`

Observed tail behavior:

- compare candidate cumulative score `+0x36CF0` between pools
- if no previous best exists, or the new score is better:
  - copy `0x37710` bytes from local working pool to global best pool `0x100745F8`
  - copy the paired fold-state via `0x10019A10` into `0x100ABE90`
  - store the winning score into `0x100745F0`
- if not directly better, it scans all candidate pools looking for an equal/better match
- helper pairs `0x1001A730` / `0x1001A8E0` move the selected objects toward the final output vectors

Interpretation:

- the algorithm does not emit the first feasible sequence
- it keeps and updates a "best-so-far" candidate based on cumulative score
- fold-state and action-pool outputs are preserved together as a pair

### 14.10 Working reconstruction of the algorithm

Based on the current disassembly, the folding planner is best described as:

1. Build an initial fold-state and clear candidate pools.
2. Load machine geometry, grip settings, search budget, and time budget into globals.
3. Recursively explore alternative fold/action branches.
4. At each branch:
   - clone current state
   - transform geometry
   - compute path distances / reachability
   - run machine-feasibility checks
   - assign cumulative score penalties/bonuses
5. Keep the best valid candidate found so far.
6. Stop early if:
   - time is exhausted and at least one valid solution already exists
   - permutation budget is exceeded and at least one valid solution already exists
   - hard upper search cap is exceeded
7. Copy the best action pool and paired fold-state back to the caller's output vectors.

This is much closer to a bounded branch-and-bound search with geometry validation than to a simple greedy fold-order routine.

### 14.11 Action-record field map recovered so far

The `0xE0`-byte action record used inside candidate pools is now partially recoverable from repeated reads/writes.

Confirmed or strongly indicated fields:

| Offset | Likely role | Evidence |
| --- | --- | --- |
| `+0x109` | boolean feature flag A | counted in `0x10009970`, also used in score logic in `0x1000A3C0` |
| `+0x10A` | boolean feature flag B | counted in `0x10009970` |
| `+0x10B` | neighbor/continuation flag | checked in `0x1000A3C0` when evaluating adjacent actions |
| `+0x110` | float metric with `1000.0` threshold | compared in `0x10009970` |
| `+0x118` | displayed float field | formatted with `%3.1f` in `0x10009970` |
| `+0x124` | primary action type enum | explicitly checked in `0x1000A3C0`; explicitly written as `1` in `0x10009970` |
| `+0x128` | displayed integer field | formatted with `%d` in `0x10009970` |
| `+0x12C` | suppression/inversion flag | counted inversely in `0x10009970` |
| `+0x150` | inherited reference / linkage index | written in `0x1000B100` from `+0x154` or `+0x158` |
| `+0x154` | one orientation-specific source index | selected when side flag is false in `0x1000B100` |
| `+0x158` | opposite orientation source index | selected when side flag is true in `0x1000B100` |
| `+0x164` | displayed integer field | formatted with `%d` in `0x10009970`, written in `0x1000B100` |
| `+0x17A` | boolean branch-availability flag | tested in `0x1000A3C0` |
| `+0x180` | multiplicity / remaining-count field | decremented and multiplied in `0x1000A3C0` |
| `+0x1A0` | sub-state flag | compared against `1` in `0x1000A3C0` |
| `+0x1A4` | secondary status/type enum `0..7` | repeatedly compared against `0,1,2,3,4,5,6,7` in `0x1000A3C0` |
| `+0x1B0` | cumulative score snapshot | written in `0x1000A3C0` |

Practical interpretation:

- `+0x124` is the main action family/type code
- `+0x1A4` is a secondary subtype/status code used by score logic
- `+0x150` links an action to a previous/related action slot
- `+0x118`, `+0x128`, `+0x164` are user-facing summary values that get formatted into the candidate labels

### 14.12 One confirmed action family: type `1`

The function:

- `0x10009970`

is now the clearest confirmed builder/finalizer for one action family.

Evidence:

- it copies the incoming candidate score into `pool+0x36CF0`
- it formats three summary columns from action fields:
  - `+0x128` with `%d`
  - `+0x118` with `%3.1f`
  - `+0x164` with `%d`
- it counts feature flags `+0x109`, `+0x10A`, `+0x12C`
- it explicitly writes:
  - `action[slot].type = 1`
  - exact instruction at `0x10009C37`

So at minimum we can now state:

- action type enum value `1` definitely exists
- `0x10009970` constructs or finalizes that type before it is compared against the global best

### 14.13 Search-control path for the type `1` branch

One clear control-flow slice is the block around:

- `0x10013040 .. 0x10013333`

Recovered sequence:

1. Call `0x1000A3C0` to score / summarize the current local candidate pool.
2. Clone fold-state `0x100AD3D0` with `0x10019570`.
3. Call `0x10009970` on candidate pool `0x1003CDB0`.
4. Compare the resulting score against global best `0x100745F0`.
5. If better:
   - copy candidate pool to `0x100745F8`
   - copy fold-state to `0x100ABE90`
6. Also mirror the current local result into:
   - `0x1003CDB0`
   - `0x100AD3D0`
   - `0x1003C588`
7. If a mode flag is enabled, recurse into `0x1000D4F0` again with cloned snapshots to explore alternate continuation branches.

Interpretation:

- the planner has a concrete "build candidate -> compare against best -> optionally recurse further" pipeline
- `0x10009970` sits on the direct handoff boundary between local candidate construction and global-best selection

### 14.14 What `0x1000B100` contributes

The helper:

- `0x1000B100`

does not appear to be the top-level scorer. It looks more like a geometric preprocessor for one candidate action.

Evidence:

- it accumulates segment lengths from consecutive polyline points
- it computes a threshold crossing along the current shape
- it writes:
  - `+0x150` from either `+0x154` or `+0x158`, depending on orientation byte `+5`
  - `+0x164` from a source field at `state+0x18`
- it then shifts all profile points by subtracting two local deltas

Interpretation:

- this helper likely determines where the action anchors on the current profile
- it also stores the reference/index that later appears in the candidate text fields
- it is part of candidate construction, not just final ranking

### 14.15 `0x1000C3F0` output contract is now partially recoverable

The callsites around:

- `0x1000DDDA`
- `0x1000DFF2`
- `0x1001082B`
- `0x10011C65`

show a stable calling pattern for `0x1000C3F0`.

Recovered output behavior:

- argument `+0x0C` is an output integer pointer
  - caller later stores it into action field `+0x1E94`
- argument `+0x18` is an output float pointer
  - caller later stores it into action field `+0x1EDC`
- argument `+0x1C` is an output float pointer
  - caller later stores it into action field `+0x1EE0`
- argument `+0x24` is an output flag byte pointer
  - caller uses it as a retry / branch-control signal

Additional observed behavior inside `0x1000C3F0`:

- on one early failure path it sets output code `9`
- it writes the retry/control flag before returning failure
- it uses input orientation bytes from the current fold-state
- it compares current geometric values against machine-model globals and several constants

Interpretation:

- `0x1000C3F0` is not just a yes/no validator
- it returns:
  - a failure or classification code
  - two derived geometric quantities
  - a flag telling the caller whether an alternate mirrored retry is worth attempting

### 14.16 Mirrored retry path is explicit

The branch around:

- `0x1000DD8C .. 0x1000E2DC`

shows a very clear fallback sequence.

Recovered logic:

1. Clone the current fold-state and local action pool.
2. Run `0x1000B100`.
3. Run `0x1000C3F0`.
4. If validation fails but the returned retry flag allows it:
   - restore the cloned state
   - call `0x1000A110`
   - flip several orientation flags:
     - state byte at `+0x0C`
     - action bytes around `+0x1F28`, `+0x1F29`
     - when available, also `+0x1E79`, `+0x1E7A`
   - rerun `0x1000B100`
   - rerun `0x1000C3F0`

Interpretation:

- the planner explicitly tries the mirrored/opposite-side variant of the same action when the first attempt is infeasible
- this is strong evidence that side/orientation is part of the search state, not just a display property

### 14.17 Intermediate metrics `0x100744C0` and `0x100744C4`

Two temporary globals are written by geometry-building paths and then read by later classification logic:

- `0x100744C0`
- `0x100744C4`

Observed facts:

- they are zeroed before some validation passes
- they are repopulated during geometry evaluation
- after `0x1000C3F0`, callers compare `0x100744C0` against these model thresholds:
  - `0x1003B0B8`
  - `0x1003B0BC`
  - `0x1003B0C0`
- based on those thresholds, the caller stores action class bucket:
  - `1`
  - `2`
  - `3`
  - `4`
  - into field `+0x1E90`

Interpretation:

- `0x100744C0` is some derived geometric or clearance metric
- it is used to classify the candidate into one of four bands
- those bands are not the same as the main action type at `+0x124`; they are a secondary runtime classification

### 14.18 Additional per-action runtime fields

From the post-validation branches, several more action-record fields can now be described:

| Offset | Likely role | Evidence |
| --- | --- | --- |
| `+0x1E79` | side-dependent flag A | set/reset before and after mirrored retry |
| `+0x1E7A` | side-dependent flag B | toggled with mirrored retry |
| `+0x1E7B` | branch-enabled marker | initialized to `1` in one builder path |
| `+0x1E7C` | float metric carried across retry | copied out and restored across mirror attempt |
| `+0x1E90` | four-band class bucket | assigned from threshold checks against `0x100744C0` |
| `+0x1E94` | result/status code from `0x1000C3F0` | written after validation call |
| `+0x1E98` | integer label/anchor reference | used in candidate text formatting |
| `+0x1E9C` | boolean valid/usable flag | counted in report summary loop |
| `+0x1EEA` | copied byte from current fold entry `+0x40` | propagated into next action record |
| `+0x1EF0`, `+0x1EF4` | inherited integer state values | copied forward between action records |
| `+0x1EF8` | max encountered edge/height-like value | updated by scanning shape points |
| `+0x1EFC`, `+0x1F00` | stored float context values | copied from local state after validation |
| `+0x1F14` | local subtype bucket | assigned values such as `2`, `3`, `6`, `7` in one branch family |
| `+0x1F28`, `+0x1F29` | orientation flags | toggled in mirrored retry |

Practical interpretation:

- the action record is not just a one-step command
- it carries enough runtime context to:
  - describe the candidate
  - resume search from it
  - compare mirrored variants
  - display a summarized label to the user

### 14.19 `type=1` is reused in multiple planner modes

Callsites to `0x10009970` were found at:

- `0x10013093`
- `0x10013612`
- `0x10013B6D`
- `0x10014100`
- `0x100146C1`
- `0x10014C3A`
- `0x100151EA`
- `0x10015763`

This matters because the surrounding code does not always set the same planner-mode globals before calling it.

Example:

- near `0x10013141`, the code sets:
  - `0x1003C58D = 1`
  - `0x1003C58C = 0`
  - `0x1003C5A0 = 3`
- near `0x100136C8`, the code sets:
  - `0x1003C58D = 0`
  - `0x1003C58C = 1`
  - `0x1003C5A0 = 2`

Interpretation:

- `type=1` is not a single rigid UI action
- it is a reusable action family emitted under multiple planner modes/submodes
- the exact operational meaning of type `1` likely depends on surrounding mode flags and current side/orientation state

### 14.20 Mechanical feasibility result codes from `0x1000C3F0`

The feasibility function `0x1000C3F0` sets an integer result/status code through its output pointer. The following codes are now confirmed from direct assignments:

| Code | Hard/soft behavior | Evidence address | Current interpretation |
| --- | --- | --- | --- |
| `0` | default / no detected issue | `0x1000C400` | initialized before checks |
| `1` | soft penalty | `0x1000C77D` | point/profile violates a backgauge or low-clearance region; adds `1,000,000` to score |
| `2` | hard reject | `0x1000C865` | angular/rotation limit breach; function returns false |
| `3` | hard reject | `0x1000C8F2` | clamp-gap / machine-envelope collision; function returns false |
| `4` | soft penalty | `0x1000C924` | overgrip or near-limit condition; adds `2,000,000` to score |
| `6` | hard reject | `0x1000CC41`, `0x1000CC8B` | profile exceeds a positive or negative maximum height/position limit; function returns false |
| `7` | soft cubic penalty | `0x1000D1D2`, `0x1000D46F`, `0x1000D4D6` | near-collision / clearance margin cost; stores margin to second float output |
| `8` | soft penalty | `0x1000CDCD`, `0x1000CE28` | reverse-side height/position near-limit; adds `1,000,000` to score |
| `9` | hard reject with retry flag | `0x1000C46D`, `0x1000C48B` | orientation/fold-side mismatch; sets mirror-retry flag and returns false |
| `10` | soft penalty | `0x1000CD04`, `0x1000CD57` | opposite-side maximum height/position condition; adds `1,000,000` to score |

Important distinction:

- codes `2`, `3`, `6`, `9` are confirmed hard rejection paths
- codes `1`, `4`, `7`, `8`, `10` can still allow a candidate but increase score/cost

This means the planner does not use a pure pass/fail feasibility model. It uses:

- hard rejection for impossible or unsafe states
- soft penalties for undesirable but still usable states

### 14.21 Score model recovered from feasibility checks

The score output pointer passed to `0x1000C3F0` is incremented in several distinct ways.

Confirmed constants:

- `0x100333B8 = 1,000,000`
- `0x10033420 = 2,000,000`
- `0x10033448 = 1000`
- `0x100333B0 = 0.1`

Observed scoring rules:

- code `1`, `8`, `10` add `1,000,000`
- code `4` adds `2,000,000`
- code `7` adds a cubic penalty:
  - `score += margin^3 * 1000`
  - margin is also written to the second float output
- small margins below about `0.1` are ignored before applying cubic penalty

Best-candidate selection uses lower score as better:

- candidate score is compared against global best score `0x100745F0`
- if no best exists or the new score is lower/equal, the candidate pool and fold-state are copied into the global best slots

Extracted rule:

- the algorithm first tries to find mechanically legal candidates
- among legal/usable candidates, it minimizes accumulated penalties
- large fixed penalties make undesirable candidates much worse but not necessarily impossible

### 14.22 Machine-model constants used by feasibility checks

The following globals are directly used by `0x1000C3F0`. Some names are inferred from config value matches and usage context.

| Global | Default observed value | Likely config meaning | Usage in feasibility |
| --- | --- | --- | --- |
| `0x1003B078` | `35.0` | `ClampGap` | base gap/envelope in trigonometric clearance checks |
| `0x1003B07C` | `25.0` | `PreferredHalfClampHeight` or clamp reference height | used to derive geometry clearance from clamp gap |
| `0x1003B080` | `165.0` | `ClampArmLength` | used in trigonometric envelope calculation |
| `0x1003B084` | `100.0` | `UpperArmWidth` | used in trigonometric envelope calculation |
| `0x1003B088` | `35.0` | gap/clearance reference | used as a positional boundary in later clearance loop |
| `0x1003B08C` | `15.0` | `ClampArmWidth` or `ApronHeight` | used in height/position branch around code `6/10` |
| `0x1003B090` | `5.0` | small limit / min default | used in angular hard reject code `2` |
| `0x1003B094` | `1000.0` | large backstop/profile boundary | used in code `1` and profile search window |
| `0x1003B098` | `13.0` | `BackgaugeHeight` | paired with code `1` check |
| `0x1003B09C` | `10.0` | `MaxOvergripHeight` | used in grip/edge conditions outside this function |
| `0x1003B0A0` | `300.0` | maximum clamp/height envelope | used in code `6/8/10` height tests |
| `0x1003B0A4` | `10.0` | overgrip / small clearance limit | used in code `4` branch |
| `0x1003B0C8` | `250.0` | reverse-side height/position threshold | used in code `8` branch |
| `0x1003B0EC` | `35.0` | angular/clearance tolerance | used by cubic margin scoring for code `7` |

Angles are handled internally in radians:

- `0x100332D0 = 180`
- `0x100332D8 = pi`
- `0x10033398 = pi / 2`
- `0x100333A0 = -pi / 2`

This confirms the feasibility logic is geometric and machine-model based, not merely table-based.

### 14.23 Practical reconstruction of mechanical feasibility

A candidate action is checked roughly as follows:

1. Reset status code, score contribution, margin output, and retry flag.
2. If fold orientation conflicts with current side/orientation state:
   - set code `9`
   - set retry flag
   - reject this branch
3. Derive multiple geometric clearances from clamp gap, clamp arm dimensions, and angular position.
4. Scan profile points to locate the relevant active segment/window.
5. Check backgauge/low-clearance zones:
   - add large penalty or reject depending on severity
6. Check angular and clamp envelope constraints:
   - reject hard collisions
7. Track derived extrema in `0x100744C0` and `0x100744C4`.
8. Compute near-collision margins:
   - convert angular/linear margin into a positive scalar
   - if margin exceeds the tiny threshold, add cubic penalty
9. Return true if no hard reject occurred.

Rule-level summary:

- impossible orientation: reject, but signal mirror retry
- hard envelope collision: reject
- soft clearance violation: keep candidate but add very large score
- small clearance margin: keep candidate but add cubic score
- lower final score wins

### 14.24 `0x1000BD60`: cross-candidate clearance margin helper

The helper `0x1000BD60` is called from the tail of `0x1000C3F0` when direct checks are not enough to decide the final clearance margin.

Inputs observed:

- current point/action index
- active window start/end indexes
- mirrored/orientation flag
- current angle value

Behavior:

- iterates over the global candidate/shape pool at `0x100ABE74`
- fetches profile points through `0x1001AAD0`
- converts point coordinates to polar coordinates through `0x10009800`
- compares the current point's polar angle against neighboring point angles
- skips candidates that are too close to both neighboring angles
- otherwise computes a positive angular clearance margin
- returns `0` if no meaningful clearance margin is found

It uses the same margin pattern as `0x1000C3F0`:

- convert angular margin from radians to degrees via `180 / pi`
- ignore margins below about `0.1`
- return a positive scalar margin to the caller

Important constants:

- `0x1003B0EC = 35.0`
  - angular clearance / tolerance used in margin calculation
- `0x10033468 = 3.14259`
  - approximately `pi + 0.001`
- `0x10033470 = -3.14159`
  - approximately `-pi`
- `0x100333B0 = 0.1`
  - tiny margin threshold

Interpretation:

- `0x1000BD60` is a secondary clearance search across already-generated geometry/candidates
- it answers “how close is this candidate to a collision or angular overlap with neighboring geometry?”
- its returned margin feeds the same code `7` cubic penalty path

### 14.25 `0x1000A3C0`: candidate-pool total score model

The function `0x1000A3C0` is the best current candidate for the overall scoring summarizer.

It walks all `0xE0` action records in a candidate pool and accumulates a double score in local `qword [ebp-0x3C]`.

Confirmed scoring components:

| Rule family | Evidence | Score effect |
| --- | --- | --- |
| initial mirrored/flagged pool | state byte `+4` | add `1000` |
| action type counters | counts `+0x124 == 7`, `+0x124 == 4`, and nonzero types | used later to scale penalties |
| repeated / linked actions | fields `+0x150`, `+0x180` | decrement remaining count and add multiplicity penalty |
| grip preference | uses `GripMaximisingCoefficient` global `0x1003B0D8` | adds coefficient-based cost |
| high search/operation count | global `0x1003B0F0` | piecewise penalty when above `1500` |
| feature flag `+0x109` | branch around `0x1000A9AB` | adds penalty based on grip/mode and available linked actions |
| secondary type `+0x1A4` | branch around `0x1000AB03` | adds subtype-specific penalties |
| angle/position magnitude | fields `+0x10C`, `+0x110`, `+0x174` | adds magnitude and ratio penalties |
| multiplicity count | field `+0x180` | adds quadratic or quartic multiplicity penalty depending mode |
| operation-specific penalty | field `+0x16C` | added directly, except type `7` may divide it by `(count - 3)` |
| custom stored penalty | field `+0x198` | added directly if positive |
| negative displayed length/position | field `+0x118` | adds `1` if negative |

Important scoring constants recovered:

- `0x10033310 = 1`
- `0x100333C8 = 500`
- `0x100333D0 = 3`
- `0x100333D8 = 20000`
- `0x100333E0 = 50000`
- `0x100333E8 = 1300`
- `0x100333F8 = 2100`
- `0x10033400 = 2000`
- `0x10033408 = 1700`
- `0x10033410 = 5000`
- `0x10033418 = 120`
- `0x10033420 = 2,000,000`
- `0x10033430 = 10,000`
- `0x10033438 = 1,500`
- `0x10033440 = 5,000,000`
- `0x10033448 = 1,000`

### 14.26 Subtype penalty table from `+0x1A4`

The score logic treats `+0x1A4` as a secondary subtype/status field, separate from main action type `+0x124`.

Confirmed rules:

- subtype `0` with `+0x1A0 == 1`
  - stores score delta into `+0x1B0`
  - this looks like a segment boundary / cumulative snapshot marker
- subtype `1` or `5`
  - uses field `+0x110`
  - if `+0x110 <= 120` and flag `+0x12C` is set:
    - add `(120 - value) * 5000`
  - else:
    - add `(1700 - value)`
- subtype `2` or `6`
  - add `2000`
- subtype `3` or `7`
  - add `2100`
- subtype `4`
  - explicitly skipped as neutral in this branch

Interpretation:

- subtype `1/5` is a graded geometry/position quality class
- subtype `2/6` and `3/7` are fixed-cost quality classes
- subtype `4` is likely a special neutral/accepted case

### 14.27 Grip and repeat-count scoring details

`GripMaximisingCoefficient` affects multiple parts of the total score.

Observed formulas:

- when a preferred linked grip is available:
  - add approximately `1,000,000 / GripMaximisingCoefficient^2`
- when coefficient is below or equal to `1` in one mode:
  - add `1000`
- otherwise:
  - add `2,000,000`
- for unlinked mode:
  - add approximately `1,000,000 / GripMaximisingCoefficient^2`
- if no preferred structure is available:
  - add `10,000`

`0x1003B0F0` behaves like an action/search/load scale:

- if `0x1003B0F0 <= 1500`:
  - add `1,000,000 / GripMaximisingCoefficient`
- otherwise:
  - add `((0x1003B0F0 + 10000) + (0x1003B0F0 - 1500) * 10) * 100 / GripMaximisingCoefficient`

Multiplicity/repetition penalties:

- field `+0x180` participates in products
- in one mode:
  - add `(+0x180 * +0x180) * 2,000,000`
- in another mode:
  - add `(+0x180^4) * 20,000`

Interpretation:

- the planner strongly discourages repeated/multiplied actions
- grip maximization reduces penalty when good grip is available
- high action/search/load value makes poor grip increasingly expensive

### 14.28 Full scoring model summary

The total candidate score combines:

1. feasibility penalties from `0x1000C3F0`
2. near-collision cubic margin penalties
3. action subtype penalties from `+0x1A4`
4. grip preference terms from `GripMaximisingCoefficient`
5. repeated/multiplied action penalties from `+0x180`
6. direct per-action penalties from `+0x16C` and `+0x198`
7. small sign/position penalties from `+0x118`

The selection rule remains:

- lower total score is better
- hard reject candidates are discarded before comparison
- soft-penalty candidates can still win if no cleaner candidate exists

This is enough to implement a compatible C# approximation:

- represent hard feasibility failures separately from score penalties
- accumulate large fixed penalties for undesirable-but-usable states
- use cubic penalty for near-collision margins
- reduce grip-related score when grip coefficient is favorable
- preserve per-step cumulative score deltas for debugging

### 14.29 Concrete mechanical feasibility rules from `0x1000C3F0`

This section rewrites the `0x1000C3F0` logic as rules that can be reimplemented.

Coordinate convention inferred from calls to exported `xy_to_polar`:

- each profile point uses stride `0x14`
- point `x` is read from point offset `+0x1C`
- point `y` is read from point offset `+0x20`
- point count is read from state offset `+0x08`
- `0x10009800` is exported as `xy_to_polar`
- polar angle is in radians

#### Rule 1: active crossing window

The function first finds an active profile window around the machine reference line.

Observed behavior:

- scan adjacent point pairs
- require both adjacent `y` values to be approximately zero:
  - absolute value compared against `0.001`
- find the pair where `x` crosses the zero/reference line
- set window indexes:
  - start index
  - end index
  - left/right side bounds
- write a window length into state offset `+0x814`

Rule interpretation:

- only the portion of the profile around the current machine reference line is used for the immediate feasibility check
- this matches a real folding-machine rule: the active fold zone is not the entire sheet, it is the segment crossing the clamp/fold reference line

#### Rule 2: orientation mismatch is a hard reject with mirror retry

The earliest hard reject checks fold-side orientation.

Condition shape:

- when the direction check flag is true:
  - convert current fold angle sign to a boolean
  - compare it with `state[+4] XOR state[+5]`
- if it conflicts with the requested retry mode:
  - set code `9`
  - set retry flag output
  - return false

Rule interpretation:

- impossible side/direction combinations are rejected before expensive geometry checks
- because retry flag is set, caller may clone state, mirror it, and try again
- this is the mechanical equivalent of “flip is a state transition, not a drawing correction”

#### Rule 3: backgauge low-clearance penalty

For each point in the active window:

- if `x` is farther than approximately `-1000` past the reference side
- and `y <= BackgaugeHeight`
- then:
  - set code `1`
  - add `1,000,000` to score

Observed constants:

- `0x1003B094 = 1000.0`
- `0x1003B098 = 13.0`

Likely config mapping:

- `BackgaugeHeight = 13.0`

Rule interpretation:

- if a profile point is far behind/under the backgauge low zone, the candidate is still possible but strongly discouraged

#### Rule 4: angular lower-bound hard reject

For points with `y < 0`:

- convert `(x, y)` to polar angle
- check whether the angle falls beyond a lower angular envelope derived from:
  - `smallLimitDeg = 5.0`
  - `pi`
- if it breaches the envelope:
  - set code `2`
  - return false

Observed constants:

- `0x1003B090 = 5.0`
- `0x100332D8 = pi`
- `0x100332D0 = 180`

Rule interpretation:

- the profile cannot rotate below a minimum angular envelope
- this is a hard machine-envelope collision or overtravel rule, not a scoring preference

#### Rule 5: clamp-gap / machine-envelope hard reject

The function computes multiple derived angular clearance limits from:

- `ClampGap`
- clamp and upper-arm geometry
- current profile point angle

Then it tests shifted coordinates such as:

- `x + derivedClearanceA`
- `x + derivedClearanceB`
- `x + derivedClearanceC`

If the polar angle is inside a forbidden clamp-envelope region and `y <= 0`:

- set code `3`
- return false

Important globals:

- `0x1003B078 = 35.0`
  - likely `ClampGap`
- `0x1003B07C = 25.0`
- `0x1003B080 = 165.0`
- `0x1003B084 = 100.0`

Rule interpretation:

- the candidate is rejected when already-formed geometry would enter the clamp or folding-arm envelope
- this is the main collision rule for the clamp/fold reference area

#### Rule 6: overgrip / near-limit soft penalty

For active points:

- if `y <= 10.0`
- then:
  - set code `4`
  - add `2,000,000` to score

Observed constant:

- `0x1003B0A4 = 10.0`

Likely config mapping:

- `MaxOvergripHeight = 10.0`

Rule interpretation:

- overgrip or low-clearance situations are not always impossible
- they are heavily penalized so a cleaner sequence wins when available

#### Rule 7: vertical/height envelope hard reject

The second half of the function checks profile height/position against a maximum envelope.

For points in a constrained horizontal band:

- compare `x` against a small machine-width / arm-width threshold
- compare `y` against `+300` or `-300` depending orientation
- if the point exceeds the hard envelope:
  - set code `6`
  - return false

Observed constants:

- `0x1003B08C = 15.0`
- `0x1003B0A0 = 300.0`
- `0x10033478 = -150.0`

Rule interpretation:

- geometry too near the clamp side and outside the allowed vertical envelope is mechanically impossible

#### Rule 8: opposite-side height/position soft penalties

Similar to code `6`, but under opposite-side or reverse-side conditions:

- if a point is inside the constrained band but near the limit:
  - set code `10`
  - add `1,000,000` to score
- if direction-check mode is enabled and the reverse-side threshold is crossed:
  - set code `8`
  - add `1,000,000` to score

Observed constants:

- `0x1003B0A0 = 300.0`
- `0x1003B0C8 = 250.0`

Rule interpretation:

- not every height/side envelope breach is fatal
- some are accepted as poor candidates and ranked below safer options

#### Rule 9: tracking maximum profile radius and travel length

The function writes derived values back into the state:

- state `+0x81C`
  - maximum radius/distance from origin across active points
- state `+0x820`
  - accumulated active profile length / travel distance
- global `0x100744C0`
  - maximum observed absolute `x`-like metric for one side
- global `0x100744C4`
  - maximum observed absolute `y`-like metric for the other side

Rule interpretation:

- these are not direct pass/fail results
- they are secondary metrics used later for classifying and scoring candidate actions

#### Rule 10: near-collision angular margin penalty

If direct collision checks pass, the function computes a clearance margin:

- compute polar angle of the current point
- compute neighboring polar angles
- compare angular separation from:
  - current point
  - previous point
  - next point
  - cross-candidate geometry via `0x1000BD60`
- convert radians to degrees:
  - `marginDeg = marginRad * 180 / pi`
- ignore tiny margins below `0.1`
- otherwise:
  - set code `7`
  - store `marginDeg` in output margin pointer
  - add `marginDeg^3 * 1000` to score

Rule interpretation:

- near-collision is not binary
- the closer the candidate comes to forbidden geometry, the cost increases cubically
- this strongly prefers sequences with comfortable mechanical clearance

### 14.30 C#-style feasibility skeleton

The following pseudocode captures the current recovered rule structure.

```csharp
FeasibilityResult CheckCandidate(ProfileState state, CandidateContext ctx)
{
    var result = new FeasibilityResult();
    result.Code = 0;
    result.ScorePenalty = 0;
    result.Margin = 0;
    result.RetryMirror = false;

    if (ctx.DirectionCheckEnabled &&
        FoldDirectionConflictsWithState(ctx.FoldAngle, state.SideFlagA, state.SideFlagB, ctx.RetryMode))
    {
        result.Code = 9;
        result.RetryMirror = true;
        result.HardReject = true;
        return result;
    }

    var window = FindReferenceCrossingWindow(state.Points);
    if (!window.Found)
    {
        result.HardReject = true;
        return result;
    }

    foreach (var p in state.Points[window.Start..window.End])
    {
        if (p.X <= -BackstopLimit && p.Y <= BackgaugeHeight)
        {
            result.Code = 1;
            result.ScorePenalty += 1_000_000;
        }

        if (BreachesAngularLowerBound(p))
        {
            result.Code = 2;
            result.HardReject = true;
            return result;
        }

        if (CollidesWithClampEnvelope(p, state.MachineModel))
        {
            result.Code = 3;
            result.HardReject = true;
            return result;
        }

        if (p.Y <= MaxOvergripHeight)
        {
            result.Code = 4;
            result.ScorePenalty += 2_000_000;
        }

        if (BreachesHardHeightEnvelope(p, ctx.Mirrored))
        {
            result.Code = 6;
            result.HardReject = true;
            return result;
        }

        if (BreachesSoftHeightEnvelope(p, ctx.Mirrored))
        {
            result.Code = ctx.DirectionCheckEnabled ? 8 : 10;
            result.ScorePenalty += 1_000_000;
        }
    }

    var margin = ComputeAngularClearanceMargin(state, window, ctx);
    if (margin > 0.1)
    {
        result.Code = 7;
        result.Margin = margin;
        result.ScorePenalty += margin * margin * margin * 1000;
    }

    return result;
}
```

This is not yet exact source code, but it is close enough to guide a C# reimplementation:

- keep hard reject separate from score
- keep retry mirror separate from drawing mirror
- keep every status code visible for debugging
- never collapse all failures into a single boolean

### 14.31 `TestSave\library.txt` dynamic comparison and light disassembly supplement

This pass used the active saved library:

- `E:\_Work\JSZW双向智能折边机\进口数控折弯机资料\TestSave\library.txt`
- `C:\Jing Gong Flashings\TestSave\library.txt`

Both files had the same SHA256 hash:

```text
0DD7B0D736C0993487734082CA2640FEFBAD03851EE18E52FFA5CCC3DC051828
```

This matters because the active runtime config points at:

```text
WorkingDirectory "C:\Jing Gong Flashings\TestSave\"
```

So the `TestSave\library.txt` file in this evidence set is not just a copied sample. It matches the working directory library that the running program is configured to use.

#### A. Controlled runtime observation

`SWIFolder.exe` was started once from the extracted package directory.

Observed window title:

```text
SWI Folder Interface version 4.18.0.4   工作单:    (modified)
```

The process remained responsive. During this short controlled start:

- no automatic rewrite of `library.txt` was observed
- no SHA256 change was observed in either copy of `library.txt`
- no new generated temp evidence was captured from the short run
- the residual `SWIFolder` process was stopped after the observation

Interpretation:

- startup alone does not force fold sequence regeneration
- regeneration is probably tied to an explicit UI action such as job load/edit, auto/semi-auto generation, reset/regenerate, or save
- the saved `SemiAutoList4` records in the library should be treated as persisted outputs from prior generation or operator-save flows, not as startup artifacts

#### B. Library-level statistics

`TestSave\library.txt` contains 34 non-empty job/profile records.

Recovered counts:

- records with `SemiAutoList4`: 34
- records with `SequenceOverride`: 26
- placeholder-like or fallback `SemiAutoList4`: 18
- full generated-like action lists: 12

Fold-count distribution:

```text
2 folds:  2 records
3 folds:  3 records
4 folds:  8 records
5 folds:  5 records
6 folds:  7 records
7 folds:  4 records
8 folds:  2 records
10 folds: 1 record
11 folds: 1 record
34 folds: 1 record
```

The placeholder-like records usually contain a single generic step, for example:

```text
0.00/10.00/0/0/0/1/0.00/0/4.00/0
90.00/50.00/0/0/0/1/0.00/0/4.00/0
90.00/165.00/1/0/1/4/0.00/0/4.00/1
```

These records should not be used as proof of the full sequence algorithm. They are more likely one of:

- incomplete profile save
- failed or skipped automatic generation
- manually saved placeholder action
- profile where only a starting or setup action was persisted

The strongest records for reverse-engineering the generated step list are:

- `20260421`
- `TG01`
- `yankoushibian`
- `20221218`
- `20230101`
- `QD04`
- `QD06`
- `Test`

#### C. `SequenceOverride` includes virtual actions

Several records have more `SequenceOverride` items than physical `Offset` fold points.

Example:

```text
Name "20221218"
physical folds: 6
SequenceOverride: 6/0,5/0,4/0,3/0,2/0,1/0,0/1
SemiAutoList4 steps: 9
```

Example:

```text
Name "dayupeng"
physical folds: 11
SequenceOverride: 11/0,10/0,9/0,8/0,7/0,6/0,5/0,4/0,3/0,2/0,1/0,0/1
```

This implies that `SequenceOverride` is not a plain zero-based list of only physical fold offsets.

Most likely interpretation:

- physical fold points are indexed
- end treatments from `Ends` can become virtual actions
- regrip, flip, or setup actions can also occupy sequence positions
- the second value after `/` stores side/direction/preference state for that sequence item

This matches the earlier binary-level conclusion that the planner carries explicit state for side ownership, mirroring, candidate mode, and retry paths.

#### D. `SemiAutoList4` is an action table, not the raw profile

Each `SemiAutoList4` item has 10 slash-separated fields:

```text
field1/field2/field3/field4/field5/field6/field7/field8/field9/field10
```

Across the 12 full generated-like records, the value distributions were:

```text
field1:  mostly fold/action angle values such as 90, 150, 80, 1, 48, 30, 179
field2:  position/backgauge/action coordinate
field3:  mostly 0, with 1, 2, 3 used for special action families
field4:  0 or 1
field5:  mostly 1, sometimes 0
field6:  almost always 4, with rare 1
field7:  always 0.00 in this library
field8:  mostly 0, with 1 and one observed 3
field9:  usually 4.00, 6.00, or 0.00
field10: mostly 0, with 1 and 2 used on some steps
```

Working interpretation:

| Field | Current interpretation |
| --- | --- |
| 1 | command angle or special action angle |
| 2 | generated machine coordinate, often backgauge/fold position after orientation transform |
| 3 | action family / type |
| 4 | side, direction, or clamping preference bit |
| 5 | enable/active flag, commonly clamp or fold participation |
| 6 | mode family, mostly `4` for normal generated semi-auto actions |
| 7 | reserved or unused in this evidence set |
| 8 | special marker, including `1` and one observed `3` |
| 9 | clamp height, clamp class, or UI/action level, commonly `4` or `6` |
| 10 | result/subtype marker, with `1` and `2` appearing on some generated steps |

This field map is still partial. It is reliable enough to separate raw geometry from generated actions, but not yet enough to drive a PLC sequence without additional validation.

#### E. Position field is transformed by orientation and extra actions

For simple cases, `SemiAutoList4` field 2 can equal a physical fold offset.

Example `20260421`:

```text
physical offsets:
60, 90, 120, 150, 500, 530, 560, 590

generated positions:
560, 530, 500, 120, 90, 60, 350, 60
```

The first six generated positions map directly to physical offsets after ordering/orientation. The extra `350` position is not a physical fold offset, so it is likely a regrip, reposition, or setup action.

For complex profiles, field 2 is often not equal to the nearest original offset.

Example `TG01`:

```text
physical offsets:
30, 95, 115, 185, 385, 469, 484, 544

generated positions:
546, 481, 461, 391, 107, 92, 32, 156
```

Most generated positions are close to a physical fold after compensation and orientation, but not exact. This is consistent with:

- bend allowance / tool compensation
- side mirroring
- press-brake angle display mode
- backgauge coordinate transformation
- end-treatment or clamping offsets

Example `yankoushibian`:

```text
physical offsets:
70, 305, 345, 440, 541, 571, 651

generated positions:
810, 740, 465, 370, 269, 239, 159, 235
```

Here the first generated action is:

```text
888.00/810.00/0/0/1/4/0.00/3/4.00/0
```

The `888.00` angle-like value and field8 value `3` are special markers, not normal bend angles. This confirms that `SemiAutoList4` can store non-fold setup or special-mode actions in the same 10-field record format.

#### F. PE export and disassembly evidence

The algorithm DLL is native 32-bit PE:

```text
FoldingMachineDll.dll
Machine: 0x14c
ImageBase: 0x10000000
EntryPoint RVA: 0x22aee
```

Relevant exports:

```text
PossibleActionList      RVA 0x0ABE44
PossibleFoldList        RVA 0x0ABE5C
generate_fold_sequence  RVA 0x0157F0
polar_to_xy             RVA 0x0098F0
read_model_parameters   RVA 0x019100
write_fold_preferences  RVA 0x0191B0
write_model_parameters  RVA 0x018FE0
xy_to_polar             RVA 0x009800
```

`SWIFolder.exe` imports `generate_fold_sequence`, `PossibleActionList`, and `PossibleFoldList` directly from `FoldingMachineDll.dll`. This confirms the EXE/DLL split:

- EXE owns UI, file parsing, config, save/load, and PLC orchestration
- DLL owns fold candidate generation, geometry transforms, and candidate/action lists

At `generate_fold_sequence` entry:

- the function creates a large stack frame of about `0x3812C` bytes
- it initializes global planner state
- it copies model and config inputs into globals
- it clears/prepares action and fold candidate vectors
- it reads fields from `FOLDLIST_TYPE`

This supports the earlier conclusion that generation is not a simple one-pass transform. It is a high-state search routine with large local scratch buffers.

#### G. Debug-print helper reveals action-record internals

The DLL contains debug strings:

```text
Permutation No %d
 Success
 Fail
  (%f,%f)
r%.1f,
b%.1f,a%.1f)
```

String references point to a block around RVA `0x0000A1E0`.

That block iterates an action array with stride `0xE0` bytes:

```text
index * 0xE0
```

Observed per-action fields in that internal record:

```text
+0x109  byte flag
+0x10A  byte flag
+0x10C  float, printed as r%.1f when non-zero
+0x110  float, printed as b%.1f
+0x118  float, printed as a%.1f
+0x124  int marker, appends a special text marker when non-zero
```

Interpretation:

- the internal `ACTIONLIST_TYPE` is much larger than the serialized 10-field `SemiAutoList4` item
- the serialized list is a compact output contract, not the full candidate record
- internal candidate scoring keeps extra flags, geometry values, and debug values that are not written verbatim to `library.txt`

#### H. Updated end-to-end model

The current best model of the sequence generator is:

```text
library.txt profile
  -> parse Sheetlength / Ends / Offset / Angle / Type / Radius / ClampHt
  -> construct FOLDLIST_TYPE
  -> call generate_fold_sequence(...)
  -> build candidate fold/action pools
  -> enumerate order, side, mirror, grip, and virtual-action choices
  -> score candidate feasibility against machine geometry
  -> preserve or learn operator sequence preference
  -> select output action list
  -> serialize SequenceOverride / ReqstFoldPref / MaxGrip / SemiAutoList4
```

Important practical conclusion:

`SemiAutoList4` should be treated as the closest available saved representation of the executable semi-auto plan. It must not be confused with the raw profile shape. It already includes orientation, virtual actions, and machine-coordinate conversion.

## 15. 与 JSZW 折弯预览规则的对照

Comparison source:

- `E:\_Work\JSZW双向智能折边机\JSZW400\文档\折弯预览生成规则.md`

That document defines several strong rules:

- preview must be generated forward from step table and fold-point state
- A/B left-right ownership must not be inferred from geometry length or visual intuition
- fold up/down controls vertical result, not left-right ownership
- flip is an independent mirror state
- squeeze / colour-side visuals must not drive the geometry rules backward

The SWI binary shows several comparable design choices.

### 15.1 Similar rule: side ownership is explicit state, not geometry intuition

JSZW rule:

- `A-B`: left = `B`, right = `A`
- `B-A`: left = `A`, right = `B`
- do not infer left/right from which side is longer or visually dominant

SWI evidence:

- the planner carries explicit side/orientation flags:
  - state byte around `+0x0C`
  - action flags around `+0x1E79`, `+0x1E7A`
  - mirror flags around `+0x1F28`, `+0x1F29`
- `0x1000B100` selects linkage field `+0x150` from either `+0x154` or `+0x158` based on orientation byte `+5`
- this selection is flag-driven; it is not based on longer branch, average X, or dominant geometry

Extractable SWI-style rule:

- side ownership should be treated as a state-machine flag
- geometry can be transformed after side is known, but geometry should not decide side ownership afterward

### 15.2 Similar rule: fold direction and side ownership are independent

JSZW rule:

- up/down fold direction only controls vertical shape around the fold point
- it must not decide left/right ownership

SWI evidence:

- `0x1000C3F0` receives orientation/state inputs and geometric/fold quantities separately
- it validates candidate feasibility and returns:
  - result/status code
  - two derived geometry values
  - retry flag
- side/orientation bytes are checked independently before deeper machine-geometry checks

Extractable SWI-style rule:

- treat fold direction as one input to feasibility and rendered shape
- do not let it overwrite side ownership
- validation can reject a candidate, but should not silently rewrite the side model

### 15.3 Strong similar rule: flip is an explicit mirror transition

JSZW rule:

- flip is either explicit `SemiAutoActionFlip` or implicit when adjacent inside/outside setting changes
- flip means mirroring already-formed geometry in the machine-left reference frame
- flip must not reverse future fold directions

SWI evidence:

- the branch around `0x1000DD8C .. 0x1000E2DC` explicitly performs a mirrored retry:
  - clone current fold-state
  - run `0x1000B100`
  - run `0x1000C3F0`
  - if validation suggests retry:
    - restore cloned state
    - call `0x1000A110`
    - flip orientation flags
    - rerun geometry and validation
- `0x1000A110` transforms coordinates and toggles an orientation bit
- the retry uses cloned current state, not a global after-the-fact picture correction

Extractable SWI-style rule:

- mirror should be modeled as a state transition with cloned pre-flip state and flipped orientation flags
- mirror should only affect the already-built state snapshot
- subsequent folds should still be generated from their own step parameters

This directly matches the JSZW rule that flip is an independent frame/state, not a convenient visual patch.

### 15.4 Similar rule: preview/state output is paired state, not a loose drawing

JSZW rule:

- `DrawStep = 0`: initial state
- odd steps: before-current-fold state
- even steps: after-current-fold state
- flip-completed frame must not share a generic odd/even mirror patch

SWI evidence:

- every accepted candidate preserves a paired output:
  - action pool snapshot
  - fold-state snapshot
- best result selection copies both:
  - candidate pool to `0x100745F8`
  - paired fold-state to `0x100ABE90`
- recursion uses `0x10019570` / `0x10019A10` to copy state before branch mutation

Extractable SWI-style rule:

- a preview frame should be generated from an explicit state snapshot
- before/after/flip frames should have distinct state transitions
- do not compute a drawing first and then patch it with special-case mirror logic

### 15.5 Similar rule: colour side / squeeze visuals are separate from geometry ownership

JSZW rule:

- squeeze/open-squeeze graphics follow `is色下`
- squeeze graphics must not reverse-drive left/right ownership

SWI evidence:

- config contains separate visual/colour controls:
  - `Initial colour side`
  - `Show C on colour side`
  - `Gap for colour line`
- serialized profiles contain `ReverseColours`
- folding templates and action records carry orientation/colour-like flags separately from the main action type
- squash/swage options are separate feature flags and offsets:
  - `SquashBackstop`
  - `ReverseSquashBackstop`
  - `OpenSquashBackstop`
  - `SwageOffset`
  - `SquashFoldAngle`

Extractable SWI-style rule:

- colour/squeeze/squash display state should be derived from current state flags
- it should not decide or rewrite A/B ownership
- this mirrors the JSZW separation between `is色下` and fold direction / left-right ownership

### 15.6 Rules that can be extracted for JSZW reference

The following SWI-style rules are useful to preserve or borrow conceptually:

1. Keep a paired `action-state + geometry-state` snapshot for every candidate frame.
2. Treat mirror/flip as a formal transition that clones state, flips orientation flags, then regenerates geometry.
3. Keep side ownership in explicit fields, not inferred from drawn geometry.
4. Keep vertical fold direction separate from side ownership.
5. Keep colour/squeeze visual state separate from geometry ownership.
6. Store validation result code and derived geometry values with the candidate action, so preview/debug can explain why a candidate was rejected.
7. When a candidate fails geometry validation, try an explicitly mirrored variant only through the mirror-transition path, not through ad-hoc image correction.

### 15.7 What is still not proven

Not yet proven from current static pass:

- exact name mapping for SWI action type values `+0x124 = 1..7`
- exact business meaning of `0x100744C0` / `0x100744C4`
- whether SWI has a direct equivalent of JSZW `DrawStep` numbering
- whether SWI distinguishes explicit flip from implicit inside/outside-change flip in the same way as JSZW

Still, the extracted structure strongly supports the same design principle:

- generate preview/state forward from step data and explicit orientation flags
- never use the rendered geometry as the source of truth for side ownership or flip correction

### 15.8 PLC tag wrapper resolution from imports

Additional static cross-reference work on `SWIFolder.exe` resolves the two local PLC wrapper helpers back to the imported EtherNet/IP entrypoints.

#### Read wrapper `0x49B990`

Observed behavior:

- it first checks the same comms/busy gates used elsewhere:
  - `0xE59194`
  - `0xE57E03`
- it normalizes the incoming tag name into a temporary buffer at `0xE52DA8`
- it then dispatches to a typed `RequestPLCTagRead...` import based on which destination pointer slot is non-null

Recovered dispatch map:

- char / char-array:
  - scalar: `0x5E5058`
  - indexed/buffered: `0x5E5054`
- long / long-array:
  - scalar: `0x5E5068`
  - indexed/buffered: `0x5E505C`
- float / float-array:
  - scalar: `0x5E5070`
  - indexed/buffered: `0x5E506C`
- short / short-array:
  - scalar: `0x5E5078`
  - indexed/buffered: `0x5E5074`
- one additional destination slot dispatches to:
  - scalar: `0x5E5064`
  - indexed/buffered: `0x5E507C`
  - exact primitive type for this last slot is still not proven from the current static pass

Interpretation:

- `0x49B990` is the general-purpose typed read helper used by UI polling code
- most visible machine status fields are not read directly from imports; they go through this local wrapper first

#### Write wrapper `0x53A5D0`

Observed behavior:

- it performs the same comms/busy gate checks and tag-name normalization as the read wrapper
- it switches on a small local type selector and forwards to the appropriate `RequestPLCTagWrite...` import

Recovered dispatch map:

- case `0`: byte/char write
  - scalar: `0x5E503C`
  - indexed: `0x5E5034`
- case `1`: long write
  - scalar: `0x5E5040`
  - indexed: `0x5E5048`
- case `2`: float write
  - scalar: `0x5E502C`
  - indexed: `0x5E5044`
- case `3`: short/word write
  - scalar: `0x5E5050`
  - indexed: `0x5E5060`

Confirmed command-side tags reaching this wrapper or direct write imports:

- `sa_StartStep`
- `sa_Backgauge`
- `m_FoldOn`
- `MiscControlWord`
- `MiscControlWord2`
- `ManualControlWord`

This confirms the EXE is not only polling PLC status; it is actively issuing machine-control writes through a small local command abstraction.

### 15.9 Observed PLC status read order

The most useful status-poll region currently recovered is the block around:

- `0x5575D4 .. 0x558125`

This is not just one field read. It is a structured polling pass that pulls position tags first, then control words, then status words.

#### A. Position / angle / control-word polling

Recovered order inside this block:

1. `BackgaugePos`
   - tag VA: `0x68B888`
   - read site: `0x5575EF`
   - stored to: `0xE5846C`
2. optional extra backgauge heads, only when `0xE591B4 != 0`
   - `Backgauge2Pos`
     - tag VA: `0x68B878`
     - read site: `0x557663`
     - stored to: `0xE58470`
   - `Backgauge3Pos`
     - tag VA: `0x68B868`
     - read site: `0x557685`
     - stored to: `0xE58474`
   - `Backgauge4Pos`
     - tag VA: `0x68B858`
     - read site: `0x5576A7`
     - stored to: `0xE58478`
3. `AnglePos`
   - tag VA: `0x68B84C`
   - read sites:
     - `0x557769`
     - `0x557805`
   - branch depends on `0xE584A0`
4. `AnglePeak`
   - tag VA: `0x68B840`
   - read site: `0x557896`
   - read through wrapper `0x49B990`
5. `MiscControlWord`
   - tag VA: `0x67DF00`
   - read site: `0x5578DD`
   - stored to: `0xE591A8`
6. optional `MiscControlWord2`
   - tag VA: `0x68B82C`
   - read site: `0x557902`
   - stored to: `0xE591AC`
   - only when the machine/profile enables the extended word path
7. `NoOfOperators`
   - tag VA: `0x68B7EC`
   - read site: `0x55808C`
   - stored to: `0x6E051C`

Interpretation:

- the EXE reads mechanical position first
- then reads the current command/control-word state
- then reads machine-count / operator-count style state

That is consistent with a UI that wants fresh coordinates before deciding what controls and overlays should be enabled.

#### B. Clamp / hydraulic / primary status-word polling

Nearby code continues the same polling family with additional status fields:

8. `ClampHeightPos`
   - tag VA: `0x68B81C`
   - read site: `0x557BDA`
   - stored to: `0xE58484`
9. `HydraulicPressure`
   - tag VA: `0x68B808`
   - read site: `0x557C30`
   - stored to: `0xE58488`
10. `StatusWord1`
    - tag VA: `0x68B7FC`
    - read site: `0x557C6F`
    - stored to: `0xE5848C`
11. `StatusWord2`
    - tag VA: `0x68B7E0`
    - read site: `0x5580BF`
    - stored to: `0xE58490`
12. `StatusWord3`
    - tag VA: `0x68B7D4`
    - read site: `0x5580F9`
    - stored to: `0xE57D78`
    - guarded by extra runtime conditions:
      - `0xE57BF4 > 0`
      - not on the `0xE57D7A` busy/alternate path

#### C. Status decoding is immediate, not deferred

`StatusWord1` is decoded in the same block right after the read:

- bits `0x04 / 0x08 / 0x10 / 0x20`
  - select a 4-state enum into `0xE591F4`
- bit `0x40`
  - becomes boolean `0xE57DC0`
- bit `0x80`
  - drives UI item `0x73A`
- bits `0x100 .. 0x2000`
  - drive UI items `0x73C .. 0x740`
- bit `0x4000`
  - participates in later command cleanup / write-back logic

Important practical result:

- the poller is not passive
- it reads `StatusWord1`, derives UI state immediately, and then may issue follow-up command writes

#### D. Poller-driven command cleanup

One especially important branch around:

- `0x557F9C .. 0x557FBF`

does the following:

- clears bit `0x0800` from `MiscControlWord`
- writes the updated word back through `0x53A5D0`

This means some parts of the "self-reset" behavior are not a one-shot operator command. They are completed by the periodic status poll once the expected machine-state bits change.

### 15.10 Reset / homing chain supplement

The earlier static pass established the presence of both:

- `Reset start fold`
- machine `Home` prompts

The additional code/xref pass now makes the split clearer.

#### A. Start-fold reset path

The `Reset start fold` confirmation is built directly in code around:

- `0x4D70D2 .. 0x4D72A5`

Recovered UI pieces:

- title:
  - `SWI Folder: Confirm start fold reset`
  - direct code reference at `0x4D70D2`
- body:
  - `Do you want to reset start fold to 1?`
  - direct code reference at `0x4D711D`
- remember-choice text:
  - `Remember choice and don't ask for this job`
  - direct code reference at `0x4D719D`

Interpretation:

- this is a dedicated business flow, not a generic message-box wrapper
- the EXE explicitly constructs the confirm dialog and the "remember choice" option for per-job behavior

#### B. Homing path

The homing messages are resource-driven rather than directly inlined in `.text`, but the string set is internally consistent:

- `Do you want to home SWI Folder?`
- `Do you want to home the backgauge?`
- `Do you want to home loading tables and the backgauge?`
- `Folder will be switched to MANUAL mode.`
- `During homing the backgauge and apron home positions will be reset`
- `and user interface controls will be disabled.`
- `Note: You are currently in HEAVY gauge mode. For homing machine will be`
- `temporary switched to LIGHT gauge mode.`

This confirms the intended home sequence:

1. leave current run mode and force `MANUAL`
2. optionally force `LIGHT` gauge mode
3. disable normal UI interaction
4. reset backgauge/apron home positions
5. complete the homing motion

#### C. Homing failure can feed sequence regeneration

In the same message cluster, the EXE also contains:

- `Failed to determine aprons zero position.`
- `Would you like to reset and re-generate fold sequence as well?`

This is strong evidence that:

- apron/backgauge homing failure is not handled only as an alarm
- one recovery path escalates into sequence reset/regeneration

#### D. Profile/config gates that shape the homing path

Current active profile values in `SWIFolder.conf`:

- `Loading tables installed 1`
- `Loading tables enabled 0`
- `PLC has StatusWord3 0`
- `PLC has MiscControlWord2 1`

Combined with the EXE strings:

- `First you need to enable usage of MiscControlWord2 in the config file.`

the most likely interpretation is:

- extended homing / loading-table behavior is gated through `MiscControlWord2`
- the same EXE can run on simpler profiles where that path is absent

So the current best end-to-end reconstruction of the self-reset loop is:

- config/profile decides whether reset is `off / always / ask`
- EXE prompts for `Reset start fold`
- EXE writes command-side tags such as `sa_StartStep`, `sa_Backgauge`, `m_FoldOn`, `MiscControlWord`, `MiscControlWord2`
- poller reads positions and status words in the order listed above
- `StatusWord1` bits are decoded immediately
- some transitions are completed by poller-side write-back cleanup
- homing failure can branch into `reset and re-generate fold sequence`

#### E. Reconstructed automatic reset / homing order

The PLC project files available in this package are binary Rockwell project files (`.ACD` / `.ACD.Recovery`) with no exported ladder/ST listing in this folder. Therefore the exact PLC rung order is not proven from readable PLC source here. The following order is the strongest reconstruction from the HMI binary, PLC tag names, config gates, and the observed status-poll / write-back chain.

| Step | HMI / operator-side action | PLC-side condition observed or implied | Transition to next step | Evidence |
| --- | --- | --- | --- | --- |
| 0 | Load active machine profile and decide whether start-fold reset is disabled, silent, or ask-on-use. | No PLC motion command yet. This is a profile gate before any reset/home command. | If `Reset start fold` is `0`, skip the start-fold reset branch. If `1`, continue silently. If `2`, show the confirm dialog. | `SWIFolder.conf` line `Reset start fold: 2`; code-built confirm dialog at `0x4D70D2 .. 0x4D72A5`. |
| 1 | Build the `Reset start fold` confirmation dialog. | PLC is still not the authority for the dialog decision; this is an HMI business rule. | If the operator cancels, the reset branch stops. If accepted, HMI resets the job start fold to fold `1`. | Dialog title/body/remember-choice xrefs at `0x4D70D2`, `0x4D711D`, `0x4D719D`. |
| 2 | Reset the job sequence start point and, where required, regenerate the fold sequence. | Sequence regeneration is HMI / `FoldingMachineDll.dll` side before machine command writes. | HMI proceeds to write machine command tags only after the local sequence state has been reset/regenerated. | Imported `generate_fold_sequence`; recovered branch text `Would you like to reset and re-generate fold sequence as well?`. |
| 3 | If the reset path requires physical homing, show one of the Home prompts: whole folder, backgauge only, or loading tables plus backgauge. | PLC command path depends on machine features. Loading-table home requires the extended command word path. | If loading-table path is selected but extended word support is not enabled, the EXE blocks with the `MiscControlWord2` config message. Otherwise continue. | Home prompt strings; `Loading tables installed 1`, `Loading tables enabled 0`, `PLC has MiscControlWord2 1`; string `First you need to enable usage of MiscControlWord2 in the config file.` |
| 4 | Force the operating context for homing: switch to `MANUAL`; if currently in heavy gauge, temporarily switch to `LIGHT`. Disable normal UI controls during homing. | PLC is expected to receive manual/light command state through control-word writes rather than normal auto/semi-auto stepping. | Once mode/gauge/UI lock state is prepared, HMI can issue homing-related control-word writes. | Resource strings `Folder will be switched to MANUAL mode.`, `temporary switched to LIGHT gauge mode.`, and `user interface controls will be disabled.` |
| 5 | Write command-side PLC tags through the local write wrapper. Confirmed tags in this command family include `sa_StartStep`, `sa_Backgauge`, `m_FoldOn`, `ManualControlWord`, `MiscControlWord`, and optional `MiscControlWord2`. | PLC begins acting on the written command bits / setpoints. The exact internal rung sequence is inside the binary `.ACD` and is not exported here. | HMI enters the poll loop and waits for positions / status words to reflect the commanded state. | Write wrapper `0x53A5D0`; confirmed command-side tag list in section 15.8. |
| 6 | Poll machine position first: `BackgaugePos`, optional extra backgauge positions, `AnglePos`, `AnglePeak`, then command words. | PLC must publish fresh position/control-word state before the HMI can judge progress. | Continue polling until the relevant position/status bits change. | Poll block `0x5575D4 .. 0x558125`; read wrapper `0x49B990`; status order in section 15.9. |
| 7 | Poll clamp/hydraulic/status: `ClampHeightPos`, `HydraulicPressure`, `StatusWord1`, `StatusWord2`, optional `StatusWord3`. | `StatusWord1` is decoded immediately. Bits `0x04 / 0x08 / 0x10 / 0x20` select a 4-state machine enum; bit `0x4000` participates in later cleanup. | If status bits indicate expected progress/completion, HMI updates UI state and may perform cleanup writes. If not, it remains in the poll/wait path. | `StatusWord1` read at `0x557C6F`; immediate decode described in section 15.9.C. |
| 8 | Perform poller-driven command cleanup. The HMI clears bit `0x0800` from `MiscControlWord` and writes the word back. | This is the observable acknowledgement/cleanup edge: the PLC status has reached a condition that causes HMI to retire the command bit. | After cleanup write-back, the one-shot part of the reset/home command is considered consumed from the HMI side. | Branch `0x557F9C .. 0x557FBF`; write-back through `0x53A5D0`. |
| 9 | Reset stored home positions for backgauge and apron as part of the homing flow. | PLC/home sensors and position feedback must converge enough for the HMI to accept zero/home position. | On success, UI controls can be re-enabled and normal operation resumes. | Home strings `During homing the backgauge and apron home positions will be reset`; setup CSV contains slide home speed/offsets and backgauge calibration parameters. |
| 10 | Failure branch: if the EXE cannot determine apron zero position, it asks whether to reset and regenerate the fold sequence. | PLC/status/position feedback did not let the HMI prove the apron zero point. | If accepted, return to the sequence reset/regeneration path; otherwise remain in a fault/operator recovery path. | Strings `Failed to determine aprons zero position.` and `Would you like to reset and re-generate fold sequence as well?`. |

Practical order summary:

1. Profile gate (`Reset start fold`, loading-table and extended-word support).
2. Optional HMI confirmation.
3. Local sequence reset/regeneration.
4. Homing prompt selection.
5. Forced `MANUAL` / optional `LIGHT` / UI lock.
6. PLC command writes through `0x53A5D0`.
7. Position and command-word polling through `0x49B990`.
8. `StatusWord1/2/3` polling and immediate bit decode.
9. HMI acknowledgement cleanup by clearing `MiscControlWord` bit `0x0800`.
10. Success resumes normal UI; zero-position failure can loop back into reset/regeneration.

The important control conclusion is that the visible automatic reset is not a single PLC-only sequence. It is an HMI-orchestrated loop: HMI decides the reset/home branch, writes PLC command tags, waits on PLC-published status/position tags, and then performs a poller-driven write-back cleanup when the expected status edge appears.

### 15.11 Reset / homing design patterns worth borrowing

This pass looked specifically for reusable control ideas, not only for SWI-specific addresses. The strongest patterns worth borrowing into a JSZW-style PLC/HMI reset design are below.

| Pattern | SWI evidence | Why it is useful | JSZW-style borrowing rule |
| --- | --- | --- | --- |
| Make reset policy configurable | `Reset start fold: 2` is parsed from config through the `Reset start fold: ` key around `0x4AC00D`; the config writer formats it through `Reset start fold: %i (0 = don't reset, 1 = always reset, 2 = ask)` around `0x4B1300`. | The same reset function can support disabled / always / ask without changing PLC code or HMI screens. | Use an explicit reset policy enum, not a hidden boolean. Suggested values: `Disabled`, `Always`, `AskOperator`. |
| Separate job/sequence reset from mechanical homing | `Reset start fold` has its own code-built dialog at `0x4D70D2 .. 0x4D72A5`; machine Home prompts are resource-driven and separate. | Resetting the program step is a business-state operation; homing axes is a machine-motion operation. Mixing them makes recovery hard to explain. | Keep `reset current job/start step` and `home machine/axis` as separate states, even if one workflow can call both. |
| Gate optional hardware with capability flags | Active config separates `Loading tables installed`, `Loading tables enabled`, `PLC has StatusWord3`, and `PLC has MiscControlWord2`. The EXE also warns when `MiscControlWord2` is not enabled. | Prevents HMI from issuing commands the installed PLC program cannot consume. | Add PLC/HMI capability flags for optional units instead of exposing every reset button on every machine. |
| Force a safe operating context before homing | Home resource strings state that the machine switches to `MANUAL`, may switch to `LIGHT`, disables UI controls, and resets backgauge/apron home positions. Nearby strings also mention light-curtain clearance and backgauge lock behavior. | Homing is treated as a controlled recovery mode, not a normal auto-cycle continuation. | Before whole-machine reset, explicitly enter manual/reset mode, block normal operation buttons, and require safety/interlock state to be true. |
| Keep command words layered by responsibility | The EXE uses distinct tag families: `ManualControlWord`, `SemiAutoControlWord`, `AutoControlWord`, `PulsedControlWord`, `MiscControlWord`, and optional `MiscControlWord2`. Cross-references show separate read/write use, for example `ManualControlWord` reads around `0x55A976`, `SemiAutoControlWord` around `0x55A995`, `AutoControlWord` around `0x55A9B7`, `MiscControlWord` around `0x55A9D6`, and `MiscControlWord2` around `0x55AA01`. | It avoids one overloaded "reset word" becoming an untraceable mixture of jog, auto, pulse, and optional hardware commands. | Split JSZW command channels into normal mode commands, manual/reset commands, transient pulse commands, and optional-unit commands. |
| Use one-shot command bits with explicit cleanup | `MiscControlWord2` bit `0` is set at `0x56746D .. 0x567482` and cleared at `0x5675B3 .. 0x5675C9`; `MiscControlWord` bit `0x0800` is cleared at `0x557F9C .. 0x557FBF`; another branch clears `0x3000` from `MiscControlWord` at `0x5675D2 .. 0x5675E8`. | Commands are not left latched forever; the HMI retires command bits after the observed state edge or user branch. | For JSZW whole reset, use request/ack/done or command/status pairs. Do not rely on an HMI button bit staying high as the state machine input. |
| Poll in a deterministic order and decode immediately | The main poll path reads positions/control words first, then `ClampHeightPos`, `HydraulicPressure`, `StatusWord1`, `StatusWord2`, optional `StatusWord3`; `StatusWord1` is decoded in the same block. | The HMI acts on a coherent snapshot instead of mixing stale position with fresh status bits. | Publish a compact PLC reset status code and let HMI render it directly. Keep position/status refresh order stable. |
| Treat zero/home failure as a recoverable branch | Resource strings include `Failed to determine aprons zero position.` followed by `Would you like to reset and re-generate fold sequence as well?`. | A failed home is not hidden as a generic alarm; the operator gets a recovery path that can repair dependent sequence state. | Add explicit reset failure states such as `CannotFindHome`, `ZeroNotReached`, `InterlockMissing`, and route each to a defined operator action. |
| Externalize homing parameters | `Folder data values.csv` maps home speed/offset and related behavior to setup words: `ApronSlideHomeSpeed` -> `SetupWord[12]`, `TopSlideHomeOffset` -> `SetupWord[13]`, `BottomSlideHomeOffset` -> `SetupWord[14]`, and `B_ApronHomeAlways` -> `SetupWord[79]`. | Homing behavior can be tuned without recompiling the HMI. | Keep reset speeds, offsets, and "home every cycle" policy in PLC parameters with HMI display/edit rules, not hardcoded in button handlers. |

The most transferable idea is the architecture, not the raw bit numbers. SWI's exact tags (`MiscControlWord`, `MiscControlWord2`, `StatusWord1`, etc.) belong to its Rockwell/EtherNet-IP program. For JSZW400, the safer borrowing target is:

1. PLC owns the reset state machine and publishes an authoritative reset status.
2. HMI owns operator confirmation, UI lockout display, and command request pulses.
3. HMI command bits are one-shot or handshake-backed, then cleared by ack/done logic.
4. Optional mechanisms are gated by capability flags before commands are exposed.
5. Failure states name the missing condition and offer a specific recovery branch.

## 16. Evidence basis

This report is based primarily on static extraction from files already present in this folder, plus one controlled startup observation of `SWIFolder.exe`:

- binary type inspection
- PE import/export parsing
- PE export RVA and light disassembly around key fold-planning blocks
- managed IL inspection for `ifjFileHandler.exe`
- config and CSV reading
- string extraction from binaries and SQL backup
- language/resource DLL string extraction for homing and failure prompts
- saved-library comparison against `TestSave\library.txt`
- runtime observation that startup alone did not rewrite the active `library.txt`

No original binary was modified.
