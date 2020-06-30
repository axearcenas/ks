using System;
using Xamarin.Forms;

namespace KeepSafe
{
    public class CustomTabbedPage : TabbedPage
    {
        public Color SelectedIconColor
        {
            get { return (Color)GetValue(SelectedIconColorProperty); }
            set { SetValue(SelectedIconColorProperty, value); }
        }

        public static readonly BindableProperty SelectedIconColorProperty = BindableProperty.Create(
            nameof(SelectedIconColor),
            typeof(Color),
            typeof(CustomTabbedPage),
            Color.White);

        public Color UnselectedIconColor
        {
            get { return (Color)GetValue(UnelectedIconColorProperty); }
            set { SetValue(UnelectedIconColorProperty, value); }
        }

        public static readonly BindableProperty UnelectedIconColorProperty = BindableProperty.Create(
            nameof(UnselectedIconColor),
            typeof(Color),
            typeof(CustomTabbedPage),
            Color.White);

        public Color SelectedTextColor
        {
            get { return (Color)GetValue(SelectedTextColorProperty); }
            set { SetValue(SelectedTextColorProperty, value); }
        }

        public static readonly BindableProperty SelectedTextColorProperty = BindableProperty.Create(
            nameof(SelectedTextColor),
            typeof(Color),
            typeof(CustomTabbedPage),
            Color.White);

        public Color UnselectedTextColor
        {
            get { return (Color)GetValue(UnselectedTextColorProperty); }
            set { SetValue(UnselectedTextColorProperty, value); }
        }

        public static readonly BindableProperty UnselectedTextColorProperty = BindableProperty.Create(
            nameof(UnselectedTextColor),
            typeof(Color),
            typeof(CustomTabbedPage),
            Color.White);

        public static readonly BindableProperty SelectedIconProperty =
            BindableProperty.Create(
                propertyName: nameof(SelectedIcon),
                returnType: typeof(ImageSource),
                declaringType: typeof(CustomTabbedPage),
                defaultValue: null);

        public ImageSource SelectedIcon
        {
            get { return (ImageSource)GetValue(SelectedIconProperty); }
            set { SetValue(SelectedIconProperty, value); }
        }

        public static readonly BindableProperty UnselectedIconProperty =
            BindableProperty.Create(
                propertyName: nameof(UnselectedIcon),
                returnType: typeof(ImageSource),
                declaringType: typeof(CustomTabbedPage),
                defaultValue: null);

        public ImageSource UnselectedIcon
        {
            get { return (ImageSource)GetValue(UnselectedIconProperty); }
            set { SetValue(UnselectedIconProperty, value); }
        }




        public CustomTabbedPage()
        {
        }
    }
}
