using System;
using System.Collections.Generic;
using System.Linq;
using KeepSafe.Helpers.Permission;
using KeepSafe.ViewModels;
using Prism.Behaviors;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;
namespace KeepSafe.Views
{
    public partial class ScanPage : MainNavigationPage
    {
        static CameraResolution CameraResolution;
        bool CanScan = true;
        public ScanPage()
        {
            InitializeComponent();
            //if (Device.RuntimePlatform == Device.Android)
                //scannerView.Options = new ZXing.Mobile.MobileBarcodeScanningOptions() { CameraResolutionSelector = CameraResolutionSelectorDelegate_Execute };
        }
        ScanPageViewModel ScanPageViewModel;
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (CanScan)
            {
                CanScan = false;

                var cameraPermission = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.Camera());
                //var flashlightPermission = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.Flashlight());

                if (cameraPermission == PermissionStatus.Granted)
                {
                    //await Navigation.ShowScannerPage(ScannerPage_OnScanResult);
                    InitScannerView();
                }
                else
                {
                    //Utilities.ShowCustomMessagePopup(new CustomMessageModel() { Title = "Permission Denied", Content = "Camera permission is required to use this app.", Button = "Okay", Icon = "exclamation-circle", IconType = 2 });
                }

                CanScan = true;
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if(ScanPageViewModel == null && BindingContext is ScanPageViewModel scanPageViewModel)
            {
                ScanPageViewModel = scanPageViewModel;
                ScanPageViewModel.IsActiveChanged += ScanPageViewModel_IsActiveChanged;
            }
        }

        private void ScanPageViewModel_IsActiveChanged(object sender, EventArgs e)
        {
            if (ScanPageViewModel.IsActive)
            {
                if(scannerView.Parent == null)
                    gridView.Children.Insert(0,scannerView);
            }
            else
            { 
                gridView.Children.Remove(scannerView);
            }
        }

        ZXingScannerView scannerView;
        void InitScannerView()
        {
            /*
             * <scanner:ZXingScannerView  Grid.Row="1"
                                   x:Name="scannerView"
                                   IsScanning="False"
                                   HorizontalOptions="Fill"
                                   VerticalOptions="Fill"
                                   WidthRequest="{local:ScaleWidthDouble Value=320}"
                                   HeightRequest="{local:ScaleHeightDouble Value=487}">
                <scanner:ZXingScannerView.Behaviors>
                    <prism:EventToCommandBehavior Command="{Binding QRScanResultComand}"
                                                  EventArgsParameterPath="."
                                                  EventName="OnScanResult"/>
                </scanner:ZXingScannerView.Behaviors>
            </scanner:ZXingScannerView>
             * */
            if (scannerView == null)
            {
                scannerView = new ZXingScannerView()
                {
                    IsScanning = false,
                    IsAnalyzing = false,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                };
                scannerView.SetBinding( ZXingScannerView.IsScanningProperty,"IsScanning");
                scannerView.SetBinding(ZXingScannerView.IsAnalyzingProperty, "IsScanning");
                Grid.SetRow(scannerView, 1);
                scannerView.OnScanResult += ScannerView_OnScanResult;
                //EventToCommandBehavior eventToCommandBehavior = new EventToCommandBehavior() { EventArgsParameterPath = ".", EventName = "OnScanResult" };
                //eventToCommandBehavior.SetBinding(EventToCommandBehavior.CommandProperty, "QRScanResultCommand");
                //scannerView.Behaviors.Add(eventToCommandBehavior);

                if (Device.RuntimePlatform == Device.Android)
                    scannerView.Options = new ZXing.Mobile.MobileBarcodeScanningOptions() { CameraResolutionSelector = CameraResolutionSelectorDelegate_Execute };
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private void ScannerView_OnScanResult(ZXing.Result result)
        {
            App.Log($"RESULT: {result.Text}");
            if(BindingContext is ScanPageViewModel scanPageViewModel)
            {
                scanPageViewModel.ScanCommand?.Execute(result);
            }
        }

        CameraResolution CameraResolutionSelectorDelegate_Execute(List<CameraResolution> availableResolutions)
        {
            if (CameraResolution == null)
            {
                CameraResolution = availableResolutions.FirstOrDefault((reso) => reso.Width <= 1000);
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
