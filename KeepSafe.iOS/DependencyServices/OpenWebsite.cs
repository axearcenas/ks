using System;
using System.Threading.Tasks;
using Foundation;
using KeepSafe.DependencyServices;
using KeepSafe.iOS.DependencyServices;
using SafariServices;
using UIKit;
using WebKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(OpenWebsite))]
namespace KeepSafe.iOS.DependencyServices
{
    public class OpenWebsite : IOpenWebsite
    {
        public Task OpenUrl(string uri)
        {
            var sfViewController = new SFSafariViewController(new NSUrl(uri));
            
            
            UIWindow uIWindow = UIApplication.SharedApplication.KeyWindow;
            if (uIWindow != null && uIWindow.RootViewController != null)
            {
                return uIWindow.RootViewController.PresentViewControllerAsync(sfViewController, true);
            }

            return Task.Delay(16);
        }
    }
}
