using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RideSharing.Models
{
    public class Ride
    {
        public int Id { get; set; }

        [Required]
        public string Origin { get; set; }

        [Required]
        public string Destination { get; set; }

        [Required]
        public DateTime RideDateTime { get; set; } 
        
        [Required]
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

        public string DriverId { get; set; }
        public ApplicationUser Driver { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<RideRequest> RideRequests { get; set; } = new List<RideRequest>();

        [Required]
        [Range(1, 10, ErrorMessage = "Available seats must be between 1 and 10.")]
        public int AvailableSeats { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Price per seat must be between 0 and 100 EUR.")]
        public decimal PricePerSeat { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Pickup location must not exceed 255 characters.")]
        public string PickupLocation { get; set; }

        [StringLength(500, ErrorMessage = "Route description must not exceed 500 characters.")]
        public string RideDescription { get; set; }
    }
}
