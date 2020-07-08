using System;
using System.Collections.ObjectModel;
using KeepSafe.Models;
using Prism.Navigation;

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

        public ObservableCollection<QrCode> QrCodes { get { return DataClass.GetInstance.Business.QrCode; } }

        public SelectEntryTypeViewModel(INavigationService navigationService) : base(navigationService)
        {

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("IsCheckIn"))
            {
                IsCheckIn = (bool)parameters["IsCheckIn"];
                EntryType = IsCheckIn ? "Entrance" : "Exit";
            }
        }
    }
}
