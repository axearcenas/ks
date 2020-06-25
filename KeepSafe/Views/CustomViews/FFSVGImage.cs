using System;
using FFImageLoading.Svg.Forms;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using Xamarin.Forms;

namespace KeepSafe
{
    public class FFSVGImage : SvgCachedImage
    {
        public static readonly BindableProperty TintProperty = BindableProperty.Create(nameof(Tint), typeof(Color), typeof(FFSVGImage), Color.Default, BindingMode.TwoWay, propertyChanged: OnPropertyChange);
        public Color Tint
        {
            get { return (Color)GetValue(TintProperty); }
            set { SetValue(TintProperty, value); }
        }
        static void OnPropertyChange(BindableObject bindable, object oldColor, object newColor)
        {
            var oldcolor = (Color)oldColor;
            var color = (Color)newColor;
            var view = (FFSVGImage)bindable;

            if (color.Equals(Color.Default))
            {
                view.Transformations = null;
            }
            else
            {
                if (!oldcolor.Equals(color))
                {
                    var transformations = new System.Collections.Generic.List<ITransformation>()
                    {
                        new TintTransformation(String.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", (int)(color.A * 255), (int)(color.R * 255), (int)(color.G * 255), (int)(color.B * 255)))
                        {
                            EnableSolidColor = view.IsSolidTint
                        }
                    };
                    view.Transformations = transformations;
                }
            }
        }

        public static readonly BindableProperty IsSolidTintProperty = BindableProperty.Create(nameof(IsSolidTint), typeof(bool), typeof(FFSVGImage), true, BindingMode.TwoWay);
        public bool IsSolidTint
        {
            get { return (bool)GetValue(IsSolidTintProperty); }
            set { SetValue(IsSolidTintProperty, value); }
        }
        public FFSVGImage()
        {
        }
    }
}
