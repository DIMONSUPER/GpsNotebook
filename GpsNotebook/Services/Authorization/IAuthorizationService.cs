using System.Threading.Tasks;
using GpsNotebook.Models;

namespace GpsNotebook.Services.Authorization
{
    public interface IAuthorizationService
    {
        Task<UserModel> SignInAsync(string email, string password);

        void LogOut();

        Task<bool> SignUpAsync(UserModel user);
    }
}
