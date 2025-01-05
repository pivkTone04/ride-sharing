using System.ComponentModel.DataAnnotations;

namespace RideSharing.ViewModels
{
    public class RideRequestCreateViewModel
    {
        public int RideId { get; set; }

        [Required]
        public string Origin { get; set; }

        [Required]
        public string Destination { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "You can book between 1 and 5 seats.")]
        public int SeatsRequested { get; set; }

        [StringLength(500, ErrorMessage = "Message to the driver must not exceed 500 characters.")]
        public string MessageToDriver { get; set; }
    }
}
