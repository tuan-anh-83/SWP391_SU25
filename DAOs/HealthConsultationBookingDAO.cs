using BOs.Models;
using Microsoft.EntityFrameworkCore;
using BOs.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace DAOs
{
    public class HealthConsultationBookingDAO
    {
        private readonly DataContext _context;
        public HealthConsultationBookingDAO(DataContext context)
        {
            _context = context;
        }

        public async Task<HealthConsultationBooking> CreateAsync(HealthConsultationBooking booking)
        {
            _context.HealthConsultationBookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<List<HealthConsultationBooking>> GetByParentIdAsync(int parentId)
        {
            return await _context.HealthConsultationBookings
                .Include(b => b.Student)
                    .ThenInclude(s => s.Class)
                .Include(b => b.Nurse)
                .Include(b => b.Parent)
                .Where(b => b.ParentId == parentId)
                .ToListAsync();
        }

        public async Task<List<HealthConsultationBooking>> GetByNurseIdAsync(int nurseId)
        {
            return await _context.HealthConsultationBookings
                .Include(b => b.Student)
                    .ThenInclude(s => s.Class)
                .Include(b => b.Nurse)
                .Include(b => b.Parent)
                .Where(b => b.NurseId == nurseId)
                .ToListAsync();
        }

        public async Task<HealthConsultationBooking?> GetByIdAsync(int bookingId)
        {
            return await _context.HealthConsultationBookings
                .Include(b => b.Student)
                    .ThenInclude(s => s.Class)
                .Include(b => b.Nurse)
                .Include(b => b.Parent)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        public async Task<bool> UpdateStatusAsync(int bookingId, string status)
        {
            var booking = await _context.HealthConsultationBookings.FindAsync(bookingId);
            if (booking == null) return false;
            booking.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
