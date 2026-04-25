namespace JSZW1000A
{
    internal static class LocalizationText
    {
        public static string EnabledDisabled(bool enabled)
        {
            return enabled ? Strings.Get("Common.Enabled") : Strings.Get("Common.Disabled");
        }

        public static string OnOff(bool enabled)
        {
            return enabled ? Strings.Get("Common.On") : Strings.Get("Common.Off");
        }

        public static string YesNo(bool accepted)
        {
            return accepted ? Strings.Get("Common.Yes") : Strings.Get("Common.No");
        }

        public static string AutoManual(bool autoMode)
        {
            return autoMode ? Strings.Get("Common.Auto") : Strings.Get("Common.Manual");
        }

        public static string PlcConnection(bool connected)
        {
            return connected
                ? Strings.Get("Common.PlcConnected")
                : Strings.Get("Common.PlcDisconnected");
        }

        public static string OrderDirection(bool reverseOrder)
        {
            return reverseOrder ? Strings.Get("AutoView.Order.Reverse") : Strings.Get("AutoView.Order.Forward");
        }

        public static string ColorSide(bool colorDown)
        {
            return colorDown ? Strings.Get("AutoView.ColorSide.Below") : Strings.Get("AutoView.ColorSide.Top");
        }

        public static string ManualApplySteps(bool dirty)
        {
            return dirty ? Strings.Get("Manual.ApplyStepsDirty") : Strings.Get("Manual.ApplySteps");
        }

        public static string GripType(int index)
        {
            return index switch
            {
                0 => Strings.Get("Manual.Grip.Push"),
                1 => Strings.Get("Manual.Grip.Grip"),
                _ => Strings.Get("Manual.Grip.OverGrip")
            };
        }

        public static string ReleaseHeight(int index)
        {
            return index switch
            {
                0 => Strings.Get("Manual.Release.Low"),
                1 => Strings.Get("Manual.Release.Medium"),
                2 => Strings.Get("Manual.Release.High"),
                _ => Strings.Get("Manual.Release.Maximum")
            };
        }

        public static string ReleaseHeightShort(int index)
        {
            return index switch
            {
                0 => Strings.Get("AutoSet.Release.Short.Low"),
                1 => Strings.Get("AutoSet.Release.Short.Medium"),
                2 => Strings.Get("AutoSet.Release.Short.High"),
                _ => Strings.Get("AutoSet.Release.Short.Maximum")
            };
        }

        public static string FoldDirectionShort(int index)
        {
            return index == 0
                ? Strings.Get("AutoSet.FoldDirection.Short.Up")
                : Strings.Get("AutoSet.FoldDirection.Short.Down");
        }

        public static string ManualStepAction(int actionType)
        {
            return actionType switch
            {
                MainFrm.SemiAutoActionSquash => Strings.Get("Manual.Step.Squash"),
                MainFrm.SemiAutoActionOpenSquash => Strings.Get("Manual.Step.OpenSquash"),
                MainFrm.SemiAutoActionSlit => Strings.Get("Manual.Step.Slit"),
                MainFrm.SemiAutoActionFlip => Strings.Get("Manual.Step.Flip"),
                _ => Strings.Get("Manual.Step.Fold")
            };
        }

        public static string ErrorMessage(int index)
        {
            return index switch
            {
                0 => Strings.Get("MainFrm.Err.PullRope"),
                1 => Strings.Get("MainFrm.Err.OilPumpBlockage"),
                2 => Strings.Get("MainFrm.Err.LubricationBlockage"),
                3 => Strings.Get("MainFrm.Err.OperationCabinetEstop"),
                4 => Strings.Get("MainFrm.Err.ControlCabinetEstop"),
                _ => " code1"
            };
        }

        public static string WarningMessage(int index)
        {
            return index switch
            {
                0 => Strings.Get("MainFrm.Warn.TopFoldNotHome"),
                1 => Strings.Get("MainFrm.Warn.TopSlideNotWork"),
                2 => Strings.Get("MainFrm.Warn.BottomFoldNotHome"),
                3 => Strings.Get("MainFrm.Warn.BottomSlideNotHome"),
                _ => " code1"
            };
        }

        public static string TipMessage(int group, int index)
        {
            if (group == 1 && index == 0)
                return Strings.Get("MainFrm.Tip.SlitterSwitchMismatch");

            if (group != 0)
                return "code1";

            return index switch
            {
                0 => Strings.Get("MainFrm.Tip.Empty"),
                1 => Strings.Get("MainFrm.Tip.UnlockBackGauge"),
                2 => Strings.Get("MainFrm.Tip.LockBackGauge"),
                3 => Strings.Get("MainFrm.Tip.LockClamp"),
                4 => Strings.Get("MainFrm.Tip.PedalFoldStart"),
                5 => Strings.Get("MainFrm.Tip.PedalExtendTable"),
                6 => Strings.Get("MainFrm.Tip.PedalRetractTable"),
                7 => Strings.Get("MainFrm.Tip.OperateSlitter"),
                _ => " code1"
            };
        }
    }
}
