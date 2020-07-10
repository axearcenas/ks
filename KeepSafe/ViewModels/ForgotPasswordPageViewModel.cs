using System;
using System.Threading.Tasks;
using KeepSafe.Enum;
using KeepSafe.Extensions;
using KeepSafe.Helpers;
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
#if DEBUG == false
                    string forgotPasswordRootURL = $"{Constants.ROOT_URL}{(UserType == UserType.User ? Constants.USER_URL : Constants.BUSINESS_URL)}{Constants.PASSWORDS_URL}";
#endif
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
                                PopupHelper.ShowLoading();
#if DEBUG
                                fileReader.SetDelegate(this);
                                await fileReader.CreateDummyResponse(JsonConvert.SerializeObject(new
                                {
                                    status = 200
                                }), cts.Token, 0);
#else
                            string content = JsonConvert.SerializeObject(new
                            {
                                email = EmailAddressEntry.Text
                            });
                            restServices.SetDelegate(this);
                            await restServices.PostRequestAsync($"{forgotPasswordRootURL}{Constants.FORGOT_PASSWORDS_URL}", content, cts.Token, 0);
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
                                PopupHelper.ShowLoading();
#if DEBUG
                                fileReader.SetDelegate(this);
                                await fileReader.CreateDummyResponse(JsonConvert.SerializeObject(new
                                {
                                    status = 200
                                }), cts.Token, 1);
#else
                            string content = JsonConvert.SerializeObject(new
                            {
                                email = EmailAddressEntry.Text,
                                verification_code = CodeEntry.Text
                            });
                            restServices.SetDelegate(this);
                            await restServices.PostRequestAsync($"{forgotPasswordRootURL}{Constants.VERIFY_CODE_URL}", content, cts.Token, 1);
#endif
                            }
                            break;
                        case ForgotPasswordType.ChangePassword:
                            if (PasswordEntry.ValidateIsTextNullOrEmpty("Password is Required") ||
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
                                PopupHelper.ShowLoading();
#if DEBUG
                                fileReader.SetDelegate(this);
                                await fileReader.CreateDummyResponse(JsonConvert.SerializeObject(new
                                {
                                    message = "Successfully Change Password",
                                    status = 200
                                }), cts.Token, 2);
#else
                            string content = JsonConvert.SerializeObject(new
                            {
                                email = EmailAddressEntry.Text,
                                password = PasswordEntry.Text,
                                confirm_password = ConfirmPasswordEntry.Text
                            });   
                            restServices.SetDelegate(this);
                            await restServices.PostRequestAsync($"{forgotPasswordRootURL}{Constants.CHANGE_PASSWORD_URL}", content, cts.Token, 2);
#endif
                            }
                            break;
                    }

                }
                catch (OperationCanceledException ox) { App.Log($"StackTrace: {ox.StackTrace}\nMESSAGE: {ox.Message}"); IsClicked = false; IsLoading = false; PopupHelper.RemoveLoading(); }
                catch (TimeoutException te) { App.Log($"StackTrace: {te.StackTrace}\nMESSAGE: {te.Message}"); IsClicked = false; IsLoading = false; PopupHelper.RemoveLoading(); }
                catch (Exception ex) { App.Log($"StackTrace: {ex.StackTrace}\nMESSAGE: {ex.Message}"); IsClicked = false; IsLoading = false; PopupHelper.RemoveLoading(); }
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
                    case 0: // verify Email
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            PopupHelper.RemoveLoading();
                            PageType = ForgotPasswordType.VerifyEmail;
                        });
                        break;
                    case 1: // verify Email Verification Code
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            PopupHelper.RemoveLoading();
                            PageType = ForgotPasswordType.ChangePassword;
                        });
                        break;
                    case 2: // Change Password
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            PopupHelper.RemoveLoading();
                            await Task.Delay(50);
                            if (jsonData.ContainsKey("message"))
                                await PageDialogService?.DisplayAlertAsync("Change Password Success", jsonData["message"].ToString(), "Okay");
                            await NavigationService.GoBackAsync();
                        });
                        break;
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    PopupHelper.RemoveLoading();
                    if (jsonData.ContainsKey("error"))
                        await PageDialogService?.DisplayAlertAsync("Change Password Error", jsonData["message"].ToString(), "Okay");
                });
            }
            PopupHelper.RemoveLoading();
            IsClicked = false;
        }

        public void ReceiveError(string title, string error, int wsType)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                PopupHelper.RemoveLoading();
                PageDialogService?.DisplayAlertAsync(title, error, "Okay");
                IsClicked = false;
            });
        }
    }

    public enum ForgotPasswordType
    {
        SendEmailVerification = 0,
        VerifyEmail = 1,
        ChangePassword = 2
    }
}
