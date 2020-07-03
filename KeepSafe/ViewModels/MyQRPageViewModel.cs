using System;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace KeepSafe.ViewModels
{
    public class MyQRPageViewModel : ViewModelBase
    {
        public DelegateCommand BackCommand { get; set; }
        public MyQRPageViewModel(INavigationService navigationService)
            :base(navigationService)
        {
            BackCommand = new DelegateCommand(OnBackCommand_Execute);
        }

        private async void OnBackCommand_Execute()
        {
            if (!IsClicked)
            {
                IsClicked = true;
                await NavigationService.GoBackAsync();
                IsClicked = false;
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            DependencyService.Get<IBrightnessService>().SetBrightness(1); // Full Brightness
            base.OnNavigatedTo(parameters);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            DependencyService.Get<IBrightnessService>().ResetBrightness(); // Restore
            base.OnNavigatedFrom(parameters);
        }
    }
}
