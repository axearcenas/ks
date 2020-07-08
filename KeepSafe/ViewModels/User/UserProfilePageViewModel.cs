using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using KeepSafe.Extensions;
using KeepSafe.Helpers;
using KeepSafe.Helpers.FileReader;
using KeepSafe.Helpers.MediaHelper;
using KeepSafe.Models;
using KeepSafe.Resources;
using KeepSafe.ViewModels.ViewViewModels;
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
    public class UserProfilePageViewModel : ViewModelBase, IFileConnector, IRestReceiver
    {
        public EntryViewModel PasswordEntry { get; } = new EntryViewModel() { Placeholder = "Current Password", PlaceholderColor = ColorResource.WHITE_COLOR, IsPassword = true };
        public EntryViewModel NewPasswordEntry { get; } = new EntryViewModel() { Placeholder = "New Password", PlaceholderColor = ColorResource.WHITE_COLOR, IsPassword = true };

        public DelegateCommand BackButtonClickedCommand { get; set; }
        public DelegateCommand UploadPhotoClickedCommand { get; set; }
        public DelegateCommand<object> DateSelectedCommand { get; set; }
        public DelegateCommand LogoutTappedCommand { get; set; }
        public DelegateCommand EditTappedCommand { get; set; }
        public DelegateCommand ChangePasswordTappedCommand { get; set; }
        public DelegateCommand<string> EntryFocusedCommand { get; set; }
        public DelegateCommand<string> ShowPasswordCommand { get; set; }

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
            set
            {
                SetProperty(ref _IsEdit, value, nameof(IsEdit));
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

        bool _IsChangePassword;
        public bool IsChangePassword
        {
            get { return _IsChangePassword; }
            set { SetProperty(ref _IsChangePassword, value, nameof(IsChangePassword)); }
        }

        User UserCached { get { return DataClass.GetInstance.User; } }
        
        public string TabIcon
        {
            get
            {
                return DataClass.GetInstance.LoginType == UserType.User ? "ProfileIcon" : "BusinessProfileIcon";
            }
        }

        public UserProfilePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {

            BackButtonClickedCommand = new DelegateCommand(OnBackButtonClicked);
            UploadPhotoClickedCommand = new DelegateCommand(OnUploadPhotoClicked);
            DateSelectedCommand = new DelegateCommand<object>(OnDateSelected);
            LogoutTappedCommand = new DelegateCommand(OnLogoutTappedCommand_Execute);
            EditTappedCommand = new DelegateCommand(OnEditTappedCommand_Execute);
            ChangePasswordTappedCommand = new DelegateCommand(OnChangePasswordLabelTapped);
            UserData.PropertyChanged += UserData_PropertyChanged;
            EntryFocusedCommand = new DelegateCommand<string>(OnEntryFocusedCommand_Execute);
            ShowPasswordCommand = new DelegateCommand<string>(OnShowPasswordCommand_Execute);
        }

        private void OnShowPasswordCommand_Execute(string obj)
        {
            switch(obj)
            {
                case "0":// Current Password
                    PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
                    break;
                case "1":// New Password
                    NewPasswordEntry.IsPassword = !NewPasswordEntry.IsPassword;
                    break;
            }
        }

        private void UserData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CanSaveEdit = !UserData.Equals(UserCached);
        }


        private void OnEntryFocusedCommand_Execute(string obj)
        {
            switch (obj)
            {
                case "0": // password
                    PasswordEntry.ToDefaultValue();
                    break;
                case "1": // new password
                    PasswordEntry.ToDefaultValue();
                    break;
            }
        }

        private void OnDateSelected(object sender)
        {
            if ((sender as CustomDatePicker).TextColor != Color.White)
                (sender as CustomDatePicker).TextColor = Color.White;
        }

        private void OnBackButtonClicked()
        {
            IsEdit = false;
            file = null;
            CanSaveEdit = false;
            IsChangePassword = false;
            PasswordEntry.Text = "";
            NewPasswordEntry.Text = "";
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
                            UserData.Image = file.Path;
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
                            UserData.Image = file.Path;
                        }
                    }
                    else if (action.ToString() == "Remove Photo")
                    {
                        if (file != null)
                        {
                            file = null;
                            UserData.Image = null;
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
                    //TODO Update API
                    if (cts != null)
                        cts.Cancel();
                    cts = new CancellationTokenSource();
                    PopupHelper.ShowLoading();
                    try
                    {
#if DEBUG
                            fileReader.SetDelegate(this);
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
                            restServices.SetDelegate(this);
                            string content = JsonConvert.SerializeObject(new { user = UserData });
                            await restServices.PostRequestAsync($"{Constants.ROOT_URL}{Constants.USER_URL}{Constants.USERS_URL}{Constants.UPDATE_DETAILS_URL}".AddAuth(), content, cts.Token, 0);
                        }
                        else
                        {
                            restServices.SetDelegate(this);
                            string content = JsonConvert.SerializeObject(new { user = UserData });
                            Dictionary<string, Stream> images = new Dictionary<string, Stream>() { { "user[image]", file.GetStreamWithImageRotatedForExternalStorage() } };
                            await restServices.MultiPartFormRequestAsync($"{Constants.ROOT_URL}{Constants.USER_URL}{Constants.USERS_URL}{Constants.UPDATE_DETAILS_URL}".AddAuth(), content, images, cts.Token, HttpMethod.Post, 0);
                        }
#endif

                    }

                    catch (OperationCanceledException ox) { App.Log($"StackTrace: {ox.StackTrace}\nMESSAGE: {ox.Message}"); IsClicked = false; IsLoading = false; }
                    catch (TimeoutException te) { App.Log($"StackTrace: {te.StackTrace}\nMESSAGE: {te.Message}"); IsClicked = false; IsLoading = false; }
                    catch (Exception ex) { App.Log($"StackTrace: {ex.StackTrace}\nMESSAGE: {ex.Message}"); IsClicked = false; IsLoading = false; }
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

        private async void OnChangePasswordLabelTapped()
        {

            if (!IsChangePassword)
            {
                IsChangePassword = true;
            }
            else
            {
                bool IsValid = true;

                if (PasswordEntry.ValidateIsTextNullOrEmpty("Current password is required!"))
                {
                    IsValid = false;
                }
                if (NewPasswordEntry.ValidateIsTextNullOrEmpty("New password is required!"))
                {
                    IsValid = false;
                }

                if (IsValid && !IsClicked)
                {
                    IsClicked = true;
                    //TODO change password API
                    if (cts != null)
                        cts.Cancel();
                    cts = new CancellationTokenSource();
                    PopupHelper.ShowLoading();
                    try
                    {
#if DEBUG
                            fileReader.SetDelegate(this);
                            await fileReader.CreateDummyResponse(JsonConvert.SerializeObject(
                                new
                                {
                                    message = "Successfully change password.",
                                    status = 200
                                }), cts.Token, 1);
#else
                        restServices.SetDelegate(this);
                        string content = JsonConvert.SerializeObject(new { user = new { current_password = PasswordEntry.Text, password = NewPasswordEntry.Text } });
                        await restServices.PostRequestAsync($"{Constants.ROOT_URL}{Constants.USER_URL}{Constants.USERS_URL}{Constants.UPDATE_PASSWORD_URL}".AddAuth(), content, cts.Token, 1);
#endif
                    }

                    catch (OperationCanceledException ox) { App.Log($"StackTrace: {ox.StackTrace}\nMESSAGE: {ox.Message}"); IsClicked = false; IsLoading = false; }
                    catch (TimeoutException te) { App.Log($"StackTrace: {te.StackTrace}\nMESSAGE: {te.Message}"); IsClicked = false; IsLoading = false; }
                    catch (Exception ex) { App.Log($"StackTrace: {ex.StackTrace}\nMESSAGE: {ex.Message}"); IsClicked = false; IsLoading = false; }
                    cts = null;
                }
            }
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
                            if (jsonData.ContainsKey("message"))
                                PageDialogService.DisplayAlertAsync("User Updated!", jsonData["message"].ToString(), "Okay");
                            if (jsonData.ContainsKey("data"))
                            { 
                                User userData = JsonConvert.DeserializeObject<User>(jsonData["data"].ToString());
                                DataClass.GetInstance.User = userData;
                                await Application.Current.SavePropertiesAsync();
                                CanSaveEdit = false;
                                IsEdit = false;
                            }
                        });
                        break;
                    case 1:
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            //TODO save new password Here
                            if (jsonData.ContainsKey("message"))
                                await PageDialogService.DisplayAlertAsync("User Updated!", jsonData["message"].ToString(), "Okay");
                            PasswordEntry.ClearText();
                            NewPasswordEntry.ClearText();
                            IsChangePassword = false;
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
