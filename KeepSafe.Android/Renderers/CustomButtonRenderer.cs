using System;
using KeepSafe.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Views;
using Android.Content;

[assembly: ExportRenderer(typeof(Button), typeof(CustomButtonRenderer))]
namespace KeepSafe.Droid.Renderers
{
    public class CustomButtonRenderer : ButtonRenderer
    {
        public CustomButtonRenderer(Context context) : base(context)
        {

        }

        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            base.OnDraw(canvas);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            Control.SetPadding(0, 0, 0, 0);
            Control.SetAllCaps(false);
        }
    }
}
