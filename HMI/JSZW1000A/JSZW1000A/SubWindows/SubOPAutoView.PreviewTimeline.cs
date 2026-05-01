namespace JSZW1000A.SubWindows
{
    public partial class SubOPAutoView
    {
        private bool IsFeedPhase()
        {
            return iDrawStep == 0 || (!showingFlipCompletionState && iDrawStep % 2 == 1);
        }

        private bool IsFoldPhase()
        {
            return !showingFlipCompletionState && iDrawStep > 0 && iDrawStep % 2 == 0;
        }

        private void PreViewSt()
        {
            int currentIndex = GetCurrentPreviewStepIndex();
            if (!showingFlipCompletionState && ShouldFlipAfterCurrentPreviewStep(currentIndex))
            {
                Flip_DataProc();
                return;
            }

            showingFlipCompletionState = false;

            iDrawStep++;

            if (iDrawStep > (MainFrm.CurtOrder.lstSemiAuto.Count) * 2)
            {
                tmr预览.Enabled = false;
                return;
            }

            refshPoint();
            redrawPreView(currentPreviewColorDown);
        }

        private int GetCurrentPreviewStepIndex()
        {
            if (MainFrm.CurtOrder.lstSemiAuto.Count <= 0)
                return -1;

            if (iDrawStep <= 0)
                return 0;

            int half = (int)((iDrawStep - 1) / 2);
            return Math.Clamp(half, 0, MainFrm.CurtOrder.lstSemiAuto.Count - 1);
        }

        private bool ShouldFlipAfterCurrentPreviewStep(int currentStepIndex)
        {
            if (showingFlipCompletionState)
                return false;
            if (iDrawStep <= 0 || iDrawStep % 2 != 0)
                return false;
            if (currentStepIndex < 0 || currentStepIndex >= MainFrm.CurtOrder.lstSemiAuto.Count - 1)
                return false;

            return MainFrm.CurtOrder.lstSemiAuto[currentStepIndex].内外选择
                != MainFrm.CurtOrder.lstSemiAuto[currentStepIndex + 1].内外选择;
        }

        private int GetDisplayedPreviewStepIndex(int currentStepIndex)
        {
            if (currentStepIndex < 0)
                return -1;

            if (showingFlipCompletionState
                && currentStepIndex < MainFrm.CurtOrder.lstSemiAuto.Count - 1
                && MainFrm.CurtOrder.lstSemiAuto[currentStepIndex].内外选择 != MainFrm.CurtOrder.lstSemiAuto[currentStepIndex + 1].内外选择)
            {
                int nextIndex = currentStepIndex + 1;
                while (nextIndex < MainFrm.CurtOrder.lstSemiAuto.Count
                    && MainFrm.CurtOrder.lstSemiAuto[nextIndex].行动类型 == MainFrm.SemiAutoActionFlip)
                {
                    nextIndex++;
                }

                if (nextIndex < MainFrm.CurtOrder.lstSemiAuto.Count)
                    return nextIndex;
            }

            return currentStepIndex;
        }

        private List<Point> BuildPreviewSegmentsForDrawStep(int drawStep)
        {
            List<Point> previewSegments = new();
            currentPreviewPolyline.Clear();
            currentPreviewAppliedStepCount = 0;
            currentPreviewColorDown = MainFrm.CurtOrder.st色下;
            if (MainFrm.CurtOrder.lstSemiAuto.Count <= 0)
                return previewSegments;

            int currentIndex = GetCurrentPreviewStepIndex();
            if (currentIndex < 0)
                currentIndex = 0;

            bool afterCurrentFold = drawStep > 0 && drawStep % 2 == 0;
            int displayIndex = GetDisplayedPreviewStepIndex(currentIndex);
            if (displayIndex < 0)
                return previewSegments;

            int appliedStepCount = showingFlipCompletionState
                ? displayIndex
                : currentIndex + (afterCurrentFold ? 1 : 0);
            currentPreviewAppliedStepCount = appliedStepCount;

            bool applyFlipAfterLastIncludedStep = !showingFlipCompletionState && !afterCurrentFold;
            MainFrm.SemiAutoType displayStep = MainFrm.CurtOrder.lstSemiAuto[displayIndex];
            currentPreviewColorDown = showingFlipCompletionState
                ? displayStep.is色下
                : MainFrm.ResolveSemiAutoPreviewColorDown(
                    MainFrm.CurtOrder,
                    appliedStepCount,
                    applyFlipAfterLastIncludedStep);

            List<PointF> stagedProfile = MainFrm.BuildSemiAutoPreviewStageProfile(
                MainFrm.CurtOrder,
                appliedStepCount,
                applyFlipAfterLastIncludedStep);
            if (stagedProfile.Count <= 1)
                return previewSegments;

            List<PointF> transformed;
            if (showingFlipCompletionState)
            {
                transformed = TransformPreviewProfileToStepFrame(stagedProfile, displayStep);
                transformed = RotatePreviewProfileAroundScreenCenter(transformed, 180.0);
            }
            else if (!afterCurrentFold && displayIndex > 0)
            {
                int previousDisplayIndex = GetPreviousRenderableStepIndex(displayIndex - 1);
                if (previousDisplayIndex >= 0)
                {
                    List<PointF> fromProfile = TransformPreviewProfileToStepFrame(stagedProfile, MainFrm.CurtOrder.lstSemiAuto[previousDisplayIndex]);
                    List<PointF> toProfile = TransformPreviewProfileToStepFrame(stagedProfile, displayStep);
                    transformed = InterpolatePreviewProfiles(fromProfile, toProfile, 0.5);
                }
                else
                {
                    transformed = TransformPreviewProfileToStepFrame(stagedProfile, displayStep);
                }
            }
            else
            {
                transformed = TransformPreviewProfileToStepFrame(stagedProfile, displayStep);
            }

            List<Point> transformedPoints = new();
            foreach (PointF point in transformed)
                transformedPoints.Add(Point.Round(point));

            foreach (Point point in transformedPoints)
                currentPreviewPolyline.Add(point);

            for (int i = 1; i < transformedPoints.Count; i++)
            {
                previewSegments.Add(transformedPoints[i - 1]);
                previewSegments.Add(transformedPoints[i]);
            }

            return previewSegments;
        }

        private List<PointF> TransformPreviewProfileToStepFrame(IReadOnlyList<PointF> stagedProfile, MainFrm.SemiAutoType displayStep)
        {
            List<PointF> transformed = new();
            if (stagedProfile.Count <= 1)
                return transformed;

            int anchorIndex = Math.Clamp(displayStep.坐标序号, 0, stagedProfile.Count - 1);
            bool feedSideUsesLowerIndex = displayStep.内外选择 == 1;
            bool feedSideShouldDisplayLeft = true;
            int neighborIndex = feedSideUsesLowerIndex
                ? Math.Max(anchorIndex - 1, 0)
                : Math.Min(anchorIndex + 1, stagedProfile.Count - 1);
            if (neighborIndex == anchorIndex)
            {
                neighborIndex = feedSideUsesLowerIndex
                    ? Math.Min(anchorIndex + 1, stagedProfile.Count - 1)
                    : Math.Max(anchorIndex - 1, 0);
            }
            if (neighborIndex == anchorIndex)
                return transformed;

            PointF anchor = stagedProfile[anchorIndex];
            PointF neighbor = stagedProfile[neighborIndex];
            double angle = Math.Atan2(neighbor.Y - anchor.Y, neighbor.X - anchor.X);
            double targetHeading = feedSideShouldDisplayLeft ? Math.PI : 0.0;
            double rotateToTarget = targetHeading - angle;

            foreach (PointF point in stagedProfile)
            {
                double dx = point.X - anchor.X;
                double dy = point.Y - anchor.Y;
                double rx = dx * Math.Cos(rotateToTarget) - dy * Math.Sin(rotateToTarget);
                double ry = dx * Math.Sin(rotateToTarget) + dy * Math.Cos(rotateToTarget);
                transformed.Add(new PointF(
                    (float)(cx + rx),
                    (float)(cy - ry)));
            }

            return transformed;
        }

        private List<PointF> RotatePreviewProfileAroundScreenCenter(IReadOnlyList<PointF> profile, double angle)
        {
            List<PointF> rotated = new(profile.Count);
            PointF center = new PointF(cx, cy);
            double radians = angle * Math.PI / 180.0;
            foreach (PointF point in profile)
            {
                double x = (point.X - center.X) * Math.Cos(radians) + (point.Y - center.Y) * Math.Sin(radians) + center.X;
                double y = -(point.X - center.X) * Math.Sin(radians) + (point.Y - center.Y) * Math.Cos(radians) + center.Y;
                rotated.Add(new PointF((float)x, (float)y));
            }

            return rotated;
        }

        private static List<PointF> InterpolatePreviewProfiles(IReadOnlyList<PointF> fromProfile, IReadOnlyList<PointF> toProfile, double progress)
        {
            if (fromProfile.Count != toProfile.Count || fromProfile.Count <= 0)
                return new List<PointF>(toProfile);

            float t = (float)Math.Clamp(progress, 0.0, 1.0);
            List<PointF> interpolated = new(fromProfile.Count);
            for (int i = 0; i < fromProfile.Count; i++)
            {
                PointF from = fromProfile[i];
                PointF to = toProfile[i];
                interpolated.Add(new PointF(
                    from.X + (to.X - from.X) * t,
                    from.Y + (to.Y - from.Y) * t));
            }

            return interpolated;
        }

        private int GetPreviousRenderableStepIndex(int startIndex)
        {
            for (int i = startIndex; i >= 0; i--)
            {
                if (MainFrm.CurtOrder.lstSemiAuto[i].行动类型 != MainFrm.SemiAutoActionFlip)
                    return i;
            }

            return -1;
        }

        private bool TryGetAppliedSquashColorDown(int targetIndex, out bool colorDown)
        {
            colorDown = currentPreviewColorDown;
            int appliedStepCount = Math.Clamp(currentPreviewAppliedStepCount, 0, MainFrm.CurtOrder.lstSemiAuto.Count);
            bool found = false;
            for (int i = 0; i < appliedStepCount; i++)
            {
                MainFrm.SemiAutoType step = MainFrm.CurtOrder.lstSemiAuto[i];
                if (step.长角序号 != targetIndex)
                    continue;

                if (MainFrm.IsLegacySemiAutoPlaceholder(step) || MainFrm.IsSemiAutoSquashAction(step.行动类型))
                {
                    colorDown = step.is色下;
                    found = true;
                }
            }

            return found;
        }

        private void DrawPreviewSquash(Graphics graphic, Pen outlinePen)
        {
            if (currentPreviewPolyline.Count < 2)
                return;

            if (MainFrm.CurtOrder.lengAngle[0].Angle > 0
                && MainFrm.CurtOrder.lengAngle[0].Length > 0
                && TryGetAppliedSquashColorDown(0, out bool headColorDown))
            {
                DrawSinglePreviewSquash(
                    graphic,
                    outlinePen,
                    currentPreviewPolyline[0],
                    currentPreviewPolyline[1],
                    MainFrm.CurtOrder.lengAngle[0].Length,
                    (int)MainFrm.CurtOrder.lengAngle[0].Angle,
                    true,
                    headColorDown);
            }

            if (MainFrm.CurtOrder.lengAngle[99].Angle > 0
                && MainFrm.CurtOrder.lengAngle[99].Length > 0
                && TryGetAppliedSquashColorDown(99, out bool tailColorDown))
            {
                int last = currentPreviewPolyline.Count - 1;
                DrawSinglePreviewSquash(
                    graphic,
                    outlinePen,
                    currentPreviewPolyline[last],
                    currentPreviewPolyline[last - 1],
                    MainFrm.CurtOrder.lengAngle[99].Length,
                    (int)MainFrm.CurtOrder.lengAngle[99].Angle,
                    false,
                    tailColorDown);
            }
        }

        private static void DrawSinglePreviewSquash(Graphics graphic, Pen outlinePen, PointF pStart, PointF pRef, double len, int type, bool isHead, bool currentColorDown)
        {
            double dist = Math.Sqrt(Math.Pow(pStart.X - pRef.X, 2) + Math.Pow(pStart.Y - pRef.Y, 2));
            if (dist < 0.001)
                return;

            double dx = pStart.X - pRef.X;
            double dy = pStart.Y - pRef.Y;
            double unitX = dx / dist;
            double unitY = dy / dist;

            double widthOffset = 4.0;
            double lengthOffset = len;
            double perpX = -unitY * widthOffset;
            double perpY = unitX * widthOffset;

            bool isTypeUp = (type == 1 || type == 3);
            bool needInvert = isHead ? !isTypeUp : isTypeUp;
            if (!currentColorDown)
                needInvert = !needInvert;
            if (needInvert)
            {
                perpX = -perpX;
                perpY = -perpY;
            }

            float p0x = pStart.X + (float)perpX;
            float p0y = pStart.Y + (float)perpY;
            float p1x = p0x - (float)(unitX * lengthOffset);
            float p1y = p0y - (float)(unitY * lengthOffset);

            graphic.DrawLine(outlinePen, p0x, p0y, p1x, p1y);
            graphic.DrawLine(outlinePen, p0x, p0y, pStart.X, pStart.Y);
        }
    }
}
