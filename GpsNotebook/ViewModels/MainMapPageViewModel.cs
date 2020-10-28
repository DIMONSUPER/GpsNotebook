using Acr.UserDialogs;
using GpsNotebook.Helpers;
using GpsNotebook.Models;
using GpsNotebook.Services;
using GpsNotebook.Services.LocationService;
using GpsNotebook.Views;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GpsNotebook.ViewModels
{
    public class MainMapPageViewModel : ViewModelBase
    {
        public ICommand PinClickedCommand => new Command<Pin>(PinClick);
        public ICommand LogOutClickedCommand => new Command(LogOutClick);
        public ICommand CurrentLocationClickedCommand => new Command(CurrentLocationClick);
        public ICommand SearchOnTextChangedCommand => new Command(SearchOnTextChanged);
        public ICommand CameraChangedCommand => new Command(CameraChanged);

        private IRepositoryService RepositoryService { get; }
        private ILocationService LocationService { get; }

        public MainMapPageViewModel(
            INavigationService navigationService,
            IRepositoryService repositoryService,
            ILocationService locationService)
            : base(navigationService)
        {
            RepositoryService = repositoryService;
            LocationService = locationService;
            RepositoryService.InitTable<PinModel>();
            PlacesList = new ObservableCollection<PinModel>();

            MapCameraPosition = LocationService.GetCameraLocation();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            await LoadPins();
            MapCameraPosition = LocationService.GetCameraLocation();

            if (parameters.TryGetValue("SelectedPin", out PinModel selectedPin))
            {
                PlacesList.Add(selectedPin);
            }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            parameters.Add(nameof(MapCameraPosition), MapCameraPosition);
        }

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

        private void CameraChanged()
        {
            LocationService.SetCameraLocation(MapCameraPosition);
        }

        private async void SearchOnTextChanged()
        {
            PlacesList.Clear();
            if (string.IsNullOrEmpty(SearchBarText))
            {
                await LoadPins();
            }
            else
            {
                var pins = await RepositoryService.GetAllAsync<PinModel>(p =>
                (p.UserId == Settings.RememberedUserId && p.IsFavourite == true)
                && (p.Label.Contains(SearchBarText) || p.Description.Contains(SearchBarText)
                || p.Latitude.Contains(SearchBarText) || p.Longitude.Contains(SearchBarText)));

                PlacesList = new ObservableCollection<PinModel>(pins);
            }
        }

        private async void PinClick(Pin pin)
        {
            var parameters = new NavigationParameters
            {
                { nameof(pin.Label), pin.Label },
                { nameof(pin.Address), pin.Address },
                { nameof(pin.Position), pin.Position }
            };

            //await NavigationService.NavigateAsync(nameof(PinInfoPage), parameters);
        }

        private async void CurrentLocationClick()
        {
            try
            {
                var location = await LocationService.GetCurrenLocationAsync();
                MapCameraPosition = new CameraPosition(new Position(location.Latitude, location.Longitude), 10d);
            }
            catch
            {
            }
        }

        private async void LogOutClick()
        {
            Settings.RememberedEmail = string.Empty;
            await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(SignInPage)}");
        }

        private async Task LoadPins()
        {
            var pins = await RepositoryService.GetAllAsync<PinModel>(p =>
            (p.UserId == Settings.RememberedUserId && p.IsFavourite == true));

            PlacesList = new ObservableCollection<PinModel>(pins);
        }
    }
}

