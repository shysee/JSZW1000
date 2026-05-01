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

## Flagged ambiguities

- "方案" was used to mean both a generated candidate and the final execution result - resolved: generated alternatives are **Fold candidate plans**, while the chosen one is the **Formal semi-auto plan**.
