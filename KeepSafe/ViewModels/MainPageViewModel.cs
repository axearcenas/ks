using System;
using HavaPassenger.ViewModels;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using KeepSafe.Views;
using ImTools;

namespace KeepSafe.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
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

        public MainPageViewModel(INavigationService navigationService,IPageDialogService pageDialogService) :
            base(navigationService, pageDialogService)
        {
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
            TextLabel = TextLabel.Equals("Changed Text: Sample") ? "Welcome to Xamarin.Forms!" : "Changed Text: Hahaha";
        }
    }
}
