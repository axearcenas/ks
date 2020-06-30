using System;
using Prism.Navigation;
using Prism.Services;

namespace KeepSafe.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {
        public ProfilePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
        }
    }
}
