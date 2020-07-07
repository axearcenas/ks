using System;
using System.Collections.Generic;
using KeepSafe.ViewModels;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Plugin.Popups;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using KeepSafe.Views;
using Prism.Services;
using Prism.Common;
using KeepSafe.Views.Popups;
using KeepSafe.ViewModels.PopupsViewModel;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;

namespace KeepSafe
{
    public partial class App : PrismApplication
    {
        public static float ScreenWidth { get; set; }
        public static float ScreenHeight { get; set; }
        public static float OriginalHeight { get; set; }
        public static float AdjustedHeight { get; set; }
        public static float OriginalWidth { get; set; }
        public static float AdjustedWidth { get; set; }
        public static float AndroidHeightPixels { get; set; }
        public static float AppScale { get; set; }
        public static int DeviceType { get; set; }
        public static string UDID { get; set; }
        public static bool IsPhone { get; set; }
        public static float StatusBarHeight { get; set; }
        public static float TopInsets { get; set; }
        public static float BottomInsets { get; set; }
        public static string SystemVersion { get; set; }
        public static bool IsAddNavHeight { get; set; }
        public static string SMSHash { get; set; } = string.Empty;

        public static Action OnAppResume { get; set; }
        public static Action OnAppSleep { get; set; }

        public static string AppScheme { get; } = "ks://";
        public static string AppRootRoute { get; } = "keepsafe.ph/";
        public static string AppNavigationRootRoute { get { return $"{AppScheme}{AppRootRoute}"; } }

        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        static INavigationService _NavigationService;
        readonly DataClass dataClass = DataClass.GetInstance;
        protected override void OnInitialized()
        {
            InitializeComponent();
            _NavigationService = NavigationService;
            Xamarin.Forms.Device.SetFlags(new string[] { "Shapes_Experimental" });
            //when using socket
            //WebsocketConnection.Link();

            VersionTracking.Track();
            SetupGlobalOptions();
            //NOTE: Set Server Settings
            Constants.ApplyServerSettings();

            //NOTE: FOR TESTING PAGE to show error initializing page
            //MainPage = new NavigationPage(new UserProfilePage() { BindingContext = new UserProfilePageViewModel(NavigationService, new PageDialogService(new ApplicationProvider())) });
            //MainPage = new CustomServerPopup() { BindingContext = new CustomServerPopupViewModel(NavigationService) };

            if (dataClass.LoginType != UserType.None)
                //TODO Add a parameter that identifies logged in account is either User or Business
                ShowHomePage(UserType.User);
            else
                ShowMainPage();
        }

        public static void Log(string msg, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            msg = DateTime.Now.ToString("HH:mm:ss:ff tt") + " [KeepSafe]-[" + memberName + "]: " + msg;
#if DEBUG
            System.Diagnostics.Debug.WriteLine(msg);
#elif RELEASE

#else
            Console.WriteLine(msg);
#endif
        }

        public void SetupGlobalOptions()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() }
            };
        }

        protected override void OnStart()
        {
#if RELEASE
            AppCenter.Start(Constants.APP_CENTER_KEY, typeof(Analytics), typeof(Crashes));
            Crashes.GetErrorAttachments += Crashes_GetErrorAttachments;
#endif
        }

        protected override void OnSleep()
        {
            OnAppSleep?.Invoke();
        }

        protected override void OnResume()
        {
            OnAppResume?.Invoke();
        }

        public static void ShowHomePage(UserType userType)
        {
            //TODO Create Landing Page
            var parameter = new NavigationParameters();
            parameter.Add("UserType", userType);
            _NavigationService.NavigateAsync($"{AppNavigationRootRoute}{nameof(MyTabbedPage)}?{KnownNavigationParameters.CreateTab}={(userType == UserType.User ? "HomePage" : "DashboardPage")}&{KnownNavigationParameters.CreateTab}=ScanPage&{KnownNavigationParameters.CreateTab}=UserProfilePage", parameter);
        }

        public static void ShowMainPage()
        {
            _NavigationService.NavigateAsync($"{AppNavigationRootRoute}NavigationPage/{nameof(Views.MainPage)}");
        }

        public async static void Logout()
        {
            //TODO Create a link to Page when Logout / the Main Page
            ShowMainPage(); //NOTE:Temporary MainPage
            await DataClass.GetInstance.Logout();
        }

        async void DeleteCachedData()
        {
            await dataClass.Logout();
        }

        private IEnumerable<ErrorAttachmentLog> Crashes_GetErrorAttachments(ErrorReport report)
        {
            var UserId = DataClass.GetInstance.User.Id;
            var IsLoggedIn = string.IsNullOrEmpty(DataClass.GetInstance.Token) == false;

            return new ErrorAttachmentLog[]
            {
                    ErrorAttachmentLog.AttachmentWithText("UserId: " + UserId.ToString(), "txt.txt"),
                    ErrorAttachmentLog.AttachmentWithText("IsLoggedIn: " + IsLoggedIn.ToString(), "txt.txt"),
                    ErrorAttachmentLog.AttachmentWithText("URL: " + Constants.URL, "txt.txt")
            };

            return null;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterPopupNavigationService();
            containerRegistry.RegisterPopupDialogService();
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<CustomServerPopup,CustomServerPopupViewModel>();
            containerRegistry.RegisterForNavigation<ScanHistoryFilter, ScanHistoryFilterViewModel>();
            containerRegistry.RegisterForNavigation<MyTabbedPage, MyTabbedPageViewModel>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<RegisterPage, RegisterViewModel>();
            containerRegistry.RegisterForNavigation<RegisterUserPage,RegisterUserViewModel>();
            containerRegistry.RegisterForNavigation<RegisterBusinessPage, RegisterBusinessViewModel>();
            containerRegistry.RegisterForNavigation<MyTabbedPage, MyTabbedPageViewModel>();
            containerRegistry.RegisterForNavigation<DashboardPage, DashboardViewModel>();
            containerRegistry.RegisterForNavigation<HomePage>();
            containerRegistry.RegisterForNavigation<ScanPage>();
            containerRegistry.RegisterForNavigation<UserProfilePage,UserProfilePageViewModel>();
            containerRegistry.RegisterForNavigation<UserCheckInPage>();
            containerRegistry.RegisterForNavigation<MyQRPage>();
            containerRegistry.RegisterForNavigation<ForgotPasswordPage, ForgotPasswordPageViewModel>();

            //NOTE: Views that has a view model in ViewModel Folder
            //containerRegistry.RegisterForNavigation<GetStartedPage>();
            //NOTE: Views that has a view model but not in ViewModel Folder
            //containerRegistry.RegisterForNavigation<CustomServerPopup, CustomServerPopupViewModel>();
        }
    }
}
