using System;
using System.Threading;
using KeepSafe;
using KeepSafe.Helpers.FileReader;
using KeepSafe.Rest;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;

namespace HavaPassenger.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; private set; }
        protected IPageDialogService PageDialogService { get; private set; }


        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        bool _IsLoading;
        public bool IsLoading { get { return _IsLoading; } set { _IsLoading = value; RaisePropertyChanged(nameof(IsLoading)); } }
        bool _IsClicked;
        public bool IsClicked { get { return _IsClicked; } set { _IsClicked = value; RaisePropertyChanged(nameof(IsClicked)); } }

        public DataClass dataClass = DataClass.GetInstance;
        public CancellationTokenSource cts;
#if DEBUG
        public FileReader fileReader = new FileReader();
#else
        public RestRequest restServices = new RestRequest();
#endif

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        public ViewModelBase(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            NavigationService = navigationService;
            PageDialogService = pageDialogService;
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }

        public virtual void OnBackButtonPressed()
        {

        }
    }
}
