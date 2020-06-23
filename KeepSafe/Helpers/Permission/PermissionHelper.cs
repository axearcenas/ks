using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace KeepSafe.Helpers.Permission
{
    public class PermissionHelper
    {

        public static async Task<PermissionStatus> CheckAndRequestPermissionAsync<T>(T permission) where T : Permissions.BasePermission
        {
            var status = await permission.CheckStatusAsync();
            if (status != PermissionStatus.Granted)
            {
                status = await permission.RequestAsync();
            }

            return status;
        }
    }
}
