using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOs.Models
{
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }

        public ICollection<Account> Accounts { get; set; }
    }

}
