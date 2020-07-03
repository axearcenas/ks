using System;
using KeepSafe.Extensions;
using KeepSafe.Helpers.MediaHelper;
using KeepSafe.Models;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace KeepSafe.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {
        public DelegateCommand BackButtonClickedCommand { get; set; }
        public DelegateCommand UploadPhotoClickedCommand { get; set; }
        public DelegateCommand<object> DateSelectedCommand { get; set; }
        public DelegateCommand LoginLabelTappedCommand { get; set; }
        public DelegateCommand ChangePasswordTappedCommand { get; set; }

        MediaHelper mediaHelper = new MediaHelper();
        MediaFile file;

        User _UserData = DataClass.GetInstance.User;
        public User UserData
        {
            get { return _UserData; }
            set { SetProperty(ref _UserData, value, nameof(UserData)); }
        }

        bool _IsEdit = false;
        public bool IsEdit
        {
            get { return _IsEdit; }
            set { SetProperty(ref _IsEdit, value, nameof(IsEdit)); UserData = value ? DataClass.GetInstance.User.Clone() : DataClass.GetInstance.User;  }
        }

        User UserCached { get { return DataClass.GetInstance.User; } }

        public ProfilePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {

            BackButtonClickedCommand = new DelegateCommand(OnBackButtonClicked);
            UploadPhotoClickedCommand = new DelegateCommand(OnUploadPhotoClicked);
            DateSelectedCommand = new DelegateCommand<object>(OnDateSelected);
            LoginLabelTappedCommand = new DelegateCommand(OnLoginLabelTapped);
            ChangePasswordTappedCommand = new DelegateCommand(OnChangePasswordLabelTapped);
            
        }

        private void OnDateSelected(object sender)
        {
            if ((sender as CustomDatePicker).TextColor != Color.White)
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
                            UserData.Photo = file.Path;
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
                            UserData.Photo = file.Path;
                        }
                    }
                    else if (action.ToString() == "Remove Photo")
                    {
                        if (file != null)
                        {
                            file = null;
                            UserData.Photo = null;
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

        private void OnLoginLabelTapped()
        {
            bool IsValid = true;

            if (file == null)
            {
                IsValid = false;
            }

            if (string.IsNullOrEmpty(UserData.FirstName) || UserData.FirstName.Equals(UserCached.FirstName))
            {
                IsValid = false;
            }


            if (UserData.Equals(UserCached))
            {
                IsValid = false;
            }

            //if (LastNameEntry.ValidateIsTextNullOrEmpty("Last Name is required"))
            //{
            //    IsValid = false;
            //}

            //if (MobileNumberEntry.ValidateIsTextNullOrEmpty("Mobile number is required"))
            //{
            //    IsValid = false;
            //}
            //else if (!(MobileNumberEntry.Text).IsValidPhoneNumber())
            //{
            //    MobileNumberEntry.ShowError("Invalid Mobile Number");
            //    IsValid = false;
            //}

            //if (AddressEntry.ValidateIsTextNullOrEmpty("Address is required"))
            //{
            //    IsValid = false;
            //}

            //if (Birthdate.Date == DateTime.Today)
            //{
            //    BirthdateTextColor = Color.Red;
            //    IsValid = false;
            //}

            //if (EmailAddressEntry.ValidateIsTextNullOrEmpty("Email is required"))
            //{
            //    IsValid = false;
            //}
            //else if (!(EmailAddressEntry.Text).IsValidEmail())
            //{
            //    EmailAddressEntry.ShowError("Invalid Email Address");
            //    IsValid = false;
            //}

            //if (PasswordEntry.ValidateIsTextNullOrEmpty("Password is required"))
            //{
            //    IsValid = false;
            //}

            //if (!IsChecked)
            //{
            //    EULATextColor = Color.Red;
            //    IsValid = false;
            //}

            if (IsValid && !IsClicked)
            {
                IsClicked = true;
                //TODO Register API
            }
        }

        private void OnChangePasswordLabelTapped()
        {
            NavigationService.NavigateAsync("/MainPage");
        }
    }
}
