using System;
using System.Threading.Tasks;
using Android.Content;
using KeepSafe.DependencyServices;
using KeepSafe.Droid.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(LaunchApp))]
namespace KeepSafe.Droid.DependencyServices
{
    public class LaunchApp : ILaunchApp
    {
        Task<bool> ILaunchApp.LaunchApp(string uri)
        {
            bool result;
            try
            {
                var aUri = Android.Net.Uri.Parse(uri);
                var intent = new Intent(Intent.ActionView, aUri);
                Xamarin.Forms.Forms.Context.StartActivity(intent);
                result = true;
            }
            catch (ActivityNotFoundException)
            {
                result = false;
            }

            return Task.FromResult(result);
        }

        bool ILaunchApp.OpenPermissionSettings()
        {
            var context = GetContext();
            if (context == null)
                return false;

            try
            {
                var settingsIntent = new Intent();
                settingsIntent.SetAction(Android.Provider.Settings.ActionApplicationDetailsSettings);
                settingsIntent.AddCategory(Intent.CategoryDefault);
                settingsIntent.SetData(Android.Net.Uri.Parse("package:" + context.PackageName));
                settingsIntent.AddFlags(ActivityFlags.NewTask);
                settingsIntent.AddFlags(ActivityFlags.NoHistory);
                settingsIntent.AddFlags(ActivityFlags.ExcludeFromRecents);
                context.StartActivity(settingsIntent);
                return true;
            }
            catch
            {
                return false;
            }
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
