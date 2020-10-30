using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using GpsNotebook.Helpers;
using GpsNotebook.Models;
using GpsNotebook.Services.Repository;

namespace GpsNotebook.Services.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IRepositoryService RepositoryService;
        private readonly IUserDialogs UserDialogs;

        public AuthorizationService(
            IRepositoryService repositoryService,
            IUserDialogs userDialogs)
        {
            RepositoryService = repositoryService;
            UserDialogs = userDialogs;
            RepositoryService.InitTable<UserModel>();
        }

        #region -- IAuthorizationService implementation --

        public async Task<UserModel> SignInAsync(string email, string password)
        {
            UserModel result = null;

            try
            {
                result = await RepositoryService.GetAsync<UserModel>(u =>
                u.Email.Equals(email.ToUpper()) && u.Password.Equals(password));

                Settings.RememberedEmail = email;
                Settings.RememberedUserId = result.Id;
            }
            catch (Exception ex)
            {
                await UserDialogs.AlertAsync(ex.Message);
            }

            return result;
        }

        public void LogOut()
        {
            Settings.RememberedEmail = string.Empty;
            Settings.RememberedUserId = -1;
        }

        public async Task<bool> SignUpAsync(UserModel user)
        {
            bool success = false;

            try
            {
                user.Email = user.Email.ToUpper();
                int result = await RepositoryService.InsertAsync(user);
                if (result != -1)
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                await UserDialogs.AlertAsync(ex.Message);
            }

            return success;
        }

        #endregion
    }
}
