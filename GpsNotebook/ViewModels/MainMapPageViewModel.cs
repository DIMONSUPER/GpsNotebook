using GpsNotebook.Helpers;
using GpsNotebook.Models;
using GpsNotebook.Services;
using GpsNotebook.Views;
using Prism.Navigation;
using Prism.Services;
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

        private IRepositoryService RepositoryService { get; }

        public MainMapPageViewModel(
            INavigationService navigationService,
            IRepositoryService repositoryService)
            : base(navigationService)
        {
            RepositoryService = repositoryService;
            RepositoryService.InitTable<PinModel>();
            PlacesList = new ObservableCollection<PinModel>();
            LoadPins();
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            LoadPins();
        }

        private ObservableCollection<PinModel> placesList;
        public ObservableCollection<PinModel> PlacesList
        {
            get { return placesList; }
            set { SetProperty(ref placesList, value); }
        }

        private void PinClick(PinClickedEventArgs args)
        {
            var parameters = new NavigationParameters
            {
                { nameof(args.Pin.Label),args.Pin.Label },
                { nameof(args.Pin.Address),args.Pin.Address },
                { nameof(args.Pin.Position),args.Pin.Position }
            };
            NavigationService.NavigateAsync(nameof(PinInfoPage), parameters);
        }

        private async void LogOutClick()
        {
            Settings.RememberedEmail = string.Empty;
            await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(SignInPage)}");
        }

        public async void LoadPins()
        {
            var pins = await RepositoryService.GetAllAsync<PinModel>(p => p.UserId == Settings.RememberedUserId);

            if (pins.Count != 0)
            {
                PlacesList = new ObservableCollection<PinModel>(pins);
            }
        }
    }
}

