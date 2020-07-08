using System;
using KeepSafe.Helpers;

namespace KeepSafe.Models
{
    public class QrCode : BaseNotify
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
}
