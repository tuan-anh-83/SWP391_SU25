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
                    StudentName = healthCheck.Student?.Fullname,
                    NurseID = healthCheck.NurseID,
                    NurseName = healthCheck.Nurse?.Fullname,
                    ParentID = healthCheck.ParentID,
                    ParentName = healthCheck.Parent?.Fullname,
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
        public async Task<ActionResult<HealthCheckResponseDTO>> CreateHealthCheck(CreateHealthCheckRequestDTO request)
        {
            try
            {
                var healthCheck = new BOs.Models.HealthCheck
                {
                    StudentID = request.StudentID,
                    NurseID = request.NurseID,
                    ParentID = request.ParentID,
                    Result = request.Result
                };

                var createdHealthCheck = await _healthCheckService.CreateHealthCheckAsync(healthCheck);
                var response = new HealthCheckResponseDTO
                {
                    HealthCheckID = createdHealthCheck.HealthCheckID,
                    StudentID = createdHealthCheck.StudentID,
                    StudentName = createdHealthCheck.Student?.Fullname,
                    NurseID = createdHealthCheck.NurseID,
                    NurseName = createdHealthCheck.Nurse?.Fullname,
                    ParentID = createdHealthCheck.ParentID,
                    ParentName = createdHealthCheck.Parent?.Fullname,
                    Result = createdHealthCheck.Result,
                    Date = createdHealthCheck.Date,
                    ConfirmByParent = createdHealthCheck.ConfirmByParent
                };

                return CreatedAtAction(nameof(GetHealthCheck), new { id = response.HealthCheckID }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/HealthCheck/5
        [HttpPut("{id}")]
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

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/HealthCheck/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHealthCheck(int id)
        {
            try
            {
                var result = await _healthCheckService.DeleteHealthCheckAsync(id);
                if (!result)
                {
                    return NotFound($"Health check with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PATCH: api/HealthCheck/5/confirm
        [HttpPatch("{id}/confirm")]
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

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PATCH: api/HealthCheck/5/decline
        [HttpPatch("{id}/decline")]
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

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
} 