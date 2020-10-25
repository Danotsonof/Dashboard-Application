using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DashboardApplication.DTOs
{
    /// <summary>
    /// Login user data transfer object.
    /// this defines the model for the login data
    /// </summary>
    public class LoginUser
    {
        [Required]
        [StringLength(30)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 5)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
