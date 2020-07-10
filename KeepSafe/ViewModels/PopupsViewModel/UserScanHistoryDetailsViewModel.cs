using System;
using KeepSafe.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace KeepSafe.ViewModels.PopupsViewModel
{
    public class UserScanHistoryDetailsViewModel : ViewModelBase
    {
        public DelegateCommand OkayCommand { get; set; }
        UserScanHistory _ScanHistoryData;
        public UserScanHistory ScanHistoryData
        {
            get { return _ScanHistoryData; }
            set { SetProperty(ref _ScanHistoryData, value, nameof(ScanHistoryData)); }
        }
        public UserScanHistoryDetailsViewModel(INavigationService navigationService): base(navigationService)
        {
            OkayCommand = new DelegateCommand(OnOkayCommand_Execute);
        }

        private async void OnOkayCommand_Execute()
        {
            if(!IsClicked)
            {
                IsClicked = true;
                await NavigationService.GoBackAsync();
                IsClicked = false;
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if(parameters.ContainsKey("ScanHistoryDetails"))
            {
                ScanHistoryData = parameters.GetValue<UserScanHistory>("ScanHistoryDetails");
            }
        }
    }
}
