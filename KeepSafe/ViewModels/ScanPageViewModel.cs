using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KeepSafe.Extensions;
using KeepSafe.Helpers;
using KeepSafe.Helpers.FileReader;
using KeepSafe.Helpers.Permission;
using KeepSafe.Models;
using KeepSafe.Resources;
using KeepSafe.ViewModels.ViewViewModels;
using KeepSafe.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing;

namespace KeepSafe.ViewModels
{
    public class ScanPageViewModel : ViewModelBase, IActiveAware , IFileConnector , IRestReceiver
    {
        public EntryViewModel SearchEntry { get; } = new EntryViewModel() { Placeholder = "Enter Manually", PlaceholderColor = ColorResource.MAIN_BLUE_COLOR, IsTextAllCaps=true };

        public DelegateCommand<Result> ScanCommand { get; set; }
        public DelegateCommand SearchCommand { get; set; }
        public DelegateCommand MyQRCommand { get; set; }

        Action<bool> ScanPageActive;

        bool _IsScanning;
        public bool IsScanning
        {
            get { return _IsScanning; }
            set { _IsScanning = value; RaisePropertyChanged(); }
        }
        
        public Color NavBgColor
        {
            get
            {
                return DataClass.GetInstance.LoginType == UserType.User ? ColorResource.MAIN_DARK_THEME_COLOR : ColorResource.ESTABLISHMENT_MAIN_THEME_COLOR;
            }
        }

        public Color ScannerBgColor
        {
            get
            {
                return DataClass.GetInstance.LoginType == UserType.User ? ColorResource.SCANNER_BACKGROUNDCOLOR : ColorResource.ESTABLISHMENT_MAIN_THEME_COLOR;
            }
        }

        public float FrameCornerRadius
        {
            get
            {
                return DataClass.GetInstance.LoginType == UserType.User ? 23.ScaleHeight() : 5.ScaleHeight();
            }
        }

        public string TabIcon
        {
            get
            {
                return DataClass.GetInstance.LoginType == UserType.User ? "ScanIcon" : "BusinessScanIcon";
            }
        }

        public string RightIcon
        {
            get
            {
                return DataClass.GetInstance.LoginType == UserType.User ? "MyQRIcon" : "";
            }
        }

        public string PageTitle
        {
            get
            {
                return DataClass.GetInstance.LoginType == UserType.User ? "Scan QR" : "Scan User QR";
            }
        }

        bool CanScan;

        public ScanPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            ScanCommand = new DelegateCommand<Result>(OnScanCommand_Execute);
            SearchCommand = new DelegateCommand(OnSearchCommand_Execute);
            ScanPageActive = new Action<bool>(OnScanPageActive_Execute);
            MyQRCommand = new DelegateCommand(OnMyQRCommand_Execute);
            SearchEntry.PropertyChanged += SearchEntry_PropertyChanged;
        }

        private async void OnMyQRCommand_Execute()
        {
            if (!IsClicked)
            {
                IsClicked = true;
                await NavigationService.NavigateAsync(nameof(MyQRPage));
                IsClicked = false;
            }
        }

        private void OnScanPageActive_Execute(bool obj)
        {
            IsActive = true;
        }

