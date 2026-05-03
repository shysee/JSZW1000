# JSZW400 Folding Workflow

This context defines the business language for JSZW400 HMI-side order generation, fold-plan preview, and semi-auto execution handoff. It exists so fold-generation, preview, save, and send actions use one consistent meaning for "plan", "candidate", and "selected result".

## Language

**Order**:
The current workpiece definition, including geometry, process settings, and the resulting semi-auto steps.
_Avoid_: Job file, worksheet

**Fold candidate plan**:
One mechanically feasible multi-step fold scheme generated from the current order.
_Avoid_: Temporary sequence, preview-only sequence

**Selected fold candidate plan**:
The fold candidate plan currently chosen by the operator in the fold preview page.
_Avoid_: Display-only plan, alternate preview

**Formal semi-auto plan**:
The selected fold candidate plan after it becomes the order's official semi-auto step list.
_Avoid_: Recomputed default plan, hidden best plan

**Saved order geometry**:
The persisted fold geometry and process input needed to regenerate candidate plans when an order is reopened.
_Avoid_: Saved candidate set, cached alternative plans

**Custom formal semi-auto plan**:
The formal semi-auto plan after an operator manually changes step order or step parameters.
_Avoid_: Still-a-candidate plan, auto-generated alternative

**Plan origin marker**:
The saved marker that records whether the current formal semi-auto plan came from candidate selection or from later manual edits.
_Avoid_: Full candidate history, hidden UI-only flag

**Plan selection**:
The in-memory act of choosing which fold candidate plan is the current formal semi-auto plan.
_Avoid_: Send to PLC, save to file

**Selected plan continuity**:
The rule that the current formal semi-auto plan remains active across page switches until generation inputs change or plans are explicitly regenerated.
_Avoid_: Page-local preview state, throwaway selection

**Saved plan reopening**:
The rule that reopening an order restores the saved formal semi-auto plan first, without automatically regenerating candidate plans.
_Avoid_: Auto-regenerated default plan on open

**Candidate exploration axis**:
An option space the generator should explore across candidate plans, even when the operator did not explicitly change it in the current session.
_Avoid_: Regeneration boundary, saved geometry input

**Regeneration boundary**:
The set of changes that invalidate the current formal semi-auto plan and require candidate plans to be regenerated.
_Avoid_: Candidate exploration axis, page switch

**Preview generation input**:
An input that is allowed to affect candidate generation in the fold preview workflow.
_Avoid_: Production-mode option, unrelated process option

**Execution plan source**:
The single formal semi-auto plan that both semi-auto send and auto execution must consume.
_Avoid_: Mode-specific plan, separate auto plan

**Execution step composition**:
The rule that production-mode steps such as inline slitting or taper-related execution details are appended only when building the final execution step list for send/auto, not while generating preview candidates.
_Avoid_: Preview candidate pollution, mixed preview/production step list

**Reset view action**:
The fold-preview action that returns to the first-ranked candidate in the current feasible set, or regenerates candidates when the current formal plan has already become invalid.
_Avoid_: Pure camera reset, display-only reset

**Confirmed sent plan**:
The formal semi-auto plan after it has been sent and confirmed as the current execution plan.
_Avoid_: Unsent selection, temporary preview plan

**Preview preference**:
A user preference that changes which feasible candidate plan is ranked first, without removing the other feasible plans from preview cycling.
_Avoid_: Hard filter, regeneration boundary

**Fold layout function**:
The workflow for arranging and editing the formal semi-auto step list, including step order and step-level parameters.
_Avoid_: Candidate browsing, preview-only choice

**Layout confirmation validation**:
The feasibility validation performed when the operator confirms edits in the layout/settings page before the edited plan becomes the accepted formal semi-auto plan.
_Avoid_: Blind save, unchecked manual plan

**Fold direction**:
The machine-action direction of a fold step, expressed as up-fold or down-fold.
_Avoid_: Rotating side, left/right ownership

**Rotating side**:
The active folding end shown on the right side of the preview screen. Fold-direction rendering is always applied to this screen-right active end.
_Avoid_: Geometry-guessed side, alternating screen-side activity

