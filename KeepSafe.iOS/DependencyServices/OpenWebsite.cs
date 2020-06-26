using System;
using System.Threading.Tasks;
using Foundation;
using KeepSafe.DependencyServices;
using KeepSafe.iOS.DependencyServices;
using SafariServices;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(OpenWebsite))]
namespace KeepSafe.iOS.DependencyServices
{
    public class OpenWebsite : IOpenWebsite
    {
        public void OpenPassWebsite(string uri)
        {
            var sfViewController = new SFSafariViewController(new NSUrl(uri));
            UIWindow uIWindow = UIApplication.SharedApplication.KeyWindow;
            if (uIWindow != null && uIWindow.RootViewController != null)
            {
                uIWindow.RootViewController.PresentViewControllerAsync(sfViewController, true);
            }
        }
    }
}
