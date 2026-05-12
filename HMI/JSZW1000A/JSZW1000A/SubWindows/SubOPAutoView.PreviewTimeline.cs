using System.Drawing.Drawing2D;

namespace JSZW1000A.SubWindows
{
    public partial class SubOPAutoView
    {
        private bool IsFeedPhase()
        {
            return iDrawStep == 0 || (!showingFlipCompletionState && iDrawStep % 2 == 1);
        }

        private const double PreviewFlipMidAngle = -90.0;
        private const double PreviewFlipPerspectiveScale = 0.42;
        private const float PreviewClosedSquashGapPixels = 2.0f;

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
            currentPreviewCollisionSegments.Clear();
            currentPreviewDisplayStep = default;
            currentPreviewIsFoldCompletionState = false;
            currentPreviewComparesWorkingFlap = false;
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

            MainFrm.SemiAutoType displayStep = MainFrm.CurtOrder.lstSemiAuto[displayIndex];
            int appliedStepCount = showingFlipCompletionState
                ? displayIndex
                : currentIndex + (afterCurrentFold ? 1 : 0);
            currentPreviewAppliedStepCount = appliedStepCount;

            bool applyFlipAfterLastIncludedStep = !showingFlipCompletionState && !afterCurrentFold;
            currentPreviewDisplayStep = displayStep;
            currentPreviewIsFoldCompletionState = afterCurrentFold;
            currentPreviewComparesWorkingFlap = MainFrm.PreviewComparesWorkingFlapAfterFeed(
                showingFlipCompletionState,
                afterCurrentFold);
            currentPreviewColorDown = showingFlipCompletionState
                ? displayStep.is色下
                : MainFrm.ResolveSemiAutoPreviewColorDown(
                    MainFrm.CurtOrder,
                    appliedStepCount,
                    applyFlipAfterLastIncludedStep);

            if (drawStep <= 0)
                return previewSegments;

            List<PointF> stagedProfile = MainFrm.BuildSemiAutoPreviewStageProfile(
                MainFrm.CurtOrder,
                appliedStepCount,
                applyFlipAfterLastIncludedStep);
            if (stagedProfile.Count <= 1)
                return previewSegments;

