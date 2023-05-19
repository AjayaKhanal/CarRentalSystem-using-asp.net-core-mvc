using HajurkoCarRental.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace HajurkoCarRental.Models
{
    //Model for damage
    public class Damage
    {
        [Key]
        public int Id { get; set; }

        public string Description { get; set; }

        public float? RepairCost { get; set; }

        public Car Car { get; set; }

        public int CarId { get; set; }

        public User User { get; set; }
        public string UserId { get; set; }

        public bool Paid { get; set; }
    }
}
