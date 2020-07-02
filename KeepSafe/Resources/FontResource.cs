using System;
using Xamarin.Forms;

namespace KeepSafe.Resources
{
    public class FontResource
    {
        //TODO: ADD THEME FONTS
        public static readonly string REGULAR = Device.RuntimePlatform == Device.iOS ? "Roboto" : "Fonts/Roboto-Regular.ttf#Roboto-Regular";
        public static readonly string BOLD = Device.RuntimePlatform == Device.iOS ? "Roboto-Bold" : "Fonts/Roboto-Bold.ttf#Roboto-Bold";
        public static readonly string LIGHT = Device.RuntimePlatform == Device.iOS ? "Roboto-Light" : "Fonts/Roboto-Light.ttf#Roboto-Light";
        public static readonly string LIGHTITALIC = Device.RuntimePlatform == Device.iOS ? "Roboto-LightItalic" : "Fonts/Roboto-LightItalic.ttf#Roboto-LightItalic";

        public static readonly string BOLDITALIC = Device.RuntimePlatform == Device.iOS ? "OpenSans-BoldItalic" : "Fonts/OpenSans-BoldItalic.ttf#OpenSans-BoldItalic";
        public static readonly string EXTRABOLD = Device.RuntimePlatform == Device.iOS ? "OpenSans-ExtraBold" : "Fonts/OpenSans-ExtraBold.ttf#OpenSans-ExtraBold";
        public static readonly string EXTRABOLDITALIC = Device.RuntimePlatform == Device.iOS ? "OpenSans-ExtraBoldItalic" : "Fonts/OpenSans-ExtraBoldItalic.ttf#OpenSans-ExtraBoldItalic";
        public static readonly string ITALIC = Device.RuntimePlatform == Device.iOS ? "OpenSans-Italic" : "Fonts/OpenSans-Italic.ttf#OpenSans-Italic";
        
        public static readonly string SEMIBOLD = Device.RuntimePlatform == Device.iOS ? "OpenSans-Semibold" : "Fonts/OpenSans-Semibold.ttf#OpenSans-Semibold";
        public static readonly string SEMIBOLDITALIC = Device.RuntimePlatform == Device.iOS ? "OpenSans-SemiboldItalic" : "Fonts/OpenSans-SemiboldItalic.ttf#OpenSans-SemiboldItalic";
        //NOTE: ADDED
        public static readonly string FONTAWESOME = Device.RuntimePlatform == Device.iOS? "FontAwesome" : "Fonts/FontAwesome.ttf#FontAwesome";
        public static readonly string FONTAWESOME5_BRANDREGULAR = Device.RuntimePlatform == Device.iOS? "FontAwesome5Brands-Regular" : "Fonts/FontAwesome5BrandsRegular.otf#FontAwesome";
        public static readonly string FONTAWESOME5_REGULAR = Device.RuntimePlatform == Device.iOS? "FontAwesome5Free-Regular" : "Fonts/FontAwesome5Regular.otf#FontAwesome";
        public static readonly string FONTAWESOME5_SOLID = Device.RuntimePlatform == Device.iOS? "FontAwesome5Free-Solid" : "Fonts/FontAwesome5Solid.otf#FontAwesome";
    }
}
