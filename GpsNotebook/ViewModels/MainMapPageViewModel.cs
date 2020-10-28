using Acr.UserDialogs;
using GpsNotebook.Helpers;
using GpsNotebook.Models;
using GpsNotebook.Services;
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

        private IRepositoryService RepositoryService { get; }

        public MainMapPageViewModel(
            INavigationService navigationService,
            IRepositoryService repositoryService)
            : base(navigationService)
        {
            RepositoryService = repositoryService;
            RepositoryService.InitTable<PinModel>();
            PlacesList = new ObservableCollection<PinModel>();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            await LoadPins();

            if (parameters.TryGetValue("SelectedPin", out PinModel selectedPin))
            {
                PlacesList.Add(selectedPin);
            }

            if (parameters.TryGetValue(nameof(MapCameraPosition), out CameraPosition mapCameraPosition))
            {
                MapCameraPosition = mapCameraPosition;
            }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            parameters.Add(nameof(MapCameraPosition), MapCameraPosition);
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
                var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Low,
                    Timeout = TimeSpan.FromSeconds(30)
                });

                if (location == null)
                {
                    location = await Geolocation.GetLastKnownLocationAsync();
                }

                if (location != null)
                {
                    MapCameraPosition = new CameraPosition(new Position(location.Latitude, location.Longitude), 10d);

                    //await UserDialogs.Instance.AlertAsync($"{location.Latitude}; {location.Longitude}");
                }
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

        public async Task LoadPins()
        {
            var pins = await RepositoryService.GetAllAsync<PinModel>(p =>
            (p.UserId == Settings.RememberedUserId && p.IsFavourite == true));

            PlacesList = new ObservableCollection<PinModel>(pins);
        }
    }
}

