using System.Windows.Input;
using GpsNotebook.Models;
using Prism.Navigation;
using Xamarin.Forms;

namespace GpsNotebook.ViewModels
{
    public class PinInfoPageViewModel : ViewModelBase
    {
        public ICommand CloseButtonClickedCommand => new Command(OnCloseButtonClicked);

        public PinInfoPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue("pinModel", out PinModel selectedPin))
            {
                SelectedPin = selectedPin;
            }
        }

        private PinModel selectedPin;
        public PinModel SelectedPin
        {
            get { return selectedPin; }
            set { SetProperty(ref selectedPin, value); }
        }

        private async void OnCloseButtonClicked()
        {
            await NavigationService.GoBackAsync();
        }
    }
}
