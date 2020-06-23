using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace KeepSafe.Models
{
    public class BaseNotify : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void SetPropertyChanged<T>(ref T oldValue,T newValue, [CallerMemberName] string propertyName = "")
        {
            if (oldValue == null ? true: !oldValue.Equals(newValue))
            {
                oldValue = newValue;
                OnPropertyChanged(propertyName);
            }
        }
    }
}