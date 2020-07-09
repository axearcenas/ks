using System;
using KeepSafe.Enum;

namespace KeepSafe.Models
{
    public class UserScanHistory : BaseNotify
    {
        Business _Business;
        public Business Business
        {
            get { return _Business; }
            set { _Business = value; OnPropertyChanged(); }
        }

        QrCode _QrCode;
        public QrCode QrCode
        {
            get { return _QrCode; }
            set { _QrCode = value; OnPropertyChanged(); }
        }

        double _Temperature;
        public double Temperature
        {
            get { return _Temperature; }
            set { _Temperature = value; OnPropertyChanged(); }
        }

        DateTime _ScanDate;
        public DateTime ScanDate
        {
            get { return _ScanDate; }
            set { _ScanDate = value; OnPropertyChanged(); }
        }

        public HistoryType HistoryType { get { return QrCode == null ? HistoryType.CheckIn : QrCode.CodeType.Equals("check_in") ? HistoryType.CheckIn : HistoryType.CheckOut; } }

        public BusinessType BusinessType { get { return Business == null ? BusinessType.Hotel : Business.BusinessType; } }

        public string BusinessTypeIcon { get{ return GetBusinessTypeIcon(BusinessType); } }

        string GetBusinessTypeIcon(BusinessType businessType)
        {
            switch(businessType)
            {
                case BusinessType.Hotel:
                    return "building";
                case BusinessType.GymAndHotel:
                    return "Hotel";
                case BusinessType.Clinic:
                    return "Hotel";
                case BusinessType.Hospital:
                    return "Hotel";
                case BusinessType.TravelAgency:
                    return "airplane";
                case BusinessType.Boutique:
                    return "Hotel";
                case BusinessType.BarAndRestaurant:
                    return "Hotel";
                case BusinessType.Manufacturing:
                    return "Hotel";
                case BusinessType.BankingAndRemittance:
                    return "Hotel";
                case BusinessType.SpaAndPersonalCare:
                    return "Hotel";
                case BusinessType.Government:
                    return "Hotel";
                case BusinessType.TheatreAndMovieHouses:
                    return "Hotel";
                case BusinessType.ApplianceAndComputerStore:
                    return "Hotel";
                case BusinessType.DepartmentStore:
                    return "Hotel";
                case BusinessType.GroceryAndSupermarket:
                    return "Hotel";
                case BusinessType.Automobile:
                    return "bus";
                case BusinessType.HomeImprovement:
                    return "Hotel";
                case BusinessType.GraphicsAndPrinting:
                    return "Hotel";
            }
            return "Hotel";
        }

        public UserScanHistory() 
        {
        }
    }
}
