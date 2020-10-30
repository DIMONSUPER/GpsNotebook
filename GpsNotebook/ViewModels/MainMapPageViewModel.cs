using GpsNotebook.Models;
using GpsNotebook.Services.Authorization;
using GpsNotebook.Services.Location;
using GpsNotebook.Services.Pin;
using GpsNotebook.Views;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GpsNotebook.ViewModels
{
    public class MainMapPageViewModel : ViewModelBase
    {
        private readonly IAuthorizationService AuthorizationService;
        private readonly IPinService PinService;
        private readonly ILocationService LocationService;

        public MainMapPageViewModel(
            INavigationService navigationService,
            IAuthorizationService authorizationService,
            IPinService pinService,
            ILocationService locationService)
            : base(navigationService)
        {
            AuthorizationService = authorizationService;
            PinService = pinService;
            LocationService = locationService;
            PlacesList = new ObservableCollection<PinModel>();

            MapCameraPosition = LocationService.GetCameraLocation();
        }

        #region -- Public properties --

        public ICommand PinClickedCommand => new Command<Pin>(OnPinClick);
        public ICommand LogOutClickedCommand => new Command(OnLogOutClick);
        public ICommand CurrentLocationClickedCommand => new Command(OnCurrentLocationClick);
        public ICommand TextChangedCommand => new Command(OnTextChanged);
        public ICommand CameraChangedCommand => new Command(OnCameraChanged);

        private string _searchBarText;
        public string SearchBarText
        {
            get { return _searchBarText; }
            set { SetProperty(ref _searchBarText, value); }
        }

        private ObservableCollection<PinModel> placesList;
        public ObservableCollection<PinModel> PlacesList
        {
            get { return placesList; }
            set { SetProperty(ref placesList, value); }
        }

        private CameraPosition mapCameraPosition;
        public CameraPosition MapCameraPosition
        {
            get { return mapCameraPosition; }
            set { SetProperty(ref mapCameraPosition, value); }
        }

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            MapCameraPosition = LocationService.GetCameraLocation();
            await LoadPins();

            if (parameters.TryGetValue("SelectedPin", out PinModel selectedPin))
            {
                PlacesList.Add(selectedPin);
                LocationService.SetCameraLocation(new CameraPosition(selectedPin.Position, 10d));
                MapCameraPosition = LocationService.GetCameraLocation();
            }
        }

        #endregion

        #region -- Private helpers --

        private void OnCameraChanged()
        {
            LocationService.SetCameraLocation(MapCameraPosition);
        }

        private async void OnTextChanged()
        {
            await LoadPins();
        }

        private async Task LoadPins()
        {
            List<PinModel> pins = await PinService.GetPinsAsync(SearchBarText);
            var favourites = pins.Where(p => p.IsFavourite == true);

            PlacesList = new ObservableCollection<PinModel>(favourites);
        }

        private async void OnPinClick(Pin pin)
        {
            var selectedPin = await PinService.GetByPosition(pin.Position);

            var parameters = new NavigationParameters
            {
                { nameof(selectedPin), selectedPin }
            };

            //await NavigationService.NavigateAsync(nameof(PinInfoPage), parameters);
            await NavigationService.NavigateAsync(nameof(PinInfoPage), parameters);
            //await PopupNavigation.Instance.PushAsync(new PinInfoPage());
        }

        private async void OnCurrentLocationClick()
        {
            var location = await LocationService.GetCurrenLocationAsync();
            LocationService.SetCameraLocation(new CameraPosition(new Position(location.Latitude, location.Longitude), MapCameraPosition.Zoom));
            MapCameraPosition = LocationService.GetCameraLocation();
        }

        private async void OnLogOutClick()
        {
            AuthorizationService.LogOut();
            await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(SignInPage)}");
        }

        #endregion
    }
}

