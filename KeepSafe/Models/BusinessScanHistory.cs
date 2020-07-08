using System;
using Xamarin.Forms;

namespace KeepSafe.Models
{
    public class BusinessScanHistory : BaseNotify
    {
        int _Id;
        public int Id
        {
            get { return _Id; }
            set { _Id = value; OnPropertyChanged(); }
        }

        User _User;
        public User User
        {
            get { return _User; }
            set { _User = value; OnPropertyChanged(); }
        }

        QrCode _QrCode;
        public QrCode QrCode
        {
            get { return _QrCode; }
            set { _QrCode = value; OnPropertyChanged(); }
        }

        string _Image => User.Image;
        public string Image
        {
            get { return _Image; }
        }

        string _Name => User.FirstName + " " + User.LastName;
        public string Name
        {
            get { return _Name; }
        }

        string _CheckingType => QrCode.CodeType;
        public string CheckingType
        {
            get
            {
                switch (_CheckingType)
                {
                    case "check_in": return "Check In";
                    case "check_out": return "Check Out";
                    default: return "";
                }
            }
        }

        Color _CheckingTypeColor;
        public Color CheckingTypeColor
        {
            get
            {
                switch(QrCode.CodeType)
                {
                    case "check_in": return Color.FromHex("#3498DB");
                    case "check_out": return Color.FromHex("#E74C3C");
                    default: return Color.Red;
                }
            }
        }

        string _ScanDate;
        public string ScanDate
        {
            get { return _ScanDate; }
            set { _ScanDate = value; OnPropertyChanged(); }
        }
    }
}

