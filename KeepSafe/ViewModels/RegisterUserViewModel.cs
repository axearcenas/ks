using System;
using System.Collections.Generic;
using KeepSafe.Helpers.MediaHelper;
using KeepSafe.ViewModels;
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
        public DelegateCommand<object> CheckBoxClickedCommand { get; set; }
        public DelegateCommand RegisterButtonClickedCommand { get; set; }
        public DelegateCommand LoginLabelTappedCommand { get; set; }

        MediaHelper mediaHelper = new MediaHelper();
        MediaFile file;

        //TODO Remove this tihs
        string _EstablishmentImage;
        public string EstablishmentImage
        {
            get { return _EstablishmentImage; }
            set { SetProperty(ref _EstablishmentImage, value, nameof(EstablishmentImage)); }
        }

        //private List<string> _EstablishmentType = new List<string> {  "Clinic", "Hospital", "Travel Agency", "Boutique", "Bar & Restaurant", "Manufacturing", "Banking & Remittance", "Spa & Personal Care", "Government", "Theatre & Movie Houses", "Appliance & Computer Store", "Department Store", "Grocery & Supermarket", "Automobile", "Home Improvement", "Graphics & Printing" };
        //public List<string> EstablishmentType
        //{
        //    get { return _EstablishmentType; }
        //    set { SetProperty(ref _EstablishmentType, value, nameof(EstablishmentType)); }
        //}

        bool IsClicked;
        bool IsChecked;


        public RegisterUserViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;

            BackButtonClickedCommand = new DelegateCommand(OnBackButtonClicked);
            UploadPhotoClickedCommand = new DelegateCommand(OnUploadPhotoClicked);
            CheckBoxClickedCommand = new DelegateCommand<object>(OnCheckboxClicked);
            RegisterButtonClickedCommand = new DelegateCommand(OnRegisterButtonClicked);
            LoginLabelTappedCommand = new DelegateCommand(OnLoginLabelTapped);
        }

        private void OnCheckboxClicked(object sender)
        {
            IsChecked = !IsChecked;
            (sender as Button).Text = IsChecked ? "check" : "";
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
                var myAction = await _pageDialogService.DisplayActionSheetAsync("Upload Photo", "Cancel", file == null ? null : "Remove Photo", "Camera", "Gallery");
                if (myAction != null && !myAction.ToString().Equals("Cancel"))
                {
                    await CrossMedia.Current.Initialize();
                    if (myAction.ToString() == "Camera")
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
                            //ShowUpdateImage();
                        }
                    }
                    else if (myAction.ToString() == "Gallery")
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
                            //ShowUpdateImage();
                        }
                    }
                    else if (myAction.ToString() == "Remove Photo")
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

        private void OnRegisterButtonClicked()
        {
            
        }

        private void OnLoginLabelTapped()
        {
            _navigationService.NavigateAsync("/MainPage");
        }
    }
}
