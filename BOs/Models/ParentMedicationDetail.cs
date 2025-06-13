using System;

namespace BOs.Models
{
    public class ParentMedicationDetail
    {
        public int MedicationDetailId { get; set; }
        public int RequestId { get; set; }
        public string Name { get; set; } // Tên thuốc (bắt buộc)
        public string Type { get; set; } // Dạng thuốc (tùy chọn)
        public string Usage { get; set; } // Hướng dẫn sử dụng (tùy chọn)
        public string Dosage { get; set; } // Liều lượng (tùy chọn)
        public DateTime? ExpiredDate { get; set; } // Ngày hết hạn (tùy chọn)
        public string Note { get; set; } // Ghi chú thêm (tùy chọn)

        public ParentMedicationRequest Request { get; set; }
    }
}