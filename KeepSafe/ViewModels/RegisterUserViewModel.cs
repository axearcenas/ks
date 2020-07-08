using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using KeepSafe.Enum;
using KeepSafe.Extension;
using KeepSafe.Extensions;
using KeepSafe.Helpers;
using KeepSafe.Helpers.FileReader;
using KeepSafe.Helpers.MediaHelper;
using KeepSafe.Models;
using KeepSafe.Resources;
using KeepSafe.ViewModels;
using KeepSafe.ViewModels.ViewViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace KeepSafe
{
    public class RegisterUserViewModel : ViewModelBase , IRestReceiver, IFileConnector
    {
        public DelegateCommand BackButtonClickedCommand { get; set; }
        public DelegateCommand UploadPhotoClickedCommand { get; set; }
        public DelegateCommand<object> DateSelectedCommand { get; set; }
        public DelegateCommand<object> CheckBoxClickedCommand { get; set; }
        public DelegateCommand EULALabelTappedCommand { get; set; }
        public DelegateCommand RegisterButtonClickedCommand { get; set; }
        public DelegateCommand LoginLabelTappedCommand { get; set; }

        public EntryViewModel FirstNameEntry { get; } = new EntryViewModel() { Placeholder = "First Name", PlaceholderColor = ColorResource.WHITE_COLOR };
        public EntryViewModel LastNameEntry { get; } = new EntryViewModel() { Placeholder = "Last Name", PlaceholderColor = ColorResource.WHITE_COLOR };
        public EntryViewModel MobileNumberEntry { get; } = new EntryViewModel() { Placeholder = "Mobile #", PlaceholderColor = ColorResource.WHITE_COLOR };        
        public EntryViewModel AddressEntry { get; } = new EntryViewModel() { Placeholder = "Address", PlaceholderColor = ColorResource.WHITE_COLOR };
        public EntryViewModel BirthdateEntry { get; } = new EntryViewModel() { Placeholder = "Birthdate", PlaceholderColor = ColorResource.WHITE_COLOR };
        public EntryViewModel EmailAddressEntry { get; } = new EntryViewModel() { Placeholder = "Email", PlaceholderColor = ColorResource.WHITE_COLOR };
        public EntryViewModel PasswordEntry { get; } = new EntryViewModel() { Placeholder = "Password", PlaceholderColor = ColorResource.WHITE_COLOR, IsPassword=true };

        //TODO Remove this tihs
        string _UserImage;
        public string UserImage
        {
            get { return _UserImage; }
            set { SetProperty(ref _UserImage, value, nameof(UserImage)); }
        }

        Color _UploadPhotoTextColor = Color.FromHex("#05257A");
        public Color UploadPhotoTextColor
        {
            get { return _UploadPhotoTextColor; }
            set { SetProperty(ref _UploadPhotoTextColor, value, nameof(UploadPhotoTextColor)); }
        }
        
        Color _EULATextColor = Color.White;
        public Color EULATextColor
        {
            get { return _EULATextColor; }
            set { SetProperty(ref _EULATextColor, value, nameof(EULATextColor)); }
        }

        Color _BirthdateTextColor = Color.White;
        public Color BirthdateTextColor
        {
            get { return _BirthdateTextColor; }
            set { SetProperty(ref _BirthdateTextColor, value, nameof(BirthdateTextColor)); }
        }

        DateTime _Birthdate = DateTime.Today;
        public DateTime Birthdate
        {
            get { return _Birthdate; }
            set { SetProperty(ref _Birthdate, value, nameof(Birthdate)); }
        }

        bool _IsValid;
        public bool IsValid
        {
            get { return _IsValid; }
            set { SetProperty(ref _IsValid, value, nameof(IsValid)); }
        }

        bool _IsChecked;
        public bool IsChecked
        {
            get { return _IsChecked; }
            set { SetProperty(ref _IsChecked, value, nameof(IsChecked)); }
        }

        MediaHelper mediaHelper = new MediaHelper();
        MediaFile file;

        public RegisterUserViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            BackButtonClickedCommand = new DelegateCommand(OnBackButtonClicked);
            UploadPhotoClickedCommand = new DelegateCommand(OnUploadPhotoClicked);
            DateSelectedCommand = new DelegateCommand<object>(OnDateSelected);
            CheckBoxClickedCommand = new DelegateCommand<object>(OnCheckboxClicked);
            EULALabelTappedCommand = new DelegateCommand(OnEULALabelTapped);
            RegisterButtonClickedCommand = new DelegateCommand(OnRegisterButtonClicked);
            LoginLabelTappedCommand = new DelegateCommand(OnLoginLabelTapped);
        }

        private void OnDateSelected(object sender)
        {
            if((sender as CustomDatePicker).TextColor != Color.White)
                (sender as CustomDatePicker).TextColor = Color.White;
        }

        private void OnBackButtonClicked()
        {
            NavigationService.GoBackAsync();
        }

        private async void OnUploadPhotoClicked()
        {
            if (!IsClicked)
            {
                IsClicked = true;
                var action = await PageDialogService.DisplayActionSheetAsync("Upload Photo", "Cancel", file == null ? null : "Remove Photo", "Camera", "Gallery");
                if (action != null && !action.ToString().Equals("Cancel"))
                {
                    await CrossMedia.Current.Initialize();
                    if (action.ToString() == "Camera")
                    {
                        file = await mediaHelper.TakePhotoAsync(new StoreCameraMediaOptions()
                        {
                            Directory = "Pass",
                            Name = $"{DateTime.UtcNow}.jpg",
                            CompressionQuality = 92,
                            MaxWidthHeight = (int)400.ScaleHeight(),
                            DefaultCamera = CameraDevice.Front,
                            PhotoSize = PhotoSize.Custom
                        });
                        if (file != null)
                        {
                            UserImage = file.Path;
                        }
                    }
                    else if (action.ToString() == "Gallery")
                    {
                        file = await mediaHelper.PickPhotoAsync(new PickMediaOptions()
                        {
                            MaxWidthHeight = (int)400.ScaleHeight(),
                            CompressionQuality = 92,
                            PhotoSize = PhotoSize.Custom,
                            RotateImage = true
                        });
                        if (file != null)
                        {
                            UserImage = file.Path;
                        }
                    }
                    else if (action.ToString() == "Remove Photo")
                    {
                        if (file != null)
                        {
                            file = null;
                            UserImage = null;
                        }
                    }
                }
                IsClicked = false;
            }
        }

        private void SelectedIndexChanged(object sender)
        {
            App.Log(((sender as Picker).SelectedItem).ToString());
        }

        private void OnCheckboxClicked(object sender)
        {
            IsChecked = !IsChecked;

            if (EULATextColor == Color.Red)
                EULATextColor = Color.White;

            (sender as Button).Text = IsChecked ? "check" : "";
        }

        private void OnEULALabelTapped()
        {
            //TODO Navigate to EULA Page
        }

        private void OnRegisterButtonClicked()
        {
            bool IsValid = true;

            if (file == null)
            {
                UploadPhotoTextColor = Color.Red;
                IsValid = false;
            }

            if (FirstNameEntry.ValidateIsTextNullOrEmpty("First Name is required"))
            {
                IsValid = false;
            }

            if (LastNameEntry.ValidateIsTextNullOrEmpty("Last Name is required"))
            {
                IsValid = false;
            }

            if (MobileNumberEntry.ValidateIsTextNullOrEmpty("Mobile number is required"))
            {
                IsValid = false;
            }
            else if (!(MobileNumberEntry.Text).IsValidPhoneNumber())
            {
                MobileNumberEntry.ShowError("Invalid Mobile Number");
                IsValid = false;
            }

            if (AddressEntry.ValidateIsTextNullOrEmpty("Address is required"))
            {
                IsValid = false;
            }

            if (Birthdate.Date == DateTime.Today)
            {
                BirthdateTextColor = Color.Red;
                IsValid = false;
            }

            if (EmailAddressEntry.ValidateIsTextNullOrEmpty("Email is required"))
            {
                IsValid = false;
            }
            else if (!(EmailAddressEntry.Text).IsValidEmail())
            {
                EmailAddressEntry.ShowError("Invalid Email Address");
                IsValid = false;
            }

            if (PasswordEntry.ValidateIsTextNullOrEmpty("Password is required"))
            {
                IsValid = false;
            }

            if (!IsChecked)
            {
                EULATextColor = Color.Red;
                IsValid = false;
            }

            if (IsValid && !IsClicked)
            {
                IsClicked = true;
                PopupHelper.ShowLoading();
                cts = new System.Threading.CancellationTokenSource();
                Task.Run(async () =>
                {
                    try
                    {
#if DEBUG
                        fileReader.SetDelegate(this);
                        await fileReader.ReadFile("UserData.json", cts.Token, 0);
#else
                        //TODO Login Rest Here
                        string content = JsonConvert.SerializeObject(new
                        {
                            registration = new {
                                first_name = FirstNameEntry.Text,
                                image = file.Path,
                                last_name = LastNameEntry.Text, 
                                contact_number = MobileNumberEntry.Text,
                                address = AddressEntry.Text,
                                birthdate = Birthdate.Date.ToLongDateString(),
                                email = EmailAddressEntry.Text,
                                password =  PasswordEntry.Text
                            }
                        });
                        restServices.SetDelegate(this);
                        //await restServices.PostRequestAsync($"{ Constants.ROOT_URL }{ Constants.USER_URL }{ Constants.REGISTER_URL }", content, cts.Token, 0);
                        Dictionary<string, Stream> images = new Dictionary<string, Stream>() { { "registration[image]", file.GetStreamWithImageRotatedForExternalStorage() } };
                        await restServices.MultiPartFormRequestAsync($"{ Constants.ROOT_URL }{ Constants.USER_URL }{ Constants.REGISTER_URL }", content, images, cts.Token, HttpMethod.Post, 0);
#endif
                    }
                    catch (OperationCanceledException ox) { App.Log($"StackTrace: {ox.StackTrace}\nMESSAGE: {ox.Message}"); IsClicked = false; IsLoading = false; PopupHelper.RemoveLoading(); }
                    catch (TimeoutException te) { App.Log($"StackTrace: {te.StackTrace}\nMESSAGE: {te.Message}"); IsClicked = false; IsLoading = false; PopupHelper.RemoveLoading(); }
                    catch (Exception ex) { App.Log($"StackTrace: {ex.StackTrace}\nMESSAGE: {ex.Message}"); IsClicked = false; IsLoading = false; PopupHelper.RemoveLoading(); }
                    cts = null;
                });
            }
        }

        private void OnLoginLabelTapped()
        {
            NavigationService.GoBackAsync();
        }

        public void ReceiveJSONData(JObject jsonData, int wsType)
        {
            if (jsonData.ContainsKey("status") && jsonData["status"].ToObject<int>() == 200)
            {
                switch (wsType)
                {
                    case 0:
                        if (jsonData.ContainsKey("data"))
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                PopupHelper.RemoveLoading();
                                dataClass.User = JsonConvert.DeserializeObject<User>(jsonData["data"].ToString());

                                dataClass.LoginType = UserType.User;
                                await Task.Delay(500);
                                await App.ShowHomePage(dataClass.LoginType);
                                FirstNameEntry.ClearText();
                                LastNameEntry.ClearText();
                                MobileNumberEntry.ClearText();
                                AddressEntry.ClearText();
                                BirthdateEntry.ClearText();
                                EmailAddressEntry.ClearText();
                                PasswordEntry.ClearText();
                            });
                        }
                        break;
                }
            }
            PopupHelper.RemoveLoading();
            IsClicked = false;
        }

        public void ReceiveError(string title, string error, int wsType)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                PopupHelper.RemoveLoading();
                await PageDialogService?.DisplayAlertAsync(title, error, "Okay");
                IsClicked = false;
            });
        }
    }
}
