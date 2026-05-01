using System.Drawing;

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
}
