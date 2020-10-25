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
        }
        public ObservableCollection<Pin> Pins { get; set; }

        private Pin pin;
        public Pin Pin
        {
            get { return pin; }
            set { SetProperty(ref pin, value); }
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
                Pin.Label = args.SelectedPin.Label;
                Pin.Position = args.SelectedPin.Position;
            }
        }

        private void MapClick(MapClickedEventArgs args)
        {
            Pin = new Pin
            {
                Label = $"MyPin",
                Position = args.Point
            };

            Pins.Clear();
            Pins.Add(Pin);
        }

        private async void SaveClick()
        {
            if (!string.IsNullOrEmpty(Pin.Label)
                || !string.IsNullOrEmpty(Pin.Position.Latitude.ToString())
                || !string.IsNullOrEmpty(Pin.Position.Longitude.ToString()))
            {
                int result = await RepositoryService.InsertAsync(new PinModel
                {
                    Label = Pin.Label,
                    Position = Pin.Position,
                    Address = this.Address,
                    UserId = Settings.RememberedUserId
                });

                if (result != -1)
                {
                    await NavigationService.GoBackAsync();
                }
            }
            else
            {
                UserDialogs.Alert(AppResources.NameInvalid);
            }
        }
    }
}
