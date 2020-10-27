using Acr.UserDialogs;
using GpsNotebook.Helpers;
using GpsNotebook.Models;
using GpsNotebook.Resources;
using GpsNotebook.Services;
using GpsNotebook.Views;
using Prism.Navigation;
using Prism.Services;
using System.Collections.ObjectModel;
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

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            RefreshList();
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
                RefreshList();
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
            var parameters = new NavigationParameters
            {
                { nameof(MapCameraPosition), MapCameraPosition }
            };

            await NavigationService.NavigateAsync(nameof(AddPinPage), parameters);
        }

        private async void RefreshList()
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
            MapCameraPosition = new CameraPosition(model.Position, 10d);
            var parameters = new NavigationParameters
            {
                { nameof(MapCameraPosition), MapCameraPosition }
            };

            await NavigationService.NavigateAsync(
                $"{nameof(MainTabbedPage)}" +
                $"?{nameof(KnownNavigationParameters.SelectedTab)}" +
                $"={nameof(MainMapPage)}", parameters);
        }
    }
}

