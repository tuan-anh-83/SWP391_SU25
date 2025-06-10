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
            // Nếu ParentId là 0 hoặc null thì không gán parent
            int? parentId = (dto.ParentId == null || dto.ParentId == 0) ? null : dto.ParentId;

            // Nếu có ParentId, kiểm tra parent có tồn tại không
            if (parentId != null)
            {
                var parent = await _accountService.GetAccountByIdAsync(parentId.Value);
                if (parent == null)
                    return BadRequest(new { message = "Parent does not exist." });
            }

            // Kiểm tra StudentCode đã tồn tại chưa
            var existing = await _studentService.GetStudentByCodeAsync(dto.StudentCode);
            if (existing != null)
                return BadRequest(new { message = "StudentCode already exists." });

            var student = new Student
            {
                Fullname = dto.Fullname,
                ClassId = dto.ClassId,
                StudentCode = dto.StudentCode,
                Gender = dto.Gender,
                ParentId = parentId,
                DateOfBirth = dto.DateOfBirth,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            var created = await _studentService.CreateStudentAsync(student);
            return Ok(new { message = "Student created successfully.", data = created });
        }
    }
}