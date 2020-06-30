using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using KeepSafe;
using KeepSafe.Extensions;
using KeepSafe.Helpers;
using KeepSafe.Helpers.FileReader;
using KeepSafe.Models;
using KeepSafe.Rest;
using KeepSafe.ViewModels.ViewViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KeepSafe.ViewModels.PopupsViewModel
{
    public class CustomServerPopupViewModel : ViewModelBase, IFileConnector, IRestReceiver
    {
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<object> CloseCommand { get; set; }
        public DelegateCommand<object> PComboBoxSelectedItemCommand { get; set; }

        public EntryViewModel ApiIpEntry { get; set; } =  new EntryViewModel();
        public EntryViewModel NotificationIpEntry { get; set; } = new EntryViewModel();

        string _ServerName;
        public string ServerName
        {
            get { return _ServerName; }
            set { SetProperty(ref _ServerName, value, nameof(ServerName)); }
        }

        bool _IsSecureServer;
        public bool IsSecureServer
        {
            get { return _IsSecureServer; }
            set { SetProperty(ref _IsSecureServer, value, nameof(IsSecureServer)); }
        }

        ObservableCollection<Server> _Servers;
        public ObservableCollection<Server> Servers
        {
            get { return _Servers; }
            set { SetProperty(ref _Servers, value, nameof(Servers)); }
        }

        //PComboBoxBehaviorRequest _PComboBoxItemSelectionRequest;
        //public PComboBoxBehaviorRequest PComboBoxItemSelectionRequest { get { return _PComboBoxItemSelectionRequest; } set { _PComboBoxItemSelectionRequest = value; OnPropertyChanged(); } }
        public PComboBoxBehaviorRequest PComboBoxItemSelectionRequest { get; set; } = new PComboBoxBehaviorRequest();

        CancellationTokenSource cts;

        int ResendTimes;

        bool isServerListOpened;
#if DEBUG
        RestRequest restServices = new RestRequest();
#else
        FileReader fileReader = FileReader.GetInstance;
        new RestRequest restServices = new RestRequest();
#endif
        public CustomServerPopupViewModel(INavigationService navigationService,IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            SaveCommand = new DelegateCommand<object>(OnSaveCommand_Execute);
            CloseCommand = new DelegateCommand<object>(OnCloseCommand_Execute);
            PComboBoxSelectedItemCommand = new DelegateCommand<object>(OnPComboBoxSelectedItemCommand_Execute);
        }

        private void OnPComboBoxSelectedItemCommand_Execute(object obj)
        {
            if (obj is Server server)
                UpdateUI(server);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);
            GetServerFiles();
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
            cts?.Cancel();
        }

        void GetServerFiles()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
#if DEBUG
                ReadEmbeddedServers();
#else
                PopupHelper.ShowLoading();
                GetServers();
#endif
            }
            else
            {
                ReadEmbeddedServers();
            }
        }

        async void ReadEmbeddedServers(string errorMessage = null)
        {
            if (ResendTimes < 5)
            {
                App.Log("Reading from embedded");
                cts = new CancellationTokenSource();

                try
                {
                    ResendTimes++;
                    fileReader.SetDelegate(this);
                    await fileReader.ReadFile("ServerList.json", cts.Token, 1);
                }
                catch (OperationCanceledException oce)
                {
                    App.Log(oce.StackTrace, "GetRequest - ReadEmbededServers");
                }
                catch (Exception e)
                {
                    App.Log(e.StackTrace, "GetRequest - ReadEmbededServers");
                }
                cts = null;
            }
            else
            {
                await PageDialogService.DisplayAlertAsync("Error Retreiving Server List", errorMessage, "Okay");
            }
        }

        async void GetServers()
        {
            cts = new CancellationTokenSource();

            try
            {
                restServices.SetDelegate( this);
                if (DataClass.GetInstance.CurrentServer.IsSecured)
                {
                    //TODO files for servers secured
                    await restServices.GetRequest("http://demo2630086.mockable.io/server_list", cts.Token, 1);
                }
                else
                {
                    //TODO files for servers not secured
                    await restServices.GetRequest("http://demo2630086.mockable.io/server_list", cts.Token, 1);
                }
            }
            catch (OperationCanceledException oce)
            {
                App.Log(oce.StackTrace, "GetRequest - GetServers");
                ReadEmbeddedServers(oce.StackTrace);
            }
            catch (TimeoutException te)
            {
                App.Log(te.StackTrace, "GetRequest - GetServers");
                ReadEmbeddedServers(te.StackTrace);
            }
            catch (Exception e)
            {
                App.Log(e.StackTrace, "GetRequest - GetServers");
                ReadEmbeddedServers(e.StackTrace);
            }
            cts = null;

            PopupHelper.RemoveLoading();
        }


        private async void OnCloseCommand_Execute(object obj)
        {
            await NavigationService.ExtPopPopupAsync(false);
        }

        private async void OnSaveCommand_Execute(object obj)
        {
            if(obj is PComboBox pComboBox)
            {
                var server = pComboBox.SelectedItems[0] as Server;
                if (server.Name.Equals("Custom Server") && (string.IsNullOrWhiteSpace(ApiIpEntry.Text) ||
                                                            string.IsNullOrWhiteSpace(NotificationIpEntry.Text) ||
                                                            !ApiIpEntry.Text.StartsWith("http") ||
                                                            !NotificationIpEntry.Text.StartsWith("ws")))
                {
                    //ToastServices.Instance.ShowToast("Invalid Server Details");
                    PopupHelper.ShowToast("Invalid Server Details");
                    return;
                }


                if (server.Name == "Custom Server")
                {
                    var customServer = new Server()
                    {
                        Name = server.Name,
                        Api = ApiIpEntry.Text,
                        Notification = NotificationIpEntry.Text,
                        IsSecured = true // isSecureSwitch.IsToggled
                    };
                    DataClass.GetInstance.CurrentServer = customServer;
                }
                else
                {
                    DataClass.GetInstance.CurrentServer = server;
                }

                Constants.ApplyServerSettings();

                await NavigationService.ExtPopPopupAsync(false);
            }
        }

        //async void Save_Clicked(object sender, EventArgs e)
        //{
        //    var server = pComboBox.SelectedItems[0] as Server;
        //    if (server.Name.Equals("Custom Server") && (string.IsNullOrWhiteSpace(apiIPEntry.Text) ||
        //                                                string.IsNullOrWhiteSpace(notificationIPEntry.Text) ||
        //                                                !apiIPEntry.Text.StartsWith("http") ||
        //                                                !notificationIPEntry.Text.StartsWith("ws")))
        //    {
        //        //ToastServices.Instance.ShowToast("Invalid Server Details");
        //        PopupHelper.ShowToast("Invalid Server Details");
        //        return;
        //    }

        //    if (server.Name == "Custom Server")
        //    {
        //        var customServer = new Server()
        //        {
        //            Name = server.Name,
        //            Api = apiIPEntry.Text,
        //            Notification = notificationIPEntry.Text,
        //            IsSecured = true // isSecureSwitch.IsToggled
        //        };
        //        DataClass.GetInstance.CurrentServer = customServer;
        //    }
        //    else
        //    {
        //        DataClass.GetInstance.CurrentServer = server;
        //    }

        //    Constants.ApplyServerSettings();

        //    await NavigationService.ExtPopPopupAsync(false);
        //}

        void UpdateUI(Server server = null)
        {
            if (server.Name.Equals("Custom Server"))
            {
                //piIPLabel.IsVisible = true;
                ApiIpEntry.IsVisible = true;
                //apiLine.IsVisible = true;
                //notificationIPLabel.IsVisible = true;
                //notificationIPEntry.IsVisible = true;
                //notificationLine.IsVisible = true;
                //hint.IsVisible = true;
                //isSecureSwitch.IsVisible = true;
                //isSecureSwitch.IsToggled = false;
                IsSecureServer = false;
            }
            else
            {
                ApiIpEntry.IsVisible = false;
                //apiIPLabel.IsVisible = false;
                //apiIPEntry.IsVisible = false;
                //apiLine.IsVisible = false;
                //notificationIPLabel.IsVisible = false;
                //notificationIPEntry.IsVisible = false;
                //notificationLine.IsVisible = false;
                //hint.IsVisible = false;
                //isSecureSwitch.IsVisible = false;
            }

            //customLabel.Text = server.Name;
            ServerName = server.Name;
        }

        public void ReceiveJSONData(JObject jsonData, int wsType)
        {
            switch (wsType)
            {
                case 1:
                    Servers = JsonConvert.DeserializeObject<ObservableCollection<Server>>(jsonData["servers"].ToString());
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        //comboBox.DataSource = servers;
                        //pComboBox.ItemSource = Servers;

                        var selected = Servers.FirstOrDefault(obj => obj.Name == DataClass.GetInstance.CurrentServer.Name);

                        if (DataClass.GetInstance.CurrentServer.Name.Equals("Custom Server") && selected != null)
                        {
                            selected.Api = DataClass.GetInstance.CurrentServer.Api;
                            selected.Notification = DataClass.GetInstance.CurrentServer.Notification;
                            selected.IsSecured = DataClass.GetInstance.CurrentServer.IsSecured;
                        }

                        //comboBox.SelectedItem = selected ?? servers[0];
                        PComboBoxItemSelectionRequest.ItemSelected(selected ?? Servers[0]);
                        //pComboBox.InvokeSelectedItem(selected ?? Servers[0]);
                    });
                    break;
            }
        }

        public void ReceiveError(string title, string error, int wsType)
        {
            App.Log("[" + title + "]:" + error);
            if (!title.Equals(Constants.NO_INTERNET_TITLE))
            {
                PopupHelper.RemoveLoading(error);
            }
            ReadEmbeddedServers();
        }
    }
}
