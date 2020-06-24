using System;
using System.Collections.Generic;
using System.Windows.Input;
using KeepSafe.Resources;
using KeepSafe.Views.CustomViews.PComboBoxViews;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace KeepSafe
{
    public partial class PComboBox : Frame
    {
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(PComboBox), null);

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly BindableProperty ItemSourceProperty = BindableProperty.Create(nameof(ItemSource), typeof(System.Collections.IEnumerable), typeof(PComboBox), null);

        public System.Collections.IEnumerable ItemSource
        {
            get { return (System.Collections.IEnumerable)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(PComboBox), "");

        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        public static readonly BindableProperty PlaceholderFontSizeProperty = BindableProperty.Create(nameof(PlaceholderFontSize), typeof(double), typeof(PComboBox), 14.ScaleFont());

        public double PlaceholderFontSize
        {
            get { return (double)GetValue(PlaceholderFontSizeProperty); }
            set { SetValue(PlaceholderFontSizeProperty, value); }
        }

        public static readonly BindableProperty PlaceholderFontFamilyProperty = BindableProperty.Create(nameof(PlaceholderFontFamily), typeof(string), typeof(PComboBox), FontResource.REGULAR);

        public string PlaceholderFontFamily
        {
            get { return (string)GetValue(PlaceholderFontFamilyProperty); }
            set { SetValue(PlaceholderFontFamilyProperty, value); }
        }

        public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(PComboBox), ColorResource.LIGHT_GRAY_COLOR);

        public Color PlaceholderColor
        {
            get { return (Color)GetValue(PlaceholderColorProperty); }
            set { SetValue(PlaceholderColorProperty, value); }
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(PComboBox), "");

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(PComboBox), 14.ScaleFont());

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); if (PlaceholderFontSize == (double)PlaceholderFontSizeProperty.DefaultValue) SetValue(PlaceholderFontSizeProperty, value); }
        }

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(PComboBox), FontResource.REGULAR);

        public string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); if (PlaceholderFontFamily == (string)PlaceholderFontFamilyProperty.DefaultValue) SetValue(PlaceholderFontFamilyProperty, value); }
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(PComboBox), ColorResource.BLACK_COLOR);

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); if (PlaceholderColor == (Color)PlaceholderColorProperty.DefaultValue) SetValue(PlaceholderColorProperty, value); }
        }

        public static readonly BindableProperty IsMultipleSelectProperty = BindableProperty.Create(nameof(IsMultipleSelect), typeof(bool), typeof(PComboBox), true);

        public bool IsMultipleSelect
        {
            get { return (bool)GetValue(IsMultipleSelectProperty); }
            set { SetValue(IsMultipleSelectProperty, value); }
        }

        public static readonly BindableProperty HasClearButtonProperty = BindableProperty.Create(nameof(HasClearButton), typeof(bool), typeof(PComboBox), true);

        public bool HasClearButton
        {
            get { return (bool)GetValue(HasClearButtonProperty); }
            set { SetValue(HasClearButtonProperty, value); }
        }

        public static readonly BindableProperty DropdownBorderColorProperty = BindableProperty.Create(nameof(DropdownBorderColor), typeof(Color), typeof(PComboBox), Color.Default);

        public Color DropdownBorderColor
        {
            get { return (Color)GetValue(DropdownBorderColorProperty); }
            set { SetValue(DropdownBorderColorProperty, value); }
        }

        public static readonly BindableProperty DropdownBorderWidthProperty = BindableProperty.Create(nameof(DropdownBorderWidth), typeof(double), typeof(PComboBox), 0D);

        public double DropdownBorderWidth
        {
            get { return (double)GetValue(DropdownBorderWidthProperty); }
            set { SetValue(DropdownBorderWidthProperty, value); }
        }

        public static readonly BindableProperty BorderWidthProperty =
            BindableProperty.Create(
                propertyName: nameof(BorderWidth),
                returnType: typeof(double),
                declaringType: typeof(PComboBox),
                defaultValue: 0D);
        public double BorderWidth
        {
            get { return (double)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }

        public static readonly BindableProperty CustomViewProperty = BindableProperty.Create(
            nameof(CustomView),
            typeof(View),
            typeof(PComboBox),
            defaultValue: null,
            propertyChanged: OnCustomView_PropertyChanged
            );

        private static void OnCustomView_PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            PComboBox pComboBox = bindable as PComboBox;
            if (newValue is View viewValue)
            {
                pComboBox.scrollView.Content = viewValue;

            }
        }

        public View CustomView
        {
            get { return (View)GetValue(CustomViewProperty); }
            set { SetValue(CustomViewProperty, value); }
        }
        /**/

        public static readonly BindableProperty SelectedItemCommandProperty =
        BindableProperty.Create(
            propertyName: nameof(SelectedItemCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(PComboBox),
            defaultValue: null);
        public ICommand SelectedItemCommand
        {
            get { return (ICommand)GetValue(SelectedItemCommandProperty); }
            set { SetValue(SelectedItemCommandProperty, value); }
        }

        public static readonly BindableProperty SelectedItemsClearedCommandProperty =
            BindableProperty.Create(
                propertyName: nameof(SelectedItemsClearedCommand),
                returnType: typeof(ICommand),
                declaringType: typeof(PComboBox),
                defaultValue: null);
        public ICommand SelectedItemsClearedCommand
        {
            get { return (ICommand)GetValue(SelectedItemsClearedCommandProperty); }
            set { SetValue(SelectedItemsClearedCommandProperty, value); }
        }

        public event EventHandler<object> SelectedItem;
        public event EventHandler SelectedItemsCleared;

        List<object> _SelectedItems = null;
        Thickness? defaultCornerRadius = null;
        public List<object> SelectedItems { get { return _SelectedItems; } }
        double HeightSet;
        double WidthSet;
        public PComboBox()
        {
            InitializeComponent();
            CustomView = reasonsComboxValueLabel;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width > 0)
                WidthSet = width;
            if (height > 0)
                HeightSet = height;
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            ShowComboBox();
        }

        public void ShowComboBox()
        {
            var comboBox = new ComboBoxListPopup(this, WidthSet, HeightSet)
            {
                ItemTemplate = ItemTemplate,
                ItemSource = ItemSource,
                BorderColor = DropdownBorderColor == Color.Default ? BorderColor : DropdownBorderColor,
                BorderWidth = DropdownBorderWidth == 0 ? BorderWidth : DropdownBorderWidth
            };
            comboBox.ItemSelected += ComboBox_ItemSelected;
            comboBox.ListHidden += ComboBox_ListHidden;
            if (defaultCornerRadius == null)
                defaultCornerRadius = new Thickness(this.CornerRadius);
            //CornerRadius = new CornerRadius(CornerRadius.TopLeft, CornerRadius.TopRight, 0, 0);
            PopupNavigation.Instance.PushAsync(comboBox);
        }

        private void ComboBox_ListHidden(object sender, EventArgs e)
        {
            //CornerRadius = new CornerRadius(defaultCornerRadius.Value.Left, defaultCornerRadius.Value.Top, defaultCornerRadius.Value.Right, defaultCornerRadius.Value.Bottom);
        }

        public void ClearSelectedItems()
        {
            _SelectedItems?.Clear();
            _SelectedItems = null;
            Text = "";
            reasonPlaceHolder.IsVisible = true;
            SelectedItemsCleared?.Invoke(this, new EventArgs());
            SelectedItemsClearedCommand?.Execute(null);
        }

        private void ComboBox_ItemSelected(object sender, object e)
        {
            if (_SelectedItems == null)
                _SelectedItems = new List<object>();

            if (!_SelectedItems.Contains(e))
            {
                if (e != null)
                {
                    if (!IsMultipleSelect)
                        _SelectedItems.Clear();

                    _SelectedItems.Add(e);
                    if (e is string stringValue)
                    {
                        if (CustomView is Label reasonsComboxValueLabel)
                        {
                            if (string.IsNullOrWhiteSpace(reasonsComboxValueLabel.Text) || !IsMultipleSelect)
                                Text = $"{stringValue}";
                            else
                                Text += $", {stringValue}";
                        }
                    }
                    reasonPlaceHolder.IsVisible = false;
                    SelectedItem?.Invoke(this, e);
                    SelectedItemCommand?.Execute(e);
                }
            }
        }

        void ClearComboBoxValueButton_Clicked(System.Object sender, System.EventArgs e)
        {
            ClearSelectedItems();
        }

        public void InvokeSelectedItem(object e)
        {
            ComboBox_ItemSelected(this, e);
        }

    }
}

