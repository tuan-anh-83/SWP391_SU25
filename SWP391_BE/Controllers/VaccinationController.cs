using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using BOs.Models;
using SWP391_BE.DTO;
using System.Threading.Tasks;
using System.Linq;
using System;

[ApiController]
[Route("api/[controller]")]
public class VaccinationController : ControllerBase
{
    private readonly IVaccinationService _service;
    private readonly IStudentService _studentService; // Thêm dòng này

    public VaccinationController(IVaccinationService service, IStudentService studentService)
    {
        _service = service;
        _studentService = studentService; // Thêm dòng này
    }

    // ----------- VACCINE -----------
    [HttpGet("Vaccines")]
    public async Task<IActionResult> GetAllVaccines()
    {
        var vaccines = await _service.GetAllVaccinesAsync();
        return Ok(vaccines.Select(v => new
        {
            v.VaccineId,
            v.Name,
            v.Description
        }));
    }

    [HttpPost("Vaccine")]
    [Authorize(Roles = "Admin,Nurse")]
    public async Task<IActionResult> CreateVaccine([FromBody] VaccineCreateDTO dto)
    {
        var vaccine = new Vaccine
        {
            Name = dto.Name,
            Description = dto.Description
        };
        var created = await _service.CreateVaccineAsync(vaccine);
        return Ok(new { message = "Vaccine created successfully.", data = created });
    }

    // ----------- CAMPAIGN -----------
    [HttpGet("Campaigns")]
    public async Task<IActionResult> GetAllCampaigns()
    {
        var campaigns = await _service.GetAllCampaignsAsync();
        return Ok(campaigns.Select(c => new
        {
            c.CampaignId,
            c.Name,
            c.VaccineId,
            VaccineName = c.Vaccine?.Name,
            c.Date,
            c.Description
        }));
    }

    [HttpPost("Campaign")]
    [Authorize(Roles = "Admin,Nurse")]
    public async Task<IActionResult> CreateCampaign([FromBody] VaccinationCampaignCreateDTO dto)
    {
        var campaign = new VaccinationCampaign
        {
            Name = dto.Name,
            VaccineId = dto.VaccineId,
            Date = dto.Date,
            Description = dto.Description
        };
        var created = await _service.CreateCampaignAsync(campaign);

        // Lấy danh sách học sinh theo nhiều lớp
        var students = await _studentService.GetStudentsByClassIdsAsync(dto.ClassIds);
        foreach (var student in students)
        {
            if (student?.ParentId != null)
            {
                var consent = new VaccinationConsent
                {
                    CampaignId = created.CampaignId,
                    StudentId = student.StudentId,
                    ParentId = student.ParentId.Value,
                    IsAgreed = null,
                    Note = null,
                    DateConfirmed = null
                };
                await _service.CreateConsentAsync(consent);
            }
        }

        return Ok(new { message = "Campaign and notifications created successfully.", data = created });
    }

    // ----------- CONSENT -----------
    [HttpGet("Consents/{campaignId}")]
    [Authorize(Roles = "Admin,Nurse")]
    public async Task<IActionResult> GetConsentsByCampaign(int campaignId)
    {
        var consents = await _service.GetConsentsByCampaignAsync(campaignId);
        return Ok(consents.Select(c => new
        {
            c.ConsentId,
            c.CampaignId,
            c.StudentId,
            StudentName = c.Student?.Fullname,
            c.ParentId,
            ParentName = c.Parent?.Fullname,
            c.IsAgreed,
            c.Note,
            c.DateConfirmed
        }));
    }

