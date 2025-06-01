using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using SWP391_BE.DTO;
using System.Security.Claims;

namespace SWP391_BE.Controllers
{
    [ApiController]
    [Route("api/Parent")]
    public class ParentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public ParentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost("add-student")]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> AddStudent([FromBody] AddStudentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.StudentCode))
                return BadRequest("Student code is required.");

            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int parentId))
                return Unauthorized("Invalid account information.");

            var student = await _studentService.GetStudentByCodeAsync(request.StudentCode);
            if (student == null)
                return NotFound("Student not found.");

            if (student.ParentId != null)
                return Conflict("This student is already linked to a parent.");

            var success = await _studentService.LinkStudentToParentAsync(request.StudentCode, parentId);
            if (success)
                return Ok("Student successfully linked to parent.");

            return StatusCode(500, "Failed to link student to parent.");
        }

        [HttpGet("student-info/{studentCode}")]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> GetStudentInfo(string studentCode)
        {
            if (string.IsNullOrWhiteSpace(studentCode))
                return BadRequest("Student code is required.");

            var student = await _studentService.GetStudentByCodeAsync(studentCode);

            if (student == null)
                return NotFound("Student not found.");

            if (student.ParentId != null)
                return Conflict("This student is already linked to a parent.");

            var studentInfo = new StudentPreviewDto
            {
                Fullname = student.Fullname,
                StudentCode = student.StudentCode,
                Gender = student.Gender,
                DateOfBirth = student.DateOfBirth,
                ClassName = student.Class?.ClassName ?? "Unknown"
            };

            return Ok(studentInfo);
        }
    }
}
