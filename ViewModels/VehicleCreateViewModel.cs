using System.ComponentModel.DataAnnotations;

namespace RideSharing.ViewModels
{
    public class VehicleCreateViewModel
    {
        [Required(ErrorMessage = "Prosim, vnesite proizvajalca.")]
        public string Make { get; set; }

        [Required(ErrorMessage = "Prosim, vnesite model.")]
        public string Model { get; set; }

        [Range(1900, 2100, ErrorMessage = "Prosimo, vnesite veljavno leto.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Prosim, vnesite registrsko številko.")]
        public string LicensePlate { get; set; }

        [Required(ErrorMessage = "Prosim, vnesite barvo.")]
        public string Color { get; set; }

        [Range(1, 10, ErrorMessage = "Kapaciteta mora biti med 1 in 10.")]
        public int Capacity { get; set; }
    }
}
