namespace Attendia.Repositories
{
    public interface IAttendanceRepository
    {
        Task<(bool Success, string Message)> CheckInStudentAsync(Guid sessionId, string studentId, string? imageUrl);
    }
}
