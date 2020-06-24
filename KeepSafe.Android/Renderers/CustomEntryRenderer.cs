using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Util;
using Android.Views;
using KeepSafe;
using KeepSafe.Droid.Renderers;
using ImTools;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace KeepSafe.Droid.Renderers
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context)
        {

        }

        double? keyboardAndViewYDiff;
        double? superParentViewY;
        CustomEntry entry;
        string lastText;
        VisualElement superView;
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            entry = (CustomEntry)this.Element;

            if (this.Control != null)
            {
                Control.SetPadding(0, 0, 0, 0);
                this.Control.HorizontalScrollBarEnabled = false;
                this.Control.VerticalScrollBarEnabled = false;
                this.Control.SetBackgroundColor(Android.Graphics.Color.Transparent);

                if (entry.Keyboard != Keyboard.Telephone && entry.Keyboard != Keyboard.Numeric)
                {
                    switch (entry.AutoCapitalization)
                    {
                        case AutoCapitalizationType.Words:
                            Control.InputType = InputTypes.TextFlagCapWords;
                            break;
                        case AutoCapitalizationType.Sentences:
                            Control.InputType = InputTypes.TextFlagCapSentences;
                            break;
                        case AutoCapitalizationType.All:
                            Control.InputType = InputTypes.TextFlagCapCharacters;
                            break;
                        case AutoCapitalizationType.None:
                            break;
                    }
                }
            }

            if (e.NewElement != null)
            {
                if (superParentViewY == null)
                {
                    superParentViewY = GetSuperParentView().Y;
                }

                if (superView == null)
                {
                    superView = GetSuperParentView();
                }

                MainActivity.GlobalLayout += Vto_GlobalLayout;
                entry.TextChanged += Entry_TextChanged;
                Element.Unfocused += Element_Unfocused;
                Element.Focused += Element_Focused; ;
            }

            if (e.OldElement != null)
            {
                UnsubsribeEvents();
            }
        }

        void UnsubsribeEvents()
        {
            try
            {
                MainActivity.GlobalLayout -= Vto_GlobalLayout;
                //Element.Unfocused -= Element_Unfocused;
                superView = null;
            }
            catch (Exception e)
            {
                App.Log(e.Message + " " + e.StackTrace);
            }
        }

        private void Element_Unfocused(object sender, FocusEventArgs e)
        {
            //if (!Control.IsFocused && entry.IsForcedHideKeyboard)
            //{
            //    InputMethodManager imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
            //    imm.HideSoftInputFromWindow(this.WindowToken, Android.Views.InputMethods.HideSoftInputFlags.None);
            //}
        }

        private void Element_Focused(object sender, FocusEventArgs e)
        {
            if (entry.MoveUp)
                CalculateKeyboardHeight(entry.MoveUpOffset);
        }

        public void Vto_GlobalLayout()
        {
            if (entry.MoveUp)
                CalculateKeyboardHeight(entry.MoveUpOffset);
        }

        async void CalculateKeyboardHeight(double offSet = 0)
        {
            try
            {
                App.Log("CalculateKeyboardHeight");
                Rect r = new Rect();
                var controlLocation = new int[2];

                Control.GetWindowVisibleDisplayFrame(r);
                Control.GetLocationOnScreen(controlLocation);

                DisplayMetrics dm = Control.Resources.DisplayMetrics;
                double viewPointY = controlLocation[1] + Control.Height + (offSet * dm.Density);

                if (r.Bottom < dm.HeightPixels)
                {
                    if (keyboardAndViewYDiff == null)
                    {
                        keyboardAndViewYDiff = (r.Bottom - viewPointY) / dm.Density;
                    }
                    if (keyboardAndViewYDiff != null && Control.IsFocused)
                    {
                        var moveDistance = superParentViewY.Value + keyboardAndViewYDiff.Value;
                        //var moveDistance = superParentViewY.Value + ((r.Bottom - viewPointY) / dm.Density);
                        if (superView.TranslationY != moveDistance)
                        {
                            await superView.TranslateTo(superView.TranslationX, Math.Min(moveDistance, superParentViewY.Value), (uint)entry.MoveUpAnimationSpeed);
                        }
                    }
                }
                else
                {
                    await superView.TranslateTo(superView.TranslationX, superParentViewY.Value, (uint)entry.MoveUpAnimationSpeed);
                }
            }
            catch (Exception ex)
            {
                App.Log($"WARNING: The PageWithKeyboardObserverEffect should be removed from a page that is not currently visible\nSTACKTRACE =>{ex.StackTrace}\nMESSAGE => {ex.Message}");
            }
        }

        private void Entry_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            lastText = e.NewTextValue;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            entry = (CustomEntry)this.Element;
            if (e.PropertyName.Equals(CustomEntry.MoveUpOffsetProperty.PropertyName))
            {
                keyboardAndViewYDiff = null;
                if (entry.MoveUp)
                    CalculateKeyboardHeight(entry.MoveUpOffset);
            }
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (e.KeyCode == Keycode.Del && e.Action == KeyEventActions.Up)
            {
                entry.OnBackspacePressed(lastText);
                if (!string.IsNullOrEmpty(lastText))
                    lastText = lastText[lastText.Length - 1].ToString();
            }
            return base.DispatchKeyEvent(e);
        }

        VisualElement GetSuperParentView()
        {
            var parent = (Element as VisualElement);
            while (parent.Parent is VisualElement visualElement)
            {
                parent = visualElement;
            }
            return parent;
        }

        protected override void Dispose(bool disposing)
        {
            UnsubsribeEvents();
            base.Dispose(disposing);
        }
    }
}
