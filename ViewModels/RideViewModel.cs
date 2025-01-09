using System;

namespace RideSharing.ViewModels
{
    public class RideViewModel
    {
        public int Id { get; set; }
        public string DriverEmail { get; set; }
        public string VehicleName { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime RideDateTime { get; set; }
        public bool CanRequest { get; set; }
        public bool IsDriver { get; set; }
        public int AvailableSeats { get; set; }
        public decimal PricePerSeat { get; set; }
        public string TotalDistance { get; set; }
        public string TotalDuration { get; set; }
    }
}
