using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace GpsNotebook.Helpers
{
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string RememberedEmailSettingsKey = "remembered_email_key";
        private const string RememberedUserIdSettingsKey = "remembered_userid_key";
        private const string RememberedLatitudeSettingsKey = "remembered_latitude_key";
        private const string RememberedLongitudeSettingsKey = "remembered_lontitude_key";
        private const string RememberedZoomSettingsKey = "remembered_zoom_key";
        private static readonly string SettingsDefault = string.Empty;

        #endregion

        public static int RememberedUserId
        {
            get
            {
                return AppSettings.GetValueOrDefault(RememberedUserIdSettingsKey, 0);
            }
            set
            {
                AppSettings.AddOrUpdateValue(RememberedUserIdSettingsKey, value);
            }
        }

        public static string RememberedEmail
        {
            get
            {
                return AppSettings.GetValueOrDefault(RememberedEmailSettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(RememberedEmailSettingsKey, value);
            }
        }

        public static string RememberedLatitude
        {
            get
            {
                return AppSettings.GetValueOrDefault(RememberedLatitudeSettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(RememberedLatitudeSettingsKey, value);
            }
        }
            
        public static string RememberedLongitude
        {
            get
            {
                return AppSettings.GetValueOrDefault(RememberedLongitudeSettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(RememberedLongitudeSettingsKey, value);
            }
        }

        public static string RememberedZoom
        {
            get
            {
                return AppSettings.GetValueOrDefault(RememberedZoomSettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(RememberedZoomSettingsKey, value);
            }
        }
    }
}
