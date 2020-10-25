using GpsNotebook.Helpers;
using GpsNotebook.Models;
using GpsNotebook.Services;
using GpsNotebook.Views;
using Prism.Navigation;
using Prism.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace GpsNotebook.ViewModels
{
    public class AllPinsPageViewModel : ViewModelBase
    {
        public ICommand LogOutClickedCommand => new Command(LogOutClick);
        public ICommand AddButtonClickedCommand => new Command(AddButtonClick);
        public ICommand PinClickedCommand => new Command<PinModel>(PinClick);

        private IRepositoryService RepositoryService { get; }

        public AllPinsPageViewModel(
            INavigationService navigationService,
            IRepositoryService repositoryService)
            : base(navigationService)
        {
            RepositoryService = repositoryService;
            RefreshList();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            RefreshList();
        }

        private PinModel selectedPin;
        public PinModel SelectedPin
        {
            get { return selectedPin; }
            set
            {
                SetProperty(ref selectedPin, value);
                PinClickedCommand.Execute(selectedPin);
            }
        }

        private ObservableCollection<PinModel> placesList;
        public ObservableCollection<PinModel> PlacesList
        {
            get { return placesList; }
            set { SetProperty(ref placesList, value); }
        }

        private async void AddButtonClick()
        {
            await NavigationService.NavigateAsync(nameof(AddPinPage));
        }

        public async void RefreshList()
        {
            var pins = await RepositoryService.GetAllAsync<PinModel>(p => p.UserId == Settings.RememberedUserId);

            if (pins.Count != 0)
            {
                PlacesList = new ObservableCollection<PinModel>(pins);
            }
        }

        private async void LogOutClick()
        {
            Settings.RememberedEmail = string.Empty;
            await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(SignInPage)}");
        }

        private async void PinClick(PinModel model)
        {
            var parameters = new NavigationParameters();

            //await NavigationService.NavigateAsync("ImagePopupPage", parameters);
        }
    }
}

