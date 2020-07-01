using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using ZXing.Mobile;

namespace KeepSafe.Views
{
    public partial class ScanPage : MainNavigationPage
    {
        static CameraResolution CameraResolution;

        public ScanPage()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.Android)
                scannerView.Options = new ZXing.Mobile.MobileBarcodeScanningOptions() { CameraResolutionSelector = CameraResolutionSelectorDelegate_Execute };
        }

        CameraResolution CameraResolutionSelectorDelegate_Execute(List<CameraResolution> availableResolutions)
        {
            if (CameraResolution == null)
            {
                CameraResolution = availableResolutions.FirstOrDefault((reso) => reso.Width <= 800);
                SetPadding(CameraResolution.Height, CameraResolution.Width);
            }
            return CameraResolution;
        }

        private void SetPadding(int videoWidth, int videoHeight)
        {
            var controlWidth = width;
            var controlHeight = height /*- Constants.STATUS_BAR_HEIGHT - Constants.NAVIGATION_HEIGHT*/ - 61.ScaleHeight();

            double widthRatio = controlWidth / videoWidth;
            double widthSet = widthRatio * videoWidth;
            double heightSet = widthRatio * videoHeight;

            int xoff = (int)(controlWidth - widthSet) / 2;
            int yoff = (int)(controlHeight - heightSet) / 2;

            scannerView.HorizontalOptions = LayoutOptions.Center;
            scannerView.VerticalOptions = LayoutOptions.Center;

            scannerView.HeightRequest = heightSet;
            scannerView.WidthRequest = widthSet;
            scannerView.Margin = new Thickness(xoff, yoff);
            //scannerViewLabel.Text = $"Resolution = ({videoWidth},{videoHeight})\nViewSize = ({widthSet},{heightSet})\nScreenSize = ({controlWidth},{controlHeight})\nMargin = ({xoff},{yoff})";
        }

        double width, height;
        void Grid_PropertyChanged(System.Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is View view)
            {
                if (e.PropertyName.Equals(HeightProperty.PropertyName))
                {
                    height = view.Height;
                }
                else if (e.PropertyName.Equals(HeightRequestProperty.PropertyName))
                {
                    height = view.HeightRequest;
                }
                else if (e.PropertyName.Equals(WidthProperty.PropertyName))
                {
                    width = view.Width;
                }
                else if (e.PropertyName.Equals(WidthRequestProperty.PropertyName))
                {
                    width = view.WidthRequest;
                }
            }
        }
    }
}
