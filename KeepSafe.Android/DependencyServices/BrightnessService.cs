using Android.Content;
using Android.Views;
using KeepSafe.Droid;
//using Plugin.CurrentActivity;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(BrightnessService))]
namespace KeepSafe.Droid
{
    public class BrightnessService : IBrightnessService
    {
        static float LastBrightness = 0.5f;
        static bool IsBrightnessAdjusted = false;

        public void ResetBrightness()
        {
            if (IsBrightnessAdjusted)
            {

                //var window = CrossCurrentActivity.Current.Activity.Window;
                var window = Xamarin.Essentials.Platform.CurrentActivity.Window;
                var attributesWindow = new WindowManagerLayoutParams();
                attributesWindow.CopyFrom(window.Attributes);
                attributesWindow.ScreenBrightness = LastBrightness;
                window.Attributes = attributesWindow;
                IsBrightnessAdjusted = false;
            }
        }

        public void SetBrightness(float brightness)
        {
            //var window = CrossCurrentActivity.Current.Activity.Window;
            var window = Xamarin.Essentials.Platform.CurrentActivity.Window;
            var attributesWindow = new WindowManagerLayoutParams();

            attributesWindow.CopyFrom(window.Attributes);
            LastBrightness = attributesWindow.ScreenBrightness;
            attributesWindow.ScreenBrightness = brightness;
            window.Attributes = attributesWindow;
            IsBrightnessAdjusted = true;
        }

        Context GetContext()
        {
            var context = Xamarin.Essentials.Platform.CurrentActivity ?? Xamarin.Essentials.Platform.AppContext;
            if (context == null)
            {
                App.Log("Unable to detect current Activity or App Context. Please ensure Xamarin.Essentials is installed in your Android project initialized.");
            }

            return context;
        }
    }
}
