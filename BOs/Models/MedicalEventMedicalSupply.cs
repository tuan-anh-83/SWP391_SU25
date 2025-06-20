using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOs.Models
{
    public class MedicalEventMedicalSupply
    {
        public int MedicalEventId { get; set; }
        public MedicalEvent MedicalEvent { get; set; }

        public int MedicalSupplyId { get; set; }
        public MedicalSupply MedicalSupply { get; set; }

        public int QuantityUsed { get; set; }
    }
}
