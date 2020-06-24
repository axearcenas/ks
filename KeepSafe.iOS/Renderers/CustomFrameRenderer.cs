using System;
using System.ComponentModel;
using CoreAnimation;
using CoreGraphics;
using KeepSafe.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Frame), typeof(CustomFrameRenderer))]
namespace KeepSafe.iOS.Renderers
{
    public class CustomFrameRenderer : FrameRenderer
    {

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null && Element != null)
            {
                Element.PropertyChanged -= OnControlPropertyChanged;
            }

            if (e.NewElement != null && NativeView != null)
            {
                //Control = e.NewElement;
                Element.PropertyChanged += OnControlPropertyChanged;

                UpdateCorners();
            }
        }

        protected void UpdateCorners()
        {
            NativeView.Layer.AllowsEdgeAntialiasing = true;
            NativeView.Layer.MasksToBounds = Element.IsClippedToBounds;
            NativeView.Layer.CornerRadius = Element.CornerRadius;

            //left side
            //var path = UIBezierPath.FromRoundedRect(NativeView.Bounds, UIRectCorner.TopLeft | UIRectCorner.BottomLeft, new CGSize(20, 20));
            //NativeView.Layer.Mask = new CAShapeLayer() { Path = path.CGPath };
            ////right side
            //NativeView.Layer.AllowsEdgeAntialiasing = true;
            //NativeView.ClipsToBounds = Element.IsClippedToBounds;
            //NativeView.Layer.CornerRadius = Element.CornerRadius * 4;
            //NativeView.Layer.MaskedCorners = CACornerMask.MaxXMaxYCorner | CACornerMask.MaxXMinYCorner;
        }

        private void OnControlPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (NativeView == null || Element == null)
                return;

            if (e.PropertyName == "IsClippedToBounds"
                || e.PropertyName == "CornerRadius")
            {
                UpdateCorners();
            }
        }
    }
}