using Acr.UserDialogs;
using GpsNotebook.Models;
using GpsNotebook.Resources;
using GpsNotebook.Services.Authorization;
using GpsNotebook.Services.Pin;
using GpsNotebook.Views;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GpsNotebook.ViewModels
{
    public class AllPinsPageViewModel : ViewModelBase
    {
        private readonly IAuthorizationService AuthorizationService;
        private readonly IPinService PinService;
        private readonly IUserDialogs UserDialogs;

        public AllPinsPageViewModel(
            INavigationService navigationService,
            IAuthorizationService authorizationService,
            IPinService pinService,
            IUserDialogs userDialogs)
            : base(navigationService)
        {
            AuthorizationService = authorizationService;
            PinService = pinService;
            UserDialogs = userDialogs;
            PlacesList = new ObservableCollection<PinModel>();
        }

        #region -- Public properties --

        public ICommand LogOutClickedCommand => new Command(OnLogOutClick);
        public ICommand AddButtonClickedCommand => new Command(OnAddButtonClick);
        public ICommand EditClickedCommand => new Command<PinModel>(OnEditClick);
        public ICommand DeleteClickedCommand => new Command<PinModel>(OnDeleteClick);
        public ICommand PinClickedCommand => new Command<PinModel>(OnPinClick);
        public ICommand TextChangedCommand => new Command(OnTextChanged);

        private string _searchBarText;
        public string SearchBarText
        {
            get { return _searchBarText; }
            set { SetProperty(ref _searchBarText, value); }
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

        #endregion

        #region -- Overrides --

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            await LoadList();
        }

        #endregion

        #region -- Private helpers --

        private async void OnDeleteClick(PinModel pinModel)
        {
            var result = await UserDialogs.ConfirmAsync(new ConfirmConfig()
                .SetTitle(AppResources.ConfirmationTitle).SetOkText(AppResources.Yes).SetCancelText(AppResources.No));

            if (result)
            {
                await PinService.DeletePinAsync(pinModel);
                await LoadList();
            }
        }

        private async void OnTextChanged()
        {
            await LoadList();
        }

        private async void OnEditClick(PinModel pinModel)
        {
            var parametеrs = new NavigationParameters
            {
                { nameof(PinModel), pinModel }
            };

            await NavigationService.NavigateAsync(nameof(AddPinPage), parametеrs);
        }

        private async void OnAddButtonClick()
        {
            await NavigationService.NavigateAsync(nameof(AddPinPage));
        }

        private async Task LoadList()
        {
            List<PinModel> pins = await PinService.GetPinsAsync(SearchBarText);
            PlacesList = new ObservableCollection<PinModel>(pins);
        }

        private async void OnLogOutClick()
        {
            AuthorizationService.LogOut();
            await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(SignInPage)}");
        }

        private async void OnPinClick(PinModel model)
        {
            if (double.TryParse(model.Latitude, out double latitude)
                && double.TryParse(model.Longitude, out double longitude))
            {
                var MapCameraPosition = new CameraPosition(new Position(latitude, longitude), 10d);

                var parameters = new NavigationParameters{
                { nameof(MapCameraPosition), MapCameraPosition },
                { nameof(SelectedPin), SelectedPin }
            };

                await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(MainTabbedPage)}" +
                    $"?{nameof(KnownNavigationParameters.SelectedTab)}={nameof(MainMapPage)}", parameters);
            }
        }

        #endregion
    }
}