**Right-side fold rotation rule**:
The rule that every true fold step rotates only the current screen-right active branch after the workpiece has already been placed into the correct A/B and flip state. Heuristics such as backgauge comparison, longer-side guesses, or flat-side detection must not choose the rotating side.
_Avoid_: `IsFlatSideLeft`-style rotation-side guessing, backgauge-driven side selection, left-right branch ambiguity during a fold

**A-B / B-A side mode**:
The explicit side-state flag that decides which branch is outside and which branch is inside for the current step.
When `A` is outside, `B` is displayed on the left and `A` is displayed on the right.
When `B` is outside, `A` is displayed on the left and `B` is displayed on the right.
_Avoid_: Treating A/B as a direct left/right activity flag

**A-B side-mode non-directionality**:
The rule that `A-B / B-A` side mode does not directly decide whether a fold is visually shown as up or down within one continuous no-flip segment. It affects outside/inside ownership and flip requirements, but screen-visible up/down still comes from board-face state plus the active direction-sign source.
_Avoid_: Using A/B as a second up-down switch, mixing side ownership with fold visual direction

**Flip reference centerline**:
The fixed machine preview centerline used as the `180` degree reference when already-formed geometry is flipped between A/B side modes.
_Avoid_: Board geometric center, active-end tip, floating pivot

**Flip pivot independence**:
The rule that preview `FLIP` transitions do not reuse the previous fold step's `坐标序号` as their geometric rotation pivot. True fold steps rotate around their own fold-geometry pivot, while `FLIP` transitions use the fixed machine preview reference centerline instead.
_Avoid_: Reusing the last fold pivot for flip, treating fold-step pivot and flip pivot as one rule

**Preview flip transition**:
The explicit preview-only transition segment shown as `FLIP` when side-mode ownership changes between adjacent steps, even if the persisted step table does not yet contain a dedicated flip action row.
_Avoid_: Silent side swap, merged fold-and-flip frame

**Flip requirement**:
The rule that a change in `A-B / B-A` side mode means the workpiece must be flipped before the next fold can continue in the correct machine reference frame.
_Avoid_: Optional style-only change, silent side reassignment

**Automatic flip step**:
An explicit `FLIP` action row inserted into the formal semi-auto step table so the machine or automatic flow performs the flip as part of the executable plan.
_Avoid_: Preview-only marker, manual reminder text

**Manual flip prompt**:
The operator-facing prompt shown when a **Flip requirement** exists but the formal semi-auto step table does not yet include an **Automatic flip step**.
_Avoid_: Executable machine action, hidden inferred flip

**Animation-driving preview setting**:
A preview setting that must change the rendered animation effect itself, not only the text explanation. Current confirmed examples are grip type and color side.
_Avoid_: Text-only metadata, explanation-only flag

**Color-side placement effect**:
The rule that color-side selection changes the board's initial placement/orientation in preview, so the same geometric fold can appear as opposite visual up/down results on screen depending on the initial color-side placement.
_Avoid_: Treating color side as a text-only skin option, using absolute target angle size alone

**Angle-sign driven preview direction**:
The rule that the screen-visible up/down result is determined from the initial color-side placement plus the source fold-angle sign, not from the absolute target angle magnitude alone.
_Avoid_: Using `90.0` vs `45.0` magnitude as visual direction truth

**Preview direction source of truth**:
The rule that preview should trust the source board-angle sign first when deciding screen-visible up/down. If the stored fold-direction field conflicts with that sign, the field is considered wrong and should be corrected in generation/mapping.
_Avoid_: Following a stale fold-direction field, drawing the preview to match a known-wrong field

**Preview visual direction truth table**:
The explicit mapping from initial color-side placement plus angle sign into the screen-visible up/down result.
`ColorUp + PositiveAngle => Up`
`ColorUp + NegativeAngle => Down`
`ColorDown + PositiveAngle => Down`
`ColorDown + NegativeAngle => Up`
_Avoid_: Ad-hoc sign inversions, magnitude-only direction logic

**Preview direction sign source**:
The source that provides the angle sign used by the preview visual-direction truth table.
Generated candidate plans use the original board-angle sign.
Custom manual plans use the operator-edited direction result instead.
_Avoid_: Mixing generated and manual sign sources in one timeline

