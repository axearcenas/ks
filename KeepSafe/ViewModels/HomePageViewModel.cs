using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using KeepSafe.Enum;
using KeepSafe.Helpers;
using KeepSafe.Helpers.FileReader;
using KeepSafe.Models;
using KeepSafe.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace KeepSafe.ViewModels
{
    public class HomePageViewModel : ViewModelBase, IFileConnector, IRestReceiver
    {
        public DelegateCommand<object> HistoryCommand { get; set; }
        public DelegateCommand PaginationCommand { get; set; }
        public DelegateCommand MyQRCommand { get; set; }

        HistoryType _SelectedHistoryType;
        public HistoryType SelectedHistoryType
        {
            get { return _SelectedHistoryType; }
            set { SetProperty(ref _SelectedHistoryType, value, nameof(SelectedHistoryType)); }
        }
        ObservableCollection<UserScanHistory> _MyScanHistory;
        public ObservableCollection<UserScanHistory> MyScanHistory
        {
            get { return _MyScanHistory; }
            set { SetProperty(ref _MyScanHistory, value, nameof(MyScanHistory)); }
        }

        Pagination MyScanHistoryPagination;

        public HomePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            HistoryCommand = new DelegateCommand<object>(OnHistoryCommand_Execute);
            MyQRCommand = new DelegateCommand(OnMyQRCommand_Execute);
            PaginationCommand = new DelegateCommand(OnPaginationCommand_Execute);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            GetHistoryData();
            base.OnNavigatingTo(parameters);
        }

        private async void OnMyQRCommand_Execute()
        {
            if(!IsClicked)
            {
                IsClicked = true;
                await NavigationService.NavigateAsync(nameof(MyQRPage));
                IsClicked = false;
            }
        }

        private void OnHistoryCommand_Execute(object obj)
        {
            if (obj is int intValue)
            {
                SelectedHistoryType = (HistoryType)intValue;
                GetHistoryData();
            }
        }

        private void OnPaginationCommand_Execute()
        {
            //TODO GET HISTORY Rest Here
            if (!IsClicked && MyScanHistoryPagination == null ? false : MyScanHistoryPagination.HasNext )
            {
                IsClicked = true;
                GetHistoryData(true);
            }
        }

        async void GetHistoryData(bool IsPagination = false)
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
                    await fileReader.ReadFile(SelectedHistoryType == 0 ? "CheckInHistory.json" : "CheckOutHistory.json", cts.Token, IsPagination <= 0 ? 0 : 1);
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


        public void ReceiveJSONData(JObject jsonData, int wsType)
        {
            if (jsonData.ContainsKey("status") && jsonData["status"].ToObject<int>() == 200)
            {
                switch (wsType)
                {
                    case 0: // histories ( CheckIn and CheckOut)                        
                        if(jsonData.ContainsKey("pagination"))
                        {
                            MyScanHistoryPagination = JsonConvert.DeserializeObject<Pagination>(jsonData["pagination"].ToString());
                        }
                        if (jsonData.ContainsKey("data"))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                MyScanHistory = JsonConvert.DeserializeObject<ObservableCollection<UserScanHistory>>(jsonData["data"].ToString());

                                PopupHelper.RemoveLoading();
                            });
                        }
                            break;
                    case 1: // histories pagination ( CheckIn and CheckOut)
                        if (jsonData.ContainsKey("pagination"))
                        {
                            MyScanHistoryPagination = JsonConvert.DeserializeObject<Pagination>(jsonData["pagination"].ToString());
                        }
                        if (jsonData.ContainsKey("data"))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                var paginationHistory = JsonConvert.DeserializeObject<List<UserScanHistory>>(jsonData["data"].ToString());
                                foreach(UserScanHistory history in paginationHistory)
                                    MyScanHistory.Add(history);

                                PopupHelper.RemoveLoading();
                            });
                        }
                        break;
                }
            }
            else
            {
                switch (wsType)
                {
                    case 0: // histories ( CheckIn and CheckOut)
                        if (jsonData.ContainsKey("errors"))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                PopupHelper.RemoveLoading();
                                PageDialogService?.DisplayAlertAsync("Error!", jsonData["errors"][0].ToString(), "Okay");
                            });
                        }
                        break;
                    case 1: // histories pagination ( CheckIn and CheckOut)
                        if (jsonData.ContainsKey("errors"))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                PopupHelper.RemoveLoading();
                                PageDialogService?.DisplayAlertAsync("Error!", jsonData["errors"][0].ToString(), "Okay");
                            });
                        }
                        break;
                }
            }
            IsClicked = false;
        }

        public void ReceiveError(string title, string error, int wsType)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                PopupHelper.RemoveLoading();
                PageDialogService?.DisplayAlertAsync(title, error, "Okay");
                IsClicked = false;
            });
        }
    }
}
