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
            if (!IsClicked)
            {
                IsClicked = true;
                GetHistoryData(10);
            }
        }

        async void GetHistoryData(int offset = 0)
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
                    await fileReader.ReadFile(SelectedHistoryType == 0 ? "CheckInHistory.json" : "CheckOutHistory.json", cts.Token, offset <= 0 ? 0 : 1);
#else
                       //TODO GET HISTORY Rest Here
                       restServices.SetDelegate(this);
                       await restServices.GetRequestAsync($"{Constants.ROOT_URL}?type={(int)SelectedHistoryType}&offset={offset}",  cts.Token, offset <= 0 ? 0 : 1);
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
                        if (jsonData.ContainsKey("histories"))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                MyScanHistory = JsonConvert.DeserializeObject<ObservableCollection<UserScanHistory>>(jsonData["histories"].ToString());

                                PopupHelper.RemoveLoading();
                            });
                        }
                        break;
                    case 1: // histories pagination ( CheckIn and CheckOut)
                        if (jsonData.ContainsKey("histories"))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                var paginationHistory = JsonConvert.DeserializeObject<List<UserScanHistory>>(jsonData["histories"].ToString());
                                foreach(UserScanHistory history in paginationHistory)
                                MyScanHistory.Add(history);

                                PopupHelper.RemoveLoading();
                            });
                        }
                        break;
                }
            }
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
