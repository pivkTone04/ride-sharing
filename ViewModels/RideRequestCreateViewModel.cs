using System.ComponentModel.DataAnnotations;

namespace RideSharing.ViewModels
{
    public class RideRequestCreateViewModel
    {
        public int RideId { get; set; }

        [Required]
        public string Origin { get; set; }

        [Required]
        public string Destination { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Lahko rezervirate med 1 in 5 sedeži.")]
        public int SeatsRequested { get; set; }

        [StringLength(500, ErrorMessage = "Sporočilo vozniku ne sme presegati 500 znakov.")]
        public string MessageToDriver { get; set; }
    }
}
