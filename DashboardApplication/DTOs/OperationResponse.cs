using DashboardApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashboardApplication.DTOs
{
    /// <summary>
    /// Class model for the user's response
    /// </summary>
    public class OperationResponse
    {
        public OperationResponse()
        {
            AllUsers = new List<UserToReturn>();
        }
        public int maleCount { get; set; }
        public int femaleCount { get; set; }
        public bool IsAdmin { get; set; }
        public string Message { get; set; }
        public string Photo { get; set; }
        public List<UserToReturn> AllUsers { get; set;}
    }
}
