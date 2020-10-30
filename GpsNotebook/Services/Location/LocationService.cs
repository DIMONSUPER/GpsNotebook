using System;
using System.Threading.Tasks;
using GpsNotebook.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms.GoogleMaps;

namespace GpsNotebook.Services.Location
{
    public class LocationService : ILocationService
    {
        #region -- ILocationService implementation --

        public async Task<bool> CanGetCurrenLocationAsync()
        {
            Xamarin.Essentials.Location location = null;

            try
            {
                await Geolocation.GetLastKnownLocationAsync();

                if (location == null)
                {
                    location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Medium,
                        Timeout = TimeSpan.FromSeconds(30)
                    });
                }
            }
            catch
            {
            }

            return location != null;
        }

        public CameraPosition GetCameraLocation()
        {
            CameraPosition result = null;
            if (!string.IsNullOrEmpty(Settings.RememberedLatitude) && double.TryParse(Settings.RememberedLatitude, out double latitude)
                && !string.IsNullOrEmpty(Settings.RememberedLongitude) && double.TryParse(Settings.RememberedLongitude, out double longitude)
                && !string.IsNullOrEmpty(Settings.RememberedZoom) && double.TryParse(Settings.RememberedZoom, out double zoom))
            {
                result = new CameraPosition(new Position(latitude, longitude), zoom);
            }
            return result;
        }

        public void SetCameraLocation(CameraPosition cameraPosition)
        {
            if (cameraPosition != null)
            {
                Settings.RememberedLatitude = cameraPosition.Target.Latitude.ToString();
                Settings.RememberedLongitude = cameraPosition.Target.Longitude.ToString();
                Settings.RememberedZoom = cameraPosition.Zoom.ToString();
            }
        }

        #endregion
    }
}
