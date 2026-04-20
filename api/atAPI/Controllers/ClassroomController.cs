using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Attendia.Models;
using Attendia.Repositories;
using System.Security.Claims;

namespace Attendia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Locks down all endpoints in this controller
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
            // Extract the InstructorID from the JWT token claims
            var instructorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(instructorIdClaim, out int instructorId))
                return Unauthorized("Invalid token claims.");

            var classrooms = await _classroomRepo.GetClassroomsByInstructorAsync(instructorId);
            return Ok(classrooms);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateClass([FromBody] Classroom request)
        {
            // Enforce the instructor ID from the token, preventing an instructor from creating a class for someone else
            var instructorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            request.InstructorID = int.Parse(instructorIdClaim!);

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