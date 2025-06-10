using Microsoft.AspNetCore.Mvc;
using Services;
using BOs.Models;
using SWP391_BE.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWP391_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;

        public ClassController(IClassService classService)
        {
            _classService = classService;
        }

        [HttpGet("GetAllClasses")]
        public async Task<IActionResult> GetAll()
        {
            var classes = await _classService.GetAllClassesAsync();
            var result = classes.ConvertAll(cls => new ClassDTO
            {
                ClassId = cls.ClassId,
                ClassName = cls.ClassName
            });
            return Ok(result);
        }

        [HttpGet("GetClassById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cls = await _classService.GetClassByIdAsync(id);
            if (cls == null)
                return NotFound(new { message = "Class not found." });
            return Ok(new ClassDTO { ClassId = cls.ClassId, ClassName = cls.ClassName });
        }

        [HttpPost("CreateClass")]
        public async Task<IActionResult> Create([FromBody] ClassCreateDTO dto)
        {
            var cls = new Class { ClassName = dto.ClassName };
            var created = await _classService.CreateClassAsync(cls);
            return Ok(new { message = "Class created successfully.", data = new ClassDTO { ClassId = created.ClassId, ClassName = created.ClassName } });
        }

        [HttpPut("UpdateClass/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClassUpdateDTO dto)
        {
            var cls = new Class { ClassId = id, ClassName = dto.ClassName };
            var result = await _classService.UpdateClassAsync(cls);
            if (!result)
                return NotFound(new { message = "Class not found." });
            return Ok(new { message = "Class updated successfully." });
        }

        [HttpDelete("DeleteClass/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _classService.DeleteClassAsync(id);
            if (!result)
                return NotFound(new { message = "Class not found." });
            return Ok(new { message = "Class deleted successfully." });
        }
    }
}