using System.IO;
using System.Text;

namespace JSZW1000A
{
    public partial class MainFrm
    {
        public static void RebuildSemiAutoDerivedState(ref OrderType order)
        {
            if (order.lstSemiAuto.Count <= 0 || order.pxList.Count <= 0)
                return;

            StringBuilder trace = new StringBuilder();
            trace.AppendLine("SemiAuto Derived State Trace");
            trace.AppendLine("Order: " + order.Name);
            trace.AppendLine("Total Width: " + order.Width.ToString("0.##"));
            trace.AppendLine("Initial Reverse: " + order.st逆序);
            trace.AppendLine("Initial ColorDown: " + order.st色下);
            trace.AppendLine();

            bool currentColorDown = order.st色下;
            bool currentReferenceLeft = order.st逆序;
            List<PointF> currentProfile = CloneCurrentProfile(order.pxList);

            for (int i = 0; i < order.lstSemiAuto.Count; i++)
            {
                SemiAutoType step = order.lstSemiAuto[i];
                List<PointF> stagedProfile = BuildStagedProfile(order, GetStageProfileStepCount(order.lstSemiAuto, i));
                int anchorIndex = GetValidAnchorIndex(step.坐标序号, currentProfile.Count);
                string referenceSide = currentReferenceLeft ? "Left" : "Right";
                double baseBackGauge = 0;

                if (UsesDerivedBackGauge(step))
                {
                    baseBackGauge = CalcBackGaugeByCurrentProfile(order, order.lstSemiAuto, i, currentProfile, step, anchorIndex);
                    step.后挡位置 = CalcStepBackGaugePosition(step, baseBackGauge);
                }

                step.is色下 = currentColorDown;

                bool explicitFlip = step.行动类型 == SemiAutoActionFlip;
                bool implicitFlip = !explicitFlip
                    && CanImplicitlyFlip(step.行动类型)
                    && HasNextStepWithDifferentSide(order.lstSemiAuto, i);
                step.操作提示 = implicitFlip ? 1 : 0;

                order.lstSemiAuto[i] = step;

                AppendTraceForStep(trace, order, i, step, stagedProfile, anchorIndex, referenceSide, baseBackGauge);

                ApplyCanonicalGeometryStep(currentProfile, step, anchorIndex);
                if (!explicitFlip && ShouldApplyCanonicalFlipAfterStep(order.lstSemiAuto, i))
                    FlipProfile(currentProfile);

                if (explicitFlip || implicitFlip)
                    currentColorDown = !currentColorDown;
            }

            WriteDerivedStateTrace(trace.ToString());
        }

        public static List<PointF> BuildSemiAutoStageProfile(OrderType order, int appliedStepCount)
        {
            if (order.lstSemiAuto.Count <= 0 || order.pxList.Count <= 0)
                return new List<PointF>();

            int clampedStepCount = Math.Clamp(appliedStepCount, 0, order.lstSemiAuto.Count);
            return BuildStagedProfile(order, clampedStepCount);
        }

        public static List<PointF> BuildSemiAutoPreviewStageProfile(OrderType order, int appliedStepCount, bool applyFlipAfterLastIncludedStep = true)
        {
            if (order.lstSemiAuto.Count <= 0 || order.pxList.Count <= 0)
                return new List<PointF>();

            int clampedStepCount = Math.Clamp(appliedStepCount, 0, order.lstSemiAuto.Count);
            return BuildPreviewProfileBySteps(order, clampedStepCount, applyFlipAfterLastIncludedStep);
        }

        public static bool ResolveSemiAutoPreviewColorDown(OrderType order, int appliedStepCount, bool applyFlipAfterLastIncludedStep = true)
        {
            bool colorDown = order.st色下;
            if (order.lstSemiAuto.Count <= 0)
                return colorDown;

            int clampedStepCount = Math.Clamp(appliedStepCount, 0, order.lstSemiAuto.Count);
            for (int i = 0; i < clampedStepCount; i++)
            {
                if (ShouldApplyPreviewFlipAfterStep(order.lstSemiAuto, i, clampedStepCount, applyFlipAfterLastIncludedStep))
                    colorDown = !colorDown;
            }

            return colorDown;
        }

        private static int GetStageProfileStepCount(IReadOnlyList<SemiAutoType> steps, int currentIndex)
        {
            return IsLastFoldStep(steps, currentIndex) ? steps.Count : currentIndex;
        }

        private static bool UsesDerivedBackGauge(SemiAutoType step)
        {
            return step.行动类型 == SemiAutoActionFold
                || step.行动类型 == SemiAutoActionSquash
                || step.行动类型 == SemiAutoActionOpenSquash;
        }

        private static List<PointF> BuildStagedProfile(OrderType order, int currentStepIndex)
        {
            double[] effectiveAngles = BuildEffectiveAngles(order, currentStepIndex);
            return BuildProfileByAngles(order, effectiveAngles);
        }

