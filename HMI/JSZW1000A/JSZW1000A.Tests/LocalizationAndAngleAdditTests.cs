using System.Globalization;

namespace JSZW1000A.Tests;

[TestClass]
public class LocalizationAndAngleAdditTests
{
    [TestInitialize]
    public void TestInitialize()
    {
        LocalizationManager.UseSessionLanguage(AppLanguage.ZhCn);
        MainFrm.ClearAngleAdditCache();
        Array.Clear(MainFrm.Hmi_rArray);
    }

    [TestMethod]
    public void GetLanguageDisplayName_ReturnsConfiguredFrenchAndRussianNames()
    {
        LocalizationManager.UseSessionLanguage(AppLanguage.ZhCn);

        Assert.AreEqual("Français", LocalizationManager.GetLanguageDisplayName(AppLanguage.FrFr));
        Assert.AreEqual("Русский", LocalizationManager.GetLanguageDisplayName(AppLanguage.RuRu));
    }

    [TestMethod]
    public void NormalizeAngleAdditType_AllowsNullAndWrapsBrackets()
    {
        Assert.AreEqual(string.Empty, MainFrm.NormalizeAngleAdditType(null));
        Assert.AreEqual("[Test]", MainFrm.NormalizeAngleAdditType("  Test  "));
        Assert.AreEqual("[Keep]", MainFrm.NormalizeAngleAdditType("[Keep]"));
    }

    [TestMethod]
    public void HasAngleAdditName_IgnoresBracketsAndCase()
    {
        MainFrm.angleAddit[0] = MainFrm.CreateEmptyAngleAddit("[MildSteel]");
        MainFrm.angleAddit[1] = MainFrm.CreateEmptyAngleAddit("[Aluminum]");

        Assert.IsTrue(MainFrm.HasAngleAdditName("mildsteel", -1));
        Assert.IsFalse(MainFrm.HasAngleAdditName("mildsteel", 0));
        Assert.IsFalse(MainFrm.HasAngleAdditName("stainless", -1));
    }

    [TestMethod]
    public void TryParseAngleAdditMeasuredOffset_TreatsNullOrWhitespaceAsZero()
    {
        Assert.IsTrue(MainFrm.TryParseAngleAdditMeasuredOffset(null, out float nullValue));
        Assert.AreEqual(0F, nullValue);

        Assert.IsTrue(MainFrm.TryParseAngleAdditMeasuredOffset("   ", out float blankValue));
        Assert.AreEqual(0F, blankValue);
    }

    [TestMethod]
    public void TryApplyAngleAdditToHmiArrays_WritesBottomAndTopOffsets()
    {
        MainFrm.angleAddit[0] = MainFrm.CreateEmptyAngleAddit("[Steel]");
        MainFrm.angleAddit[0].MachingGauging = "2.5";
        MainFrm.angleAddit[0].AngleRange[0] = 10F;
        MainFrm.angleAddit[0].AngleRange[1] = -5F;
        MainFrm.angleAddit[0].AngleRange[MainFrm.AngleAdditVisibleRows] = 8F;
        MainFrm.angleAddit[0].AngleRange[MainFrm.AngleAdditVisibleRows + 1] = -4F;

        bool success = MainFrm.TryApplyAngleAdditToHmiArrays(0, out string errorMessage);

        Assert.IsTrue(success, errorMessage);
        Assert.AreEqual(string.Empty, errorMessage);
        Assert.AreEqual(12.5F, MainFrm.Hmi_rArray[150]);
        Assert.AreEqual(-2.5F, MainFrm.Hmi_rArray[151]);
        Assert.AreEqual(10.5F, MainFrm.Hmi_rArray[170]);
        Assert.AreEqual(-1.5F, MainFrm.Hmi_rArray[171]);
    }

    [TestMethod]
    public void TryApplyAngleAdditToHmiArrays_RejectsEffectiveOffsetsOutsideRange()
    {
        LocalizationManager.UseSessionLanguage(AppLanguage.EnUs);
        MainFrm.angleAddit[0] = MainFrm.CreateEmptyAngleAddit("[Steel]");
        MainFrm.angleAddit[0].MachingGauging = "20";
        MainFrm.angleAddit[0].AngleRange[0] = 170F;

        bool success = MainFrm.TryApplyAngleAdditToHmiArrays(0, out string errorMessage);

        Assert.IsFalse(success);
        StringAssert.Contains(errorMessage, "Bottom apron");
        StringAssert.Contains(errorMessage, "190.00");
    }

    [TestMethod]
    public void GetAngleAdditMeasuredOffset_UsesInvariantParsing()
    {
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
            MainFrm.angleAddit[0] = MainFrm.CreateEmptyAngleAddit("[Steel]");
            MainFrm.angleAddit[0].MachingGauging = "12.75";

            float value = MainFrm.GetAngleAdditMeasuredOffset(0);

            Assert.AreEqual(12.75F, value, 0.001F);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
