using Acr.UserDialogs;
using GpsNotebook.Helpers;
using GpsNotebook.Models;
using GpsNotebook.Resources;
using GpsNotebook.Services;
using GpsNotebook.Views;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GpsNotebook.ViewModels
{
    public class AllPinsPageViewModel : ViewModelBase
    {
        public ICommand LogOutClickedCommand => new Command(LogOutClick);
        public ICommand AddButtonClickedCommand => new Command(AddButtonClick);
        public ICommand EditClickedCommand => new Command<PinModel>(EditClick);
        public ICommand DeleteClickedCommand => new Command<PinModel>(DeleteClick);
        public ICommand PinClickedCommand => new Command<PinModel>(PinClick);
        public ICommand SearchOnTextChangedCommand => new Command(SearchOnTextChanged);

        private IRepositoryService RepositoryService { get; }
        private IUserDialogs UserDialogs { get; }

        public AllPinsPageViewModel(
            INavigationService navigationService,
            IRepositoryService repositoryService,
            IUserDialogs userDialogs)
            : base(navigationService)
        {
            RepositoryService = repositoryService;
            UserDialogs = userDialogs;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            await RefreshList();
        }

        private string _searchBarText;
        public string SearchBarText
        {
            get { return _searchBarText; }
            set { SetProperty(ref _searchBarText, value); }
        }

        private CameraPosition mapCameraPosition;
        public CameraPosition MapCameraPosition
        {
            get { return mapCameraPosition; }
            set { SetProperty(ref mapCameraPosition, value); }
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

        private async void DeleteClick(PinModel pinModel)
        {
            var result = await UserDialogs.ConfirmAsync(new ConfirmConfig()
                .SetTitle(AppResources.ConfirmationTitle)
                .SetOkText(AppResources.Yes)
                .SetCancelText(AppResources.No));

            if (result)
            {
                await RepositoryService.DeleteAsync(pinModel);
                await RefreshList();
            }
        }

        private async void SearchOnTextChanged()
        {
            PlacesList.Clear();
            if (string.IsNullOrEmpty(SearchBarText))
            {
                await RefreshList();
            }
            else
            {
                var pins = await RepositoryService.GetAllAsync<PinModel>(p => (p.UserId == Settings.RememberedUserId)
                && (p.Label.Contains(SearchBarText) || p.Description.Contains(SearchBarText)
                || p.Latitude.Contains(SearchBarText) || p.Longitude.Contains(SearchBarText)));

                PlacesList = new ObservableCollection<PinModel>(pins);
            }
        }

        private async void EditClick(PinModel pinModel)
        {
            var parametеrs = new NavigationParameters
            {
                { nameof(PinModel), pinModel }
            };

            await NavigationService.NavigateAsync(nameof(AddPinPage), parametеrs);
        }

        private async void AddButtonClick()
        {
            await NavigationService.NavigateAsync(nameof(AddPinPage));
        }

        private async Task RefreshList()
        {
            var pins = await RepositoryService.GetAllAsync<PinModel>(p => p.UserId == Settings.RememberedUserId);

            PlacesList = new ObservableCollection<PinModel>(pins);
        }

        private async void LogOutClick()
        {
            Settings.RememberedEmail = string.Empty;
            await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(SignInPage)}");
        }

        private async void PinClick(PinModel model)
        {
            if (double.TryParse(model.Latitude, out double latitude)
                && double.TryParse(model.Longitude, out double longitude))
            {
                MapCameraPosition = new CameraPosition(new Position(latitude, longitude), 10d);
                var parameters = new NavigationParameters
            {
                { nameof(MapCameraPosition), MapCameraPosition },
                { nameof(SelectedPin), SelectedPin }
            };

                await NavigationService.NavigateAsync(
                    $"/{nameof(NavigationPage)}" +
                    $"/{nameof(MainTabbedPage)}" +
                    $"?{nameof(KnownNavigationParameters.SelectedTab)}" +
                    $"={nameof(MainMapPage)}", parameters);
            }
        }
    }
}

