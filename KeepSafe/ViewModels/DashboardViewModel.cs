using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using KeepSafe.Helpers;
using KeepSafe.Helpers.FileReader;
using KeepSafe.Models;
using KeepSafe.ViewModels.ViewViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace KeepSafe.ViewModels
{
    public class DashboardViewModel : ViewModelBase, IFileConnector, IRestReceiver, INavigationAware
    {
        public DelegateCommand QRCodeButtonClickedCommand { get; set; }
        public DelegateCommand SearchCommand { get; set; }
        public DelegateCommand FilterButtonClickedCommand { get; set; }

        ObservableCollection<BusinessScanHistory> _ScanHistory;
        public ObservableCollection<BusinessScanHistory> ScanHistory
        {
            get { return _ScanHistory; }
            set { SetProperty(ref _ScanHistory, value, nameof(ScanHistory)); }
        }

        string _SearchText;
        public string SearchText
        {
            get { return _SearchText; }
            set { SetProperty(ref _SearchText, value, nameof(SearchText)); }
        }

        int _CheckInCount;
        public int CheckInCount
        {
            get { return _CheckInCount; }
            set { SetProperty(ref _CheckInCount, value, nameof(CheckInCount)); }
        }

        int _CheckOutCount;
        public int CheckOutCount
        {
            get { return _CheckOutCount; }
            set { SetProperty(ref _CheckOutCount, value, nameof(CheckOutCount)); }
        }

        string _FilterType = "All";
        public string FilterType
        {
            get { return _FilterType; }
            set { SetProperty(ref _FilterType, value, nameof(FilterType)); }
        }

        public DashboardViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            QRCodeButtonClickedCommand = new DelegateCommand(OnQRCodeButtonClicked);
            SearchCommand = new DelegateCommand(OnSearch);
            FilterButtonClickedCommand = new DelegateCommand(OnFilterButtonClicked);
        }

        private void OnFilterButtonClicked()
        {
            var navigationParams = new NavigationParameters
            {
                { "FilterType", FilterType }
            };

            NavigationService.NavigateAsync("ScanHistoryFilter", navigationParams);
        }

        private async void OnSearch()
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
                    await fileReader.ReadFile(string.IsNullOrEmpty(SearchText) ? "EstablishmentScanHistory.json" : "QRCodeUsersListSearch.json", cts.Token, string.IsNullOrEmpty(SearchText) ? 0 : 1);
#else
                //TODO GET HISTORY Rest Here
                restServices.SetDelegate(this);
                await restServices.GetRequest($"{Constants.ROOT_URL}{Constants.USER_URL}{Constants.SCAN_HISTORIES_URL}" ,  cts.Token, 1,Constants.DEFAULT_AUTH);
#endif
                }
                catch (OperationCanceledException ox) { App.Log($"StackTrace: {ox.StackTrace}\nMESSAGE: {ox.Message}"); IsClicked = false; PopupHelper.RemoveLoading(); }
                catch (TimeoutException te) { App.Log($"StackTrace: {te.StackTrace}\nMESSAGE: {te.Message}"); IsClicked = false; PopupHelper.RemoveLoading(); }
                catch (Exception ex) { App.Log($"StackTrace: {ex.StackTrace}\nMESSAGE: {ex.Message}"); IsClicked = false; PopupHelper.RemoveLoading(); }
                cts = null;
            });
        }

        private async void OnQRCodeButtonClicked()
        {
            await NavigationService.NavigateAsync("BusinessQRCodesPage", useModalNavigation: true);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            GetHistoryData();
            if (parameters.ContainsKey("FilterType"))
            {
                FilterType = parameters.GetValue<string>("FilterType");
            }
            base.OnNavigatingTo(parameters);
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        private async void GetHistoryData(int offset = 0)
        {
            PopupHelper.ShowLoading();
            await Task.Delay(1500);
            await Task.Run(async () =>
            {
                if (cts != null)
                    cts.Cancel();
                cts = new System.Threading.CancellationTokenSource();
                try
                {
#if DEBUG
                    fileReader.SetDelegate(this);
                    await fileReader.ReadFile("EstablishmentScanHistory.json", cts.Token, offset <= 0 ? 0 : 1);
#else
                       //TODO GET HISTORY Rest Here
                       restServices.SetDelegate(this);
                       await restServices.GetRequest($"{Constants.ROOT_URL}?type={FilterType}&offset={offset}",  cts.Token, offset <= 0 ? 0 : 1);
#endif
                }
                catch (OperationCanceledException ox) { App.Log($"StackTrace: {ox.StackTrace}\nMESSAGE: {ox.Message}"); IsClicked = false; PopupHelper.RemoveLoading(); }
                catch (TimeoutException te) { App.Log($"StackTrace: {te.StackTrace}\nMESSAGE: {te.Message}"); IsClicked = false; PopupHelper.RemoveLoading(); }
                catch (Exception ex) { App.Log($"StackTrace: {ex.StackTrace}\nMESSAGE: {ex.Message}"); IsClicked = false; PopupHelper.RemoveLoading(); }
                cts = null;
            });
        }

        public void ReceiveJSONData(JObject jsonData, int wsType)
        {
            if (jsonData.ContainsKey("status") && jsonData["status"].ToObject<int>() == 200)
            {
                switch (wsType)
                {
                    case 0:
                        if (jsonData.ContainsKey("data"))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                if (ScanHistory != null)
                                    ScanHistory = null;
                                ScanHistory = JsonConvert.DeserializeObject<ObservableCollection<BusinessScanHistory>>(jsonData["data"].ToString());
                                CheckInCount = (int)jsonData["check_in_count"];
                                CheckOutCount = (int)jsonData["check_out_count"];
                            });
                        }
                        break;
                    case 1:
                        if (jsonData.ContainsKey("users"))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                if (ScanHistory != null)
                                    ScanHistory = null;
                                ScanHistory = JsonConvert.DeserializeObject<ObservableCollection<BusinessScanHistory>>(jsonData["users"].ToString());
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
