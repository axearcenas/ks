using System;
using System.Globalization;
using System.Linq;
using KeepSafe.Helpers.FileReader;
using KeepSafe.Models;
using KeepSafe.Resources;
using KeepSafe.ViewModels.ViewViewModels;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace KeepSafe.ViewModels
{
    public class BusinessReceptionViewModel : ViewModelBase, IFileConnector, IRestReceiver
    {
        public EntryViewModel TemperatureEntry { get; } = new EntryViewModel() { Placeholder = "Enter user temperature", PlaceholderColor = ColorResource.MAIN_BLUE_COLOR };

        public DelegateCommand<object> TextChangedCommand { get; set; }

        User _ScannedUser;
        public User ScannedUser
        {
            get { return _ScannedUser; }
            set { SetProperty(ref _ScannedUser, value, nameof(ScannedUser)); }
        }

        public BusinessReceptionViewModel(INavigationService navigationService) : base(navigationService)
        {
            TextChangedCommand = new DelegateCommand<object>(OnTemperatureTextChanged);
        }        

        private void OnTemperatureTextChanged(object sender)
        {
            var e = (TextChangedEventArgs)sender;

            if (e.OldTextValue == null)
                return;

            if (e.OldTextValue.Length < e.NewTextValue.Length && e.NewTextValue.Length == 2)
            {
                TemperatureEntry.Text += ".";
            }

            if (e.NewTextValue.Count(f => f == '.') > 1)
            {
                TemperatureEntry.Text = e.OldTextValue;
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);
            if (parameters.ContainsKey("User"))
            {
                ScannedUser = (User)parameters["User"];
            }
        }

        public void ReceiveError(string title, string error, int wsType)
        {
            
        }

        public void ReceiveJSONData(JObject jsonData, int wsType)
        {
            
        }
    }
}
