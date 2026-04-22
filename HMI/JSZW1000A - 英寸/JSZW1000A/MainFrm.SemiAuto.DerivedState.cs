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
            double remainingFlatLength = order.Width;

            for (int i = 0; i < order.lstSemiAuto.Count; i++)
            {
                SemiAutoType step = order.lstSemiAuto[i];
                List<PointF> stagedProfile = BuildStagedProfile(order, GetStageProfileStepCount(order.lstSemiAuto, i));
                int anchorIndex = GetValidAnchorIndex(step.坐标序号, stagedProfile.Count);
                string referenceSide = currentReferenceLeft ? "Left" : "Right";
                double baseBackGauge = 0;

                if (UsesDerivedBackGauge(step))
                {
                    remainingFlatLength = ConsumeFlatLengthForStep(order, step, remainingFlatLength);
                    baseBackGauge = CalcBackGaugeBySequenceFormula(step, remainingFlatLength);
                    step.后挡位置 = CalcDerivedBackGaugePosition(step, baseBackGauge);
                }

                step.is色下 = currentColorDown;

                bool explicitFlip = step.行动类型 == SemiAutoActionFlip;
                bool implicitFlip = !explicitFlip
                    && CanImplicitlyFlip(step.行动类型)
                    && HasNextStepWithDifferentSide(order.lstSemiAuto, i);
                step.操作提示 = implicitFlip ? 1 : 0;

                order.lstSemiAuto[i] = step;

                AppendTraceForStep(trace, order, i, step, stagedProfile, anchorIndex, referenceSide, baseBackGauge);

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
                    ApplyPreviewFoldStep(order, profile, step, anchorIndex);

                if (ShouldApplyPreviewFlipAfterStep(order.lstSemiAuto, i, appliedStepCount, applyFlipAfterLastIncludedStep))
                    RotateWholePreviewProfile(profile, anchorIndex, 180.0);
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

        private static void ApplyPreviewFoldStep(OrderType order, List<PointF> profile, SemiAutoType step, int anchorIndex)
        {
            double foldAngle = Math.Abs(step.折弯角度);
            if (foldAngle < 0.001)
                return;

            bool positiveAngle = UsesPositivePreviewAngle(order, step);
            double rotateAngle = positiveAngle ? 180.0 - foldAngle : 180.0 + foldAngle;
            bool rotateLowerIndexSide = step.内外选择 == 0;
            RotateProfileSide(profile, anchorIndex, rotateAngle, rotateLowerIndexSide);
        }

        private static bool UsesPositivePreviewAngle(OrderType order, SemiAutoType step)
        {
            // `is色下` only describes the initial feed orientation.
            // After flip, new folds still follow the configured fold direction.
            return order.st色下
                ? step.折弯方向 == 1
                : step.折弯方向 == 0;
        }

        private static bool ShouldApplyPreviewFlipAfterStep(IReadOnlyList<SemiAutoType> steps, int currentIndex, int appliedStepCount, bool applyFlipAfterLastIncludedStep)
        {
            if (currentIndex < 0 || currentIndex >= steps.Count)
                return false;

            if (steps[currentIndex].行动类型 == SemiAutoActionFlip)
                return true;

            if (currentIndex >= steps.Count - 1 || !CanImplicitlyFlip(steps[currentIndex].行动类型))
                return false;

            if (steps[currentIndex].内外选择 == steps[currentIndex + 1].内外选择)
                return false;

            bool isTrailingAppliedStep = currentIndex == appliedStepCount - 1;
            return !isTrailingAppliedStep || applyFlipAfterLastIncludedStep;
        }

        private static void RotateWholePreviewProfile(List<PointF> profile, int pivotIndex, double angle)
        {
            if (profile.Count <= 0)
                return;

            PointF pivot = profile[GetValidAnchorIndex(pivotIndex, profile.Count)];
            for (int i = 0; i < profile.Count; i++)
                profile[i] = RotatePoint(pivot, profile[i], angle);
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

        private static double CalcBackGaugeByReferenceSide(List<PointF> profile, SemiAutoType step, int anchorIndex, bool useLeftReference)
        {
            if (profile.Count <= 0)
                return 0;

            return CalcProjectedBackGaugeByReferenceDirection(profile, anchorIndex, useLeftReference);
        }

        private static double ConsumeFlatLengthForStep(OrderType order, SemiAutoType step, double remainingFlatLength)
        {
            if (step.行动类型 != SemiAutoActionFold)
                return remainingFlatLength;

            double deduction = 0;
            if (step.长角序号 == 0 || step.长角序号 == 99)
            {
                deduction = order.lengAngle[step.长角序号].Length;
            }
            else if (!order.st逆序)
            {
                deduction = order.lengAngle[step.长角序号].Length;
            }
            else
            {
                int reverseIndex = Math.Min(step.长角序号 + 1, order.lengAngle.Length - 1);
                deduction = order.lengAngle[reverseIndex].Length;
            }

            if (deduction <= 0)
                return remainingFlatLength;

            return RoundBackGaugePosition(Math.Max(0, remainingFlatLength - deduction));
        }

        private static double CalcBackGaugeBySequenceFormula(SemiAutoType step, double remainingFlatLength)
        {
            return remainingFlatLength;
        }

        private static double CalcDerivedBackGaugePosition(SemiAutoType step, double baseBackGauge)
        {
            double target = baseBackGauge;
            if (step.行动类型 == SemiAutoActionSquash)
                target += (step.折弯方向 == 0) ? Hmi_rArray[115] : Hmi_rArray[116];
            else if (step.行动类型 == SemiAutoActionOpenSquash)
                target += (step.折弯方向 == 0) ? Hmi_rArray[118] : Hmi_rArray[119];

            return RoundBackGaugePosition(target);
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

        private static double CalcProjectedBackGaugeByReferenceDirection(List<PointF> profile, int anchorIndex, bool useLeftReference)
        {
            if (profile.Count <= 1)
                return 0;

            int neighborIndex = useLeftReference
                ? Math.Max(anchorIndex - 1, 0)
                : Math.Min(anchorIndex + 1, profile.Count - 1);
            if (neighborIndex == anchorIndex)
                return 0;

            PointF anchor = profile[anchorIndex];
            PointF neighbor = profile[neighborIndex];
            double vx = neighbor.X - anchor.X;
            double vy = neighbor.Y - anchor.Y;
            double length = Math.Sqrt(vx * vx + vy * vy);
            if (length < 0.001)
                return 0;

            double ux = vx / length;
            double uy = vy / length;
            double maxProjection = 0;

            IEnumerable<int> indices = useLeftReference
                ? Enumerable.Range(0, anchorIndex + 1)
                : Enumerable.Range(anchorIndex, profile.Count - anchorIndex);

            foreach (int idx in indices)
            {
                double projection = (profile[idx].X - anchor.X) * ux + (profile[idx].Y - anchor.Y) * uy;
                if (projection > maxProjection)
                    maxProjection = projection;
            }

            return RoundBackGaugePosition(maxProjection);
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
