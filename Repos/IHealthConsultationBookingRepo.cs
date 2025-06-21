using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BOs.Models;

namespace Repos
{
    public interface IHealthConsultationBookingRepo
    {
        Task<HealthConsultationBooking> CreateAsync(HealthConsultationBooking booking);
        Task<List<HealthConsultationBooking>> GetByParentIdAsync(int parentId);
        Task<List<HealthConsultationBooking>> GetByNurseIdAsync(int nurseId);
        Task<HealthConsultationBooking?> GetByIdAsync(int bookingId);
        Task<bool> UpdateStatusAsync(int bookingId, string status);
    }
}
