using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherApp.Resources
{
    public static class Translations
    {
        public static string GetSunriseText()
        {
            return AppTranslations.Sunrise;
        }
        public static string GetSunsetText()
        {
            return AppTranslations.Sunset;
        }
        public static string GetNoInternetConnectionText()
        {
            return AppTranslations.NoInternetConnection;
        }
        public static string GetMissingLocalizationPermissionsText()
        {
            return AppTranslations.MissingLocalizationPermissions;
        }

    }
}
