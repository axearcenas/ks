using System;
using FFImageLoading.Svg.Forms;

namespace KeepSafe.Helpers.Extentsions
{
    public static class SVGExtension
    {
        static SvgImageSourceConverter svgImageSourceConverter = new SvgImageSourceConverter();
        public static string SVGPathFormat { get { return "resource://KeepSafe.Resources.SVG.{0}.svg"; } }

        /// <summary>
        /// Gets the svg file path using image name.
        /// <para>and returns:</para>
        /// <para>resource://KeepSafe.Resources.SVG.[imageName].svg</para>
        /// </summary>
        /// <param name="imageName"></param>
        /// <value>resource://KeepSafe.Resources.SVG.[imageName].svg</value>
        /// <returns>resource://KeepSafe.Resources.SVG.[imageName].svg</returns>
        public static string GetSVGPath(this string imageName)
        {
            if (string.IsNullOrEmpty(imageName)) return null;
            return string.Format(SVGPathFormat, imageName);
        }

        public static object GetSVGImageSource(this string imageName)
        {
            if (string.IsNullOrEmpty(imageName)) return null;
            return svgImageSourceConverter.ConvertFromInvariantString(GetSVGPath(imageName));
        }
    }
}
