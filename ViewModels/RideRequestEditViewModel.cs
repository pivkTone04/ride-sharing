using System.ComponentModel.DataAnnotations;

namespace RideSharing.ViewModels
{
    public class RideRequestEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the origin.")]
        public string Origin { get; set; }

        [Required(ErrorMessage = "Please enter the destination.")]
        public string Destination { get; set; }

        [Required(ErrorMessage = "Please select a status.")]
        public string Status { get; set; }
    }
}
