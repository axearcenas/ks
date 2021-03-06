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
        public static readonly Color MAIN_THEME_COLOR = Color.FromHex("#EE3E5B");
        public static readonly Color MAIN_DARK_THEME_COLOR = Color.FromHex("#5D172C");
        public static readonly Color MAIN_BLUE_COLOR = Color.FromHex("#05257A");

        public static readonly Color SCANNER_BACKGROUNDCOLOR = Color.FromHex("#804456");

        
        public static readonly Color ESTABLISHMENT_MAIN_THEME_COLOR = Color.FromHex("#22144D");

        public static readonly Color LIST_CARD_BACKGROUNDCOLOR = Color.FromHex("#885967");
        public static readonly Color LIST_CARD_ICON_BACKGROUNDCOLOR = Color.FromHex("#64293B");

        public static readonly Color MAIN_BUTTON_COLOR = Color.FromHex("#DE3344");
        public static readonly Color MAIN_TEXT_COLOR = Color.FromHex("#64A731");
        public static readonly Color MAIN_BLACK_COLOR = Color.FromHex("#2F353D");

        public static readonly Color DISABLE_BUTTON_COLOR = Color.FromHex("#64A731");
        public static readonly Color PLACEHOLDER_COLOR = Color.FromHex("#777E91");// HINT TEXT

        public static readonly Color WHITE_COLOR = Color.FromHex("#FFFFFF");
        public static readonly Color BLACK_COLOR = Color.FromHex("#000000");
        public static readonly Color LIGHT_GRAY_COLOR = Color.FromHex("#DEDEDE");
        public static readonly Color GRAY_COLOR = Color.FromHex("#A9A9A9");

        public static readonly Color BORDER_COLOR = Color.FromHex("#D3D3D3");

        public static readonly Color LOADING_BACKGROUNDCOLOR = Color.FromHex("#40000000");

        public static readonly Color TAB_SELECTED_ICONCOLOR = Color.FromHex("#E6455B");
        public static readonly Color TAB_DEFAULT_ICONCOLOR = Color.FromHex("#702D41");
        public static readonly Color TAB_DEFAULT_TEXTCOLOR = Color.FromHex("#D290A4");
        public static readonly Color TAB_SELECTED_TEXTCOLOR = Color.FromHex("#FFFFFF");

        public static readonly Color PROFILE_ENTRY_COLOR = Color.FromHex("#8859676B");
        public static readonly Color PROFILE_BACKGROUND_COLOR = Color.FromHex("#804456");
        public static readonly Color TAB_ESTABLISHMENT_DEFAULT_TEXTCOLOR = Color.FromHex("#7FB2D4");

    }
}
