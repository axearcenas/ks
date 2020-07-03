using System;
using KeepSafe.Models;
using Xamarin.Forms;

namespace KeepSafe.ViewModels.ViewViewModels
{
    public class EntryViewModel : BaseNotify
    {
        string _Text;
        public string Text {
            get { return  _Text; }
            set { SetPropertyChanged(ref _Text, value); }
        }

        string _Placeholder;
        public string Placeholder {
            get { return _Placeholder; }
            set
            {
                if (DefaultPlaceholder == null)
                    DefaultPlaceholder = value;
                SetPropertyChanged(ref _Placeholder, value);
            }
        }

        Color _PlaceholderColor;
        public Color PlaceholderColor {
            get { return _PlaceholderColor; }
            set {
                _PlaceholderColor = value;
                if (DefaultColor == null)
                    DefaultColor = value;
                OnPropertyChanged(); }
        }

        bool _IsVisible;
        public bool IsVisible {
            get { return _IsVisible; }
            set { SetPropertyChanged(ref _IsVisible, value); }
        }

        bool _IsPassword;
        public bool IsPassword
        {
            get { return _IsPassword; }
            set { SetPropertyChanged(ref _IsPassword, value, nameof(IsPassword)); }
        }

        public bool IsTextAllCaps { get; set; }


        Color? DefaultColor;
        string DefaultPlaceholder;
        bool IsError;
        public EntryViewModel()
        {
        }

        public bool IsTextNullOrEmpty()
        {
            return string.IsNullOrEmpty(Text) || string.IsNullOrWhiteSpace(Text);
        }

        public bool ValidateIsTextNullOrEmpty(string errorMessage = null, Color? errorColor = null)
        {
            if(string.IsNullOrEmpty(Text) || string.IsNullOrWhiteSpace(Text))
            {
                ShowError(errorMessage, errorColor);
                return true;
            }
            Placeholder = DefaultPlaceholder;
            PlaceholderColor = DefaultColor ?? Color.Black;
            return false;
        }

        public void ShowError(string errorMessage = null, Color? errorColor = null)
        {
            Text = string.Empty;
            Placeholder = errorMessage;
            PlaceholderColor = errorColor ?? Color.Red;
        }

        public void ToDefaultValue(string TextValue = null)
        {
            Text = string.IsNullOrEmpty(Text) ? TextValue : Text;
            Placeholder = DefaultPlaceholder;
            PlaceholderColor = DefaultColor ?? Color.Black;
        }

        public void ClearText()
        {
            Text = string.Empty;
        }
    }
}
