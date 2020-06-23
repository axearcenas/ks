using System;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace KeepSafe
{
    public static class ScaleCS
    {
        /// <summary>
        /// Scales the height.
        /// </summary>
        /// <returns>The height.</returns>
        /// <param name="number">Number.</param>
        /// <param name="iOS">Value for iOS.</param>
		public static float ScaleHeight(this int number, int? iOS = null)
        {
            if (iOS.HasValue && Device.RuntimePlatform == Device.iOS)
                number = iOS.Value;

            return (float)(number * (App.ScreenHeight / 568.0));
        }

        /// <summary>
        /// Scales the height.
        /// </summary>
        /// <returns>The height.</returns>
        /// <param name="number">Number.</param>
        /// <param name="iOS">Value for iOS.</param>
        public static float ScaleHeight(this double number, double? iOS = null)
        {
            if (iOS.HasValue && Device.RuntimePlatform == Device.iOS)
                number = iOS.Value;

            return (float)(number * (App.ScreenHeight / 568.0));
        }

        /// <summary>
        /// Scales the height.
        /// </summary>
        /// <returns>The height.</returns>
        /// <param name="number">Number.</param>
        /// <param name="iOS">Value for iOS.</param>
        public static float ScaleHeight(this float number, float? iOS = null)
        {
            if (iOS.HasValue && Device.RuntimePlatform == Device.iOS)
                number = iOS.Value;

            return (float)(number * (App.ScreenHeight / 568.0));
        }

        /// <summary>
        /// Scales the width.
        /// </summary>
        /// <returns>The width.</returns>
        /// <param name="number">Number.</param>
        /// <param name="iOS">Value for iOS.</param>
        public static float ScaleWidth(this int number, int? iOS = null)
        {
            if (iOS.HasValue && Device.RuntimePlatform == Device.iOS)
                number = iOS.Value;

            return (float)(number * (App.ScreenWidth / 320.0));
        }

        /// <summary>
        /// Scales the width.
        /// </summary>
        /// <returns>The width.</returns>
        /// <param name="number">Number.</param>
        /// <param name="iOS">Value for iOS.</param>
        public static float ScaleWidth(this double number, double? iOS = null)
        {
            if (iOS.HasValue && Device.RuntimePlatform == Device.iOS)
                number = iOS.Value;

            return (float)(number * (App.ScreenWidth / 320.0));
        }

        /// <summary>
        /// Scales the width.
        /// </summary>
        /// <returns>The width.</returns>
        /// <param name="number">Number.</param>
        /// <param name="iOS">Value for iOS.</param>
        public static float ScaleWidth(this float number, float? iOS = null)
        {
            if (iOS.HasValue && Device.RuntimePlatform == Device.iOS)
                number = iOS.Value;

            return (float)(number * (App.ScreenWidth / 320.0));
        }

        /// <summary>
        /// Scales the font.
        /// </summary>
        /// <returns>The font.</returns>
        /// <param name="number">Number.</param>
        /// <param name="iOS">Value for iOS.</param>
        public static double ScaleFont(this int number, int? iOS = null)
        {
            if (iOS.HasValue && Device.RuntimePlatform == Device.iOS)
                number = iOS.Value;

            //return (number * (App.ScreenHeight / 568.0) - (Device.RuntimePlatform == Device.iOS ? 0.5 : 0));
            return (number * GetMinSize() - (Device.RuntimePlatform == Device.iOS ? 0.5 : 0));
        }

        /// <summary>
        /// Format the chips to : [ 1,000 -> 1K | 1,500 -> 1.5K | 1,000,000 -> 1M | 1,000,000,000 -> 1B]
        /// </summary>
        /// <returns>Formatted chips</returns>
        /// <param name="number">Number.</param>
        /// <param name="iOS">Value for iOS.</param>
        public static double ScaleFont(this double number, double? iOS = null)
        {
            if (iOS.HasValue && Device.RuntimePlatform == Device.iOS)
                number = iOS.Value;

            //return (number * (App.ScreenHeight / 568.0) - (Device.RuntimePlatform == Device.iOS ? 0.5 : 0));
            return (number * GetMinSize() - (Device.RuntimePlatform == Device.iOS ? 0.5 : 0));
        }

        public static float ScaleHeightResponsive(this int number, int? iOS = null)
        {
            if (iOS.HasValue && Device.RuntimePlatform == Device.iOS)
                number = iOS.Value;

            return (float)(number * (App.OriginalHeight / 568.0));
        }

        public static float ScaleHeightResponsive(this double number, double? iOS = null)
        {
            if (iOS.HasValue && Device.RuntimePlatform == Device.iOS)
                number = iOS.Value;

            return (float)(number * (App.OriginalHeight / 568.0));
        }

        public static float ScaleWidthResponsive(this int number, int? iOS = null)
        {
            if (iOS.HasValue && Device.RuntimePlatform == Device.iOS)
                number = iOS.Value;

            return (float)(number * (App.OriginalWidth / 320.0));
        }

        public static float ScaleWidthResponsive(this double number, double? iOS = null)
        {
            if (iOS.HasValue && Device.RuntimePlatform == Device.iOS)
                number = iOS.Value;

            return (float)(number * (App.OriginalWidth / 320.0));
        }

        public static float ScaleWidthResponsive(this float number, float? iOS = null)
        {
            if (iOS.HasValue && Device.RuntimePlatform == Device.iOS)
                number = iOS.Value;

            return (float)(number * (App.OriginalWidth / 320.0));
        }

        public static Thickness ScaleThickness(this Thickness number, Thickness? iOS = null)
        {
            if (iOS.HasValue && Device.RuntimePlatform == Device.iOS)
                number = iOS.Value;
            number.Left = number.Left.ScaleWidth();
            number.Top = number.Top.ScaleHeight();
            number.Right = number.Right.ScaleWidth();
            number.Bottom = number.Bottom.ScaleHeight();
            return number;
        }

        public static CornerRadius ScaleCornerRadius(this CornerRadius number, CornerRadius? iOS = null)
        {
            if (iOS.HasValue && Device.RuntimePlatform == Device.iOS)
                number = iOS.Value;

            return new CornerRadius(number.TopLeft.ScaleHeight(), number.TopRight.ScaleHeight(), number.BottomLeft.ScaleHeight(), number.BottomRight.ScaleHeight());
        }

        public static Thickness ScaleThicknessResponsive(this Thickness number, Thickness? iOS = null)
        {
            if (iOS.HasValue && Device.RuntimePlatform == Device.iOS)
                number = iOS.Value;
            number.Left = number.Left.ScaleWidthResponsive();
            number.Top = number.Top.ScaleHeightResponsive();
            number.Right = number.Right.ScaleWidthResponsive();
            number.Bottom = number.Bottom.ScaleHeightResponsive();
            return number;
        }

        public static CornerRadius ScaleCornerRadiusResponsive(this CornerRadius number, CornerRadius? iOS = null)
        {
            if (iOS.HasValue && Device.RuntimePlatform == Device.iOS)
                number = iOS.Value;

            return new CornerRadius(number.TopLeft.ScaleHeightResponsive(), number.TopRight.ScaleHeightResponsive(), number.BottomLeft.ScaleHeightResponsive(), number.BottomRight.ScaleHeightResponsive());
        }

        public static Thickness ScaleThicknessWidth(this Thickness number, Thickness? iOS = null)
        {
            if (iOS.HasValue && Device.RuntimePlatform == Device.iOS)
                number = iOS.Value;
            number.Left = number.Left.ScaleWidth();
            number.Top = number.Top.ScaleWidth();
            number.Right = number.Right.ScaleWidth();
            number.Bottom = number.Bottom.ScaleWidth();
            return number;
        }

        public static double GetMinSize()
        {
            return Math.Min((App.ScreenHeight / 568.0), (App.ScreenWidth / 320.0));
        }
    }
}
