using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BOs.Models;
using DAOs;

namespace Repos
{
    public class HealthConsultationBookingRepo : IHealthConsultationBookingRepo
    {
        private readonly HealthConsultationBookingDAO _dao;
        public HealthConsultationBookingRepo(HealthConsultationBookingDAO dao)
        {
            _dao = dao;
        }

        public Task<HealthConsultationBooking> CreateAsync(HealthConsultationBooking booking) => _dao.CreateAsync(booking);
        public Task<List<HealthConsultationBooking>> GetByParentIdAsync(int parentId) => _dao.GetByParentIdAsync(parentId);
        public Task<List<HealthConsultationBooking>> GetByNurseIdAsync(int nurseId) => _dao.GetByNurseIdAsync(nurseId);
        public Task<HealthConsultationBooking?> GetByIdAsync(int bookingId) => _dao.GetByIdAsync(bookingId);
        public Task<bool> UpdateStatusAsync(int bookingId, string status) => _dao.UpdateStatusAsync(bookingId, status);
    }
}