            List<PointF> collisionFrameProfile = TransformPreviewProfileToStepFrame(stagedProfile, displayStep);
            List<PointF> transformed;
            if (showingFlipCompletionState)
            {
                transformed = BuildPreviewFlipTransitionFrame(collisionFrameProfile);
            }
            else
            {
                transformed = collisionFrameProfile;
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

        private void RecalculatePreviewCollisionSegmentsForCurrentDraw()
        {
            currentPreviewCollisionSegments.Clear();
            if (showingFlipCompletionState || currentPreviewPolyline.Count <= 1)
                return;

            List<PointF> collisionProfile = BuildPreviewCollisionProfile(currentPreviewPolyline);
            foreach (KeyValuePair<int, MainFrm.PreviewCollisionSeverity> entry in MainFrm.GetPreviewCollisionSegmentSeverities(
                collisionProfile,
                currentPreviewDisplayStep,
                currentPreviewIsFoldCompletionState,
                currentPreviewComparesWorkingFlap))
            {
                currentPreviewCollisionSegments[entry.Key] = entry.Value;
            }
        }

        private List<PointF> BuildPreviewCollisionProfile(IReadOnlyList<PointF> screenProfile)
        {
            List<PointF> collisionProfile = new(screenProfile.Count);
            foreach (PointF point in screenProfile)
            {
                collisionProfile.Add(new PointF(
                    point.X - cx,
                    cy - point.Y));
            }

            return collisionProfile;
        }

        private void DrawPreviewCollisionBasis(Graphics graphic)
        {
            if (!MainFrm.PreviewCollisionAreaVisible)
                return;

            if (showingFlipCompletionState || currentPreviewDisplayStep.行动类型 != MainFrm.SemiAutoActionFold)
                return;

            List<PointF[]> collisionPolygons = MainFrm.GetPreviewCollisionPolygons(currentPreviewDisplayStep);
            if (collisionPolygons.Count <= 0)
                return;

            using Brush fillBrush = new SolidBrush(Color.FromArgb(70, 255, 0, 0));
            using Pen outlinePen = new(Color.FromArgb(220, 255, 64, 64), 2);
            outlinePen.DashStyle = DashStyle.Dash;
            foreach (PointF[] machinePolygon in collisionPolygons)
            {
                if (machinePolygon.Length < 3)
                    continue;

                PointF[] screenPolygon = machinePolygon
                    .Select(point => ToPreviewScreenPoint(point.X, point.Y))
                    .ToArray();
                graphic.FillPolygon(fillBrush, screenPolygon);
                graphic.DrawPolygon(outlinePen, screenPolygon);
            }
        }

        private void DrawPreviewMotionOverlay(Graphics graphic)
        {
            if (showingFlipCompletionState)
            {
                DrawPreviewFlipMotionOverlay(graphic);
                return;
            }

        }

        private void DrawPreviewFlipMotionOverlay(Graphics graphic)
        {
            Rectangle arcBounds = new(cx - 72, cy - 72, 144, 144);
            using Pen arcPen = new(Color.FromArgb(230, 96, 176, 255), 3);
            arcPen.EndCap = LineCap.ArrowAnchor;
            graphic.DrawArc(arcPen, arcBounds, 35, -250);

            using Brush labelBrush = new SolidBrush(Color.FromArgb(230, 96, 176, 255));
            using Font font = new("Microsoft YaHei UI", 12F, FontStyle.Bold);
            string text = MainFrm.Lang == 0 ? "翻面中" : "Flipping";
            graphic.DrawString(text, font, labelBrush, cx - 16, cy - 150);
        }

        private PointF ToPreviewScreenPoint(double x, double y)
        {
            return new PointF((float)(cx + x), (float)(cy - y));
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

            ApplyPreviewSquashFeedBackGaugeOffset(transformed, displayStep);
            return transformed;
        }

        private void ApplyPreviewSquashFeedBackGaugeOffset(List<PointF> transformed, MainFrm.SemiAutoType displayStep)
        {
            if (transformed.Count <= 0 || !MainFrm.IsSemiAutoSquashAction(displayStep.行动类型) || currentPreviewIsFoldCompletionState)
                return;

            int displayIndex = GetDisplayedPreviewStepIndex(GetCurrentPreviewStepIndex());
            if (displayIndex <= 0 || !MainFrm.IsLegacySemiAutoPlaceholder(MainFrm.CurtOrder.lstSemiAuto[displayIndex - 1]))
                return;

            MainFrm.SemiAutoType preformStep = MainFrm.CurtOrder.lstSemiAuto[displayIndex - 1];
            float offsetX = (float)(preformStep.后挡位置 - displayStep.后挡位置);
            if (Math.Abs(offsetX) < 0.001f)
                return;

            for (int i = 0; i < transformed.Count; i++)
                transformed[i] = new PointF(transformed[i].X + offsetX, transformed[i].Y);
        }

        private static List<PointF> RotatePreviewProfileAroundPoint(IReadOnlyList<PointF> profile, PointF center, double angle)
        {
            List<PointF> rotated = new(profile.Count);
            double radians = angle * Math.PI / 180.0;
            foreach (PointF point in profile)
            {
                double x = (point.X - center.X) * Math.Cos(radians) + (point.Y - center.Y) * Math.Sin(radians) + center.X;
                double y = -(point.X - center.X) * Math.Sin(radians) + (point.Y - center.Y) * Math.Cos(radians) + center.Y;
                rotated.Add(new PointF((float)x, (float)y));
            }

            return rotated;
        }

        private List<PointF> BuildPreviewFlipTransitionFrame(IReadOnlyList<PointF> profile)
        {
            PointF foldCenter = new(cx, cy);
            List<PointF> rotated = RotatePreviewProfileAroundPoint(profile, foldCenter, PreviewFlipMidAngle);
            return ApplyPreviewFlipPerspective(rotated, foldCenter, PreviewFlipPerspectiveScale);
        }

        private static List<PointF> ApplyPreviewFlipPerspective(IReadOnlyList<PointF> profile, PointF pivot, double xScale)
        {
            List<PointF> transformed = new(profile.Count);
            foreach (PointF point in profile)
            {
                double x = pivot.X + (point.X - pivot.X) * xScale;
                double y = point.Y;
                transformed.Add(new PointF((float)x, (float)y));
            }

            return transformed;
        }

        private void ApplyPreviewSquashDeformations(List<PointF> screenProfile, int appliedStepCount)
        {
            if (screenProfile.Count < 3 || MainFrm.CurtOrder.lstSemiAuto.Count <= 0)
                return;

            int clampedCount = Math.Clamp(appliedStepCount, 0, MainFrm.CurtOrder.lstSemiAuto.Count);
            for (int i = 0; i < clampedCount; i++)
            {
                MainFrm.SemiAutoType step = MainFrm.CurtOrder.lstSemiAuto[i];
                if (!MainFrm.IsSemiAutoSquashAction(step.行动类型))
                    continue;
                if (!TryGetPreviewSquashEdgeSegmentIndex(screenProfile.Count, step, out int segmentIndex))
                    continue;

                float gap = GetPreviewSquashGapPixels(step);
                ApplyPreviewSquashEdgeDeformation(screenProfile, segmentIndex, gap);
            }
        }

        private float GetPreviewSquashGapPixels(MainFrm.SemiAutoType step)
        {
            if (step.行动类型 == MainFrm.SemiAutoActionOpenSquash)
                return Math.Max(PreviewClosedSquashGapPixels, (float)(MainFrm.Hmi_rArray[129] * 0.5));

            return PreviewClosedSquashGapPixels;
        }

        private static void ApplyPreviewSquashEdgeDeformation(List<PointF> screenProfile, int segmentIndex, float gap)
        {
            int endPointIndex;
            int foldPointIndex;
            int bodyPointIndex;
            if (segmentIndex <= 0)
            {
                endPointIndex = 0;
                foldPointIndex = 1;
                bodyPointIndex = 2;
            }
            else
            {
                endPointIndex = screenProfile.Count - 1;
                foldPointIndex = screenProfile.Count - 2;
                bodyPointIndex = screenProfile.Count - 3;
            }

            PointF foldPoint = screenProfile[foldPointIndex];
            PointF bodyPoint = screenProfile[bodyPointIndex];
            PointF originalEndPoint = screenProfile[endPointIndex];
            float edgeLength = Distance(foldPoint, originalEndPoint);
            if (edgeLength < 0.001f)
                return;

            PointF bodyVector = new(bodyPoint.X - foldPoint.X, bodyPoint.Y - foldPoint.Y);
            float bodyLength = Distance(PointF.Empty, bodyVector);
            if (bodyLength < 0.001f)
                return;

            PointF unit = new(bodyVector.X / bodyLength, bodyVector.Y / bodyLength);
            PointF normal = new(-unit.Y, unit.X);
            float side = Cross(unit, new PointF(originalEndPoint.X - foldPoint.X, originalEndPoint.Y - foldPoint.Y)) >= 0
                ? 1.0f
                : -1.0f;

            screenProfile[endPointIndex] = new PointF(
                foldPoint.X + unit.X * edgeLength + normal.X * gap * side,
                foldPoint.Y + unit.Y * edgeLength + normal.Y * gap * side);
        }

        private static float Distance(PointF first, PointF second)
        {
            float dx = first.X - second.X;
            float dy = first.Y - second.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        private static float Cross(PointF first, PointF second)
        {
            return first.X * second.Y - first.Y * second.X;
        }

        private void DrawCurrentPreviewActiveDisplayEdge(Graphics graphic, Pen foldPen)
        {
            if (TryGetCurrentPreviewActiveFoldEdge(out PointF foldStartPoint, out PointF foldEndPoint))
            {
                graphic.DrawLine(foldPen, foldStartPoint, foldEndPoint);
                return;
            }

            if (TryGetCurrentPreviewSquashEdge(out foldStartPoint, out foldEndPoint))
                graphic.DrawLine(foldPen, foldStartPoint, foldEndPoint);
        }

        private void DrawCurrentPreviewSquashCue(Graphics graphic, Pen squashPen)
        {
            if (TryGetCurrentPreviewSquashCueEdge(out PointF startPoint, out PointF endPoint))
                graphic.DrawLine(squashPen, startPoint, endPoint);
        }

        private void DrawAppliedPreviewSquashEdges(Graphics graphic, Pen squashPen)
        {
            if (showingFlipCompletionState || currentPreviewPolyline.Count < 2)
                return;

            int clampedCount = Math.Clamp(currentPreviewAppliedStepCount, 0, MainFrm.CurtOrder.lstSemiAuto.Count);
            for (int i = 0; i < clampedCount; i++)
            {
                MainFrm.SemiAutoType step = MainFrm.CurtOrder.lstSemiAuto[i];
                if (!MainFrm.IsSemiAutoSquashAction(step.行动类型))
                    continue;
                if (i == GetDisplayedPreviewStepIndex(GetCurrentPreviewStepIndex()) && currentPreviewIsFoldCompletionState)
                    continue;
                if (!TryGetCompletedPreviewSquashEdge(step, out PointF startPoint, out PointF endPoint))
                    continue;

                graphic.DrawLine(squashPen, startPoint, endPoint);
            }
        }

        private void DrawCurrentPreviewFoldAnnotation(Graphics graphic)
        {
            if (!TryGetCurrentPreviewFoldPoint(out PointF foldPoint))
                return;

            using Pen markerPen = new(Color.Lime, 2);
            using Brush markerBrush = new SolidBrush(Color.FromArgb(210, 0, 255, 0));
            const float radius = 6f;
            graphic.FillEllipse(markerBrush, foldPoint.X - radius, foldPoint.Y - radius, radius * 2, radius * 2);
            graphic.DrawEllipse(markerPen, foldPoint.X - radius - 3, foldPoint.Y - radius - 3, (radius + 3) * 2, (radius + 3) * 2);
            graphic.DrawLine(markerPen, foldPoint.X - 18, foldPoint.Y, foldPoint.X + 18, foldPoint.Y);
            graphic.DrawLine(markerPen, foldPoint.X, foldPoint.Y - 18, foldPoint.X, foldPoint.Y + 18);
        }

        private void DrawCurrentPreviewFoldInfo(Graphics graphic)
        {
            List<string> lines = BuildCurrentPreviewFoldInfoLines();
            if (lines.Count <= 0)
                return;

            using Font font = new("Microsoft YaHei UI", 13F, FontStyle.Bold);
            int padding = 10;
            float width = 0;
            float height = padding * 2;
            foreach (string line in lines)
            {
                SizeF size = graphic.MeasureString(line, font);
                width = Math.Max(width, size.Width);
                height += size.Height + 2;
            }

            RectangleF bounds = new(
                pictureBox1.Width - width - padding * 3 - 18,
                118,
                width + padding * 2,
                height);
            using Brush backgroundBrush = new SolidBrush(Color.FromArgb(170, 0, 0, 0));
            using Pen borderPen = new(Color.FromArgb(220, 0, 255, 0), 2);
            using Brush textBrush = new SolidBrush(Color.Lime);
            graphic.FillRectangle(backgroundBrush, bounds);
            graphic.DrawRectangle(borderPen, bounds.X, bounds.Y, bounds.Width, bounds.Height);

            float y = bounds.Y + padding;
            foreach (string line in lines)
            {
                graphic.DrawString(line, font, textBrush, bounds.X + padding, y);
                y += font.GetHeight(graphic) + 2;
            }
        }

        private bool ShouldSkipPreviewPolylineSegment(int segmentIndex)
        {
            return false;
        }

        private bool TryGetCurrentPreviewFoldSegmentIndex(out int segmentIndex)
        {
            segmentIndex = -1;
            if (showingFlipCompletionState || currentPreviewPolyline.Count < 2)
                return false;
            if (!TryGetCurrentPreviewFoldEndpointIndices(out int firstIndex, out int secondIndex))
                return false;

            segmentIndex = Math.Min(firstIndex, secondIndex);
            return segmentIndex >= 0 && segmentIndex < currentPreviewPolyline.Count - 1;
        }

        private bool TryGetCurrentPreviewSquashSegmentIndex(out int segmentIndex)
        {
            segmentIndex = -1;
            return false;
        }

        private bool TryGetCurrentPreviewSquashCueEdge(out PointF startPoint, out PointF endPoint)
        {
            startPoint = PointF.Empty;
            endPoint = PointF.Empty;
            return false;
        }

        private bool IsCurrentPreviewSquashDisplayStep()
        {
            return MainFrm.IsSemiAutoSquashAction(currentPreviewDisplayStep.行动类型)
                || MainFrm.IsLegacySemiAutoPlaceholder(currentPreviewDisplayStep);
        }

        private bool TryGetCurrentPreviewFoldPoint(out PointF foldPoint)
        {
            foldPoint = PointF.Empty;
            if (showingFlipCompletionState || currentPreviewPolyline.Count <= 0)
                return false;
            if (TryGetCurrentPreviewActiveFoldEdge(out PointF activeStart, out _))
            {
                foldPoint = activeStart;
                return true;
            }
            if (!TryGetCurrentPreviewFoldEndpointIndices(out int firstIndex, out int secondIndex))
                return false;

            int anchorIndex = Math.Clamp(currentPreviewDisplayStep.坐标序号, 0, currentPreviewPolyline.Count - 1);
            if (anchorIndex != firstIndex && anchorIndex != secondIndex)
                anchorIndex = firstIndex;
            foldPoint = currentPreviewPolyline[anchorIndex];
            return true;
        }

        private bool TryGetCurrentPreviewActiveFoldEdge(out PointF startPoint, out PointF endPoint)
        {
            startPoint = PointF.Empty;
            endPoint = PointF.Empty;
            if (currentPreviewPolyline.Count < 2)
                return false;
            if (MainFrm.IsLegacySemiAutoPlaceholder(currentPreviewDisplayStep))
                return false;

            if (!TryGetCurrentPreviewFoldEndpointIndices(out int firstIndex, out int secondIndex))
                return false;

            startPoint = currentPreviewPolyline[firstIndex];
            endPoint = currentPreviewPolyline[secondIndex];
            return true;
        }

        private bool TryGetCurrentPreviewActiveDisplayEdge(out PointF startPoint, out PointF endPoint)
        {
            if (TryGetCurrentPreviewActiveFoldEdge(out startPoint, out endPoint))
                return true;

            return TryGetCurrentPreviewSquashEdge(out startPoint, out endPoint);
        }

        private bool TryGetCurrentPreviewSquashEdge(out PointF startPoint, out PointF endPoint)
        {
            startPoint = PointF.Empty;
            endPoint = PointF.Empty;
            if (showingFlipCompletionState || currentPreviewPolyline.Count <= 0 || !TryGetCurrentPreviewSquashEdgeIndex(out int edgeIndex))
                return false;

            float edgeLength = (float)MainFrm.CurtOrder.lengAngle[edgeIndex].Length;
            if (edgeLength <= 0.001f)
                return false;

            double angle = GetCurrentPreviewIndependentSquashEdgeAngle();
            PointF foldEndpoint = GetCurrentPreviewSquashFoldEndpoint(edgeIndex);
            if (MainFrm.IsSemiAutoSquashAction(currentPreviewDisplayStep.行动类型) && currentPreviewIsFoldCompletionState)
            {
                return TryBuildCompletedPreviewSquashEdge(edgeIndex, edgeLength, out startPoint, out endPoint);
            }

            startPoint = foldEndpoint;
            endPoint = new PointF(
                (float)(foldEndpoint.X + Math.Cos(angle) * edgeLength),
                (float)(foldEndpoint.Y - Math.Sin(angle) * edgeLength));
            return true;
        }

        private PointF GetCurrentPreviewSquashFoldEndpoint(int edgeIndex)
        {
            if (currentPreviewPolyline.Count <= 0)
                return new PointF(cx, cy);
            if (edgeIndex == 99)
                return currentPreviewPolyline[^1];

            return currentPreviewPolyline[0];
        }

        private bool TryGetCompletedPreviewSquashEdge(MainFrm.SemiAutoType step, out PointF startPoint, out PointF endPoint)
        {
            startPoint = PointF.Empty;
            endPoint = PointF.Empty;
            int edgeIndex = ResolvePreviewSquashEdgeIndex(step);
            if (!HasValidSquashEdgeLength(edgeIndex))
                return false;

            return TryBuildCompletedPreviewSquashEdge(
                edgeIndex,
                (float)MainFrm.CurtOrder.lengAngle[edgeIndex].Length,
                out startPoint,
                out endPoint);
        }

        private bool TryBuildCompletedPreviewSquashEdge(int edgeIndex, float edgeLength, out PointF startPoint, out PointF endPoint)
        {
            startPoint = PointF.Empty;
            endPoint = PointF.Empty;
            if (currentPreviewPolyline.Count < 2 || edgeLength <= 0.001f)
                return false;

            int endpointIndex = edgeIndex == 99 ? currentPreviewPolyline.Count - 1 : 0;
            int neighborIndex = edgeIndex == 99 ? currentPreviewPolyline.Count - 2 : 1;
            PointF endpoint = currentPreviewPolyline[endpointIndex];
            PointF neighbor = currentPreviewPolyline[neighborIndex];
            float dx = neighbor.X - endpoint.X;
            float dy = neighbor.Y - endpoint.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);
            if (distance <= 0.001f)
                return false;

            endPoint = endpoint;
            startPoint = new PointF(
                endpoint.X + dx / distance * edgeLength,
                endpoint.Y + dy / distance * edgeLength);
            return true;
        }

        private double GetCurrentPreviewIndependentSquashEdgeAngle()
        {
            if (MainFrm.IsLegacySemiAutoPlaceholder(currentPreviewDisplayStep) && currentPreviewIsFoldCompletionState)
            {
                double signedAngle = currentPreviewDisplayStep.折弯方向 == 0 ? 150.0 : -150.0;
                return signedAngle * Math.PI / 180.0;
            }

            int displayIndex = GetDisplayedPreviewStepIndex(GetCurrentPreviewStepIndex());
            if (MainFrm.IsSemiAutoSquashAction(currentPreviewDisplayStep.行动类型)
                && !currentPreviewIsFoldCompletionState
                && displayIndex > 0
                && MainFrm.IsLegacySemiAutoPlaceholder(MainFrm.CurtOrder.lstSemiAuto[displayIndex - 1]))
            {
                MainFrm.SemiAutoType preformStep = MainFrm.CurtOrder.lstSemiAuto[displayIndex - 1];
                double signedAngle = preformStep.折弯方向 == 0 ? 150.0 : -150.0;
                return signedAngle * Math.PI / 180.0;
            }

            return 0.0;
        }

        private static bool TryGetPreviewSquashEdgeSegmentIndex(int pointCount, MainFrm.SemiAutoType step, out int segmentIndex)
        {
            segmentIndex = -1;
            if (pointCount < 2)
                return false;

            if (step.长角序号 == 0 || step.坐标序号 <= 0)
            {
                segmentIndex = 0;
                return true;
            }

            if (step.长角序号 == 99 || step.坐标序号 >= pointCount - 1)
            {
                segmentIndex = pointCount - 2;
                return true;
            }

            return false;
        }

        private bool TryGetCurrentPreviewFoldEndpointIndices(out int firstIndex, out int secondIndex)
        {
            firstIndex = -1;
            secondIndex = -1;
            if (currentPreviewPolyline.Count < 2)
                return false;

            if (MainFrm.IsLegacySemiAutoPlaceholder(currentPreviewDisplayStep))
                return false;
            if (currentPreviewDisplayStep.行动类型 != MainFrm.SemiAutoActionFold)
                return false;

            int anchorIndex = Math.Clamp(currentPreviewDisplayStep.坐标序号, 0, currentPreviewPolyline.Count - 1);
            bool feedSideUsesLowerIndex = currentPreviewDisplayStep.内外选择 == 1;
            int activeNeighborIndex = feedSideUsesLowerIndex
                ? anchorIndex + 1
                : anchorIndex - 1;
            int feedNeighborIndex = feedSideUsesLowerIndex
                ? anchorIndex - 1
                : anchorIndex + 1;
            int neighborIndex = IsValidPreviewPolylineIndex(activeNeighborIndex)
                ? activeNeighborIndex
                : feedNeighborIndex;
            if (!IsValidPreviewPolylineIndex(neighborIndex))
            {
                neighborIndex = feedSideUsesLowerIndex
                    ? anchorIndex - 1
                    : anchorIndex + 1;
            }
            if (!IsValidPreviewPolylineIndex(neighborIndex))
                return false;

            firstIndex = anchorIndex;
            secondIndex = neighborIndex;
            return true;
        }

        private bool IsValidPreviewPolylineIndex(int index)
        {
            return index >= 0 && index < currentPreviewPolyline.Count;
        }

        private List<string> BuildCurrentPreviewFoldInfoLines()
        {
            List<string> lines = new();
            if (currentPreviewDisplayStep.行动类型 == MainFrm.SemiAutoActionFlip
                || currentPreviewDisplayStep.行动类型 == MainFrm.SemiAutoActionSlit)
            {
                return lines;
            }

            double length = GetCurrentPreviewFoldLength();
            double angle = GetCurrentPreviewDisplayAngle();
            string direction = GetCurrentPreviewFoldDirectionText();
            if (MainFrm.Lang == 0)
            {
                lines.Add($"当前边长: {MainFrm.FormatDisplayLength(length)}");
                lines.Add($"折弯角度: {angle:0.0}°");
                lines.Add($"折弯方向: {direction}");
            }
            else
            {
                lines.Add($"Length: {MainFrm.FormatDisplayLength(length)}");
                lines.Add($"Angle: {angle:0.0}°");
                lines.Add($"Direction: {direction}");
            }

            return lines;
        }

        private double GetCurrentPreviewFoldLength()
        {
            if (TryGetCurrentPreviewSquashEdgeIndex(out int squashEdgeIndex))
                return MainFrm.CurtOrder.lengAngle[squashEdgeIndex].Length;

            int targetIndex = currentPreviewDisplayStep.长角序号;
            if (targetIndex >= 0
                && targetIndex < MainFrm.CurtOrder.lengAngle.Length
                && MainFrm.CurtOrder.lengAngle[targetIndex].Length > 0)
            {
                return MainFrm.CurtOrder.lengAngle[targetIndex].Length;
            }

            if (TryGetCurrentPreviewFoldEndpointIndices(out int firstIndex, out int secondIndex))
            {
                PointF first = currentPreviewPolyline[firstIndex];
                PointF second = currentPreviewPolyline[secondIndex];
                return Math.Sqrt(Math.Pow(second.X - first.X, 2) + Math.Pow(second.Y - first.Y, 2));
            }

            return 0;
        }

        private bool TryGetCurrentPreviewSquashEdgeIndex(out int edgeIndex)
        {
            edgeIndex = ResolvePreviewSquashEdgeIndex(currentPreviewDisplayStep);
            if (!IsCurrentPreviewSquashDisplayStep())
                return false;

            if (HasValidSquashEdgeLength(edgeIndex))
                return true;

            int displayIndex = GetDisplayedPreviewStepIndex(GetCurrentPreviewStepIndex());
            if (MainFrm.IsSemiAutoSquashAction(currentPreviewDisplayStep.行动类型)
                && displayIndex > 0
                && MainFrm.IsLegacySemiAutoPlaceholder(MainFrm.CurtOrder.lstSemiAuto[displayIndex - 1]))
            {
                MainFrm.SemiAutoType preformStep = MainFrm.CurtOrder.lstSemiAuto[displayIndex - 1];
                int preformEdgeIndex = preformStep.长角序号 == 99 ? 99 : 0;
                if (HasValidSquashEdgeLength(preformEdgeIndex))
                {
                    edgeIndex = preformEdgeIndex;
                    return true;
                }
            }

            return false;
        }

        private static int ResolvePreviewSquashEdgeIndex(MainFrm.SemiAutoType step)
        {
            if (step.长角序号 == 99)
                return 99;
            if (step.长角序号 == 0)
                return 0;
            if (MainFrm.CurtOrder.pxList.Count > 0 && step.坐标序号 >= MainFrm.CurtOrder.pxList.Count - 1)
                return 99;

            return 0;
        }

        private static bool HasValidSquashEdgeLength(int edgeIndex)
        {
            return edgeIndex >= 0
                && edgeIndex < MainFrm.CurtOrder.lengAngle.Length
                && MainFrm.CurtOrder.lengAngle[edgeIndex].Length > 0;
        }

        private double GetCurrentPreviewDisplayAngle()
        {
            return Math.Abs(currentPreviewDisplayStep.折弯角度);
        }

        private string GetCurrentPreviewFoldDirectionText()
        {
            int direction = GetCurrentPreviewEffectiveDirection();
            return LocalizationText.FoldDirectionShort(direction);
        }

        private int GetCurrentPreviewEffectiveDirection()
        {
            int displayIndex = GetDisplayedPreviewStepIndex(GetCurrentPreviewStepIndex());
            if (displayIndex < 0)
                return currentPreviewDisplayStep.折弯方向;
            if (MainFrm.IsSemiAutoSquashAction(currentPreviewDisplayStep.行动类型)
                && displayIndex > 0
                && MainFrm.IsLegacySemiAutoPlaceholder(MainFrm.CurtOrder.lstSemiAuto[displayIndex - 1]))
            {
                return MainFrm.CurtOrder.lstSemiAuto[displayIndex - 1].折弯方向;
            }

            return MainFrm.ResolveEffectivePreviewDirection(MainFrm.CurtOrder, MainFrm.CurtOrder.lstSemiAuto, displayIndex);
        }

    }
}
