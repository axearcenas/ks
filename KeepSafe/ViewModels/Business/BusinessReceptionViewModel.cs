using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using KeepSafe.Extensions;
using KeepSafe.Helpers;
using KeepSafe.Helpers.FileReader;
using KeepSafe.Models;
using KeepSafe.Resources;
using KeepSafe.ViewModels.ViewViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace KeepSafe.ViewModels
{
    public class BusinessReceptionViewModel : ViewModelBase, IFileConnector, IRestReceiver
    {
        public EntryViewModel TemperatureEntry { get; } = new EntryViewModel() { Placeholder = "Enter user temperature", PlaceholderColor = ColorResource.MAIN_BLUE_COLOR };

        public DelegateCommand<object> TextChangedCommand { get; set; }
        public DelegateCommand CheckInButtonClickedCommand { get; set; }
        public DelegateCommand CheckOutButtonClickedCommand { get; set; }
        public DelegateCommand BackButtonClickedCommand { get; set; }

        User _ScannedUser;
        public User ScannedUser
        {
            get { return _ScannedUser; }
            set { SetProperty(ref _ScannedUser, value, nameof(ScannedUser)); }
        }

        Action<bool> ScanPageActiveAction;
        Action<QrCode> SelectApi;
        ObservableCollection<QrCode> Exit { get; set; }
        ObservableCollection<QrCode> Entrance { get; set; }

        public BusinessReceptionViewModel(INavigationService navigationService) : base(navigationService)
        {
            TextChangedCommand = new DelegateCommand<object>(OnTemperatureTextChanged);
            BackButtonClickedCommand = new DelegateCommand(OnBackButtonClicked);
            CheckInButtonClickedCommand = new DelegateCommand(OnCheckInButtonClicked);
            CheckOutButtonClickedCommand = new DelegateCommand(OnCheckOutButtonClicked);

            SelectApi = new Action<QrCode>(OnSelectApi_Execute);
        }

        private async void OnSelectApi_Execute(QrCode obj)
        {
            PopupHelper.ShowLoading();
            await Task.Run(async () =>
            {
                if (cts != null)
                    cts.Cancel();
                cts = new System.Threading.CancellationTokenSource();
                try
                {
#if DEBUG

                    fileReader.SetDelegate(this);
                    await fileReader.ReadFile("BusinessQRCodes.json", cts.Token, 1);
                    IsClicked = false;
#else
                    restServices.SetDelegate(this);
                    string content = JsonConvert.SerializeObject(
                        new {
                            scan_history = new {
                                qrcode = ScannedUser.Qrcode,
                                temperature = TemperatureEntry.Text ?? "0",
                                qr_code_id = obj.Id
                            }
                        });
                    await restServices.PostRequestAsync($"{Constants.ROOT_URL}{Constants.BUSINESS_URL}{Constants.SCAN_HISTORIES_URL}", content, cts.Token, 1, Constants.DEFAULT_AUTH);
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
                ScanPageActiveAction?.Invoke(true);
                IsClicked = false;
            }
        }

        private void OnTemperatureTextChanged(object sender)
        {
            var e = (TextChangedEventArgs)sender;

            if (e.OldTextValue == null)
                return;

            if (e.OldTextValue.Length < e.NewTextValue.Length && e.NewTextValue.Length == 2)
            {
                TemperatureEntry.Text += ".";
            }

            if (e.NewTextValue.Count(f => f == '.') > 1)
            {
                TemperatureEntry.Text = e.OldTextValue;
            }

            if (e.OldTextValue.Length < e.NewTextValue.Length && e.NewTextValue.Length == 4)
            {
                TemperatureEntry.Text += " \x00B0C";
            }

            if (e.NewTextValue.Count(f => f == ' ') > 1)
            {
                TemperatureEntry.Text = e.OldTextValue;
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);
            if (parameters.ContainsKey("User"))
            {
                ScannedUser = (User)parameters["User"];
            }
            if (parameters.ContainsKey("ScanPageActiveAction"))
            {
                ScanPageActiveAction = (Action<bool>)parameters["ScanPageActiveAction"];
            }
            if(Exit == null && Entrance == null)
                FetchQrCodes();
        }

        async void FetchQrCodes()
        {
            PopupHelper.ShowLoading();
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
                    IsClicked = false;
#else
                    restServices.SetDelegate(this);
                    await restServices.GetRequest($"{Constants.ROOT_URL}{Constants.QR_CODES_URL}", cts.Token, 0, Constants.DEFAULT_AUTH);
#endif
                }
                catch (OperationCanceledException ox) { App.Log($"StackTrace: {ox.StackTrace}\nMESSAGE: {ox.Message}"); IsClicked = false; PopupHelper.RemoveLoading(); }
                catch (TimeoutException te) { App.Log($"StackTrace: {te.StackTrace}\nMESSAGE: {te.Message}"); IsClicked = false; PopupHelper.RemoveLoading(); }
                catch (Exception ex) { App.Log($"StackTrace: {ex.StackTrace}\nMESSAGE: {ex.Message}"); IsClicked = false; PopupHelper.RemoveLoading(); }
                cts = null;
            });
        }

        private async void OnCheckInButtonClicked()
        {
            if (!TemperatureEntry.ValidateIsTextNullOrEmpty("Temperature Is Needed when check in") && !IsClicked)
            {
                IsClicked = true;
                INavigationParameters parameter = new NavigationParameters();
                parameter.Add("IsCheckIn", true);
                parameter.Add("QrCodes", Entrance);
                parameter.Add("SelectApi", SelectApi);
                await NavigationService.NavigateAsync("SelectEntryTypePopup", parameter);
                IsClicked = false;
            }
        }

        private async void OnCheckOutButtonClicked()
        {
            if (!IsClicked)
            {
                IsClicked = true;
                INavigationParameters parameter = new NavigationParameters();
                parameter.Add("IsCheckIn", false);
                parameter.Add("QrCodes", Exit);
                parameter.Add("SelectApi", SelectApi);
                await NavigationService.NavigateAsync("SelectEntryTypePopup", parameter);
                IsClicked = false;
            }
        }

        public void ReceiveJSONData(JObject jsonData, int wsType)
        {
            if (jsonData.ContainsKey("status") && jsonData["status"].ToObject<int>() == 200)
            {
                switch (wsType)
                {
                    case 0: // fetch qr codes
                        if (jsonData.ContainsKey("data"))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                               if(jsonData["data"].ContainsKey("exit"))
                                {
                                    Exit = JsonConvert.DeserializeObject<ObservableCollection<QrCode>>(jsonData["data"]["exit"].ToString());
                                }
                                if (jsonData["data"].ContainsKey("entrance"))
                                {
                                    Entrance = JsonConvert.DeserializeObject<ObservableCollection<QrCode>>(jsonData["data"]["entrance"].ToString());
                                }
                            });
                        }
                        break;
                    case 1: // Selecting Api
                        if (jsonData.ContainsKey("data"))
                        {
                            Device.BeginInvokeOnMainThread( async() =>
                            {
                                PopupHelper.RemoveLoading();
                                await Task.Delay(100);
                                await NavigationService.GoBackAsync();
                                ScanPageActiveAction?.Invoke(true);
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
