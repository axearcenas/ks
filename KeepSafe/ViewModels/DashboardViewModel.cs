using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using KeepSafe.Helpers;
using KeepSafe.Helpers.FileReader;
using KeepSafe.Models;
using KeepSafe.ViewModels.ViewViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace KeepSafe.ViewModels
{
    public class DashboardViewModel : ViewModelBase, IFileConnector, IRestReceiver, INavigationAware , IActiveAware
    {
        public DelegateCommand QRCodeButtonClickedCommand { get; set; }
        public DelegateCommand SearchCommand { get; set; }
        public DelegateCommand FilterButtonClickedCommand { get; set; }
        public DelegateCommand ScanHistoryListTreshHoldCommand { get; set; }

        Pagination ScanHistoryPagination;

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

        public event EventHandler IsActiveChanged;

        public string FilterType
        {
            get { return _FilterType; }
            set { SetProperty(ref _FilterType, value, nameof(FilterType)); }
        }



        bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }
            set { SetProperty(ref _IsActive, value, nameof(IsActive)); RaiseIsActiveChanged(); }
        }

        protected virtual async void RaiseIsActiveChanged()
        {
            if(IsActive)
                GetHistoryData();
        }

        public DashboardViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            QRCodeButtonClickedCommand = new DelegateCommand(OnQRCodeButtonClicked);
            SearchCommand = new DelegateCommand(OnSearch);
            FilterButtonClickedCommand = new DelegateCommand(OnFilterButtonClicked);
            ScanHistoryListTreshHoldCommand = new DelegateCommand(OnScanHistoryListTreshHoldCommand_Execute);
        }

        private void OnScanHistoryListTreshHoldCommand_Execute()
        {
            if(!IsClicked && ScanHistoryPagination != null && ScanHistoryPagination.HasNext)
            {
                IsClicked = true;
                GetHistoryData(true);
            }
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
            if(!IsClicked)
            {
                IsClicked = true;
                GetHistoryData();
            }
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

        private async void GetHistoryData(bool isPagination = false)
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
                    string type = FilterType.ToLower().Equals("all") ? "" : $"type={(FilterType.Equals("Check Ins") ? 1 : 2)}&";
                    string search = string.IsNullOrEmpty(SearchText) ? "" : $"search={SearchText.Trim()}";
                    string url = $"{Constants.ROOT_URL}{Constants.BUSINESS_URL}{Constants.SCAN_HISTORIES_URL}?{type}{search}";
                    await restServices.GetRequest( isPagination ? $"{Constants.ROOT_URL}{ScanHistoryPagination?.Url}" : url,  cts.Token, isPagination ? 1 : 0,Constants.DEFAULT_AUTH);
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
                    case 0: // fetch data or for search
                        if (jsonData.ContainsKey("data"))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                if (ScanHistory != null)
                                    ScanHistory = null;
                                ScanHistory = JsonConvert.DeserializeObject<ObservableCollection<BusinessScanHistory>>(jsonData["data"].ToString());
                                CheckInCount = (int)jsonData["check_in_count"];
                                CheckOutCount = (int)jsonData["check_out_count"];
                                if (jsonData.ContainsKey("pagination"))
                                    ScanHistoryPagination = JsonConvert.DeserializeObject<Pagination>(jsonData["pagination"].ToString());
                            });
                        }
                        break;
                    case 1: // for pagination
                        if (jsonData.ContainsKey("data"))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                if (jsonData.ContainsKey("data"))
                                {
                                    var scanHistory = JsonConvert.DeserializeObject<ObservableCollection<BusinessScanHistory>>(jsonData["data"].ToString());
                                    foreach (var scanData in scanHistory)
                                    {
                                        ScanHistory.Add(scanData);
                                    }
                                }
                                CheckInCount = (int)jsonData["check_in_count"];
                                CheckOutCount = (int)jsonData["check_out_count"];
                                if (jsonData.ContainsKey("pagination"))
                                    ScanHistoryPagination = JsonConvert.DeserializeObject<Pagination>(jsonData["pagination"].ToString());
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
