using System.Security.Claims;
using BOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using SWP391_BE.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWP391_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalEventController : ControllerBase
    {
        private readonly IMedicalEventService _medicalEventService;
        private readonly IAccountService _accountService;

        public MedicalEventController(IMedicalEventService medicalEventService, IAccountService accountService)
        {
            _medicalEventService = medicalEventService;
            _accountService = accountService;
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
                me.Type,
                me.Description,
                me.Note,
                me.Date,
                Medications = me.MedicalEventMedications?.Select(mem => new
                {
                    mem.MedicationId,
                    mem.Medication?.Name,
                    mem.QuantityUsed
                }).ToList(),
                MedicalSupplies = me.MedicalEventMedicalSupplies?.Select(mes => new
                {
                    mes.MedicalSupplyId,
                    mes.MedicalSupply?.Name,
                    mes.QuantityUsed
                }).ToList()
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
                medicalEvent.Type,
                medicalEvent.Description,
                medicalEvent.Note,
                medicalEvent.Date,
                Medications = medicalEvent.MedicalEventMedications?.Select(mem => new
                {
                    mem.MedicationId,
                    mem.Medication?.Name,
                    mem.QuantityUsed
                }).ToList(),
                MedicalSupplies = medicalEvent.MedicalEventMedicalSupplies?.Select(mes => new
                {
                    mes.MedicalSupplyId,
                    mes.MedicalSupply?.Name,
                    mes.QuantityUsed
                }).ToList()
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
                Type = dto.Type.Trim(),
                Description = dto.Description?.Trim(),
                Note = dto.Note?.Trim(),
                Date = dto.Date
            };

            var medicationUsages = dto.Medications?.Select(m => new MedicalEventMedication
            {
                MedicationId = m.MedicationId,
                QuantityUsed = m.QuantityUsed
            }).ToList() ?? new List<MedicalEventMedication>();

            var supplyUsages = dto.MedicalSupplies?.Select(s => new MedicalEventMedicalSupply
            {
                MedicalSupplyId = s.MedicalSupplyId,
                QuantityUsed = s.QuantityUsed
            }).ToList() ?? new List<MedicalEventMedicalSupply>();

            var created = await _medicalEventService.CreateMedicalEventAsync(
                medicalEvent,
                medicationUsages,
                supplyUsages
            );
            if (!created)
                return BadRequest(new { message = "Failed to create medical event." });

            return CreatedAtAction(nameof(GetMedicalEventById), new { id = medicalEvent.MedicalEventId }, new { message = "Medical event created successfully.", medicalEventId = medicalEvent.MedicalEventId });
        }

        [HttpPut("UpdateMedicalEvent/{id}")]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<IActionResult> UpdateMedicalEvent(int id, [FromBody] MedicalEventUpdateDTO dto)
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

            if (!string.IsNullOrWhiteSpace(dto.Type) && dto.Type != "string")
                medicalEvent.Type = dto.Type.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Description) && dto.Description != "string")
                medicalEvent.Description = dto.Description.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Note) && dto.Note != "string")
                medicalEvent.Note = dto.Note.Trim();
            if (dto.Date.HasValue && dto.Date.Value != medicalEvent.Date)
                medicalEvent.Date = dto.Date.Value;

            var medicationUsages = dto.Medications?.Select(m => new MedicalEventMedication
            {
                MedicationId = m.MedicationId,
                QuantityUsed = m.QuantityUsed
            }).ToList() ?? new List<MedicalEventMedication>();

            var supplyUsages = dto.MedicalSupplies?.Select(s => new MedicalEventMedicalSupply
            {
                MedicalSupplyId = s.MedicalSupplyId,
                QuantityUsed = s.QuantityUsed
            }).ToList() ?? new List<MedicalEventMedicalSupply>();

            var updated = await _medicalEventService.UpdateMedicalEventAsync(
                medicalEvent,
                medicationUsages,
                supplyUsages
            );
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

        [HttpGet("Parent/MedicalEvents")]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> GetMedicalEventsForParent()
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int parentId))
                return Unauthorized(new { message = "Invalid or missing token." });

            var data = await _medicalEventService.GetMedicalEventsByParentIdAsync(parentId);

            var result = data.Select(d => new
            {
                StudentId = d.student.StudentId,
                StudentName = d.student.Fullname,
                Events = d.events.Select(me => new
                {
                    me.MedicalEventId,
                    me.Type,
                    me.Description,
                    me.Note,
                    me.Date,
                    NurseName = me.Nurse?.Fullname,
                    Medications = me.MedicalEventMedications?.Select(mem => new
                    {
                        mem.MedicationId,
                        mem.Medication?.Name,
                        mem.QuantityUsed
                    }).ToList(),
                    MedicalSupplies = me.MedicalEventMedicalSupplies?.Select(mes => new
                    {
                        mes.MedicalSupplyId,
                        mes.MedicalSupply?.Name,
                        mes.QuantityUsed
                    }).ToList()
                }).ToList(),
                Message = d.events.Any() ? null : "Hiện chưa có sự kiện y tế"
            });

            return Ok(result);
        }
    }
}