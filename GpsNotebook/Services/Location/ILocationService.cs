using System.Threading.Tasks;
using Xamarin.Forms.GoogleMaps;

namespace GpsNotebook.Services.Location
{
    public interface ILocationService
    {
        CameraPosition GetCameraLocation();

        void SetCameraLocation(CameraPosition cameraPosition);

        Task<bool> CanGetCurrenLocationAsync();
    }
}
