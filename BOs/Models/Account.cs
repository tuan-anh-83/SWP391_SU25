using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOs.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public int RoleId { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool Status { get; set; }

        public Role Role { get; set; }
        public ICollection<Student> Students { get; set; } 
    }

}
