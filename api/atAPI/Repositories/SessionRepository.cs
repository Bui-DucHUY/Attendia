using Dapper;
using Attendia.Data;
using Attendia.Models;

namespace Attendia.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly DapperContext _context;

        public SessionRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Session>> GetSessionsByClassAsync(string classCrn)
        {
            var query = "SELECT * FROM Sessions WHERE ClassCRN = @ClassCRN";

            using (var connection = _context.CreateConnection())
            {
                // Dapper maps the @ClassCRN parameter anonymously
                return await connection.QueryAsync<Session>(query, new { ClassCRN = classCrn });
            }
        }

        public async Task<Guid> CreateSessionAsync(Session session)
        {
            session.SessionID = Guid.NewGuid();

            var query = @"
                INSERT INTO Sessions (SessionID, ClassCRN, StartTime, ExpiryTime, RequiresImage)
                VALUES (@SessionID, @ClassCRN, @StartTime, @ExpiryTime, @RequiresImage)";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, session);
                return session.SessionID;
            }
        }
    }
}