        private void SearchEntry_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Text"))
            {
                if (!SearchEntry.Text.Equals(SearchEntry.Text.ToUpper()))
                {
                    SearchEntry.Text = SearchEntry.Text.ToUpper();
                }
            }
        }

        private void OnSearchCommand_Execute()
        {
            if (!IsClicked && !SearchEntry.ValidateIsTextNullOrEmpty("Enter Manually"))
            {
                IsClicked = true;
                SearchQR(SearchEntry.Text, false);
            }
        }
        private void OnScanCommand_Execute(Result obj)
        {
            if (!IsClicked)
            {
                IsClicked = true;
                SearchQR(obj.Text, true);
            }
        }

        bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }
            set { SetProperty(ref _IsActive, value, nameof(IsActive)); RaiseIsActiveChanged(); }
        }

        public event EventHandler IsActiveChanged;

        protected virtual async  void RaiseIsActiveChanged()
        {
            IsScanning = IsActive;
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }

        async void SearchQR(string code, bool IsQrCode = true)
        {
            if (cts != null)
                cts.Cancel();
            cts = new CancellationTokenSource();
            PopupHelper.ShowLoading();
            try
            {
#if DEBUG
                IsScanning = false;
                await Task.Delay(500);
                fileReader.SetDelegate(this);
                //await fileReader.ReadFile("UserProfile.json", cts.Token, 0);
                if(dataClass.LoginType == UserType.User)
                {
                    await fileReader.CreateDummyResponse(JsonConvert.SerializeObject(new
                    {
                        data = new
                        {
                            message = $"Scanned: {code}",
                            business = new Business() { Id = 0, Image = RandomizerHelper.GetRandomImageUrl((int)158.ScaleWidth(), (int)158.ScaleHeight(), category: ImageCategory.tech), Name = "Golden Prince Hotel" },
                            qrcode = QrCode.Mock()
                        },
                        status = 200
                    }), cts.Token, 0);
                }
                else if(dataClass.LoginType == UserType.Establishment)
                {
                    await fileReader.CreateDummyResponse(JsonConvert.SerializeObject(new
                    {
                        data = new
                        {
                            message = $"Scanned: {code}",
                            user = new User() { Id = 0, Image = RandomizerHelper.GetRandomImageUrl((int)158.ScaleWidth(), (int)158.ScaleHeight(), category: ImageCategory.people), FirstName = "Uvuvwevwevwe", LastName = "Ossas" },
                            qrcode = QrCode.Mock()
                        },
                        status = 200
                    }), cts.Token, 1);
                }
                
#else
                restServices.SetDelegate(this);
                string content = JsonConvert.SerializeObject(new { scan_history= new { code ,qrcode = code} });
                await restServices.PostRequestAsync($"{Constants.ROOT_URL}{( dataClass.LoginType == UserType.User ? Constants.USER_URL : Constants.BUSINESS_URL)}{Constants.SCAN_HISTORIES_URL}", content, cts.Token, dataClass.LoginType == UserType.User ? 0 : 1, Constants.DEFAULT_AUTH);
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

        public void ReceiveJSONData(JObject jsonData, int wsType)
        {
            if (jsonData.ContainsKey("status") && jsonData["status"].ToObject<int>() == 200)
            {
                switch (wsType)
                {
                    case 0: // user scan establishment response
                        if(jsonData.ContainsKey("data"))
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                //TODO save USER Here
                                if (jsonData.ContainsKey("message"))
                                {
                                    PageDialogService?.DisplayAlertAsync("Scan Succesfully", jsonData["message"].ToString(), "Okay");
                                }
                                INavigationParameters keys = new NavigationParameters();
                                keys.Add("ScanPageActiveAction", ScanPageActive);
                                if(jsonData["data"].ContainsKey("qrcode") && jsonData["data"]["qrcode"].ContainsKey("code_type"))
                                    keys.Add("IsCheckIn", jsonData["data"]["qrcode"]["code_type"].ToObject<string>().Equals("check_in"));
                                if (jsonData["data"].ContainsKey("business"))
                                    keys.Add("Business", jsonData["data"]["business"].ToObject<Business>());
                                if (jsonData["data"].ContainsKey("qrcode") && jsonData["data"]["qrcode"].ContainsKey("code"))
                                    keys.Add("Qrcode", jsonData["data"]["qrcode"]["code"].ToObject<string>());
                                await NavigationService.NavigateAsync(nameof(UserCheckInPage), keys, useModalNavigation: true);
                                SearchEntry.ClearText();
                                IsScanning = true;
                                IsClicked = false;
                            });
                        break;
                    case 1: // establishment scan user response
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            if (jsonData.ContainsKey("data"))
                            {
                                if (jsonData.ContainsKey("message"))
                                {
                                    PageDialogService?.DisplayAlertAsync("Scan Succesfully", jsonData["message"].ToString(), "Okay");
                                }
                                INavigationParameters parameter = new NavigationParameters();
                                parameter.Add("ScanPageActiveAction", ScanPageActive);
                                if (jsonData["data"].ContainsKey("user"))
                                    parameter.Add("User", jsonData["data"]["user"].ToObject<User>());

                                await NavigationService.NavigateAsync(nameof(BusinessReceptionPage), parameter, useModalNavigation: true);
                                SearchEntry.ClearText();
                                IsScanning = true;
                                IsClicked = false;
                            }
                        });
                    break;
                }
            }
            
            PopupHelper.RemoveLoading();
        }

        public void ReceiveError(string title, string error, int wsType)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                PopupHelper.RemoveLoading();
                PageDialogService?.DisplayAlertAsync(title, error, "Okay");
                IsClicked = false;
            });
        }
    }
}
