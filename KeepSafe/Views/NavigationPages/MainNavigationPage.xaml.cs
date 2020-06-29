using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Input;
using KeepSafe.Resources;
using Xamarin.Forms;

namespace KeepSafe
{
    public partial class MainNavigationPage : ContentPage
    {
        public static readonly BindableProperty NavImageProperty = BindableProperty.Create(nameof(NavImage), typeof(string), typeof(MainNavigationPage), "");
        public string NavImage
        {
            set { SetValue(NavImageProperty, value); }
            get { return (string)GetValue(NavImageProperty); }
        }

        public static readonly BindableProperty NavBackgroundColorProperty = BindableProperty.Create(nameof(NavBackgroundColor), typeof(Color), typeof(MainNavigationPage), Color.Transparent);
        public Color NavBackgroundColor
        {
            set { SetValue(NavBackgroundColorProperty, value); }
            get { return (Color)GetValue(NavBackgroundColorProperty); }
        }

        public static readonly BindableProperty NavOpacityProperty = BindableProperty.Create(nameof(NavOpacity), typeof(double), typeof(MainNavigationPage), 1.0);
        public double NavOpacity
        {
            set { SetValue(NavOpacityProperty, value); }
            get { return (double)GetValue(NavOpacityProperty); }
        }

        public static readonly BindableProperty PageTitleProperty = BindableProperty.Create(nameof(PageTitle), typeof(string), typeof(MainNavigationPage), null);
        public string PageTitle
        {
            set { SetValue(PageTitleProperty, value); }
            get { return (string)GetValue(PageTitleProperty); }
        }

        public static readonly BindableProperty TitleVerticalOptionsProperty = BindableProperty.Create(nameof(TitleVerticalOptions), typeof(LayoutOptions), typeof(MainNavigationPage), LayoutOptions.Center);
        public LayoutOptions TitleVerticalOptions
        {
            set { SetValue(TitleVerticalOptionsProperty, value); }
            get { return (LayoutOptions)GetValue(TitleVerticalOptionsProperty); }
        }

        public static readonly BindableProperty TitleHorizontalOptionsProperty = BindableProperty.Create(nameof(TitleHorizontalOptions), typeof(LayoutOptions), typeof(MainNavigationPage), LayoutOptions.Center);
        public LayoutOptions TitleHorizontalOptions
        {
            set { SetValue(TitleHorizontalOptionsProperty, value); }
            get { return (LayoutOptions)GetValue(TitleHorizontalOptionsProperty); }
        }

        public static readonly BindableProperty TitleHeightRequestProperty = BindableProperty.Create(nameof(TitleHeightRequest), typeof(double), typeof(MainNavigationPage), 25.0);
        public double TitleHeightRequest
        {
            set { SetValue(TitleHeightRequestProperty, value); }
            get { return (double)GetValue(TitleHeightRequestProperty); }
        }

        public static readonly BindableProperty TitleFontSizeProperty = BindableProperty.Create(nameof(TitleFontSize), typeof(double), typeof(MainNavigationPage), 17.0);
        public double TitleFontSize
        {
            set { SetValue(TitleFontSizeProperty, value); }
            get { return (double)GetValue(TitleFontSizeProperty); }
        }

        public static readonly BindableProperty TitleFontFamilyProperty = BindableProperty.Create(nameof(TitleFontFamily), typeof(string), typeof(MainNavigationPage), FontResource.REGULAR);
        public string TitleFontFamily
        {
            set { SetValue(TitleFontFamilyProperty, value); }
            get { return (string)GetValue(TitleFontFamilyProperty); }
        }

        public static readonly BindableProperty TitleFontColorProperty = BindableProperty.Create(nameof(TitleFontColor), typeof(Color), typeof(MainNavigationPage), Color.White);
        public Color TitleFontColor
        {
            set { SetValue(TitleFontColorProperty, value); }
            get { return (Color)GetValue(TitleFontColorProperty); }
        }

        public static readonly BindableProperty LeftTextProperty = BindableProperty.Create(nameof(LeftText), typeof(string), typeof(MainNavigationPage), null);
        public string LeftText
        {
            set { SetValue(LeftTextProperty, value); }
            get { return (string)GetValue(LeftTextProperty); }
        }

        public static readonly BindableProperty LeftIconProperty = BindableProperty.Create(nameof(LeftIcon), typeof(string), typeof(MainNavigationPage), null);
        public string LeftIcon
        {
            set { SetValue(LeftIconProperty, value); }
            get { return (string)GetValue(LeftIconProperty); }
        }

