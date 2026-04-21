using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attendia.Data;
using Attendia.Models;
using Dapper;

namespace Attendia.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly DapperContext _context;

        public SessionRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateSessionAsync(Session session)
        {
            var query = @"
                INSERT INTO Sessions (SessionID, ClassCRN, StartTime, ExpiryTime, RequiresImage)
                VALUES (@SessionID, @ClassCRN, @StartTime, @ExpiryTime, @RequiresImage)";

            using var connection = _context.CreateConnection();

            // Execute the insert
            await connection.ExecuteAsync(query, session);

            // Return the GUID to satisfy the ISessionRepository interface
            return session.SessionID;
        }

        public async Task<IEnumerable<Session>> GetSessionsByClassAsync(string classCrn)
        {
            var query = "SELECT * FROM Sessions WHERE ClassCRN = @ClassCrn ORDER BY StartTime DESC";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Session>(query, new { ClassCrn = classCrn });
        }

        public async Task<Session> GetSessionByIdAsync(Guid sessionId)
        {
            var query = "SELECT * FROM Sessions WHERE SessionID = @SessionID";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Session>(query, new { SessionID = sessionId });
        }

        public async Task<bool> DeleteSessionAsync(Guid sessionId)
        {
            var query = "DELETE FROM Sessions WHERE SessionID = @SessionID";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, new { SessionID = sessionId }) > 0;
        }
    }
}