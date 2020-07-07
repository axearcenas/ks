using System;
using System.Collections.ObjectModel;

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

        string _Photo;
        public string Photo
        {
            get { return _Photo; }
            set { _Photo = value; OnPropertyChanged(); }
        }

        string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged(); }
        }

        int _BusinessType;
        public int BusinessType
        {
            get { return _BusinessType; }
            set { _BusinessType = value; OnPropertyChanged(); }
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

        string _ContactPerson;
        public string ContactPerson
        {
            get { return _ContactPerson; }
            set { _ContactPerson = value; OnPropertyChanged(); }
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

        string _Password;
        public string Password
        {
            get { return _Password; }
            set { _Password = value; OnPropertyChanged(); }
        }

        public bool Equals(Business business)
        {
            bool IsEquals = true;
            if (!business.Id.Equals(Id))
                IsEquals = false;
            if (!business.Photo.Equals(Photo))
                IsEquals = false;
            if (!business.Name.Equals(Name))
                IsEquals = false;
            if (!business.BusinessType.Equals(BusinessType))
                IsEquals = false;
            if (!business.PhoneNumber.Equals(PhoneNumber))
                IsEquals = false;
            if (!business.ContactNumber.Equals(ContactNumber))
                IsEquals = false;
            if (!business.ContactPerson.Equals(ContactPerson))
                IsEquals = false;
            if (!business.Address.Equals(Address))
                IsEquals = false;
            if (!business.Email.Equals(Email))
                IsEquals = false;
            return IsEquals;
        }

        public void Update(Business business)
        {
            Name = business.Name;
            BusinessType = business.BusinessType;
            Photo = business.Photo;
            PhoneNumber = business.PhoneNumber;
            ContactPerson = business.ContactPerson;
            Address = business.Address;
            Email = business.Email;
        }
    }
}
