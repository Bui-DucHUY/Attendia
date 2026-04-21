using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Attendia.Models;
using Attendia.Repositories;

namespace Attendia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] //
    public class ClassroomController : ControllerBase
    {
        private readonly IClassroomRepository _classroomRepo;

        public ClassroomController(IClassroomRepository classroomRepo)
        {
            _classroomRepo = classroomRepo;
        }

        [HttpGet("my-classes")]
        public async Task<IActionResult> GetMyClasses()
        {
            var instructorIdClaim = User.FindFirst("InstructorID")?.Value;

            if (!int.TryParse(instructorIdClaim, out int instructorId))
                return Unauthorized("Invalid token claims.");

            var classrooms = await _classroomRepo.GetClassroomsByInstructorAsync(instructorId);
            return Ok(classrooms);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateClass([FromBody] Classroom request)
        {
            var instructorIdClaim = User.FindFirst("InstructorID")?.Value;

            if (!int.TryParse(instructorIdClaim, out int parsedId))
                return Unauthorized("Invalid token claims.");

            request.InstructorID = parsedId;

            var success = await _classroomRepo.CreateClassroomAsync(request);

            if (!success)
                return BadRequest("Failed to create classroom. The CRN might already exist.");

            return Ok(new { Message = "Classroom created successfully." });
        }

        [HttpPost("{classCrn}/enroll")]
        public async Task<IActionResult> EnrollStudents(string classCrn, [FromBody] List<string> studentIds)
        {
            if (studentIds == null || !studentIds.Any())
                return BadRequest("Student list cannot be empty.");

            try
            {
                var success = await _classroomRepo.EnrollStudentsAsync(classCrn, studentIds);
                return Ok(new { Message = $"Successfully enrolled {studentIds.Count} students." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}