using Dapper;
using Attendia.Data;
using Attendia.Models;

namespace Attendia.Repositories
{
    public class InstructorRepository : IInstructorRepository
    {
        private readonly DapperContext _context;

        public InstructorRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> RegisterInstructorAsync(Instructor instructor)
        {
            var query = @"
                INSERT INTO Instructors (InstructorMail, InstructorName, InstructorPassword)
                OUTPUT INSERTED.InstructorID
                VALUES (@InstructorMail, @InstructorName, @InstructorPassword)";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleAsync<int>(query, instructor);
            }
        }

        public async Task<Instructor?> GetInstructorByEmailAsync(string email)
        {
            var query = "SELECT * FROM Instructors WHERE InstructorMail = @Email";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Instructor>(query, new { Email = email });
            }
        }
    }
}