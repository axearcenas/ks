using System;
using System.Threading;
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
    public class RegisterBusinessViewModel : ViewModelBase, IRestReceiver, IFileConnector
    {
        public DelegateCommand BackButtonClickedCommand { get; set; }
        public DelegateCommand UploadPhotoClickedCommand { get; set; }
        public DelegateCommand<object> SelectedIndexChangedCommand { get; set; }
        public DelegateCommand<object> CheckBoxClickedCommand { get; set; }
        public DelegateCommand EULALabelTappedCommand { get; set; }
        public DelegateCommand RegisterButtonClickedCommand { get; set; }
        public DelegateCommand LoginLabelTappedCommand { get; set; }

        public EntryViewModel BusinessNameEntry { get; } = new EntryViewModel() { Placeholder = "Business Name", PlaceholderColor = ColorResource.MAIN_BLACK_COLOR };
        public EntryViewModel MobileNumberEntry { get; } = new EntryViewModel() { Placeholder = "Mobile #", PlaceholderColor = ColorResource.MAIN_BLACK_COLOR };
        public EntryViewModel ContactPersonEntry { get; } = new EntryViewModel() { Placeholder = "Contact Person", PlaceholderColor = ColorResource.MAIN_BLACK_COLOR };
        public EntryViewModel AddressEntry { get; } = new EntryViewModel() { Placeholder = "Address", PlaceholderColor = ColorResource.MAIN_BLACK_COLOR };
        public EntryViewModel EmailAddressEntry { get; } = new EntryViewModel() { Placeholder = "Email", PlaceholderColor = ColorResource.MAIN_BLACK_COLOR };
        public EntryViewModel PasswordEntry { get; } = new EntryViewModel() { Placeholder = "Password", PlaceholderColor = ColorResource.MAIN_BLACK_COLOR, IsPassword = true };

        //TODO Remove this tihs
        string _EstablishmentImage;
        public string EstablishmentImage
        {
            get { return _EstablishmentImage; }
            set { SetProperty(ref _EstablishmentImage, value, nameof(EstablishmentImage)); }
        }

        string _EstablishmentTypeSelectedItem;
        public string EstablishmentTypeSelectedItem
        {
            get { return _EstablishmentTypeSelectedItem; }
            set { SetProperty(ref _EstablishmentTypeSelectedItem, value, nameof(EstablishmentTypeSelectedItem)); }
        }

        Color _UploadPhotoTextColor = Color.FromHex("#05257A");
        public Color UploadPhotoTextColor
        {
            get { return _UploadPhotoTextColor; }
            set { SetProperty(ref _UploadPhotoTextColor, value, nameof(UploadPhotoTextColor)); }
        }

        Color _EstablishmentTypeTextColor = ColorResource.MAIN_BLACK_COLOR;
        public Color EstablishmentTypeTextColor
        {
            get { return _EstablishmentTypeTextColor; }
            set
            {
                SetProperty(ref _EstablishmentTypeTextColor, value, nameof(EstablishmentTypeTextColor));
                if (_EstablishmentTypeSelectedItem != null)
                    EstablishmentTypeTextColor = Color.White;
            }
        }

        Color _EULATextColor = ColorResource.MAIN_BLACK_COLOR;
        public Color EULATextColor
        {
            get { return _EULATextColor; }
            set { SetProperty(ref _EULATextColor, value, nameof(EULATextColor)); }
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

        UserType _UserType = UserType.Establishment;
        public UserType UserType
        {
            get { return _UserType; }
            set { SetProperty(ref _UserType, value, nameof(UserType)); }
        }

        MediaHelper mediaHelper = new MediaHelper();
        MediaFile file;

        public RegisterBusinessViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            BackButtonClickedCommand = new DelegateCommand(OnBackButtonClicked);
            UploadPhotoClickedCommand = new DelegateCommand(OnUploadPhotoClicked);
            SelectedIndexChangedCommand = new DelegateCommand<object>(SelectedIndexChanged);
            CheckBoxClickedCommand = new DelegateCommand<object>(OnCheckboxClicked);
            EULALabelTappedCommand = new DelegateCommand(OnEULALabelTapped);
            RegisterButtonClickedCommand = new DelegateCommand(OnRegisterButtonClicked);
            LoginLabelTappedCommand = new DelegateCommand(OnLoginLabelTapped);
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
                            Directory = "KeepSafe",
                            Name = $"{DateTime.UtcNow}.jpg",
                            CompressionQuality = 92,
                            MaxWidthHeight = (int)400.ScaleHeight(),
                            DefaultCamera = CameraDevice.Front,
                            PhotoSize = PhotoSize.Custom
                        });
                        if (file != null)
                        {
                            EstablishmentImage = file.Path;
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
                            EstablishmentImage = file.Path;
                        }
                    }
                    else if (action.ToString() == "Remove Photo")
                    {
                        if (file != null)
                        {
                            file = null;
                            EstablishmentImage = null;
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
                EULATextColor = ColorResource.MAIN_BLACK_COLOR;

            (sender as Button).Text = IsChecked ? "check" : "";
        }

        private void OnEULALabelTapped()
        {
            //TODO Navigate to EULA Page
        }

        private async void OnRegisterButtonClicked()
        {
            bool IsValid = true;

            if (file == null)
            {
                UploadPhotoTextColor = Color.Red;
                IsValid = false;
            }

            if (EstablishmentTypeSelectedItem == null)
            {
                EstablishmentTypeTextColor = Color.Red;
                IsValid = false;
            }

            if (BusinessNameEntry.ValidateIsTextNullOrEmpty("Business Name is required"))
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

            if (ContactPersonEntry.ValidateIsTextNullOrEmpty("Contact Person is required"))
            {
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

                if (cts != null)
                    cts.Cancel();
                cts = new CancellationTokenSource();
                PopupHelper.ShowLoading();
                try
                {
#if DEBUG
                    fileReader.SetDelegate(this);
                    await fileReader.ReadFile("BusinessData.json", cts.Token, 0);
#else
                        restServices.SetDelegate(this);
                        string content = JsonConvert.SerializeObject(new { current_password = PasswordEntry.Text, new_password = NewPasswordEntry.Text });
                        await restServices.PostRequestAsync($"{Constants.ROOT_API_URL}".AddAuth(), content, cts.Token, 0);
#endif

                }

                catch (OperationCanceledException ox) { App.Log($"StackTrace: {ox.StackTrace}\nMESSAGE: {ox.Message}"); IsClicked = false; IsLoading = false; }
                catch (TimeoutException te) { App.Log($"StackTrace: {te.StackTrace}\nMESSAGE: {te.Message}"); IsClicked = false; IsLoading = false; }
                catch (Exception ex) { App.Log($"StackTrace: {ex.StackTrace}\nMESSAGE: {ex.Message}"); IsClicked = false; IsLoading = false; }
                cts = null;
            }
        }

        private void OnLoginLabelTapped()
        {
            NavigationService.NavigateAsync("/MainPage");
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
                                dataClass.Business = JsonConvert.DeserializeObject<Business>(jsonData["data"].ToString());
                                dataClass.LoginType = UserType.Establishment;
                                await App.ShowHomePage(dataClass.LoginType);
                            });
                        }
                        break;
                }
            }
            IsClicked = false;
            PopupHelper.RemoveLoading();
        }

        public void ReceiveError(string title, string error, int wsType)
        {
            PageDialogService?.DisplayAlertAsync(title, error, "Okay");
            IsClicked = false;
            PopupHelper.RemoveLoading();
        }
    }
}
