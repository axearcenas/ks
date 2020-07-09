using System;
namespace KeepSafe.Models
{
    public class QrCodeScanHistory : BaseNotify
    {
        string _Image;
        public string Image
        {
            get { return _Image; }
            set { _Image = value; OnPropertyChanged(); }
        }

        string _FirstName;
        public string FirstName
        {
            get { return _FirstName; }
            set { _FirstName = value; OnPropertyChanged(); }
        }

        string _LastName;
        public string LastName
        {
            get { return _LastName; }
            set { _LastName = value; OnPropertyChanged(); }
        }

        string _ScanDate;
        public string ScanDate
        {
            get { return _ScanDate; }
            set { _ScanDate = value; OnPropertyChanged(); }
        }
    }
}
