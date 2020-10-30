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
        private readonly IAuthorizationService AuthorizationService;
        private readonly IUserDialogs UserDialogs;

        public SignInPageViewModel(
            INavigationService navigationService,
            IAuthorizationService authorizationService,
            IUserDialogs userDialogs)
            : base(navigationService)
        {
            AuthorizationService = authorizationService;
            UserDialogs = userDialogs;
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

        private string userPassword;
        public string UserPassword
        {
            get { return userPassword; }
            set { SetProperty(ref userPassword, value); }
        }

        #endregion

        #region -- Private helpers --

        private async void OnSignInClick()
        {
            var user = await AuthorizationService.SignInAsync(UserEmail.ToUpper(), UserPassword);

            if (user != null)
            {
                await NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(MainTabbedPage)}");
            }
            else
            {
                await UserDialogs.AlertAsync(AppResources.InvalidLogin, AppResources.InvalidLogin, AppResources.OK);
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

