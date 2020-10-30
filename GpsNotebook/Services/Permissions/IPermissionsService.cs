using System.Threading.Tasks;
using Plugin.Permissions.Abstractions;

namespace GpsNotebook.Services.Permissions
{
    public interface IPermissionsService
    {
        Task<bool> GetPermissionAsync(Permission permission);
    }
}
