using BOs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IHealthConsultationBookingService
    {
        Task<HealthConsultationBooking> CreateBookingAsync(HealthConsultationBooking booking);
        Task<List<HealthConsultationBooking>> GetBookingsByParentAsync(int parentId);
        Task<List<HealthConsultationBooking>> GetBookingsByNurseAsync(int nurseId);
        Task<HealthConsultationBooking?> GetBookingByIdAsync(int bookingId);
        Task<bool> UpdateBookingStatusAsync(int bookingId, string status);
    }
}
