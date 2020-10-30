using Acr.UserDialogs;
using GpsNotebook.Models;
using GpsNotebook.Resources;
using GpsNotebook.Services.Location;
using GpsNotebook.Services.Pin;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GpsNotebook.ViewModels
{
    public class AddPinPageViewModel : ViewModelBase
    {
        private IPinService PinService { get; }
        private IUserDialogs UserDialogs { get; }
        private ILocationService LocationService { get; }

        public AddPinPageViewModel(
            INavigationService navigationService,
            IPinService pinService,
            IUserDialogs userDialogs,
            ILocationService locationService)
            : base(navigationService)
        {
            PinService = pinService;
            UserDialogs = userDialogs;
            LocationService = locationService;
        }

        #region -- Public properties --

        public ICommand MapClickedCommand => new Command<Position>(MapClick);
        public ICommand SaveClickedCommand => new Command(SaveClick);

        private string label;
        public string Label
        {
            get { return label; }
            set { SetProperty(ref label, value); }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        private bool isFavourite = true;
        public bool IsFavourite
        {
            get { return isFavourite; }
            set { SetProperty(ref isFavourite, value); }
        }

        private double latitude;
        public double Latitude
        {
            get { return latitude; }
            set { SetProperty(ref latitude, value); }
        }

        private double longitude;
        public double Longitude
        {
            get { return longitude; }
            set { SetProperty(ref longitude, value); }
        }

        private CameraPosition mapCameraPosition;
        public CameraPosition MapCameraPosition
        {
            get { return mapCameraPosition; }
            set { SetProperty(ref mapCameraPosition, value); }
        }

        private ObservableCollection<PinModel> pins;
        public ObservableCollection<PinModel> Pins
        {
            get { return pins; }
            set { SetProperty(ref pins, value); }
        }

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (Pins == null)
            {
                Pins = new ObservableCollection<PinModel>();
            }
            if (parameters.TryGetValue(nameof(PinModel), out PinModel pinModel))
            {
                if (double.TryParse(pinModel.Latitude, out double latitude)
                    && double.TryParse(pinModel.Longitude, out double longitude))
                {
                    Latitude = latitude;
                    Longitude = longitude;
                }

                pinId = pinModel.Id;
                Label = pinModel.Label;
                Description = pinModel.Description;
                IsFavourite = pinModel.IsFavourite;

                Pins.Add(pinModel);

                MapCameraPosition = new CameraPosition(new Position(Latitude, Longitude), 10d);
            }
            else
            {
                MapCameraPosition = LocationService.GetCameraLocation();
            }
        }

        #endregion

        #region -- Private helpers --
        private int pinId;

        private void MapClick(Position point)
        {
            if (string.IsNullOrEmpty(Label))
            {
                Label = "NewPin";
            }
            Latitude = point.Latitude;
            Longitude = point.Longitude;
            Pins.Clear();

            var newPin = new PinModel
            {
                Label = Label,
                Latitude = Latitude.ToString(),
                Longitude = Longitude.ToString(),
            };

            Pins.Add(newPin);
        }

        private async void SaveClick()
        {
            if (!string.IsNullOrEmpty(Label)
                && Latitude != 0 && Longitude != 0)
            {
                PinModel pinModel = new PinModel
                {
                    Id = pinId,
                    Label = Label,
                    Description = Description,
                    IsFavourite = IsFavourite,
                    Latitude = Latitude.ToString(),
                    Longitude = Longitude.ToString(),
                };

                int result = await PinService.AddPinAsync(pinModel);

                if (result != -1)
                {
                    await NavigationService.GoBackAsync();
                }
            }
            else
            {
                UserDialogs.Alert(AppResources.FieldsFilledError);
            }
        }

        #endregion
    }
}
