using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Content;

namespace KeepSafe.Droid
{
    [Activity(Label = "KeepSafe", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, ViewTreeObserver.IOnGlobalLayoutListener
    {
        public static Action GlobalLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.SetTheme(Resource.Style.MainTheme);

            #region Scale Helper

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat) // Extend layout beyond statusbar and navigationbar
            {
                var newUiOptions = 0;

                newUiOptions |= (int)SystemUiFlags.LayoutStable;
                newUiOptions |= (int)SystemUiFlags.LayoutFullscreen;

                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;
            }

            if (Build.VERSION.SdkInt == BuildVersionCodes.Lollipop) // Don't set statusbar to transparent since we can't set statusbar text color
            {
                Window.SetStatusBarColor(Color.Black);
            }
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                SetWindowFlag(WindowManagerFlags.TranslucentStatus, false);
                Window.SetStatusBarColor(Color.Transparent);
            }

            var density = Resources.DisplayMetrics.Density;
            App.ScreenWidth = Resources.DisplayMetrics.WidthPixels / density;
            App.ScreenHeight = Resources.DisplayMetrics.HeightPixels / density;
            App.AppScale = density;
            App.DeviceType = 1;
            int resourceId = Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
            {
                App.StatusBarHeight = Resources.GetDimensionPixelSize(resourceId) / density;
            }

            Point realSize = new Point();
            Point usableSize = new Point();
            GetScreenSize(ref realSize, ref usableSize);

            App.ScreenHeight = realSize.Y / density;
            App.ScreenWidth = realSize.X / density;
            App.OriginalHeight = App.ScreenHeight;
            App.OriginalWidth = App.ScreenWidth;

            // Work around for devices with tall displays
            // Check if app is running on phone
            if (Xamarin.Forms.Device.Idiom == Xamarin.Forms.TargetIdiom.Phone)
            {
                App.ScreenHeight = (16 * App.ScreenWidth) / 9;
                App.AdjustedHeight = App.ScreenHeight;
                App.IsPhone = true;

                // But wait! If device has navigationbar? and screen ratio is already 16:9
                if ((int)App.OriginalHeight <= (int)App.ScreenHeight)
                {
                    App.ScreenHeight = usableSize.Y / density;
                    App.ScreenWidth = (9 * App.ScreenHeight) / 16;
                    App.AdjustedHeight = App.ScreenHeight;
                }
            }
            else if (Xamarin.Forms.Device.Idiom == Xamarin.Forms.TargetIdiom.Tablet)
            {
                App.ScreenWidth = (9 * App.ScreenHeight) / 16;
                App.AdjustedWidth = App.ScreenWidth;
                App.IsPhone = false;
            }

            App.SystemVersion = Build.VERSION.Sdk;
#endregion

            base.OnCreate(savedInstanceState);

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            var ignore = typeof(FFImageLoading.Svg.Forms.SvgCachedImage);

            global::Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            App.Log("Width: " + App.ScreenWidth.ToString() + " Height: " + App.ScreenHeight.ToString());

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void SetWindowFlag(WindowManagerFlags bits, bool on)
        {
            var windows = Window;
            var attrib = windows.Attributes;
            if (on)
                attrib.Flags |= bits;
            else
                attrib.Flags &= ~bits;

            windows.Attributes = attrib;
        }

        // Android JellyBeanMr1 and Up Only API 17
        public void GetScreenSize(ref Point realSize, ref Point usableSize)
        {
            IWindowManager windowManager = Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            Display display = windowManager.DefaultDisplay;

            display.GetRealSize(realSize);
            display.GetSize(usableSize);
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }

        public override void SetContentView(View view)
        {
            base.SetContentView(view);
            view.ViewTreeObserver.AddOnGlobalLayoutListener(this);
        }

        public void OnGlobalLayout()
        {
            GlobalLayout?.Invoke();
        }
    }
}