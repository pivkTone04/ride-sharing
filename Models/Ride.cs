using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RideSharing.Models
{
    public class Ride
    {
        public int Id { get; set; }

        [Required]
        public string DriverId { get; set; }
        public ApplicationUser Driver { get; set; }

        [Required]
        public string Origin { get; set; }

        [Required]
        public string Destination { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

        public ICollection<RideRequest> RideRequests { get; set; }
    }
}
