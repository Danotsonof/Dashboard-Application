using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DashboardApplication.Models
{
    /// <summary>
    /// Application user entity class model.
    /// this extends the identity class
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(30, ErrorMessage = "Max characters allowed is 30")]
        public string LastName { get; set; }
        [Required]
        [MaxLength(30, ErrorMessage = "Max characters allowed is 30")]
        public string FirstName { get; set; }
        public string Photo { get; set; }
        public string Gender { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
