using System;
using KeepSafe.Enum;
using KeepSafe.Resources;
using KeepSafe.Views;
using Prism;
using Prism.Navigation;
using Prism.Navigation.TabbedPages;
using Xamarin.Forms;

namespace KeepSafe.ViewModels
{
    public class MyTabbedPageViewModel : ViewModelBase, IActiveAware, INavigationAware
    {
        public event EventHandler IsActiveChanged;

        bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }
            set { SetProperty(ref _IsActive, value, nameof(IsActive)); RaiseIsActiveChanged(); }
        }

        //UserType _UserType = DataClass.GetInstance.LoginType;
        //public UserType UserType
        //{
        //    get { return _UserType; }
        //    set
        //    {
        //        _UserType = value;
        //        switch (UserType)
        //        {
        //            case UserType.User:
        //                BarBackgroundColor = ColorResource.MAIN_DARK_THEME_COLOR;
        //                UnselectedTabColor = ColorResource.TAB_DEFAULT_TEXTCOLOR;
        //                break;
        //            case UserType.Establishment:
        //                BarBackgroundColor = ColorResource.ESTABLISHMENT_MAIN_THEME_COLOR;
        //                UnselectedTabColor = ColorResource.TAB_ESTABLISHMENT_DEFAULT_TEXTCOLOR;
        //                break;
        //        }
        //        RaiseIsActiveChanged();
        //        RaisePropertyChanged(nameof(UserType));
        //        RaisePropertyChanged(nameof(BarBackgroundColor));
        //        RaisePropertyChanged(nameof(UnselectedTabColor));
        //    }
        //}
        
        
        public Color BarBackgroundColor
        {
            get
            {
                return DataClass.GetInstance.LoginType == UserType.User ? ColorResource.MAIN_DARK_THEME_COLOR : ColorResource.ESTABLISHMENT_MAIN_THEME_COLOR;
            }
        }

        
        public Color UnselectedTabColor
        {
            get
            {
                return DataClass.GetInstance.LoginType == UserType.User ? ColorResource.TAB_DEFAULT_TEXTCOLOR : ColorResource.TAB_ESTABLISHMENT_DEFAULT_TEXTCOLOR;
            }
            
        }

        public MyTabbedPageViewModel(INavigationService navigationService):
            base(navigationService)
        {
            NavigationService.SelectTabAsync(nameof(MainPage));            
        }

        protected virtual void RaiseIsActiveChanged()
        {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }
    }
}
