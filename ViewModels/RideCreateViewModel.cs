using System;
using System.ComponentModel.DataAnnotations;

namespace RideSharing.ViewModels
{
    public class RideCreateViewModel
    {
        [Required]
        public string Origin { get; set; }

        [Required]
        public string Destination { get; set; }

        [Required]
        [Display(Name = "Datum Prevoza")]
        public DateTime RideDateTime { get; set; }

        [Required]
        [Display(Name = "Vozilo")]
        public int VehicleId { get; set; }
    }
}
