using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashboardApplication.DTOs
{
    /// <summary>
    /// Data transfer object for the user's details to return.
    /// </summary>
    public class UserToReturn
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
    }
}
