using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Attendia.Models;
using Attendia.Repositories;
using System;
using System.Threading.Tasks;

namespace Attendia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly ISessionRepository _sessionRepo;
        private readonly IClassroomRepository _classroomRepo;

        public AttendanceController(IAttendanceRepository attendanceRepo, ISessionRepository sessionRepo, IClassroomRepository classroomRepo)
        {
            _attendanceRepo = attendanceRepo;
            _sessionRepo = sessionRepo;
            _classroomRepo = classroomRepo;
        }

        [AllowAnonymous] // Leaves only this specific route open for students
        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] AttendanceRecord request)
        {
            var session = await _sessionRepo.GetSessionByIdAsync(request.SessionID);
            if (session == null) return NotFound(new { message = "Session not found." });
            if (session.ExpiryTime < DateTime.UtcNow) return BadRequest(new { message = "Session has expired." });

            var isEnrolled = await _classroomRepo.IsStudentEnrolledAsync(session.ClassCRN, request.StudentID);
            if (!isEnrolled) return BadRequest(new { message = "You are not enrolled in this class roster." });

            if (session.RequiresImage && string.IsNullOrEmpty(request.ImageUrl))
            {
                return BadRequest(new { message = "This class strictly requires a live photo to verify attendance." });
            }

            request.CheckInTime = DateTime.UtcNow;
            var success = await _attendanceRepo.CheckInAsync(request);
            if (!success) return BadRequest(new { message = "You have already checked into this session." });
            return Ok(new { Message = "Check-in successful." });
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetRecords(Guid sessionId)
        {
            var records = await _attendanceRepo.GetRecordsBySessionAsync(sessionId);
            return Ok(records);
        }

        [HttpPatch("approve/{recordId}")]
        public async Task<IActionResult> ApproveRecord(Guid recordId, [FromBody] ApproveRequest request)
        {
            var success = await _attendanceRepo.ApproveRecordAsync(recordId, request.IsApproved);
            if (!success) return BadRequest(new { message = "Failed to update status." });
            return Ok(new { Message = "Status updated." });
        }

        [HttpDelete("{recordId}")]
        public async Task<IActionResult> DeleteRecord(Guid recordId)
        {
            var success = await _attendanceRepo.DeleteRecordAsync(recordId);
            if (!success) return BadRequest(new { message = "Failed to delete record." });
            return Ok(new { Message = "Record deleted." });
        }
    }

    public class ApproveRequest
    {
        public bool IsApproved { get; set; }
    }
}