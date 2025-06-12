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
        public string ParentNote { get; set; } // Ghi chú của phụ huynh
        public string NurseNote { get; set; }  // Ghi chú của nurse/admin khi duyệt
        public DateTime DateCreated { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected

        public Account Parent { get; set; }
        public Student Student { get; set; }
        public ICollection<Medication> Medications { get; set; }
    }
}
