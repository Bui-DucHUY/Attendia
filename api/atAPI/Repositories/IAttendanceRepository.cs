using Attendia.Models;

namespace Attendia.Repositories
{
    public interface IAttendanceRepository
    {
        Task<(bool Success, string Message)> CheckInStudentAsync(Guid sessionId, string studentId, string? imageUrl);
        Task<IEnumerable<AttendanceRecord>> GetAttendanceBySessionAsync(Guid sessionId);
        Task<bool> UpdateApprovalStatusAsync(Guid recordId, bool isApproved);
    }
}
