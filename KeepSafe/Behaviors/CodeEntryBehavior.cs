using System;
using Prism.Behaviors;
using Xamarin.Forms;

namespace KeepSafe
{
    public class CodeEntryBehavior : BehaviorBase<CustomEntry>
    {
        protected override void OnAttachedTo(BindableObject bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedObject.TextChanged += AssociatedObject_TextChanged;
            AssociatedObject.BackspacePressed += AssociatedObject_BackspacePressed;
        }

        protected override void OnDetachingFrom(CustomEntry bindable)
        {
            base.OnDetachingFrom(bindable);
            AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
        }

        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!string.IsNullOrEmpty( e.NewTextValue))
                AssociatedObject.NextView?.Focus();
        }

        private void AssociatedObject_BackspacePressed(object sender, string oldText)
        {
            AssociatedObject.PreviousView?.Focus();
        }
    }
}