        private static double[] BuildEffectiveAngles(OrderType order, int currentStepIndex)
        {
            double[] effectiveAngles = new double[order.lengAngle.Length];
            for (int i = 0; i < effectiveAngles.Length; i++)
                effectiveAngles[i] = 180.0;

            for (int i = 0; i < currentStepIndex; i++)
            {
                SemiAutoType step = order.lstSemiAuto[i];
                if (step.行动类型 != SemiAutoActionFold || IsLegacySemiAutoPlaceholder(step))
                    continue;

                int angleIndex = GetAngleIndexForStep(step, order.lengAngle.Length);
                if (angleIndex >= 0)
                    effectiveAngles[angleIndex] = GetOriginalAngleForStep(order, angleIndex);
            }

            return effectiveAngles;
        }

        private static List<PointF> BuildPreviewProfileBySteps(OrderType order, int appliedStepCount, bool applyFlipAfterLastIncludedStep)
        {
            List<PointF> profile = BuildFlatPreviewProfile(order);
            for (int i = 0; i < appliedStepCount; i++)
            {
                SemiAutoType step = order.lstSemiAuto[i];
                int anchorIndex = GetValidAnchorIndex(step.坐标序号, profile.Count);

                if (step.行动类型 == SemiAutoActionFold && !IsLegacySemiAutoPlaceholder(step))
                    ApplyPreviewFoldStep(order, profile, step, i, anchorIndex);

                if (ShouldApplyPreviewFlipAfterStep(order.lstSemiAuto, i, appliedStepCount, applyFlipAfterLastIncludedStep))
                    RotateWholePreviewProfile(order, profile, 180.0);
            }

            return profile;
        }

        private static List<PointF> BuildFlatPreviewProfile(OrderType order)
        {
            double[] effectiveAngles = new double[order.lengAngle.Length];
            for (int i = 0; i < effectiveAngles.Length; i++)
                effectiveAngles[i] = 180.0;

            return BuildProfileByAngles(order, effectiveAngles);
        }

        private static void ApplyPreviewFoldStep(OrderType order, List<PointF> profile, SemiAutoType step, int stepIndex, int anchorIndex)
        {
            double foldAngle = Math.Abs(step.折弯角度);
            if (foldAngle < 0.001)
                return;

            int direction = ResolveEffectivePreviewDirection(order, order.lstSemiAuto, stepIndex);
            double rotateAngle = direction == 0 ? 180.0 + foldAngle : 180.0 - foldAngle;
            bool rotateLowerIndexSide = step.内外选择 == 0;
            RotateProfileSide(profile, anchorIndex, rotateAngle, rotateLowerIndexSide);
        }

        private static bool ShouldApplyPreviewFlipAfterStep(IReadOnlyList<SemiAutoType> steps, int currentIndex, int appliedStepCount, bool applyFlipAfterLastIncludedStep)
        {
            if (currentIndex < 0 || currentIndex >= steps.Count)
                return false;

            if (steps[currentIndex].行动类型 == SemiAutoActionFlip)
                return true;

            if (currentIndex >= steps.Count - 1 || !CanImplicitlyFlip(steps[currentIndex].行动类型))
                return false;

            if (!ShouldApplyImplicitFlipAfterStep(steps, currentIndex))
                return false;

            bool isTrailingAppliedStep = currentIndex == appliedStepCount - 1;
            return !isTrailingAppliedStep || applyFlipAfterLastIncludedStep;
        }

        private static void RotateWholePreviewProfile(OrderType order, List<PointF> profile, double angle)
        {
            if (profile.Count <= 0)
                return;

            PointF pivot = GetPreviewFlipReferencePoint(order);
            for (int i = 0; i < profile.Count; i++)
                profile[i] = RotatePoint(pivot, profile[i], angle);
        }

        private static PointF GetPreviewFlipReferencePoint(OrderType order)
        {
            double width = order.Width > 0 ? order.Width : CalculateOrderWidth(order.lengAngle);
            return new PointF((float)(-width / 2.0), 0f);
        }

        private static int GetAngleIndexForStep(SemiAutoType step, int maxLength)
        {
            int angleIndex = step.长角序号 + 1;
            if (step.长角序号 == 99)
                angleIndex = 99;

            if (angleIndex <= 0 || angleIndex >= maxLength)
                return -1;

            return angleIndex;
        }

        private static double GetOriginalAngleForStep(OrderType order, int angleIndex)
        {
            if (angleIndex < 0 || angleIndex >= order.lengAngle.Length)
                return 180.0;

            double originalAngle = order.lengAngle[angleIndex].Angle;
            if (Math.Abs(originalAngle) < 0.001)
                return 180.0;

            return originalAngle;
        }

