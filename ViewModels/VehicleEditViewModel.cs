using System.ComponentModel.DataAnnotations;

namespace RideSharing.ViewModels
{
    public class VehicleEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        [Range(1900, 2100, ErrorMessage = "Prosimo, vnesite veljavno leto.")]
        public int Year { get; set; }

        [Required]
        public string LicensePlate { get; set; }

        public string Color { get; set; }

        [Range(1, 10, ErrorMessage = "Kapaciteta mora biti med 1 in 10.")]
        public int Capacity { get; set; }
    }
}
