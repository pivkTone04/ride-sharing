using System;
using System.ComponentModel.DataAnnotations;

namespace RideSharing.Models
{
    public class RideRequest
    {
        public int Id { get; set; }
        public int RideId { get; set; }
        public Ride Ride { get; set; }
        public string PassengerId { get; set; }
        public ApplicationUser Passenger { get; set; }

        [Required]
        public string Origin { get; set; }

        [Required]
        public string Destination { get; set; }

        [Required]
        [Range(1, 5)]
        public int SeatsRequested { get; set; }

        [StringLength(500)]
        public string MessageToDriver { get; set; }

        public DateTime RequestedAt { get; set; }
        public string Status { get; set; }
    }
}
