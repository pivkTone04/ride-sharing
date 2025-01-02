using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RideSharing.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        public string? City { get; set; }


        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<Ride> Rides { get; set; } = new List<Ride>();
        public ICollection<RideRequest> RideRequests { get; set; } = new List<RideRequest>();
    }
}
