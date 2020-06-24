using System;
using Xamarin.Forms;

namespace KeepSafe
{
    public class CustomEntry : Entry
    {
        public static readonly BindableProperty AutoCapitalizationProperty =
            BindableProperty.Create(
                nameof(AutoCapitalization),
                typeof(AutoCapitalizationType),
                typeof(CustomEntry),
                AutoCapitalizationType.None);
        public AutoCapitalizationType AutoCapitalization
        {
            get { return (AutoCapitalizationType)GetValue(AutoCapitalizationProperty); }
            set { SetValue(AutoCapitalizationProperty, value); }
        }

        public static readonly BindableProperty MoveUpProperty =
            BindableProperty.Create(
                nameof(MoveUp),
                typeof(bool),
                typeof(CustomEntry),
                true);
        public bool MoveUp
        {
            get { return (bool)GetValue(MoveUpProperty); }
            set { SetValue(MoveUpProperty, value); }
        }

        public static readonly BindableProperty MoveUpOffsetProperty =
            BindableProperty.Create(
                nameof(MoveUpOffset),
                typeof(double),
                typeof(CustomEntry),
                0D);

        public double MoveUpOffset
        {
            get { return (double)GetValue(MoveUpOffsetProperty); }
            set { SetValue(MoveUpOffsetProperty, value); }
        }

        public static readonly BindableProperty MoveUpAnimationSpeedProperty =
            BindableProperty.Create(
                propertyName: nameof(MoveUpAnimationSpeed),
                returnType: typeof(int),
                declaringType: typeof(CustomEntry),
                defaultValue: 50);
        public int MoveUpAnimationSpeed
        {
            get { return (int)GetValue(MoveUpAnimationSpeedProperty); }
            set { SetValue(MoveUpAnimationSpeedProperty, value); }
        }



        public static readonly BindableProperty CanProceedToViewProperty =
            BindableProperty.Create(
                propertyName: nameof(CanProceedToView),
                returnType: typeof(bool),
                declaringType: typeof(CustomEntry),
                defaultValue: false);
        public bool CanProceedToView
        {
            get { return (bool)GetValue(CanProceedToViewProperty); }
            set { SetValue(CanProceedToViewProperty, value); }
        }



        public delegate void BackspaceEventHandler(object sender, string oldText);

        public event BackspaceEventHandler BackspacePressed;
        public View NextView { get; set; }
        public View PreviousView { get; set; }

        public CustomEntry()
        {
            Completed += CustomEntry_Completed;
        }

        private void CustomEntry_Completed(object sender, EventArgs e)
        {
           if(CanProceedToView)
            {
                NextView?.Focus();
            }
        }

        public void OnBackspacePressed(string oldText)
        {
            if (BackspacePressed != null)
            {
                BackspacePressed(this, oldText);
            }
        }
    }

    public enum AutoCapitalizationType
    {
        Words,
        Sentences,
        None,
        All
    }
}
