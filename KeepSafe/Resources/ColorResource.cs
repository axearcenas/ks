using System;
using Xamarin.Forms;

namespace KeepSafe.Resources
{
    public static class ColorResource
    {
        public static T GetThemeValue<T>(this T LightValue, T DarkValue)
        {
            return App.Current.RequestedTheme == OSAppTheme.Dark ? DarkValue == null ?  LightValue : DarkValue : LightValue;
        }

        //COLOR
        public static readonly Color THEME_COLOR = Color.FromHex("#FF0000");
        public static readonly Color TEXT_COLOR = Color.FromHex("#64A731");
        public static readonly Color BUTTON_COLOR = Color.FromHex("#64A731");
        public static readonly Color DISABLE_BUTTON_COLOR = Color.FromHex("#64A731");
        public static readonly Color PLACEHOLDER_COLOR = Color.FromHex("#64A731");
        public static readonly Color WHITE_COLOR = Color.FromHex("#64A731");

    }
}
