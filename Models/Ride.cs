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
        [Range(1, 10, ErrorMessage = "Prosti sedeži morajo biti med 1 in 10.")]
        public int AvailableSeats { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Cena na sedež mora biti med 0 in 100 EUR.")]
        public decimal PricePerSeat { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Lokacije pobiranja ne sme biti daljša od 255 znakov.")]
        public string PickupLocation { get; set; }

        [StringLength(500, ErrorMessage = "Opis poti ne sme biti daljši od 500 znakov.")]
        public string RideDescription { get; set; }
    }
}
