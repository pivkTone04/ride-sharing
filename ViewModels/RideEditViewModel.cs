using System;
using System.ComponentModel.DataAnnotations;

namespace RideSharing.ViewModels
{
    public class RideEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Origin { get; set; }

        [Required]
        public string Destination { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ride date")]
        public DateTime RideDateTime { get; set; }

        [Required]
        [Display(Name = "Vehicle")]
        public int VehicleId { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "Empty seats must be between 1 and 10.")]
        [Display(Name = "Empty seats")]
        public int AvailableSeats { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Price per seat must be between 0 and 100 EUR.")]
        [Display(Name = "Price per seat (€)")]
        public decimal PricePerSeat { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Pickup location must not exceed 255 characters.")]
        [Display(Name = "Possible stops")]
        public string PickupLocation { get; set; }

        [StringLength(500, ErrorMessage = "Route description must not exceed 500 characters.")]
        [Display(Name = "Route description")]
        public string RideDescription { get; set; }

        [Display(Name = "Total Distance")]
        public string TotalDistance { get; set; }

        [Display(Name = "Total Duration")]
        public string TotalDuration { get; set; }
    }
}
