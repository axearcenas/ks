﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using KeepSafe.Enum;
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
    public class BusinessProfileViewModel : ViewModelBase, IFileConnector, IRestReceiver
    {
        public DelegateCommand BackButtonClickedCommand { get; set; }
        public DelegateCommand UploadPhotoClickedCommand { get; set; }
        public DelegateCommand<object> SelectedIndexChangedCommand { get; set; }
        public DelegateCommand EditTappedCommand { get; set; }
        public DelegateCommand ChangePasswordTappedCommand { get; set; }
        public DelegateCommand LogoutTappedCommand { get; set; }
        public DelegateCommand<string> EntryFocusedCommand { get; set; }

        public EntryViewModel PasswordEntry { get; } = new EntryViewModel() { Placeholder = "Password", PlaceholderColor = ColorResource.WHITE_COLOR, IsPassword = true };
        public EntryViewModel NewPasswordEntry { get; } = new EntryViewModel() { Placeholder = "New Password", PlaceholderColor = ColorResource.WHITE_COLOR, IsPassword = true };

        BusinessType _SelectedBusinessType;
        public BusinessType SelectedBusinessType
        {
            get { return _SelectedBusinessType; }
            set { SetProperty(ref _SelectedBusinessType, value, nameof(SelectedBusinessType)); }
        }

        string _EstablishmentImage;
        public string EstablishmentImage
        {
            get { return _EstablishmentImage; }
            set { SetProperty(ref _EstablishmentImage, value, nameof(EstablishmentImage)); }
        }

        Business _BusinessData = DataClass.GetInstance.Business;
        public Business BusinessData
        {
            get { return _BusinessData; }
            set { SetProperty(ref _BusinessData, value, nameof(BusinessData)); }
        }

        bool _IsEdit = false;
        public bool IsEdit
        {
            get { return _IsEdit; }
            set
            {
                SetProperty(ref _IsEdit, value, nameof(IsEdit));
                BusinessData.PropertyChanged -= UserData_PropertyChanged;
                BusinessData = _IsEdit ? DataClass.GetInstance.Business.Clone() : DataClass.GetInstance.Business;
                BusinessData.PropertyChanged += UserData_PropertyChanged;
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

        Business BusinessCachedData { get { return DataClass.GetInstance.Business; } }

        MediaHelper mediaHelper = new MediaHelper();
        MediaFile file;

        public string TabIcon
        {
            get
            {
                return DataClass.GetInstance.LoginType == UserType.User ? "ProfileIcon" : "BusinessProfileIcon";
            }
        }

        public BusinessProfileViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            BackButtonClickedCommand = new DelegateCommand(OnBackButtonClicked);
            UploadPhotoClickedCommand = new DelegateCommand(OnUploadPhotoClicked);
            SelectedIndexChangedCommand = new DelegateCommand<object>(SelectedIndexChanged);
            EditTappedCommand = new DelegateCommand(OnEditTappedCommand_Execute);
            LogoutTappedCommand = new DelegateCommand(OnLogoutTappedCommand_Execute);
            ChangePasswordTappedCommand = new DelegateCommand(OnChangePasswordLabelTapped);
            EntryFocusedCommand = new DelegateCommand<string>(OnEntryFocusedCommand_Execute);

            BusinessData.PropertyChanged += UserData_PropertyChanged;
        }

        private void UserData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CanSaveEdit = !BusinessData.Equals(BusinessCachedData);
        }

        private void OnBackButtonClicked()
        {
            IsEdit = false;
            file = null;
            IsChangePassword = false;
            PasswordEntry.Text = "";
            NewPasswordEntry.Text = "";
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

        private async void OnEditTappedCommand_Execute()
        {
            if (IsEdit)
            {
                bool IsValid = true;

                if (BusinessData.Equals(BusinessCachedData))
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
                                user = BusinessData,
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

        private async void OnChangePasswordLabelTapped()
        {

            if (!IsChangePassword)
            {
                IsChangePassword = true;
            }
            else
            {
                bool IsValid = true;

                if (PasswordEntry.ValidateIsTextNullOrEmpty("Password is required!"))
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
                            restService.SetDelegate(this);
                            string content = JsonConvert.SerializeObject(new { code, IsQrCode });
                            await RestRequest.PostRequestAsync($"{Constants.ROOT_API_URL}{Constants.HEROES_URL}{Constants.POWERS_URL}{Constants.VALIDATE_URL}".AddAuth(), content, cts.Token, 0);
#endif

                    }

                    catch (OperationCanceledException ox) { App.Log($"StackTrace: {ox.StackTrace}\nMESSAGE: {ox.Message}"); IsClicked = false; IsLoading = false; }
                    catch (TimeoutException te) { App.Log($"StackTrace: {te.StackTrace}\nMESSAGE: {te.Message}"); IsClicked = false; IsLoading = false; }
                    catch (Exception ex) { App.Log($"StackTrace: {ex.StackTrace}\nMESSAGE: {ex.Message}"); IsClicked = false; IsLoading = false; }
                    cts = null;
                }
            }
        }

        private void OnLogoutTappedCommand_Execute()
        {
            App.Logout();
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
                                PageDialogService.DisplayAlertAsync("Business Profile Updated!", jsonData["message"].ToString(), "Okay");
                            DataClass.GetInstance.Business = BusinessData;
                            await Application.Current.SavePropertiesAsync();
                            CanSaveEdit = false;
                            IsEdit = false;
                        });
                        break;
                    case 1:
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            //TODO save new password Here
                            if (jsonData.ContainsKey("message"))
                                await PageDialogService.DisplayAlertAsync("Business Profile Updated!", jsonData["message"].ToString(), "Okay");
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
