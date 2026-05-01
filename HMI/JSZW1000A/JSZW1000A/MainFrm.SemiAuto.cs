namespace JSZW1000A
{
    public partial class MainFrm
    {
        public const int SemiAutoActionFold = 0;
        public const int SemiAutoActionSquash = 1;
        public const int SemiAutoActionOpenSquash = 2;
        public const int SemiAutoActionSlit = 3;
        public const int SemiAutoActionFlip = 8;
        public const string SemiAutoPlanOriginGeneratedSelected = "generated-selected";
        public const string SemiAutoPlanOriginCustomManual = "custom-manual";

        public readonly struct SemiAutoPlanValidationResult
        {
            public SemiAutoPlanValidationResult(bool isAccepted, bool requiresConfirmation, string message)
            {
                IsAccepted = isAccepted;
                RequiresConfirmation = requiresConfirmation;
                Message = message;
            }

            public bool IsAccepted { get; }
            public bool RequiresConfirmation { get; }
            public string Message { get; }
        }

        public int CurrentSemiAutoPlanIndex { get; private set; }
        public int CurrentSemiAutoPlanCount { get; private set; }
        public double CurrentSemiAutoPlanScore { get; private set; }
        private readonly List<FoldSequenceCandidate> previewableSemiAutoCandidates = new();
        private int currentPreviewableSemiAutoCandidateIndex = -1;
        private bool isPreviewCandidateBrowsingActive = false;
        private bool hasConfirmedSentPlan = false;

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
                IsTaper = false;
                IsSlitter = false;
                IsInlineSlitEnabled = false;
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
                string decisionSummary,
                string failureCode,
                string failureMessage)
            {
                Success = success;
                StrategyName = strategyName;
                Steps = steps;
                DecisionSummary = decisionSummary;
                FailureCode = failureCode;
                FailureMessage = failureMessage;
            }

            public bool Success { get; }
            public string StrategyName { get; }
            public List<SemiAutoType> Steps { get; }
            public string DecisionSummary { get; }
            public string FailureCode { get; }
            public string FailureMessage { get; }

            public static SemiAutoGenerationResult Ok(string strategyName, List<SemiAutoType> steps, string decisionSummary = "")
            {
                return new SemiAutoGenerationResult(true, strategyName, steps, decisionSummary, "", "");
            }

            public static SemiAutoGenerationResult Fail(string strategyName, string failureCode, string failureMessage)
            {
                return new SemiAutoGenerationResult(false, strategyName, new List<SemiAutoType>(), "", failureCode, failureMessage);
            }
        }

        private readonly struct FoldShapeProfile
        {
            public FoldShapeProfile(OrderType order, SemiAutoGenerationContext context)
            {
                FoldCount = 0;
                PositiveAngleCount = 0;
                NegativeAngleCount = 0;
                MaxFoldAngle = 0;
                for (int i = 1; i < order.lengAngle.Length - 1; i++)
                {
                    if (order.lengAngle[i].Length <= 0)
                        continue;

                    FoldCount++;
                    if (order.lengAngle[i].Angle > 0)
                        PositiveAngleCount++;
                    else if (order.lengAngle[i].Angle < 0)
                        NegativeAngleCount++;

                    MaxFoldAngle = Math.Max(MaxFoldAngle, Math.Abs(order.lengAngle[i].Angle));
                }

                Width = order.Width > 0 ? order.Width : CalculateOrderWidth(order.lengAngle);
                Thickness = order.Thickness;
                HasHeadSquash = context.HasHeadSquash;
                HasTailSquash = context.HasTailSquash;
                IsTaper = context.IsTaper;
                IsSlitter = context.IsSlitter;
                IsInlineSlitEnabled = context.IsInlineSlitEnabled;
                ShapeKey = string.Join(
                    "|",
                    FoldCount,
                    PositiveAngleCount,
                    NegativeAngleCount,
                    HasHeadSquash ? 1 : 0,
                    HasTailSquash ? 1 : 0,
                    IsTaper ? 1 : 0,
                    IsSlitter ? 1 : 0,
                    Math.Round(Width, 1),
                    Math.Round(Thickness, 2));
            }

            public int FoldCount { get; }
            public int PositiveAngleCount { get; }
            public int NegativeAngleCount { get; }
            public double MaxFoldAngle { get; }
            public double Width { get; }
            public double Thickness { get; }
            public bool HasHeadSquash { get; }
            public bool HasTailSquash { get; }
            public bool IsTaper { get; }
            public bool IsSlitter { get; }
            public bool IsInlineSlitEnabled { get; }
            public bool HasMixedFoldDirections => PositiveAngleCount > 0 && NegativeAngleCount > 0;
            public string ShapeKey { get; }
        }

        private readonly struct FoldStepSnapshot
        {
            public FoldStepSnapshot(
                int stepIndex,
                int actionType,
                bool isColorDown,
                int innerOuter,
                int coordinateIndex,
                double backGauge,
                bool hasFlipAfterStep,
                bool hasRegrip,
                bool hasShortGripRisk,
                double maxProfileHeight,
                double clearanceMargin)
            {
                StepIndex = stepIndex;
                ActionType = actionType;
                IsColorDown = isColorDown;
                InnerOuter = innerOuter;
                CoordinateIndex = coordinateIndex;
                BackGauge = backGauge;
                HasFlipAfterStep = hasFlipAfterStep;
                HasRegrip = hasRegrip;
                HasShortGripRisk = hasShortGripRisk;
                MaxProfileHeight = maxProfileHeight;
                ClearanceMargin = clearanceMargin;
            }

            public int StepIndex { get; }
            public int ActionType { get; }
            public bool IsColorDown { get; }
            public int InnerOuter { get; }
            public int CoordinateIndex { get; }
            public double BackGauge { get; }
            public bool HasFlipAfterStep { get; }
            public bool HasRegrip { get; }
            public bool HasShortGripRisk { get; }
            public double MaxProfileHeight { get; }
            public double ClearanceMargin { get; }
        }

        private readonly struct FoldCollisionEvaluation
        {
            public FoldCollisionEvaluation(
                string primaryCode,
                bool hardReject,
                double scoreDelta,
                double margin,
                bool retrySuggested,
                int hardRejectCount,
                int softPenaltyCount,
                int nearCollisionCount,
                double penalty,
                double minClearanceMargin,
                string summary)
            {
                PrimaryCode = primaryCode;
                HardReject = hardReject;
                ScoreDelta = scoreDelta;
                Margin = margin;
                RetrySuggested = retrySuggested;
                HardRejectCount = hardRejectCount;
                SoftPenaltyCount = softPenaltyCount;
                NearCollisionCount = nearCollisionCount;
                Penalty = penalty;
                MinClearanceMargin = minClearanceMargin;
                Summary = summary;
            }

            public string PrimaryCode { get; }
            public bool HardReject { get; }
            public double ScoreDelta { get; }
            public double Margin { get; }
            public bool RetrySuggested { get; }
            public int HardRejectCount { get; }
            public int SoftPenaltyCount { get; }
            public int NearCollisionCount { get; }
            public double Penalty { get; }
            public double MinClearanceMargin { get; }
            public string Summary { get; }
        }

        private readonly struct FoldSecondaryMarginEvaluation
        {
            public FoldSecondaryMarginEvaluation(int count, double margin, double penalty)
            {
                Count = count;
                Margin = margin;
                Penalty = penalty;
            }

            public int Count { get; }
            public double Margin { get; }
            public double Penalty { get; }
            public bool HasMargin => Count > 0 && Margin > 0;
        }

        private readonly struct FoldDiagnosticMessage
        {
            public FoldDiagnosticMessage(string severity, string reason, string action)
            {
                Severity = severity;
                Reason = reason;
                Action = action;
            }

            public string Severity { get; }
            public string Reason { get; }
            public string Action { get; }

            public string ToUserText()
            {
                if (string.IsNullOrWhiteSpace(Reason))
                    return "";

                return $"{Severity}: {Reason} 建议: {Action}";
            }
        }

        private readonly struct FoldScoreBreakdown
        {
            public FoldScoreBreakdown(
                double feasibilityPenalty,
                double strategyPenalty,
                double flipPenalty,
                double regripPenalty,
                double shortGripPenalty,
                double backGaugeTravelPenalty,
                double backGaugeTravel,
                double habitScore,
                double manualPreferenceScore)
            {
                FeasibilityPenalty = feasibilityPenalty;
                StrategyPenalty = strategyPenalty;
                FlipPenalty = flipPenalty;
                RegripPenalty = regripPenalty;
                ShortGripPenalty = shortGripPenalty;
                BackGaugeTravelPenalty = backGaugeTravelPenalty;
                BackGaugeTravel = backGaugeTravel;
                HabitScore = habitScore;
                ManualPreferenceScore = manualPreferenceScore;
                TotalPenalty = feasibilityPenalty
                    + strategyPenalty
                    + flipPenalty
                    + regripPenalty
                    + shortGripPenalty
                    + backGaugeTravelPenalty
                    - habitScore
                    - manualPreferenceScore;
            }

            public double FeasibilityPenalty { get; }
            public double StrategyPenalty { get; }
            public double FlipPenalty { get; }
            public double RegripPenalty { get; }
            public double ShortGripPenalty { get; }
            public double BackGaugeTravelPenalty { get; }
            public double BackGaugeTravel { get; }
            public double HabitScore { get; }
            public double ManualPreferenceScore { get; }
            public double TotalPenalty { get; }
            public double MotionPenalty => StrategyPenalty + FlipPenalty + RegripPenalty + ShortGripPenalty + BackGaugeTravelPenalty;
        }

        private sealed class FoldManualPreferenceSample
        {
            public FoldManualPreferenceSample(string signature)
            {
                Signature = signature;
                HitCount = 1;
            }

            public string Signature { get; }
            public int HitCount { get; private set; }

            public void Increment()
            {
                if (HitCount < 1000)
                    HitCount++;
            }
        }

        private sealed class FoldSequenceCandidate
        {
            public FoldSequenceCandidate(
                string strategyName,
                bool isReverse,
                bool isColorDown,
                FoldShapeProfile shape,
                List<SemiAutoType> steps)
            {
                StrategyName = strategyName;
                IsReverse = isReverse;
                IsColorDown = isColorDown;
                Shape = shape;
                Steps = steps;
                FailureCode = "";
                FailureMessage = "";
                CollisionSummary = "";
                DecisionSummary = "";
                Snapshots = new List<FoldStepSnapshot>();
            }

            public string StrategyName { get; }
            public bool IsReverse { get; }
            public bool IsColorDown { get; }
            public FoldShapeProfile Shape { get; }
            public List<SemiAutoType> Steps { get; }
            public bool IsFeasible { get; private set; }
            public string FailureCode { get; private set; }
            public string FailureMessage { get; private set; }
            public string CollisionSummary { get; private set; }
            public string DecisionSummary { get; private set; }
            public double ScorePenalty { get; private set; }
            public double HabitScore { get; private set; }
            public double ManualPreferenceScore { get; private set; }
            public List<FoldStepSnapshot> Snapshots { get; private set; }

            public void Reject(string failureCode, string failureMessage)
            {
                Reject(failureCode, failureMessage, "");
            }

            public void Reject(string failureCode, string failureMessage, string decisionSummary)
            {
                IsFeasible = false;
                FailureCode = failureCode;
                FailureMessage = failureMessage;
                DecisionSummary = string.IsNullOrWhiteSpace(decisionSummary)
                    ? MainFrm.BuildRejectedCandidateSummary(this, failureCode, failureMessage)
                    : decisionSummary;
            }

            public void Accept(
                FoldScoreBreakdown scoreBreakdown,
                FoldCollisionEvaluation collision,
                List<FoldStepSnapshot> snapshots)
            {
                Accept(scoreBreakdown, collision, snapshots, "");
            }

            public void Accept(
                FoldScoreBreakdown scoreBreakdown,
                FoldCollisionEvaluation collision,
                List<FoldStepSnapshot> snapshots,
                string decisionSummary)
            {
                IsFeasible = true;
                ScorePenalty = scoreBreakdown.TotalPenalty;
                HabitScore = scoreBreakdown.HabitScore;
                ManualPreferenceScore = scoreBreakdown.ManualPreferenceScore;
                CollisionSummary = collision.Summary;
                Snapshots = snapshots;
                DecisionSummary = string.IsNullOrWhiteSpace(decisionSummary)
                    ? MainFrm.BuildAcceptedCandidateSummary(this, snapshots, collision, scoreBreakdown)
                    : decisionSummary;
            }
        }

        private static readonly Dictionary<string, FoldManualPreferenceSample> FoldManualPreferenceSamples = new(StringComparer.Ordinal);

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

        public List<SemiAutoType> BuildExecutionSemiAutoSteps()
        {
            List<SemiAutoType> baseSteps = new List<SemiAutoType>(CurtOrder.lstSemiAuto);
            if (baseSteps.Count <= 0)
                return baseSteps;

            List<SemiAutoType> executionSteps = new List<SemiAutoType>(baseSteps);

            if (TryGetInlineSlitPlan(out InlineSlitPlan inlinePlan))
            {
                if (TryBuildInlineSlitSequence(baseSteps, inlinePlan, out List<SemiAutoType> inlineSequence))
                    executionSteps = inlineSequence;
            }
            else if (CurtOrder.isSlitter || CurtOrder.isTaper)
            {
                executionSteps.Insert(0, CreateLeadingSlitExecutionStep(baseSteps[0]));
            }

            ApplyExecutionTaperData(executionSteps);
            NormalizeSemiAutoStepSequence(executionSteps);
            return executionSteps;
        }

        private SemiAutoType CreateLeadingSlitExecutionStep(SemiAutoType firstStep)
        {
            return new SemiAutoType
            {
                行动类型 = SemiAutoActionSlit,
                折弯方向 = 0,
                折弯角度 = 888.0,
                回弹值 = 4.0,
                后挡位置 = RoundBackGaugePosition(CurtOrder.Width),
                抓取类型 = 1,
                松开高度 = 0,
                翻板收缩值 = GetDefaultSemiAutoRetractValue(),
                重新抓取 = 0,
                is色下 = firstStep.is色下,
                锥度斜率 = 0,
                操作提示 = 0,
                内外选择 = firstStep.内外选择
            };
        }

        private void ApplyExecutionTaperData(List<SemiAutoType> steps)
        {
            if (steps.Count <= 0)
                return;

            if (!CurtOrder.isTaper || CurtOrder.TaperLength == 0)
            {
                for (int i = 0; i < steps.Count; i++)
                {
                    SemiAutoType step = steps[i];
                    step.锥度斜率 = 0;
                    steps[i] = step;
                }
                return;
            }

            double remainingLength = CurtOrder.Width;
            double remainingTaperWidth = CalculateRemainingTaperWidth(CurtOrder);
            HashSet<int> consumedLongAngles = new HashSet<int>();

            for (int i = 0; i < steps.Count; i++)
            {
                SemiAutoType step = steps[i];
                int longAngleIndex = step.长角序号;
                if (longAngleIndex >= 0
                    && longAngleIndex < CurtOrder.lengAngle.Length
                    && !consumedLongAngles.Contains(longAngleIndex))
                {
                    LengAngle longAngle = CurtOrder.lengAngle[longAngleIndex];
                    bool canConsume = longAngleIndex == 0 || longAngleIndex == 99
                        ? longAngle.Length > 0 && longAngle.Angle > 0
                        : longAngle.Length > 0;
                    if (canConsume)
                    {
                        remainingLength -= longAngle.Length;
                        remainingTaperWidth -= longAngle.TaperWidth;
                        consumedLongAngles.Add(longAngleIndex);
                    }
                }

                step.锥度斜率 = (CurtOrder.TaperLength == 0)
                    ? 0
                    : (remainingTaperWidth - remainingLength) / CurtOrder.TaperLength * 100000;
                steps[i] = step;
            }
        }

        private static double CalculateRemainingTaperWidth(OrderType order)
        {
            double total = 0;
            for (int i = 0; i < order.lengAngle.Length; i++)
            {
                bool isHeadOrTail = i == 0 || i == order.lengAngle.Length - 1;
                if (isHeadOrTail)
                {
                    if (order.lengAngle[i].Length > 0 && order.lengAngle[i].Angle > 0)
                        total += order.lengAngle[i].TaperWidth;
                }
                else if (order.lengAngle[i].Length > 0)
                {
                    total += order.lengAngle[i].TaperWidth;
                }
            }

            return total;
        }

        private static void NormalizeSemiAutoStepSequence(List<SemiAutoType> steps)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                SemiAutoType step = steps[i];
                step.折弯序号 = i + 1;
                steps[i] = step;
            }
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

        private List<FoldSequenceCandidate> BuildStandardFoldCandidates(SemiAutoGenerationContext context)
        {
            FoldShapeProfile shape = new FoldShapeProfile(CurtOrder, context);
            List<FoldSequenceCandidate> candidates = new List<FoldSequenceCandidate>();
            HashSet<string> seen = new HashSet<string>(StringComparer.Ordinal);

            AddStandardFoldCandidate(candidates, seen, shape, "StandardFoldStrategy", context.IsReverse, context.IsColorDown);
            AddStandardFoldCandidate(candidates, seen, shape, "OppositeOrderStrategy", !context.IsReverse, context.IsColorDown);
            AddStandardFoldCandidate(candidates, seen, shape, "OppositeColorSideStrategy", context.IsReverse, !context.IsColorDown);
            AddStandardFoldCandidate(candidates, seen, shape, "OppositeOrderColorSideStrategy", !context.IsReverse, !context.IsColorDown);

            return candidates;
        }

        private void SetSemiAutoPlanSummary(int index, int count, double score)
        {
            CurrentSemiAutoPlanIndex = Math.Max(0, index);
            CurrentSemiAutoPlanCount = Math.Max(0, count);
            CurrentSemiAutoPlanScore = score;
            isPreviewCandidateBrowsingActive = count > 0;
            hasConfirmedSentPlan = false;
        }

        private void ClearSemiAutoPlanSummary()
        {
            CurrentSemiAutoPlanIndex = 0;
            CurrentSemiAutoPlanCount = 0;
            CurrentSemiAutoPlanScore = 0;
            previewableSemiAutoCandidates.Clear();
            currentPreviewableSemiAutoCandidateIndex = -1;
            isPreviewCandidateBrowsingActive = false;
            hasConfirmedSentPlan = false;
        }

        private void SetPreviewableSemiAutoCandidates(IReadOnlyList<FoldSequenceCandidate> candidates, FoldSequenceCandidate selectedCandidate)
        {
            previewableSemiAutoCandidates.Clear();
            previewableSemiAutoCandidates.AddRange(candidates);
            currentPreviewableSemiAutoCandidateIndex = previewableSemiAutoCandidates.FindIndex(candidate => ReferenceEquals(candidate, selectedCandidate));
            if (currentPreviewableSemiAutoCandidateIndex < 0)
                currentPreviewableSemiAutoCandidateIndex = 0;
            isPreviewCandidateBrowsingActive = previewableSemiAutoCandidates.Count > 0;
            hasConfirmedSentPlan = false;
        }

        public bool CanPreviewNextSemiAutoPlan()
        {
            return !HasManualSemiAutoEdits() && isPreviewCandidateBrowsingActive && previewableSemiAutoCandidates.Count > 1;
        }

        public bool TryApplyPreviewPreferences()
        {
            if (HasManualSemiAutoEdits() || !isPreviewCandidateBrowsingActive || previewableSemiAutoCandidates.Count <= 0)
                return false;

            List<FoldSequenceCandidate> rankedCandidates = RankPreviewableSemiAutoCandidates(
                previewableSemiAutoCandidates,
                CurtOrder.st逆序,
                CurtOrder.st色下);
            if (rankedCandidates.Count <= 0)
                return false;

            previewableSemiAutoCandidates.Clear();
            previewableSemiAutoCandidates.AddRange(rankedCandidates);
            currentPreviewableSemiAutoCandidateIndex = 0;

            FoldSequenceCandidate candidate = previewableSemiAutoCandidates[0];
            CurtOrder.lstSemiAuto = new List<SemiAutoType>(candidate.Steps);
            MarkCurrentPlanAsGeneratedSelected();
            NormalizeGeneratedSemiAutoSequence();
            RebuildSemiAutoDerivedState(ref CurtOrder);
            SetSemiAutoPlanSummary(1, previewableSemiAutoCandidates.Count, candidate.ScorePenalty);
            return true;
        }

        public bool TryPreviewNextSemiAutoPlan()
        {
            if (!CanPreviewNextSemiAutoPlan())
                return false;

            currentPreviewableSemiAutoCandidateIndex++;
            if (currentPreviewableSemiAutoCandidateIndex >= previewableSemiAutoCandidates.Count)
                currentPreviewableSemiAutoCandidateIndex = 0;

            FoldSequenceCandidate candidate = previewableSemiAutoCandidates[currentPreviewableSemiAutoCandidateIndex];
            CurtOrder.lstSemiAuto = new List<SemiAutoType>(candidate.Steps);
            MarkCurrentPlanAsGeneratedSelected();
            NormalizeGeneratedSemiAutoSequence();
            RebuildSemiAutoDerivedState(ref CurtOrder);
            SetSemiAutoPlanSummary(
                currentPreviewableSemiAutoCandidateIndex + 1,
                previewableSemiAutoCandidates.Count,
                candidate.ScorePenalty);
            return true;
        }

        public void ConfirmCurrentSemiAutoPlanSent()
        {
            hasConfirmedSentPlan = true;
            isPreviewCandidateBrowsingActive = false;
            previewableSemiAutoCandidates.Clear();
            currentPreviewableSemiAutoCandidateIndex = -1;
        }

        public string GetCurrentFormalPlanSummaryText()
        {
            if (isPreviewCandidateBrowsingActive && CurrentSemiAutoPlanCount > 0)
                return $"当前{CurrentSemiAutoPlanIndex}/{CurrentSemiAutoPlanCount}  RP:{CurrentSemiAutoPlanScore:0.###}";

            if (hasConfirmedSentPlan)
                return MainFrm.Lang == 0 ? "当前确认方案" : "Confirmed plan";

            if (CurtOrder.lstSemiAuto.Count <= 0)
                return string.Empty;

            if (CurtOrder.SemiAutoPlanOrigin == SemiAutoPlanOriginCustomManual)
                return MainFrm.Lang == 0 ? "自定义方案" : "Custom plan";

            return MainFrm.Lang == 0 ? "已保存方案" : "Saved plan";
        }

        public void MarkCurrentPlanAsGeneratedSelected()
        {
            CurtOrder.SemiAutoPlanOrigin = SemiAutoPlanOriginGeneratedSelected;
        }

        public void MarkCurrentPlanAsCustomManual()
        {
            CurtOrder.SemiAutoPlanOrigin = SemiAutoPlanOriginCustomManual;
            isPreviewCandidateBrowsingActive = false;
            previewableSemiAutoCandidates.Clear();
            currentPreviewableSemiAutoCandidateIndex = -1;
        }

        public void MarkLoadedFormalPlan()
        {
            CurrentSemiAutoPlanIndex = 0;
            CurrentSemiAutoPlanCount = 0;
            CurrentSemiAutoPlanScore = 0;
            isPreviewCandidateBrowsingActive = false;
            hasConfirmedSentPlan = false;
            previewableSemiAutoCandidates.Clear();
            currentPreviewableSemiAutoCandidateIndex = -1;
        }

        public SemiAutoPlanValidationResult ValidateCurrentFormalSemiAutoPlan(IReadOnlyList<SemiAutoType> steps)
        {
            if (steps.Count <= 0)
                return new SemiAutoPlanValidationResult(false, false, "当前没有可确认的折弯步骤。");

            SemiAutoGenerationContext context = new SemiAutoGenerationContext(CurtOrder);
            FoldShapeProfile shape = new FoldShapeProfile(CurtOrder, context);
            FoldSequenceCandidate candidate = new FoldSequenceCandidate(
                "LayoutConfirmation",
                CurtOrder.st逆序,
                CurtOrder.st色下,
                shape,
                new List<SemiAutoType>(steps));

            int maxStepCount = Hmi_iSemiAuto.Length / 10;
            if (candidate.Steps.Count > maxStepCount)
            {
                return new SemiAutoPlanValidationResult(
                    false,
                    false,
                    $"生产步骤 {candidate.Steps.Count} 超过 PLC 半自动表容量 {maxStepCount}。");
            }

            for (int i = 0; i < candidate.Steps.Count; i++)
            {
                if (!TryValidateSemiAutoStepForCandidate(candidate.Steps[i], i, candidate.Shape, out string failureCode, out string failureMessage))
                {
                    return new SemiAutoPlanValidationResult(
                        false,
                        false,
                        $"{failureCode}: {failureMessage}");
                }
            }

            FoldCollisionEvaluation collision = EvaluateFoldCollision(candidate.Steps, candidate.Shape, out List<FoldStepSnapshot> snapshots);
            if (collision.HardReject)
            {
                return new SemiAutoPlanValidationResult(
                    false,
                    false,
                    BuildCollisionRejectedCandidateSummary(candidate, collision));
            }

            if (collision.PrimaryCode != "Ok")
            {
                double habitScore = CalculateFoldHabitScore(candidate, snapshots);
                double manualPreferenceScore = CalculateManualPreferenceScore(candidate);
                FoldScoreBreakdown scoreBreakdown = CalculateFoldScoreBreakdown(candidate, snapshots, collision, habitScore, manualPreferenceScore);
                return new SemiAutoPlanValidationResult(
                    true,
                    true,
                    BuildAcceptedCandidateSummary(candidate, snapshots, collision, scoreBreakdown));
            }

            return new SemiAutoPlanValidationResult(true, false, "当前布置方案验证通过。");
        }

        public static string SerializeSemiAutoPlanReserve(OrderType order)
        {
            string origin = string.IsNullOrWhiteSpace(order.SemiAutoPlanOrigin)
                ? SemiAutoPlanOriginGeneratedSelected
                : order.SemiAutoPlanOrigin;
            return $"{origin}/{(order.st逆序 ? 1 : 0)}/{(order.st色下 ? 1 : 0)}";
        }

        public static void DeserializeSemiAutoPlanReserve(string raw, ref OrderType order)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return;

            string[] parts = raw.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
                order.SemiAutoPlanOrigin = parts[0].Trim();
            if (parts.Length > 1)
                order.st逆序 = parts[1].Trim() == "1";
            if (parts.Length > 2)
                order.st色下 = parts[2].Trim() == "1";
        }

        private void AddStandardFoldCandidate(
            List<FoldSequenceCandidate> candidates,
            HashSet<string> seen,
            FoldShapeProfile shape,
            string strategyName,
            bool isReverse,
            bool isColorDown)
        {
            List<SemiAutoType> steps = BuildStandardStepsForOptions(isReverse, isColorDown);
            FoldSequenceCandidate candidate = new FoldSequenceCandidate(strategyName, isReverse, isColorDown, shape, steps);
            AddFoldSequenceCandidate(candidates, seen, candidate);
        }

        private void AddFoldSequenceCandidate(
            List<FoldSequenceCandidate> candidates,
            HashSet<string> seen,
            FoldSequenceCandidate candidate)
        {
            if (!seen.Add(BuildFoldSequenceSignature(candidate.Steps)))
                return;

            EvaluateFoldSequenceCandidate(candidate);
            candidates.Add(candidate);
        }

        private static int CalculatePreviewPreferenceDistance(
            FoldSequenceCandidate candidate,
            bool preferredReverse,
            bool preferredColorDown)
        {
            int distance = 0;
            if (candidate.IsReverse != preferredReverse)
                distance++;
            if (candidate.IsColorDown != preferredColorDown)
                distance++;
            return distance;
        }

        private static List<FoldSequenceCandidate> RankPreviewableSemiAutoCandidates(
            IEnumerable<FoldSequenceCandidate> candidates,
            bool preferredReverse,
            bool preferredColorDown)
        {
            return candidates
                .Where(candidate => candidate.IsFeasible)
                .OrderBy(candidate => CalculatePreviewPreferenceDistance(candidate, preferredReverse, preferredColorDown))
                .ThenBy(candidate => candidate.ScorePenalty)
                .ThenByDescending(candidate => candidate.HabitScore)
                .ToList();
        }

        private List<SemiAutoType> BuildStandardStepsForOptions(bool isReverse, bool isColorDown)
        {
            bool savedReverse = CurtOrder.st逆序;
            bool savedColorDown = CurtOrder.st色下;
            bool savedSlitter = CurtOrder.isSlitter;
            bool savedTaper = CurtOrder.isTaper;
            bool savedInlineSlitEnabled = CurtOrder.边做边分切启用;
            List<SemiAutoType> savedSteps = new List<SemiAutoType>(CurtOrder.lstSemiAuto);

            try
            {
                CurtOrder.st逆序 = isReverse;
                CurtOrder.st色下 = isColorDown;
                CurtOrder.isSlitter = false;
                CurtOrder.isTaper = false;
                CurtOrder.边做边分切启用 = false;
                create标准生产序列();
                for (int i = 0; i < CurtOrder.lstSemiAuto.Count; i++)
                {
                    SemiAutoType step = CurtOrder.lstSemiAuto[i];
                    step.锥度斜率 = 0;
                    CurtOrder.lstSemiAuto[i] = step;
                }
                return new List<SemiAutoType>(CurtOrder.lstSemiAuto);
            }
            finally
            {
                CurtOrder.st逆序 = savedReverse;
                CurtOrder.st色下 = savedColorDown;
                CurtOrder.isSlitter = savedSlitter;
                CurtOrder.isTaper = savedTaper;
                CurtOrder.边做边分切启用 = savedInlineSlitEnabled;
                CurtOrder.lstSemiAuto = savedSteps;
            }
        }

        private static string BuildFoldSequenceSignature(IReadOnlyList<SemiAutoType> steps)
        {
            return string.Join(
                ";",
                steps.Select(s => string.Join(
                    "/",
                    s.行动类型,
                    s.长角序号,
                    s.坐标序号,
                    s.内外选择,
                    s.折弯方向,
                s.抓取类型,
                Math.Round(s.后挡位置, 3))));
        }

        private static FoldSequenceCandidate? SelectBestFoldSequenceCandidate(IReadOnlyList<FoldSequenceCandidate> candidates)
        {
            FoldSequenceCandidate? best = null;
            for (int i = 0; i < candidates.Count; i++)
            {
                FoldSequenceCandidate candidate = candidates[i];
                if (!candidate.IsFeasible)
                    continue;

                if (best == null
                    || candidate.ScorePenalty < best.ScorePenalty
                    || (Math.Abs(candidate.ScorePenalty - best.ScorePenalty) < 0.001 && candidate.HabitScore > best.HabitScore))
                {
                    best = candidate;
                }
            }

            return best;
        }

        private static string BuildFoldGenerationFailureMessage(IReadOnlyList<FoldSequenceCandidate> candidates)
        {
            if (candidates.Count <= 0)
                return "未生成可评估的折弯候选序列。";

            List<string> reasons = new List<string>();
            for (int i = 0; i < candidates.Count && reasons.Count < 3; i++)
            {
                FoldSequenceCandidate candidate = candidates[i];
                if (!string.IsNullOrWhiteSpace(candidate.DecisionSummary))
                    reasons.Add(candidate.DecisionSummary);
                else if (!string.IsNullOrWhiteSpace(candidate.FailureMessage))
                    reasons.Add($"{candidate.StrategyName} [{candidate.FailureCode}]: {candidate.FailureMessage}");
            }

            return reasons.Count > 0
                ? string.Join(Environment.NewLine, reasons)
                : "所有候选序列均未通过可行性校验。";
        }

        private static string BuildRejectedCandidateSummary(
            FoldSequenceCandidate candidate,
            string failureCode,
            string failureMessage)
        {
            return $"{candidate.StrategyName} [{failureCode}]: {failureMessage}";
        }

        private static string BuildCollisionRejectedCandidateSummary(
            FoldSequenceCandidate candidate,
            FoldCollisionEvaluation collision)
        {
            FoldDiagnosticMessage diagnostic = GetFoldDiagnosticMessage(collision.PrimaryCode, collision.HardReject);
            string userText = diagnostic.ToUserText();
            if (string.IsNullOrWhiteSpace(userText))
                userText = "禁止执行: 当前折弯方案未通过机械可行性检查。建议: 检查板型、后挡位置和折弯顺序后重新生成。";

            return $"{candidate.StrategyName}: {userText} 工程: {collision.Summary}";
        }

        private static string BuildAcceptedCandidateSummary(
            FoldSequenceCandidate candidate,
            IReadOnlyList<FoldStepSnapshot> snapshots,
            FoldCollisionEvaluation collision,
            FoldScoreBreakdown scoreBreakdown)
        {
            int flipCount = snapshots.Count(s => s.HasFlipAfterStep);
            int regripCount = snapshots.Count(s => s.HasRegrip);
            int shortGripRiskCount = snapshots.Count(s => s.HasShortGripRisk);
            double minClearance = snapshots.Count > 0 ? snapshots.Min(s => s.ClearanceMargin) : 0;
            string userText = BuildAcceptedCandidateUserMessage(collision);

            return $"{candidate.StrategyName}: {userText} 工程: steps={candidate.Steps.Count}, flip={flipCount}, regrip={regripCount}, shortGripRisk={shortGripRiskCount}, soft={collision.SoftPenaltyCount}, near={collision.NearCollisionCount}, margin={collision.Margin:0.###}, minClearance={minClearance:0.###}, feasibility={scoreBreakdown.FeasibilityPenalty:0.###}, motion={scoreBreakdown.MotionPenalty:0.###}, backGaugeTravel={scoreBreakdown.BackGaugeTravel:0.###}, penalty={scoreBreakdown.TotalPenalty:0.###}, habit={scoreBreakdown.HabitScore:0.###}, manual={scoreBreakdown.ManualPreferenceScore:0.###}";
        }

        private static string BuildAcceptedCandidateUserMessage(FoldCollisionEvaluation collision)
        {
            if (collision.PrimaryCode == "Ok")
                return "已生成推荐折弯顺序。";

            FoldDiagnosticMessage diagnostic = GetFoldDiagnosticMessage(collision.PrimaryCode, collision.HardReject);
            string userText = diagnostic.ToUserText();
            if (!string.IsNullOrWhiteSpace(userText))
                return userText;

            return "已生成推荐折弯顺序，但该方案包含需复核的机械风险，系统已在评分中降低其优先级。";
        }

        private static FoldDiagnosticMessage GetFoldDiagnosticMessage(string primaryCode, bool hardReject)
        {
            switch (primaryCode)
            {
                case "Ok":
                    return new FoldDiagnosticMessage("", "", "");
                case "MirrorRetrySuggested":
                    return new FoldDiagnosticMessage(
                        "禁止执行",
                        "当前折弯方向或板面状态可能与机器侧别不匹配，继续执行存在碰撞风险。",
                        "尝试反向生成或换面生成；若仍失败，请检查当前板面、内外侧选择和折弯方向。");
                case "ClampEnvelopeHardReject":
                    return new FoldDiagnosticMessage(
                        "禁止执行",
                        "折后板形进入夹钳或翻板危险区域，当前方案可能撞机。",
                        "检查后挡位置、夹钳间隙和折弯顺序，必要时调整板型或改用其它生成方向。");
                case "HardVerticalEnvelope":
                case "HardHeightLimit":
                    return new FoldDiagnosticMessage(
                        "禁止执行",
                        "折后高度超过机器允许范围，当前方案不适合直接执行。",
                        "降低成型高度风险，调整折弯顺序或让现场人员复核夹钳/翻板空间。");
                case "BackGaugeWidthPenalty":
                    return new FoldDiagnosticMessage(
                        "需复核",
                        "后挡位置超过当前板宽范围，定位可能不可靠。",
                        "检查订单宽度、后挡参数和分切/折弯顺序。");
                case "BackGaugeLowClearancePenalty":
                    return new FoldDiagnosticMessage(
                        "需复核",
                        "板形可能进入后挡低位干涉区域，存在刮碰或定位不稳风险。",
                        "检查后挡最大位置、板宽参数，或尝试反向/换面生成。");
                case "ProfileHeightSoftPenalty":
                    return new FoldDiagnosticMessage(
                        "需复核",
                        "折后轮廓接近夹钳或翻板高度限制，系统已降低该方案优先级。",
                        "优先选择其它推荐方案；若必须使用，请现场确认夹钳和翻板空间。");
                case "NearCollisionMargin":
                case "SecondaryClearanceMargin":
                    return new FoldDiagnosticMessage(
                        "需复核",
                        "当前方案与危险区域距离较小，存在近距离干涉风险。",
                        "优先选择评分更低的方案，并复核后挡、翻面和重抓步骤。");
                case "OvergripHeightPenalty":
                    return new FoldDiagnosticMessage(
                        "需复核",
                        "抓取位置过短或过低，板料夹持稳定性可能不足。",
                        "调整抓取类型、后挡位置或折弯顺序后重新生成。");
                case "OppositeSideSoftEnvelope":
                case "ReverseSideSoftEnvelope":
                    return new FoldDiagnosticMessage(
                        "需复核",
                        "板形接近另一侧夹钳/翻板包络区域，系统已降低该方案优先级。",
                        "尝试换面、反向生成，或现场复核对应侧空间。");
                default:
                    return new FoldDiagnosticMessage(
                        hardReject ? "禁止执行" : "需复核",
                        "当前折弯方案存在机械可行性风险。",
                        "检查板型、后挡、夹钳和折弯顺序后重新生成。");
            }
        }

        private void EvaluateFoldSequenceCandidate(FoldSequenceCandidate candidate)
        {
            if (candidate.Steps.Count <= 0)
            {
                candidate.Reject("NoSteps", "未生成生产步骤。");
                return;
            }

            int maxStepCount = Hmi_iSemiAuto.Length / 10;
            if (candidate.Steps.Count > maxStepCount)
            {
                candidate.Reject("SemiAutoCapacity", $"生产步骤 {candidate.Steps.Count} 超过 PLC 半自动表容量 {maxStepCount}。");
                return;
            }

            for (int i = 0; i < candidate.Steps.Count; i++)
            {
                if (!TryValidateSemiAutoStepForCandidate(candidate.Steps[i], i, candidate.Shape, out string failureCode, out string failureMessage))
                {
                    candidate.Reject(failureCode, failureMessage);
                    return;
                }
            }

            FoldCollisionEvaluation collision = EvaluateFoldCollision(candidate.Steps, candidate.Shape, out List<FoldStepSnapshot> snapshots);
            if (collision.HardReject)
            {
                candidate.Reject("CollisionHardReject", collision.Summary, BuildCollisionRejectedCandidateSummary(candidate, collision));
                return;
            }

            double habitScore = CalculateFoldHabitScore(candidate, snapshots);
            double manualPreferenceScore = CalculateManualPreferenceScore(candidate);
            FoldScoreBreakdown scoreBreakdown = CalculateFoldScoreBreakdown(candidate, snapshots, collision, habitScore, manualPreferenceScore);
            candidate.Accept(scoreBreakdown, collision, snapshots);
        }

        private static bool TryValidateSemiAutoStepForCandidate(
            SemiAutoType step,
            int stepIndex,
            FoldShapeProfile shape,
            out string failureCode,
            out string failureMessage)
        {
            failureCode = "";
            failureMessage = "";

            if (step.行动类型 != SemiAutoActionFold
                && step.行动类型 != SemiAutoActionSquash
                && step.行动类型 != SemiAutoActionOpenSquash
                && step.行动类型 != SemiAutoActionSlit
                && step.行动类型 != SemiAutoActionFlip)
            {
                failureCode = "UnknownAction";
                failureMessage = $"第 {stepIndex + 1} 步动作类型 {step.行动类型} 未定义。";
                return false;
            }

            if (step.抓取类型 < 0 || step.抓取类型 > 2)
            {
                failureCode = "GripTypeRange";
                failureMessage = $"第 {stepIndex + 1} 步抓取类型 {step.抓取类型} 超出 0..2。";
                return false;
            }

            if (!CanPackSemiAutoStep(step))
            {
                failureCode = "PackingRange";
                failureMessage = $"第 {stepIndex + 1} 步存在超出 short 打包范围的数值。";
                return false;
            }

            bool needsGeometry = step.行动类型 == SemiAutoActionFold || IsSemiAutoSquashAction(step.行动类型);
            if (needsGeometry)
            {
                int pointCount = CurtOrder.pxList.Count;
                if (step.坐标序号 < 0 || (pointCount > 0 && step.坐标序号 >= pointCount))
                {
                    failureCode = "CoordinateIndexOutOfRange";
                    failureMessage = $"第 {stepIndex + 1} 步坐标序号 {step.坐标序号} 超出版型范围。";
                    return false;
                }
            }

            return true;
        }

        private static bool CanPackSemiAutoStep(SemiAutoType step)
        {
            if (!CanPackShort(step.行动类型) || !CanPackShort(step.折弯方向))
                return false;

            if (step.行动类型 == SemiAutoActionSlit)
            {
                if (!CanPackShort(8880) || !CanPackShort(0))
                    return false;
            }
            else if (IsSemiAutoSquashAction(step.行动类型))
            {
                if (!CanPackShort(10) || !CanPackShort(Math.Round(step.回弹值)))
                    return false;
            }
            else if (step.行动类型 == SemiAutoActionFlip)
            {
                if (!CanPackShort(80) || !CanPackShort(0))
                    return false;
            }
            else if (IsLegacySemiAutoPlaceholder(step))
            {
                if (!CanPackShort(300) || !CanPackShort(0))
                    return false;
            }
            else
            {
                if (!CanPackShort(Math.Round(step.折弯角度 * 10.0))
                    || !CanPackShort(Math.Round(step.回弹值 * 10.0)))
                    return false;
            }

            return CanPackShort(Math.Round(step.后挡位置 * 10.0))
                && CanPackShort(step.抓取类型)
                && CanPackShort(step.松开高度)
                && CanPackShort(step.翻板收缩值)
                && CanPackShort(step.重新抓取)
                && CanPackShort(Math.Round(step.锥度斜率));
        }

        private static bool CanPackShort(double value)
        {
            return value >= short.MinValue && value <= short.MaxValue;
        }

        private static FoldCollisionEvaluation EvaluateFoldCollision(
            IReadOnlyList<SemiAutoType> steps,
            FoldShapeProfile shape,
            out List<FoldStepSnapshot> snapshots)
        {
            snapshots = new List<FoldStepSnapshot>();
            List<PointF> profile = CloneCurrentProfile(CurtOrder.pxList);
            double width = shape.Width > 0 ? shape.Width : CalculateOrderWidth(CurtOrder.lengAngle);
            double softHeightLimit = GetPlannerSoftHeightLimit(shape);
            double hardHeightLimit = GetPlannerHardHeightLimit(shape, width);
            double comfortableClearance = Math.Max(8.0, shape.Thickness * 2.0);
            double clampGap = GetPlannerClampGap();
            double clampReference = GetPlannerClampReference();
            double clampSideBand = GetPlannerClampSideBandLimit();
            double reverseSideSoftEnvelope = GetPlannerReverseSideSoftEnvelope();
            double hardVerticalEnvelope = GetPlannerHardVerticalEnvelope();
            double maxOvergripHeight = GetPlannerMaxOvergripHeight();
            double backstopLimit = GetPlannerBackstopLimit();
            double backGaugeHeight = GetPlannerBackGaugeHeight();
            double marginIgnoreThreshold = GetPlannerMarginIgnoreThreshold();
            double penalty = 0;
            int hardRejects = 0;
            int softPenalties = 0;
            int nearCollisions = 0;
            double minClearance = double.MaxValue;
            double nearCollisionMargin = 0;
            string primaryCode = "Ok";
            bool retrySuggested = false;

            for (int i = 0; i < steps.Count; i++)
            {
                SemiAutoType step = steps[i];
                int anchorIndex = GetValidAnchorIndex(step.坐标序号, profile.Count);

                if (profile.Count > 0)
                    ApplyGeometryStep(profile, step, anchorIndex);

                bool hasFlipAfterStep = step.行动类型 == SemiAutoActionFlip || ShouldFlipAfterStep(steps, i);
                if (step.行动类型 != SemiAutoActionFlip && ShouldFlipAfterStep(steps, i))
                    FlipProfile(profile);

                double maxProfileHeight = GetMaxProfileHeight(profile);
                double minProfileY = GetMinProfileY(profile);
                double clearance = softHeightLimit - maxProfileHeight;
                minClearance = Math.Min(minClearance, clearance);
                bool clampEnvelopeHardReject = minProfileY < 0
                    && step.后挡位置 > 0
                    && step.后挡位置 < clampGap
                    && maxProfileHeight > clampReference;
                bool hardVerticalEnvelopeHit = HasProfilePointInClampBand(profile, clampSideBand, hardVerticalEnvelope);
                bool oppositeSideSoftEnvelopeHit = HasPositiveProfilePointInClampBand(profile, clampSideBand, reverseSideSoftEnvelope, hardVerticalEnvelope);
                bool reverseSideSoftEnvelopeHit = HasNegativeProfilePointInClampBand(profile, clampSideBand, reverseSideSoftEnvelope, hardVerticalEnvelope);
                bool hardEnvelopeHit = clampEnvelopeHardReject || hardVerticalEnvelopeHit || (hardHeightLimit > 0 && maxProfileHeight > hardHeightLimit);

                if (hardEnvelopeHit)
                {
                    hardRejects++;
                    if (ShouldSuggestMirrorRetryForStep(steps, i, step, hasFlipAfterStep))
                    {
                        retrySuggested = true;
                        primaryCode = KeepFirstCollisionCode(primaryCode, "MirrorRetrySuggested");
                    }
                    else if (clampEnvelopeHardReject)
                    {
                        primaryCode = KeepFirstCollisionCode(primaryCode, "ClampEnvelopeHardReject");
                    }
                    else if (hardVerticalEnvelopeHit)
                    {
                        primaryCode = KeepFirstCollisionCode(primaryCode, "HardVerticalEnvelope");
                    }
                    else
                    {
                        primaryCode = KeepFirstCollisionCode(primaryCode, "HardHeightLimit");
                    }
                }
                else if (clearance < 0)
                {
                    softPenalties++;
                    penalty += 1000000.0 + Math.Abs(clearance) * 10000.0;
                    primaryCode = KeepFirstCollisionCode(primaryCode, "ProfileHeightSoftPenalty");
                }
                else if (TryGetNearCollisionMargin(clearance, comfortableClearance, marginIgnoreThreshold, out double currentNearMargin))
                {
                    nearCollisions++;
                    nearCollisionMargin = Math.Max(nearCollisionMargin, currentNearMargin);
                    penalty += CalculateNearCollisionMarginPenalty(currentNearMargin);
                    primaryCode = KeepFirstCollisionCode(primaryCode, "NearCollisionMargin");
                }

                if (width > 0 && step.后挡位置 > width + InlineSlitWidthTolerance)
                {
                    softPenalties++;
                    penalty += 1000000.0 + (step.后挡位置 - width) * 1000.0;
                    primaryCode = KeepFirstCollisionCode(primaryCode, "BackGaugeWidthPenalty");
                }

                if (HasBackGaugeLowClearancePoint(profile, backstopLimit, backGaugeHeight))
                {
                    softPenalties++;
                    penalty += 1000000.0;
                    primaryCode = KeepFirstCollisionCode(primaryCode, "BackGaugeLowClearancePenalty");
                }

                bool shortGripRisk = step.抓取类型 == 0 || (step.后挡位置 > 0 && step.后挡位置 < 20.0);
                if (step.后挡位置 > 0 && step.后挡位置 <= maxOvergripHeight)
                {
                    softPenalties++;
                    penalty += 2000000.0;
                    primaryCode = KeepFirstCollisionCode(primaryCode, "OvergripHeightPenalty");
                }

                if (oppositeSideSoftEnvelopeHit)
                {
                    softPenalties++;
                    penalty += 1000000.0;
                    primaryCode = KeepFirstCollisionCode(primaryCode, "OppositeSideSoftEnvelope");
                }

                if (reverseSideSoftEnvelopeHit)
                {
                    softPenalties++;
                    penalty += 1000000.0;
                    primaryCode = KeepFirstCollisionCode(primaryCode, "ReverseSideSoftEnvelope");
                }

                snapshots.Add(new FoldStepSnapshot(
                    i + 1,
                    step.行动类型,
                    step.is色下,
                    step.内外选择,
                    step.坐标序号,
                    step.后挡位置,
                    hasFlipAfterStep,
                    step.重新抓取 != 0,
                    shortGripRisk,
                    maxProfileHeight,
                    clearance));
            }

            if (minClearance == double.MaxValue)
                minClearance = 0;

            FoldSecondaryMarginEvaluation secondaryMargin = EvaluateSecondaryClearanceMargin(
                snapshots,
                comfortableClearance,
                marginIgnoreThreshold,
                nearCollisionMargin);
            if (secondaryMargin.HasMargin)
            {
                nearCollisions += secondaryMargin.Count;
                penalty += secondaryMargin.Penalty;
                nearCollisionMargin = Math.Max(nearCollisionMargin, secondaryMargin.Margin);
                primaryCode = KeepFirstCollisionCode(primaryCode, "SecondaryClearanceMargin");
            }

            bool hardReject = hardRejects > 0;
            double scoreDelta = penalty;
            double margin = nearCollisionMargin;
            string summary = $"shape={shape.ShapeKey}; code={primaryCode}; hardReject={hardReject}; retry={retrySuggested}; margin={margin:0.###}; secondaryMargin={secondaryMargin.Margin:0.###}; hard={hardRejects}; soft={softPenalties}; near={nearCollisions}; minClearance={minClearance:0.###}; clampGap={clampGap:0.###}; backstop={backstopLimit:0.###}; backGaugeHeight={backGaugeHeight:0.###}; penalty={penalty:0.###}";
            return new FoldCollisionEvaluation(primaryCode, hardReject, scoreDelta, margin, retrySuggested, hardRejects, softPenalties, nearCollisions, penalty, minClearance, summary);
        }

        private static string KeepFirstCollisionCode(string currentCode, string nextCode)
        {
            return currentCode == "Ok" ? nextCode : currentCode;
        }

        private static bool ShouldSuggestMirrorRetryForStep(
            IReadOnlyList<SemiAutoType> steps,
            int stepIndex,
            SemiAutoType step,
            bool hasFlipAfterStep)
        {
            if (step.行动类型 == SemiAutoActionFlip)
                return false;

            if (step.行动类型 != SemiAutoActionFold && !IsSemiAutoSquashAction(step.行动类型))
                return false;

            bool hasSideTransition = hasFlipAfterStep;
            if (stepIndex > 0)
            {
                SemiAutoType previousStep = steps[stepIndex - 1];
                hasSideTransition |= previousStep.内外选择 != step.内外选择;
                hasSideTransition |= previousStep.is色下 != step.is色下;
                hasSideTransition |= previousStep.折弯方向 != step.折弯方向;
            }

            return hasSideTransition;
        }

        private static bool TryGetNearCollisionMargin(
            double clearance,
            double comfortableClearance,
            double marginIgnoreThreshold,
            out double margin)
        {
            margin = comfortableClearance - clearance;
            if (margin <= marginIgnoreThreshold)
            {
                margin = 0;
                return false;
            }

            return true;
        }

        private static double CalculateNearCollisionMarginPenalty(double margin)
        {
            return margin * margin * margin * 1000.0;
        }

        private static FoldSecondaryMarginEvaluation EvaluateSecondaryClearanceMargin(
            IReadOnlyList<FoldStepSnapshot> snapshots,
            double comfortableClearance,
            double marginIgnoreThreshold,
            double directMargin)
        {
            double margin = 0;
            for (int i = 1; i < snapshots.Count; i++)
            {
                FoldStepSnapshot previous = snapshots[i - 1];
                FoldStepSnapshot current = snapshots[i];
                if (!CanUseSnapshotForSecondaryMargin(previous) || !CanUseSnapshotForSecondaryMargin(current))
                    continue;

                double pairedClearance = Math.Min(previous.ClearanceMargin, current.ClearanceMargin);
                if (!TryGetNearCollisionMargin(pairedClearance, comfortableClearance, marginIgnoreThreshold, out double currentMargin))
                    continue;

                if (currentMargin <= directMargin + marginIgnoreThreshold)
                    continue;

                margin = Math.Max(margin, currentMargin);
            }

            if (margin <= 0)
                return default;

            return new FoldSecondaryMarginEvaluation(1, margin, CalculateNearCollisionMarginPenalty(margin));
        }

        private static bool CanUseSnapshotForSecondaryMargin(FoldStepSnapshot snapshot)
        {
            return snapshot.ActionType == SemiAutoActionFold || IsSemiAutoSquashAction(snapshot.ActionType);
        }

        private static double GetPlannerClampGap()
        {
            return ReadPlannerSetupValue(100, 35.0);
        }

        private static double GetPlannerClampReference()
        {
            return ReadPlannerSetupValue(101, 25.0);
        }

        private static double GetPlannerSoftHeightLimit(FoldShapeProfile shape)
        {
            double clampGap = GetPlannerClampGap();
            double clampReference = GetPlannerClampReference();
            double thicknessAllowance = Math.Max(2.0, shape.Thickness * 2.0);
            return Math.Max(35.0, Math.Max(clampGap, clampReference) + thicknessAllowance);
        }

        private static double GetPlannerHardHeightLimit(FoldShapeProfile shape, double width)
        {
            double apronHeight = ReadPlannerSetupValue(102, 0);
            double geometryFallback = Math.Max(120.0, Math.Max(width, 1.0) * 2.0);
            if (apronHeight <= 0)
                return geometryFallback;

            return Math.Max(apronHeight, geometryFallback);
        }

        private static double ReadPlannerSetupValue(int index, double fallback)
        {
            if (index >= 0 && index < Hmi_rArray.Length && Hmi_rArray[index] > 0)
                return Hmi_rArray[index];

            return fallback;
        }

        private static double GetMaxProfileHeight(IReadOnlyList<PointF> profile)
        {
            double max = 0;
            for (int i = 0; i < profile.Count; i++)
                max = Math.Max(max, Math.Abs(profile[i].Y));

            return max;
        }

        private static double GetMinProfileY(IReadOnlyList<PointF> profile)
        {
            double min = double.MaxValue;
            for (int i = 0; i < profile.Count; i++)
                min = Math.Min(min, profile[i].Y);

            return min == double.MaxValue ? 0 : min;
        }

        private static bool HasProfilePointInClampBand(
            IReadOnlyList<PointF> profile,
            double bandLimit,
            double heightLimit)
        {
            for (int i = 0; i < profile.Count; i++)
            {
                if (Math.Abs(profile[i].X) <= bandLimit && Math.Abs(profile[i].Y) >= heightLimit)
                    return true;
            }

            return false;
        }

        private static bool HasPositiveProfilePointInClampBand(
            IReadOnlyList<PointF> profile,
            double bandLimit,
            double minHeight,
            double hardHeightLimit)
        {
            for (int i = 0; i < profile.Count; i++)
            {
                if (Math.Abs(profile[i].X) <= bandLimit
                    && profile[i].Y >= minHeight
                    && profile[i].Y < hardHeightLimit)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasNegativeProfilePointInClampBand(
            IReadOnlyList<PointF> profile,
            double bandLimit,
            double minDepth,
            double hardDepthLimit)
        {
            for (int i = 0; i < profile.Count; i++)
            {
                if (Math.Abs(profile[i].X) <= bandLimit
                    && profile[i].Y <= -minDepth
                    && profile[i].Y > -hardDepthLimit)
                {
                    return true;
                }
            }

            return false;
        }

        private static double GetPlannerClampSideBandLimit()
        {
            return 15.0;
        }

        private static double GetPlannerReverseSideSoftEnvelope()
        {
            return 250.0;
        }

        private static double GetPlannerHardVerticalEnvelope()
        {
            return 300.0;
        }

        private static double GetPlannerMaxOvergripHeight()
        {
            return 10.0;
        }

        private static double GetPlannerBackstopLimit()
        {
            double configuredBackstopLimit = ReadPlannerSetupValue(110, 1000.0);
            return Math.Max(1000.0, configuredBackstopLimit);
        }

        private static double GetPlannerBackGaugeHeight()
        {
            return 13.0;
        }

        private static bool HasBackGaugeLowClearancePoint(
            IReadOnlyList<PointF> profile,
            double backstopLimit,
            double backGaugeHeight)
        {
            for (int i = 0; i < profile.Count; i++)
            {
                if (profile[i].X <= -backstopLimit && profile[i].Y <= backGaugeHeight)
                    return true;
            }

            return false;
        }

        private static double GetPlannerMarginIgnoreThreshold()
        {
            return 0.1;
        }

        private static double CalculateFoldHabitScore(FoldSequenceCandidate candidate, IReadOnlyList<FoldStepSnapshot> snapshots)
        {
            double score = candidate.StrategyName == "StandardFoldStrategy" ? 100.0 : 0.0;
            if (candidate.Shape.HasMixedFoldDirections && snapshots.Count(s => s.HasFlipAfterStep) <= 1)
                score += 40.0;
            if (candidate.Shape.HasHeadSquash || candidate.Shape.HasTailSquash)
                score += candidate.Steps.Count(s => IsSemiAutoSquashAction(s.行动类型)) * 5.0;

            return score;
        }

        private static double CalculateManualPreferenceScore(FoldSequenceCandidate candidate)
        {
            string key = BuildManualPreferenceKey(candidate.Shape.ShapeKey, BuildFoldSequenceSignature(candidate.Steps));
            if (!FoldManualPreferenceSamples.TryGetValue(key, out FoldManualPreferenceSample? sample))
                return 0.0;

            return Math.Min(500.0, 150.0 + sample.HitCount * 50.0);
        }

        private static string BuildManualPreferenceKey(string shapeKey, string sequenceSignature)
        {
            return shapeKey + "=>" + sequenceSignature;
        }

        private void RegisterManualSemiAutoPreference()
        {
            if (CurtOrder.lstSemiAuto.Count <= 0 || CurtOrder.pxList.Count <= 0)
                return;

            SemiAutoGenerationContext context = new SemiAutoGenerationContext(CurtOrder);
            FoldShapeProfile shape = new FoldShapeProfile(CurtOrder, context);
            string sequenceSignature = BuildFoldSequenceSignature(CurtOrder.lstSemiAuto);
            if (string.IsNullOrWhiteSpace(sequenceSignature))
                return;

            string key = BuildManualPreferenceKey(shape.ShapeKey, sequenceSignature);
            if (FoldManualPreferenceSamples.TryGetValue(key, out FoldManualPreferenceSample? sample))
                sample.Increment();
            else
                FoldManualPreferenceSamples[key] = new FoldManualPreferenceSample(sequenceSignature);
        }

        private static FoldScoreBreakdown CalculateFoldScoreBreakdown(
            FoldSequenceCandidate candidate,
            IReadOnlyList<FoldStepSnapshot> snapshots,
            FoldCollisionEvaluation collision,
            double habitScore,
            double manualPreferenceScore)
        {
            double backGaugeTravel = 0;
            for (int i = 1; i < snapshots.Count; i++)
                backGaugeTravel += Math.Abs(snapshots[i].BackGauge - snapshots[i - 1].BackGauge);

            int flipCount = snapshots.Count(s => s.HasFlipAfterStep);
            int regripCount = snapshots.Count(s => s.HasRegrip);
            int shortGripRiskCount = snapshots.Count(s => s.HasShortGripRisk);
            double strategyPenalty = candidate.StrategyName == "StandardFoldStrategy" ? 0.0 : 250.0;
            double flipPenalty = flipCount * 20000.0;
            double regripPenalty = regripCount * 10000.0;
            double shortGripPenalty = shortGripRiskCount * 50000.0;
            double backGaugeTravelPenalty = backGaugeTravel * 10.0;

            return new FoldScoreBreakdown(
                collision.Penalty,
                strategyPenalty,
                flipPenalty,
                regripPenalty,
                shortGripPenalty,
                backGaugeTravelPenalty,
                backGaugeTravel,
                habitScore,
                manualPreferenceScore);
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
