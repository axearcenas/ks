using System;
using KeepSafe.iOS.DependencyServices;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(ChangeBarColor))]
namespace KeepSafe.iOS.DependencyServices
{
    public class ChangeBarColor : IChangeBarColor
    {
        public void ChangeColor(BarStyle barStyle)
        {
            switch (barStyle)
            {
                case BarStyle.Light:
                    (UIApplication.SharedApplication).SetStatusBarStyle(UIStatusBarStyle.LightContent, false);
                    break;
                case BarStyle.Dark:
                    (UIApplication.SharedApplication).SetStatusBarStyle(UIStatusBarStyle.Default, false);
                    break;
            }
        }
    }
}
