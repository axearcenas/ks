using System;
using System.Collections.ObjectModel;
using KeepSafe.Extension;
using KeepSafe.Models;
using Prism.Commands;
using Prism.Navigation;
using Prism.Navigation.Xaml;

namespace KeepSafe.ViewModels
{
    public class SelectEntryTypeViewModel : ViewModelBase
    {
        bool _IsCheckIn = true;
        public bool IsCheckIn
        {
            get { return _IsCheckIn; }
            set { SetProperty(ref _IsCheckIn, value, nameof(IsCheckIn)); }
        }

        string _EntryType;
        public string EntryType
        {
            get { return _EntryType; }
            set { SetProperty(ref _EntryType, value, nameof(EntryType)); }
        }

        ObservableCollection<QrCode> _QrCodes;
        public ObservableCollection<QrCode> QrCodes
        {
            get { return _QrCodes; }
            set { SetProperty(ref _QrCodes, value, nameof(QrCodes)); }
        }
        Action<QrCode> SelectApi;

        public DelegateCommand<object> SelectedQrCodeCommand { get; set; }

        public SelectEntryTypeViewModel(INavigationService navigationService) : base(navigationService)
        {
            SelectedQrCodeCommand = new DelegateCommand<object>(OnSelectedQrCodeCommand_Execute);
        }

        private async void OnSelectedQrCodeCommand_Execute(object obj)
        {
            if(obj is QrCode qrCode && !IsClicked)
            {
                QrCodes.SelectItemById(qrCode.Id);
                IsClicked = true;
                await NavigationService.GoBackAsync();
                SelectApi?.Invoke(qrCode);
                qrCode.IsSelected = false;
                IsClicked = false;
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("IsCheckIn"))
            {
                IsCheckIn = (bool)parameters["IsCheckIn"];
                EntryType = IsCheckIn ? "Entrance" : "Exit";
            }
            if (parameters.ContainsKey("QrCodes"))
            {
                QrCodes = parameters.GetValue<ObservableCollection<QrCode>>("QrCodes");
            }
            if (parameters.ContainsKey("SelectApi"))
            {
                SelectApi = parameters.GetValue<Action<QrCode>>("SelectApi");
            }
        }
    }
}
