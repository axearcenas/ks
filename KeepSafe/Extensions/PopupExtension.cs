using System;
using System.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace KeepSafe.Extensions
{
    public static class PopupExtension
    {
        public static Task ExtPopModalAsync(this INavigation sender, bool animate = true)
        {
            if (sender.ModalStack.Count > 0)
            {
                return sender.PopModalAsync(animate);
            }
            return Task.Delay(16);
        }

        public static Task ExtPopAllPopupAsync(this INavigation sender, bool animate = true)
        {
            if (PopupNavigation.Instance.PopupStack.Count > 0)
            {
                return PopupNavigation.Instance.PopAllAsync(animate);
            }
            return Task.Delay(16);
        }

        public static Task ExtPopAllExceptPopupAsync(this INavigation sender, Type page, bool animate = true)
        {
            if (PopupNavigation.Instance.PopupStack.Count > 0)
            {
                for (int i = PopupNavigation.Instance.PopupStack.Count - 1; i >= 0; i--)
                {
                    var currentPage = PopupNavigation.Instance.PopupStack[i];
                    if (currentPage.GetType() == page) // Skip
                    {
                        continue;
                    }
                    PopupNavigation.Instance.RemovePageAsync(currentPage);
                }
            }
            return Task.Delay(16);
        }

        public static Task ExtPopPopupAsync(this INavigation sender, bool animate = true)
        {
            if (PopupNavigation.Instance.PopupStack.Count > 0)
            {
                return PopupNavigation.Instance.PopAsync(animate);
            }
            return Task.Delay(16);
        }

        #region PRISM Extension
        public static Task ExtPopModalAsync(this INavigationService sender, bool animate = true)
        {
            if (sender.GetNavigationUriPath().Count( charNav => charNav == '/') > 0)
            {
                return sender.GoBackAsync(animated:animate);
            }
            return Task.Delay(16);
        }

        public static Task ExtPopAllPopupAsync(this INavigationService sender, bool animate = true)
        {
            if (PopupNavigation.Instance.PopupStack.Count > 0)
            {
                return PopupNavigation.Instance.PopAllAsync(animate);
            }
            return Task.Delay(16);
        }

        public static Task ExtPopAllExceptPopupAsync(this INavigationService sender, Type page, bool animate = true)
        {
            if (PopupNavigation.Instance.PopupStack.Count > 0)
            {
                for (int i = PopupNavigation.Instance.PopupStack.Count - 1; i >= 0; i--)
                {
                    var currentPage = PopupNavigation.Instance.PopupStack[i];
                    if (currentPage.GetType() == page) // Skip
                    {
                        continue;
                    }
                    PopupNavigation.Instance.RemovePageAsync(currentPage);
                }
            }
            return Task.Delay(16);
        }

        public static Task ExtPopPopupAsync(this INavigationService sender, bool animate = true)
        {
            if (PopupNavigation.Instance.PopupStack.Count > 0)
            {
                return PopupNavigation.Instance.PopAsync(animate);
            }
            return Task.Delay(16);
        }
        #endregion
    }
}
