using System;
using Prism.Behaviors;
using Xamarin.Forms;

namespace KeepSafe
{
    public class PComboBoxBehavior : BehaviorBase<PComboBox>
    {
        public PComboBoxBehavior()
        {
        }

        public static readonly BindableProperty RequestProperty =
            BindableProperty.Create(
                propertyName: nameof(Request),
                returnType: typeof(PComboBoxBehaviorRequest),
                declaringType: typeof(PComboBoxBehavior),
                defaultValue: null,
                propertyChanged: OnRequestChanged);

        private static void OnRequestChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is PComboBoxBehavior thisBehavior)
            {
                if (oldValue is PComboBoxBehaviorRequest oldRequest)
                {
                    oldRequest.PComboBoxItemSelectionRequest = null;
                }
                if (newValue is PComboBoxBehaviorRequest newRequest)
                {
                    newRequest.PComboBoxItemSelectionRequest = thisBehavior;
                }
            }
        }
        public PComboBoxBehaviorRequest Request
        {
            get { return (PComboBoxBehaviorRequest)GetValue(RequestProperty); }
            set { SetValue(RequestProperty, value); }
        }


    }

    public sealed class PComboBoxBehaviorRequest
    {
        internal PComboBoxBehavior PComboBoxItemSelectionRequest { get; set; }
        public void ItemSelected(object result)
        {
            if (PComboBoxItemSelectionRequest == null) throw new InvalidOperationException("Not binding to ExpandGroupBehavior.");

            PComboBoxItemSelectionRequest.AssociatedObject.InvokeSelectedItem(result);
        }

        public void ItemCleared(object result)
        {
            if (PComboBoxItemSelectionRequest == null) throw new InvalidOperationException("Not binding to ExpandGroupBehavior.");

            PComboBoxItemSelectionRequest.AssociatedObject.InvokeSelectedItem(result);
        }
    }
}
