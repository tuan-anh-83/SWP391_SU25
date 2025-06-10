using System.Security.Claims;
using BOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using SWP391_BE.DTO;

namespace SWP391_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalEventController : ControllerBase
    {
        private readonly IMedicalEventService _medicalEventService;
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MedicalEventController(IMedicalEventService medicalEventService, IAccountService accountService, IHttpContextAccessor httpContextAccessor)
        {
            _medicalEventService = medicalEventService;
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("GetAllMedicalEvents")]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<IActionResult> GetAllMedicalEvents()
        {
            var medicalEvents = await _medicalEventService.GetAllMedicalEventsAsync();

            var result = medicalEvents.Select(me => new
            {
                me.MedicalEventId,
                me.StudentId,
                StudentName = me.Student?.Fullname,
                me.NurseId,
                NurseName = me.Nurse?.Fullname,
                me.MedicationId,
                MedicationName = me.Medication?.Name,
                me.Type,
                me.Description,
                me.Note,
                me.Date
            });

            return Ok(result);
        }

        [HttpGet("GetMedicalEventById/{id}")]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<IActionResult> GetMedicalEventById(int id)
        {
            var medicalEvent = await _medicalEventService.GetMedicalEventByIdAsync(id);
            if (medicalEvent == null) return NotFound(new { message = "Medical event not found." });

            var result = new
            {
                medicalEvent.MedicalEventId,
                medicalEvent.StudentId,
                StudentName = medicalEvent.Student?.Fullname,
                medicalEvent.NurseId,
                NurseName = medicalEvent.Nurse?.Fullname,
                medicalEvent.MedicationId,
                MedicationName = medicalEvent.Medication?.Name,
                medicalEvent.Type,
                medicalEvent.Description,
                medicalEvent.Note,
                medicalEvent.Date
            };

            return Ok(result);
        }

        [HttpPost("CreateMedicalEvent")]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<IActionResult> CreateMedicalEvent([FromBody] MedicalEventCreateDTO dto)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                return Unauthorized(new { message = "Invalid or missing token." });

            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
                return NotFound(new { message = "Account not found." });
            if (!account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(new { message = "Account is not active." });

            var medicalEvent = new MedicalEvent
            {
                StudentId = dto.StudentId,
                NurseId = account.AccountID,
                MedicationId = dto.MedicationId,
                Type = dto.Type.Trim(),
                Description = dto.Description.Trim(),
                Note = dto.Note?.Trim(),
                Date = dto.Date
            };

            var created = await _medicalEventService.CreateMedicalEventAsync(medicalEvent);
            if (!created)
                return BadRequest(new { message = "Failed to create medical event." });

            return CreatedAtAction(nameof(GetMedicalEventById), new { id = medicalEvent.MedicalEventId }, new { message = "Medical event created successfully.", medicalEventId = medicalEvent.MedicalEventId });
        }

        [HttpPut("UpdateMedicalEvent/{id}")]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<IActionResult> UpdateMedicalEvent(int id, [FromBody] MedicalEventUpdateModel model)
        {
            var medicalEvent = await _medicalEventService.GetMedicalEventByIdAsync(id);
            if (medicalEvent == null) return NotFound(new { message = "Medical event not found." });

            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                return Unauthorized(new { message = "Invalid or missing token." });

            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
                return NotFound(new { message = "Account not found." });
            if (!account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(new { message = "Account is not active." });

            medicalEvent.Type = model.Type?.Trim() ?? medicalEvent.Type;
            medicalEvent.Description = model.Description?.Trim() ?? medicalEvent.Description;
            medicalEvent.Note = model.Note?.Trim() ?? medicalEvent.Note;
            medicalEvent.Date = model.Date ?? medicalEvent.Date;
            medicalEvent.MedicationId = model.MedicationId ?? medicalEvent.MedicationId;

            var updated = await _medicalEventService.UpdateMedicalEventAsync(medicalEvent);
            if (!updated)
                return BadRequest(new { message = "Failed to update medical event." });

            return Ok(new { message = "Medical event updated successfully." });
        }

        [HttpDelete("DeleteMedicalEvent/{id}")]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<IActionResult> DeleteMedicalEvent(int id)
        {
            var medicalEvent = await _medicalEventService.GetMedicalEventByIdAsync(id);
            if (medicalEvent == null) return NotFound(new { message = "Medical event not found." });

            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                return Unauthorized(new { message = "Invalid or missing token." });

            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
                return NotFound(new { message = "Account not found." });
            if (!account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(new { message = "Account is not active." });

            var deleted = await _medicalEventService.DeleteMedicalEventAsync(id);
            if (!deleted)
                return BadRequest(new { message = "Failed to delete medical event." });

            return Ok(new { message = "Medical event deleted successfully." });
        }
    }
}