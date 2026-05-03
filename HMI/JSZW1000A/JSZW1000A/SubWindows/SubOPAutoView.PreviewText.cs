namespace JSZW1000A.SubWindows
{
    public partial class SubOPAutoView
    {
        private void UpdatePreviewStepInfo()
        {
            int currentIndex = GetCurrentPreviewStepIndex();
            if (currentIndex < 0)
            {
                lb下一操作提示.Text = string.Empty;
                lblPlanSummary.Text = mf.GetCurrentFormalPlanSummaryText();
                RefreshPreviewInfoText(-1);
                return;
            }

            int displayIndex = GetDisplayedPreviewStepIndex(currentIndex);
            if (displayIndex < 0)
                return;

            MainFrm.SemiAutoType currentStep = MainFrm.CurtOrder.lstSemiAuto[displayIndex];
            lblPlanSummary.Text = mf.GetCurrentFormalPlanSummaryText();

            if (showingFlipCompletionState)
            {
                lb下一操作提示.Text = Strings.Get("AutoView.NextAction.FlipComplete");
            }
            else if (IsFeedPhase())
            {
                lb下一操作提示.Text = MainFrm.Lang == 0
                    ? $"后挡送料 {MainFrm.FormatDisplayLength(currentStep.后挡位置)}"
                    : $"Backgauge feed {MainFrm.FormatDisplayLength(currentStep.后挡位置)}";
            }
            else if (currentIndex < MainFrm.CurtOrder.lstSemiAuto.Count - 1
                && MainFrm.CurtOrder.lstSemiAuto[currentIndex].内外选择 != MainFrm.CurtOrder.lstSemiAuto[currentIndex + 1].内外选择)
            {
                lb下一操作提示.Text = Strings.Get("AutoView.NextAction.Flip");
            }
            else if (currentStep.行动类型 == 1 || currentStep.行动类型 == 2)
            {
                lb下一操作提示.Text = Strings.Get("AutoView.NextAction.Squash");
            }
            else if (MainFrm.ResolveEffectivePreviewDirection(MainFrm.CurtOrder, MainFrm.CurtOrder.lstSemiAuto, displayIndex) == 0)
            {
                lb下一操作提示.Text = Strings.Get("AutoView.NextAction.FoldUp");
            }
            else
            {
                lb下一操作提示.Text = Strings.Get("AutoView.NextAction.FoldDown");
            }

            RefreshPreviewInfoText(displayIndex);
        }

        private void RefreshPreviewInfoText(int displayIndex)
        {
            string nextAction = lb下一操作提示.Text?.Trim() ?? string.Empty;
            string runtimeMessages = mf?.GetRuntimeMessagesSnapshot()?.Trim() ?? string.Empty;
            bool showRuntimeMessages = !string.IsNullOrWhiteSpace(runtimeMessages);
            if (!string.Equals(rtbRuntimeMessages.Text, runtimeMessages, StringComparison.Ordinal))
                rtbRuntimeMessages.Text = runtimeMessages;
            if (lblRuntimeMessagesTitle.Visible != showRuntimeMessages)
                lblRuntimeMessagesTitle.Visible = showRuntimeMessages;
            if (rtbRuntimeMessages.Visible != showRuntimeMessages)
                rtbRuntimeMessages.Visible = showRuntimeMessages;

            if (showStructureExplanationMode)
            {
                string foldList = mf?.GetCurrentFormalPlanStructureExplanation(displayIndex) ?? string.Empty;
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                if (!string.IsNullOrWhiteSpace(nextAction))
                    builder.AppendLine(nextAction);

                if (!string.IsNullOrWhiteSpace(foldList))
                {
                    if (builder.Length > 0)
                        builder.AppendLine();
                    builder.Append(foldList.TrimEnd());
                }

                string newText = builder.ToString();
                if (!string.Equals(rtbPreviewPlan.Text, newText, StringComparison.Ordinal))
                    rtbPreviewPlan.Text = newText;
                return;
            }

            RenderFoldListText(nextAction, displayIndex);
        }

        private void UpdatePreviewModeVisuals()
        {
            lblFoldListTitle.Text = showStructureExplanationMode
                ? (MainFrm.Lang == 0 ? "结构说明:" : "Structure:")
                : Strings.Get("AutoView.Label.FoldList");
            btnPreviewTextMode.Text = showStructureExplanationMode
                ? (MainFrm.Lang == 0 ? "步骤列表" : "Step List")
                : (MainFrm.Lang == 0 ? "结构说明" : "Explain");
        }

        private void RenderFoldListText(string nextAction, int currentDisplayIndex)
        {
            rtbPreviewPlan.SuspendLayout();
            rtbPreviewPlan.Clear();

            if (!string.IsNullOrWhiteSpace(nextAction))
            {
                AppendPreviewLine(nextAction, false, true, Color.White, Color.Transparent);
                AppendPreviewLine(string.Empty, false, false, Color.Black, Color.Transparent);
            }

            int currentBlockStart = -1;
            for (int i = 0; i < MainFrm.CurtOrder.lstSemiAuto.Count; i++)
            {
                MainFrm.SemiAutoType step = MainFrm.CurtOrder.lstSemiAuto[i];
                bool isCurrent = i == currentDisplayIndex;
                if (isCurrent)
                    currentBlockStart = rtbPreviewPlan.TextLength;

                string prefix = isCurrent ? "> " : "  ";
                AppendPreviewLine(prefix + BuildStepTitle(step, i + 1), isCurrent, true);

                foreach (string detail in BuildStepDetails(step, i))
                    AppendPreviewLine(detail, isCurrent, false);

                AppendPreviewLine(string.Empty, false, false, Color.Black, Color.Transparent);
            }

            if (currentBlockStart >= 0)
            {
                rtbPreviewPlan.SelectionStart = currentBlockStart;
                rtbPreviewPlan.SelectionLength = 0;
                rtbPreviewPlan.ScrollToCaret();
            }

            rtbPreviewPlan.SelectionLength = 0;
            rtbPreviewPlan.ResumeLayout();
        }

        private void AppendPreviewLine(string text, bool highlight, bool bold, Color? foreground = null, Color? background = null)
        {
            rtbPreviewPlan.SelectionStart = rtbPreviewPlan.TextLength;
            rtbPreviewPlan.SelectionLength = 0;
            rtbPreviewPlan.SelectionFont = new Font(
                rtbPreviewPlan.Font,
                bold ? FontStyle.Bold : FontStyle.Regular);
            rtbPreviewPlan.SelectionColor = foreground ?? (highlight ? Color.White : Color.Black);
            rtbPreviewPlan.SelectionBackColor = background ?? (highlight ? Color.FromArgb(128, 48, 48) : rtbPreviewPlan.BackColor);
            rtbPreviewPlan.AppendText(text + Environment.NewLine);
        }

        private string BuildStepTitle(MainFrm.SemiAutoType step, int displayOrder)
        {
            string actionText = step.行动类型 switch
            {
                MainFrm.SemiAutoActionSlit => MainFrm.Lang == 0 ? "分条" : "Slit",
                MainFrm.SemiAutoActionFlip => MainFrm.Lang == 0 ? "翻面" : "Flip",
                MainFrm.SemiAutoActionSquash => MainFrm.Lang == 0 ? "压死边" : "Squash",
                MainFrm.SemiAutoActionOpenSquash => MainFrm.Lang == 0 ? "压开边" : "Open Squash",
                _ => MainFrm.Lang == 0 ? $"折弯 {step.折弯序号}" : $"Fold {step.折弯序号}"
            };

            return MainFrm.Lang == 0
                ? $"{displayOrder}. {actionText}"
                : $"Step {displayOrder}. {actionText}";
        }

        private IEnumerable<string> BuildStepDetails(MainFrm.SemiAutoType step, int stepIndex)
        {
            yield return (MainFrm.Lang == 0 ? "后挡位置 " : "Backgauge ")
                + MainFrm.FormatDisplayLength(step.后挡位置)
                + (MainFrm.Lang == 0 ? $"；{LocalizationText.GripType(step.抓取类型)}" : $"; {LocalizationText.GripType(step.抓取类型)}");

            if (step.行动类型 == MainFrm.SemiAutoActionFlip)
            {
                yield return MainFrm.Lang == 0 ? "材料翻面" : "Material side flip";
                yield break;
            }

            if (step.行动类型 == MainFrm.SemiAutoActionSlit)
            {
                yield return MainFrm.Lang == 0 ? "执行分条动作" : "Execute slitting";
                yield break;
            }

            int effectiveDirection = MainFrm.ResolveEffectivePreviewDirection(MainFrm.CurtOrder, MainFrm.CurtOrder.lstSemiAuto, stepIndex);
            string foldDirection = LocalizationText.FoldDirectionShort(effectiveDirection);
            string springback = step.回弹值 == 0
                ? "+0.0"
                : $"{step.回弹值:+0.0;-0.0;0.0}";
            yield return (MainFrm.Lang == 0 ? "折弯角度 " : "Fold Angle ")
                + $"{step.折弯角度:0.0}° ({springback}°) {foldDirection}";

            string clampHeight = MainFrm.Lang == 0
                ? LocalizationText.ReleaseHeightShort(step.松开高度)
                : LocalizationText.ReleaseHeight(step.松开高度);
            string regrip = step.重新抓取 != 0
                ? (MainFrm.Lang == 0 ? "；重新抓取" : "; Regrip")
                : string.Empty;
            yield return (MainFrm.Lang == 0 ? "压钳高度 " : "Clamp Height ")
                + clampHeight
                + regrip;
        }
    }
}
