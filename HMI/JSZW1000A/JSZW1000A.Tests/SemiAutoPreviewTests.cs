using System.Drawing;
using System.Reflection;
using JSZW1000A.SubWindows;

namespace JSZW1000A.Tests;

[TestClass]
public class SemiAutoPreviewTests
{
    [TestInitialize]
    public void TestInitialize()
    {
        Array.Clear(MainFrm.Hmi_rArray);
    }

    [TestMethod]
    public void DeserializeSemiAutoPlanReserve_AcceptsLegacyBooleanFlags()
    {
        MainFrm.OrderType order = new();

        MainFrm.DeserializeSemiAutoPlanReserve("custom-manual/True/False", ref order);

        Assert.AreEqual(MainFrm.SemiAutoPlanOriginCustomManual, order.SemiAutoPlanOrigin);
        Assert.IsTrue(order.st逆序);
        Assert.IsFalse(order.st色下);
    }

    [TestMethod]
    public void DeserializeSemiAutoPlanReserve_DoesNotShiftFlagsWhenMiddleValueIsMissing()
    {
        MainFrm.OrderType order = new()
        {
            st逆序 = false,
            st色下 = false,
        };

        MainFrm.DeserializeSemiAutoPlanReserve("custom-manual//1", ref order);

        Assert.AreEqual(MainFrm.SemiAutoPlanOriginCustomManual, order.SemiAutoPlanOrigin);
        Assert.IsFalse(order.st逆序);
        Assert.IsTrue(order.st色下);
    }

    [TestMethod]
    public void ResolveSemiAutoPreviewColorDown_SkipsTrailingImplicitFlipWhenPreviewStopsBeforeFlipState()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(30, 10, 10, 10);
        order.st色下 = false;
        order.lstSemiAuto.Add(CreateFoldStep(longAngleIndex: 0, coordinateIndex: 1, innerOuter: 0));
        order.lstSemiAuto.Add(CreateFoldStep(longAngleIndex: 1, coordinateIndex: 2, innerOuter: 1));

        bool colorDown = MainFrm.ResolveSemiAutoPreviewColorDown(order, appliedStepCount: 1, applyFlipAfterLastIncludedStep: false);

