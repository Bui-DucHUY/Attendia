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
        [HttpPut("{classCrn}")]
        public async Task<IActionResult> UpdateClass(string classCrn, [FromBody] Classroom request)
        {
            var instructorIdClaim = User.FindFirst("InstructorID")?.Value;
            request.InstructorID = int.Parse(instructorIdClaim!);
            request.ClassCRN = classCrn; // Ensure CRN matches route

            var success = await _classroomRepo.UpdateClassroomAsync(request);
            if (!success) return BadRequest("Failed to update classroom.");
            return Ok(new { Message = "Classroom updated." });
        }

        [HttpDelete("{classCrn}")]
        public async Task<IActionResult> DeleteClass(string classCrn)
        {
            var instructorIdClaim = User.FindFirst("InstructorID")?.Value;
            var success = await _classroomRepo.DeleteClassroomAsync(classCrn, int.Parse(instructorIdClaim!));
            if (!success) return BadRequest("Failed to delete classroom. Ensure no active sessions exist first.");
            return Ok(new { Message = "Classroom deleted." });
        }
        [HttpGet("{classCrn}/roster")]
        public async Task<IActionResult> GetRoster(string classCrn)
        {
            var students = await _classroomRepo.GetEnrolledStudentsAsync(classCrn);
            return Ok(students);
        }

        [HttpDelete("{classCrn}/roster/{studentId}")]
        public async Task<IActionResult> RemoveStudent(string classCrn, string studentId)
        {
            var success = await _classroomRepo.RemoveStudentAsync(classCrn, studentId);
            if (!success) return BadRequest("Failed to remove student.");
            return Ok(new { Message = "Student removed." });
        }
    }
}