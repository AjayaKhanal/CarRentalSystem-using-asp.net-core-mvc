using System.ComponentModel.DataAnnotations;

namespace HajurkoCarRental.Models
{
    //Model for car return
    public class CarReturn
    {

        [Key]
        public int Id { get; set; }

        public RentalRequest RentalRequest { get; set; }
        public int RentalRequestId { get; set; }


        [Display(Name = "Fuel Level")]
        public float FuelLevel { get; set; }

        public bool IsDamaged { get; set; }

        [Required]
        [Display(Name = "Return Date")]
        public DateTime ReturnDate { get; set; }
    }
}
