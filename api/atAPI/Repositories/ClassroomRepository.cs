using Dapper;
using Attendia.Data;
using Attendia.Models;
using Microsoft.Data.SqlClient;

namespace Attendia.Repositories
{
    public class ClassroomRepository : IClassroomRepository
    {
        private readonly DapperContext _context;

        public ClassroomRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Classroom>> GetClassroomsByInstructorAsync(int instructorId)
        {
            var query = "SELECT * FROM Classrooms WHERE InstructorID = @InstructorID";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Classroom>(query, new { InstructorID = instructorId });
            }
        }

        public async Task<bool> CreateClassroomAsync(Classroom classroom)
        {
            var query = @"
                INSERT INTO Classrooms (ClassCRN, ClassName, ClassDescription, InstructorID)
                VALUES (@ClassCRN, @ClassName, @ClassDescription, @InstructorID)";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var rowsAffected = await connection.ExecuteAsync(query, classroom);
                    return rowsAffected > 0;
                }
                catch (SqlException ex) when (ex.Number == 2627) // Primary key violation (CRN already exists)
                {
                    return false;
                }
            }
        }

        public async Task<bool> EnrollStudentsAsync(string classCrn, List<string> studentIds)
        {
            // Dapper makes batch inserts incredibly easy by passing a list of anonymous objects
            var enrollments = studentIds.Select(id => new { ClassCRN = classCrn, StudentID = id }).ToList();

            var query = @"
                INSERT INTO Enrollments (ClassCRN, StudentID)
                VALUES (@ClassCRN, @StudentID)";

            using (var connection = _context.CreateConnection())
            {
                var rowsAffected = await connection.ExecuteAsync(query, enrollments);
                return rowsAffected == studentIds.Count;
            }
        }
    }
}