        public static readonly BindableProperty LeftIconTypeProperty =
            BindableProperty.Create(
                propertyName: nameof(LeftIconType),
                returnType: typeof(IconType),
                declaringType: typeof(MainNavigationPage),
                defaultValue: IconType.SVG);
        public IconType LeftIconType
        {
            get { return (IconType)GetValue(LeftIconTypeProperty); }
            set { SetValue(LeftIconTypeProperty, value); }
        }

        public static readonly BindableProperty LeftIconColorProperty = BindableProperty.Create(nameof(LeftIconColor), typeof(Color), typeof(MainNavigationPage), Color.Default);
        public Color LeftIconColor
        {
            get { return (Color)GetValue(LeftIconColorProperty); }
            set { SetValue(LeftIconColorProperty, value); }
        }

        public static readonly BindableProperty LeftButtonCommandProperty = BindableProperty.Create(nameof(LeftButtonCommand), typeof(ICommand), typeof(MainNavigationPage), null);
        public ICommand LeftButtonCommand
        {
            set { SetValue(LeftButtonCommandProperty, value); }
            get { return (ICommand)GetValue(LeftButtonCommandProperty); }
        }

        public static readonly BindableProperty LeftButtonColorProperty = BindableProperty.Create(nameof(LeftButtonColor), typeof(Color), typeof(MainNavigationPage), Color.Transparent);
        public Color LeftButtonColor
        {
            set { SetValue(LeftButtonColorProperty, value); }
            get { return (Color)GetValue(LeftButtonColorProperty); }
        }

        public static readonly BindableProperty RightTextProperty = BindableProperty.Create(nameof(RightText), typeof(string), typeof(MainNavigationPage), null);
        public string RightText
        {
            set { SetValue(RightTextProperty, value); }
            get { return (string)GetValue(RightTextProperty); }
        }

        public static readonly BindableProperty RightIconProperty = BindableProperty.Create(nameof(RightIcon), typeof(string), typeof(MainNavigationPage), null);
        public string RightIcon
        {
            set { SetValue(RightIconProperty, value); }
            get { return (string)GetValue(RightIconProperty); }
        }

        public static readonly BindableProperty RightIconTypeProperty =
            BindableProperty.Create(
                propertyName: nameof(RightIconType),
                returnType: typeof(IconType),
                declaringType: typeof(MainNavigationPage),
                defaultValue: IconType.SVG);
        public IconType RightIconType
        {
            get { return (IconType)GetValue(RightIconTypeProperty); }
            set { SetValue(RightIconTypeProperty, value); }
        }

        public static readonly BindableProperty RightIconColorProperty = BindableProperty.Create(nameof(RightIconColor), typeof(Color), typeof(MainNavigationPage), Color.Default);
        public Color RightIconColor
        {
            set { SetValue(RightIconColorProperty, value); }
            get { return (Color)GetValue(RightIconColorProperty); }
        }

        public static readonly BindableProperty RightButtonCommandProperty = BindableProperty.Create(nameof(RightButtonCommand), typeof(ICommand), typeof(MainNavigationPage), null);
        public ICommand RightButtonCommand
        {
            set { SetValue(RightButtonCommandProperty, value); }
            get { return (ICommand)GetValue(RightButtonCommandProperty); }
        }

        public static readonly BindableProperty RightButtonColorProperty = BindableProperty.Create(nameof(RightButtonColor), typeof(Color), typeof(MainNavigationPage), Color.Transparent);
        public Color RightButtonColor
        {
            set { SetValue(RightButtonColorProperty, value); }
            get { return (Color)GetValue(RightButtonColorProperty); }
        }

        public static readonly new BindableProperty BackgroundImageSourceProperty =
            BindableProperty.Create(
                propertyName: nameof(BackgroundImageSource),
                returnType: typeof(ImageSource),
                declaringType: typeof(MainNavigationPage),
                defaultValue: null);

        public new ImageSource BackgroundImageSource
        {
            get { return (ImageSource)GetValue(BackgroundImageSourceProperty); }
            set { SetValue(BackgroundImageSourceProperty, value); }
        }

        public MainNavigationPage()
        {
            InitializeComponent();

            DependencyService.Get<IChangeBarColor>().ChangeColor(BarStyle.Dark);
        }
    }

    public enum IconType
    {
        FontAwesome5Solid,
        FontAwesome5Regular,
        FontAwesome5Brand,
        SVG
    }
}
