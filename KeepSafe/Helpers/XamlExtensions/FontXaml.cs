//using System;
//using Xamarin.Forms;
//using Xamarin.Forms.Xaml;

//namespace KeepSafe
//{
//    public class FontXaml : IMarkupExtension
//    {
//        public Fonts Value { get; set; } 

//        public object ProvideValue(IServiceProvider serviceProvider)
//        {
//            switch(Value)
//            {
//                case Fonts.FontAwesome: return Device.RuntimePlatform == Device.iOS ? "FontAwesome" : "Fonts/FontAwesome.ttf#FontAwesome";
//                case Fonts.FontAwesome5BrandsRegular: return Device.RuntimePlatform == Device.iOS ? "FontAwesome5Brands-Regular" : "Fonts/FontAwesome5BrandsRegular.otf#FontAwesome";
//                case Fonts.FontAwesome5Regular: return Device.RuntimePlatform == Device.iOS ? "FontAwesome5Free-Regular" : "Fonts/FontAwesome5Regular.otf#FontAwesome";
//                case Fonts.FontAwesome5Solid: return Device.RuntimePlatform == Device.iOS ? "FontAwesome5Free-Solid" : "Fonts/FontAwesome5Solid.otf#FontAwesome";
//                case Fonts.MontserratBold: return Device.RuntimePlatform == Device.iOS ? "Montserrat-Bold" : "Fonts/Montserrat-Bold.ttf#Montserrat-Bold";
//                case Fonts.MontserratRegular: return Device.RuntimePlatform == Device.iOS ? "Montserrat-Regular" : "Fonts/Montserrat-Regular.ttf#Montserrat-Regular";
//                case Fonts.MontserratLight: return Device.RuntimePlatform == Device.iOS ? "Montserrat-Light" : "Fonts/Montserrat-Light.otf#Montserrat-Light";
//                case Fonts.MontserratLightItalic: return Device.RuntimePlatform == Device.iOS ? "Montserrat-LightItalic" : "Fonts/Montserrat-LightItalic.otf#Montserrat-LightItalic";
//            }
//            return null;
//        }

//        public static string GetFont(Fonts font)
//        {
//            switch (font)
//            {
//                case Fonts.FontAwesome: return Device.RuntimePlatform == Device.iOS ? "FontAwesome" : "Fonts/FontAwesome.ttf#FontAwesome";
//                case Fonts.FontAwesome5BrandsRegular: return Device.RuntimePlatform == Device.iOS ? "FontAwesome5BrandsRegular" : "Fonts/FontAwesome5BrandsRegular.otf#FontAwesome";
//                case Fonts.FontAwesome5Regular: return Device.RuntimePlatform == Device.iOS ? "FontAwesome5Regular" : "Fonts/FontAwesome5Regular.otf#FontAwesome";
//                case Fonts.FontAwesome5Solid: return Device.RuntimePlatform == Device.iOS ? "FontAwesome5Solid" : "Fonts/FontAwesome5Solid.otf#FontAwesome";
//                case Fonts.MontserratBold: return Device.RuntimePlatform == Device.iOS ? "Montserrat-Bold" : "Fonts/Montserrat-Bold.ttf#Montserrat-Bold";
//                case Fonts.MontserratRegular: return Device.RuntimePlatform == Device.iOS ? "Montserrat-Regular" : "Fonts/Montserrat-Regular.ttf#Montserrat-Regular";
//                case Fonts.MontserratLight: return Device.RuntimePlatform == Device.iOS ? "Montserrat-Light" : "Fonts/Montserrat-Light.otf#Montserrat-Light";
//                case Fonts.MontserratLightItalic: return Device.RuntimePlatform == Device.iOS ? "Montserrat-LightItalic" : "Fonts/Montserrat-LightItalic.otf#Montserrat-LightItalic";
//            }
//            return null;
//        }
//    }

//    public enum Fonts 
//    {
//        FontAwesome,
//        FontAwesome5BrandsRegular,
//        FontAwesome5Regular,
//        FontAwesome5Solid,
//        MontserratBold,
//        MontserratRegular,
//        MontserratLight,
//        MontserratLightItalic
//    }
//}
