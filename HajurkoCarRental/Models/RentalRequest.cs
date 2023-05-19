using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Xml.Linq;
using HajurkoCarRental.Areas.Identity.Data;

namespace HajurkoCarRental.Models
{
    //Model for rental request
    public class RentalRequest
    {
        [Key]
        public int Id { get; set; }


        public Car Car { get; set; }
        public int CarId { get; set; }


        public User User { get; set; }
        public string UserId { get; set; }


        [Required]
        [Display(Name ="Rental Price")]
        public float Charge { get; set; }

        [Required]
        [Display(Name="Request Date")]
        public DateTime RequestDate { get; set; }

        [Required]
        public bool Approved { get; set; }

        [Required]
        public bool Cancelled { get; set; }

        [Required]
        [Display(Name = "Return Date")]
        public DateTime ReturnDate { get; set; }

        public User? AuthorizedBy { get; set; }
        public string? AuthorizedById { get; set; }


    }
}