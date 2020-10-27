using Acr.UserDialogs;
using GpsNotebook.Helpers;
using GpsNotebook.Models;
using GpsNotebook.Services;
using GpsNotebook.Views;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GpsNotebook.ViewModels
{
    public class MainMapPageViewModel : ViewModelBase
    {
        public ICommand PinClickedCommand => new Command<PinClickedEventArgs>(PinClick);
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
            MapCameraPosition = new CameraPosition(new Position(45.34546, 28.79337), 8d);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(nameof(MapCameraPosition), out CameraPosition mapCameraPosition))
            {
                MapCameraPosition = mapCameraPosition;
            }

            LoadPins();
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

        private async void PinClick(PinClickedEventArgs args)
        {
            var parameters = new NavigationParameters
            {
                { nameof(args.Pin.Label),args.Pin.Label },
                { nameof(args.Pin.Address),args.Pin.Address },
                { nameof(args.Pin.Position),args.Pin.Position }
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

        public async void LoadPins()
        {
            var pins = await RepositoryService.GetAllAsync<PinModel>(p =>
            (p.UserId == Settings.RememberedUserId && p.IsFavoirite == true));

            if (pins.Count != 0)
            {
                PlacesList = new ObservableCollection<PinModel>(pins);
            }
        }
    }
}

