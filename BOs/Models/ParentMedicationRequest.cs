using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOs.Models
{
    public class ParentMedicationRequest
    {
        public int RequestId { get; set; }
        public int ParentId { get; set; }
        public int StudentId { get; set; }
        public string? ParentNote { get; set; } // Cho phép null
        public string? NurseNote { get; set; }  // Cho phép null
        public DateTime DateCreated { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected

        public Account Parent { get; set; }
        public Student Student { get; set; }
        public ICollection<ParentMedicationDetail> Medications { get; set; }
    }
}
