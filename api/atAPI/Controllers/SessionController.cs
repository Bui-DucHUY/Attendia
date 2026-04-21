using System;
using System.Threading.Tasks;
using Attendia.Models;
using Attendia.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Attendia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Only logged-in instructors can manage sessions
    public class SessionController : ControllerBase
    {
        private readonly ISessionRepository _sessionRepo;

        public SessionController(ISessionRepository sessionRepo)
        {
            _sessionRepo = sessionRepo;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest request)
        {
            try
            {
                var newSession = new Session
                {
                    SessionID = Guid.NewGuid(),
                    ClassCRN = request.ClassCRN,
                    StartTime = request.StartTime,
                    ExpiryTime = request.ExpiryTime,
                    RequiresImage = request.RequiresImage
                };

                await _sessionRepo.CreateSessionAsync(newSession);

                // Return the newly generated Guid so the Angular frontend can build the QR code
                return Ok(new { SessionID = newSession.SessionID, Message = "Session created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("{classCrn}")]
        public async Task<IActionResult> GetSessionsByClass(string classCrn)
        {
            try
            {
                var sessions = await _sessionRepo.GetSessionsByClassAsync(classCrn);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
        [HttpDelete("{sessionId}")]
        public async Task<IActionResult> DeleteSession(Guid sessionId)
        {
            var success = await _sessionRepo.DeleteSessionAsync(sessionId);
            if (!success) return BadRequest("Failed to delete session.");
            return Ok(new { Message = "Session deleted." });
        }
    }

    public class CreateSessionRequest
    {
        public string ClassCRN { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? ExpiryTime { get; set; }
        public bool RequiresImage { get; set; }
    }

}