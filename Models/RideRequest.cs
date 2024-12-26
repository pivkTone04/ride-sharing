using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RideSharing.Models
{
    public class RideRequest
    {
        public int Id { get; set; }

        [Required]
        public int RideId { get; set; }
        public Ride Ride { get; set; }

        [Required]
        public string PassengerId { get; set; }
        public ApplicationUser Passenger { get; set; }

        [Required]
        public string Status { get; set; }

        public DateTime RequestedAt { get; set; }
    }
}
