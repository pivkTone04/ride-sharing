using System.ComponentModel.DataAnnotations;

namespace RideSharing.ViewModels
{
    public class RideRequestCreateViewModel
    {
        [Required]
        public int RideId { get; set; }

        [Required(ErrorMessage = "Prosim, vnesite izvor (Origin).")]
        public string Origin { get; set; }

        [Required(ErrorMessage = "Prosim, vnesite destinacijo (Destination).")]
        public string Destination { get; set; }
    }
}
