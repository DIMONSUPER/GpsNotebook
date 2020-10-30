using System.Threading.Tasks;
using Acr.UserDialogs;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace GpsNotebook.Services.Permissions
{
    public class PermissionsService : IPermissionsService
    {
		private readonly IUserDialogs UserDialogs;

        public PermissionsService(IUserDialogs userDialogs)
        {
			UserDialogs = userDialogs;
        }

        public async Task<bool> GetPermissionAsync(Permission permission)
        {
			bool result = false;

			try
			{
				PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();
				if (status != PermissionStatus.Granted)
				{
					if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
					{
						await UserDialogs.AlertAsync("Need location", "Gunna need that location", "OK");
					}

					status = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
				}

				result = status == PermissionStatus.Granted;
			}
			catch
			{
			}

			return result;
		}
    }
}
