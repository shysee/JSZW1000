namespace JSZW1000A
{
    public partial class MainFrm
    {
        public const int SemiAutoActionFold = 0;
        public const int SemiAutoActionSquash = 1;
        public const int SemiAutoActionOpenSquash = 2;
        public const int SemiAutoActionSlit = 3;
        public const int SemiAutoActionFlip = 8;

        private const double LegacySemiAutoHeadAngle = 3.001;
        private const double LegacySemiAutoTailAngle = 3.99;
        private const double InlineSlitWidthTolerance = 0.5;

        private readonly struct InlineSlitPlan
        {
            public InlineSlitPlan(double offcutWidth, List<double> productWidths, bool offcutFirst)
            {
                OffcutWidth = offcutWidth;
                ProductWidths = productWidths;
                OffcutFirst = offcutFirst;
            }

            public double OffcutWidth { get; }
            public List<double> ProductWidths { get; }
            public bool OffcutFirst { get; }
        }

        private readonly struct SemiAutoGenerationContext
        {
            public SemiAutoGenerationContext(OrderType order)
            {
                Order = order;
                IsReverse = order.st逆序;
                IsColorDown = order.st色下;
                IsTaper = order.isTaper;
                IsSlitter = order.isSlitter;
                IsInlineSlitEnabled = order.边做边分切启用;
                HasHeadSquash = order.lengAngle[0].Angle > 0 && order.lengAngle[0].Length > 0;
                HasTailSquash = order.lengAngle[99].Angle > 0 && order.lengAngle[99].Length > 0;
            }

            public OrderType Order { get; }
            public bool IsReverse { get; }
            public bool IsColorDown { get; }
            public bool IsTaper { get; }
            public bool IsSlitter { get; }
            public bool IsInlineSlitEnabled { get; }
            public bool HasHeadSquash { get; }
            public bool HasTailSquash { get; }
        }

        private readonly struct SemiAutoGenerationResult
        {
            public SemiAutoGenerationResult(
                bool success,
                string strategyName,
                List<SemiAutoType> steps,
                string failureCode,
                string failureMessage)
            {
                Success = success;
                StrategyName = strategyName;
                Steps = steps;
                FailureCode = failureCode;
                FailureMessage = failureMessage;
            }

            public bool Success { get; }
            public string StrategyName { get; }
            public List<SemiAutoType> Steps { get; }
            public string FailureCode { get; }
            public string FailureMessage { get; }

            public static SemiAutoGenerationResult Ok(string strategyName, List<SemiAutoType> steps)
            {
                return new SemiAutoGenerationResult(true, strategyName, steps, "", "");
            }

            public static SemiAutoGenerationResult Fail(string strategyName, string failureCode, string failureMessage)
            {
                return new SemiAutoGenerationResult(false, strategyName, new List<SemiAutoType>(), failureCode, failureMessage);
            }
        }

        public static bool IsSemiAutoSquashAction(int actionType)
        {
            return actionType == SemiAutoActionSquash || actionType == SemiAutoActionOpenSquash;
        }

        public static bool IsLegacySemiAutoPlaceholder(SemiAutoType step)
        {
            return step.行动类型 == SemiAutoActionFold
                && (IsLegacyAngle(step.折弯角度, LegacySemiAutoHeadAngle)
                    || IsLegacyAngle(step.折弯角度, LegacySemiAutoTailAngle));
        }

        public static string GetSemiAutoActionDisplayName(SemiAutoType step, int displayIndex, int lang)
        {
            return step.行动类型 switch
            {
                SemiAutoActionSquash => Strings.Get("SemiAuto.Action.SquashPrefix") + displayIndex.ToString("D2"),
                SemiAutoActionOpenSquash => Strings.Get("SemiAuto.Action.OpenSquashPrefix") + displayIndex.ToString("D2"),
                SemiAutoActionSlit => Strings.Get("SemiAuto.Action.SlitPrefix") + displayIndex.ToString("D2"),
                SemiAutoActionFlip => Strings.Get("SemiAuto.Action.FlipPrefix") + displayIndex.ToString("D2"),
                _ => Strings.Get("SemiAuto.Action.FoldPrefix") + displayIndex.ToString("D2"),
            };
        }

        public void NormalizeGeneratedSemiAutoSequence()
        {
            if (CurtOrder.lstSemiAuto.Count <= 0)
            {
                return;
            }

            List<SemiAutoType> normalized = new List<SemiAutoType>(CurtOrder.lstSemiAuto);
            for (int i = 0; i < normalized.Count; i++)
            {
                SemiAutoType temp = normalized[i];
                temp.折弯序号 = i + 1;
                normalized[i] = temp;
            }

            CurtOrder.lstSemiAuto = normalized;
        }

        public static void PackSemiAutoStepsToPlc(IReadOnlyList<SemiAutoType> steps, short[] target)
        {
            Array.Clear(target, 0, target.Length);

            int maxSteps = Math.Min(steps.Count, target.Length / 10);
            for (int i = 0; i < maxSteps; i++)
            {
                PackSemiAutoStep(steps, i, target);
            }
        }

        private static void PackSemiAutoStep(IReadOnlyList<SemiAutoType> steps, int index, short[] target)
        {
            SemiAutoType step = steps[index];
            int baseIndex = index * 10;

            target[baseIndex + 0] = (short)step.行动类型;
            target[baseIndex + 1] = (short)step.折弯方向;

            if (step.行动类型 == SemiAutoActionSlit)
            {
                target[baseIndex + 2] = 8880;
                target[baseIndex + 3] = 0;
            }
            else if (IsSemiAutoSquashAction(step.行动类型))
            {
                target[baseIndex + 2] = 10;
                target[baseIndex + 3] = (short)Math.Round(step.回弹值);
            }
            else if (step.行动类型 == SemiAutoActionFlip)
            {
                target[baseIndex + 2] = 80;
                target[baseIndex + 3] = 0;
            }
            else if (IsLegacySemiAutoPlaceholder(step))
            {
                target[baseIndex + 2] = 300;
                target[baseIndex + 3] = 0;
            }
            else
            {
                target[baseIndex + 2] = (short)Math.Round(step.折弯角度 * 10.0);
                target[baseIndex + 3] = (short)Math.Round(step.回弹值 * 10.0);
            }

            target[baseIndex + 4] = (short)Math.Round(step.后挡位置 * 10.0);
            target[baseIndex + 5] = (short)step.抓取类型;
            target[baseIndex + 6] = (short)step.松开高度;
            target[baseIndex + 7] = (short)step.翻板收缩值;
            target[baseIndex + 8] = (short)((step.重新抓取 != 0
                || (index < steps.Count - 1 && step.内外选择 != steps[index + 1].内外选择)) ? 1 : 0);
            target[baseIndex + 9] = (short)Math.Round(step.锥度斜率);
        }

        private static bool IsLegacyAngle(double value, double target)
        {
            return Math.Abs(value - target) < 0.001;
        }

        public static double RoundBackGaugePosition(double value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        public static double CalculateOrderWidth(IReadOnlyList<LengAngle> lengAngles)
        {
            if (lengAngles == null || lengAngles.Count <= 0)
                return 0;

            double width = 0;
            for (int i = 0; i < lengAngles.Count; i++)
            {
                bool isHeadOrTail = (i == 0 || i == lengAngles.Count - 1);
                if (isHeadOrTail)
                {
                    if (lengAngles[i].Length > 0 && lengAngles[i].Angle > 0)
                        width += lengAngles[i].Length;
                }
                else
                {
                    width += lengAngles[i].Length;
                }
            }

            return RoundBackGaugePosition(width);
        }

        public static void RecalculateBackGaugePositionsByCurrentProfile(ref OrderType order)
        {
            if (order.lstSemiAuto.Count <= 0 || order.pxList.Count <= 0)
                return;

            List<PointF> currentProfile = CloneCurrentProfile(order.pxList);
            for (int i = 0; i < order.lstSemiAuto.Count; i++)
            {
                SemiAutoType step = order.lstSemiAuto[i];
                int anchorIndex = GetValidAnchorIndex(step.坐标序号, currentProfile.Count);
                double baseBackGauge = CalcBackGaugeByCurrentProfile(currentProfile, step, anchorIndex);
                step.后挡位置 = CalcStepBackGaugePosition(step, baseBackGauge);
                order.lstSemiAuto[i] = step;

                ApplyGeometryStep(currentProfile, step, anchorIndex);
                if (ShouldFlipAfterStep(order.lstSemiAuto, i))
                    FlipProfile(currentProfile);
            }
        }

        public static List<PointF> BuildSemiAutoGeometrySnapshot(OrderType order, int appliedStepCount)
        {
            if (order.lstSemiAuto.Count <= 0 || order.pxList.Count <= 0)
                return new List<PointF>();

            List<PointF> currentProfile = CloneCurrentProfile(order.pxList);
            int clampedCount = Math.Clamp(appliedStepCount, 0, order.lstSemiAuto.Count);
            for (int i = 0; i < clampedCount; i++)
            {
                SemiAutoType step = order.lstSemiAuto[i];
                int anchorIndex = GetValidAnchorIndex(step.坐标序号, currentProfile.Count);
                ApplyGeometryStep(currentProfile, step, anchorIndex);
                if (ShouldFlipAfterStep(order.lstSemiAuto, i))
                    FlipProfile(currentProfile);
            }

            return currentProfile;
        }

        public static bool ResolveFlatSideIsLeft(List<PointF> profile, SemiAutoType step, int anchorIndex)
        {
            return IsFlatSideLeft(profile, anchorIndex, step.后挡位置);
        }

        private static List<PointF> CloneCurrentProfile(IReadOnlyList<PointF> source)
        {
            List<PointF> profile = new List<PointF>(source.Count);
            for (int i = 0; i < source.Count; i++)
                profile.Add(source[i]);
            return profile;
        }

        private static int GetValidAnchorIndex(int anchorIndex, int pointCount)
        {
            if (pointCount <= 0)
                return 0;
            if (anchorIndex < 0)
                return 0;
            if (anchorIndex >= pointCount)
                return pointCount - 1;
            return anchorIndex;
        }

        private static double CalcBackGaugeByCurrentProfile(List<PointF> profile, SemiAutoType step, int anchorIndex)
        {
            if (profile.Count <= 0)
                return 0;

            bool flatIsLeftSide = IsFlatSideLeft(profile, anchorIndex, step.后挡位置);
            PointF endPoint = flatIsLeftSide ? profile[0] : profile[profile.Count - 1];
            if (step.行动类型 == SemiAutoActionFold
                || step.行动类型 == SemiAutoActionSquash
                || step.行动类型 == SemiAutoActionOpenSquash)
            {
                int neighborIndex = flatIsLeftSide
                    ? Math.Max(anchorIndex - 1, 0)
                    : Math.Min(anchorIndex + 1, profile.Count - 1);
                float contactX = Math.Min(profile[anchorIndex].X, profile[neighborIndex].X);
                return RoundBackGaugePosition(Math.Abs(endPoint.X - contactX));
            }

            PointF anchorPoint = profile[anchorIndex];
            return RoundBackGaugePosition(Math.Abs(endPoint.X - anchorPoint.X));
        }

        private static double CalcStepBackGaugePosition(SemiAutoType step, double baseBackGauge)
        {
            double target = baseBackGauge;
            if (step.行动类型 == SemiAutoActionSquash)
            {
                target += (step.折弯方向 == 0) ? Hmi_rArray[115] : Hmi_rArray[116];
            }
            else if (step.行动类型 == SemiAutoActionOpenSquash)
            {
                target += (step.折弯方向 == 0) ? Hmi_rArray[118] : Hmi_rArray[119];
            }

            return RoundBackGaugePosition(target);
        }

        private static void ApplyGeometryStep(List<PointF> profile, SemiAutoType step, int anchorIndex)
        {
            if (step.行动类型 == SemiAutoActionFold)
            {
                double foldAngle = IsLegacySemiAutoPlaceholder(step) ? 30.0 : step.折弯角度;
                double rotateAngle = (step.折弯方向 == 0) ? (180.0 - foldAngle) : (180.0 + foldAngle);
                bool rotateLeftSide = IsFlatSideLeft(profile, anchorIndex, step.后挡位置);
                RotateProfileSide(profile, anchorIndex, rotateAngle, rotateLeftSide);
            }
            else if (step.行动类型 == SemiAutoActionFlip)
            {
                FlipProfile(profile);
            }
        }

        private static void RotateProfileSide(List<PointF> profile, int anchorIndex, double angle, bool rotateLeftSide)
        {
            if (profile.Count <= 0)
                return;

            PointF center = profile[anchorIndex];
            if (rotateLeftSide)
            {
                for (int i = 0; i < anchorIndex; i++)
                    profile[i] = RotatePoint(center, profile[i], angle);
            }
            else
            {
                for (int i = anchorIndex + 1; i < profile.Count; i++)
                    profile[i] = RotatePoint(center, profile[i], angle);
            }
        }

        private static PointF RotatePoint(PointF center, PointF point, double angle)
        {
            double radians = angle * Math.PI / 180.0;
            double x = (point.X - center.X) * Math.Cos(radians) + (point.Y - center.Y) * Math.Sin(radians) + center.X;
            double y = -(point.X - center.X) * Math.Sin(radians) + (point.Y - center.Y) * Math.Cos(radians) + center.Y;
            return new PointF((float)x, (float)y);
        }

        private static bool ShouldFlipAfterStep(IReadOnlyList<SemiAutoType> steps, int currentIndex)
        {
            if (currentIndex < 0 || currentIndex >= steps.Count - 1)
                return false;
            if (steps[currentIndex].行动类型 == SemiAutoActionFlip)
                return false;

            return steps[currentIndex].内外选择 != steps[currentIndex + 1].内外选择;
        }

        private static bool IsFlatSideLeft(IReadOnlyList<PointF> profile, int pivotIndex, double targetFlatLength)
        {
            float leftLength = GetSidePathLength(profile, pivotIndex, true);
            float rightLength = GetSidePathLength(profile, pivotIndex, false);

            double leftDelta = Math.Abs(leftLength - targetFlatLength);
            double rightDelta = Math.Abs(rightLength - targetFlatLength);
            if (Math.Abs(leftDelta - rightDelta) < 0.01)
                return leftLength >= rightLength;

            return leftDelta <= rightDelta;
        }

        private static float GetSidePathLength(IReadOnlyList<PointF> profile, int pivotIndex, bool leftSide)
        {
            if (profile.Count <= 1)
                return 0f;

            float length = 0f;
            if (leftSide)
            {
                for (int i = pivotIndex; i > 0; i--)
                    length += GetSegmentLength(profile[i], profile[i - 1]);
            }
            else
            {
                for (int i = pivotIndex; i < profile.Count - 1; i++)
                    length += GetSegmentLength(profile[i], profile[i + 1]);
            }

            return length;
        }

        private static float GetSegmentLength(PointF p1, PointF p2)
        {
            return (float)Math.Sqrt(
                Math.Pow(p2.X - p1.X, 2) +
                Math.Pow(p2.Y - p1.Y, 2));
        }

        private static void FlipProfile(List<PointF> profile)
        {
            if (profile.Count <= 0)
                return;

            float minX = profile[0].X, maxX = profile[0].X;
            float minY = profile[0].Y, maxY = profile[0].Y;
            for (int i = 1; i < profile.Count; i++)
            {
                PointF p = profile[i];
                if (p.X < minX) minX = p.X;
                if (p.X > maxX) maxX = p.X;
                if (p.Y < minY) minY = p.Y;
                if (p.Y > maxY) maxY = p.Y;
            }

            PointF center = new PointF((minX + maxX) / 2f, (minY + maxY) / 2f);
            for (int i = 0; i < profile.Count; i++)
                profile[i] = RotatePoint(center, profile[i], 180.0);
        }

        private bool TryBuildInlineSlitSequence(
            List<SemiAutoType> baseSteps,
            InlineSlitPlan plan,
            out List<SemiAutoType> sequence)
        {
            sequence = new List<SemiAutoType>();
            if (baseSteps.Count <= 0)
            {
                return false;
            }

            SemiAutoType firstBaseStep = baseSteps[0];
            if (plan.OffcutFirst && plan.OffcutWidth > InlineSlitWidthTolerance)
            {
                double remainingWidthAfterOffcut = 0;
                for (int i = 0; i < plan.ProductWidths.Count; i++)
                    remainingWidthAfterOffcut += plan.ProductWidths[i];

                sequence.Add(CreateInlineSlitStep(
                    remainingWidthAfterOffcut,
                    firstBaseStep,
                    firstBaseStep,
                    firstBaseStep.坐标序号));
            }

            for (int i = 0; i < plan.ProductWidths.Count; i++)
            {
                double futureWidthOffset = 0;
                for (int j = i + 1; j < plan.ProductWidths.Count; j++)
                    futureWidthOffset += plan.ProductWidths[j];

                foreach (SemiAutoType step in baseSteps)
                {
                    sequence.Add(CloneSemiAutoStep(step, futureWidthOffset));
                }

                if (i < plan.ProductWidths.Count - 1)
                {
                    SemiAutoType previousStep = sequence[sequence.Count - 1];
                    sequence.Add(CreateInlineSlitStep(
                        futureWidthOffset,
                        previousStep,
                        firstBaseStep,
                        previousStep.坐标序号));
                }
            }

            return sequence.Count > 0;
        }

        private bool TryGetInlineSlitPlan(out InlineSlitPlan plan)
        {
            plan = default;

            if (!CurtOrder.边做边分切启用 || CurtOrder.isTaper)
            {
                return false;
            }

            if (!TryGetInlineSlitPieceCount(CurtOrder, out int pieceCount, out double frontOffcutWidth))
            {
                return false;
            }

            List<double> productWidths = new List<double>();
            for (int i = 0; i < pieceCount; i++)
                productWidths.Add(CurtOrder.Width);

            plan = new InlineSlitPlan(frontOffcutWidth, productWidths, true);
            return true;
        }

        public static bool TryGetInlineSlitPieceCount(OrderType order, out int pieceCount, out double frontOffcutWidth)
        {
            pieceCount = 0;
            frontOffcutWidth = 0;

            if (!order.边做边分切启用 || order.Width <= InlineSlitWidthTolerance)
                return false;

            if (order.边做边分切块数 <= 1)
                return false;

            if (order.边做边分切整板宽 <= order.Width)
                return false;

            pieceCount = order.边做边分切块数;
            frontOffcutWidth = order.边做边分切整板宽 - order.Width * pieceCount;
            if (frontOffcutWidth < -InlineSlitWidthTolerance)
                return false;

            if (Math.Abs(frontOffcutWidth) <= InlineSlitWidthTolerance)
                frontOffcutWidth = 0;

            return true;
        }

        public static string SerializeInlineSlitReserve(OrderType order)
        {
            string totalWidth = order.边做边分切整板宽.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
            return $"{(order.边做边分切启用 ? 1 : 0)}/{totalWidth}/{order.边做边分切块数}";
        }

        public static void DeserializeInlineSlitReserve(string raw, ref OrderType order)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return;

            string[] parts = raw.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3)
                return;

            if (parts[0] == "1")
                order.边做边分切启用 = true;

            if (double.TryParse(parts[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double totalWidth))
                order.边做边分切整板宽 = totalWidth;

            if (int.TryParse(parts[2], out int pieceCount))
            {
                order.边做边分切块数 = pieceCount;
                order.边做边分切前废料 = Math.Max(0, order.边做边分切整板宽 - order.Width * pieceCount);
                return;
            }

            if (double.TryParse(parts[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double offcutWidth))
            {
                order.边做边分切前废料 = offcutWidth;
                if (order.Width > InlineSlitWidthTolerance)
                {
                    int legacyCount = (int)Math.Floor((order.边做边分切整板宽 - offcutWidth + InlineSlitWidthTolerance) / order.Width);
                    if (legacyCount > 1)
                        order.边做边分切块数 = legacyCount;
                }
            }
        }

        private static SemiAutoType CloneSemiAutoStep(SemiAutoType step, double backGaugeOffset)
        {
            step.后挡位置 = RoundBackGaugePosition(step.后挡位置 + backGaugeOffset);
            return step;
        }

        private static SemiAutoType CreateInlineSlitStep(
            double slitWidth,
            SemiAutoType previousStep,
            SemiAutoType nextStep,
            int anchorIndex)
        {
            return new SemiAutoType
            {
                行动类型 = SemiAutoActionSlit,
                折弯方向 = nextStep.折弯方向,
                折弯角度 = 888.0,
                回弹值 = 4.0,
                后挡位置 = RoundBackGaugePosition(slitWidth),
                抓取类型 = previousStep.抓取类型,
                松开高度 = 0,
                翻板收缩值 = 3,
                长角序号 = previousStep.长角序号,
                坐标序号 = anchorIndex,
                重新抓取 = 0,
                is色下 = nextStep.is色下,
                锥度斜率 = 0,
                操作提示 = 0,
                内外选择 = nextStep.内外选择,
            };
        }
    }
}
