using System.ComponentModel.DataAnnotations;

namespace RideSharing.ViewModels
{
    public class VehicleCreateViewModel
    {
        [Required(ErrorMessage = "Please enter the manufacturer.")]
        public string Make { get; set; }

        [Required(ErrorMessage = "Please enter the model.")]
        public string Model { get; set; }

        [Range(1900, 2100, ErrorMessage = "Please enter a valid year.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Please enter the license plate number.")]
        public string LicensePlate { get; set; }

        [Required(ErrorMessage = "Please enter the color.")]
        public string Color { get; set; }

        [Range(1, 10, ErrorMessage = "Capacity must be between 1 and 10.")]
        public int Capacity { get; set; }
    }
}
