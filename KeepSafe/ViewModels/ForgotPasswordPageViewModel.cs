using System;
using KeepSafe.Extensions;
using KeepSafe.Helpers.FileReader;
using KeepSafe.Models;
using KeepSafe.Resources;
using KeepSafe.ViewModels.ViewViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using Prism.Navigation.Xaml;
using Prism.Services;
using Xamarin.Forms;

namespace KeepSafe.ViewModels
{
    public class ForgotPasswordPageViewModel : ViewModelBase , IFileConnector, IRestReceiver
    {
        public EntryViewModel EmailAddressEntry { get; } = new EntryViewModel() { Placeholder = "Email Address", PlaceholderColor = ColorResource.MAIN_BLACK_COLOR , IsVisible = true };
        public EntryViewModel CodeEntry { get; } = new EntryViewModel() { Placeholder = "Verification Code", PlaceholderColor = ColorResource.MAIN_BLACK_COLOR, IsVisible = false };
        public EntryViewModel PasswordEntry { get; } = new EntryViewModel() { Placeholder = "New Password", PlaceholderColor = ColorResource.MAIN_BLACK_COLOR, IsVisible = false , IsPassword = true };
        public EntryViewModel ConfirmPasswordEntry { get; } = new EntryViewModel() { Placeholder = "Confirm Password", PlaceholderColor = ColorResource.MAIN_BLACK_COLOR, IsVisible = false, IsPassword = true };

        public DelegateCommand BackCommand { get; set; }
        public DelegateCommand ForgotPasswordButtonCommand { get; set; }

        ForgotPasswordType _PageType;
        public ForgotPasswordType PageType
        {
            get { return _PageType; }
            set { SetProperty(ref _PageType, value, nameof(PageType));
                RaisePropertyChanged(nameof(PageMessage));
                RaisePropertyChanged(nameof(PageButton));
                EmailAddressEntry.IsVisible = _PageType == ForgotPasswordType.SendEmailVerification;
                CodeEntry.IsVisible = _PageType == ForgotPasswordType.VerifyEmail;
                PasswordEntry.IsVisible = _PageType == ForgotPasswordType.ChangePassword;
            }
        }
        public string PageMessage { get { return PageType == ForgotPasswordType.SendEmailVerification ? "Please enter email you used\nat the time of registration" : PageType == ForgotPasswordType.VerifyEmail ? "Enter Verification Code you\nreceived from your email" : "Change Password"; } }
        public string PageButton { get { return PageType == ForgotPasswordType.SendEmailVerification ? "Verify" : PageType == ForgotPasswordType.VerifyEmail ? "Reset" : "Change"; } }

        UserType _UserType = UserType.User;
        public UserType UserType
        {
            get { return _UserType; }
            set { SetProperty(ref _UserType, value, nameof(UserType)); }
        }

        public ForgotPasswordPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService):
            base(navigationService, pageDialogService)
        {
            BackCommand = new DelegateCommand(OnBackCommand_Execute);
            ForgotPasswordButtonCommand = new DelegateCommand(OnForgotPasswordButtonCommand_Execute);
        }

