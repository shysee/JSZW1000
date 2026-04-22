using System.Globalization;
using System.Resources;

namespace JSZW1000A
{
    internal static class Strings
    {
        private static readonly ResourceManager ResourceManager = new("JSZW1000A.Strings", typeof(Strings).Assembly);

        public static string? TryGet(string key)
        {
            return ResourceManager.GetString(key, LocalizationManager.CurrentUICulture);
        }

        public static string Get(string key, string? fallback = null)
        {
            string? value = TryGet(key);
            return value ?? fallback ?? key;
        }

        public static string Format(string key, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, Get(key), args);
        }
    }
}