**Preview direction sign applicability**:
The rule that only true fold steps with a valid source board-angle mapping should use the original board-angle sign as preview direction truth. Auxiliary execution steps such as `FLIP`, `Squash`, or `OpenSquash` do not claim a source board-angle sign and must use their own execution semantics instead.
_Avoid_: Forcing every helper step through the board-angle truth table, inventing source-angle signs for non-fold actions

**Source-angle sign mapping field**:
The rule that for true fold steps in generated plans, `长角序号` is the canonical field used to map back to `CurtOrder.lengAngle[idx].Angle` when preview needs the original board-angle sign. If that mapping is not valid, preview must not fall back to geometry guessing.
_Avoid_: Reconstructing source-angle sign from current geometry, mixing pivot indices and source-angle identity

**Fold-geometry pivot field**:
The rule that `坐标序号` is the canonical field for the geometric fold pivot of a true fold step. It identifies which node or bend point the current step rotates around, and it does not serve as the source-angle sign identity.
_Avoid_: Using source-angle identity as pivot, deriving pivot from long-angle mapping, mixing geometric anchor and direction truth

**Operator-facing preview direction text**:
The main up/down wording shown to the operator in preview must match the final screen-visible preview result, not blindly echo the raw stored fold-direction field. Engineering details about source sign or manual override may be shown separately, but the primary text must stay aligned with the animation truth.
_Avoid_: Left text says up while preview animates down, treating raw field values as operator truth

**Generated preview direction consistency**:
The rule that auto-generated candidate plans must keep fold-direction fields consistent with color-side placement plus the source board-angle sign.
_Avoid_: Generated direction drift, candidate-specific silent overrides

**Generated direction normalization layer**:
The rule that when an auto-generated fold step has a valid source board-angle mapping, any direction correction must happen in the generation or mapping layer so preview, operator text, settings, save, and send all consume one normalized fold-direction value.
_Avoid_: Preview-only correction, multiple conflicting direction truths across UI and execution

**Generated formal direction field**:
The rule that in an auto-generated formal plan, `step.折弯方向` itself must store the normalized final direction value. There is no separate long-lived hidden preview direction for generated plans; preview and execution read the same formal direction field after normalization.
_Avoid_: Dual-track generated directions, keeping a known-wrong stored direction beside a corrected preview-only direction

**Manual direction override**:
The rule that an operator-edited custom formal semi-auto plan may intentionally override the generated fold direction, even when that makes preview geometry diverge from the original board-angle interpretation.
_Avoid_: Pretending custom override is still source-derived, silent inconsistency

**Manual preview authority**:
The rule that once a plan has become a custom formal semi-auto plan, preview must be driven by the manually set fold-point data for each step.
_Avoid_: Falling back to generated geometry truth mid-sequence, partial manual authority

**Implicit preview flip insertion**:
The rule that preview inserts a `FLIP` transition whenever adjacent steps switch A/B side mode, even if the formal step table has no explicit flip row.
_Avoid_: Silent side swap, missing flip transition in preview timeline

**Explicit flip execution semantics**:
The rule that an explicit `FLIP` row marks the existing flip requirement as an automatically executed machine step, rather than adding a second geometric flip.
_Avoid_: Double flip geometry, duplicated flip transition

**Current board-face orientation state**:
The running preview state that determines which board face/orientation is currently presented to the machine reference frame. It is initialized from color-side placement and toggled by each flip transition.
_Avoid_: Re-reading raw color-side as a per-step direction switch

**Fold-point centered preview geometry**:
The rule that fold-preview board geometry is distributed from the current fold point against the machine-centered preview reference, rather than being repositioned by backgauge value.
_Avoid_: Backgauge-driven board geometry, text-only fold center

**Backgauge display value**:
The step-level backgauge setting used for operator explanation and later machine execution coordination, without becoming the anchor for fold-preview board geometry.
_Avoid_: Fold-geometry center, board-position owner

**Backgauge feed translation segment**:
The preview segment in which the whole board is translated by backgauge feed so the current fold pivot is brought into the machine bending position. This translation prepares the next fold, but it does not redefine the canonical fold-geometry pivot rule.
_Avoid_: Treating backgauge as text-only, letting backgauge replace the formal fold pivot, skipping board translation when the machine should feed to the next bend

**Backgauge feed rigid translation**:
The rule that the backgauge feed segment is a rigid-body translation of the current board state along the machine feed axis. It does not rotate the board, deform the geometry, or preview the next fold early; it only moves the current fold pivot into bending position.
_Avoid_: Sneaking fold rotation into feed motion, diagonal composite motion by default, shape deformation during feed

