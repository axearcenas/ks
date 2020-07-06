using System;
using System.Threading;
using System.Threading.Tasks;
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
            set { _IsScanning = value; OnPropertyChanged(); }
        }

        public string TabIcon
        {
            get
            {
                return DataClass.GetInstance.AccountType == UserType.User ? "ScanIcon" : "BusinessScanIcon";
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
            if(e.PropertyName.Equals("Text"))
            {
                if(!SearchEntry.Text.Equals(SearchEntry.Text.ToUpper()))
                {
                    SearchEntry.Text = SearchEntry.Text.ToUpper();
                }
            }
        }

        private void OnSearchCommand_Execute()
        {
            if (!IsClicked && !SearchEntry.ValidateIsTextNullOrEmpty())
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

        async void SearchQR(string code, bool IsQrCode)
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
                await fileReader.CreateDummyResponse(JsonConvert.SerializeObject(
                    new
                    {
                        message = $"Scanned: {code}",
                        business = new Business() { Id = 0,Photo = RandomizerHelper.GetRandomImageUrl((int)158.ScaleWidth(), (int)158.ScaleHeight(), category: ImageCategory.tech), Name = "Golden Prince Hotel" },
                        type = RandomizerHelper.GetRandomInteger(10) % 2 == 0 ? 0 : 1,
                        status = 200
                    }),cts.Token, 0);
#else
                restService.SetDelegate(this);
                string content = JsonConvert.SerializeObject(new { code, IsQrCode });
                await RestRequest.PostRequestAsync($"{Constants.ROOT_API_URL}{Constants.HEROES_URL}{Constants.POWERS_URL}{Constants.VALIDATE_URL}".AddAuth(), content, cts.Token, 0);
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
                    case 0:
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                //TODO save USER Here
                                if (jsonData.ContainsKey("message"))
                                {
                                    PageDialogService?.DisplayAlertAsync("Scan Succesfully", jsonData["message"].ToString(), "Okay");
                                }
                                INavigationParameters keys = new NavigationParameters();
                                keys.Add("ScanPageActiveAction", ScanPageActive);
                                if(jsonData.ContainsKey("type"))
                                    keys.Add("IsCheckIn", jsonData["type"].ToObject<int>() == 0);
                                if (jsonData.ContainsKey("business"))
                                    keys.Add("Business", jsonData["business"].ToObject<Business>());
                                await NavigationService.NavigateAsync(nameof(UserCheckInPage), keys, useModalNavigation: true);
                                IsScanning = true;
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
