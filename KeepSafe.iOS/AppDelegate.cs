using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Prism;
using Prism.Ioc;
using UIKit;

namespace KeepSafe.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {

            //Xamarin.Calabash.Start() should not be enabled when creating archive to be uploaded on appstore
#if !RELEASE
            Xamarin.Calabash.Start();
#endif
            #region Scale Helper Init
            App.ScreenWidth = (float)UIScreen.MainScreen.Bounds.Width;
            App.ScreenHeight = (float)UIScreen.MainScreen.Bounds.Height;

            App.OriginalHeight = App.ScreenHeight;
            App.OriginalWidth = App.ScreenWidth;
            App.AppScale = (float)UIScreen.MainScreen.Scale;
            App.DeviceType = 0;
            App.StatusBarHeight = (float)UIApplication.SharedApplication.StatusBarFrame.Size.Height;

            App.TopInsets = 0.0f;
            App.BottomInsets = 0.0f;

            global::Xamarin.Forms.Forms.Init();

            //Check if we are running on a phone
            switch (UIKit.UIDevice.CurrentDevice.UserInterfaceIdiom)
            {
                case UIUserInterfaceIdiom.Pad:
                    App.IsPhone = false;
                    App.ScreenWidth = (9 * App.ScreenHeight) / 16;
                    App.AdjustedWidth = App.ScreenWidth;
                    break;
                default:
                    App.IsPhone = true;
                    App.ScreenHeight = (16 * App.ScreenWidth) / 9;
                    App.AdjustedHeight = App.ScreenHeight;
                    break;
            }

            UIWindow w = new UIWindow();
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                if (w.SafeAreaInsets.Top > 0 && w.SafeAreaInsets.Bottom > 0)
                {
                    App.IsAddNavHeight = true;
                    App.TopInsets = (float)w.SafeAreaInsets.Top;
                    App.BottomInsets = (float)w.SafeAreaInsets.Bottom;
                }
            }

            App.SystemVersion = UIDevice.CurrentDevice.SystemVersion;
            #endregion

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            var ignore = typeof(FFImageLoading.Svg.Forms.SvgCachedImage);

            //Syncfusion.XForms.iOS.Border.SfBorderRenderer.Init();
            Rg.Plugins.Popup.Popup.Init();

            LoadApplication(new App(new iOSInitializer()));

            //FirebasePushNotificationManager.Initialize(options, true);
            //ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            //SfEffectsViewRenderer.Init();


            App.Log("Width: " + App.ScreenWidth.ToString() + " Height: " + App.ScreenHeight.ToString());

            PrintFonts();

            return base.FinishedLaunching(app, options);
        }

        public void PrintFonts()
        {
            //foreach (var familyNames in UIFont.FamilyNames.Where(f => f.StartsWith("Font Awesome")).OrderBy(c => c).ToList())
            //{
            //    Console.WriteLine(" * " + familyNames);
            //    foreach (var familyName in UIFont.FontNamesForFamilyName(familyNames).OrderBy(c => c).ToList())
            //    {
            //        Console.WriteLine(" *-- " + familyName);
            //    }
            //}
        }

        public override void WillEnterForeground(UIApplication application)
        {
            base.WillEnterForeground(application);
        }

        public override void DidEnterBackground(UIApplication uiApplication)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
            //FirebasePushNotificationManager.Disconnect();
            base.DidEnterBackground(uiApplication);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            //FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            //FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);
        }

        // To receive notifications in foregroung on iOS 9 and below.
        // To receive notifications in background in any iOS version
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired 'till the user taps on the notification launching the application.

            // If you disable method swizzling, you'll need to call this method. 
            // This lets FCM track message delivery and analytics, which is performed
            // automatically with method swizzling enabled.
            //FirebasePushNotificationManager.DidReceiveMessage(userInfo);

            // Do your magic to handle the notification data
            System.Console.WriteLine(userInfo);
        }

        //[Export("messaging:didReceiveMessage:")]
        //public void DidReceiveMessage(Firebase.CloudMessaging.Messaging messaging, Firebase.CloudMessaging.RemoteMessage remoteMessage)
        //{
        //    Console.WriteLine(remoteMessage.AppData);
        //    FirebasePushNotificationManager.DidReceiveMessage(remoteMessage.AppData);
        //}

        public override void OnActivated(UIApplication uiApplication)
        {
            //FirebasePushNotificationManager.Connect();
            //Plugin.FacebookClient.FacebookClientManager.OnActivated();
        }
    }

    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
        }
    }
}
