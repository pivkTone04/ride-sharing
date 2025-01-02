using System.ComponentModel.DataAnnotations;

namespace RideSharing.ViewModels
{
    public class RideRequestEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Prosim, vnesite izvor (Origin).")]
        public string Origin { get; set; }

        [Required(ErrorMessage = "Prosim, vnesite destinacijo (Destination).")]
        public string Destination { get; set; }

        [Required(ErrorMessage = "Prosim, izberite status.")]
        public string Status { get; set; }
    }
}
