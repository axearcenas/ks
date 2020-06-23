using System;
using KeepSafe;

namespace KeepSafe.Models
{
    public class Server : BaseNotify
    {
        string _Name;
        public string Name { get { return _Name; } set { _Name = value; OnPropertyChanged(nameof(Name)); } }

        string _Api;
        public string Api { get { return _Api; } set { _Api = value; OnPropertyChanged(nameof(Api)); } }

        string _Notification;
        public string Notification { get { return _Notification; } set { _Notification = value; OnPropertyChanged(nameof(Notification)); } }

        bool _IsSecured;
        public bool IsSecured { get { return _IsSecured; } set { _IsSecured = value; OnPropertyChanged(nameof(IsSecured)); } }

        bool _IsLocal;
        public bool IsLocal { get { return _IsLocal; } set { _IsLocal = value; OnPropertyChanged(nameof(IsLocal)); } }
    }
}
