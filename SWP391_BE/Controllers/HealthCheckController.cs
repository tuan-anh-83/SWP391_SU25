using Microsoft.AspNetCore.Mvc;
using BOs.Models;
using Services;
using SWP391_BE.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Services.Email;

namespace SWP391_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private readonly IHealthCheckService _service;
        private readonly IStudentService _studentService;
        private readonly IEmailService _emailService;

        public HealthCheckController(IHealthCheckService service, IStudentService studentService, IEmailService emailService)
        {
            _service = service;
            _studentService = studentService;
            _emailService = emailService;
        }

        //[HttpPost]
        //[Authorize(Roles = "Nurse,Admin")]
        //public async Task<ActionResult<HealthCheckResponseDTO>> Create([FromBody] CreateHealthCheckDTO dto)
        //{
        //    var model = new HealthCheck
        //    {
        //        NurseID = dto.NurseId,
        //        StudentID = dto.StudentId,
        //        ParentID = dto.ParentId,
        //        Date = System.DateTime.UtcNow,
        //        HealthCheckDescription = dto.HealthCheckDescription
        //    };
        //    var result = await _service.CreateHealthCheckAsync(model);
        //    return Ok(new HealthCheckResponseDTO(result));
        //}

        [HttpPut("{id}")]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateHealthCheckDTO dto)
        {
            var existing = await _service.GetHealthCheckByIdAsync(id);
            if (existing == null) return NotFound();
            existing.Result = dto.Result;
            existing.Height = dto.Height;
            existing.Weight = dto.Weight;
            existing.LeftEye = dto.LeftEye;
            existing.RightEye = dto.RightEye;
            var result = await _service.UpdateHealthCheckAsync(existing);
            if (result == null) return NotFound();
            return Ok(new HealthCheckResponseDTO(result));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<ActionResult<HealthCheckResponseDTO>> GetById(int id)
        {
            var result = await _service.GetHealthCheckByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        [Authorize]
        public async Task<ActionResult<List<HealthCheckResponseDTO>>> GetByStudent(int studentId)
        {
            var list = await _service.GetHealthChecksByStudentIdAsync(studentId);
            return Ok(list.Select(hc => new HealthCheckResponseDTO(hc)).ToList());
        }

        [HttpGet("parent/{parentId}")]
        [Authorize]
        public async Task<ActionResult<List<HealthCheckResponseDTO>>> GetByParent(int parentId)
        {
            var list = await _service.GetHealthChecksByParentIdAsync(parentId);
            return Ok(list.Select(hc => new HealthCheckResponseDTO(hc)).ToList());
        }

        [HttpGet("nurse/{nurseId}")]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<ActionResult<List<HealthCheckResponseDTO>>> GetByNurse(int nurseId)
        {
            var list = await _service.GetHealthChecksByNurseIdAsync(nurseId);
            return Ok(list.Select(hc => new HealthCheckResponseDTO(hc)).ToList());
        }

        [HttpGet]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<ActionResult<List<HealthCheckResponseDTO>>> GetAll()
        {
            var list = await _service.GetAllHealthChecksAsync();
            return Ok(list.Select(hc => new HealthCheckResponseDTO(hc)).ToList());
        }

        [HttpGet("date/{date}")]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<ActionResult<List<HealthCheckResponseDTO>>> GetByDate(string date)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
                return BadRequest("Invalid date format. Use yyyy-MM-dd.");
            var list = await _service.GetHealthChecksByDateAsync(parsedDate);
            return Ok(list.Select(hc => new HealthCheckResponseDTO(hc)).ToList());
        }
        [Authorize(Roles = "Nurse,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteHealthCheckAsync(id);
            if (!result) return NotFound();
            return Ok(result);
        }

        [HttpPost("CreateHealthCheckList")]
      
        public async Task<IActionResult> CreateHealthCheckList([FromBody] HealthCheckBatchCreateDTO dto)
        {
            var students = await _studentService.GetStudentsByClassIdsAsync(dto.ClassIds);
            var createdChecks = new List<HealthCheckResponseDTO>();
            foreach (var student in students)
            {
              
                    var healthCheck = new HealthCheck
                    {
                        NurseID = dto.NurseId,
                        StudentID = student.StudentId,
                        ParentID = student.ParentId,
                        Date = dto.Date,
                        HealthCheckDescription = dto.HealthCheckDescription,
                    };
                    var created = await _service.CreateHealthCheckAsync(healthCheck);
                    createdChecks.Add(new HealthCheckResponseDTO(created));
                
            }
            return Ok(new { message = "Batch health checks created and parents notified.", data = createdChecks });
        }

        [HttpGet("ParentNotifications")]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> GetParentNotifications()
        {
            var accountIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int parentId))
                return Unauthorized(new { message = "Invalid or missing token." });

            var healthChecks = await _service.GetHealthChecksByParentIdAsync(parentId);
            var result = healthChecks.Select(hc => new
            {
                hc.HealthCheckID,
                hc.StudentID,
                StudentName = hc.Student?.Fullname,
                hc.Date,
                hc.HealthCheckDescription
            });
            return Ok(result);
        }
    }
}
