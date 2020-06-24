using System;
using KeepSafe.ViewModels;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using KeepSafe.Views;
using ImTools;
using KeepSafe.Helpers;
using System.Threading.Tasks;
using KeepSafe.Views.Popups;
using Rg.Plugins.Popup.Services;
using Prism.Common;
using KeepSafe.ViewModels.PopupsViewModel;

namespace KeepSafe.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public DelegateCommand LogoTapped { get; set; }
        public DelegateCommand ChangeLabelCommand { get; set; }
        public DelegateCommand ShowDisplayAlertCommand { get; set; }
        public DelegateCommand ChangePageCommand { get; set; }
        public DelegateCommand BackCommand { get; set; }

        string _TextLabel = "Welcome to Xamarin.Forms!";
        public string TextLabel
        {
            get { return _TextLabel; }
            set { SetProperty(ref _TextLabel, value, nameof(TextLabel)); }
        }

        bool _ShowPreviousButton;
        public bool ShowPreviousButton
        {
            get { return _ShowPreviousButton; }
            set { SetProperty(ref _ShowPreviousButton, value, nameof(ShowPreviousButton)); }
        }

        int _TapCount;
        public int TapCount
        {
            get { return _TapCount; }
            set { SetProperty(ref _TapCount, value, nameof(TapCount)); }
        }

        public MainPageViewModel(INavigationService navigationService,IPageDialogService pageDialogService) :
            base(navigationService, pageDialogService)
        {
            LogoTapped = new DelegateCommand(OnLogoTapped_Execute);
            ChangeLabelCommand = new DelegateCommand(OnChangeLabelCommand_Execute);
            ShowDisplayAlertCommand = new DelegateCommand(OnShowDisplayAlertCommand_Execute);
            ChangePageCommand = new DelegateCommand(OnChangePageCommand_Execute);
            BackCommand = new DelegateCommand(OnBackCommand_Execute);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
            if (parameters.ContainsKey("ShowPreviousButton"))
            {
                ShowPreviousButton = (bool)parameters["ShowPreviousButton"];
            }
        }

        private void OnBackCommand_Execute()
        {
            NavigationService.GoBackAsync();
        }

        private void OnChangePageCommand_Execute()
        {
            INavigationParameters parameters = new NavigationParameters();
            parameters.Add(nameof(ShowPreviousButton), true);

            NavigationService.NavigateAsync(nameof(MainPage), parameters);
        }

        private void OnShowDisplayAlertCommand_Execute()
        {
            PageDialogService.DisplayAlertAsync("Display Alert Sample","This is how to display alert","Okay");
        }

        private void OnChangeLabelCommand_Execute()
        {
            TextLabel = TextLabel.Equals("Changed Text: Sample") ? "Welcome to Xamarin.Forms!" : "Changed Text: Sample";
        }

        private async void OnLogoTapped_Execute()
        {
            if (TapCount < 9)
            {
                TapCount++;
            }
            else
            {
                if (!IsClicked)
                {
                    App.Log($"LOGO TAPPED");
                    IsClicked = true;
                    PopupHelper.ShowLoading();
                    await NavigationService.NavigateAsync(nameof(CustomServerPopup));
                    await Task.Delay(100);
                    IsClicked = false;
                    TapCount = 0;
                }
            }
        }
    }
}
