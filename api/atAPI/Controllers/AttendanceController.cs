using Attendia.Repositories;
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
    }

    // DTO (Data Transfer Object) for incoming JSON requests
    public class CheckInRequest
    {
        public Guid SessionID { get; set; }
        public string StudentID { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}