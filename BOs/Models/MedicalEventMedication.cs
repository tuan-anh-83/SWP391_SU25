using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOs.Models
{
    public class MedicalEventMedication
    {
        public int MedicalEventId { get; set; }
        public MedicalEvent MedicalEvent { get; set; }

        public int MedicationId { get; set; }
        public Medication Medication { get; set; }

        public int QuantityUsed { get; set; }
    }
}
