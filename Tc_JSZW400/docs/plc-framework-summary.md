# PLC Framework Summary

## Project Root

- TwinCAT PLC project: `JSZW/JSZW400.tsproj`
- PLC source root: `JSZW/Untitled1`

## Top-Level Execution Flow

1. `POUs/MAIN.TcPOU`
   - Calls `TaskInfo()`, `Boot()`, `ModeSw()`.
   - Checks PackML command legality in `nStateCommandCheck`.
   - Sends mode/state commands into `fbMachine`.
   - Calls visualization and persistent write helpers.
2. `POUs/FB_Generic.TcPOU`
   - Base FB for machine/unit state handling.
   - Runs PackML mode change state machine through `fbUnitModeManager`.
   - Runs PackML state machine through `fbStateMachine`.
   - Dispatches logic by current `eState`, especially `M_Execute()`.
3. `POUs/fbMachine.TcPOU`
   - Extends `FB_Generic`.
   - Instantiates and coordinates the major machine units:
     - `fbApron`
     - `fbBackGauge`
     - `fbClamp`
     - `fbSlitter`
   - Publishes subunit status into `fbStateMachine.stSubUnitInfoRef`.
   - Defines available unit modes (`Auto`, `SemiAuto`, `SlitterAuto`).
   - In `M_Execute()`, dispatches by unit mode to high-level actions.

## Shared Data Model

### Globals

- `GVLs/GVL.TcGVL`
  - `PackTags`: PackML command and status exchange.
  - `Hmi_bArray`, `Hmi_iArray`, `Hmi_rArray`: HMI mapped memory.
  - `stFolder`: runtime machine state snapshot.
  - `stFolderPara`: persistent machine parameters.
- `GVLs/GVL_Com.TcGVL`
  - `stFolderCom`: cross-unit task and parameter command bus.
- `GVLs/GVL_IO.TcGVL`
  - Physical input/output and analog mappings.

### DUTs

- `DUTs/DUTs/ST_Folder.TcDUT`
  - Runtime process values and current status container.
- `DUTs/DUTs/ST_FolderStatus.TcDUT`
  - Boolean machine position/interlock status.
- `DUTs/DUTs/ST_FolderPara.TcDUT`
  - Persistent parameters and setup values.
- `DUTs/DUTs/ST_FolderCom.TcDUT`
  - Per-unit task requests and task parameters.
- `DUTs/DUTs/E_UnitTask.TcDUT`
  - Enumerates unit task IDs such as clamp, backgauge, apron, slitter, table, feed.

## Unit Layer

### `POUs/UNIT/fb_Apron/fb_Apron.TcPOU`

- Reads analog feedback for top/bottom apron slide and fold.
- Updates `stFolder` runtime values and `stFolder.Status.*`.
- Directly gates apron outputs with interlocks:
  - slitter home
  - fold/slide home conditions
  - clamp position conditions
- Main execution split:
  - `A_ManualRun`
  - `A_SemiAuto`
  - `A_AutoRun`
  - `M_Execute`

### `POUs/UNIT/fb_BackGauge/fb_BackGauge.TcPOU`

- Owns axes:
  - `Axis_T1`..`Axis_T4`
  - `Axis_Table`
  - `Axis_Feed`
- Maintains backgauge, table, feed actual position/speed in `stFolder`.
- Handles gripper, suction cup, flip cup, gear-in/out logic.
- Uses helper FBs:
  - `FB_BGAbsPos`
  - `FB_TblAbsPos`
  - `FB_FeedAbsPos`
- Main execution split:
  - `M01_Axis`
  - `GripperMan`
  - `GearInOut`
  - `A_ManualRun`
  - `A_SemiAuto`
  - `A_AutoRun`
  - `M_Execute`

### `POUs/UNIT/fb_Clamp/fb_Clamp.TcPOU`