**Preview transition ordering**:
The rule that when a step requires multiple transition segments, preview resolves them in execution order: `FLIP` first if required, then `REGRIP` or other grip-transition handling, then the backgauge feed translation that brings the board to the next fold pivot, and only then the actual fold motion.
_Avoid_: Feeding before a required flip, translating before the grip state is settled, mixing transition order ad hoc per step

**Springback compensation**:
The execution-time angle compensation value used to explain why the machine may overbend or underbend relative to the target angle, while the preview final geometry still represents the target formed angle.
_Avoid_: Alternate final geometry, mandatory compensation animation

**Grip-transition preview segment**:
The preview-only transition segment that represents grip-state changes such as transitional grip or regrip before the next fold animation continues, without simulating the full gripper motion path.
_Avoid_: Hidden grip-state jump, full gripper kinematics by default

## Relationships

- An **Order** can generate one or more **Fold candidate plans**
- Exactly one **Selected fold candidate plan** may exist for an **Order** at a time
- The **Formal semi-auto plan** for an **Order** is the current **Selected fold candidate plan**
- Saving an **Order** persists the **Formal semi-auto plan** and the **Saved order geometry**, but not the other unselected **Fold candidate plans**
- Once an operator manually edits the **Formal semi-auto plan**, it becomes a **Custom formal semi-auto plan** and candidate cycling is no longer valid until plans are regenerated
- Saving an **Order** also persists a **Plan origin marker** for the current formal semi-auto plan
- **Plan selection** immediately updates the in-memory **Formal semi-auto plan**, but sending to PLC and saving the order remain separate explicit actions
- **Selected plan continuity** means leaving and re-entering the fold preview page does not reset the current formal semi-auto plan while generation inputs stay unchanged
- **Saved plan reopening** means reopening an order shows the saved formal semi-auto plan first; candidate plans are regenerated only by an explicit regenerate action
- **Candidate exploration axes** include reverse order and color side preferences; the generator may produce alternatives across them even when the operator did not explicitly change them
- The **Regeneration boundary** currently includes board geometry changes and fold-order changes, but excludes reverse-order and color-side preference changes
- **Preview generation inputs** currently exclude inline-slitting and taper options; those are production-mode options and should not affect fold-preview candidate generation
- Reverse order and color side act as **Preview preferences**: they influence the default first-ranked plan, but do not filter out the other feasible candidate plans
- Changing reverse order or color side in fold preview reorders the current feasible candidate set and immediately switches the formal semi-auto plan to the new first-ranked candidate, without triggering full regeneration
- The **Execution plan source** is the current **Formal semi-auto plan** for both semi-auto send and auto execution
- The **Execution step composition** starts from the formal fold plan and appends production-mode steps only at send/auto time
- The **Fold layout function** belongs to the layout/settings page and edits the current formal semi-auto plan directly
- The **Layout confirmation validation** must run when the operator confirms layout edits, so the edited plan is checked for feasibility before it is accepted
- The **Fold layout function** only edits the pure fold portion of the formal semi-auto plan; production-mode execution steps appended at send/auto time are not shown or edited there
- After a successful layout confirmation, the workflow returns to fold preview and shows the newly accepted formal semi-auto plan
- The reset action inside the layout page only discards unconfirmed layout edits and restores the formal semi-auto plan that was active when the page was opened
- The **Reset view action** is the preview-page recovery entry: it returns to the first-ranked candidate of the current feasible set, or regenerates candidates when the current plan is invalid
- When a preview preference is active, the **Reset view action** returns to the first-ranked candidate under the current preference, not to the originally opened plan or the last saved plan
- Sending and saving remain separate actions, but the normal operator sequence is to send the current formal plan first and save the order afterward
- Sending turns the current **Formal semi-auto plan** into the **Confirmed sent plan**
- After a **Confirmed sent plan** exists, alternative candidate browsing is closed; the operator must use the **Reset view action** before previewing the next candidate again
- The **Reset view action** regenerates a fresh feasible candidate set from the current board geometry plus the current preview preferences
- If the current formal plan has become a **Custom formal semi-auto plan**, the **Reset view action** must ask for confirmation before discarding manual edits and regenerating candidates
- If regeneration under the current geometry produces no feasible candidate plans, the current formal semi-auto plan becomes invalid and must not remain sendable
- **Fold direction** and **Rotating side** are separate concepts: fold direction only describes the machine motion as up/down, while the preview always applies that motion to the screen-right active end
- **A-B / B-A side mode** decides outside/inside branch ownership, not whether the active end moves to the left side of the screen
- When **A-B / B-A side mode** swaps, preview must first rotate the already-formed shape `180` degrees around the **Flip reference centerline**, then continue folding on the screen-right active end under the new outside/inside assignment
- A change in **A-B / B-A side mode** between adjacent steps is enough to trigger a **Preview flip transition**, and that transition should be shown to the operator as `FLIP` in preview even before the project decides whether flip rows should be auto-inserted into the formal step table
- A change in **A-B / B-A side mode** creates a **Flip requirement**
- A **Flip requirement** may later be realized either as an **Automatic flip step** in the formal semi-auto plan or as a **Manual flip prompt**, depending on board-shape rules and execution-mode decisions
- Even when the formal step table does not yet contain an **Automatic flip step**, preview must still represent the **Flip requirement** explicitly so subsequent folds are interpreted in the correct post-flip reference frame
- **Animation-driving preview settings** must affect preview rendering directly; they must not be treated as text-only explanation fields
- **Backgauge position** does not decide fold-preview board geometry; the board still folds from the current fold point in the machine-centered preview reference
- **Backgauge display value** remains part of step explanation and machine-side coordination, but it does not move the fold-preview board geometry away from the current fold point
- **Springback compensation** explains machine execution compensation, while preview final geometry still shows the target formed angle rather than a separate compensation end state
- **Grip type** affects preview timing and transition segmentation; transitional grip and regrip should appear as explicit **Grip-transition preview segments**, even if full gripper motion is not yet animated
- **Color-side placement effect** means color-side selection can invert the visual up/down result of the same step on screen
- **Angle-sign driven preview direction** means visible up/down is derived from initial color-side placement plus the source fold-angle sign; the absolute target angle magnitude is not sufficient to decide screen direction
- **Preview direction source of truth** means preview follows the source board-angle sign first; conflicting fold-direction fields should be treated as generation/mapping defects, not as reasons to draw the wrong preview
- **Preview visual direction truth table** is fixed: `ColorDown` flips the screen-visible up/down result of the same angle sign compared with `ColorUp`
- **Preview direction sign source** depends on plan type: generated candidate plans use source board-angle sign, while custom manual plans use the operator-edited direction result
- **Generated preview direction consistency** applies to generated candidate plans only
- A **Manual direction override** is allowed only after the plan has become a **Custom formal semi-auto plan**
- When a **Manual direction override** exists, preview must follow the operator-edited direction while clearly treating the result as diverging from the original board-angle interpretation
- **Manual preview authority** means that once a plan is manually edited, all subsequent preview progression, including later folds and transition states, must continue from the manually defined step data rather than snapping back to generated-source geometry rules
- **Implicit preview flip insertion** still applies under **Manual preview authority**: if adjacent manually defined steps switch A/B side mode, preview must insert a `FLIP` transition even before the project decides whether an explicit automatic flip row should exist in the formal step table
- **Explicit flip execution semantics** means preview geometry still flips only once; explicit `FLIP` rows only change whether the transition is treated as automatic execution or manual operator responsibility

