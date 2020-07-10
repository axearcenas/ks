using System;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KeepSafe.ViewModels
{
    public class DashboardAlertViewModel : ViewModelBase
    {
        public DelegateCommand GoButtonClickedCommand { get; set; }

        public DashboardAlertViewModel(INavigationService navigationService) : base(navigationService)
        {
            GoButtonClickedCommand = new DelegateCommand(OnGoButtonClicked);
        }

        public async void OnGoButtonClicked()
        {
            await Browser.OpenAsync("https://www.keepsafe.ph/", BrowserLaunchMode.External);
        }
    }
}

