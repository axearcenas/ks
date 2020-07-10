using System;
using System.Collections.ObjectModel;
using KeepSafe.Extension;
using KeepSafe.Helpers;

namespace KeepSafe.Models
{
    public class QrCode : BaseNotify , ISelect
    {
        int _Id;
        public int Id
        {
            get { return _Id; }
            set { _Id = value; OnPropertyChanged(); }
        }

        string _Image;
        public string Image
        {
            get { return _Image; }
            set { _Image = value; OnPropertyChanged(); }
        }

        string _Code;
        public string Code
        {
            get { return _Code; }
            set { _Code = value; OnPropertyChanged(); }
        }

        string _CodeType;
        public string CodeType
        {
            get { return _CodeType; }
            set { _CodeType = value; OnPropertyChanged(); }
        }

        int _Count;
        public int Count
        {
            get { return _Count; }
            set { _Count = value; OnPropertyChanged(); }
        }

        string _QrCodeName;
        public string QrCodeName
        {
            get { return _QrCodeName; }
            set { _QrCodeName = value; OnPropertyChanged(); }
        }

        bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; OnPropertyChanged(); }
        }

        public static QrCode Mock()
        {
            string code = RandomizerHelper.GetRandomString(8);
            return new QrCode()
            {
                Id = RandomizerHelper.GetRandomInteger(100),
                Image = RandomizerHelper.GetRandomQRCODE(code),
                Code = code,
                CodeType = RandomizerHelper.GetRandomBoolean() ? "check_out" : "check_in" // check_in
            };
        }
    }

    public class QrCodeGroup : ObservableCollection<QrCode>
    {
        public string Type { get; private set; }

        public QrCodeGroup(string type, ObservableCollection<QrCode> qrCodes) : base(qrCodes)
        {
            Type = type;
        }
    }
}
