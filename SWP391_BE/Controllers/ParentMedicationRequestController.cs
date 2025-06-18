using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using SWP391_BE.DTO;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using BOs.Models;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class ParentMedicationRequestController : ControllerBase
{
    private readonly IParentMedicationRequestService _service;
    private readonly IAccountService _accountService;
    private readonly IStudentService _studentService;

    public ParentMedicationRequestController(
        IParentMedicationRequestService service,
        IAccountService accountService,
        IStudentService studentService)
    {
        _service = service;
        _accountService = accountService;
        _studentService = studentService;
    }

    [HttpPost("SendMedicationToStudent")]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> SendMedicationToStudent([FromBody] ParentMedicationRequestCreateDTO dto)
    {
        var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int parentId))
            return Unauthorized(new { message = "Invalid or missing token." });

        var parent = await _accountService.GetAccountByIdAsync(parentId);
        if (parent == null)
            return NotFound(new { message = "Parent not found." });

        var student = await _studentService.GetStudentByIdAsync(dto.StudentId);
        if (student == null)
            return NotFound(new { message = "Student not found." });

        if (student.ParentId != parentId)
            return Forbid("You are not the parent of this student.");

        var request = new ParentMedicationRequest
        {
            ParentId = parentId,
            StudentId = dto.StudentId,
            ParentNote = dto.ParentNote,
            DateCreated = DateTime.UtcNow,
            Status = "Pending",
            Medications = dto.Medications?.Select(m => new ParentMedicationDetail
            {
                Name = m.Name,
                Type = m.Type,
                Usage = m.Usage,
                Dosage = m.Dosage,
                ExpiredDate = m.ExpiredDate,
                Note = m.Note
            }).ToList()
        };

        var created = await _service.CreateAsync(request);
        if (!created)
            return BadRequest(new { message = "Failed to send medication request." });

        // Trả về cả RequestId cho parent
        return Ok(new { requestId = request.RequestId, message = "Medication request sent successfully." });
    }

    [HttpGet("GetById/{requestId}")]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> GetById(int requestId)
    {
        var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int parentId))
            return Unauthorized(new { message = "Invalid or missing token." });

        var request = await _service.GetByIdAsync(requestId);
        if (request == null || request.ParentId != parentId)
            return NotFound(new { message = "Request not found." });

        return Ok(new
        {
            request.RequestId,
            request.ParentId,
            ParentName = request.Parent?.Fullname,
            request.StudentId,
            StudentName = request.Student?.Fullname,
            request.ParentNote,
            request.NurseNote,
            request.DateCreated,
            request.Status,
            Medications = request.Medications.Select(m => new
            {
                m.MedicationDetailId,
                m.Name,
                m.Type,
                m.Usage,
                m.Dosage,
                m.ExpiredDate,
                m.Note
            }).ToList()
        });
    }

    [HttpPut("UpdateRequest/{requestId}")]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> UpdateRequest(int requestId, [FromBody] ParentMedicationRequestUpdateDTO dto)
    {
        var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int parentId))
            return Unauthorized(new { message = "Invalid or missing token." });

        var request = await _service.GetByIdAsync(requestId);
        if (request == null || request.ParentId != parentId)
            return NotFound(new { message = "Request not found." });

        if (request.Status != "Pending")
            return BadRequest(new { message = "Only pending requests can be updated." });

        request.ParentNote = dto.ParentNote;
        request.Medications = dto.Medications?.Select(m => new ParentMedicationDetail
        {
            Name = m.Name,
            Type = m.Type,
            Usage = m.Usage,
            Dosage = m.Dosage,
            ExpiredDate = m.ExpiredDate,
            Note = m.Note
        }).ToList();

        var updated = await _service.UpdateAsync(request);
        if (!updated)
            return BadRequest(new { message = "Failed to update request." });

        return Ok(new { message = "Request updated successfully." });
    }

    [HttpGet("History")]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> GetHistory()
    {
        var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int parentId))
            return Unauthorized(new { message = "Invalid or missing token." });

        var requests = await _service.GetByParentIdAsync(parentId);
        var result = requests.Select(r => new
        {
            r.RequestId,
            r.StudentId,
            StudentName = r.Student?.Fullname,
            r.ParentNote,
            r.NurseNote,
            r.DateCreated,
            r.Status,
            Medications = r.Medications.Select(m => new
            {
                m.MedicationDetailId,
                m.Name,
                m.Type,
                m.Usage,
                m.Dosage,
                m.ExpiredDate,
                m.Note
            }).ToList()
        });

        return Ok(result);
    }

    [HttpPut("ApproveRequest/{requestId}")]
    [Authorize(Roles = "Nurse,Admin")]
    public async Task<IActionResult> ApproveRequest(int requestId, [FromBody] ParentMedicationRequestApproveDTO dto)
    {
        if (dto.Status != "Approved" && dto.Status != "Rejected")
            return BadRequest(new { message = "Status must be 'Approved' or 'Rejected'." });

        var result = await _service.ApproveAsync(requestId, dto.Status, dto.NurseNote); 
        if (!result)
            return BadRequest(new { message = "Failed to update request status." });

        return Ok(new { message = $"Request has been {dto.Status.ToLower()}." });
    }

    [HttpGet("GetAllRequests")]
    [Authorize(Roles = "Nurse,Admin")]
    public async Task<IActionResult> GetAllRequests()
    {
        var requests = await _service.GetAllAsync();
        var result = requests.Select(r => new
        {
            r.RequestId,
            r.ParentId,
            ParentName = r.Parent?.Fullname,
            r.StudentId,
            StudentName = r.Student?.Fullname,
            r.ParentNote,
            r.NurseNote,
            r.DateCreated,
            r.Status,
            Medications = r.Medications.Select(m => new
            {
                m.MedicationDetailId,
                m.Name,
                m.Type,
                m.Usage,
                m.Dosage,
                m.ExpiredDate,
                m.Note
            }).ToList()
        });
        return Ok(result);
    }
}