## Example dialogue

> **Dev:** "When the operator clicks next plan in fold preview, are they only browsing, or changing the real plan?"
> **Domain expert:** "They are changing the selected candidate. Once selected, that candidate is the formal semi-auto plan used for save and send."
>
> **Dev:** "When the order is saved, do we keep every generated candidate?"
> **Domain expert:** "No. We only save the selected formal plan and the geometry needed to regenerate alternatives next time."
>
> **Dev:** "When we save the formal plan, do we also remember whether it was selected or later hand-edited?"
> **Domain expert:** "Yes. Save a lightweight origin marker so reopening can tell whether the plan is selected-from-candidates or custom-manual."
>
> **Dev:** "If the operator manually tweaks the chosen step list, can they still cycle to the next candidate?"
> **Domain expert:** "No. After manual changes, that plan is now a custom formal plan. To compare generated candidates again, they must regenerate."
>
> **Dev:** "When the operator clicks next plan, does that already count as choosing the real plan?"
> **Domain expert:** "Yes, it updates the formal plan in memory immediately. But PLC send and order save still require explicit actions."
>
> **Dev:** "If they leave fold preview and come back, do we keep the chosen plan or jump back to the default recommendation?"
> **Domain expert:** "Keep the chosen formal plan until geometry or generation inputs change, or until the operator explicitly regenerates plans."
>
> **Dev:** "When an old order is reopened, do we immediately recompute candidates?"
> **Domain expert:** "No. Show the saved formal plan first. Regeneration is an explicit action."
>
> **Dev:** "Does changing reverse order or color side always force regeneration?"
> **Domain expert:** "No. Those are candidate exploration axes, not regeneration boundaries. Geometry changes and fold-order changes are the real regeneration boundary."
>
> **Dev:** "Do inline slitting and taper settings belong to preview candidate generation?"
> **Domain expert:** "No. They are production-mode options, not preview generation inputs."
>
> **Dev:** "When the operator prefers a certain reverse-order or color-side setting, do we hide the other plans?"
> **Domain expert:** "No. That preference only changes which feasible plan is ranked first. The other feasible plans stay available in preview cycling."
>
> **Dev:** "If they toggle reverse order or color side in preview, do we regenerate everything?"
> **Domain expert:** "No. Reorder the current feasible candidates and switch immediately to the new first-ranked plan."
>
> **Dev:** "Does auto mode use a different plan from semi-auto mode?"
> **Domain expert:** "No. Both modes consume the same formal semi-auto plan selected in fold preview."
>
> **Dev:** "Do slitting and taper execution details belong inside preview candidates?"
> **Domain expert:** "No. Preview candidates are pure fold plans. Production-mode steps are appended only when building the final execution step list."
>
> **Dev:** "Does fold layout belong in preview or in the layout/settings page?"
> **Domain expert:** "Layout belongs in the layout/settings page. Preview only browses and selects candidate plans."
>
> **Dev:** "When the operator enters the layout page, do they still see all candidates?"
> **Domain expert:** "No. The layout page edits only the current formal semi-auto plan. Candidate browsing stays in preview."
>
> **Dev:** "Should the layout page show slitting or taper execution steps that are appended later?"
> **Domain expert:** "No. The layout page edits only the pure fold plan. Send-time execution steps stay outside layout editing."
>
> **Dev:** "What happens after the operator confirms layout edits successfully?"
> **Domain expert:** "Return to fold preview immediately and show the newly accepted formal semi-auto plan."
>
> **Dev:** "What does the reset button inside the layout page reset?"
> **Domain expert:** "Only the unconfirmed edits made in the layout page. Reopening candidate selection remains the job of the main preview-page reset action."
>
> **Dev:** "When the operator confirms edits in the layout page, do we accept them immediately?"
> **Domain expert:** "No. Confirm must run feasibility validation first. Only a validated edited plan becomes the accepted formal semi-auto plan."
>
> **Dev:** "What does the reset view button mean in this workflow?"
> **Domain expert:** "It is not just a visual reset. It restores the preview to the first-ranked feasible candidate, and if the current plan is stale it regenerates the candidate set first."
>
> **Dev:** "If the operator has already browsed to another plan, what does reset return to?"
> **Domain expert:** "Reset returns to the current preference's first-ranked candidate, not to the plan that happened to be open earlier."
>
> **Dev:** "Do send and save happen together?"
> **Domain expert:** "No. They are separate actions, but operators usually send the selected formal plan first and save the order afterward."
>
> **Dev:** "Can the operator keep cycling candidates after sending?"
> **Domain expert:** "No. Once sent, that plan is the confirmed sent plan. To preview another candidate, they must reset the view and reopen candidate selection."
>
> **Dev:** "When reset reopens candidate selection, do we reuse old candidates or recompute them?"
> **Domain expert:** "Recompute them from the current board geometry and the current preview preferences."
>
> **Dev:** "What if the operator already hand-edited the current formal plan and then clicks reset view?"
> **Domain expert:** "Ask for confirmation first. Only after confirmation may the system discard manual edits and regenerate candidates."
>
> **Dev:** "What if regeneration finds zero feasible candidates after the geometry changed?"
> **Domain expert:** "Then the old formal plan is no longer valid for the new geometry. Clear candidate browsing, block sending, and tell the operator there is no executable fold plan."
>
> **Dev:** "Can I use the fold direction itself to decide which side rotates in preview?"
> **Domain expert:** "No. Fold direction is only up/down machine motion. Preview always applies that motion to the screen-right active end."
>
> **Dev:** "What does A-B or B-A decide in preview?"
> **Domain expert:** "It decides which branch is outside and which branch is inside. The active folding end still stays on the right side of the screen."
>
> **Dev:** "What happens when A and B swap?"
> **Domain expert:** "First rotate the already-formed shape 180 degrees around the fixed machine preview centerline. Then continue the next fold on the screen-right active end with the swapped outside/inside ownership."
>
> **Dev:** "If there is no explicit flip step in the saved step table, do we still show a flip in preview?"
> **Domain expert:** "Yes. Side-mode change itself is enough to show a preview-only `FLIP` transition. Whether formal flip rows should be auto-inserted can be decided later from board-shape rules."
>
> **Dev:** "Does every A/B side swap mean the part must be flipped?"
> **Domain expert:** "Yes. A/B side swap creates a flip requirement. The only open question is whether that requirement becomes an automatic flip step or stays as a manual flip prompt."
>
> **Dev:** "Are backgauge position, grip type, and color side only explanation fields?"
> **Domain expert:** "Grip type and color side must change the rendered preview effect. Backgauge position does not decide the fold geometry itself."
>
> **Dev:** "So what anchors the fold geometry in preview?"
> **Domain expert:** "The current fold point in the machine-centered preview reference. Backgauge value does not become the fold-geometry anchor."
>
> **Dev:** "Should springback change the final preview angle?"
> **Domain expert:** "No. The final preview still shows the target formed angle. Springback is execution compensation, not an alternate final geometry."
>
> **Dev:** "Do we need to animate the compensation process itself right now?"
> **Domain expert:** "No. For now, preview should show the compensated final result only in text/metadata, not as a separate overbend-and-return animation."
>
> **Dev:** "How far should grip type affect animation right now?"
> **Domain expert:** "It should affect the preview timeline and explicit transition segments like regrip, but it does not require full gripper-motion animation yet."
>
> **Dev:** "If only color side changes, can the same fold direction look opposite on screen?"
> **Domain expert:** "Yes. Color side changes the initial board placement, so the visual up/down result can invert."
>
> **Dev:** "What does preview use to decide whether a fold looks up or down on screen?"
> **Domain expert:** "Initial color-side placement plus the source fold-angle sign. Do not use the absolute target angle magnitude alone."
>
> **Dev:** "What if the saved fold-direction field disagrees with the source angle sign?"
> **Domain expert:** "Preview should trust the source angle sign. The fold-direction field should then be corrected in the generation or mapping layer."
>
> **Dev:** "Can we write the color-side rule as a fixed truth table?"
> **Domain expert:** "Yes. ColorUp keeps the angle sign's visual meaning; ColorDown inverts it."
>
> **Dev:** "Does the truth table always read the same sign source?"
> **Domain expert:** "No. Generated plans read the source board-angle sign. Custom manual plans read the operator-edited direction result."
>
> **Dev:** "Can an operator still force the opposite direction manually?"
> **Domain expert:** "Yes, but only as a custom formal semi-auto plan. Generated candidate plans must stay consistent with the source angle sign."
>
> **Dev:** "After manual edits, what should preview trust for the rest of the animation?"
> **Domain expert:** "It should trust the manually set fold-point data for every step and continue the preview from those manual results."
>
> **Dev:** "If manual steps switch A/B side mode but there is no explicit FLIP row, should preview still show a flip?"
> **Domain expert:** "Yes. A/B side switch itself is enough to insert a preview-only FLIP transition."
>
> **Dev:** "If an explicit FLIP row also exists, do we flip geometry twice?"
> **Domain expert:** "No. Geometry still flips once. The explicit FLIP row only marks that this flip is automatically executed."

## Flagged ambiguities

- "方案" was used to mean both a generated candidate and the final execution result - resolved: generated alternatives are **Fold candidate plans**, while the chosen one is the **Formal semi-auto plan**.
