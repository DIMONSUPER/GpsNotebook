using GpsNotebook.Models;
using GpsNotebook.Services.Authorization;
using GpsNotebook.Services.Location;
using GpsNotebook.Services.Permissions;
using GpsNotebook.Services.Pin;
using GpsNotebook.Views;
using Plugin.Permissions.Abstractions;
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
        private readonly IAuthorizationService _authorizationService;
        private readonly IPinService _pinService;
        private readonly ILocationService _locationService;
        private readonly IPermissionsService _permissionsService;

        public MainMapPageViewModel(
            INavigationService navigationService,
            IAuthorizationService authorizationService,
            IPinService pinService,
            ILocationService locationService,
            IPermissionsService permissionsService)
            : base(navigationService)
        {
            _authorizationService = authorizationService;
            _pinService = pinService;
            _locationService = locationService;
            _permissionsService = permissionsService;

            InitializeCurrentLocation();

            MapCameraPosition = _locationService.GetCameraLocation();
        }

        #region -- Public properties --

        public ICommand PinClickedCommand => new Command<Pin>(OnPinClick);
        public ICommand LogOutClickedCommand => new Command(OnLogOutClick);
        public ICommand TextChangedCommand => new Command(OnTextChanged);
        public ICommand CameraChangedCommand => new Command(OnCameraChanged);
        public ICommand CloseButtonClickedCommand => new Command(OnCloseButtonClicked);

        private string _searchBarText;
        public string SearchBarText
        {
            get { return _searchBarText; }
            set { SetProperty(ref _searchBarText, value); }
        }

        private bool _isInfoVisible;
        public bool IsInfoVisible
        {
            get { return _isInfoVisible; }
            set { SetProperty(ref _isInfoVisible, value); }
        }

        private bool _isMyLocationEnabled;
        public bool IsMyLocationEnabled
        {
            get { return _isMyLocationEnabled; }
            set { SetProperty(ref _isMyLocationEnabled, value); }
        }

        private PinModel _selectedPin;
        public PinModel SelectedPin
        {
            get { return _selectedPin; }
            set { SetProperty(ref _selectedPin, value); }
        }

        private ObservableCollection<PinModel> _placesList;
        public ObservableCollection<PinModel> PlacesList
        {
            get { return _placesList; }
            set { SetProperty(ref _placesList, value); }
        }

        private CameraPosition _mapCameraPosition;
        public CameraPosition MapCameraPosition
        {
            get { return _mapCameraPosition; }
            set { SetProperty(ref _mapCameraPosition, value); }
        }

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            MapCameraPosition = _locationService.GetCameraLocation();
            await LoadPins();

            if (parameters.TryGetValue(nameof(SelectedPin), out PinModel selectedPin))
            {
                SelectedPin = selectedPin;
                PlacesList.Add(selectedPin);

                _locationService.SetCameraLocation(new CameraPosition(selectedPin.Position, 10d));
                MapCameraPosition = _locationService.GetCameraLocation();

                IsInfoVisible = true;
            }
            else
            {
                IsInfoVisible = false;
            }
        }

        #endregion

        #region -- Private helpers --

        private void OnCameraChanged()
        {
            _locationService.SetCameraLocation(MapCameraPosition);
        }

        private async void OnTextChanged()
        {
            await LoadPins();
        }

        private async Task LoadPins()
        {
            List<PinModel> pins = await _pinService.GetPinsAsync(SearchBarText);
            var favourites = pins.Where(p => p.IsFavourite == true);

            PlacesList = new ObservableCollection<PinModel>(favourites);
        }

        private async void OnPinClick(Pin pin)
        {
            PinModel selectedPin = await _pinService.GetByLabelAsync(pin.Label);

            SelectedPin = selectedPin;

            IsInfoVisible = true;
        }

        private async void OnLogOutClick()
        {
            _authorizationService.LogOut();
            await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(SignInPage)}");
        }

        private void OnCloseButtonClicked()
        {
            IsInfoVisible = false;
        }

        private async void InitializeCurrentLocation()
        {
            bool result = await _permissionsService.GetPermissionAsync(Permission.Location);
            IsMyLocationEnabled = result;
        }

        #endregion
    }
}

