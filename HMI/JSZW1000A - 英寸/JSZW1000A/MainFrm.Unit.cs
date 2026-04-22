using System.Globalization;

namespace JSZW1000A
{
    public partial class MainFrm
    {
        public const double MmPerInch = 25.4;

        public static double MmToDisplayLength(double mm)
        {
            return DisplayUnitManager.CurrentDisplayUnit == DisplayLengthUnit.Millimeter
                ? mm
                : mm / MmPerInch;
        }

        public static double DisplayLengthToMm(double displayLength)
        {
            return DisplayUnitManager.CurrentDisplayUnit == DisplayLengthUnit.Millimeter
                ? displayLength
                : displayLength * MmPerInch;
        }

        public static string FormatDisplayLength(double mm, int decimals = 3)
        {
            string format = decimals <= 0 ? "0" : "0." + new string('#', decimals);
            return MmToDisplayLength(mm).ToString(format, CultureInfo.InvariantCulture);
        }

        public static string FormatDisplayLengthWithUnit(double mm, int decimals = 3)
        {
            return FormatDisplayLength(mm, decimals) + " " + GetLengthUnitLabel();
        }

        public static bool TryParseDisplayLength(string? text, out double mm)
        {
            mm = 0;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            string valueText = text.Trim();
            bool explicitMm = EndsWithUnit(valueText, "mm") || EndsWithUnit(valueText, "毫米");
            bool explicitInch = EndsWithUnit(valueText, "in") || EndsWithUnit(valueText, "inch") || EndsWithUnit(valueText, "英寸") || valueText.EndsWith("\"", StringComparison.Ordinal);

            valueText = valueText
                .Replace("毫米", "", StringComparison.OrdinalIgnoreCase)
                .Replace("英寸", "", StringComparison.OrdinalIgnoreCase)
                .Replace("inch", "", StringComparison.OrdinalIgnoreCase)
                .Replace("in", "", StringComparison.OrdinalIgnoreCase)
                .Replace("mm", "", StringComparison.OrdinalIgnoreCase)
                .Replace("\"", "", StringComparison.Ordinal)
                .Trim();

            if (!TryParseDouble(valueText, out double displayValue))
                return false;

            if (explicitMm && !explicitInch)
            {
                mm = displayValue;
            }
            else if (explicitInch && !explicitMm)
            {
                mm = displayValue * MmPerInch;
            }
            else
            {
                mm = DisplayLengthToMm(displayValue);
            }
            return true;
        }

        internal static bool TryParseLengthByUnit(string? text, DisplayLengthUnit unit, out double mm)
        {
            mm = 0;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            if (!TryParseDouble(text.Trim(), out double value))
                return false;

            mm = unit == DisplayLengthUnit.Millimeter ? value : value * MmPerInch;
            return true;
        }

        internal static string FormatLengthByUnit(double mm, DisplayLengthUnit unit, int decimals = 3)
        {
            string format = decimals <= 0 ? "0" : "0." + new string('#', decimals);
            double displayValue = unit == DisplayLengthUnit.Millimeter ? mm : (mm / MmPerInch);
            return displayValue.ToString(format, CultureInfo.InvariantCulture);
        }

        public static double ParseDisplayLengthOrZero(string? text)
        {
            return TryParseDisplayLength(text, out double mm) ? mm : 0;
        }

        public static float ParseDisplayLengthFloatOrZero(string? text)
        {
            return (float)ParseDisplayLengthOrZero(text);
        }

        private static bool EndsWithUnit(string value, string unit)
        {
            return value.EndsWith(unit, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetLengthUnitLabel()
        {
            return DisplayUnitManager.GetUnitLabel(DisplayUnitManager.CurrentDisplayUnit);
        }

        private static bool TryParseDouble(string text, out double value)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                || double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value);
        }
    }
}
