using System.Windows.Input;
using Acr.UserDialogs;
using GpsNotebook.Resources;
using GpsNotebook.Services.Authorization;
using GpsNotebook.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace GpsNotebook.ViewModels
{
    public class SignInPageViewModel : ViewModelBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserDialogs _userDialogs;

        public SignInPageViewModel(
            INavigationService navigationService,
            IAuthorizationService authorizationService,
            IUserDialogs userDialogs)
            : base(navigationService)
        {
            _authorizationService = authorizationService;
            _userDialogs = userDialogs;
        }

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(nameof(UserEmail), out string userEmail))
            {
                UserEmail = userEmail;
            }
        }

        #endregion

        #region -- Public properties --

        public ICommand SignInClickCommand => new Command(OnSignInClick);
        public ICommand SignUpClickCommand => new Command(OnSignUpClick);

        private string _userEmail;
        public string UserEmail
        {
            get { return _userEmail; }
            set { SetProperty(ref _userEmail, value); }
        }

        private string _userPassword;
        public string UserPassword
        {
            get { return _userPassword; }
            set { SetProperty(ref _userPassword, value); }
        }

        #endregion

        #region -- Private helpers --

        private async void OnSignInClick()
        {
            var user = await _authorizationService.SignInAsync(UserEmail.ToUpper(), UserPassword);

            if (user != null)
            {
                await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(MainTabbedPage)}");
            }
            else
            {
                await _userDialogs.AlertAsync(AppResources.InvalidLogin, AppResources.InvalidLogin, AppResources.OK);
                UserPassword = string.Empty;
            }
        }

        private async void OnSignUpClick()
        {
            await NavigationService.NavigateAsync(nameof(SignUpPage));
        }

        #endregion
    }
}

