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

        string _ContactNumber;
        public string ContactNumber
        {
            get { return _ContactNumber; }
            set { _ContactNumber = value; OnPropertyChanged(); }
        }

        string _Address;
        public string Address
        {
            get { return _Address; }
            set { _Address = value; OnPropertyChanged(); }
        }

        DateTime _Birthdate = DateTime.Now;
        public DateTime Birthdate
        {
            get { return _Birthdate == null ? DateTime.Now : _Birthdate; }
            set { _Birthdate = value; OnPropertyChanged(); }
        }

        string _Email;
        public string Email
        {
            get { return _Email; }
            set { _Email = value; OnPropertyChanged(); }
        }

        string _Qrcode;
        public string Qrcode
        {
            get { return _Qrcode; }
            set { _Qrcode = value; OnPropertyChanged(); }
        }

        string _QrcodeImage;
        public string QrcodeImage
        {
            get { return _QrcodeImage; }
            set { _QrcodeImage = value; OnPropertyChanged(); }
        }

        double _Temperature;
        public double Temperature
        {
            get { return _Temperature; }
            set { _Temperature = value; OnPropertyChanged(nameof(Temperature)); }
        }

        public bool Equals(User user)
        {
            if (user == null)
                return false;

            bool IsEquals = true;
            
            IsEquals = false;
            if (!user.Id.Equals(Id))
                IsEquals = false;
            if (string.IsNullOrEmpty(user.Image) ? true : !user.Image.Equals(Image))
                IsEquals = false;
            if (string.IsNullOrEmpty(user.FirstName) ? true : !user.FirstName.Equals(FirstName))
                IsEquals = false;
            if (string.IsNullOrEmpty(user.LastName) ? true : !user.LastName.Equals(LastName))
                IsEquals = false;
            if (string.IsNullOrEmpty(user.ContactNumber) ? true : !user.ContactNumber.Equals(ContactNumber))
                IsEquals = false;
            if (string.IsNullOrEmpty(user.Address) ? true : !(bool)user.Address?.Equals(Address))
                IsEquals = false;
            if (!user.Birthdate.Equals(Birthdate))
                IsEquals = false;
            if (string.IsNullOrEmpty(user.Email) ? true : !user.Email.Equals(Email))
                IsEquals = false;
            
            return IsEquals;
        }

        public void Update(User user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            Image = user.Image;
            ContactNumber = user.ContactNumber;
            Address = user.Address;
            Birthdate = user.Birthdate;
            Email = user.Email;
        }
    }
}
