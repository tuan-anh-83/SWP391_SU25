using Microsoft.AspNetCore.Mvc;
using Services;
using BOs.Models;
using SWP391_BE.DTO;
using System.Threading.Tasks;

namespace SWP391_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IAccountService _accountService;

        public StudentController(IStudentService studentService, IAccountService accountService)
        {
            _studentService = studentService;
            _accountService = accountService;
        }

        [HttpPost("CreateStudent")]
        public async Task<IActionResult> CreateStudent([FromBody] StudentCreateDTO dto)
        {
            int? parentId = (dto.ParentId == null || dto.ParentId == 0) ? null : dto.ParentId;

            if (parentId != null)
            {
                var parent = await _accountService.GetAccountByIdAsync(parentId.Value);
                if (parent == null)
                    return BadRequest(new { message = "Parent does not exist." });
            }

            var existing = await _studentService.GetStudentByCodeAsync(dto.StudentCode);
            if (existing != null)
                return BadRequest(new { message = "StudentCode already exists." });

            string gender = string.IsNullOrWhiteSpace(dto.Gender)
                ? null
                : char.ToUpper(dto.Gender[0]) + dto.Gender.Substring(1).ToLower();

            var student = new Student
            {
                Fullname = dto.Fullname,
                ClassId = dto.ClassId,
                StudentCode = dto.StudentCode,
                Gender = gender,
                ParentId = parentId,
                DateOfBirth = dto.DateOfBirth,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            var created = await _studentService.CreateStudentAsync(student);

            // Lấy lại student vừa tạo để lấy navigation property
            var s = await _studentService.GetStudentByIdAsync(created.StudentId);
            var result = new
            {
                s.StudentId,
                s.Fullname,
                s.ClassId,
                ClassName = s.Class?.ClassName,
                s.StudentCode,
                s.Gender,
                s.ParentId,
                ParentName = s.Parent?.Fullname,
                s.DateOfBirth,
                s.CreatedAt,
                s.UpdateAt
            };

            return Ok(new { message = "Student created successfully.", data = result });
        }

        [HttpGet("GetStudentById/{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var s = await _studentService.GetStudentByIdAsync(id);
            if (s == null)
                return NotFound(new { message = "Student not found." });
            var result = new
            {
                s.StudentId,
                s.Fullname,
                s.ClassId,
                ClassName = s.Class?.ClassName,
                s.StudentCode,
                s.Gender,
                s.ParentId,
                ParentName = s.Parent?.Fullname,
                s.DateOfBirth,
                s.CreatedAt,
                s.UpdateAt
            };
            return Ok(result);
        }

        [HttpGet("GetAllStudents")]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _studentService.GetAllStudentsAsync();
            var result = students.Select(s => new
            {
                s.StudentId,
                s.Fullname,
                s.ClassId,
                ClassName = s.Class?.ClassName,
                s.StudentCode,
                s.Gender,
                s.ParentId,
                ParentName = s.Parent?.Fullname,
                s.DateOfBirth,
                s.CreatedAt,
                s.UpdateAt
            });
            return Ok(result);
        }

        [HttpGet("GetStudentByCode/{studentCode}")]
        public async Task<IActionResult> GetStudentByCode(string studentCode)
        {
            var s = await _studentService.GetStudentByCodeAsync(studentCode);
            if (s == null)
                return NotFound(new { message = "Student not found." });
            var result = new
            {
                s.StudentId,
                s.Fullname,
                s.ClassId,
                ClassName = s.Class?.ClassName,
                s.StudentCode,
                s.Gender,
                s.ParentId,
                ParentName = s.Parent?.Fullname,
                s.DateOfBirth,
                s.CreatedAt,
                s.UpdateAt
            };
            return Ok(result);
        }

        [HttpPut("UpdateStudent/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentUpdateDTO dto)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound(new { message = "Student not found." });

            // Chỉ cập nhật trường nào có giá trị hợp lệ
            if (!string.IsNullOrWhiteSpace(dto.Fullname)) student.Fullname = dto.Fullname;
            if (dto.ClassId.HasValue && dto.ClassId.Value != 0) student.ClassId = dto.ClassId.Value;
            if (!string.IsNullOrWhiteSpace(dto.StudentCode) && dto.StudentCode != "string") student.StudentCode = dto.StudentCode;
            if (!string.IsNullOrWhiteSpace(dto.Gender) && dto.Gender != "string") student.Gender = dto.Gender;
            if (dto.DateOfBirth.HasValue && dto.DateOfBirth.Value != student.DateOfBirth)
                student.DateOfBirth = dto.DateOfBirth.Value;

            // ParentId có thể null hoặc 0
            if (dto.ParentId == null || dto.ParentId == 0)
            {
                student.ParentId = null;
            }
            else
            {
                var parent = await _accountService.GetAccountByIdAsync(dto.ParentId.Value);
                if (parent == null)
                    return BadRequest(new { message = "Parent does not exist." });
                student.ParentId = dto.ParentId;
            }

            student.UpdateAt = DateTime.UtcNow;

            var result = await _studentService.UpdateStudentAsync(student);
            if (!result)
                return BadRequest(new { message = "Update failed." });

            return Ok(new { message = "Student updated successfully." });
        }

        [HttpDelete("DeleteStudent/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var result = await _studentService.DeleteStudentAsync(id);
            if (!result)
                return NotFound(new { message = "Student not found." });

            return Ok(new { message = "Student deleted successfully." });
        }
    }
}