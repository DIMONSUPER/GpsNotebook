using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Clustering;

namespace GpsNotebook.Controls
{
    public class MyMap : ClusteredMap
    {
        public MyMap()
        {
            MoveCamera(CameraUpdateFactory.NewCameraPosition(MapCameraPosition));
            CameraIdled += OnCameraIdled;
        }

        public static readonly BindableProperty MapCameraPositionProperty = BindableProperty.Create(
            nameof(MapCameraPosition),
            typeof(CameraPosition),
            typeof(MyMap),
            propertyChanged: OnMapCameraPositionPropertyChanged);

        public CameraPosition MapCameraPosition
        {
            get { return (CameraPosition)GetValue(MapCameraPositionProperty); }
            set { SetValue(MapCameraPositionProperty, value); }
        }

        private void OnCameraIdled(object sender, CameraIdledEventArgs e)
        {
            MapCameraPosition = e.Position;
        }

        private static void OnMapCameraPositionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is MyMap castedMap
                && newValue is CameraPosition cameraPosition
                && cameraPosition.Target != castedMap.CameraPosition.Target)
            {
                castedMap.InitialCameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                castedMap.MoveCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
            }
        }
    }
}
