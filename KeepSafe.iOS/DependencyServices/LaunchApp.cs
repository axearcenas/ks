using System;
using System.Threading.Tasks;
using Foundation;
using KeepSafe.DependencyServices;
using KeepSafe.iOS.DependencyServices;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(LaunchApp))]
namespace KeepSafe.iOS.DependencyServices
{
    public class LaunchApp : ILaunchApp
    {
        Task<bool> ILaunchApp.LaunchApp(string uri)
        {
            var canOpen = UIApplication.SharedApplication.CanOpenUrl(new NSUrl(uri));

            if (!canOpen) return Task.FromResult(false);

            return Task.FromResult(UIApplication.SharedApplication.OpenUrl(new NSUrl(uri)));
        }

        bool ILaunchApp.OpenPermissionSettings()
        {
            //Opening settings only open in iOS 8+
            if (!UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                return false;

            try
            {
                UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
