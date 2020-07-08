using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using KeepSafe.Enum;
using KeepSafe.Extensions;

namespace KeepSafe.Models
{
    public class Business : BaseNotify
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

        string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged(); }
        }

        BusinessType _BusinessType;
        public BusinessType BusinessType
        {
            get { return _BusinessType; }
            set { _BusinessType = value; OnPropertyChanged(); }
        }

        string _ContactPerson;
        public string ContactPerson
        {
            get { return _ContactPerson; }
            set { _ContactPerson = value; OnPropertyChanged(); }
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

        string _Email;
        public string Email
        {
            get { return _Email; }
            set { _Email = value; OnPropertyChanged(); }
        }

        ObservableCollection<QrCode> _QrCode = new ObservableCollection<QrCode>();
        public ObservableCollection<QrCode> QrCode
        {
            get { return _QrCode; }
            set { _QrCode = value; OnPropertyChanged(); }
        }

        public bool Equals(Business business)
        {
            if (business == null)
                return false;

            bool IsEquals = true;

            if (!business.Id.Equals(Id))
                IsEquals = false;
            if (string.IsNullOrEmpty(business.Image) ? true : !business.Image.Equals(Image))
                IsEquals = false;
            if (string.IsNullOrEmpty(business.ContactPerson) ? true : !business.ContactPerson.Equals(ContactPerson))
                IsEquals = false;
            if (string.IsNullOrEmpty(business.ContactNumber) ? true : !business.ContactNumber.Equals(ContactNumber))
                IsEquals = false;
            if (string.IsNullOrEmpty(business.Address) ? true : !(bool)business.Address?.Equals(Address))
                IsEquals = false;
            if (string.IsNullOrEmpty(business.Email) ? true : !business.Email.Equals(Email))
                IsEquals = false;
            if (string.IsNullOrEmpty(business.Name) ? true : !business.Email.Equals(Name))
                IsEquals = false;
            return IsEquals;
        }

        public void Update(Business business)
        {
            ContactPerson = business.ContactPerson;
            Name = business.Name;
            Image = business.Image;
            ContactNumber = business.ContactNumber;
            Address = business.Address;
            Email = business.Email;
        }
    }
}
