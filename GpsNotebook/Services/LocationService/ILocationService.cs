using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.GoogleMaps;

namespace GpsNotebook.Services.LocationService
{
    public interface ILocationService
    {
        CameraPosition GetCameraLocation();
        void SetCameraLocation(CameraPosition cameraPosition);
        Task<Location> GetCurrenLocationAsync();
    }
}