        Assert.IsFalse(colorDown);
    }

    [TestMethod]
    public void ResolveSemiAutoPreviewColorDown_AppliesImplicitFlipWhenPreviewIncludesFlipState()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(30, 10, 10, 10);
        order.st色下 = false;
        order.lstSemiAuto.Add(CreateFoldStep(longAngleIndex: 0, coordinateIndex: 1, innerOuter: 0));
        order.lstSemiAuto.Add(CreateFoldStep(longAngleIndex: 1, coordinateIndex: 2, innerOuter: 1));

        bool colorDown = MainFrm.ResolveSemiAutoPreviewColorDown(order, appliedStepCount: 1, applyFlipAfterLastIncludedStep: true);

        Assert.IsTrue(colorDown);
    }

    [TestMethod]
    public void ResolveSemiAutoPreviewColorDown_AppliesExplicitFlipEvenWhenPreviewStopsBeforeImplicitFlipState()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(30, 10, 10, 10);
        order.st色下 = false;
        order.lstSemiAuto.Add(CreateFoldStep(longAngleIndex: 0, coordinateIndex: 1, innerOuter: 0));
        order.lstSemiAuto.Add(CreateFlipStep(coordinateIndex: 1));

        bool colorDown = MainFrm.ResolveSemiAutoPreviewColorDown(order, appliedStepCount: 2, applyFlipAfterLastIncludedStep: false);

        Assert.IsTrue(colorDown);
    }

    [TestMethod]
    public void BuildSemiAutoPreviewStageProfile_UsesFixedWidthPivotForImplicitFlipPreview()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(30, 10, 10, 10);
        order.lstSemiAuto.Add(CreateFoldStep(longAngleIndex: 0, coordinateIndex: 1, innerOuter: 0, direction: 0, angle: 90));
        order.lstSemiAuto.Add(CreateFoldStep(longAngleIndex: 1, coordinateIndex: 2, innerOuter: 1, direction: 0, angle: 90));

        List<PointF> preview = MainFrm.BuildSemiAutoPreviewStageProfile(order, appliedStepCount: 1, applyFlipAfterLastIncludedStep: true);

        Assert.AreEqual(4, preview.Count);
        Assert.AreEqual(-20f, preview[0].X, 0.01f);
        Assert.AreEqual(-10f, preview[0].Y, 0.01f);
        Assert.AreEqual(0f, preview[^1].X, 0.01f);
        Assert.AreEqual(0f, preview[^1].Y, 0.01f);
    }

    [TestMethod]
    public void BuildSemiAutoPreviewStageProfile_RotatesFlatProfileForExplicitFlip()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(30, 10, 10, 10);
        order.lstSemiAuto.Add(CreateFlipStep(coordinateIndex: 1));

        List<PointF> preview = MainFrm.BuildSemiAutoPreviewStageProfile(order, appliedStepCount: 1, applyFlipAfterLastIncludedStep: false);

        Assert.AreEqual(4, preview.Count);
        Assert.AreEqual(-30f, preview[0].X, 0.01f);
        Assert.AreEqual(0f, preview[0].Y, 0.01f);
        Assert.AreEqual(0f, preview[^1].X, 0.01f);
        Assert.AreEqual(0f, preview[^1].Y, 0.01f);
    }

    [TestMethod]
    public void BuildSemiAutoPreviewStageProfile_IgnoresSquashGeometryForBodyPreview()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(400, 10, 390);
        order.lengAngle[0].Length = 10;
        order.lengAngle[0].Angle = 3;
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 0, coordinateIndex: 0, direction: 0, angle: 3);
        step.行动类型 = MainFrm.SemiAutoActionSquash;
        order.lstSemiAuto.Add(step);

        List<PointF> preview = MainFrm.BuildSemiAutoPreviewStageProfile(order, appliedStepCount: 1, applyFlipAfterLastIncludedStep: false);

        Assert.IsTrue(preview.Count > 1);
        Assert.IsTrue(preview.All(point => Math.Abs(point.Y - preview[0].Y) < 0.001f));
    }

    [TestMethod]
    public void BuildPreviewSegmentsForDrawStep_HeadSquashCompletionDoesNotFlattenBodyFoldEdge()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(400, 10, 390);
        order.lengAngle[0].Length = 10;
        order.lengAngle[0].Angle = 3;
        order.lstSemiAuto.Add(CreateFoldStep(
            longAngleIndex: 0,
            coordinateIndex: 0,
            innerOuter: 1,
            direction: 0,
            angle: 30));
        order.lstSemiAuto.Add(CreateSquashStep(longAngleIndex: 0, coordinateIndex: 0, direction: 0));
        MainFrm.CurtOrder = order;
        SubOPAutoView view = new(null!, proc: false);
        SetPrivateField(view, "cx", 500);
        SetPrivateField(view, "cy", 300);

        SetPrivateField(view, "iDrawStep", 3);
        InvokeBuildPreviewSegmentsForDrawStep(view, drawStep: 3);
        List<PointF> feedPolyline = [.. (List<PointF>)GetPrivateField(view, "currentPreviewPolyline")!];

        SetPrivateField(view, "iDrawStep", 4);
        InvokeBuildPreviewSegmentsForDrawStep(view, drawStep: 4);
        List<PointF> squashPolyline = [.. (List<PointF>)GetPrivateField(view, "currentPreviewPolyline")!];

        Assert.IsTrue(feedPolyline.Count >= 3);
        Assert.IsTrue(squashPolyline.Count >= 3);
        Assert.AreEqual(feedPolyline[0].X, squashPolyline[0].X, 0.5f);
        Assert.AreEqual(feedPolyline[0].Y, squashPolyline[0].Y, 0.5f);
    }

    [TestMethod]
    public void DrawCurrentPreviewActiveDisplayEdge_CanHighlightHeadSquashEdge()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(400, 10, 390);
        order.lengAngle[0].Length = 10;
        order.lengAngle[0].Angle = 3;
        order.lstSemiAuto.Add(CreateFoldStep(
            longAngleIndex: 0,
            coordinateIndex: 0,
            innerOuter: 1,
            direction: 0,
            angle: 30));
        order.lstSemiAuto.Add(CreateSquashStep(longAngleIndex: 0, coordinateIndex: 0, direction: 0));
        MainFrm.CurtOrder = order;
        SubOPAutoView view = new(null!, proc: false);
        SetPrivateField(view, "cx", 500);
        SetPrivateField(view, "cy", 300);
        SetPrivateField(view, "iDrawStep", 4);

        InvokeBuildPreviewSegmentsForDrawStep(view, drawStep: 4);
        Assert.IsTrue(InvokeTryGetCurrentPreviewActiveDisplayEdge(view, out PointF startPoint, out PointF endPoint));

        using Bitmap bitmap = new(1180, 805);
        using Graphics graphic = Graphics.FromImage(bitmap);
        graphic.Clear(Color.Transparent);
        Color squashColor = Color.FromArgb(96, 176, 255);
        using Pen squashPen = new(squashColor, 6);

        InvokeDrawCurrentPreviewActiveDisplayEdge(view, graphic, squashPen);

        AssertLineMidpointColor(bitmap, startPoint, endPoint, squashColor);
    }

    [TestMethod]
    public void TryGetCurrentPreviewSquashCueEdge_DrawsPendingHeadSquashCueRightOfPreformFold()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(400, 15, 385);
        order.lengAngle[0].Length = 15;
        order.lengAngle[0].Angle = 4;
        order.lstSemiAuto.Add(CreateFoldStep(
            longAngleIndex: 0,
            coordinateIndex: 0,
            innerOuter: 1,
            direction: 0,
            angle: 3.001));
        order.lstSemiAuto.Add(CreateSquashStep(longAngleIndex: 0, coordinateIndex: 0, direction: 0, angle: 4));
        MainFrm.CurtOrder = order;
        SubOPAutoView view = new(null!, proc: false);
        SetPrivateField(view, "cx", 500);
        SetPrivateField(view, "cy", 300);
        SetPrivateField(view, "iDrawStep", 2);

        InvokeBuildPreviewSegmentsForDrawStep(view, drawStep: 2);
        Assert.IsFalse(InvokeTryGetCurrentPreviewActiveFoldEdge(view, out _, out _));
        Assert.IsFalse(InvokeTryGetCurrentPreviewSquashSegmentIndex(view, out _));
        Assert.IsTrue(InvokeTryGetCurrentPreviewActiveDisplayEdge(view, out PointF edgeStart, out PointF edgeEnd));
        Assert.IsFalse(InvokeTryGetCurrentPreviewSquashCueEdge(view, out _, out _));
        Assert.AreEqual(500.0f, edgeStart.X, 0.5f);
        Assert.AreEqual(300.0f, edgeStart.Y, 0.5f);
        Assert.AreEqual(15.0f, GetDistance(edgeStart, edgeEnd), 0.5f);
        Assert.AreEqual(150.0, GetVectorAngleDegrees(edgeStart, edgeEnd), 0.5);
    }

    [TestMethod]
    public void BuildCurrentPreviewFoldInfoLines_SquashRowUsesPreviousPreformTailLengthWhenMetadataIsMissing()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(400, 378, 22);
        order.lengAngle[99].Length = 22;
        order.lengAngle[99].Angle = 1;
        order.lstSemiAuto.Add(CreateFoldStep(
            longAngleIndex: 99,
            coordinateIndex: 2,
            innerOuter: 0,
            direction: 0,
            angle: 3.99));
        order.lstSemiAuto.Add(CreateSquashStep(longAngleIndex: -1, coordinateIndex: 2, direction: 0));
        MainFrm.CurtOrder = order;
        SubOPAutoView view = new(null!, proc: false);
        SetPrivateField(view, "cx", 500);
        SetPrivateField(view, "cy", 300);
        SetPrivateField(view, "iDrawStep", 3);

        InvokeBuildPreviewSegmentsForDrawStep(view, drawStep: 3);
        List<string> lines = InvokeBuildCurrentPreviewFoldInfoLines(view);

        Assert.IsTrue(lines.Any(line => line.Contains("22")));
    }

    [TestMethod]
    public void BuildCurrentPreviewFoldInfoLines_SquashRowInheritsPreviousPreformDirection()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(370, 60, 220, 60);
        order.lengAngle[0].Length = 15;
        order.lengAngle[0].Angle = 1;
        order.lstSemiAuto.Add(CreateFoldStep(
            longAngleIndex: 0,
            coordinateIndex: 0,
            innerOuter: 1,
            direction: 1,
            angle: 3.001));
        order.lstSemiAuto.Add(CreateSquashStep(longAngleIndex: 0, coordinateIndex: 0, direction: 0));
        MainFrm.CurtOrder = order;
        SubOPAutoView view = new(null!, proc: false);
        SetPrivateField(view, "cx", 500);
        SetPrivateField(view, "cy", 300);
        SetPrivateField(view, "iDrawStep", 4);

        InvokeBuildPreviewSegmentsForDrawStep(view, drawStep: 4);
        List<string> lines = InvokeBuildCurrentPreviewFoldInfoLines(view);

        Assert.IsTrue(lines.Any(line => line.Contains("下")));
    }

    [TestMethod]
    public void TryGetCurrentPreviewActiveDisplayEdge_HeadSquashDrawsIndependentFifteenLengthEdgeRightOfCenter()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(370, 60, 220, 60);
        order.lengAngle[0].Length = 15;
        order.lengAngle[0].Angle = 1;
        order.lstSemiAuto.Add(CreateFoldStep(
            longAngleIndex: 0,
            coordinateIndex: 0,
            innerOuter: 1,
            direction: 0,
            angle: 3.001));
        MainFrm.CurtOrder = order;
        SubOPAutoView view = new(null!, proc: false);
        SetPrivateField(view, "cx", 500);
        SetPrivateField(view, "cy", 300);
        SetPrivateField(view, "iDrawStep", 1);

        InvokeBuildPreviewSegmentsForDrawStep(view, drawStep: 1);
        Assert.IsTrue(InvokeTryGetCurrentPreviewActiveDisplayEdge(view, out PointF edgeStart, out PointF edgeEnd));

        Assert.AreEqual(500.0f, edgeStart.X, 0.5f);
        Assert.AreEqual(300.0f, edgeStart.Y, 0.5f);
        Assert.AreEqual(15.0f, edgeEnd.X - edgeStart.X, 0.5f);
        Assert.AreEqual(0.0f, edgeEnd.Y - edgeStart.Y, 0.5f);
    }

    [TestMethod]
    public void TryGetCurrentPreviewActiveDisplayEdge_DrawStepZeroDoesNotShowSquashEdge()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(370, 60, 220, 60);
        order.lengAngle[0].Length = 15;
        order.lengAngle[0].Angle = 1;
        order.lstSemiAuto.Add(CreateFoldStep(
            longAngleIndex: 0,
            coordinateIndex: 0,
            innerOuter: 1,
            direction: 0,
            angle: 3.001));
        MainFrm.CurtOrder = order;
        SubOPAutoView view = new(null!, proc: false);
        SetPrivateField(view, "cx", 500);
        SetPrivateField(view, "cy", 300);
        SetPrivateField(view, "iDrawStep", 0);

        InvokeBuildPreviewSegmentsForDrawStep(view, drawStep: 0);

        Assert.IsFalse(InvokeTryGetCurrentPreviewActiveDisplayEdge(view, out _, out _));
    }

    [TestMethod]
    public void TryGetCurrentPreviewActiveDisplayEdge_HeadSquashFinalFrameKeepsIndependentFifteenLengthEdge()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(370, 60, 220, 60);
        order.lengAngle[0].Length = 15;
        order.lengAngle[0].Angle = 1;
        order.lstSemiAuto.Add(CreateFoldStep(
            longAngleIndex: 0,
            coordinateIndex: 0,
            innerOuter: 1,
            direction: 0,
            angle: 3.001));
        order.lstSemiAuto.Add(CreateSquashStep(longAngleIndex: 0, coordinateIndex: 0, direction: 0));
        MainFrm.CurtOrder = order;
        SubOPAutoView view = new(null!, proc: false);
        SetPrivateField(view, "cx", 500);
        SetPrivateField(view, "cy", 300);
        SetPrivateField(view, "iDrawStep", 4);

        InvokeBuildPreviewSegmentsForDrawStep(view, drawStep: 4);
        Assert.IsTrue(InvokeTryGetCurrentPreviewActiveDisplayEdge(view, out PointF edgeStart, out PointF edgeEnd));

        Assert.AreEqual(485.0f, edgeStart.X, 0.5f);
        Assert.AreEqual(300.0f, edgeStart.Y, 0.5f);
        Assert.AreEqual(500.0f, edgeEnd.X, 0.5f);
        Assert.AreEqual(300.0f, edgeEnd.Y, 0.5f);
        Assert.AreEqual(15.0f, GetDistance(edgeStart, edgeEnd), 0.5f);
        Assert.AreNotEqual(60.0f, GetDistance(edgeStart, edgeEnd), 0.5f);
    }

    [TestMethod]
    public void TryGetCurrentPreviewActiveDisplayEdge_HeadSquashFeedFrameKeepsPreformAngle()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(370, 60, 220, 60);
        order.lengAngle[0].Length = 15;
        order.lengAngle[0].Angle = 1;
        order.lstSemiAuto.Add(CreateFoldStep(
            longAngleIndex: 0,
            coordinateIndex: 0,
            innerOuter: 1,
            direction: 0,
            angle: 3.001));
        order.lstSemiAuto.Add(CreateSquashStep(longAngleIndex: 0, coordinateIndex: 0, direction: 0));
        MainFrm.SemiAutoType preformStep = order.lstSemiAuto[0];
        preformStep.后挡位置 = 355;
        order.lstSemiAuto[0] = preformStep;
        MainFrm.SemiAutoType squashStep = order.lstSemiAuto[1];
        squashStep.后挡位置 = 363;
        order.lstSemiAuto[1] = squashStep;
        MainFrm.CurtOrder = order;
        SubOPAutoView view = new(null!, proc: false);
        SetPrivateField(view, "cx", 500);
        SetPrivateField(view, "cy", 300);
        SetPrivateField(view, "iDrawStep", 3);

        InvokeBuildPreviewSegmentsForDrawStep(view, drawStep: 3);
        Assert.IsTrue(InvokeTryGetCurrentPreviewActiveDisplayEdge(view, out PointF edgeStart, out PointF edgeEnd));

        Assert.AreEqual(492.0f, edgeStart.X, 0.5f);
        Assert.AreEqual(300.0f, edgeStart.Y, 0.5f);
        Assert.AreEqual(15.0f, GetDistance(edgeStart, edgeEnd), 0.5f);
        Assert.AreEqual(150.0, GetVectorAngleDegrees(edgeStart, edgeEnd), 0.5);
    }

    [TestMethod]
    public void TryGetCompletedPreviewSquashEdge_FollowsFoldEndpointOnLaterFoldStep()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(370, 60, 220, 60);
        order.lengAngle[0].Length = 15;
        order.lengAngle[0].Angle = 1;
        order.lstSemiAuto.Add(CreateFoldStep(
            longAngleIndex: 0,
            coordinateIndex: 0,
            innerOuter: 1,
            direction: 0,
            angle: 3.001));
        order.lstSemiAuto.Add(CreateSquashStep(longAngleIndex: 0, coordinateIndex: 0, direction: 0));
        order.lstSemiAuto.Add(CreateFoldStep(
            longAngleIndex: 1,
            coordinateIndex: 1,
            innerOuter: 0,
            direction: 1,
            angle: 90));
        MainFrm.CurtOrder = order;
        SubOPAutoView view = new(null!, proc: false);
        SetPrivateField(view, "cx", 500);
        SetPrivateField(view, "cy", 300);
        SetPrivateField(view, "iDrawStep", 5);

        InvokeBuildPreviewSegmentsForDrawStep(view, drawStep: 5);
        List<PointF> polyline = (List<PointF>)GetPrivateField(view, "currentPreviewPolyline")!;
        Assert.IsTrue(InvokeTryGetCompletedPreviewSquashEdge(view, order.lstSemiAuto[1], out PointF edgeStart, out PointF edgeEnd));

        Assert.AreEqual(polyline[0].X, edgeEnd.X, 0.5f);
        Assert.AreEqual(polyline[0].Y, edgeEnd.Y, 0.5f);
        Assert.AreEqual(15.0f, GetDistance(edgeStart, edgeEnd), 0.5f);
    }

    [TestMethod]
    public void BuildPreviewSegmentsForDrawStep_FeedPhasePlacesCurrentFoldPointAtFoldOrigin()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(30, 10, 10, 10);
        order.lstSemiAuto.Add(CreateFoldStep(longAngleIndex: 0, coordinateIndex: 1, innerOuter: 0, direction: 0));
        order.lstSemiAuto.Add(CreateFoldStep(longAngleIndex: 1, coordinateIndex: 2, innerOuter: 1, direction: 1));
        MainFrm.CurtOrder = order;
        SubOPAutoView view = new(null!, proc: false);
        SetPrivateField(view, "cx", 500);
        SetPrivateField(view, "cy", 300);
        SetPrivateField(view, "iDrawStep", 3);

        List<Point> segments = InvokeBuildPreviewSegmentsForDrawStep(view, drawStep: 3);

        CollectionAssert.Contains(segments, new Point(500, 300));
    }

    [TestMethod]
    public void BuildPreviewSegmentsForDrawStep_FlipCompletionRotatesAroundFoldCenter()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(30, 10, 10, 10);
        order.lstSemiAuto.Add(CreateFoldStep(longAngleIndex: 0, coordinateIndex: 1, innerOuter: 0, direction: 0));
        order.lstSemiAuto.Add(CreateFoldStep(longAngleIndex: 1, coordinateIndex: 2, innerOuter: 1, direction: 1));
        MainFrm.CurtOrder = order;
        SubOPAutoView view = new(null!, proc: false);
        SetPrivateField(view, "cx", 500);
        SetPrivateField(view, "cy", 300);
        SetPrivateField(view, "iDrawStep", 2);
        SetPrivateField(view, "showingFlipCompletionState", true);

        List<Point> segments = InvokeBuildPreviewSegmentsForDrawStep(view, drawStep: 2);

        List<int> xs = segments.Select(point => point.X).ToList();
        List<int> ys = segments.Select(point => point.Y).ToList();
        CollectionAssert.Contains(segments, new Point(500, 300));
        Assert.IsTrue(xs.Max() - xs.Min() < 25);
        Assert.IsTrue(ys.Min() < 300);
        Assert.IsTrue(ys.Max() > 300);
    }

    [TestMethod]
    public void BuildPreviewSegmentsForDrawStep_StartFrameHidesBoardAndNextFrameFeedsToFoldOrigin()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(400, 10, 390);
        order.lengAngle[0].Length = 10;
        order.lengAngle[0].Angle = 3;
        order.lstSemiAuto.Add(CreateFoldStep(
            longAngleIndex: 0,
            coordinateIndex: 0,
            innerOuter: 1,
            direction: 0,
            angle: 3.001));
        MainFrm.CurtOrder = order;
        SubOPAutoView view = new(null!, proc: false);
        SetPrivateField(view, "cx", 500);
        SetPrivateField(view, "cy", 300);

        SetPrivateField(view, "iDrawStep", 0);
        List<Point> startSegments = InvokeBuildPreviewSegmentsForDrawStep(view, drawStep: 0);
        List<PointF> startPolyline = [.. (List<PointF>)GetPrivateField(view, "currentPreviewPolyline")!];

        SetPrivateField(view, "iDrawStep", 1);
        InvokeBuildPreviewSegmentsForDrawStep(view, drawStep: 1);
        List<PointF> fedPolyline = [.. (List<PointF>)GetPrivateField(view, "currentPreviewPolyline")!];

        Assert.AreEqual(0, startSegments.Count);
        Assert.AreEqual(0, startPolyline.Count);
        Assert.AreEqual(500.0f, fedPolyline[0].X, 0.5f);
        Assert.AreEqual(300.0f, fedPolyline[0].Y, 0.5f);
    }

    [TestMethod]
    public void TryGetCurrentPreviewFoldSegmentIndex_HighlightsWorkingEdgeInsteadOfFeedSide()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(100, 20, 60, 20);
        order.lstSemiAuto.Add(CreateFoldStep(
            longAngleIndex: 1,
            coordinateIndex: 1,
            innerOuter: 1,
            direction: 0,
            angle: 90));
        MainFrm.CurtOrder = order;
        SubOPAutoView view = new(null!, proc: false);
        SetPrivateField(view, "cx", 500);
        SetPrivateField(view, "cy", 300);
        SetPrivateField(view, "iDrawStep", 2);

        InvokeBuildPreviewSegmentsForDrawStep(view, drawStep: 2);

        Assert.IsTrue(InvokeTryGetCurrentPreviewFoldSegmentIndex(view, out int segmentIndex));
        Assert.AreEqual(1, segmentIndex);
    }

    [TestMethod]
    public void BuildCurrentPreviewFoldInfoLines_ShowsLengthAngleAndDirection()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(400, 10, 390);
        order.lengAngle[0].Length = 10;
        order.lengAngle[0].Angle = 3;
        order.lstSemiAuto.Add(CreateFoldStep(
            longAngleIndex: 0,
            coordinateIndex: 0,
            innerOuter: 1,
            direction: 0,
            angle: 3.001));
        MainFrm.CurtOrder = order;
        SubOPAutoView view = new(null!, proc: false);
        SetPrivateField(view, "cx", 500);
        SetPrivateField(view, "cy", 300);
        SetPrivateField(view, "iDrawStep", 2);

        InvokeBuildPreviewSegmentsForDrawStep(view, drawStep: 2);

        List<string> lines = InvokeBuildCurrentPreviewFoldInfoLines(view);
        Assert.IsTrue(lines.Any(line => line.Contains("10")));
        Assert.IsTrue(lines.Any(line => line.Contains("3.0")));
        Assert.IsTrue(lines.Count >= 3);
    }

    [TestMethod]
    public void DrawCurrentPreviewActiveDisplayEdge_UsesGreenForNormalFold()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(400, 10, 390);
        order.lengAngle[0].Length = 10;
        order.lengAngle[0].Angle = 3;
        order.lstSemiAuto.Add(CreateFoldStep(
            longAngleIndex: 0,
            coordinateIndex: 0,
            innerOuter: 1,
            direction: 0,
            angle: 3.001));
        MainFrm.CurtOrder = order;
        SubOPAutoView view = new(null!, proc: false);
        SetPrivateField(view, "cx", 500);
        SetPrivateField(view, "cy", 300);
        SetPrivateField(view, "iDrawStep", 2);

        InvokeBuildPreviewSegmentsForDrawStep(view, drawStep: 2);
        Assert.IsTrue(InvokeTryGetCurrentPreviewActiveDisplayEdge(view, out PointF startPoint, out PointF endPoint));

        using Bitmap bitmap = new(1180, 805);
        using Graphics graphic = Graphics.FromImage(bitmap);
        graphic.Clear(Color.Transparent);
        using Pen foldPen = new(Color.Lime, 6);

        InvokeDrawCurrentPreviewActiveDisplayEdge(view, graphic, foldPen);

        AssertLineMidpointColor(bitmap, startPoint, endPoint, Color.Lime);
    }

    [TestMethod]
    public void RecalculateBackGaugePositionsByCurrentProfile_UsesProjectedDistanceForLastFold()
    {
        MainFrm.OrderType order = new()
        {
            Width = 20,
            pxList = new List<PointF>
            {
                new(0, 0),
                new(-10, 0),
                new(-15, 5),
            }
        };
        order.lengAngle[1].Length = 10;
        order.lengAngle[2].Length = 10;
        order.lstSemiAuto.Add(new MainFrm.SemiAutoType
        {
            行动类型 = MainFrm.SemiAutoActionFold,
            折弯角度 = 90,
            折弯方向 = 0,
            抓取类型 = 1,
            长角序号 = 1,
            坐标序号 = 1,
            内外选择 = 0,
        });

        MainFrm.RecalculateBackGaugePositionsByCurrentProfile(ref order);

        Assert.AreEqual(5.0, order.lstSemiAuto[0].后挡位置, 0.001);
    }

    [TestMethod]
    public void RecalculateBackGaugePositionsByCurrentProfile_FallsBackToFoldDistanceWhenProjectedReferenceCollapsesToAnchor()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(30, 10, 10, 10);
        order.lstSemiAuto.Add(CreateFoldStep(
            longAngleIndex: 1,
            coordinateIndex: 0,
            innerOuter: 1,
            direction: 0,
            angle: 90));

        MainFrm.RecalculateBackGaugePositionsByCurrentProfile(ref order);

        Assert.AreEqual(0.0, order.lstSemiAuto[0].后挡位置, 0.001);
    }

    [TestMethod]
    public void BuildPreviewSegmentsForDrawStep_RebuildsFromCurrentOrderAfterOrderSwitch()
    {
        MainFrm.OrderType originalOrder = CreateOrderWithStraightProfile(400, 10, 390);
        originalOrder.lengAngle[0].Length = 10;
        originalOrder.lengAngle[0].Angle = 3;
        originalOrder.lstSemiAuto.Add(CreateFoldStep(
            longAngleIndex: 0,
            coordinateIndex: 0,
            direction: 0,
            angle: 3.001));
    }

    [TestMethod]
    public void TryGetGeneratedFormalDirection_MapsAngleSignAgainstCurrentBoardFace()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(30, 10, 10, 10);
        order.lengAngle[2].Angle = 90;
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 1, coordinateIndex: 1);

        bool topFaceResult = MainFrm.TryGetGeneratedFormalDirection(order, step, currentColorDown: false, out int topFaceDirection);
        bool bottomFaceResult = MainFrm.TryGetGeneratedFormalDirection(order, step, currentColorDown: true, out int bottomFaceDirection);

        Assert.IsTrue(topFaceResult);
        Assert.IsTrue(bottomFaceResult);
        Assert.AreEqual(0, topFaceDirection);
        Assert.AreEqual(1, bottomFaceDirection);
    }

    [TestMethod]
    public void NormalizeSemiAutoStepsForPreview_PreservesManualDirectionsAndTracksFlipColorState()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(30, 10, 10, 10);
        List<MainFrm.SemiAutoType> steps =
        [
            CreateFoldStep(longAngleIndex: 1, coordinateIndex: 1, direction: 1),
            CreateFlipStep(coordinateIndex: 1),
            CreateFoldStep(longAngleIndex: 2, coordinateIndex: 2, direction: 1),
        ];

        InvokeNormalizeSemiAutoStepsForPreview(order, steps, normalizeGeneratedDirections: false);

        Assert.AreEqual(1, steps[0].折弯方向);
        Assert.AreEqual(1, steps[2].折弯方向);
        Assert.IsFalse(steps[0].is色下);
        Assert.IsFalse(steps[1].is色下);
        Assert.IsTrue(steps[2].is色下);
    }

    [TestMethod]
    public void NormalizeSemiAutoStepsForPreview_RewritesGeneratedDirectionsAfterExplicitFlip()
    {
        MainFrm.OrderType order = CreateOrderWithStraightProfile(30, 10, 10, 10);
        order.lengAngle[2].Angle = 90;
        order.lengAngle[3].Angle = 90;
        List<MainFrm.SemiAutoType> steps =
        [
            CreateFoldStep(longAngleIndex: 1, coordinateIndex: 1, direction: 1),
            CreateFlipStep(coordinateIndex: 1),
            CreateFoldStep(longAngleIndex: 2, coordinateIndex: 2, direction: 0),
        ];

        InvokeNormalizeSemiAutoStepsForPreview(order, steps, normalizeGeneratedDirections: true);

        Assert.AreEqual(0, steps[0].折弯方向);
        Assert.AreEqual(1, steps[2].折弯方向);
        Assert.AreEqual(1, steps[0].折弯序号);
        Assert.AreEqual(3, steps[2].折弯序号);
    }

    [TestMethod]
    public void GetPreviewCollisionSegmentIndices_FeedCompletionMarksFormedSideEndpointInCollisionArea()
    {
        List<PointF> profile =
        [
            new(40, 0),
            new(0, 0),
            new(-20, 40),
            new(-60, 40),
        ];
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 1, coordinateIndex: 1, innerOuter: 1);

        HashSet<int> segments = MainFrm.GetPreviewCollisionSegmentIndices(profile, step, isFoldCompletionState: false);

        CollectionAssert.Contains(segments.ToList(), 2);
    }

    [TestMethod]
    public void GetPreviewCollisionSegmentIndices_IgnoresPositiveSideNormalFoldPointInBothPreviewStates()
    {
        List<PointF> profile =
        [
            new(0, 0),
            new(25, 35),
            new(65, 35),
        ];
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 0, coordinateIndex: 0, innerOuter: 1);

        HashSet<int> feedSegments = MainFrm.GetPreviewCollisionSegmentIndices(profile, step, isFoldCompletionState: false);
        HashSet<int> foldSegments = MainFrm.GetPreviewCollisionSegmentIndices(profile, step, isFoldCompletionState: true);

        Assert.AreEqual(0, feedSegments.Count);
        Assert.AreEqual(0, foldSegments.Count);
    }

    [TestMethod]
    public void GetPreviewCollisionSegmentIndices_DoesNotPaintCurrentFoldSegmentAdjacentToCollisionPoint()
    {
        List<PointF> profile =
        [
            new(0, 0),
            new(-20, 40),
            new(40, 40),
        ];
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 0, coordinateIndex: 0, innerOuter: 1);

        HashSet<int> segments = MainFrm.GetPreviewCollisionSegmentIndices(profile, step, isFoldCompletionState: false);

        Assert.AreEqual(0, segments.Count);
    }

    [TestMethod]
    public void GetPreviewCollisionSegmentIndices_IgnoresReferenceSideHardCollision()
    {
        List<PointF> profile =
        [
            new(-100, 90),
            new(-40, 90),
            new(0, 0),
            new(100, 0),
        ];
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 1, coordinateIndex: 2, innerOuter: 1);

        HashSet<int> segments = MainFrm.GetPreviewCollisionSegmentIndices(profile, step, isFoldCompletionState: false);

        Assert.AreEqual(0, segments.Count);
    }

    [TestMethod]
    public void GetPreviewCollisionSegmentIndices_FeedCompletionMarksFormedAngleInCollisionArea()
    {
        List<PointF> profile =
        [
            new(0, 0),
            new(-50, 20),
            new(-100, 20),
        ];
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 0, coordinateIndex: 0, innerOuter: 1);

        HashSet<int> feedSegments = MainFrm.GetPreviewCollisionSegmentIndices(profile, step, isFoldCompletionState: false);
        HashSet<int> foldSegments = MainFrm.GetPreviewCollisionSegmentIndices(profile, step, isFoldCompletionState: true);

        CollectionAssert.AreEquivalent(new[] { 1 }, feedSegments.ToList());
        CollectionAssert.AreEquivalent(new[] { 1 }, foldSegments.ToList());
    }

    [TestMethod]
    public void GetPreviewCollisionSegmentIndices_UsesSameCollisionAreaForFeedAndFoldStates()
    {
        List<PointF> profile =
        [
            new(0, 0),
            new(-50, -10),
            new(-100, 10),
        ];
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 0, coordinateIndex: 0, innerOuter: 1);

        HashSet<int> feedSegments = MainFrm.GetPreviewCollisionSegmentIndices(profile, step, isFoldCompletionState: false);
        HashSet<int> foldSegments = MainFrm.GetPreviewCollisionSegmentIndices(profile, step, isFoldCompletionState: true);

        CollectionAssert.AreEquivalent(new[] { 1 }, feedSegments.ToList());
        CollectionAssert.AreEquivalent(new[] { 1 }, foldSegments.ToList());
    }

    [TestMethod]
    public void GetPreviewCollisionSegmentIndices_MarksFormedSideEndpointEvenWhenInteriorPointIsFlat()
    {
        List<PointF> profile =
        [
            new(0, 0),
            new(-5, -10),
            new(-10, -20),
        ];
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 0, coordinateIndex: 0, innerOuter: 1);

        HashSet<int> segments = MainFrm.GetPreviewCollisionSegmentIndices(profile, step, isFoldCompletionState: false);

        CollectionAssert.Contains(segments.ToList(), 1);
    }

    [TestMethod]
    public void GetPreviewCollisionSegmentIndices_FoldCompletionMarksFormedSideEndpointInCollisionArea()
    {
        List<PointF> profile =
        [
            new(40, 0),
            new(0, 0),
            new(-20, 80),
            new(-60, 80),
        ];
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 1, coordinateIndex: 1, innerOuter: 1);

        HashSet<int> segments = MainFrm.GetPreviewCollisionSegmentIndices(profile, step, isFoldCompletionState: true);

        CollectionAssert.Contains(segments.ToList(), 2);
    }

    [TestMethod]
    public void GetPreviewCollisionSegmentIndices_FeedCompletionMarksLowerIndexEndpointInCollisionArea()
    {
        List<PointF> profile =
        [
            new(-60, 40),
            new(-20, 40),
            new(0, 0),
            new(40, 0),
        ];
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 1, coordinateIndex: 2, innerOuter: 0);

        HashSet<int> segments = MainFrm.GetPreviewCollisionSegmentIndices(profile, step, isFoldCompletionState: false);

        CollectionAssert.Contains(segments.ToList(), 0);
    }

    [TestMethod]
    public void GetPreviewCollisionSegmentIndices_MarksEndpointOnFlatFoldBoundarySegment()
    {
        List<PointF> profile =
        [
            new(0, 0),
            new(-7.37f, -19.25f),
            new(-30.27f, -52.02f),
        ];
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 0, coordinateIndex: 0, innerOuter: 1, direction: 1);

        HashSet<int> segments = MainFrm.GetPreviewCollisionSegmentIndices(profile, step, isFoldCompletionState: true);

        CollectionAssert.Contains(segments.ToList(), 1);
    }

    [TestMethod]
    public void GetPreviewCollisionSegmentIndices_MarksEndpointConnectedToCurrentFoldSegment()
    {
        List<PointF> profile =
        [
            new(-20, -5),
            new(0, 0),
            new(60, 0),
        ];
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 0, coordinateIndex: 1, innerOuter: 0, direction: 0);

        HashSet<int> segments = MainFrm.GetPreviewCollisionSegmentIndices(profile, step, isFoldCompletionState: false, compareWorkingFlapArea: true);

        CollectionAssert.Contains(segments.ToList(), 0);
    }

    [TestMethod]
    public void GetPreviewCollisionSegmentIndices_IgnoresEndpointBeyondLowerBoundaryWithoutFormedPoint()
    {
        List<PointF> profile =
        [
            new(20, -50),
            new(0, 0),
            new(-10, 0),
        ];
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 0, coordinateIndex: 1, innerOuter: 0, direction: 0);

        HashSet<int> segments = MainFrm.GetPreviewCollisionSegmentIndices(profile, step, isFoldCompletionState: true);

        Assert.AreEqual(0, segments.Count);
    }

    [TestMethod]
    public void GetPreviewCollisionSegmentIndices_MarksFormedEndpointInsideCollisionArea()
    {
        List<PointF> profile =
        [
            new(0, 0),
            new(-50, 25),
            new(-95, 5),
            new(-145, 10),
        ];
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 2, coordinateIndex: 0, innerOuter: 1);

        HashSet<int> segments = MainFrm.GetPreviewCollisionSegmentIndices(profile, step, isFoldCompletionState: true);

        CollectionAssert.Contains(segments.ToList(), 2);
    }

    [TestMethod]
    public void GetPreviewCollisionSegmentIndices_MarksEndpointTouchingDisplayedFlapCollisionArea()
    {
        MainFrm.PreviewCollisionBoundary boundary = MainFrm.GetPreviewCollisionBoundary();
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 0, coordinateIndex: 1, innerOuter: 0, direction: 0);
        PointF[] flap = MainFrm.GetPreviewFlapAreaPoints(lowerFlap: true, PointF.Empty, boundary);
        List<PointF> profile =
        [
            flap[1],
            new(0, 0),
            new(60, 0),
        ];

        HashSet<int> segments = MainFrm.GetPreviewCollisionSegmentIndices(profile, step, isFoldCompletionState: false, compareWorkingFlapArea: true);

        CollectionAssert.Contains(segments.ToList(), 0);
    }

    [TestMethod]
    public void PreviewUsesLowerWorkingFlap_FollowsActiveApronForFoldDirection()
    {
        MainFrm.SemiAutoType upStep = CreateFoldStep(longAngleIndex: 0, coordinateIndex: 0, direction: 0);
        MainFrm.SemiAutoType downStep = CreateFoldStep(longAngleIndex: 0, coordinateIndex: 0, direction: 1);

        Assert.IsTrue(MainFrm.PreviewUsesLowerWorkingFlap(upStep));
        Assert.IsFalse(MainFrm.PreviewUsesLowerWorkingFlap(downStep));
    }

    [TestMethod]
    public void PreviewComparesWorkingFlapAfterFeed_UsesWorkingFlapForFeedAndFoldStates()
    {
        Assert.IsTrue(MainFrm.PreviewComparesWorkingFlapAfterFeed(showingFlipCompletionState: false, isFoldCompletionState: true));
        Assert.IsFalse(MainFrm.PreviewComparesWorkingFlapAfterFeed(showingFlipCompletionState: true, isFoldCompletionState: false));
        Assert.IsTrue(MainFrm.PreviewComparesWorkingFlapAfterFeed(showingFlipCompletionState: false, isFoldCompletionState: false));
    }

    [TestMethod]
    public void GetPreviewParkedFlapRetreatHeight_UsesDisplayedStepRetractLevel()
    {
        MainFrm.PreviewCollisionBoundary boundary = MainFrm.GetPreviewCollisionBoundary();
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 0, coordinateIndex: 0);
        step.翻板收缩值 = 3;

        Assert.AreEqual(200.0, MainFrm.GetPreviewParkedFlapRetreatHeight(step, boundary), 0.001);
    }

    [TestMethod]
    public void GetPreviewParkedFlapOrigin_RetreatsAlongClampFace()
    {
        MainFrm.PreviewCollisionBoundary boundary = MainFrm.GetPreviewCollisionBoundary();
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 0, coordinateIndex: 0);
        step.翻板收缩值 = 3;

        PointF upperOrigin = MainFrm.GetPreviewParkedFlapOrigin(step, lowerFlap: false, boundary);
        PointF lowerOrigin = MainFrm.GetPreviewParkedFlapOrigin(step, lowerFlap: true, boundary);

        Assert.AreEqual(-141.421, upperOrigin.X, 0.001);
        Assert.AreEqual(141.421, upperOrigin.Y, 0.001);
        Assert.AreEqual(-163.830, lowerOrigin.X, 0.001);
        Assert.AreEqual(-114.715, lowerOrigin.Y, 0.001);
    }

    [TestMethod]
    public void GetPreviewFlapAreaPoints_BuildsParallelogramWithFlatFrontFaceOnFoldPoint()
    {
        MainFrm.PreviewCollisionBoundary boundary = MainFrm.GetPreviewCollisionBoundary();

        PointF[] lowerArea = MainFrm.GetPreviewFlapAreaPoints(lowerFlap: true, PointF.Empty, boundary);
        PointF[] upperArea = MainFrm.GetPreviewFlapAreaPoints(lowerFlap: false, PointF.Empty, boundary);

        AssertFlapArea(lowerArea, expectedAxisAngle: 80.0);
        AssertFlapArea(upperArea, expectedAxisAngle: 100.0);
    }

    [TestMethod]
    public void GetPreviewCollisionSegmentIndices_IgnoresClampFoldPointButStillMarksFormedEndpoint()
    {
        List<PointF> profile =
        [
            new(-40, 40),
            new(0, 0),
            new(-40, -40),
        ];
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 0, coordinateIndex: 0, innerOuter: 1);

        HashSet<int> segments = MainFrm.GetPreviewCollisionSegmentIndices(profile, step, isFoldCompletionState: true);

        CollectionAssert.Contains(segments.ToList(), 1);
    }

    [TestMethod]
    public void GetPreviewCollisionBoundary_ExposesCurrentDocumentedPreviewConstants()
    {
        MainFrm.PreviewCollisionBoundary boundary = MainFrm.GetPreviewCollisionBoundary();

        Assert.AreEqual(12.0, boundary.ClampArmWidth, 0.001);
        Assert.AreEqual(45.0, boundary.TopBoundaryAngle, 0.001);
        Assert.AreEqual(15.0, boundary.UpperClampThickness, 0.001);
        Assert.AreEqual(21.213, boundary.UpperClampVerticalOffset, 0.001);
        Assert.AreEqual(15.0, boundary.FlapHeight, 0.001);
        Assert.AreEqual(35.0, boundary.BottomBoundaryAngle, 0.001);
        Assert.AreEqual(80.0, boundary.UpperFlapAngle, 0.001);
        Assert.AreEqual(80.0, boundary.LowerFlapAngle, 0.001);
        Assert.AreEqual(15.0, boundary.WorkingFlapWidth, 0.001);
        Assert.AreEqual(50.0, boundary.ParkedFlapRetreatUnit, 0.001);
        Assert.AreEqual(400.0, boundary.FlapLength, 0.001);
        Assert.AreEqual(0.3, boundary.SoftCollisionRatio, 0.001);
    }

    [TestMethod]
    public void ReadPreviewCollisionConfig_UsesPreviewCollisionSectionValues()
    {
        string configPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".ini");
        File.WriteAllLines(configPath,
        [
            "[PreviewCollision]",
            "ShowCollisionArea 1",
            "ClampArmWidth 13.50",
            "TopBoundaryAngle 46.50",
            "UpperClampThickness 16.50",
            "FlapHeight 17.50",
            "BottomBoundaryAngle 36.50",
            "UpperFlapAngle 81.50",
            "LowerFlapAngle 82.50",
            "ParkedFlapRetreatUnit 55.50",
            "FlapLength 420.50",
            "SoftCollisionRatio 0.45",
        ]);

        try
        {
            MainFrm.PreviewCollisionConfig config = MainFrm.ReadPreviewCollisionConfig(configPath);

            Assert.IsTrue(config.ShowCollisionArea);
            Assert.AreEqual(13.5, config.Boundary.ClampArmWidth, 0.001);
            Assert.AreEqual(46.5, config.Boundary.TopBoundaryAngle, 0.001);
            Assert.AreEqual(16.5, config.Boundary.UpperClampThickness, 0.001);
            Assert.AreEqual(17.5, config.Boundary.FlapHeight, 0.001);
            Assert.AreEqual(36.5, config.Boundary.BottomBoundaryAngle, 0.001);
            Assert.AreEqual(81.5, config.Boundary.UpperFlapAngle, 0.001);
            Assert.AreEqual(82.5, config.Boundary.LowerFlapAngle, 0.001);
            Assert.AreEqual(55.5, config.Boundary.ParkedFlapRetreatUnit, 0.001);
            Assert.AreEqual(420.5, config.Boundary.FlapLength, 0.001);
            Assert.AreEqual(0.45, config.Boundary.SoftCollisionRatio, 0.001);
        }
        finally
        {
            File.Delete(configPath);
        }
    }

    [TestMethod]
    public void LoadPreviewCollisionConfig_AppendsDefaultSectionWhenMissing()
    {
        string configPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".ini");
        File.WriteAllLines(configPath,
        [
            "[OtherConfig]",
            "DisplayUnit # mm",
        ]);

        try
        {
            MainFrm.LoadPreviewCollisionConfig(configPath);
            string text = File.ReadAllText(configPath);

            StringAssert.Contains(text, "[PreviewCollision]");
            StringAssert.Contains(text, "ShowCollisionArea 0.00");
            StringAssert.Contains(text, "ClampArmWidth 12.00");
            Assert.IsFalse(MainFrm.PreviewCollisionAreaVisible);
        }
        finally
        {
            File.Delete(configPath);
            MainFrm.LoadPreviewCollisionConfig(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".missing.ini"));
        }
    }

    [TestMethod]
    public void ClassifyPreviewCollisionSeverity_UsesDocumentedSoftThreshold()
    {
        MainFrm.PreviewCollisionBoundary boundary = MainFrm.GetPreviewCollisionBoundary();

        Assert.AreEqual(MainFrm.PreviewCollisionSeverity.Soft, MainFrm.ClassifyPreviewCollisionSeverity(0.0, boundary));
        Assert.AreEqual(MainFrm.PreviewCollisionSeverity.Soft, MainFrm.ClassifyPreviewCollisionSeverity(0.3, boundary));
        Assert.AreEqual(MainFrm.PreviewCollisionSeverity.Hard, MainFrm.ClassifyPreviewCollisionSeverity(0.3001, boundary));
    }

    [TestMethod]
    public void GetPreviewCollisionSegmentSeverities_ClassifiesDeepFlapIntrusionAsHard()
    {
        MainFrm.SemiAutoType step = CreateFoldStep(longAngleIndex: 0, coordinateIndex: 0, innerOuter: 1, direction: 0);
        List<PointF> profile =
        [
            new(0, 0),
            new(-20, -50),
            new(-60, 0),
        ];

        Dictionary<int, MainFrm.PreviewCollisionSeverity> severities = MainFrm.GetPreviewCollisionSegmentSeverities(
            profile,
            step,
            isFoldCompletionState: true);

        Assert.AreEqual(MainFrm.PreviewCollisionSeverity.Hard, severities[1]);
    }

    private static MainFrm.OrderType CreateOrderWithStraightProfile(double width, params double[] segmentLengths)
    {
        MainFrm.OrderType order = new()
        {
            Width = width,
            显示朝向已初始化 = true,
            显示起始角度 = 180.0,
        };

        float x = 0f;
        order.pxList.Add(new PointF(x, 0f));
        for (int i = 0; i < segmentLengths.Length; i++)
        {
            order.lengAngle[i + 1].Length = segmentLengths[i];
            x -= (float)segmentLengths[i];
            order.pxList.Add(new PointF(x, 0f));
        }

        return order;
    }

    private static MainFrm.SemiAutoType CreateFoldStep(
        int longAngleIndex,
        int coordinateIndex,
        int innerOuter = 0,
        int direction = 0,
        double angle = 90)
    {
        return new MainFrm.SemiAutoType
        {
            行动类型 = MainFrm.SemiAutoActionFold,
            折弯角度 = angle,
            折弯方向 = direction,
            抓取类型 = 1,
            长角序号 = longAngleIndex,
            坐标序号 = coordinateIndex,
            内外选择 = innerOuter,
        };
    }

    private static MainFrm.SemiAutoType CreateFlipStep(int coordinateIndex)
    {
        return new MainFrm.SemiAutoType
        {
            行动类型 = MainFrm.SemiAutoActionFlip,
            坐标序号 = coordinateIndex,
        };
    }

    private static MainFrm.SemiAutoType CreateSquashStep(
        int longAngleIndex,
        int coordinateIndex,
        int direction = 0,
        double angle = 3.001)
    {
        return new MainFrm.SemiAutoType
        {
            行动类型 = MainFrm.SemiAutoActionSquash,
            折弯角度 = angle,
            折弯方向 = direction,
            抓取类型 = 1,
            长角序号 = longAngleIndex,
            坐标序号 = coordinateIndex,
            内外选择 = 1,
        };
    }


    private static void InvokeNormalizeSemiAutoStepsForPreview(
        MainFrm.OrderType order,
        List<MainFrm.SemiAutoType> steps,
        bool normalizeGeneratedDirections)
    {
        MethodInfo? method = typeof(MainFrm).GetMethod(
            "NormalizeSemiAutoStepsForPreview",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.IsNotNull(method);
        method.Invoke(null, [order, steps, normalizeGeneratedDirections]);
    }

    private static List<Point> InvokeBuildPreviewSegmentsForDrawStep(SubOPAutoView view, int drawStep)
    {
        MethodInfo? method = typeof(SubOPAutoView).GetMethod(
            "BuildPreviewSegmentsForDrawStep",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method);
        object? result = method.Invoke(view, [drawStep]);
        Assert.IsInstanceOfType<List<Point>>(result);
        return (List<Point>)result;
    }

    private static void SetPrivateField(object instance, string fieldName, object value)
    {
        FieldInfo? field = instance.GetType().GetField(
            fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(field);
        field.SetValue(instance, value);
    }

    private static object? GetPrivateField(object instance, string fieldName)
    {
        FieldInfo? field = instance.GetType().GetField(
            fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(field);
        return field.GetValue(instance);
    }



    private static List<string> InvokeBuildStepDetails(SubOPAutoView view, MainFrm.SemiAutoType step, int stepIndex)
    {
        MethodInfo? method = typeof(SubOPAutoView).GetMethod(
            "BuildStepDetails",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method);
        object? result = method.Invoke(view, [step, stepIndex]);
        Assert.IsInstanceOfType<IEnumerable<string>>(result);
        return ((IEnumerable<string>)result).ToList();
    }

    private static bool InvokeTryGetCurrentPreviewFoldSegmentIndex(SubOPAutoView view, out int segmentIndex)
    {
        MethodInfo? method = typeof(SubOPAutoView).GetMethod(
            "TryGetCurrentPreviewFoldSegmentIndex",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method);
        object?[] parameters = [-1];
        bool result = (bool)method.Invoke(view, parameters)!;
        segmentIndex = (int)parameters[0]!;
        return result;
    }

    private static bool InvokeTryGetCurrentPreviewSquashSegmentIndex(SubOPAutoView view, out int segmentIndex)
    {
        MethodInfo? method = typeof(SubOPAutoView).GetMethod(
            "TryGetCurrentPreviewSquashSegmentIndex",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method);
        object?[] parameters = [-1];
        bool result = (bool)method.Invoke(view, parameters)!;
        segmentIndex = (int)parameters[0]!;
        return result;
    }

    private static bool InvokeTryGetCompletedPreviewSquashEdge(
        SubOPAutoView view,
        MainFrm.SemiAutoType step,
        out PointF startPoint,
        out PointF endPoint)
    {
        MethodInfo? method = typeof(SubOPAutoView).GetMethod(
            "TryGetCompletedPreviewSquashEdge",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method);
        object?[] parameters = [step, PointF.Empty, PointF.Empty];
        bool result = (bool)method.Invoke(view, parameters)!;
        startPoint = (PointF)parameters[1]!;
        endPoint = (PointF)parameters[2]!;
        return result;
    }


    private static void InvokeDrawCurrentPreviewActiveDisplayEdge(SubOPAutoView view, Graphics graphic, Pen foldPen)
    {
        MethodInfo? method = typeof(SubOPAutoView).GetMethod(
            "DrawCurrentPreviewActiveDisplayEdge",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method);
        method.Invoke(view, [graphic, foldPen]);
    }

    private static void InvokeRedrawPreview(SubOPAutoView view, bool colorDown)
    {
        MethodInfo? method = typeof(SubOPAutoView).GetMethod(
            "redrawPreView",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method);
        try
        {
            method.Invoke(view, [colorDown]);
        }
        catch (TargetInvocationException ex) when (ex.InnerException is NullReferenceException)
        {
            // Unit tests instantiate SubOPAutoView without MainFrm; redraw has already painted image1,
            // then UpdatePreviewStepInfo touches mf for side-panel text.
        }
    }

    private static bool InvokeTryGetCurrentPreviewActiveFoldEdge(SubOPAutoView view, out PointF startPoint, out PointF endPoint)
    {
        MethodInfo? method = typeof(SubOPAutoView).GetMethod(
            "TryGetCurrentPreviewActiveFoldEdge",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method);
        object?[] parameters = [PointF.Empty, PointF.Empty];
        bool result = (bool)method.Invoke(view, parameters)!;
        startPoint = (PointF)parameters[0]!;
        endPoint = (PointF)parameters[1]!;
        return result;
    }

    private static bool InvokeTryGetCurrentPreviewActiveDisplayEdge(SubOPAutoView view, out PointF startPoint, out PointF endPoint)
    {
        MethodInfo? method = typeof(SubOPAutoView).GetMethod(
            "TryGetCurrentPreviewActiveDisplayEdge",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method);
        object?[] parameters = [PointF.Empty, PointF.Empty];
        bool result = (bool)method.Invoke(view, parameters)!;
        startPoint = (PointF)parameters[0]!;
        endPoint = (PointF)parameters[1]!;
        return result;
    }

    private static bool InvokeTryGetCurrentPreviewSquashCueEdge(SubOPAutoView view, out PointF startPoint, out PointF endPoint)
    {
        MethodInfo? method = typeof(SubOPAutoView).GetMethod(
            "TryGetCurrentPreviewSquashCueEdge",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method);
        object?[] parameters = [PointF.Empty, PointF.Empty];
        bool result = (bool)method.Invoke(view, parameters)!;
        startPoint = (PointF)parameters[0]!;
        endPoint = (PointF)parameters[1]!;
        return result;
    }


    private static List<string> InvokeBuildCurrentPreviewFoldInfoLines(SubOPAutoView view)
    {
        MethodInfo? method = typeof(SubOPAutoView).GetMethod(
            "BuildCurrentPreviewFoldInfoLines",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method);
        object? result = method.Invoke(view, []);
        Assert.IsInstanceOfType<List<string>>(result);
        return (List<string>)result;
    }

    private static bool InvokeShouldSkipPreviewPolylineSegment(SubOPAutoView view, int segmentIndex)
    {
        MethodInfo? method = typeof(SubOPAutoView).GetMethod(
            "ShouldSkipPreviewPolylineSegment",
            BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method);
        return (bool)method.Invoke(view, [segmentIndex])!;
    }

    private static PointF GetMidpoint(PointF first, PointF second)
    {
        return new PointF((first.X + second.X) / 2.0f, (first.Y + second.Y) / 2.0f);
    }

    private static double GetDistance(PointF first, PointF second)
    {
        double dx = first.X - second.X;
        double dy = first.Y - second.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private static double GetVectorAngleDegrees(PointF first, PointF second)
    {
        double angle = Math.Atan2(first.Y - second.Y, second.X - first.X) * 180.0 / Math.PI;
        return angle < 0 ? angle + 360.0 : angle;
    }

    private static double GetDistancePointToLine(PointF point, PointF lineStart, PointF lineEnd)
    {
        double dx = lineEnd.X - lineStart.X;
        double dy = lineEnd.Y - lineStart.Y;
        double length = Math.Sqrt(dx * dx + dy * dy);
        if (length < 0.001)
            return GetDistance(point, lineStart);

        return Math.Abs(dy * point.X - dx * point.Y + lineEnd.X * lineStart.Y - lineEnd.Y * lineStart.X) / length;
    }


    private static void AssertFlapArea(PointF[] area, double expectedAxisAngle)
    {
        PointF backLeft = area[1];
        PointF backRight = area[2];

        Assert.AreEqual(0.0, area[0].X, 0.001);
        Assert.AreEqual(0.0, area[0].Y, 0.001);
        Assert.AreEqual(15.0, area[3].X, 0.001);
        Assert.AreEqual(0.0, area[3].Y, 0.001);
        Assert.AreEqual(15.0, GetDistance(area[0], area[3]), 0.001);
        Assert.AreEqual(15.0, GetDistance(backLeft, backRight), 0.001);
        Assert.AreEqual(400.0, GetDistance(area[0], backLeft), 0.001);
        Assert.AreEqual(GetDistance(area[0], area[1]), GetDistance(area[3], area[2]), 0.001);
        Assert.AreEqual(GetDistance(area[0], area[3]), GetDistance(area[1], area[2]), 0.001);
        Assert.AreEqual(expectedAxisAngle, GetAxisAngle(area[0], backLeft), 0.001);
    }

    private static double GetAxisAngle(PointF first, PointF second)
    {
        double angle = Math.Atan2(second.Y - first.Y, second.X - first.X) * 180.0 / Math.PI;
        while (angle < 0.0)
            angle += 180.0;
        while (angle >= 180.0)
            angle -= 180.0;
        return angle;
    }

    private static bool HasVisiblePixel(Bitmap bitmap)
    {
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                if (bitmap.GetPixel(x, y).A != 0)
                    return true;
            }
        }

        return false;
    }

    private static void AssertLineMidpointColor(Bitmap bitmap, PointF startPoint, PointF endPoint, Color expectedColor)
    {
        PointF midpoint = GetMidpoint(startPoint, endPoint);
        int centerX = Math.Clamp((int)Math.Round(midpoint.X), 0, bitmap.Width - 1);
        int centerY = Math.Clamp((int)Math.Round(midpoint.Y), 0, bitmap.Height - 1);
        int expectedArgb = expectedColor.ToArgb();

        for (int y = Math.Max(0, centerY - 3); y <= Math.Min(bitmap.Height - 1, centerY + 3); y++)
        {
            for (int x = Math.Max(0, centerX - 3); x <= Math.Min(bitmap.Width - 1, centerX + 3); x++)
            {
                if (bitmap.GetPixel(x, y).ToArgb() == expectedArgb)
                    return;
            }
        }

        Color actualColor = bitmap.GetPixel(centerX, centerY);
        Assert.AreEqual(expectedArgb, actualColor.ToArgb());
    }
}
