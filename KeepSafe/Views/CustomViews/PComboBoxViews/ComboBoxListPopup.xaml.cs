using System;
using System.Collections.Generic;
using System.Linq;
using KeepSafe.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace KeepSafe.Views.CustomViews.PComboBoxViews
{
    public partial class ComboBoxListPopup : RGPopupPage
    {
        public static readonly BindableProperty ComboBoxViewProperty = BindableProperty.Create(nameof(ComboBoxView), typeof(PComboBox), typeof(ComboBoxListPopup), null);

        public PComboBox ComboBoxView
        {
            get { return (PComboBox)GetValue(ComboBoxViewProperty); }
            set { SetValue(ComboBoxViewProperty, value); }
        }

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(ComboBoxListPopup), null);

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly BindableProperty ItemSourceProperty = BindableProperty.Create(nameof(ItemSource), typeof(System.Collections.IEnumerable), typeof(ComboBoxListPopup), null);

        public System.Collections.IEnumerable ItemSource
        {
            get { return (System.Collections.IEnumerable)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(ComboBoxListPopup), Color.Black);

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(nameof(BorderWidth), typeof(Thickness), typeof(ComboBoxListPopup), new Thickness(1.ScaleHeight()));

        public Thickness BorderWidth
        {
            get { return (Thickness)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }




        double HeightSet;

        double WidthSet;
        double VisualElementWidthSet, VisualElementHeightSet;
        Thickness SystemPaddingSet = new Thickness(0, 0, 0, 0);

        public event EventHandler<object> ItemSelected;

        public event EventHandler ListHidden;

        public ComboBoxListPopup(PComboBox visualElement, double widthSet, double heightSet)
        {
            InitializeComponent();
            ComboBoxView = visualElement;
            VisualElementWidthSet = widthSet;
            VisualElementHeightSet = heightSet;
            SetMainViewPosition();

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ListHidden?.Invoke(this, new EventArgs());
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case "ItemTemplate":
                    listView.ItemTemplate = ItemTemplate;
                    break;
                //case "SystemPadding":
                //    SystemPaddingSet = SystemPadding;
                //    SetMainViewPosition();
                //    break;

                default:
                    break;
            }
        }

        void mainView_SizeChanged(System.Object sender, System.EventArgs e)
        {
            if (sender is View view)
            {
                if (view.Width > 0)
                    WidthSet = view.Width;
                if (view.Height > 0)
                    HeightSet = view.Height;
                SetMainViewPosition();
            }
        }

        void SetMainViewPosition()
        {
            var y = ComboBoxView.Y;
            var x = ComboBoxView.X;
            var parent = (ComboBoxView.Parent as VisualElement);
            double marginTop = 0;

            while (parent != null)
            {
                y += parent.Y;
                x += parent.X;
                if (parent.GetType().ToString().Equals("MainNavigationPage"))
                {
                    if (Device.RuntimePlatform == Device.Android) // ¯\_(ツ)_/¯
                        marginTop = Constants.NAVIGATION_HEIGHT;
                    else
                        marginTop = Constants.NAVIGATION_HEIGHT + App.StatusBarHeight;
                }
                else
                if (parent is PopupPage page)
                {
                    if (Device.RuntimePlatform == Device.Android) // ¯\_(ツ)_/¯
                        marginTop = -page.SystemPadding.Top;
                }
                parent = (parent.Parent as VisualElement);
            }

            marginTop += (y + VisualElementHeightSet);
            mainView.Margin = new Thickness(x, marginTop, 0, 25.ScaleHeight());

            if (VisualElementWidthSet > 0)
                mainView.WidthRequest = VisualElementWidthSet;
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            if (sender is View view && view.GestureRecognizers.Any() && view.GestureRecognizers[0] is TapGestureRecognizer tap)
            {
                ItemSelected?.Invoke(this, tap.CommandParameter);
            }
            listView.SelectedItem = null;
            Navigation.ExtPopPopupAsync();
        }

        void listView_ItemSelected(System.Object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            ItemSelected?.Invoke(this, e.SelectedItem);
            listView.SelectedItem = null;
            Navigation.ExtPopPopupAsync();
        }
    }
}
