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
    public class HealthCheckController : ControllerBase
    {
        private readonly IHealthCheckService _healthCheckService;

        public HealthCheckController(IHealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        // GET: api/HealthCheck
        [HttpGet]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<ActionResult<IEnumerable<HealthCheckListResponseDTO>>> GetAllHealthChecks()
        {
            try
            {
                var healthChecks = await _healthCheckService.GetAllHealthChecksAsync();
                var response = healthChecks.Select(h => new HealthCheckListResponseDTO
                {
                    HealthCheckID = h.HealthCheckID,
                    StudentName = h.Student?.Fullname,
                    NurseName = h.Nurse?.Fullname,
                    Result = h.Result,
                    Date = h.Date,
                    ConfirmByParent = h.ConfirmByParent
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/HealthCheck/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Nurse, Admin, Parent")]
        public async Task<ActionResult<HealthCheckResponseDTO>> GetHealthCheck(int id)
        {
            try
            {
                var healthCheck = await _healthCheckService.GetHealthCheckByIdAsync(id);
                if (healthCheck == null)
                {
                    return NotFound($"Health check with ID {id} not found");
                }

                var response = new HealthCheckResponseDTO
                {
                    HealthCheckID = healthCheck.HealthCheckID,
                    StudentID = healthCheck.StudentID,
                    NurseID = healthCheck.NurseID,
                    ParentID = healthCheck.ParentID,
                    Result = healthCheck.Result,
                    Date = healthCheck.Date,
                    ConfirmByParent = healthCheck.ConfirmByParent
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/HealthCheck/student/5
        [Authorize(Roles = "Nurse, Admin, Parent")]
        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<HealthCheckListResponseDTO>>> GetHealthChecksByStudent(int studentId)
        {
            try
            {
                var healthChecks = await _healthCheckService.GetHealthChecksByStudentIdAsync(studentId);
                var response = healthChecks.Select(h => new HealthCheckListResponseDTO
                {
                    HealthCheckID = h.HealthCheckID,
                    StudentName = h.Student?.Fullname,
                    NurseName = h.Nurse?.Fullname,
                    Result = h.Result,
                    Date = h.Date,
                    ConfirmByParent = h.ConfirmByParent
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/HealthCheck/nurse/5
        [HttpGet("nurse/{nurseId}")]
        [Authorize(Roles = "Nurse, Admin, Parent")]
        public async Task<ActionResult<IEnumerable<HealthCheckListResponseDTO>>> GetHealthChecksByNurse(int nurseId)
        {
            try
            {
                var healthChecks = await _healthCheckService.GetHealthChecksByNurseIdAsync(nurseId);
                var response = healthChecks.Select(h => new HealthCheckListResponseDTO
                {
                    HealthCheckID = h.HealthCheckID,
                    StudentName = h.Student?.Fullname,
                    NurseName = h.Nurse?.Fullname,
                    Result = h.Result,
                    Date = h.Date,
                    ConfirmByParent = h.ConfirmByParent
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/HealthCheck/parent/5
        [HttpGet("parent/{parentId}")]
        [Authorize(Roles = "Nurse, Admin, Parent")]
        public async Task<ActionResult<IEnumerable<HealthCheckListResponseDTO>>> GetHealthChecksByParent(int parentId)
        {
            try
            {
                var healthChecks = await _healthCheckService.GetHealthChecksByParentIdAsync(parentId);
                var response = healthChecks.Select(h => new HealthCheckListResponseDTO
                {
                    HealthCheckID = h.HealthCheckID,
                    StudentName = h.Student?.Fullname,
                    NurseName = h.Nurse?.Fullname,
                    Result = h.Result,
                    Date = h.Date,
                    ConfirmByParent = h.ConfirmByParent
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/HealthCheck
        [HttpPost]
        [Authorize(Roles = "Nurse, Admin")]
        public async Task<ActionResult<HealthCheckResponseDTO>> CreateHealthCheck(CreateHealthCheckRequestDTO request)
        {
            try
            {
                // Validate required fields
                if (request.StudentID <= 0)
                    return BadRequest("Student ID is required and must be greater than 0");
                if (request.NurseID <= 0)
                    return BadRequest("Nurse ID is required and must be greater than 0");
                if (request.ParentID <= 0)
                    return BadRequest("Parent ID is required and must be greater than 0");
                if (request.Result != null && request.Result.Length > 500)
                    return BadRequest("Result must not exceed 500 characters");

                var healthCheck = new BOs.Models.HealthCheck
                {
                    StudentID = request.StudentID,
                    NurseID = request.NurseID,
                    ParentID = request.ParentID,
                    Result = request.Result,
                    Date = DateTime.Now,
                    ConfirmByParent = null
                };

                var createdHealthCheck = await _healthCheckService.CreateHealthCheckAsync(healthCheck);
                var response = new HealthCheckResponseDTO
                {
                    HealthCheckID = createdHealthCheck.HealthCheckID,
                    StudentID = createdHealthCheck.StudentID,
                    NurseID = createdHealthCheck.NurseID,
                    ParentID = createdHealthCheck.ParentID,
                    Result = createdHealthCheck.Result,
                    Date = createdHealthCheck.Date,
                    ConfirmByParent = createdHealthCheck.ConfirmByParent
                };

                return CreatedAtAction(nameof(GetHealthCheck), new { id = response.HealthCheckID }, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/HealthCheck/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Nurse, Admin")]
        public async Task<IActionResult> UpdateHealthCheck(int id, UpdateHealthCheckRequestDTO request)
        {
            try
            {
                if (id != request.HealthCheckID)
                {
                    return BadRequest("ID mismatch");
                }

                var existingHealthCheck = await _healthCheckService.GetHealthCheckByIdAsync(id);
                if (existingHealthCheck == null)
                {
                    return NotFound($"Health check with ID {id} not found");
                }

                existingHealthCheck.Result = request.Result;
                await _healthCheckService.UpdateHealthCheckAsync(existingHealthCheck);

                return Ok(existingHealthCheck);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/HealthCheck/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Nurse, Admin")]
        public async Task<IActionResult> DeleteHealthCheck(int id)
        {
            try
            {
                var result = await _healthCheckService.DeleteHealthCheckAsync(id);
                if (!result)
                {
                    return NotFound($"Health check with ID {id} not found");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PATCH: api/HealthCheck/5/confirm
        [HttpPatch("{id}/confirm")]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> ConfirmHealthCheck(int id, HealthCheckConfirmationRequestDTO request)
        {
            try
            {
                if (id != request.HealthCheckID)
                {
                    return BadRequest("ID mismatch");
                }

                var result = await _healthCheckService.ConfirmHealthCheckAsync(id);
                if (!result)
                {
                    return NotFound($"Health check with ID {id} not found");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PATCH: api/HealthCheck/5/decline
        [HttpPatch("{id}/decline")]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> DeclineHealthCheck(int id, HealthCheckConfirmationRequestDTO request)
        {
            try
            {
                if (id != request.HealthCheckID)
                {
                    return BadRequest("ID mismatch");
                }

                var result = await _healthCheckService.DeclineHealthCheckAsync(id);
                if (!result)
                {
                    return NotFound($"Health check with ID {id} not found");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
} 