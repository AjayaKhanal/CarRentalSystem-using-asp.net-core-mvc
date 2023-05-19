using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;

namespace HajurkoCarRental.Areas.Identity.Data;

// Add profile data for application users by adding properties to the User class
public class User : IdentityUser
{
    [Required]
    [DisplayName("Full Name")]
    public string? FullName { get; set; }

    [Required]
    [DefaultValue("Customer")]
    public string Role { get; set; } = "Customer";

    public byte[]? Picture { get; set; }

    [Required]
    public string? Address { get; set; }

    public bool RegularCustomer { get; set; }

    public string? VerificationFileName { get; set; }

    [NotMapped]
    public IFormFile[]? VerificationFile { get; set; }

}