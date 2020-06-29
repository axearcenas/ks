using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Xamarin.Forms.Button;

namespace KeepSafe.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        void Button_PressedReleased(System.Object sender, System.EventArgs e)
        {
            if (Device.RuntimePlatform == Device.iOS && sender is Button button && button.Parent is View view)
            {
                view.FadeTo(button.IsPressed ? .4 : 1, 50, button.IsPressed ? Easing.CubicOut : Easing.CubicIn);
            }
        }
    }
}
