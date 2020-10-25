using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DashboardApplication.DTOs
{
    /// <summary>
    /// Update user data transfer object.
    /// this defines the model for the update data
    /// </summary>
    public class UpdateUser
    {
        [StringLength(30)]
        public string LastName { get; set; }
        [StringLength(30)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(30)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Password { get; set; }
        public string Photo { get; set; }
    }
}
