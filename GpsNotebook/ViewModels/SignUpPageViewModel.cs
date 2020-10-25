using Acr.UserDialogs;
using GpsNotebook.Models;
using GpsNotebook.Resources;
using GpsNotebook.Services;
using Prism.Navigation;
using Prism.Services;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;

namespace GpsNotebook.ViewModels
{
    public class SignUpPageViewModel : ViewModelBase
    {
        public ICommand SignUpClickCommand => new Command(SignUpClick);

        private IRepositoryService RepositoryService { get; }
        private IUserDialogs UserDialogs { get; }
        public SignUpPageViewModel(
            INavigationService navigationService,
            IRepositoryService repositoryService,
            IUserDialogs userDialogs)
            : base(navigationService)
        {
            RepositoryService = repositoryService;
            UserDialogs = userDialogs;
        }

        private string userEmail;
        public string UserEmail
        {
            get { return userEmail; }
            set { SetProperty(ref userEmail, value); }
        }

        private string userName;
        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName, value); }
        }

        private string userPassword;
        public string UserPassword
        {
            get { return userPassword; }
            set { SetProperty(ref userPassword, value); }
        }

        private string confirmUserPassword;
        public string ConfirmUserPassword
        {
            get { return confirmUserPassword; }
            set { SetProperty(ref confirmUserPassword, value); }
        }

        private async void SignUpClick()
        {
            string message = string.Empty;

            if (ValidateEmail(UserEmail, ref message))
            {
                if (string.IsNullOrEmpty(userName)
                    || userName.Length < 6)
                {
                    await UserDialogs.AlertAsync(
                        AppResources.NameInvalid,
                        AppResources.NameInvalid,
                        AppResources.OK);
                }
                else if (UserPassword != ConfirmUserPassword)
                {
                    await UserDialogs.AlertAsync(
                        AppResources.PasswordsMatch,
                        AppResources.PasswordsMatch,
                        AppResources.OK);
                }
                else if (ValidatePassword(UserPassword, ref message))
                {
                    int result = await RepositoryService.InsertAsync(new UserModel { Email = UserEmail, Password = UserPassword });
                    if (result != -1)
                    {
                        await UserDialogs.AlertAsync(
                            AppResources.RegistrationSuccessfull,
                            AppResources.RegistrationSuccessfullTitle,
                            AppResources.OK);

                        var parameters = new NavigationParameters
                        {
                            { nameof(UserEmail), UserEmail }
                        };

                        await NavigationService.GoBackAsync(parameters);
                    }
                    else
                    {
                        await UserDialogs.AlertAsync(
                            AppResources.RegistrationFail,
                            AppResources.RegistrationFailTitle,
                            AppResources.OK);
                    }
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                await UserDialogs.AlertAsync(
                    message,
                    AppResources.RegistrationFailTitle,
                    AppResources.OK);
            }
        }

        private bool ValidateEmail(string email, ref string message)
        {
            bool result = true;

            var valideEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,})+)$");
            if (string.IsNullOrEmpty(email)
                || !valideEmail.IsMatch(email))
            {
                message += $"\n{AppResources.EmailInvalid}";

                result = false;
            }

            return result;
        }

        private bool ValidatePassword(string pass, ref string message)
        {
            bool result = true;

            var hasMinChars = new Regex(@"^.{8,}");
            if (string.IsNullOrEmpty(pass)
                || !hasMinChars.IsMatch(pass))
            {
                message += $"\n{AppResources.PasswordMinChar}";

                result = false;
            }

            var hasNumber = new Regex(@"[0-9]+");
            if (!hasNumber.IsMatch(pass))
            {
                message += $"\n{AppResources.PasswordNumber}";

                result = false;
            }

            var hasLowerChar = new Regex(@"[a-z]+");
            if (!hasLowerChar.IsMatch(pass))
            {
                message += $"\n{AppResources.PasswordLower}";

                result = false;
            }

            var hasUpperChar = new Regex(@"[A-Z]+");
            if (!hasUpperChar.IsMatch(pass))
            {
                message += $"\n{AppResources.PasswordUpper}";

                result = false;
            }

            var hasMaxChars = new Regex(@"^.{1,16}$");
            if (!hasMaxChars.IsMatch(pass))
            {
                message += $"\n{AppResources.PasswordMaxChar}";

                result = false;
            }

            return result;
        }
    }
}
