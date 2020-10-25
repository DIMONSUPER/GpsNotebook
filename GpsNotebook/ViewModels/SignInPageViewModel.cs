using System.Windows.Input;
using Acr.UserDialogs;
using GpsNotebook.Helpers;
using GpsNotebook.Models;
using GpsNotebook.Resources;
using GpsNotebook.Services;
using GpsNotebook.Views;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace GpsNotebook.ViewModels
{
    public class SignInPageViewModel : ViewModelBase
    {
        public ICommand SignInClickCommand => new Command(SignInClick);
        public ICommand SignUpClickCommand => new Command(SignUpClick);

        private IRepositoryService RepositoryService { get; }
        private IUserDialogs UserDialogs { get; }
        public SignInPageViewModel(
            INavigationService navigationService,
            IRepositoryService repositoryService,
            IUserDialogs userDialogs)
            :base(navigationService)
        {
            RepositoryService = repositoryService;
            UserDialogs = userDialogs;
            RepositoryService.InitTable<UserModel>();
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(nameof(UserEmail), out string userEmail))
            {
                UserEmail = userEmail;
            }
            else
            { UserEmail = string.Empty; }
        }

        private string userEmail;
        public string UserEmail
        {
            get { return userEmail; }
            set
            {
                SetProperty(ref userEmail, value);
            }
        }

        private string userPassword;
        public string UserPassword
        {
            get { return userPassword; }
            set
            {
                SetProperty(ref userPassword, value);
            }
        }

        private async void SignInClick()
        {
            var myquery = await RepositoryService.GetAsync<UserModel>(u => u.Email.Equals(UserEmail) && u.Password.Equals(UserPassword));

            if (myquery != null)
            {
                Settings.RememberedEmail = UserEmail;
                Settings.RememberedUserId = myquery.Id;
                await NavigationService.NavigateAsync(
                    $"/{nameof(NavigationPage)}" +
                    $"/{nameof(MainTabbedPage)}" +
                    $"?{KnownNavigationParameters.SelectedTab}" +
                    $"={nameof(MainMapPage)}");
            }
            else
            {
                await UserDialogs.AlertAsync(AppResources.InvalidLogin, AppResources.InvalidLogin, AppResources.OK);
                Settings.RememberedEmail = string.Empty;
                Settings.RememberedUserId = 0;
                UserPassword = string.Empty;
            }
        }

        private async void SignUpClick()
        {
            await NavigationService.NavigateAsync(nameof(SignUpPage));
        }
    }
}

