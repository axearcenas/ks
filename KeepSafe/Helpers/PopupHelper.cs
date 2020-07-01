using System;
using System.Linq;
using KeepSafe.Views.Popups;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace KeepSafe.Helpers
{
    public class PopupHelper
    {
        static object RemoveLoadingLock = new object();

        public static void ShowLoading(string message = "", int timeout = 0, Action callback = null)
        {
            lock (RemoveLoadingLock)
            {
                //TODO Add LoadingPage 
                Device.BeginInvokeOnMainThread(() =>
                {
                    var loadingPage = PopupNavigation.Instance.PopupStack.FirstOrDefault(obj => obj is LoadingPage);
                    if (loadingPage == null)
                    {
                        //DataClass.GetInstance.LoadingTimeoutMessage = message;
                        PopupNavigation.Instance.PushAsync(new LoadingPage(message, timeout, callback), false);
                        
                    }
                });
            }
        }

        public static void RemoveLoading(string message = "", bool isBackgroundTransparent = false, bool isTimerOn = true)
        {
            lock (RemoveLoadingLock)
            {
                //TODO Add LoadingPage 
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (PopupNavigation.Instance.PopupStack.Count > 0)
                    {
                        var loadingPage = (LoadingPage)PopupNavigation.Instance.PopupStack.FirstOrDefault(obj => obj is LoadingPage);
                        if (loadingPage != null)
                        {
                            loadingPage.StopLoading();
                            PopupNavigation.Instance.RemovePageAsync(loadingPage, false);
                            //if (!string.IsNullOrWhiteSpace(message))
                            //    ToastServices.Instance.ShowToast(message, isBackgroundTransparent, isTimerOn);
                        }
                    }
                });
            }
        }

        public static void RemoveCustomAlert()
        {
            //TODO Add CustomAlertPopup
            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    var alertPage = PopupNavigation.Instance.PopupStack.FirstOrDefault(obj => obj is CustomAlertPopup);
            //    if (alertPage != null && alertPage is CustomAlertPopup customAlertPopup)
            //    {
            //        PopupNavigation.Instance.RemovePageAsync(customAlertPopup, false);
            //    }
            //});
        }
        //TODO Add CustomMessageModel
        //public static void ShowCustomMessagePopup(CustomMessageModel cstomMessageModel)
        //{
        //    Device.BeginInvokeOnMainThread(() =>
        //    {
        //        var alertPage = (LogInErrorPopUp)PopupNavigation.Instance.PopupStack.FirstOrDefault(obj => obj is LogInErrorPopUp);
        //        if (alertPage == null || (alertPage != null && alertPage.CustomMessageModel.Content == Constants.NO_INTERNET_CONTENT))
        //        {
        //            PopupNavigation.Instance.PushAsync(new LogInErrorPopUp(cstomMessageModel));
        //        }
        //        else
        //        {
        //            alertPage.BindingContext = cstomMessageModel;
        //        }
        //    });

        //    RemoveLoading();
        //}

        //public static void ShowCustomMessagePopup(Action eventHandler, CustomMessageModel cstomMessageModel)
        //{
        //    Device.BeginInvokeOnMainThread(() =>
        //    {
        //        var alertPage = (LogInErrorPopUp)PopupNavigation.Instance.PopupStack.FirstOrDefault(obj => obj is LogInErrorPopUp);
        //        if (alertPage == null || (alertPage != null && alertPage.CustomMessageModel.Content == Constants.NO_INTERNET_CONTENT))
        //        {
        //            PopupNavigation.Instance.PushAsync(new LogInErrorPopUp(eventHandler, cstomMessageModel));
        //        }
        //        else
        //        {
        //            alertPage.BindingContext = cstomMessageModel;
        //        }
        //    });

        //    RemoveLoading();
        //}

        //public static void ShowCustomMessagePopup(Action onButtonClicked, Action onBackgroundClicked, CustomMessageModel cstomMessageModel)
        //{
        //    Device.BeginInvokeOnMainThread(() =>
        //    {
        //        var alertPage = (LogInErrorPopUp)PopupNavigation.Instance.PopupStack.FirstOrDefault(obj => obj is LogInErrorPopUp);
        //        if (alertPage == null || (alertPage != null && alertPage.CustomMessageModel.Content == Constants.NO_INTERNET_CONTENT))
        //        {
        //            PopupNavigation.Instance.PushAsync(new LogInErrorPopUp(onButtonClicked, onBackgroundClicked, cstomMessageModel));
        //        }
        //        else
        //        {
        //            alertPage.BindingContext = cstomMessageModel;
        //        }
        //    });

        //    RemoveLoading();
        //}

        static string OfflineMessage = "You’re using hava offline";
        public static void ShowOffline()
        {
            //TODO Add ToastPopupPage
            //if (PopupNavigation.Instance != null ? !PopupNavigation.Instance.PopupStack.Any((popuppage) => popuppage is ToastPopupPage toastPopup && toastPopup.Message.Equals(OfflineMessage)) : false)
            //{
            //    PopupNavigation.Instance.PushAsync(new ToastPopupPage() { Message = OfflineMessage, MessageBackgroundColor = Constants.OFFLINE_COLOR });
            //}
        }

        public static void HideOffline()
        {
            //TODO Add ToastPopupPage
            //if (PopupNavigation.Instance != null ? PopupNavigation.Instance.PopupStack.Any((popuppage) => popuppage is ToastPopupPage toastPopup && toastPopup.Message.Equals(OfflineMessage)) : false)
            //{
            //    var page = PopupNavigation.Instance.PopupStack.FirstOrDefault((popuppage) => popuppage is ToastPopupPage toastPopup && toastPopup.Message.Equals(OfflineMessage));
            //    PopupNavigation.Instance.RemovePageAsync(page, false);
            //}
        }

        static string OnlineMessage = "You’re back online!";
        public static void ShowOnline()
        {
            //TODO Add ToastPopupPage
            //if (PopupNavigation.Instance != null ? !PopupNavigation.Instance.PopupStack.Any((popuppage) => popuppage is ToastPopupPage toastPopup && toastPopup.Message.Equals(OnlineMessage)) : false)
            //{
            //    PopupNavigation.Instance.PushAsync(new ToastPopupPage(5) { Message = OnlineMessage, MessageBackgroundColor = Constants.ONLINE_COLOR });
            //}
        }

        public static void ShowToast(string message, int timeInSec = 3, bool isBackgroundTransparent = false)
        {
            //TODO Add ToastPopupPage
            //if (PopupNavigation.Instance != null ? !PopupNavigation.Instance.PopupStack.Any((popuppage) => popuppage is ToastPopupPage toastPopup && toastPopup.Message.Equals(OnlineMessage)) : false)
            //{
            //    PopupNavigation.Instance.PushAsync(new ToastPopupPage(timeInSec) { Message = message, MessageBackgroundColor = Constants.MAIN_GRAY_COLOR, BackgroundInputTransparent = isBackgroundTransparent });
            //}
        }

        public static void HideOnline()
        {
            //TODO Add ToastPopupPage
            //if (PopupNavigation.Instance != null ? PopupNavigation.Instance.PopupStack.Any((popuppage) => popuppage is ToastPopupPage toastPopup && toastPopup.Message.Equals(OnlineMessage)) : false)
            //{
            //    var page = PopupNavigation.Instance.PopupStack.FirstOrDefault((popuppage) => popuppage is ToastPopupPage toastPopup && toastPopup.Message.Equals(OnlineMessage));
            //    PopupNavigation.Instance.RemovePageAsync(page, false);
            //}
        }

    }
}
