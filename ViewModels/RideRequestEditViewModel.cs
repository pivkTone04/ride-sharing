using System.ComponentModel.DataAnnotations;

namespace RideSharing.ViewModels
{
    public class RideRequestEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int RideId { get; set; }

        [Required]
        public string Status { get; set; }
    }
}
