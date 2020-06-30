using System;
using Prism.Navigation;
using Prism.Services;

namespace KeepSafe.ViewModels
{
    public class HomePageViewModel: ViewModelBase
    {
        public HomePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
        }
    }
}
