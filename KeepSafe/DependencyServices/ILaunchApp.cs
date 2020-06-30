using System;
using System.Threading.Tasks;

namespace KeepSafe
{
    public interface ILaunchApp
    {
        Task<bool> LaunchApp(string uri);
        bool OpenPermissionSettings();
    }
}
