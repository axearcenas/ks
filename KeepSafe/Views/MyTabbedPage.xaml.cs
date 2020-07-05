using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace KeepSafe.Views
{
    public partial class MyTabbedPage : TabbedPage
    {
        Page PreviousPage;
        ImageSource PreviousImageSource;

        public MyTabbedPage()
        {
            InitializeComponent();
            //this.ToolbarItems.Add(new ToolbarItem("HOMAGE", "HomeIcon", OnActivated));
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if(propertyName.Equals(nameof(CurrentPage)))
            {
                if (PreviousPage != null && PreviousImageSource != null)
                {
                    PreviousPage.IconImageSource = PreviousImageSource;
                }
                PreviousPage = CurrentPage;
                PreviousImageSource = CurrentPage.IconImageSource;
                if (CurrentPage.IconImageSource is FileImageSource fileImageSource)
                {
                    App.Log($"asdasd {fileImageSource.File}");
                    CurrentPage.IconImageSource = $"Selected{fileImageSource.File}";
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

        }

        private void OnActivated()
        {
            
        }
    }
}
