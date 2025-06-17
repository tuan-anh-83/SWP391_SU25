using System;
using System.Collections.Generic;

namespace BOs.Models
{
    public class VaccinationRecord
    {
        public int RecordId { get; set; }
        public int CampaignId { get; set; }
        public int StudentId { get; set; }
        public int NurseId { get; set; }
        public DateTime DateInjected { get; set; }
        public string Result { get; set; } // "Đã tiêm", "Chưa tiêm", "Hoãn"
        public string? ImmediateReaction { get; set; } // Phản ứng tức thì sau tiêm
        public string? Note { get; set; }

        public VaccinationCampaign Campaign { get; set; }
        public Student Student { get; set; }
        public Account Nurse { get; set; }
    }
}