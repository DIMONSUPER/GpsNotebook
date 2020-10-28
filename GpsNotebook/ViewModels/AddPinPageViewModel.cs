using Acr.UserDialogs;
using GpsNotebook.Helpers;
using GpsNotebook.Models;
using GpsNotebook.Resources;
using GpsNotebook.Services;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GpsNotebook.ViewModels
{
    public class AddPinPageViewModel : ViewModelBase
    {
        public ICommand MapClickedCommand => new Command<MapClickedEventArgs>(MapClick);
        public ICommand SelectedPinChangedCommand => new Command<SelectedPinChangedEventArgs>(SelectedPinChanged);
        public ICommand SaveClickedCommand => new Command(SaveClick);

        private IRepositoryService RepositoryService { get; }
        private IUserDialogs UserDialogs { get; }
        public AddPinPageViewModel(
            INavigationService navigationService,
            IRepositoryService repositoryService,
            IUserDialogs userDialogs)
            : base(navigationService)
        {
            RepositoryService = repositoryService;
            UserDialogs = userDialogs;
            RepositoryService.InitTable<PinModel>();
            Pins = new ObservableCollection<Pin>();
            SelectedPin = new Pin { };
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(nameof(PinModel), out PinModel pinModel))
            {
                Pin = pinModel;
                SelectedPin.Position = pinModel.Position;
                SelectedPin.Label = pinModel.Label;
                Pins?.Add(SelectedPin);
                IsFavourite = pinModel.IsFavoirite;
                Address = pinModel.Address;
                MapCameraPosition = new CameraPosition(pinModel.Position, 10d);
            }
            else if (parameters.TryGetValue(nameof(MapCameraPosition), out CameraPosition mapCameraPosition))
            {
                Pin = new PinModel();
                MapCameraPosition = mapCameraPosition;
            }
        }

        private bool isFavourite;
        public bool IsFavourite
        {
            get { return isFavourite; }
            set { SetProperty(ref isFavourite, value); }
        }

        private CameraPosition mapCameraPosition;
        public CameraPosition MapCameraPosition
        {
            get { return mapCameraPosition; }
            set { SetProperty(ref mapCameraPosition, value); }
        }

        public ObservableCollection<Pin> Pins { get; set; }

        private PinModel pin;
        public PinModel Pin
        {
            get { return pin; }
            set
            {
                SetProperty(ref pin, value);
            }
        }

        private Pin selected_pin;
        public Pin SelectedPin
        {
            get { return selected_pin; }
            set
            {
                SetProperty(ref selected_pin, value);
            }
        }

        private string address;
        public string Address
        {
            get { return address; }
            set { SetProperty(ref address, value); }
        }

        private void SelectedPinChanged(SelectedPinChangedEventArgs args)
        {
            if (args.SelectedPin != null)
            {
                SelectedPin.Label = args.SelectedPin.Label;
                SelectedPin.Position = args.SelectedPin.Position;
            }
        }

        private void MapClick(MapClickedEventArgs args)
        {
            if (string.IsNullOrEmpty(SelectedPin.Label))
            {
                SelectedPin.Label = "NewPin";
            }
            SelectedPin.Position = args.Point;

            Pins.Clear();
            Pins?.Add(SelectedPin);
        }

        private async void SaveClick()
        {
            if (!string.IsNullOrEmpty(SelectedPin.Label)
                && !string.IsNullOrEmpty(SelectedPin.Position.Latitude.ToString())
                && !string.IsNullOrEmpty(SelectedPin.Position.Longitude.ToString()))
            {
                int result = await RepositoryService.InsertAsync(new PinModel
                {
                    Id = Pin.Id,
                    Label = SelectedPin.Label,
                    Position = SelectedPin.Position,
                    IsFavoirite = IsFavourite,
                    Address = Address,
                    UserId = Settings.RememberedUserId
                });

                if (result != -1)
                {
                    await NavigationService.GoBackAsync();
                }
            }
            else
            {
                UserDialogs.Alert(AppResources.FieldsFilledError);
            }
        }
    }
}
