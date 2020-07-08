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

        public bool Equals(User user)
        {
            if (user == null)
                return false;

            bool IsEquals = true;


            if (user.Id != Id)
                IsEquals = false;
            if (user.Image != Image)
                IsEquals = false;
            if (user.FirstName != FirstName)
                IsEquals = false;
            if (user.LastName != LastName)
                IsEquals = false;
            if (user.ContactNumber != ContactNumber)
                IsEquals = false;
            if (user.Address != Address)
                IsEquals = false;
            if (user.Birthdate != Birthdate)
                IsEquals = false;
            if (user.Email != Email)
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
