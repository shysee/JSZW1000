using System.ComponentModel;
using System.Globalization;

namespace JSZW1000A
{
    internal static class LocalizationManager
    {
        private static readonly string DefaultConfigPath = Path.Combine(Application.StartupPath, "Config.ini");

        public static AppLanguage CurrentLanguage { get; private set; } = AppLanguage.ZhCn;

        public static CultureInfo CurrentUICulture { get; private set; } = GetCulture(CurrentLanguage);

        public static void InitializeFromConfig(string? configPath = null)
        {
            UseSessionLanguage(ReadConfiguredLanguage(configPath));
        }

        public static void UseSessionLanguage(AppLanguage language)
        {
            CurrentLanguage = language;
            CurrentUICulture = GetCulture(language);

            CultureInfo.CurrentUICulture = CurrentUICulture;
            CultureInfo.DefaultThreadCurrentUICulture = CurrentUICulture;

            MainFrm.Lang = ToLegacyLanguageId(language);
        }

        public static CultureInfo GetCulture(AppLanguage language)
        {
            return language switch
            {
                AppLanguage.EnUs => new CultureInfo("en-US"),
                AppLanguage.FrFr => new CultureInfo("fr-FR"),
                AppLanguage.RuRu => new CultureInfo("ru-RU"),
                _ => new CultureInfo("zh-CN")
            };
        }

        public static int ToLegacyLanguageId(AppLanguage language)
        {
            return (int)language;
        }

        public static AppLanguage FromLegacyLanguageId(float languageId)
        {
            int legacyId = Convert.ToInt32(Math.Round(languageId, MidpointRounding.AwayFromZero));
            return Enum.IsDefined(typeof(AppLanguage), legacyId)
                ? (AppLanguage)legacyId
                : AppLanguage.ZhCn;
        }

        public static AppLanguage ReadConfiguredLanguage(string? configPath = null)
        {
            string path = configPath ?? DefaultConfigPath;
            if (!File.Exists(path))
                return AppLanguage.ZhCn;

            foreach (string rawLine in File.ReadLines(path))
            {
                string line = rawLine.Trim();
                if (!line.StartsWith("Language ", StringComparison.OrdinalIgnoreCase))
                    continue;

                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length <= 1)
                    return AppLanguage.ZhCn;

                if (float.TryParse(parts[^1], NumberStyles.Float, CultureInfo.InvariantCulture, out float languageId))
                    return FromLegacyLanguageId(languageId);

                return AppLanguage.ZhCn;
            }

            return AppLanguage.ZhCn;
        }

        public static bool SaveLanguage(AppLanguage language, string? configPath = null)
        {
            string path = configPath ?? DefaultConfigPath;
            if (!File.Exists(path))
                return false;

            string[] lines = File.ReadAllLines(path);
            bool updated = false;
            string replacement = string.Format(CultureInfo.InvariantCulture, "Language {0:F2}", ToLegacyLanguageId(language));

            for (int i = 0; i < lines.Length; i++)
            {
                string trimmed = lines[i].TrimStart();
                if (!trimmed.StartsWith("Language ", StringComparison.OrdinalIgnoreCase))
                    continue;

                lines[i] = replacement;
                updated = true;
                break;
            }

            if (!updated)
                return false;

            File.WriteAllLines(path, lines);
            MainFrm.ConfigData[8] = ToLegacyLanguageId(language);
            return true;
        }

        public static string GetLanguageDisplayName(AppLanguage language)
        {
            return language switch
            {
                AppLanguage.EnUs => Strings.Get("Language.Display.EnUs", "English"),
                AppLanguage.FrFr => Strings.Get("Language.Display.FrFr", "Français"),
                AppLanguage.RuRu => Strings.Get("Language.Display.RuRu", "Русский"),
                _ => Strings.Get("Language.Display.ZhCn", "中文")
            };
        }

        public static void PromptReload(IWin32Window? owner = null)
        {
            MessageBox.Show(
                owner,
                Strings.Get("Localization.RestartRequired.Message", "语言切换将在重开页面或重启软件后生效。"),
                Strings.Get("Localization.RestartRequired.Title", "语言设置"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public static void ApplyResources(Control root)
        {
            ComponentResourceManager manager = new(root.GetType());
            ApplyResourcesRecursive(manager, root, true);
            ApplySharedTextResources(root, root.GetType().Name);
        }

        private static void ApplyResourcesRecursive(ComponentResourceManager manager, Control control, bool isRoot = false)
        {
            manager.ApplyResources(control, isRoot ? "$this" : control.Name, CurrentUICulture);

            if (control is DataGridView dataGridView)
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                    manager.ApplyResources(column, column.Name, CurrentUICulture);
            }

            foreach (Control child in control.Controls)
                ApplyResourcesRecursive(manager, child);
        }

        private static void ApplySharedTextResources(Control root, string scope)
        {
            ApplySharedTextRecursive(root, scope, true);
        }

        private static void ApplySharedTextRecursive(Control control, string scope, bool isRoot = false)
        {
            string key = isRoot ? $"{scope}.$this.Text" : $"{scope}.{control.Name}.Text";
            string? localizedText = Strings.TryGet(key);
            if (!string.IsNullOrEmpty(localizedText))
                control.Text = localizedText;

            if (control is DataGridView dataGridView)
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    string? headerText = Strings.TryGet($"{scope}.{column.Name}.HeaderText");
                    if (!string.IsNullOrEmpty(headerText))
                        column.HeaderText = headerText;
                }
            }

            foreach (Control child in control.Controls)
                ApplySharedTextRecursive(child, scope);
        }
    }
}
