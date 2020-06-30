using System;
using System.ComponentModel;
using CoreAnimation;
using CoreGraphics;
using KeepSafe.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(CustomTabbedPage))]

namespace KeepSafe.iOS.Renderers
{
    public class CustomTabbedPage : TabbedRenderer
    {
        public CustomTabbedPage()
        {
        }
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            TabBar.TintColor = UIKit.UIColor.Clear;
        }
        public override void ViewWillAppear(bool animated)
        {
            //if (TabBar?.Items == null) { return; }

            //if (Element is TabbedPage tabs)
            //{
            //    for (int i = 0; i < TabBar.Items.Length; i++)
            //    {
            //        if (tabs.Children[i].IconImageSource is FileImageSource fileImageSource)
            //        {
            //            //disable tint color
            //            TabBar.Items[i].SelectedImage = new UIImage(fileImageSource.File).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
            //        }
            //    }
            //}

            base.ViewWillAppear(animated);
        }
    }
}
