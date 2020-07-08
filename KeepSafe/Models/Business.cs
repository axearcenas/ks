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

        int _BusinessType;
        public int BusinessType
        {
            get { return _BusinessType; }
            set { _BusinessType = value; OnPropertyChanged(); }
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

        public List<string> BusinessTypeList
        {
            get
            {
                return System.Enum.GetNames(typeof(BusinessType)).Select(b => b.SplitCamelCase()).ToList();
            }
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

            IsEquals = false;
            if (!business.Id.Equals(Id))
                IsEquals = false;
            if (!business.Image.Equals(Image))
                IsEquals = false;
            if (!business.Name.Equals(Name))
                IsEquals = false;
            if (!business.BusinessType.Equals(BusinessType))
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
            Image = business.Image;
            ContactNumber = business.ContactNumber;
            ContactPerson = business.ContactPerson;
            Address = business.Address;
            Email = business.Email;
        }
    }

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
    }
}
