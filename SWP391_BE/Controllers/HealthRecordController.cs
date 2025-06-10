using BOs.Models;
using Microsoft.AspNetCore.Mvc;
using Repos;
using Services;
using SWP391_BE.DTO;

namespace SWP391_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthRecordController : ControllerBase
    {
        private readonly IHealthRecordService _healthRecordService;
        private readonly IAccountService _accountService;
        private readonly IStudentRepo _studentRepo;

        public HealthRecordController(
            IHealthRecordService healthRecordService,
            IAccountService accountService,
            IStudentRepo studentRepo)
        {
            _healthRecordService = healthRecordService;
            _accountService = accountService;
            _studentRepo = studentRepo;
        }

        [HttpGet("GetAllHealthRecords")]
        public async Task<IActionResult> GetAll()
        {
            var records = await _healthRecordService.GetAllHealthRecordsAsync();
            return Ok(records);
        }

        [HttpGet("GetHealthRecordById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var record = await _healthRecordService.GetHealthRecordByIdAsync(id);
            if (record == null)
                return NotFound(new { message = "Health record not found." });
            return Ok(record);
        }

        [HttpGet("GetHealthRecordsByStudentId/{studentId}")]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            var records = await _healthRecordService.GetHealthRecordsByStudentIdAsync(studentId);
            return Ok(records);
        }

        [HttpPost("CreateHealthRecord")]
        public async Task<IActionResult> Create([FromBody] HealthRecordCreateDTO dto)
        {
            // 1. Kiểm tra ParentId có tồn tại không
            var parent = await _accountService.GetAccountByIdAsync(dto.ParentId);
            if (parent == null)
                return BadRequest(new { message = "Parent does not exist." });

            // 2. Kiểm tra StudentCode có tồn tại và có phải là con của Parent không
            var student = await _studentRepo.GetStudentByCodeAsync(dto.StudentCode);
            if (student == null)
                return BadRequest(new { message = "Student does not exist." });

            if (student.ParentId != dto.ParentId)
                return BadRequest(new { message = "Student is not a child of this parent." });

            var healthRecord = new HealthRecord
            {
                ParentId = dto.ParentId,
                StudentCode = dto.StudentCode,
                Note = dto.Note,
                Height = dto.Height,
                Weight = dto.Weight
            };
            var created = await _healthRecordService.CreateHealthRecordAsync(healthRecord);
            return Ok(new { message = "Health record created successfully.", data = created });
        }

        [HttpPut("UpdateHealthRecord/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] HealthRecordUpdateDTO dto)
        {
            var existing = await _healthRecordService.GetHealthRecordByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Health record not found." });

            existing.Height = dto.Height;
            existing.Weight = dto.Weight;
            if (dto.Note != null && dto.Note != "string")
                existing.Note = dto.Note;

            var updated = await _healthRecordService.UpdateHealthRecordAsync(existing);
            if (updated == null)
                return NotFound(new { message = "Health record not found." });

            // Chỉ trả về DTO
            return Ok(new
            {
                message = "Health record updated successfully.",
                data = ToDTO(updated)
            });
        }

        private HealthRecordDTO ToDTO(HealthRecord record)
        {
            return new HealthRecordDTO
            {
                HealthRecordId = record.HealthRecordId,
                ParentId = record.ParentId,
                StudentId = record.StudentId,
                StudentName = record.StudentName,
                StudentCode = record.StudentCode,
                Gender = record.Gender,
                DateOfBirth = record.DateOfBirth,
                Note = record.Note,
                Height = record.Height,
                Weight = record.Weight,
                BMI = record.BMI,
                NutritionStatus = record.NutritionStatus
            };
        }

        [HttpDelete("DeleteHealthRecord/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _healthRecordService.DeleteHealthRecordAsync(id);
            if (!result)
                return NotFound(new { message = "Health record not found." });
            return Ok(new { message = "Health record deleted successfully." });
        }
    }
}