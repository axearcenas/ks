using System;
using System.Threading;
using KeepSafe.Extensions;
using KeepSafe.Helpers;
using KeepSafe.Helpers.FileReader;
using KeepSafe.Helpers.MediaHelper;
using KeepSafe.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace KeepSafe.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase , IFileConnector, IRestReceiver
    {
        public DelegateCommand BackButtonClickedCommand { get; set; }
        public DelegateCommand UploadPhotoClickedCommand { get; set; }
        public DelegateCommand<object> DateSelectedCommand { get; set; }
        public DelegateCommand LogoutTappedCommand { get; set; }
        public DelegateCommand EditTappedCommand { get; set; }
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
            set { SetProperty(ref _IsEdit, value, nameof(IsEdit));
                UserData.PropertyChanged -= UserData_PropertyChanged;
                UserData = _IsEdit ? DataClass.GetInstance.User.Clone() : DataClass.GetInstance.User;
                UserData.PropertyChanged += UserData_PropertyChanged;
            }
        }

        bool _CanSaveEdit;
        public bool CanSaveEdit
        {
            get { return _CanSaveEdit; }
            set { SetProperty(ref _CanSaveEdit, value, nameof(CanSaveEdit)); }
        }

        User UserCached { get { return DataClass.GetInstance.User; } }

        public ProfilePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {

            BackButtonClickedCommand = new DelegateCommand(OnBackButtonClicked);
            UploadPhotoClickedCommand = new DelegateCommand(OnUploadPhotoClicked);
            DateSelectedCommand = new DelegateCommand<object>(OnDateSelected);
            LogoutTappedCommand = new DelegateCommand(OnLogoutTappedCommand_Execute);
            EditTappedCommand = new DelegateCommand(OnEditTappedCommand_Execute);
            ChangePasswordTappedCommand = new DelegateCommand(OnChangePasswordLabelTapped);
            UserData.PropertyChanged += UserData_PropertyChanged;
        }

        private void UserData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CanSaveEdit = !UserData.Equals(UserCached);
        }

        private void OnDateSelected(object sender)
        {
            if ((sender as CustomDatePicker).TextColor != Color.White)
                (sender as CustomDatePicker).TextColor = Color.White;
        }

        private void OnBackButtonClicked()
        {
            IsEdit = false;
        }

        private async void OnUploadPhotoClicked()
        {
            if (!IsClicked && IsEdit)
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

        private async void OnEditTappedCommand_Execute()
        {
            if (IsEdit)
            {
                bool IsValid = true;

                if (UserData.Equals(UserCached))
                {
                    IsValid = false;
                }

                if (IsValid && !IsClicked)
                {
                    IsClicked = true;
                    //TODO Register API
                    if (cts != null)
                        cts.Cancel();
                    cts = new CancellationTokenSource();
                    PopupHelper.ShowLoading();
                    try
                    {                        
#if DEBUG
                            fileReader.SetDelegate(this);
                            //await fileReader.ReadFile("UserProfile.json", cts.Token, 0);
                            await fileReader.CreateDummyResponse(JsonConvert.SerializeObject(
                                new
                                {
                                    user = UserData,
                                    message = "Successfully update the user",
                                    status = 200
                                }), cts.Token, 0);
#else
                        if (file == null)
                        {
                            restService.SetDelegate(this);
                            string content = JsonConvert.SerializeObject(new { code, IsQrCode });
                            await RestRequest.PostRequestAsync($"{Constants.ROOT_API_URL}{Constants.HEROES_URL}{Constants.POWERS_URL}{Constants.VALIDATE_URL}".AddAuth(), content, cts.Token, 0);
                        }
                        else
                        {

                        }
#endif

                    }
                    catch (OperationCanceledException oce)
                    {
                        App.Log(oce.StackTrace, "SEARCH CODE");
                        PopupHelper.RemoveLoading(oce.Message);
                        IsClicked = true;
                    }
                    catch (Exception ex)
                    {
                        App.Log(ex.StackTrace, "SEARCH CODE");
                        PopupHelper.RemoveLoading(ex.Message);
                        IsClicked = true;
                    }
                    cts = null;
                }
            }
            else
            {
                IsEdit = true;
            }
        }

        private void OnLogoutTappedCommand_Execute()
        {
            App.Logout();
        }

        private void OnChangePasswordLabelTapped()
        {
            //TODO NAVIGATE TO CHANGE PASSWORD
            NavigationService.NavigateAsync("ChangePasswordPage");
        }

        public void ReceiveJSONData(JObject jsonData, int wsType)
        {
            if (jsonData.ContainsKey("status") && jsonData["status"].ToObject<int>() == 200)
            {
                switch (wsType)
                {
                    case 0:
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            //TODO save USER Here
                            if(jsonData.ContainsKey("message"))
                                PageDialogService.DisplayAlertAsync("User Updated!", jsonData["message"].ToString(), "Okay");
                            DataClass.GetInstance.User = UserData;
                            await Application.Current.SavePropertiesAsync();
                            CanSaveEdit = false;
                            IsEdit = false;
                        });
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
