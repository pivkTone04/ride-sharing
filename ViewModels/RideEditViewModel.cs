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
        [Range(1, 10, ErrorMessage = "Empty seats morajo biti med 1 in 10.")]
        [Display(Name = "Empty seats")]
        public int AvailableSeats { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Price per seat mora biti med 0 in 100 EUR.")]
        [Display(Name = "Price per seat (€)")]
        public decimal PricePerSeat { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Lokacija pobiranja ne sme biti daljša od 255 znakov.")]
        [Display(Name = "Possible stops")]
        public string PickupLocation { get; set; }

        [StringLength(500, ErrorMessage = "Route description ne sme biti daljši od 500 znakov.")]
        [Display(Name = "Route description")]
        public string RideDescription { get; set; }
        public string TotalDistance { get; set; }
        public string TotalDuration { get; set; }
    }
}
