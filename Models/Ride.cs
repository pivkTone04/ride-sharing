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
        public DateTime DateTime { get; set; }

        [Required]
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

        public string DriverId { get; set; }
        public ApplicationUser Driver { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<RideRequest> RideRequests { get; set; }
    }
}
