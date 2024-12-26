using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RideSharing.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Required]
        public string DriverId { get; set; }
        public ApplicationUser Driver { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        [Range(1900, 2100)]
        public int Year { get; set; }

        [Required]
        public string LicensePlate { get; set; }

        public string Color { get; set; }

        [Range(1, 10)]
        public int Capacity { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Ride> Rides { get; set; }
    }
}
