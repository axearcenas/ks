using System;
using System.Threading.Tasks;
using FFImageLoading;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;
using KeepSafe.Helpers.Permission;
using Xamarin.Forms.PlatformConfiguration;

namespace KeepSafe.Helpers.MediaHelper
{
    [Preserve(AllMembers = true)]
    public class MediaHelper : IMediaHelper
    {
        public async Task<MediaFile> PickPhotoAsync(PickMediaOptions options = null)
        {
            
            var photosPermission = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.Photos()); // iOS
            var storageReadPermissions = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.StorageRead()); // Android
            var storageWritePermissions = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.StorageWrite()); // Android

            if (photosPermission != PermissionStatus.Granted)
            {
                //PopupHelper.ShowCustomMessagePopup(OpenSettings, new CustomMessageModel() { Title = "Permission Denied", Content = "Photos permission is required so you can take a photo as display picture for your profile. Please go to your phone's Settings and turn on permissions for the app.", Button = "Open Settings", Icon = "exclamation-circle", IconType = 2 });
                ShowErrorAlert("Permission Denied", "Photos permission is required so you can take a photo as display picture for your profile. Please go to your phone's Settings and turn on permissions for the app.");
            }

            if (storageReadPermissions != PermissionStatus.Granted || storageReadPermissions != PermissionStatus.Granted)
            {
                //PopupHelper.ShowCustomMessagePopup(OpenSettings, new CustomMessageModel() { Title = "Permission Denied", Content = "Storage permission is required so you can pick a photo as display picture for your profile. Please go to your phone's Settings and turn on permissions for the app.", Button = "Open Settings", Icon = "exclamation-circle", IconType = 2 });
                ShowErrorAlert("Permission Denied", "Storage permission is required so you can pick a photo as display picture for your profile. Please go to your phone's Settings and turn on permissions for the app.");
            }

            if (photosPermission == PermissionStatus.Granted && storageReadPermissions == PermissionStatus.Granted && storageWritePermissions == PermissionStatus.Granted)
            {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    //PopupHelper.ShowCustomMessagePopup(new CustomMessageModel() { Title = "Not Supported", Content = "Picking photo is not supported.", Button = "Okay", Icon = "exclamation-circle", IconType = 2 });
                    return null;
                }
                return await CrossMedia.Current.PickPhotoAsync(options);
            }

            return null;
        }

        public async Task<MediaFile> TakePhotoAsync(StoreCameraMediaOptions options)
        {
            var cameraPermissions = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.Camera()); // iOS and Android
            var storageReadPermissions = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.StorageRead()); // Android
            var storageWritePermissions = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.StorageWrite()); // Android

            if (cameraPermissions != PermissionStatus.Granted)
            {
                //PopupHelper.ShowCustomMessagePopup(OpenSettings, new CustomMessageModel() { Title = "Permission Denied", Content = "Camera permission is required so you can take a photo as display picture for your profile. Please go to your phone's into Settings and turn on permissions for the app.", Button = "Open Settings", Icon = "exclamation-circle", IconType = 2 });
            }

            if (storageReadPermissions != PermissionStatus.Granted || storageReadPermissions != PermissionStatus.Granted)
            {
                //PopupHelper.ShowCustomMessagePopup(OpenSettings, new CustomMessageModel() { Title = "Permission Denied", Content = "Storage permission is required so you can pick a photo as display picture for your profile. Please go to your phone's Settings and turn on permissions for the app.", Button = "Open Settings", Icon = "exclamation-circle", IconType = 2 });
            }

            if (cameraPermissions == PermissionStatus.Granted && storageReadPermissions == PermissionStatus.Granted && storageWritePermissions == PermissionStatus.Granted)
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    //PopupHelper.ShowCustomMessagePopup(new CustomMessageModel() { Title = "Not Supported", Content = "No camera available or taking photo is not supported.", Button = "Okay", Icon = "exclamation-circle", IconType = 2 });
                    return null;
                }
                return await CrossMedia.Current.TakePhotoAsync(options);
            }

            return null;
        }

        public async Task<MediaFile> PickVideoAsync()
        {
            var photosPermission = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.Photos()); // iOS
            var storageReadPermissions = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.StorageRead()); // Android
            var storageWritePermissions = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.StorageWrite()); // Android

            if (photosPermission != PermissionStatus.Granted)
            {
                //PopupHelper.ShowCustomMessagePopup(OpenSettings, new CustomMessageModel() { Title = "Permission Denied", Content = "Photos permission is required to pick a video. Please go to your phone's Settings and turn on permissions for the app.", Button = "Open Settings", Icon = "exclamation-circle", IconType = 2 });
                ShowErrorAlert("Permission Denied", "Photos permission is required to pick a video. Please go to your phone's Settings and turn on permissions for the app.");
            }

            if (storageReadPermissions != PermissionStatus.Granted || storageReadPermissions != PermissionStatus.Granted)
            {
                //PopupHelper.ShowCustomMessagePopup(OpenSettings, new CustomMessageModel() { Title = "Permission Denied", Content = "Storage permission is required so you can pick a video. Please go to your phone's Settings and turn on permissions for the app.", Button = "Open Settings", Icon = "exclamation-circle", IconType = 2 });
                ShowErrorAlert("Permission Denied", "Storage permission is required so you can pick a video. Please go to your phone's Settings and turn on permissions for the app.");
            }

            if (photosPermission == PermissionStatus.Granted && storageReadPermissions == PermissionStatus.Granted && storageWritePermissions == PermissionStatus.Granted)
            {
                if (!CrossMedia.Current.IsPickVideoSupported)
                {
                    //PopupHelper.ShowCustomMessagePopup(new CustomMessageModel() { Title = "Not Supported", Content = "Picking photo is not supported.", Button = "Okay", Icon = "exclamation-circle", IconType = 2 });
                    return null;
                }
                return await CrossMedia.Current.PickVideoAsync();
            }

            return null;
        }

        public async Task<MediaFile> TakeVideoAsync(StoreVideoOptions options)
        {
            var cameraPermissions = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.Camera()); // iOS and Android
            var microphonePermissions = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.Microphone()); // iOS
            var storageReadPermissions = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.StorageRead()); // Android
            var storageWritePermissions = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.StorageWrite()); // Android
            
            if (cameraPermissions != PermissionStatus.Granted)
            {
                //PopupHelper.ShowCustomMessagePopup(OpenSettings, new CustomMessageModel() { Title = "Permission Denied", Content = "Camera permission is required so you can take a video. Please go to your phone's into Settings and turn on permissions for the app.", Button = "Open Settings", Icon = "exclamation-circle", IconType = 2 });
                ShowErrorAlert("Permission Denied", "Camera permission is required so you can take a video. Please go to your phone's into Settings and turn on permissions for the app.");
            }

            if (microphonePermissions != PermissionStatus.Granted)
            {
                //PopupHelper.ShowCustomMessagePopup(OpenSettings, new CustomMessageModel() { Title = "Permission Denied", Content = "Microphone permission is required so you can take a video. Please go to your phone's Settings and turn on permissions for the app.", Button = "Open Settings", Icon = "exclamation-circle", IconType = 2 });
                ShowErrorAlert("Permission Denied", "Microphone permission is required so you can take a video. Please go to your phone's Settings and turn on permissions for the app.");
            }

            if (storageReadPermissions != PermissionStatus.Granted || storageReadPermissions != PermissionStatus.Granted)
            {
                //PopupHelper.ShowCustomMessagePopup(OpenSettings, new CustomMessageModel() { Title = "Permission Denied", Content = "Storage permission is required so you can take a video. Please go to your phone's Settings and turn on permissions for the app.", Button = "Open Settings", Icon = "exclamation-circle", IconType = 2 });
                ShowErrorAlert("Permission Denied", "Storage permission is required so you can take a video. Please go to your phone's Settings and turn on permissions for the app.");
            }

            if (cameraPermissions == PermissionStatus.Granted && microphonePermissions == PermissionStatus.Granted && storageReadPermissions == PermissionStatus.Granted && storageWritePermissions == PermissionStatus.Granted)
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported)
                {
                    //PopupHelper.ShowCustomMessagePopup(new CustomMessageModel() { Title = "Not Supported", Content = "No camera available or taking photo is not supported.", Button = "Okay", Icon = "exclamation-circle", IconType = 2 });
                    return null;
                }
                return await CrossMedia.Current.TakeVideoAsync(options);
            }

            return null;
        }

        async void ShowErrorAlert(string title , string message ,bool CanOpenSettings = true)
        {
            var IsOpenSettings = await App.Current.MainPage.DisplayAlert(title, message, "Open Settings", "Cancel");
            if (IsOpenSettings && CanOpenSettings)
            {
                OpenSettings();
            }
        }

        public void OpenSettings()
        {
            Xamarin.Essentials.AppInfo.ShowSettingsUI();
        }
    }
}
