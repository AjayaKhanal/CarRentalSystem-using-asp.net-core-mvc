using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HajurkoCarRental.Models
{
    //Model for Offer
    public class Offer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }


        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}