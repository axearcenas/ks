using System;
using System.Threading;
using System.Threading.Tasks;
using KeepSafe.Extensions;
using KeepSafe.Helpers;
using KeepSafe.Helpers.FileReader;
using KeepSafe.Models;
using KeepSafe.Resources;
using KeepSafe.ViewModels.ViewViewModels;
using KeepSafe.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace KeepSafe.ViewModels
{
    public class UserCheckInPageViewModel : ViewModelBase, IFileConnector, IRestReceiver
    {
        public EntryViewModel TemperatureEntry { get; } = new EntryViewModel() { Placeholder = "Enter your temperature", PlaceholderColor = ColorResource.MAIN_BLUE_COLOR, IsTextAllCaps = true };

        public DelegateCommand BackCommand { get; set; }
        public DelegateCommand TemperatureFocusedCommand { get; set; }
        public DelegateCommand ConfirmCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        bool _IsCheckIn = true;
        public bool IsCheckIn
        {
            get { return _IsCheckIn; }
            set { SetProperty(ref _IsCheckIn, value, nameof(IsCheckIn)); }
        }

        bool _IsConfirmed;
        public bool IsConfirmed
        {
            get { return _IsConfirmed; }
            set { SetProperty(ref _IsConfirmed, value, nameof(IsConfirmed)); }
        }

        Business _ScannedBusiness;
        public Business ScannedBusiness
        {
            get { return _ScannedBusiness; }
            set { SetProperty(ref _ScannedBusiness, value, nameof(ScannedBusiness)); }
        }

        string Qrcode;

        Action<bool> ScanPageActiveAction;

        public UserCheckInPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            BackCommand = new DelegateCommand(OnBackCommand_Execute);
            ConfirmCommand = new DelegateCommand(OnConfirmCommand_Execute);
            CancelCommand = new DelegateCommand(OnCancelCommand_Execute);
            TemperatureFocusedCommand = new DelegateCommand(OnTemperatureFocusedCommand_Execute);
            Title = "Check In";
            TemperatureEntry.PropertyChanged += TemperatureEntry_PropertyChanged;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);
            if (parameters.ContainsKey("IsCheckIn"))
            {
                IsCheckIn = (bool)parameters["IsCheckIn"];
                Title = IsCheckIn ? "Check In" : "Check Out";
                IsConfirmed = !IsCheckIn ;
            }
            if (parameters.ContainsKey("Business"))
            {
                ScannedBusiness = (Business)parameters["Business"];
            }
            if (parameters.ContainsKey("ScanPageActiveAction"))
            {
                ScanPageActiveAction = (Action<bool>)parameters["ScanPageActiveAction"];
            }
            if (parameters.ContainsKey("Qrcode"))
            {
                Qrcode = (string)parameters["Qrcode"];
            }
        }

        private void TemperatureEntry_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals("Text"))
            {
                if(IsCheckIn)
                    IsConfirmed = !string.IsNullOrEmpty(TemperatureEntry.Text);
            }
        }

        private void OnTemperatureFocusedCommand_Execute()
        {
            TemperatureEntry.ToDefaultValue();
        }

        private async void OnBackCommand_Execute()
        {
            if (!IsClicked)
            {
                IsClicked = true;
                await NavigationService.GoBackAsync();
                ScanPageActiveAction?.Invoke(true);
                IsClicked = false;
            }
        }

        private void OnCancelCommand_Execute()
        {
            OnBackCommand_Execute();
        }

        private async void OnConfirmCommand_Execute()
        {
            if(!IsClicked)
            {
                IsClicked = true;
                if (cts != null)
                    cts.Cancel();
                cts = new CancellationTokenSource();
                PopupHelper.ShowLoading();
                //Task.Run(async()=>
                //{
                   try
                   {
#if DEBUG
                        //TODO CHECKIN | CHECKOUT API HERE
                       fileReader.SetDelegate(this);
                        await fileReader.CreateDummyResponse(JsonConvert.SerializeObject(
                           new
                           {
                               status = 200
                           }), cts.Token, 0);
#else
                    restServices.SetDelegate(this);
                    string content = JsonConvert.SerializeObject(new { scan_history = new { code = Qrcode, temperature = TemperatureEntry.Text ?? "0" } });
                    await restServices.PostRequestAsync($"{Constants.ROOT_URL}{Constants.USER_URL}{Constants.SCAN_HISTORIES_URL}", content, cts.Token, 0, Constants.DEFAULT_AUTH);
#endif
                    }
                    catch (OperationCanceledException ox) { App.Log($"StackTrace: {ox.StackTrace}\nMESSAGE: {ox.Message}"); IsClicked = false; IsLoading = false; }
                    catch (TimeoutException te) { App.Log($"StackTrace: {te.StackTrace}\nMESSAGE: {te.Message}"); IsClicked = false; IsLoading = false; }
                    catch (Exception ex) { App.Log($"StackTrace: {ex.StackTrace}\nMESSAGE: {ex.Message}"); IsClicked = false; IsLoading = false; }

                //});
                cts = null;
            }
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
                            PopupHelper.RemoveLoading();
                            await Task.Delay(100);
                            await NavigationService.GoBackAsync();
                            ScanPageActiveAction?.Invoke(true);
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
