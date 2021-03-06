﻿using System;
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
using System.Threading;
using KeepSafe.Enum;

namespace KeepSafe.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public DelegateCommand LogoTapped { get; set; }
        public DelegateCommand ToUserLoginCommand { get; set; }
        public DelegateCommand ToEstablishmentLoginCommand { get; set; }
        public DelegateCommand ToRegistrationCommand { get; set; }

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

        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) :
            base(navigationService, pageDialogService)
        {
            LogoTapped = new DelegateCommand(OnLogoTapped_Execute);
            ToUserLoginCommand = new DelegateCommand(OnToUserLoginCommand_Execute);
            ToEstablishmentLoginCommand = new DelegateCommand(OnEstablishmentClickedCommand_Execute);
            ToRegistrationCommand = new DelegateCommand(OnToRegistrationCommand_Execute);            
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
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

        private async void OnToUserLoginCommand_Execute()
        {
            if (!IsClicked)
            {
                IsClicked = true;
                SellectedType = UserType.User;
                INavigationParameters keys = new NavigationParameters();
                keys.Add("UserType", SellectedType);
                await Task.Delay(16);
                await NavigationService.NavigateAsync(nameof(LoginPage), keys);
                StartTimer();
            }
        }

        private async void OnEstablishmentClickedCommand_Execute()
        {
            if (!IsClicked)
            {
                IsClicked = true;
                SellectedType = UserType.Establishment;
                INavigationParameters keys = new NavigationParameters();
                keys.Add("UserType", SellectedType);
                await Task.Delay(16);
                await NavigationService.NavigateAsync(nameof(LoginPage), keys);
                StartTimer();
            }
        }

        void StartTimer()
        {
            timer = new Timer(Timer_Elapsed,this,1000, 0);
        }

        private void Timer_Elapsed(object state)
        {
            timer.Dispose();
            SellectedType = UserType.None;
            IsClicked = false;
        }

        private void OnToRegistrationCommand_Execute()
        {
            NavigationService.NavigateAsync("RegisterPage");
        }

    }
}