    [HttpGet("ParentNotifications")]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> GetParentNotifications()
    {
        var accountIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int parentId))
            return Unauthorized(new { message = "Invalid or missing token." });

        var consents = await _service.GetConsentsByParentIdAsync(parentId);
        var result = consents.Select(c => new
        {
            c.ConsentId,
            c.CampaignId,
            CampaignName = c.Campaign?.Name,
            c.StudentId,
            StudentName = c.Student?.Fullname,
            c.IsAgreed,
            c.Note,
            c.DateConfirmed
        });
        return Ok(result);
    }

    [HttpPost("Consent")]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> CreateConsent([FromBody] VaccinationConsentCreateDTO dto)
    {
        var accountIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int parentId))
            return Unauthorized(new { message = "Invalid or missing token." });

        // Kiểm tra consent đã tồn tại chưa (theo campaignId, studentId, parentId)
        var existingConsent = await _service.GetConsentAsync(dto.CampaignId, dto.StudentId, parentId);
        VaccinationConsent consent;
        if (existingConsent != null)
        {
            // Update consent
            existingConsent.IsAgreed = dto.IsAgreed;
            existingConsent.Note = dto.Note;
            existingConsent.DateConfirmed = DateTime.UtcNow;
            consent = await _service.UpdateConsentAsync(existingConsent);
        }
        else
        {
            // Tạo mới consent
            consent = new VaccinationConsent
            {
                CampaignId = dto.CampaignId,
                StudentId = dto.StudentId,
                ParentId = parentId,
                IsAgreed = dto.IsAgreed,
                Note = dto.Note,
                DateConfirmed = DateTime.UtcNow
            };
            consent = await _service.CreateConsentAsync(consent);
        }
        return Ok(new { message = "Consent submitted successfully.", data = consent });
    }

    // ----------- RECORD -----------
    [HttpGet("Records/{campaignId}")]
    [Authorize(Roles = "Admin,Nurse")]
    public async Task<IActionResult> GetRecordsByCampaign(int campaignId)
    {
        var records = await _service.GetRecordsByCampaignAsync(campaignId);
        return Ok(records.Select(r => new
        {
            r.RecordId,
            r.CampaignId,
            r.StudentId,
            StudentName = r.Student?.Fullname,
            r.NurseId,
            NurseName = r.Nurse?.Fullname,
            r.DateInjected,
            r.Result,
            r.ImmediateReaction,
            r.Note
        }));
    }

    [HttpPost("Record")]
    [Authorize(Roles = "Nurse,Admin")]
    public async Task<IActionResult> CreateRecord([FromBody] VaccinationRecordCreateDTO dto)
    {
        // Lấy consent mới nhất hoặc đúng parent
        var consent = await _service.GetLatestConsentAsync(dto.CampaignId, dto.StudentId);
        if (consent == null || consent.IsAgreed != true)
        {
            return BadRequest(new { message = "Không thể tạo record: Phụ huynh chưa đồng ý tiêm." });
        }

        var record = new VaccinationRecord
        {
            CampaignId = dto.CampaignId,
            StudentId = dto.StudentId,
            NurseId = dto.NurseId,
            DateInjected = dto.DateInjected,
            Result = dto.Result,
            ImmediateReaction = dto.ImmediateReaction,
            Note = dto.Note
        };
        var created = await _service.CreateRecordAsync(record);
        return Ok(new { message = "Vaccination record created successfully.", data = created });
    }

    // ----------- FOLLOW UP -----------
    [HttpGet("FollowUps/{recordId}")]
    [Authorize(Roles = "Admin,Nurse")]
    public async Task<IActionResult> GetFollowUpsByRecord(int recordId)
    {
        var followUps = await _service.GetFollowUpsByRecordAsync(recordId);
        return Ok(followUps.Select(f => new
        {
            f.FollowUpId,
            f.RecordId,
            f.Date,
            f.Reaction,
            f.Note
        }));
    }

    [HttpPost("FollowUp")]
    [Authorize(Roles = "Nurse,Admin")]
    public async Task<IActionResult> CreateFollowUp([FromBody] VaccinationFollowUpCreateDTO dto)
    {
        var followUp = new VaccinationFollowUp
        {
            RecordId = dto.RecordId,
            Date = dto.Date,
            Reaction = dto.Reaction,
            Note = dto.Note
        };
        var created = await _service.CreateFollowUpAsync(followUp);
        return Ok(new { message = "Follow up created successfully.", data = created });
    }

    [HttpGet("RecordsByStudent/{studentId}")]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> GetRecordsByStudent(int studentId)
    {
        // Kiểm tra studentId có thuộc parent đang đăng nhập không (bảo mật)
        var accountIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int parentId))
            return Unauthorized(new { message = "Invalid or missing token." });

        // (Có thể kiểm tra student.ParentId == parentId nếu muốn chặt chẽ)
        var records = await _service.GetRecordsByStudentIdAsync(studentId);
        var result = records.Select(r => new
        {
            r.RecordId,
            r.CampaignId,
            CampaignName = r.Campaign?.Name,
            r.DateInjected,
            r.Result,
            r.ImmediateReaction,
            r.Note
        });
        return Ok(result);
    }
}