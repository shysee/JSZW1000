using System.Globalization;

namespace JSZW1000A
{
    internal static class DisplayUnitManager
    {
        private static readonly string DefaultConfigPath = Path.Combine(Application.StartupPath, "Config.ini");
        private const string ConfigKey = "DisplayUnit";

        public static DisplayLengthUnit CurrentDisplayUnit { get; private set; } = DisplayLengthUnit.Millimeter;

        public static void InitializeFromConfig(string? configPath = null)
        {
            UseSessionDisplayUnit(ReadConfiguredDisplayUnit(configPath));
        }

        public static void UseSessionDisplayUnit(DisplayLengthUnit unit)
        {
            CurrentDisplayUnit = unit;
            MainFrm.ConfigStr[2] = ToConfigToken(unit);
        }

        public static DisplayLengthUnit ReadConfiguredDisplayUnit(string? configPath = null)
        {
            string path = configPath ?? DefaultConfigPath;
            if (!File.Exists(path))
                return DisplayLengthUnit.Millimeter;

            foreach (string rawLine in File.ReadLines(path))
            {
                string line = rawLine.Trim();
                if (!line.StartsWith(ConfigKey, StringComparison.OrdinalIgnoreCase))
                    continue;

                string token = ExtractConfigValue(line);
                return ParseConfigToken(token);
            }

            return DisplayLengthUnit.Millimeter;
        }

        public static bool SaveDisplayUnit(DisplayLengthUnit unit, string? configPath = null)
        {
            string path = configPath ?? DefaultConfigPath;
            if (!File.Exists(path))
                return false;

            List<string> lines = File.ReadAllLines(path).ToList();
            string replacement = $"{ConfigKey} # {ToConfigToken(unit)}";

            for (int i = 0; i < lines.Count; i++)
            {
                string trimmed = lines[i].Trim();
                if (trimmed.StartsWith(ConfigKey, StringComparison.OrdinalIgnoreCase))
                {
                    lines[i] = replacement;
                    MainFrm.ConfigStr[2] = ToConfigToken(unit);
                    File.WriteAllLines(path, lines);
                    return true;
                }
            }

            int otherConfigIndex = lines.FindIndex(line => line.Trim().Equals("[OtherConfig]", StringComparison.OrdinalIgnoreCase));
            if (otherConfigIndex < 0)
                return false;

            int insertIndex = lines.FindIndex(otherConfigIndex + 1, line => line.TrimStart().StartsWith("-----", StringComparison.OrdinalIgnoreCase));
            if (insertIndex < 0)
            {
                insertIndex = lines.Count;
                while (insertIndex > otherConfigIndex + 1 && string.IsNullOrWhiteSpace(lines[insertIndex - 1]))
                    insertIndex--;
            }

            lines.Insert(insertIndex, replacement);
            MainFrm.ConfigStr[2] = ToConfigToken(unit);
            File.WriteAllLines(path, lines);
            return true;
        }

        public static string GetDisplayName(DisplayLengthUnit unit)
        {
            return unit switch
            {
                DisplayLengthUnit.Millimeter => Strings.Get("DisplayUnit.Display.Millimeter", "mm"),
                _ => Strings.Get("DisplayUnit.Display.Inch", "in")
            };
        }

        public static string GetUnitLabel(DisplayLengthUnit unit)
        {
            return unit == DisplayLengthUnit.Millimeter ? "mm" : "in";
        }

        public static string ToConfigToken(DisplayLengthUnit unit)
        {
            return unit == DisplayLengthUnit.Millimeter ? "mm" : "in";
        }

        private static string ExtractConfigValue(string line)
        {
            if (line.Contains('#'))
            {
                string[] parts = line.Split('#', 2);
                return parts[1].Trim();
            }

            string[] tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return tokens.Length > 1 ? tokens[^1].Trim() : string.Empty;
        }

        private static DisplayLengthUnit ParseConfigToken(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return DisplayLengthUnit.Millimeter;

            token = token.Trim();
            if (token.Equals("mm", StringComparison.OrdinalIgnoreCase)
                || token.Equals("millimeter", StringComparison.OrdinalIgnoreCase)
                || token.Equals("millimetre", StringComparison.OrdinalIgnoreCase))
            {
                return DisplayLengthUnit.Millimeter;
            }

            if (token.Equals("in", StringComparison.OrdinalIgnoreCase)
                || token.Equals("inch", StringComparison.OrdinalIgnoreCase)
                || token.Equals("inches", StringComparison.OrdinalIgnoreCase))
            {
                return DisplayLengthUnit.Inch;
            }

            if (float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out float legacyValue))
            {
                int legacyId = Convert.ToInt32(Math.Round(legacyValue, MidpointRounding.AwayFromZero));
                if (Enum.IsDefined(typeof(DisplayLengthUnit), legacyId))
                    return (DisplayLengthUnit)legacyId;
            }

            return DisplayLengthUnit.Millimeter;
        }
    }
}
