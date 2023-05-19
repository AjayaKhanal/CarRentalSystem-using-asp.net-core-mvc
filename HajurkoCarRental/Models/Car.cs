using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Xml.Linq;

namespace HajurkoCarRental.Models
{
    //Model for car
    public class Car
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        [Display(Name="Plate Number")]
        public string PlateNumber { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Color { get; set; }
        [Required]
        public DateTime Year { get; set; }

        [Required]
        [Display(Name="Rental Rate")]
        public int RentalRate { get; set; }

        [Required]
        [Display(Name="Is Available")]
        public bool IsAvailable { get; set; }

        public byte[]? Picture { get; set; }

    }
}