        private async void OnForgotPasswordButtonCommand_Execute()
        {
            //TODO API HERE
            if (!IsClicked)
            {
                IsClicked = true;
                cts = new System.Threading.CancellationTokenSource();
                try
                {
                    switch (UserType)
                    {
                        case UserType.User:
                            switch (PageType)
                            {
                                case ForgotPasswordType.SendEmailVerification:
                                    if (!EmailAddressEntry.Text.IsValidEmail())
                                    {
                                        EmailAddressEntry.ShowError("Email address is required");
                                        IsClicked = false;
                                    }
                                    else
                                    { 
#if DEBUG
                                        fileReader.SetDelegate(this);
                                        await fileReader.CreateDummyResponse(JsonConvert.SerializeObject(new
                                        {
                                            status = 200
                                        }), cts.Token, 0);
#else
                                   //TODO Verify Email API Here
                                   string content = JsonConvert.SerializeObject(new { email_address = EmailAddressEntry.Text, password = PasswordEntry.Text, device = App.DeviceType });
                                   restServices.SetDelegate(this);
                                   await restServices.PostRequestAsync($"{Constants.ROOT_URL}", content, cts.Token,0);
#endif
                                    }
                                    break;
                                case ForgotPasswordType.VerifyEmail:
                                    if (CodeEntry.ValidateIsTextNullOrEmpty("Verification code is Required"))
                                    {
                                        IsClicked = false;
                                    }
                                    else
                                    {
#if DEBUG
                                        fileReader.SetDelegate(this);
                                        await fileReader.CreateDummyResponse(JsonConvert.SerializeObject(new
                                        {
                                            status = 200
                                        }), cts.Token, 1);
#else
                                   //TODO Verify Email API Here
                                   string content = JsonConvert.SerializeObject(new { email_address = EmailAddressEntry.Text, password = PasswordEntry.Text, device = App.DeviceType });
                                   restServices.SetDelegate(this);
                                   await restServices.PostRequestAsync($"{Constants.ROOT_URL}", content, cts.Token,0);
#endif
                                    }
                                    break;
                                case ForgotPasswordType.ChangePassword:
                                    if (PasswordEntry.ValidateIsTextNullOrEmpty("Password is Required") &&
                                        ConfirmPasswordEntry.ValidateIsTextNullOrEmpty("Confirm password code is Required"))
                                    {
                                        IsClicked = false;
                                    }
                                    else if (!PasswordEntry.Text.Equals(ConfirmPasswordEntry.Text))
                                    {
                                        ConfirmPasswordEntry.ShowError("Confirm Password Mismatch");
                                        IsClicked = false;
                                    }
                                    else
                                    {
#if DEBUG
                                        fileReader.SetDelegate(this);
                                        await fileReader.CreateDummyResponse(JsonConvert.SerializeObject(new
                                        {
                                            message = "Successfully Change Password",
                                            status = 200
                                        }), cts.Token, 2);
#else
                                   //TODO Verify Email API Here
                                   string content = JsonConvert.SerializeObject(new { email_address = EmailAddressEntry.Text, password = PasswordEntry.Text, device = App.DeviceType });
                                   restServices.SetDelegate(this);
                                   await restServices.PostRequestAsync($"{Constants.ROOT_URL}", content, cts.Token,0);
#endif
                                    }
                                    break;
                            }
                            break;
                        case UserType.Establishment:
                            switch (PageType)
                            {
                                case ForgotPasswordType.SendEmailVerification:
                                    if (!EmailAddressEntry.Text.IsValidEmail())
                                    {
                                        EmailAddressEntry.ShowError("Email address is required");
                                        IsClicked = false;
                                    }
                                    else
                                    {
#if DEBUG
                                        fileReader.SetDelegate(this);
                                        await fileReader.CreateDummyResponse(JsonConvert.SerializeObject(new
                                        {
                                            status = 200
                                        }), cts.Token, 0);
#else
                                   //TODO Verify Email API Here
                                   string content = JsonConvert.SerializeObject(new { email_address = EmailAddressEntry.Text, password = PasswordEntry.Text, device = App.DeviceType });
                                   restServices.SetDelegate(this);
                                   await restServices.PostRequestAsync($"{Constants.ROOT_URL}", content, cts.Token,0);
#endif
                                    }
                                    break;
                                case ForgotPasswordType.VerifyEmail:
                                    if (CodeEntry.ValidateIsTextNullOrEmpty("Verification code is Required"))
                                    {
                                        IsClicked = false;
                                    }
                                    else
                                    {
#if DEBUG
                                        fileReader.SetDelegate(this);
                                        await fileReader.CreateDummyResponse(JsonConvert.SerializeObject(new
                                        {
                                            status = 200
                                        }), cts.Token, 1);
#else
                                   //TODO Verify Email API Here
                                   string content = JsonConvert.SerializeObject(new { email_address = EmailAddressEntry.Text, password = PasswordEntry.Text, device = App.DeviceType });
                                   restServices.SetDelegate(this);
                                   await restServices.PostRequestAsync($"{Constants.ROOT_URL}", content, cts.Token,0);
#endif
                                    }
                                    break;
                                case ForgotPasswordType.ChangePassword:
                                    if (PasswordEntry.ValidateIsTextNullOrEmpty("Password is Required") &&
                                        ConfirmPasswordEntry.ValidateIsTextNullOrEmpty("Confirm password code is Required"))
                                    {
                                        IsClicked = false;
                                    }
                                    else if (!PasswordEntry.Text.Equals(ConfirmPasswordEntry.Text))
                                    {
                                        ConfirmPasswordEntry.ShowError("Confirm Password Mismatch");
                                    }
                                    else
                                    {
#if DEBUG
                                        fileReader.SetDelegate(this);
                                        await fileReader.CreateDummyResponse(JsonConvert.SerializeObject(new
                                        {
                                            message = "Successfully Change Password",
                                            status = 200
                                        }), cts.Token, 2);
#else
                                   //TODO Verify Email API Here
                                   string content = JsonConvert.SerializeObject(new { email_address = EmailAddressEntry.Text, password = PasswordEntry.Text, device = App.DeviceType });
                                   restServices.SetDelegate(this);
                                   await restServices.PostRequestAsync($"{Constants.ROOT_URL}", content, cts.Token,0);
#endif
                                    }
                                    break;
                            }
                            break;
                    }

                }
                catch (OperationCanceledException ox) { App.Log($"StackTrace: {ox.StackTrace}\nMESSAGE: {ox.Message}"); IsClicked = false; IsLoading = false; }
                catch (TimeoutException te) { App.Log($"StackTrace: {te.StackTrace}\nMESSAGE: {te.Message}"); IsClicked = false; IsLoading = false; }
                catch (Exception ex) { App.Log($"StackTrace: {ex.StackTrace}\nMESSAGE: {ex.Message}"); IsClicked = false; IsLoading = false; }
                cts = null;
            }
        }

