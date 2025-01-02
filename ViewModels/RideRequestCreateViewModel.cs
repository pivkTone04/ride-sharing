using System.ComponentModel.DataAnnotations;

namespace RideSharing.ViewModels
{
    public class RideRequestCreateViewModel
    {
        [Required]
        public int RideId { get; set; }

        [Required]
        public string Status { get; set; }
    }
}
