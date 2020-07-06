using System;
using System.Threading.Tasks;
using KeepSafe.Extension;
using KeepSafe.Extensions;
using KeepSafe.Helpers.FileReader;
using KeepSafe.Models;
using KeepSafe.Resources;
using KeepSafe.ViewModels.ViewViewModels;
using KeepSafe.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KeepSafe.ViewModels
{
    public class LoginPageViewModel : ViewModelBase, IFileConnector, IRestReceiver
    {
        public EntryViewModel EmailAddressEntry { get; } = new EntryViewModel() { Placeholder = "Mobile No.", PlaceholderColor = ColorResource.MAIN_BLACK_COLOR };
        public EntryViewModel PasswordEntry { get; } = new EntryViewModel() { Placeholder = "Password", PlaceholderColor = ColorResource.MAIN_BLACK_COLOR, IsPassword = true};

        public DelegateCommand BackCommand { get; set; }
        public DelegateCommand ForotPasswordCommand { get; set; }
        public DelegateCommand LoginCommand { get; set; }
        public DelegateCommand RegisterCommand { get; set; }
        public DelegateCommand<object> EntryFocusedCommand { get; set; }
        public DelegateCommand ShowPasswordCommand { get; set; }
        public DelegateCommand LoginFacebookCommand { get; set; }
        public DelegateCommand LoginGoogleCommand { get; set; }

        bool _IsPasswordButtonVisible = false;
        public bool IsPasswordButtonVisible
        {
            get { return _IsPasswordButtonVisible; }
            set { _IsPasswordButtonVisible = value; OnPropertyChanged(); }
        }

        UserType _UserType = UserType.User;
        public UserType UserType
        {
            get { return _UserType; }
            set { SetProperty(ref _UserType, value, nameof(UserType)); }
        }

        public LoginPageViewModel(INavigationService navigationService,IPageDialogService pageDialogService)
            :base (navigationService, pageDialogService)
        {
            BackCommand = new DelegateCommand(OnBackCommand_Execute);
            ForotPasswordCommand = new DelegateCommand(OnForotPasswordCommand_Execute);
            LoginCommand = new DelegateCommand(OnLoginCommand_Execute);
            RegisterCommand = new DelegateCommand(OnRegisterCommandCommand_Execute);
            EntryFocusedCommand = new DelegateCommand<object>(OnEntryFocusedCommand_Execute);
            ShowPasswordCommand = new DelegateCommand(OnShowPasswordCommand_Execute);
            LoginFacebookCommand = new DelegateCommand(OnLoginFacebookCommand_Execute);
            LoginGoogleCommand = new DelegateCommand(OnLoginGoogleCommand_Execute);
            PasswordEntry.PropertyChanged += PasswordEntry_PropertyChanged;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if(parameters.ContainsKey("UserType"))
            {
                UserType = (UserType)parameters["UserType"];
            }
        }

        private void PasswordEntry_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals("Text"))
            {
                IsPasswordButtonVisible = !string.IsNullOrEmpty(PasswordEntry.Text);
            }
        }

        private async void OnBackCommand_Execute()
        {
            if (!IsClicked)
            {
                IsClicked = true;
                await NavigationService.GoBackAsync();
                IsClicked = false;
            }
        }

        private void OnEntryFocusedCommand_Execute(object obj)
        {
            if(obj is string stringValue )
            {
                switch(stringValue)
                {
                    case "0"://Email Address Entry
                        EmailAddressEntry.ToDefaultValue();
                        break;
                    case "1":
                        PasswordEntry.ToDefaultValue();
                        break;
                }
            }
        }

        private void OnForotPasswordCommand_Execute()
        {
            //TODO Forgot Password Page
            if(!IsClicked)
            {
                IsClicked = true;
                INavigationParameters keys = new NavigationParameters();
                keys.Add("UserType", UserType);
                NavigationService.NavigateAsync(nameof(ForgotPasswordPage));
                IsClicked = false;
            }
        }

        private void OnShowPasswordCommand_Execute()
        {
            PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        }

        private void OnLoginCommand_Execute()
        {
            bool IsNotError = true;
            
            if(EmailAddressEntry.ValidateIsTextNullOrEmpty("Mobile number is required"))
            {
                IsNotError = false;
            }
            else if (!("+63" + EmailAddressEntry.Text).IsValidPhoneNumber())
            {
                EmailAddressEntry.ShowError("Not a valid mobile number");
                IsNotError = false;
            }

            if (PasswordEntry.ValidateIsTextNullOrEmpty("Password is required"))
            {
                IsNotError = false;
            }

            if(IsNotError && !IsClicked)
            {
                IsClicked = true;

                cts = new System.Threading.CancellationTokenSource();
                Task.Run( async() =>
               {                   
                   try
                   {
#if DEBUG
                       switch (UserType)
                       {
                           case UserType.User:
                               fileReader.SetDelegate(this);
                               await fileReader.ReadFile("UserData.json", cts.Token, 0);
#else
                               //TODO Login Rest Here
                               string content = JsonConvert.SerializeObject(new { email_address = EmailAddressEntry.Text, password = PasswordEntry.Text, device = App.DeviceType });
                               restServices.SetDelegate(this);
                               await restServices.PostRequestAsync($"{Constants.ROOT_URL}", content, cts.Token,0);
#endif
                               break;
                           case UserType.Establishment:

#if DEBUG
                               fileReader.SetDelegate(this);
                               await fileReader.ReadFile("UserData.json", cts.Token, 0);
#else
                               //TODO Login Rest Here
                               string content = JsonConvert.SerializeObject(new { email_address = EmailAddressEntry.Text, password = PasswordEntry.Text, device = App.DeviceType });
                               restServices.SetDelegate(this);
                               await restServices.PostRequestAsync($"{Constants.ROOT_URL}", content, cts.Token,0);
#endif
                                break;                               
                       }
                       
                   }
                   catch (OperationCanceledException ox) { App.Log($"StackTrace: {ox.StackTrace}\nMESSAGE: {ox.Message}"); IsClicked = false; IsLoading = false; }
                   catch (TimeoutException te) { App.Log($"StackTrace: {te.StackTrace}\nMESSAGE: {te.Message}"); IsClicked = false; IsLoading = false; }
                   catch (Exception ex) { App.Log($"StackTrace: {ex.StackTrace}\nMESSAGE: {ex.Message}"); IsClicked = false; IsLoading = false; }
                   cts = null;
               });
            }
        }

        private async void OnRegisterCommandCommand_Execute()
        {
            if(!IsClicked)
            {
                IsClicked = true;
                await NavigationService.NavigateAsync(UserType == UserType.User ? nameof(RegisterUserPage) : nameof(RegisterBusinessPage));
                IsClicked = false;
            }
        }

        private void OnLoginFacebookCommand_Execute()
        {
            //TODO Implement facebook OAuth
            //Xamarin.Essentials.WebAuthenticator.AuthenticateAsync();
        }

        private void OnLoginGoogleCommand_Execute()
        {
            //TODO Implement google OAuth
        }

        public void ReceiveJSONData(JObject jsonData, int wsType)
        {
            if(jsonData.ContainsKey("status") && jsonData["status"].ToObject<int>() == 200)
            {
                switch (wsType)
                {
                    case 0:
                        switch (UserType)
                        {
                            case UserType.User:
                                if (jsonData.ContainsKey("user"))
                                {
                                    Device.BeginInvokeOnMainThread(async () =>
                                    {
                                    //TODO save USER Here
                                    //PageDialogService?.DisplayAlertAsync("Login Succesfully",jsonData["message"].ToString(),"Okay");
                                    dataClass.User = JsonConvert.DeserializeObject<User>(jsonData["user"].ToString());
                                        dataClass.IsLoggedIn = true;
                                        App.ShowHomePage(UserType.User);
                                        EmailAddressEntry.ClearText();
                                        PasswordEntry.ClearText();
                                    });
                                }
                            break;
                            case UserType.Establishment:
                                if (jsonData.ContainsKey("user"))
                                {
                                    Device.BeginInvokeOnMainThread(async () =>
                                    {
                                        //TODO save USER Here
                                        //PageDialogService?.DisplayAlertAsync("Login Succesfully",jsonData["message"].ToString(),"Okay");
                                        dataClass.User = JsonConvert.DeserializeObject<User>(jsonData["user"].ToString());
                                        dataClass.IsLoggedIn = true;
                                        App.ShowHomePage(UserType.Establishment);
                                        EmailAddressEntry.ClearText();
                                        PasswordEntry.ClearText();
                                    });
                                }
                                break;
                        }                
                    break;
                }
            }
            IsClicked = false;
        }

        public void ReceiveError(string title, string error, int wsType)
        {
            PageDialogService?.DisplayAlertAsync(title, error, "Okay");
            IsClicked = false;
        }
    }
}
