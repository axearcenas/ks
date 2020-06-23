using System;
using KeepSafe;
using Xamarin.Forms;

namespace KeepSafe.Models
{
    public class CustomAlertModel : BaseNotify
    {
        double _AndroidBuild;
        public double AndroidBuild { get { return _AndroidBuild; } set { _AndroidBuild = value; OnPropertyChanged(); } }
        double _MinimumAndroidBuild;
        public double MinimumAndroidBuild { get { return _MinimumAndroidBuild; } set { _MinimumAndroidBuild = value; OnPropertyChanged(); } }
        string _AndroidLink;
        public string AndroidLink { get { return _AndroidLink; } set { _AndroidLink = value; OnPropertyChanged(); } }
        string _AndroidDescription;
        public string AndroidDescription { get { return _AndroidDescription; } set { _AndroidDescription = value; OnPropertyChanged(); OnPropertyChanged(nameof(Content)); } }
        int _AndroidHTextAlignment;
        public int AndroidHTextAlignment { get { return _AndroidHTextAlignment; } set { _AndroidHTextAlignment = value; OnPropertyChanged(); if (Device.RuntimePlatform == Device.Android) { OnPropertyChanged(nameof(HTextAlignment)); } } }


        double _IosBuild;
        public double IosBuild { get { return _IosBuild; } set { _IosBuild = value; OnPropertyChanged(); } }
        double _MinimumIosBuild;
        public double MinimumIosBuild { get { return _MinimumIosBuild; } set { _MinimumIosBuild = value; OnPropertyChanged(); } }
        string _IosLink;
        public string IosLink { get { return _IosLink; } set { _IosLink = value; OnPropertyChanged(); } }
        string _IosDescription;
        public string IosDescription { get { return _IosDescription; } set { _IosDescription = value; OnPropertyChanged(); OnPropertyChanged(nameof(Content)); } }
        int _IosHTextAlignment;
        public int IosHTextAlignment { get { return _IosHTextAlignment; } set { _IosHTextAlignment = value; OnPropertyChanged(); if (Device.RuntimePlatform == Device.iOS) { OnPropertyChanged(nameof(HTextAlignment)); } } }

        public string Content
        {
            get
            {
                return Device.RuntimePlatform == Device.iOS ? IosDescription : AndroidDescription;
            }
        }

        public TextAlignment HTextAlignment
        {
            get
            {
                switch (Device.RuntimePlatform == Device.iOS ? IosHTextAlignment : AndroidHTextAlignment)
                {
                    case 0: return TextAlignment.Center;
                    case 1: return TextAlignment.Start;
                    case 2: return TextAlignment.End;
                    default: return TextAlignment.Center;
                }
            }
        }

        public double BuildVersion => Device.RuntimePlatform == Device.iOS ? IosBuild : AndroidBuild;
        public double MinimumBuildVersion => Device.RuntimePlatform == Device.iOS ? MinimumIosBuild : MinimumAndroidBuild;

        string _Description;
        public string Description { get { return _Description; } set { _Description = value; OnPropertyChanged(); } }

        string _Title;
        public string Title { get { return _Title; } set { _Title = value; OnPropertyChanged(); } }

        string _Image;
        public string Image { get { return _Image; } set { _Image = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasImage)); } }
        double _ImageWidth;
        public double ImageWidth { get { return _ImageWidth.ScaleWidth(); } set { _ImageWidth = value; OnPropertyChanged(); } }
        double _ImageHeight;
        public double ImageHeight { get { return _ImageHeight.ScaleHeight(); } set { _ImageHeight = value; OnPropertyChanged(); } }
        public bool HasImage { get { return !string.IsNullOrEmpty(Image); } }

        string _ButtonText;
        public string ButtonText { get { return _ButtonText; } set { _ButtonText = value; OnPropertyChanged(); } }
        double _ButtonWidth;
        public double ButtonWidth { get { return _ButtonWidth.ScaleWidth(); } set { _ButtonWidth = value; OnPropertyChanged(); } }

        bool _IsShowButton;
        public bool IsShowButton { get { return _IsShowButton; } set { _IsShowButton = value; OnPropertyChanged(); } }

        bool _IsShowClose;
        public bool IsShowClose { get { return _IsShowClose; } set { _IsShowClose = value; OnPropertyChanged(); } }

        int _PopupType;
        /// <summary>
        /// 0 - App Update
        /// 1 - App Update - No Internet Connection
        /// 2 - Unauthorized Access
        /// 3 - No Internet Connection
        /// 4 - Maintenance Start
        /// 5 - Maintenance Done
        /// </summary>
        public int PopupType { get { return _PopupType; } set { _PopupType = value; OnPropertyChanged(); } }

        bool _IsForceLogout;
        public bool IsForceLogout { get { return _IsForceLogout; } set { _IsForceLogout = value; OnPropertyChanged(); } }
    }
}
