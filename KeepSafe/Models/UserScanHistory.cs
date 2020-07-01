using System;
using KeepSafe.Enum;

namespace KeepSafe.Models
{
    public class UserScanHistory : BaseNotify
    {
        int _Id;
        public int Id
        {
            get { return _Id; }
            set { _Id = value; OnPropertyChanged(); }
        }

        string _PlateNumber;
        public string PlateNumber
        {
            get { return _PlateNumber; }
            set { _PlateNumber = value; OnPropertyChanged(); }
        }

        string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged(); }
        }

        string _Address;
        public string Address
        {
            get { return _Address; }
            set { _Address = value; OnPropertyChanged(); }
        }

        DateTime _DateScanned;
        public DateTime DateScanned
        {
            get { return _DateScanned; }
            set { _DateScanned = value; OnPropertyChanged(); }
        }

        HistoryType _HistoryType;
        public HistoryType HistoryType
        {
            get { return _HistoryType; }
            set { _HistoryType = value; OnPropertyChanged(); }
        }

        BusinessType _BusinessType;
        public BusinessType BusinessType
        {
            get { return _BusinessType; }
            set { _BusinessType = value; OnPropertyChanged(); OnPropertyChanged(nameof(BusinessTypeIcon)); }
        }

        public string BusinessTypeIcon { get{ return GetBusinessTypeIcon(BusinessType); } }

        string GetBusinessTypeIcon(BusinessType businessType)
        {
            switch(businessType)
            {
                case BusinessType.Hotel:
                    return "building";
                case BusinessType.GymOrYoga:
                    return "Hotel";
                case BusinessType.Clinic:
                    return "Hotel";
                case BusinessType.Hospital:
                    return "Hotel";
                case BusinessType.TravelAgency:
                    return "airplane";
                case BusinessType.Boutique:
                    return "Hotel";
                case BusinessType.BarOrRestaurant:
                    return "Hotel";
                case BusinessType.Manufacturing:
                    return "Hotel";
                case BusinessType.BankingOrRemittance:
                    return "Hotel";
                case BusinessType.SpaOrPersonalCare:
                    return "Hotel";
                case BusinessType.Government:
                    return "Hotel";
                case BusinessType.TheatreOrMovieHouses:
                    return "Hotel";
                case BusinessType.ApplianceOrComputerStore:
                    return "Hotel";
                case BusinessType.DepartmentStore:
                    return "Hotel";
                case BusinessType.GroceryOrSupermarket:
                    return "Hotel";
                case BusinessType.Automobile:
                    return "bus";
                case BusinessType.HomeImprovement:
                    return "Hotel";
                case BusinessType.GraphicsOrPrinting:
                    return "Hotel";
            }
            return "Hotel";
        }

        public UserScanHistory() 
        {
        }
    }
}
