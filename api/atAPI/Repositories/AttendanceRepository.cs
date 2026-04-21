using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attendia.Data;
using Attendia.Models;
using Dapper;

namespace Attendia.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly DapperContext _context;

        // Matched exactly to your database_init.sql
        private readonly string _tableName = "AttendanceRecords";

        public AttendanceRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckInAsync(AttendanceRecord record)
        {
            var query = $@"
                IF NOT EXISTS (SELECT 1 FROM {_tableName} WHERE SessionID = @SessionID AND StudentID = @StudentID)
                BEGIN
                    -- NEWID() safely generates your UNIQUEIDENTIFIER Primary Key
                    INSERT INTO {_tableName} (RecordID, SessionID, StudentID, CheckInTime, ImageUrl, IsApproved)
                    VALUES (NEWID(), @SessionID, @StudentID, @CheckInTime, @ImageUrl, @IsApproved)
                END";

            using var connection = _context.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(query, record);
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<AttendanceRecord>> GetRecordsBySessionAsync(Guid sessionId)
        {
            var query = $"SELECT * FROM {_tableName} WHERE SessionID = @SessionID ORDER BY CheckInTime DESC";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<AttendanceRecord>(query, new { SessionID = sessionId });
        }

        public async Task<bool> ApproveRecordAsync(Guid recordId, bool isApproved)
        {
            var query = $"UPDATE {_tableName} SET IsApproved = @IsApproved WHERE RecordID = @RecordId";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, new { IsApproved = isApproved, RecordId = recordId }) > 0;
        }

        public async Task<bool> DeleteRecordAsync(Guid recordId)
        {
            var query = $"DELETE FROM {_tableName} WHERE RecordID = @RecordId";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, new { RecordId = recordId }) > 0;
        }
    }
}