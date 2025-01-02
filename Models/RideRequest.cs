using System;
using System.ComponentModel.DataAnnotations;

namespace RideSharing.Models
{
    public class RideRequest
    {
        public int Id { get; set; }

        [Required]
        public string Origin { get; set; }

        [Required]
        public string Destination { get; set; }

        [Required]
        public DateTime RequestedAt { get; set; }

        [Required]
        public string PassengerId { get; set; }
        public ApplicationUser Passenger { get; set; }

        [Required]
        public int RideId { get; set; } 

        public Ride Ride { get; set; }   

        [Required]
        public string Status { get; set; }
    }
}
