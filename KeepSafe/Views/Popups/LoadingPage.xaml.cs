using System;
using System.Collections.Generic;
using System.Timers;
using KeepSafe.Helpers;
using Xamarin.Forms;

namespace KeepSafe.Views.Popups
{
    public partial class LoadingPage : RGPopupPage
    {
        Timer timer = new Timer(10000);
        Action _callBack;
        string _message;
        int _timeout;
        public LoadingPage(string message = "", int timeout = 0, Action callback = null)
        {
            InitializeComponent();
            _callBack = callback;
            _timeout = timeout;
            _message = message;
        }
        private void Timer_Elapsed(object sender, EventArgs e)
        {
            PopupHelper.RemoveLoading();
            _callBack?.Invoke();
            //if (!string.IsNullOrWhiteSpace(_message))
            //    ToastServices.Instance.ShowToast(_message);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_timeout > 0)
            {
                timer.Elapsed += Timer_Elapsed;
                timer.Interval = _timeout * 1000;
                timer.Start();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        public void StopLoading()
        {
            timer.Stop();
        }
    }
}
