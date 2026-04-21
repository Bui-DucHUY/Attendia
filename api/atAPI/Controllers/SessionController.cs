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
    public class SessionController : ControllerBase
    {
        private readonly ISessionRepository _sessionRepo;

        public SessionController(ISessionRepository sessionRepo)
        {
            _sessionRepo = sessionRepo;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSession([FromBody] Session request)
        {
            request.SessionID = Guid.NewGuid();
            var sessionId = await _sessionRepo.CreateSessionAsync(request);
            return Ok(new { Message = "Session created.", SessionID = sessionId });
        }

        [HttpGet("class/{classCrn}")]
        public async Task<IActionResult> GetSessionsByClass(string classCrn)
        {
            var sessions = await _sessionRepo.GetSessionsByClassAsync(classCrn);
            return Ok(sessions);
        }

        [HttpDelete("{sessionId}")]
        public async Task<IActionResult> DeleteSession(Guid sessionId)
        {
            var success = await _sessionRepo.DeleteSessionAsync(sessionId);
            if (!success) return BadRequest("Failed to delete session.");
            return Ok(new { Message = "Session deleted." });
        }

        // --- NEW: Expose the End Session route ---
        [HttpPatch("{sessionId}/end")]
        public async Task<IActionResult> EndSession(Guid sessionId)
        {
            var success = await _sessionRepo.EndSessionAsync(sessionId);
            if (!success) return BadRequest("Failed to end session.");
            return Ok(new { Message = "Session ended." });
        }

        [AllowAnonymous]
        [HttpGet("public/{sessionId}")]
        public async Task<IActionResult> GetPublicSession(Guid sessionId)
        {
            var session = await _sessionRepo.GetSessionByIdAsync(sessionId);
            if (session == null) return NotFound(new { message = "Session not found." });

            return Ok(new
            {
                classCRN = session.ClassCRN,
                requiresImage = session.RequiresImage,
                isExpired = session.ExpiryTime < DateTime.UtcNow
            });
        }
    }
}