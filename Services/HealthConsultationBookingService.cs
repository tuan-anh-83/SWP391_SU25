using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BOs.Models;
using Repos;

namespace Services
{
    public class HealthConsultationBookingService : IHealthConsultationBookingService
    {
        private readonly IHealthConsultationBookingRepo _repo;
        public HealthConsultationBookingService(IHealthConsultationBookingRepo repo)
        {
            _repo = repo;
        }

        public Task<HealthConsultationBooking> CreateBookingAsync(HealthConsultationBooking booking) => _repo.CreateAsync(booking);
        public Task<List<HealthConsultationBooking>> GetBookingsByParentAsync(int parentId) => _repo.GetByParentIdAsync(parentId);
        public Task<List<HealthConsultationBooking>> GetBookingsByNurseAsync(int nurseId) => _repo.GetByNurseIdAsync(nurseId);
        public Task<HealthConsultationBooking?> GetBookingByIdAsync(int bookingId) => _repo.GetByIdAsync(bookingId);
        public Task<bool> UpdateBookingStatusAsync(int bookingId, string status) => _repo.UpdateStatusAsync(bookingId, status);
    }
}
