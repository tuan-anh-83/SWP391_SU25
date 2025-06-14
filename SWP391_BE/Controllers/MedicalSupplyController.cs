using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using BOs.Models;
using SWP391_BE.DTO;
using System.Threading.Tasks;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class MedicalSupplyController : ControllerBase
{
    private readonly IMedicalSupplyService _service;

    public MedicalSupplyController(IMedicalSupplyService service)
    {
        _service = service;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var supplies = await _service.GetAllAsync();
        var result = supplies.Select(s => new
        {
            s.MedicalSupplyId,
            s.Name,
            s.Type,
            s.Description,
            s.ExpiredDate
        });
        return Ok(result);
    }

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var s = await _service.GetByIdAsync(id);
        if (s == null)
            return NotFound(new { message = "Medical supply not found." });
        return Ok(new
        {
            s.MedicalSupplyId,
            s.Name,
            s.Type,
            s.Description,
            s.ExpiredDate
        });
    }

    [HttpPost("Create")]
    [Authorize(Roles = "Nurse,Admin")]
    public async Task<IActionResult> Create([FromBody] MedicalSupplyCreateDTO dto)
    {
        var supply = new MedicalSupply
        {
            Name = dto.Name,
            Type = dto.Type,
            Description = dto.Description,
            ExpiredDate = dto.ExpiredDate
        };
        var created = await _service.CreateAsync(supply);
        return Ok(new { message = "Medical supply created successfully.", data = created });
    }

    [HttpPut("Update/{id}")]
    [Authorize(Roles = "Nurse,Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] MedicalSupplyCreateDTO dto)
    {
        var supply = await _service.GetByIdAsync(id);
        if (supply == null)
            return NotFound(new { message = "Medical supply not found." });

        // Chỉ cập nhật trường hợp lệ, nếu không thì giữ nguyên
        if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != "string")
            supply.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.Type) && dto.Type != "string")
            supply.Type = dto.Type;
        if (!string.IsNullOrWhiteSpace(dto.Description) && dto.Description != "string")
            supply.Description = dto.Description;

        // Xử lý expiredDate: nếu là null hoặc giá trị mặc định của swagger thì giữ nguyên
        if (dto.ExpiredDate == null || dto.ExpiredDate == DateTime.MinValue)
        {
            // Không cập nhật, giữ nguyên giá trị cũ
        }
        else
        {
            supply.ExpiredDate = dto.ExpiredDate;
        }

        var result = await _service.UpdateAsync(supply);
        if (!result)
            return BadRequest(new { message = "Update failed." });

        return Ok(new { message = "Medical supply updated successfully." });
    }

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = "Nurse,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result)
            return NotFound(new { message = "Medical supply not found." });

        return Ok(new { message = "Medical supply deleted successfully." });
    }
}