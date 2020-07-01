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

        private ObservableCollection<string> _EstablishmentType = new ObservableCollection<string> { "Clinic", "Hospital", "Travel Agency", "Boutique", "Bar & Restaurant", "Manufacturing", "Banking & Remittance", "Spa & Personal Care", "Government", "Theatre & Movie Houses", "Appliance & Computer Store", "Department Store", "Grocery & Supermarket", "Automobile", "Home Improvement", "Graphics & Printing" };
        public ObservableCollection<string> EstablishmentType
        {
            get { return _EstablishmentType; }
            set { _EstablishmentType = value; OnPropertyChanged(); }
        }
    }
}
