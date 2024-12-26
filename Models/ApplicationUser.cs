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

        public ICollection<Ride> Rides { get; set; }
        public ICollection<RideRequest> RideRequests { get; set; }
    }
}
