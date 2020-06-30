using System;
using KeepSafe.Views;
using Prism;
using Prism.Navigation;
using Prism.Navigation.TabbedPages;
namespace KeepSafe.ViewModels
{
    public class MyTabbedPageViewModel : ViewModelBase , IActiveAware
    {

        public MyTabbedPageViewModel(INavigationService navigationService):
            base(navigationService)
        {
            NavigationService.SelectTabAsync(nameof(MainPage));
        }

        bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }
            set { SetProperty(ref _IsActive, value, nameof(IsActive)); RaiseIsActiveChanged(); }
        }
        
        public event EventHandler IsActiveChanged;


        protected virtual void RaiseIsActiveChanged()
        {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
