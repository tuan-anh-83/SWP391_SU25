using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using SWP391_BE.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWP391_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthRecordController : ControllerBase
    {
        private readonly IHealthRecordService _healthRecordService;

        public HealthRecordController(IHealthRecordService healthRecordService)
        {
            _healthRecordService = healthRecordService;
        }

        // GET: api/HealthRecord
        [HttpGet]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<ActionResult<IEnumerable<HealthRecordResponseDTO>>> GetAllHealthRecords()
        {
            try
            {
                var healthRecords = await _healthRecordService.GetAllHealthRecords();
                return Ok(healthRecords);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving health records", error = ex.Message });
            }
        }

        // GET: api/HealthRecord/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<ActionResult<HealthRecordResponseDTO>> GetHealthRecordById(int id)
        {
            try
            {
                var healthRecord = await _healthRecordService.GetHealthRecordById(id);
                if (healthRecord == null)
                {
                    return NotFound(new { message = "Health record not found" });
                }
                return Ok(healthRecord);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving health record", error = ex.Message });
            }
        }

        // GET: api/HealthRecord/student/5
        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<ActionResult<IEnumerable<HealthRecordResponseDTO>>> GetHealthRecordsByStudentId(int studentId)
        {
            try
            {
                var healthRecords = await _healthRecordService.GetHealthRecordsByStudentId(studentId);
                return Ok(healthRecords);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving health records for student", error = ex.Message });
            }
        }

        // POST: api/HealthRecord
        [HttpPost]
        [Authorize(Roles = "Admin,Parent")]
        public async Task<ActionResult<HealthRecordResponseDTO>> CreateHealthRecord(CreateHealthRecordRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var healthRecord = new BOs.Models.HealthRecord
                {
                    ParentId = request.ParentId,
                    StudentId = request.StudentId,
                    StudentName = request.StudentName,
                    StudentCode = request.StudentCode,
                    Gender = request.Gender,
                    DateOfBirth = request.DateOfBirth,
                    Note = request.Note,
                    Height = request.Height,
                    Weight = request.Weight
                };

                var result = await _healthRecordService.CreateHealthRecord(healthRecord);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating health record", error = ex.Message });
            }
        }

        // PUT: api/HealthRecord/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Parent")]
        public async Task<IActionResult> UpdateHealthRecord(int id, UpdateHealthRecordRequestDTO request)
        {
            try
            {
                if (id != request.HealthRecordId)
                {
                    return BadRequest(new { message = "ID mismatch" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
               
                var existingRecord = await _healthRecordService.GetHealthRecordById(id);
                if (existingRecord == null)
                {
                    return NotFound(new { message = "Health record not found" });
                }

                existingRecord.Note = request.Note;
                existingRecord.Height = request.Height;
                existingRecord.Weight = request.Weight;
                existingRecord.DateOfBirth = request.DateOfBirth;
               var result = await _healthRecordService.UpdateHealthRecord(existingRecord);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating health record", error = ex.Message });
            }
        }

        // DELETE: api/HealthRecord/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Parent")]
        public async Task<IActionResult> DeleteHealthRecord(int id)
        {
            try
            {
                var result = await _healthRecordService.DeleteHealthRecord(id);
                if (!result)
                {
                    return NotFound(new { message = "Health record not found" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting health record", error = ex.Message });
            }
        }
    }
} 