        private static List<PointF> BuildProfileByAngles(OrderType order, double[] effectiveAngles)
        {
            double currentHeading;
            if (order.显示朝向已初始化)
            {
                currentHeading = order.显示起始角度;
            }
            else if (order.pxList != null && order.pxList.Count > 1)
            {
                float dx = order.pxList[1].X - order.pxList[0].X;
                float dy = order.pxList[1].Y - order.pxList[0].Y;
                currentHeading = Math.Atan2(dy, -dx) * 180.0 / Math.PI;
            }
            else
            {
                currentHeading = 180.0;
            }

            List<PointF> profile = new List<PointF>();
            float currentX = 0f;
            float currentY = 0f;
            profile.Add(new PointF(currentX, currentY));

            for (int i = 1; i < order.lengAngle.Length; i++)
            {
                bool isCurrentZero = Math.Abs(order.lengAngle[i].Length) < 0.001;
                bool isNextZero = true;
                if (i + 1 < order.lengAngle.Length)
                    isNextZero = Math.Abs(order.lengAngle[i + 1].Length) < 0.001;

                if (isCurrentZero && isNextZero)
                    break;

                double len = order.lengAngle[i].Length;
                double includedAngle = effectiveAngles[i];
                double deflection = 180.0 - Math.Abs(includedAngle);
                if (includedAngle < 0)
                    deflection = -deflection;

                currentHeading -= deflection;
                double radians = currentHeading * Math.PI / 180.0;

                currentX += (float)(len * Math.Cos(radians));
                currentY += (float)(len * Math.Sin(radians));
                profile.Add(new PointF(currentX, currentY));
            }

            return profile;
        }

        private static bool CanImplicitlyFlip(int actionType)
        {
            return actionType == SemiAutoActionFold
                || actionType == SemiAutoActionSquash
                || actionType == SemiAutoActionOpenSquash;
        }

        private static bool HasNextStepWithDifferentSide(IReadOnlyList<SemiAutoType> steps, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= steps.Count - 1)
                return false;

            return steps[currentIndex].内外选择 != steps[currentIndex + 1].内外选择;
        }

        private static bool IsLastFoldStep(IReadOnlyList<SemiAutoType> steps, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= steps.Count)
                return false;
            if (steps[currentIndex].行动类型 != SemiAutoActionFold)
                return false;

            for (int i = currentIndex + 1; i < steps.Count; i++)
            {
                if (steps[i].行动类型 == SemiAutoActionFold)
                    return false;
            }
            return true;
        }

        private static void AppendTraceForStep(
            StringBuilder trace,
            OrderType order,
            int stepIndex,
            SemiAutoType step,
            List<PointF> stagedProfile,
            int anchorIndex,
            string referenceSide,
            double baseBackGauge)
        {
            trace.AppendLine($"Step {stepIndex + 1}");
            trace.AppendLine($"  折弯序号: {step.折弯序号}");
            trace.AppendLine($"  行动类型: {step.行动类型}");
            trace.AppendLine($"  长角序号: {step.长角序号}");
            trace.AppendLine($"  坐标序号: {step.坐标序号}");
            trace.AppendLine($"  内外选择: {step.内外选择}");
            trace.AppendLine($"  折弯方向: {step.折弯方向}");
            trace.AppendLine($"  参考侧: {referenceSide}");
            trace.AppendLine($"  AnchorIndex: {anchorIndex}");
            trace.AppendLine($"  BaseBackGauge: {baseBackGauge:0.00}");
            trace.AppendLine($"  FinalBackGauge: {step.后挡位置:0.00}");
            trace.AppendLine($"  StageProfile: {FormatProfile(stagedProfile)}");
            trace.AppendLine($"  EffectiveAngles: {FormatEffectiveAngles(order, stepIndex)}");
            trace.AppendLine();
        }

        private static string FormatEffectiveAngles(OrderType order, int currentStepIndex)
        {
            double[] angles = BuildEffectiveAngles(order, currentStepIndex);
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < angles.Length; i++)
            {
                bool isCurrentZero = Math.Abs(order.lengAngle[i].Length) < 0.001;
                bool isNextZero = true;
                if (i + 1 < order.lengAngle.Length)
                    isNextZero = Math.Abs(order.lengAngle[i + 1].Length) < 0.001;
                if (isCurrentZero && isNextZero)
                    break;

                if (i > 1)
                    sb.Append(", ");
                sb.Append(i);
                sb.Append(":");
                sb.Append(angles[i].ToString("0.0"));
            }
            return sb.ToString();
        }

        private static string FormatProfile(List<PointF> profile)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < profile.Count; i++)
            {
                if (i > 0)
                    sb.Append(" | ");
                sb.Append("P");
                sb.Append(i.ToString("D2"));
                sb.Append("=(");
                sb.Append(profile[i].X.ToString("0.00"));
                sb.Append(",");
                sb.Append(profile[i].Y.ToString("0.00"));
                sb.Append(")");
            }
            return sb.ToString();
        }

        private static void WriteDerivedStateTrace(string content)
        {
            try
            {
                string path = Path.Combine(AppContext.BaseDirectory, "SemiAutoDerivedStateTrace.txt");
                File.WriteAllText(path, content, Encoding.UTF8);
            }
            catch
            {
                // Ignore trace write failures to avoid affecting runtime behavior.
            }
        }
    }
}
