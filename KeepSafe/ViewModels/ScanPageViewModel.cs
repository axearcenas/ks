using System;
using Prism.Navigation;
using Prism.Services;

namespace KeepSafe.ViewModels
{
    public class ScanPageViewModel : ViewModelBase
    {
        public ScanPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
        }
    }
}
