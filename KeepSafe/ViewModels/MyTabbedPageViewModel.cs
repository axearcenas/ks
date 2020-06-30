using System;
using Prism.Navigation;

namespace KeepSafe.ViewModels
{
    public class MyTabbedPageViewModel : ViewModelBase
    {
        public MyTabbedPageViewModel(INavigationService navigationService):
            base(navigationService)
        {
        }
    }
}