        private void OnBackCommand_Execute()
        {
            switch (PageType)
            {
                case ForgotPasswordType.SendEmailVerification:
                    NavigationService.GoBackAsync();
                    break;
                case ForgotPasswordType.VerifyEmail:
                    PageType = ForgotPasswordType.SendEmailVerification;
                    break;
                case ForgotPasswordType.ChangePassword:
                    PageType = ForgotPasswordType.VerifyEmail;
                    break;
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);
            if(parameters.ContainsKey("UserType"))
            {
                UserType = (UserType)parameters["UserType"];
            }
        }

        public void ReceiveJSONData(JObject jsonData, int wsType)
        {
            if (jsonData.ContainsKey("status") && jsonData["status"].ToObject<int>() == 200)
            {
                switch (wsType)
                {
                     //switch (PageType)
                     //   {
                     //       case ForgotPasswordType.SendEmailVerification:
                     //           PageType = ForgotPasswordType.VerifyEmail;
                     //           break;
                     //       case ForgotPasswordType.VerifyEmail:
                     //           PageType = ForgotPasswordType.ChangePassword;
                     //           break;
                     //       case ForgotPasswordType.ChangePassword:
                     //           break;
                     //   }
                    case 0: // verify Email
                        switch (UserType)
                        {
                            case UserType.User:
                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    //TODO save USER Here
                                    EmailAddressEntry.ClearText();
                                    PageType = ForgotPasswordType.VerifyEmail;
                                });
                                break;
                            case UserType.Establishment:
                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    //TODO save USER Here
                                    EmailAddressEntry.ClearText();
                                    PageType = ForgotPasswordType.VerifyEmail;
                                });
                                break;
                        }
                        break;
                    case 1: // verify Email Verification Code
                        switch (UserType)
                        {
                            case UserType.User:
                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    //TODO save USER Here
                                    EmailAddressEntry.ClearText();
                                    PageType = ForgotPasswordType.ChangePassword;
                                });
                                break;
                            case UserType.Establishment:
                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    //TODO save USER Here
                                    EmailAddressEntry.ClearText();
                                    PageType = ForgotPasswordType.ChangePassword;
                                });
                                break;
                        }
                        break;
                    case 2: // Change Password
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            //TODO save USER Here
                            if(jsonData.ContainsKey("message"))
                                PageDialogService?.DisplayAlertAsync("Change Password Success", jsonData["message"].ToString(), "Okay");
                            await NavigationService.GoBackAsync();
                        });
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

    public enum ForgotPasswordType
    {
        SendEmailVerification = 0,
        VerifyEmail = 1,
        ChangePassword = 2
    }
}
