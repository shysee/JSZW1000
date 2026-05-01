using System.Reflection;

namespace JSZW1000A
{
    public partial class MainFrm
    {
        private const string OrderSequenceGeneratedFieldName = nameof(OrderType.生产序列已生成);
        private const string OrderManualSemiAutoFieldName = nameof(OrderType.半自动步骤已手动编辑);
        private const string SemiAutoActionFieldName = nameof(SemiAutoType.行动类型);
        private const string SemiAutoLongAngleFieldName = nameof(SemiAutoType.长角序号);
        private const string SemiAutoCoordinateFieldName = nameof(SemiAutoType.坐标序号);

        public bool HasManualSemiAutoEdits()
        {
            return GetOrderBoolField(OrderManualSemiAutoFieldName) && CurtOrder.lstSemiAuto.Count > 0;
        }

        public bool HasValidSemiAutoGeometryData()
        {
            if (CurtOrder.lstSemiAuto.Count <= 0 || CurtOrder.pxList.Count <= 1)
                return false;

            bool hasNormalFold = false;
            foreach (var step in CurtOrder.lstSemiAuto)
            {
                if (GetSemiAutoIntField(step, SemiAutoActionFieldName) != SemiAutoActionFold || IsLegacySemiAutoPlaceholder(step))
                    continue;

                hasNormalFold = true;
                int longAngleIndex = GetSemiAutoIntField(step, SemiAutoLongAngleFieldName);
                if (longAngleIndex == 0 || longAngleIndex == 99)
                    return false;

                int coordinateIndex = GetSemiAutoIntField(step, SemiAutoCoordinateFieldName);
                if (coordinateIndex <= 0 || coordinateIndex >= CurtOrder.pxList.Count - 1)
                    return false;
            }

            return hasNormalFold;
        }

        public void MarkSemiAutoStepsManuallyEdited()
        {
            if (CurtOrder.lstSemiAuto.Count <= 0)
                return;

            RegisterManualSemiAutoPreference();
            SetOrderBoolField(OrderSequenceGeneratedFieldName, true);
            SetOrderBoolField(OrderManualSemiAutoFieldName, true);
            MarkCurrentPlanAsCustomManual();
        }

        public void ResetSemiAutoManualEditFlag()
        {
            SetOrderBoolField(OrderManualSemiAutoFieldName, false);
        }

        public void RestoreSemiAutoPlanEditingState(IReadOnlyList<SemiAutoType> steps, bool manualEdited, string origin)
        {
            CurtOrder.lstSemiAuto = new List<SemiAutoType>(steps);
            CurtOrder.SemiAutoPlanOrigin = origin;
            SetOrderBoolField(OrderSequenceGeneratedFieldName, CurtOrder.lstSemiAuto.Count > 0);
            SetOrderBoolField(OrderManualSemiAutoFieldName, manualEdited);
        }

        public void MarkSemiAutoSequenceStale()
        {
            SetOrderBoolField(OrderSequenceGeneratedFieldName, false);
            SetOrderBoolField(OrderManualSemiAutoFieldName, false);
        }

        public int GetDefaultSemiAutoRetractValue()
        {
            int configuredValue = Convert.ToInt32(ConfigData[L6_MachineSetup + 9]);
            if (configuredValue < 2 || configuredValue > 4)
            {
                configuredValue = 4;
            }

            return configuredValue - 1;
        }

        private static bool GetOrderBoolField(string fieldName)
        {
            FieldInfo? field = typeof(OrderType).GetField(fieldName);
            if (field == null)
                return false;

            object boxed = CurtOrder;
            return (bool)(field.GetValue(boxed) ?? false);
        }

        private static void SetOrderBoolField(string fieldName, bool value)
        {
            FieldInfo? field = typeof(OrderType).GetField(fieldName);
            if (field == null)
                return;

            object boxed = CurtOrder;
            field.SetValue(boxed, value);
            CurtOrder = (OrderType)boxed;
        }

        private static int GetSemiAutoIntField(SemiAutoType step, string fieldName)
        {
            FieldInfo? field = typeof(SemiAutoType).GetField(fieldName);
            if (field == null)
                return 0;

            object boxed = step;
            return (int)(field.GetValue(boxed) ?? 0);
        }
    }
}
