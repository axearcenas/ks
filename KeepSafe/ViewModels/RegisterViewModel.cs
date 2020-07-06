using System;
using System.Threading;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;

namespace KeepSafe.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        public DelegateCommand UserButtonClickedCommand { get; set; }
        public DelegateCommand EstablishmentButtonClickedCommand { get; set; }
        public DelegateCommand LoginLabelTappedCommand { get; set; }

        UserType _SellectedType = UserType.None;
        public UserType SellectedType
        {
            get { return _SellectedType; }
            set { SetProperty(ref _SellectedType, value, nameof(SellectedType)); }
        }

        int _TapCount;
        public int TapCount
        {
            get { return _TapCount; }
            set { SetProperty(ref _TapCount, value, nameof(TapCount)); }
        }

        Timer timer;

        public RegisterViewModel(INavigationService navigationService) : base(navigationService)
        {
            UserButtonClickedCommand = new DelegateCommand(OnUserButtonClicked);
            EstablishmentButtonClickedCommand = new DelegateCommand(OnEstablishmentButtonClicked);
            LoginLabelTappedCommand = new DelegateCommand(OnLoginLabelTapped);
        }

        private async void OnUserButtonClicked()
        {
            if (!IsClicked)
            {
                IsClicked = true;
                SellectedType = UserType.User;
                await Task.Delay(16);
                await NavigationService.NavigateAsync(nameof(RegisterUserPage));
                StartTimer();
            }
        }

        private async void OnEstablishmentButtonClicked()
        {
            if (!IsClicked)
            {
                IsClicked = true;
                SellectedType = UserType.Establishment;
                await NavigationService.NavigateAsync(nameof(RegisterBusinessPage));
                StartTimer();
            }
        }

        void StartTimer()
        {
            timer = new Timer(Timer_Elapsed, this, 1000, 0);
        }

        private void Timer_Elapsed(object state)
        {
            timer.Dispose();
            SellectedType = UserType.None;
            IsClicked = false;
        }

        private void OnLoginLabelTapped()
        {
            NavigationService.GoBackAsync();
        }

    }
}
