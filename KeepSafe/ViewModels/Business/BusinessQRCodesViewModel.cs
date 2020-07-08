using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using KeepSafe.Helpers;
using KeepSafe.Helpers.FileReader;
using KeepSafe.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace KeepSafe.ViewModels
{
    public class BusinessQRCodesViewModel : ViewModelBase, IFileConnector, IRestReceiver
    {
        public DelegateCommand BackButtonClickedCommand { get; set; }
        public DelegateCommand<object> QRCodeSelectedCommand { get; set; }

        public ObservableCollection<QrCodeGroup> QrCodes { get; private set; } = new ObservableCollection<QrCodeGroup>();

        ObservableCollection<BusinessScanHistory> _ScanHistory;
        public ObservableCollection<BusinessScanHistory> ScanHistory
        {
            get { return _ScanHistory; }
            set { SetProperty(ref _ScanHistory, value, nameof(ScanHistory)); }
        }

        bool IsDataLoaded;

        public BusinessQRCodesViewModel(INavigationService navigationService) : base(navigationService)
        {
            BackButtonClickedCommand = new DelegateCommand(OnBackButtonClicked);
            QRCodeSelectedCommand = new DelegateCommand<object>(OnQRCodeSelected);
        }

        private async void OnBackButtonClicked()
        {
            if (!IsClicked)
            {
                IsClicked = true;
                await NavigationService.GoBackAsync();
                IsClicked = false;
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if(!IsDataLoaded)
                GetData();
        }

        private async void GetData()
        {
            PopupHelper.ShowLoading();
            await Task.Delay(1000);
            await Task.Run(async () =>
            {
                if (cts != null)
                    cts.Cancel();
                cts = new System.Threading.CancellationTokenSource();
                try
                {
#if DEBUG
                    fileReader.SetDelegate(this);
                    await fileReader.ReadFile("BusinessQRCodes.json", cts.Token, 0);
#else
                //TODO GET HISTORY Rest Here
                restServices.SetDelegate(this);
                string url = $"{Constants.ROOT_URL}{Constants.USER_URL}{Constants.SCAN_HISTORIES_URL}{ ($"?type={(SelectedHistoryType == HistoryType.CheckIn ? "check_in" : "check_out")}") }";
                await restServices.GetRequest(IsPagination ? MyScanHistoryPagination.Url : url ,  cts.Token, IsPagination ? 1 : 0,Constants.DEFAULT_AUTH);
#endif
                }
                catch (OperationCanceledException ox) { App.Log($"StackTrace: {ox.StackTrace}\nMESSAGE: {ox.Message}"); IsClicked = false; PopupHelper.RemoveLoading(); }
                catch (TimeoutException te) { App.Log($"StackTrace: {te.StackTrace}\nMESSAGE: {te.Message}"); IsClicked = false; PopupHelper.RemoveLoading(); }
                catch (Exception ex) { App.Log($"StackTrace: {ex.StackTrace}\nMESSAGE: {ex.Message}"); IsClicked = false; PopupHelper.RemoveLoading(); }
                cts = null;
            });
        }

        public async void OnQRCodeSelected(object obj)
        {
            if (!IsClicked)
            {
                IsClicked = true;
                var qrCode = obj as QrCode;
                INavigationParameters keys = new NavigationParameters();
                keys.Add("QrCode", qrCode);
                await NavigationService.NavigateAsync("QRCodeUsersListPage", keys);
                IsClicked = false;
            }
        }

        public void ReceiveJSONData(JObject jsonData, int wsType)
        {
            if (jsonData.ContainsKey("status") && jsonData["status"].ToObject<int>() == 200)
            {
                switch (wsType)
                {
                    case 0:                       
                        if (jsonData.ContainsKey("entrance") || jsonData.ContainsKey("exit"))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                var entrance = JsonConvert.DeserializeObject<ObservableCollection<QrCode>>(jsonData["entrance"].ToString());
                                QrCodes.Add(new QrCodeGroup("Entrance", entrance));
                                var exit = JsonConvert.DeserializeObject<ObservableCollection<QrCode>>(jsonData["exit"].ToString());
                                QrCodes.Add(new QrCodeGroup("Exit", exit));
                                IsDataLoaded = true;
                            });
                        }
                        break;
                }
            }

            PopupHelper.RemoveLoading();
            IsClicked = false;
        }

        public void ReceiveError(string title, string error, int wsType)
        {
            PageDialogService?.DisplayAlertAsync(title, error, "Okay");
            IsClicked = false;
            PopupHelper.RemoveLoading();
        }
    }
}
