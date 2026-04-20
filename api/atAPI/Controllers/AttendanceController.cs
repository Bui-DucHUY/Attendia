using System;
using System.Threading.Tasks;
using Attendia.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Attendia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceRepository _attendanceRepo;

        public AttendanceController(IAttendanceRepository attendanceRepo)
        {
            _attendanceRepo = attendanceRepo;
        }

        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.StudentID))
            {
                return BadRequest(new { message = "Student ID is required." });
            }

            try
            {
                var result = await _attendanceRepo.CheckInStudentAsync(request.SessionID, request.StudentID, request.ImageUrl);

                if (result.Success)
                {
                    return Ok(new { message = result.Message });
                }
                else
                {
                    return BadRequest(new { message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // GET: api/attendance/session/{sessionId}
        [HttpGet("session/{sessionId}")]
        [Authorize] // Only logged-in instructors can view attendance lists
        public async Task<IActionResult> GetSessionAttendance(Guid sessionId)
        {
            try
            {
                var records = await _attendanceRepo.GetAttendanceBySessionAsync(sessionId);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // PATCH: api/attendance/approve/{recordId}
        [HttpPatch("approve/{recordId}")]
        [Authorize] // Only logged-in instructors can approve/deny records
        public async Task<IActionResult> UpdateApprovalStatus(Guid recordId, [FromBody] ApprovalRequest request)
        {
            try
            {
                var success = await _attendanceRepo.UpdateApprovalStatusAsync(recordId, request.IsApproved);

                if (!success)
                    return NotFound(new { message = "Attendance record not found." });

                return Ok(new { message = "Approval status updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
    }

    // DTO (Data Transfer Object) for incoming JSON requests
    public class CheckInRequest
    {
        public Guid SessionID { get; set; }
        public string StudentID { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }

    // DTO for the approval request
    public class ApprovalRequest
    {
        public bool IsApproved { get; set; }
    }
}