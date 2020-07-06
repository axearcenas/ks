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

        string _Photo;
        public string Photo
        {
            get { return _Photo; }
            set { _Photo = value; OnPropertyChanged(); }
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

        string _PhoneNumber;
        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set { _PhoneNumber = value; OnPropertyChanged(); }
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

        DateTime _Birthdate;
        public DateTime Birthdate
        {
            get { return _Birthdate; }
            set { _Birthdate = value; OnPropertyChanged(); }
        }

        string _Gender;
        public string Gender
        {
            get { return _Gender; }
            set { _Gender = value; OnPropertyChanged(); }
        }

        string _Email;
        public string Email
        {
            get { return _Email; }
            set { _Email = value; OnPropertyChanged(); }
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

        public bool Equals(User user)
        {
            bool IsEquals = true;
            if (!user.Id.Equals(Id))
                IsEquals =  false;
            if (!user.Photo.Equals(Photo))
                IsEquals = false;
            if (!user.FirstName.Equals(FirstName))
                IsEquals = false;
            if (!user.LastName.Equals(LastName))
                IsEquals = false;
            if (!user.PhoneNumber.Equals(PhoneNumber))
                IsEquals = false;
            if (!user.Address.Equals(Address))
                IsEquals = false;
            if (!user.Birthdate.Equals(Birthdate))
                IsEquals = false;
            if (!user.Email.Equals(Email))
                IsEquals = false;
            return IsEquals;
        }

        public void Update(User user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            Photo = user.Photo;
            PhoneNumber = user.PhoneNumber;
            Address = user.Address;
            Birthdate = user.Birthdate;
            Email = user.Email;
        }
    }
}
