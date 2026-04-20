using Attendia.Data;
using Attendia.Models;
using Attendia.Data;
using Attendia.Models;
using Attendia.Repositories;
using Dapper;

namespace Attendia.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly DapperContext _context;

        public AttendanceRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> CheckInStudentAsync(Guid sessionId, string studentId, string? imageUrl)
        {
            using (var connection = _context.CreateConnection())
            {
                // 1. Validate Session
                var sessionQuery = "SELECT * FROM Sessions WHERE SessionID = @SessionID";
                var session = await connection.QuerySingleOrDefaultAsync<Session>(sessionQuery, new { SessionID = sessionId });

                if (session == null)
                    return (false, "Session does not exist.");

                if (session.ExpiryTime.HasValue && DateTime.UtcNow > session.ExpiryTime.Value)
                    return (false, "This check-in session has expired.");

                // 2. Validate Enrollment Match (Enforces the StudentID + ClassCRN logic)
                var enrollQuery = "SELECT CAST(COUNT(1) AS BIT) FROM Enrollments WHERE ClassCRN = @ClassCRN AND StudentID = @StudentID";
                var isEnrolled = await connection.ExecuteScalarAsync<bool>(enrollQuery,
                    new { ClassCRN = session.ClassCRN, StudentID = studentId });

                if (!isEnrolled)
                    return (false, "Student ID doesn't match a valid enrollment for this class.");

                // 3. Record the Check-In
                var insertQuery = @"
                    INSERT INTO AttendanceRecords (RecordID, SessionID, StudentID, CheckInTime, ImageUrl, IsApproved)
                    VALUES (@RecordID, @SessionID, @StudentID, @CheckInTime, @ImageUrl, @IsApproved)";

                var record = new AttendanceRecord
                {
                    RecordID = Guid.NewGuid(),
                    SessionID = sessionId,
                    StudentID = studentId,
                    CheckInTime = DateTime.UtcNow,
                    ImageUrl = imageUrl,
                    IsApproved = false // Requires instructor manual approval later per your spec
                };

                await connection.ExecuteAsync(insertQuery, record);

                return (true, "Checked in successfully.");
            }
        }
        public async Task<IEnumerable<AttendanceRecord>> GetAttendanceBySessionAsync(Guid sessionId)
        {
            var query = "SELECT * FROM AttendanceRecords WHERE SessionID = @SessionID ORDER BY CheckInTime DESC";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<AttendanceRecord>(query, new { SessionID = sessionId });
            }
        }

        public async Task<bool> UpdateApprovalStatusAsync(Guid recordId, bool isApproved)
        {
            var query = "UPDATE AttendanceRecords SET IsApproved = @IsApproved WHERE RecordID = @RecordID";

            using (var connection = _context.CreateConnection())
            {
                var rowsAffected = await connection.ExecuteAsync(query, new { IsApproved = isApproved, RecordID = recordId });
                return rowsAffected > 0;
            }
        }
    }
}