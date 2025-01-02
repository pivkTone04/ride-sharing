using System;

namespace RideSharing.ViewModels
{
    public class RideRequestDetailsViewModel
    {
        public int Id { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime RequestedAt { get; set; }
        public string Status { get; set; }
        public string PassengerName { get; set; }
        public string VehicleDetails { get; set; }
    }
}
