using Microsoft.AspNetCore.Mvc;
using Services;
using BOs.Models;
using SWP391_BE.DTO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SWP391_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicationController : ControllerBase
    {
        private readonly IMedicationService _service;

        public MedicationController(IMedicationService service)
        {
            _service = service;
        }

        [HttpGet("GetAllMedications")]
        public async Task<IActionResult> GetAllMedications()
        {
            var meds = await _service.GetAllAsync();
            return Ok(meds);
        }

        [HttpGet("GetMedicationById/{id}")]
        public async Task<IActionResult> GetMedicationById(int id)
        {
            var med = await _service.GetByIdAsync(id);
            if (med == null)
                return NotFound(new { message = "Medication not found." });
            return Ok(med);
        }

        [HttpPost("CreateMedication")]
        public async Task<IActionResult> CreateMedication([FromBody] MedicationCreateDTO dto)
        {
            var medication = new Medication
            {
                Name = dto.Name,
                Type = dto.Type,
                Usage = dto.Usage,
                ExpiredDate = dto.ExpiredDate
            };
            var created = await _service.CreateAsync(medication);
            return Ok(new { message = "Medication created successfully.", data = created });
        }

        [HttpPut("UpdateMedication/{id}")]
        public async Task<IActionResult> UpdateMedication(int id, [FromBody] MedicationUpdateDTO dto)
        {
            var medication = await _service.GetByIdAsync(id);
            if (medication == null)
                return NotFound(new { message = "Medication not found." });

            // Chỉ cập nhật trường nào có giá trị hợp lệ
            if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != "string")
                medication.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.Type) && dto.Type != "string")
                medication.Type = dto.Type;
            if (!string.IsNullOrWhiteSpace(dto.Usage) && dto.Usage != "string")
                medication.Usage = dto.Usage;
            if (dto.ExpiredDate.HasValue && dto.ExpiredDate.Value != medication.ExpiredDate)
                medication.ExpiredDate = dto.ExpiredDate.Value;

            var result = await _service.UpdateAsync(medication);
            if (!result)
                return BadRequest(new { message = "Update failed." });

            return Ok(new { message = "Medication updated successfully." });
        }

        [HttpDelete("DeleteMedication/{id}")]
        public async Task<IActionResult> DeleteMedication(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = "Medication not found." });

            return Ok(new { message = "Medication deleted successfully." });
        }
    }
}