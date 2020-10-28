using Acr.UserDialogs;
using GpsNotebook.Helpers;
using GpsNotebook.Models;
using GpsNotebook.Resources;
using GpsNotebook.Services;
using GpsNotebook.Services.LocationService;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GpsNotebook.ViewModels
{
    public class AddPinPageViewModel : ViewModelBase
    {
        public ICommand MapClickedCommand => new Command<Position>(MapClick);
        public ICommand SaveClickedCommand => new Command(SaveClick);

        private IRepositoryService RepositoryService { get; }
        private IUserDialogs UserDialogs { get; }
        private ILocationService LocationService { get; }

        private int pinId;

        public AddPinPageViewModel(
            INavigationService navigationService,
            IRepositoryService repositoryService,
            IUserDialogs userDialogs,
            ILocationService locationService)
            : base(navigationService)
        {
            RepositoryService = repositoryService;
            UserDialogs = userDialogs;
            LocationService = locationService;
            RepositoryService.InitTable<PinModel>();
        }

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

        private bool isFavourite;
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
                Description = Description,
                IsFavourite = IsFavourite,
                Latitude = Latitude.ToString(),
                Longitude = Longitude.ToString(),
                UserId = Settings.RememberedUserId
            };
            Pins.Add(newPin);
        }

        private async void SaveClick()
        {
            if (!string.IsNullOrEmpty(Label)
                && Latitude != 0 && Longitude != 0)
            {
                int result = await RepositoryService.InsertAsync(new PinModel
                {
                    Id = pinId,
                    Label = Label,
                    Description = Description,
                    IsFavourite = IsFavourite,
                    Latitude = Latitude.ToString(),
                    Longitude = Longitude.ToString(),
                    UserId = Settings.RememberedUserId
                });

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
    }
}
