using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using SWP391_BE.DTO;
using BOs.Models;
using static AgoraIO.Rtc.RtcTokenBuilder;
using AgoraIO.Rtc;

namespace SWP391_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthConsultationBookingController : ControllerBase
    {
        private readonly IHealthConsultationBookingService _service;
        private readonly IStudentService _studentService;
        private readonly IAccountService _accountService;

        public HealthConsultationBookingController(
            IHealthConsultationBookingService service,
            IStudentService studentService,
            IAccountService accountService)
        {
            _service = service;
            _studentService = studentService;
            _accountService = accountService;
        }

        [HttpPost("NurseBook")]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<IActionResult> NurseBook([FromBody] HealthConsultationBookingCreateDTO dto)
        {
            int? parentId = null;
            if (dto.StudentId.HasValue)
            {
                var student = await _studentService.GetStudentByIdAsync(dto.StudentId.Value);
                parentId = student?.ParentId;
            }

            // Kiểm tra trùng giờ cho Nurse
            var existingBookings = await _service.GetBookingsByNurseAsync(dto.NurseId);
            var conflict = existingBookings.Any(b =>
                b.Status != "Cancelled" &&
                Math.Abs((b.ScheduledTime - dto.ScheduledTime).TotalMinutes) < 30
            );
            if (conflict)
            {
                return BadRequest(new { message = "Nurse đã có lịch trong khoảng 30 phút này. Vui lòng chọn thời gian khác." });
            }

            var booking = new HealthConsultationBooking
            {
                StudentId = dto.StudentId ?? 0,
                NurseId = dto.NurseId,
                ParentId = parentId ?? 0,
                ScheduledTime = dto.ScheduledTime,
                Reason = dto.Reason,
                Status = "Pending"
            };
            var created = await _service.CreateBookingAsync(booking);

            // Lấy lại booking từ DB, include các navigation property
            var fullBooking = await _service.GetBookingByIdAsync(created.BookingId);

            return Ok(new HealthConsultationBookingResponseDTO(fullBooking));
        }

        [HttpPost("ParentBook")]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> ParentBook([FromBody] HealthConsultationBookingCreateDTO dto)
        {
            var accountIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int parentId))
                return Unauthorized(new { message = "Invalid or missing token." });

            // Lấy StudentId từ StudentCode
            int studentId;
            if (!string.IsNullOrWhiteSpace(dto.StudentCode))
            {
                var student = await _studentService.GetStudentByCodeAsync(dto.StudentCode);
                if (student == null || student.ParentId != parentId)
                    return BadRequest(new { message = "Student code không hợp lệ hoặc không thuộc quyền quản lý của phụ huynh." });
                studentId = student.StudentId;
            }
            else if (dto.StudentId.HasValue)
            {
                studentId = dto.StudentId.Value;
            }
            else
            {
                return BadRequest(new { message = "Thiếu mã học sinh (StudentCode)." });
            }

            // Kiểm tra trùng giờ cho Nurse
            var existingBookings = await _service.GetBookingsByNurseAsync(dto.NurseId);
            var conflict = existingBookings.Any(b =>
                b.Status != "Cancelled" &&
                Math.Abs((b.ScheduledTime - dto.ScheduledTime).TotalMinutes) < 30
            );
            if (conflict)
            {
                return BadRequest(new { message = "Nurse đã có lịch trong khoảng 30 phút này. Vui lòng chọn thời gian khác." });
            }

            var booking = new HealthConsultationBooking
            {
                StudentId = studentId,
                NurseId = dto.NurseId,
                ParentId = parentId,
                ScheduledTime = dto.ScheduledTime,
                Reason = dto.Reason,
                Status = "Pending"
            };
            var created = await _service.CreateBookingAsync(booking);

            // Lấy lại booking từ DB, include các navigation property
            var fullBooking = await _service.GetBookingByIdAsync(created.BookingId);

            return Ok(new HealthConsultationBookingResponseDTO(fullBooking));
        }

        // Lấy danh sách booking của Parent
        [HttpGet("Parent")]
        [Authorize(Roles = "Parent,Nurse,Admin")]
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
        [Authorize(Roles = "Nurse,Admin,Parent")]
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

        [HttpGet("Nurses")]
        [Authorize(Roles = "Parent,Nurse,Admin")]
        public async Task<IActionResult> GetAvailableNurses()
        {
            var nurses = await _accountService.GetActiveNursesAsync();
            var result = nurses.Select(n => new
            {
                n.AccountID,
                n.Fullname,
                n.Email,
                n.PhoneNumber,
                Image = n.Image != null ? Convert.ToBase64String(n.Image) : null
            });
            return Ok(result);
        }

        [HttpGet("{bookingId}/agora-token")]
        [Authorize]
        public async Task<IActionResult> GetAgoraToken(int bookingId)
        {
            try
            {
                // Verify the booking exists and user has access
                var booking = await _service.GetBookingByIdAsync(bookingId);
                if (booking == null)
                {
                    return NotFound("Booking not found");
                }

                // Get current user ID from JWT token
                var accountIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int currentUserId))
                    return Unauthorized("Invalid or missing token.");

                // Verify user has access to this booking (parent or nurse)
                if (booking.ParentId != currentUserId && booking.NurseId != currentUserId)
                {
                    return Forbid("Access denied");
                }

                // Generate channel name using booking ID
                var channelName = $"consultation_{bookingId}";

                // Generate UID (unique for each user)
                var uid = (uint)currentUserId;

                // Generate token using RtcTokenBuilder from Agora package
                var appId = "3e9d60aafb8645a69fbb30b9a42045bc"; // Your Agora App ID
                var appCertificate = "fd8b1de747e64c09841cd3dab19eaffd"; // Your Agora App Certificate

                var builder = new RtcTokenBuilder();
                var timeStamp = (uint)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600); // 1 hour from now
                var token = builder.BuildToken(appId, appCertificate, channelName, true, timeStamp);

                return Ok(new
                {
                    token = token,
                    channelName = channelName,
                    uid = uid,
                    appId = appId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to generate token");
            }
        }
    }
}