- Reads clamp pressure and clamp height.
- Updates clamp status bits in `stFolder.Status`.
- Uses helper FBs:
  - `FB_ClampLock`
  - `FB_ClampUnlock`
  - `FB_ClampJog`
  - `FB_ClampMove`
- Main task entry is `stFolderCom.TaskClamp`.
- `A_SemiAuto` processes lock/unlock/jog task requests and clears task when done.

### `POUs/UNIT/fb_Slitter/fb_Slitter.TcPOU`

- Updates `stFolder.Status.Slitter_AtHome`.
- Uses `FB_SlitterAct` for forward/backward movement.
- Slitter outputs are interlocked by bottom apron position/home conditions.
- Main task entry is `stFolderCom.TaskSlitter`.

## Command And Task Flow

1. HMI or PackML command writes into `PackTags.Command` and HMI arrays.
2. `MAIN` validates whether the PackML command is legal in the current state.
3. `fbMachine` receives mode command and state command.
4. `FB_Generic` resolves mode/state transitions.
5. Machine-level action logic writes unit tasks into `stFolderCom`.
6. Unit FBs consume `stFolderCom.Task*` plus `para*` fields.
7. Unit FBs drive outputs and refresh `stFolder` / `stFolder.Status`.

## Modification Entry Points

- PackML command behavior:
  - `POUs/MAIN.TcPOU`
  - `POUs/FB_Generic.TcPOU`
- Machine-level process sequence:
  - `POUs/fbMachine.TcPOU`
- Apron logic:
  - `POUs/UNIT/fb_Apron/fb_Apron.TcPOU`
- Backgauge, table, feed, gripper:
  - `POUs/UNIT/fb_BackGauge/fb_BackGauge.TcPOU`
- Clamp logic:
  - `POUs/UNIT/fb_Clamp/fb_Clamp.TcPOU`
- Slitter logic:
  - `POUs/UNIT/fb_Slitter/fb_Slitter.TcPOU`
- Global runtime data:
  - `DUTs/DUTs/ST_Folder.TcDUT`
  - `DUTs/DUTs/ST_FolderStatus.TcDUT`
- Persistent parameters:
  - `DUTs/DUTs/ST_FolderPara.TcDUT`
- Unit task definitions or new task channels:
  - `DUTs/DUTs/E_UnitTask.TcDUT`
  - `DUTs/DUTs/ST_FolderCom.TcDUT`
- New field IO mapping:
  - `GVLs/GVL_IO.TcGVL`

## Notes For Follow-Up Changes

- Unit FBs generally inherit `FB_Generic` and call `SUPER^()` first.
- Most cross-unit coordination is not done by direct FB calls but through:
  - `stFolder.Status.*`
  - `stFolderCom.Task*`
  - `stFolderCom.para*`
- Safety/interlock conditions are coded inline at each output assignment and should be preserved when changing behavior.

## Field-Verified Proportional Valve Mapping

- This section reflects the current field-verified wiring and output behavior.
- The authoritative mapping is the TwinCAT project link table in `JSZW/JSZW400.tsproj`, not older `AQ*` comments.
- EL4024 mappings currently in use:
  - `Term 41` channel 1 -> `GVL_IO.ANALOG_out_BtmApronFold`
  - `Term 41` channel 2 -> `GVL_IO.ANALOG_out_BtmApronSlide`
  - `Term 42` channel 1 -> `GVL_IO.ANALOG_out_ClampDown`
  - `Term 42` channel 2 -> `GVL_IO.ANALOG_out_ClampUp`
  - `Term 42` channel 3 -> `GVL_IO.ANALOG_out_TopApronFold`
  - `Term 42` channel 4 -> `GVL_IO.ANALOG_out_TopApronSlide`
- Related files:
  - `JSZW/Untitled1/GVLs/GVL_IO.TcGVL`
  - `JSZW/Untitled1/POUs/fbMachine.TcPOU`
  - `JSZW/JSZW400.tsproj`
