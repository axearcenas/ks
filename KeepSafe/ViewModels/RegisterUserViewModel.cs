using System;
using System.Collections.Generic;
using KeepSafe.Extension;
using KeepSafe.Extensions;
using KeepSafe.Helpers.MediaHelper;
using KeepSafe.Resources;
using KeepSafe.ViewModels;
using KeepSafe.ViewModels.ViewViewModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace KeepSafe
{
    public class RegisterUserViewModel : ViewModelBase
    {
        INavigationService _navigationService;
        IPageDialogService _pageDialogService;

        public DelegateCommand BackButtonClickedCommand { get; set; }
        public DelegateCommand UploadPhotoClickedCommand { get; set; }
        public DelegateCommand<object> SelectedIndexChangedCommand { get; set; }
        public DelegateCommand<object> CheckBoxClickedCommand { get; set; }
        public DelegateCommand EULALabelTappedCommand { get; set; }
        public DelegateCommand RegisterButtonClickedCommand { get; set; }
        public DelegateCommand LoginLabelTappedCommand { get; set; }

        public EntryViewModel BusinessNameEntry { get; } = new EntryViewModel() { Placeholder = "Business Name", PlaceholderColor = ColorResource.WHITE_COLOR };
        public EntryViewModel MobileNumberEntry { get; } = new EntryViewModel() { Placeholder = "Mobile #", PlaceholderColor = ColorResource.WHITE_COLOR };
        public EntryViewModel ContactPersonEntry { get; } = new EntryViewModel() { Placeholder = "Contact Person", PlaceholderColor = ColorResource.WHITE_COLOR };
        public EntryViewModel AddressEntry { get; } = new EntryViewModel() { Placeholder = "Address", PlaceholderColor = ColorResource.WHITE_COLOR };
        public EntryViewModel EmailAddressEntry { get; } = new EntryViewModel() { Placeholder = "Email", PlaceholderColor = ColorResource.WHITE_COLOR };
        public EntryViewModel PasswordEntry { get; } = new EntryViewModel() { Placeholder = "Password", PlaceholderColor = ColorResource.WHITE_COLOR, IsPassword=true };

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

        Color _EstablishmentTypeTextColor = Color.White;
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

        Color _EULATextColor = Color.White;
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

        MediaHelper mediaHelper = new MediaHelper();
        MediaFile file;

        public RegisterUserViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;

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
            _navigationService.GoBackAsync();
        }

        private async void OnUploadPhotoClicked()
        {
            if (!IsClicked)
            {
                IsClicked = true;
                var action = await _pageDialogService.DisplayActionSheetAsync("Upload Photo", "Cancel", file == null ? null : "Remove Photo", "Camera", "Gallery");
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
            }
        }

        private void OnLoginLabelTapped()
        {
            _navigationService.NavigateAsync("/MainPage");
        }
    }
}
