using System;
using System.Threading;
using System.Threading.Tasks;
using KeepSafe.Helpers;
using KeepSafe.Helpers.FileReader;
using KeepSafe.Helpers.Permission;
using KeepSafe.Resources;
using KeepSafe.ViewModels.ViewViewModels;
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

        bool _IsScanning;
        public bool IsScanning
        {
            get { return _IsScanning; }
            set { _IsScanning = value; OnPropertyChanged(); }
        }

        bool CanScan;
        public ScanPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            ScanCommand = new DelegateCommand<Result>(OnScanCommand_Execute);
            SearchCommand = new DelegateCommand(OnSearchCommand_Execute);
            SearchEntry.PropertyChanged += SearchEntry_PropertyChanged;
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
            if (!IsClicked && SearchEntry.ValidateIsTextNullOrEmpty())
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
                await Task.Delay(500);
                fileReader.SetDelegate(this);
                //await fileReader.ReadFile("UserProfile.json", cts.Token, 0);
                await fileReader.CreateDummyResponse(JsonConvert.SerializeObject(
                    new
                    {
                        message = $"Scanned: {code}",
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
                        if (jsonData.ContainsKey("message"))
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                //TODO save USER Here
                                PageDialogService?.DisplayAlertAsync("Scan Succesfully", jsonData["message"].ToString(), "Okay");
                            });
                        }
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
