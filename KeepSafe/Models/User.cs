using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using KeepSafe.Extension;

namespace KeepSafe.Models
{
    public class User : BaseNotify
    {
        int _Id;
        public int Id
        {
            get { return _Id; }
            set { _Id = value; OnPropertyChanged(); }
        }

        string _Code;
        public string Code
        {
            get { return _Code; }
            set { _Code = value; OnPropertyChanged(); }
        }

        string _QrCode;
        public string QrCode
        {
            get { return _QrCode; }
            set { _QrCode = value; OnPropertyChanged(); }
        }
    }
}
