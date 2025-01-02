using System;
using System.ComponentModel.DataAnnotations;

namespace RideSharing.ViewModels
{
    public class RideEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Origin { get; set; }

        [Required]
        public string Destination { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public int VehicleId { get; set; }
    }
}
