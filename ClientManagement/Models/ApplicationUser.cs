using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ClientManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; } 

        [Required]
        public string LastName { get; set; } 

        [Required, EmailAddress]
        public string Email {get; set; } 

        [Required]
        [Compare("ConfirmPassword")]
        public string Password { get; set; } 

        [Required]
        public string ConfirmPassword { get; set; } 
    }
}
