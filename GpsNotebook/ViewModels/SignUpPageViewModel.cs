using Acr.UserDialogs;
using GpsNotebook.Models;
using GpsNotebook.Resources;
using GpsNotebook.Services.Authorization;
using Prism.Navigation;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;

namespace GpsNotebook.ViewModels
{
    public class SignUpPageViewModel : ViewModelBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserDialogs _userDialogs;

        public SignUpPageViewModel(
            INavigationService navigationService,
            IAuthorizationService authorizationService,
            IUserDialogs userDialogs)
            : base(navigationService)
        {
            _authorizationService = authorizationService;
            _userDialogs = userDialogs;
        }

        #region -- Public properties --

        public ICommand SignUpClickCommand => new Command(SignUpClick);

        private string _userEmail;
        public string UserEmail
        {
            get { return _userEmail; }
            set { SetProperty(ref _userEmail, value); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        private string _userPassword;
        public string UserPassword
        {
            get { return _userPassword; }
            set { SetProperty(ref _userPassword, value); }
        }

        private string _confirmUserPassword;
        public string ConfirmUserPassword
        {
            get { return _confirmUserPassword; }
            set { SetProperty(ref _confirmUserPassword, value); }
        }

        #endregion

        #region -- Private helpers --

        private async void SignUpClick()
        {
            string message = string.Empty;

            if (ValidateEmail(UserEmail, ref message))
            {
                if (string.IsNullOrEmpty(_userName))
                {
                    await _userDialogs.AlertAsync(AppResources.NameInvalid, AppResources.NameInvalid, AppResources.OK);
                }
                else if (ValidatePassword(UserPassword, ref message))
                {
                    if (UserPassword != ConfirmUserPassword)
                    {
                        await _userDialogs.AlertAsync(AppResources.PasswordsMatch, AppResources.PasswordsMatch, AppResources.OK);
                    }
                    else
                    {
                        bool result = await _authorizationService.SignUpAsync(new UserModel { Email = UserEmail.ToUpper(), Name = UserName, Password = UserPassword });

                        if (result)
                        {
                            await _userDialogs.AlertAsync(AppResources.RegistrationSuccessfull, AppResources.RegistrationSuccessfullTitle, AppResources.OK);

                            var parameters = new NavigationParameters { { nameof(UserEmail), UserEmail } };

                            await NavigationService.GoBackAsync(parameters);
                        }
                        else
                        {
                            await _userDialogs.AlertAsync(AppResources.RegistrationFail, AppResources.RegistrationFailTitle, AppResources.OK);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                await _userDialogs.AlertAsync(message, AppResources.RegistrationFailTitle, AppResources.OK);
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

        #endregion
    }
}
