using Acr.UserDialogs;
using GpsNotebook.Models;
using GpsNotebook.Resources;
using GpsNotebook.Services.Location;
using GpsNotebook.Services.Pin;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GpsNotebook.ViewModels
{
    public class AddPinPageViewModel : ViewModelBase
    {
        private readonly IPinService _pinService;
        private readonly IUserDialogs _userDialogs;
        private readonly ILocationService _locationService;

        public AddPinPageViewModel(
            INavigationService navigationService,
            IPinService pinService,
            IUserDialogs userDialogs,
            ILocationService locationService)
            : base(navigationService)
        {
            _pinService = pinService;
            _userDialogs = userDialogs;
            _locationService = locationService;
        }

        #region -- Public properties --

        public ICommand MapClickedCommand => new Command<Position>(MapClick);
        public ICommand SaveClickedCommand => new Command(SaveClick);

        private string _label;
        public string Label
        {
            get { return _label; }
            set { SetProperty(ref _label, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private bool _isFavourite = true;
        public bool IsFavourite
        {
            get { return _isFavourite; }
            set { SetProperty(ref _isFavourite, value); }
        }

        private double _latitude;
        public double Latitude
        {
            get { return _latitude; }
            set { SetProperty(ref _latitude, value); }
        }

        private double _longitude;
        public double Longitude
        {
            get { return _longitude; }
            set { SetProperty(ref _longitude, value); }
        }

        private CameraPosition _mapCameraPosition;
        public CameraPosition MapCameraPosition
        {
            get { return _mapCameraPosition; }
            set { SetProperty(ref _mapCameraPosition, value); }
        }

        private ObservableCollection<PinModel> _pins;
        public ObservableCollection<PinModel> Pins
        {
            get { return _pins; }
            set { SetProperty(ref _pins, value); }
        }

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (Pins == null)
            {
                Pins = new ObservableCollection<PinModel>();
            }
            if (parameters.TryGetValue(nameof(PinModel), out PinModel pinModel))
            {
                if (double.TryParse(pinModel.Latitude, out double latitude)
                    && double.TryParse(pinModel.Longitude, out double longitude))
                {
                    Latitude = latitude;
                    Longitude = longitude;
                }

                pinId = pinModel.Id;
                Label = pinModel.Label;
                Description = pinModel.Description;
                IsFavourite = pinModel.IsFavourite;

                Pins.Add(pinModel);

                MapCameraPosition = new CameraPosition(new Position(Latitude, Longitude), 10d);
            }
            else
            {
                MapCameraPosition = _locationService.GetCameraLocation();
            }
        }

        #endregion

        #region -- Private helpers --
        private int pinId;

        private void MapClick(Position point)
        {
            if (string.IsNullOrEmpty(Label))
            {
                Label = "NewPin";
            }
            Latitude = point.Latitude;
            Longitude = point.Longitude;
            Pins.Clear();

            var newPin = new PinModel
            {
                Label = Label,
                Latitude = Latitude.ToString(),
                Longitude = Longitude.ToString(),
            };

            Pins.Add(newPin);
        }

        private async Task UpdateProfile()
        {
            PinModel pinModel = new PinModel
            {
                Id = pinId,
                Label = Label,
                Description = Description,
                IsFavourite = IsFavourite,
                Latitude = Latitude.ToString(),
                Longitude = Longitude.ToString(),
            };

            int result = await _pinService.AddPinAsync(pinModel);

            if (result != -1)
            {
                await NavigationService.GoBackAsync();
            }
        }

        private async void SaveClick()
        {
            if (!string.IsNullOrEmpty(Label)
                && Latitude != 0 && Longitude != 0)
            {
                PinModel pin = await _pinService.GetByLabelAsync(Label);

                if (pinId != 0 || pin == null)
                {
                    await UpdateProfile();
                }
                else
                {
                    var result = await _userDialogs.ConfirmAsync(new ConfirmConfig()
                        .SetTitle(AppResources.LabelError).SetOkText(AppResources.Yes).SetCancelText(AppResources.No));

                    if (result)
                    {
                        pin.Description = Description;
                        pin.IsFavourite = IsFavourite;
                        pin.Latitude = Latitude.ToString();
                        pin.Longitude = Longitude.ToString();

                        await _pinService.AddPinAsync(pin);
                        await NavigationService.GoBackAsync();
                    }
                }
            }
            else
            {
                _userDialogs.Alert(AppResources.FieldsFilledError);
            }
        }

        #endregion
    }
}
