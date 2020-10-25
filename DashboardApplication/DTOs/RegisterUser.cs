using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DashboardApplication.DTOs
{
    /// <summary>
    /// Register user data transfer object.
    /// this defines the model for the registration data
    /// </summary>
    public class RegisterUser
    {
        [Required]
        [StringLength(30)]
        public string LastName { get; set; }
        [Required]
        [StringLength(30)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(30)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Password { get; set; }
        public string Gender { get; set; }
        public string Photo { get; set; }
    }
}
