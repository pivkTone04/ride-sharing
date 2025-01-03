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
        [Display(Name = "Datum Prevoza")]
        public DateTime RideDateTime { get; set; }

        [Required]
        [Display(Name = "Vozilo")]
        public int VehicleId { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "Prosti sedeži morajo biti med 1 in 10.")]
        [Display(Name = "Prosti sedeži")]
        public int AvailableSeats { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Cena na sedež mora biti med 0 in 100 EUR.")]
        [Display(Name = "Cena na sedež (€)")]
        public decimal PricePerSeat { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Lokacija pobiranja ne sme biti daljša od 255 znakov.")]
        [Display(Name = "Kje te lahko poberem?")]
        public string PickupLocation { get; set; }

        [StringLength(500, ErrorMessage = "Opis poti ne sme biti daljši od 500 znakov.")]
        [Display(Name = "Opis poti")]
        public string RideDescription { get; set; }
    }
}
