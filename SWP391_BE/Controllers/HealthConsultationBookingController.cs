using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Services;
using SWP391_BE.DTO;
using BOs.Models;

namespace SWP391_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthConsultationBookingController : ControllerBase
    {
        private readonly IHealthConsultationBookingService _service;

        public HealthConsultationBookingController(IHealthConsultationBookingService service)
        {
            _service = service;
        }

        [HttpPost("NurseBook")]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<IActionResult> NurseBook([FromBody] HealthConsultationBookingCreateDTO dto)
        {
            var booking = new HealthConsultationBooking
            {
                StudentId = dto.StudentId,
                NurseId = dto.NurseId,
                ParentId = dto.ParentId,
                ScheduledTime = dto.ScheduledTime,
                Reason = dto.Reason,
                Status = "Pending"
            };
            var created = await _service.CreateBookingAsync(booking);
            return Ok(new HealthConsultationBookingResponseDTO(created));
        }

        [HttpPost("ParentBook")]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> ParentBook([FromBody] HealthConsultationBookingCreateDTO dto)
        {
            var booking = new HealthConsultationBooking
            {
                StudentId = dto.StudentId,
                NurseId = dto.NurseId,
                ParentId = dto.ParentId,
                ScheduledTime = dto.ScheduledTime,
                Reason = dto.Reason,
                Status = "Pending"
            };
            var created = await _service.CreateBookingAsync(booking);
            return Ok(new HealthConsultationBookingResponseDTO(created));
        }

        // Lấy danh sách booking của Parent
        [HttpGet("Parent")]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> GetByParent()
        {
            var accountIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int parentId))
                return Unauthorized(new { message = "Invalid or missing token." });

            var bookings = await _service.GetBookingsByParentAsync(parentId);
            return Ok(bookings.Select(b => new HealthConsultationBookingResponseDTO(b)));
        }

        // Lấy danh sách booking của Nurse
        [HttpGet("Nurse")]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<IActionResult> GetByNurse()
        {
            var accountIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int nurseId))
                return Unauthorized(new { message = "Invalid or missing token." });

            var bookings = await _service.GetBookingsByNurseAsync(nurseId);
            return Ok(bookings.Select(b => new HealthConsultationBookingResponseDTO(b)));
        }

        // Lấy chi tiết 1 booking
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _service.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();
            return Ok(new HealthConsultationBookingResponseDTO(booking));
        }

        // Xác nhận booking (Nurse hoặc Parent đều có thể xác nhận)
        [HttpPut("{id}/confirm")]
        [Authorize(Roles = "Nurse,Admin,Parent")]
        public async Task<IActionResult> Confirm(int id)
        {
            var updated = await _service.UpdateBookingStatusAsync(id, "Confirmed");
            if (!updated) return NotFound();
            return Ok(new { message = "Booking confirmed." });
        }

        // Hủy booking (Nurse hoặc Parent đều có thể hủy)
        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Nurse,Admin,Parent")]
        public async Task<IActionResult> Cancel(int id)
        {
            var updated = await _service.UpdateBookingStatusAsync(id, "Cancelled");
            if (!updated) return NotFound();
            return Ok(new { message = "Booking cancelled." });
        }

        // Đánh dấu hoàn thành (Nurse xác nhận đã tư vấn xong)
        [HttpPut("{id}/done")]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<IActionResult> Done(int id)
        {
            var updated = await _service.UpdateBookingStatusAsync(id, "Done");
            if (!updated) return NotFound();
            return Ok(new { message = "Booking marked as done." });
        }
    }
}
