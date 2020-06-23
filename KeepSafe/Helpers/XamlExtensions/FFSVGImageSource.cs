using System;
using Xamarin.Forms.Xaml;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;
using System.Reflection;
using KeepSafe.Helpers.Extentsions;

namespace KeepSafe
{
    public class FFSVGImageSource : IMarkupExtension
    {
        static SvgImageSourceConverter svgImageSourceConverter = new SvgImageSourceConverter();
        public string ImageName { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!string.IsNullOrEmpty(ImageName))
            {
                return ImageName.GetSVGPath();
            }
            return "";
        }

        public static object GetImageSource(string ImageName)
        {
            return ImageName.GetSVGImageSource();
        }
    }

    public class FFSVGBindingImageSource : IMarkupExtension<BindingBase>
    {
        static SvgImageSourceConverter svgImageSourceConverter = new SvgImageSourceConverter();
        public string BindingImageName { get; set; }
        public bool IsTemplateBinding { get; set; } = false;

        public BindingBase ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding
            {
                Path = BindingImageName,
                Mode = BindingMode.OneWay,
                StringFormat = SVGExtension.SVGPathFormat
            };

            if (IsTemplateBinding)
                binding.Source = RelativeBindingSource.TemplatedParent;

            return binding;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }
    }
}
