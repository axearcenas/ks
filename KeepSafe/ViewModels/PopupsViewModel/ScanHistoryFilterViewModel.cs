using System;
using System.Runtime.CompilerServices;
using KeepSafe.Extensions;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace KeepSafe.ViewModels.PopupsViewModel
{
    public class ScanHistoryFilterViewModel : ViewModelBase, INavigationAware
    {
        public DelegateCommand<object> FilterOptionClickedCommand { get; set; }

        string _FilterType;
        public string FilterType
        {
            get { return _FilterType; }
            set { SetProperty(ref _FilterType, value, nameof(FilterType)); }
        }

        public ScanHistoryFilterViewModel(INavigationService navigationService) : base(navigationService)
        {
            FilterOptionClickedCommand = new DelegateCommand<object>(OnFilterOptionClicked);
        }

        private async void OnFilterOptionClicked(object obj)
        {
            switch((obj as Button).Text)
            {
                case "All":
                    FilterType = "All";
                    break;
                case "Check Ins":
                    FilterType = "Check Ins";
                    break;
                case "Check Outs":
                    FilterType = "Check Outs";
                    break;
            }

            var navigationParams = new NavigationParameters
            {
                { "FilterType", FilterType }
            };

            await NavigationService.GoBackAsync(navigationParams);
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            FilterType = parameters.GetValue<string>("FilterType");
        }
    }
}
