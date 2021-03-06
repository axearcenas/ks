﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using KeepSafe.Extensions;
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
    public class QRCodeUsersListViewModel : ViewModelBase, IFileConnector, IRestReceiver
    {
        public DelegateCommand BackButtonClickedCommand { get; set; }
        public DelegateCommand SearchCommand { get; set; }

        ObservableCollection<QrCodeScanHistory> _ScanHistory;
        public ObservableCollection<QrCodeScanHistory> ScanHistory
        {
            get { return _ScanHistory; }
            set { SetProperty(ref _ScanHistory, value, nameof(ScanHistory)); }
        }

        QrCode _QrCode;
        public QrCode QrCode
        {
            get { return _QrCode; }
            set { SetProperty(ref _QrCode, value, nameof(QrCode)); }
        }

        public string ButtonText
        {
            get { return QrCode.CodeType == "in" ? "Check In" : "Check Out"; }
        }
        
        public Color ButtonColor
        {
            get { return QrCode.CodeType == "in" ? Color.FromHex("#3498DB") : Color.FromHex("#E74C3C"); }
        }

        public QRCodeUsersListViewModel(INavigationService navigationService) : base(navigationService)
        {
            BackButtonClickedCommand = new DelegateCommand(OnBackButtonClicked);
            SearchCommand = new DelegateCommand(OnSearch);
        }

        string _SearchText;
        public string SearchText
        {
            get { return _SearchText; }
            set { SetProperty(ref _SearchText, value, nameof(SearchText)); }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if(parameters.ContainsKey("QrCode"))
            {
                QrCode = (QrCode)parameters["QrCode"];
            }
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
                    await fileReader.ReadFile("QRCodeUsersListSearch.json", cts.Token, 0);
#else
                //TODO GET HISTORY Rest Here
                restServices.SetDelegate(this);
                await restServices.GetRequest($"{Constants.ROOT_URL}{Constants.QR_CODES_URL}/{QrCode.Id}".AddAuth(), cts.Token, 0);
#endif
                }
                catch (OperationCanceledException ox) { App.Log($"StackTrace: {ox.StackTrace}\nMESSAGE: {ox.Message}"); IsClicked = false; PopupHelper.RemoveLoading(); }
                catch (TimeoutException te) { App.Log($"StackTrace: {te.StackTrace}\nMESSAGE: {te.Message}"); IsClicked = false; PopupHelper.RemoveLoading(); }
                catch (Exception ex) { App.Log($"StackTrace: {ex.StackTrace}\nMESSAGE: {ex.Message}"); IsClicked = false; PopupHelper.RemoveLoading(); }
                cts = null;
            });
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
                    await fileReader.ReadFile("QRCodeUsersListSearch.json", cts.Token, 0);
#else
                    //TODO GET HISTORY Rest Here
                    restServices.SetDelegate(this);
                    await restServices.GetRequest($"{Constants.ROOT_URL}{Constants.QR_CODES_URL}/{QrCode.Id}?search={SearchText}".AddAuth(), cts.Token, 0);
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
                        if (jsonData.ContainsKey("users"))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                if (ScanHistory != null)
                                    ScanHistory = null;
                                ScanHistory = JsonConvert.DeserializeObject<ObservableCollection<QrCodeScanHistory>>(jsonData["users"]["data"].ToString());
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
                                ScanHistory = JsonConvert.DeserializeObject<ObservableCollection<QrCodeScanHistory>>(jsonData["users"]["data"].ToString());
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
