﻿using System;
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
        public static readonly Color MAIN_THEME_COLOR = Color.FromHex("#FF0000");
        public static readonly Color MAIN_TEXT_COLOR = Color.FromHex("#64A731");
        public static readonly Color MAIN_BUTTON_COLOR = Color.FromHex("#64A731");
        public static readonly Color DISABLE_BUTTON_COLOR = Color.FromHex("#64A731");
        public static readonly Color PLACEHOLDER_COLOR = Color.FromHex("#64A731");
        public static readonly Color WHITE_COLOR = Color.FromHex("#FFFFFF");
        public static readonly Color BLACK_COLOR = Color.FromHex("#000000");
        public static readonly Color LIGHT_GRAY_COLOR = Color.FromHex("#D3D3D3");
        public static readonly Color GRAY_COLOR = Color.FromHex("#A9A9A9");

        public static readonly Color BORDER_COLOR = Color.FromHex("#D3D3D3");

        public static readonly Color LOADING_BACKGROUNDCOLOR = Color.FromHex("#40000000");

        //lightgray / lightgrey	#D3D3D3	rgb(211,211,211)
        //silver	#C0C0C0	rgb(192,192,192)
        //darkgray / darkgrey	#A9A9A9	rgb(169,169,169)
        //gray / grey	#808080	

    }
}
