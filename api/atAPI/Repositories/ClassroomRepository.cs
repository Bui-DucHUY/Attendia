using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Attendia.Data;
using Attendia.Models;
using Dapper;

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
            var query = "SELECT * FROM Classrooms WHERE InstructorID = @InstructorId";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Classroom>(query, new { InstructorId = instructorId });
        }

        public async Task<bool> CreateClassroomAsync(Classroom classroom)
        {
            var query = @"
                INSERT INTO Classrooms (ClassCRN, ClassName, ClassDescription, InstructorID)
                VALUES (@ClassCRN, @ClassName, @ClassDescription, @InstructorID)";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, classroom) > 0;
        }

        public async Task<bool> UpdateClassroomAsync(Classroom classroom)
        {
            var query = "UPDATE Classrooms SET ClassName = @ClassName, ClassDescription = @ClassDescription WHERE ClassCRN = @ClassCRN AND InstructorID = @InstructorID";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, classroom) > 0;
        }

        public async Task<bool> DeleteClassroomAsync(string classCrn, int instructorId)
        {
            var query = "DELETE FROM Classrooms WHERE ClassCRN = @ClassCRN AND InstructorID = @InstructorID";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, new { ClassCRN = classCrn, InstructorID = instructorId }) > 0;
        }

        // --- ROSTER MANAGEMENT ---
        public async Task<bool> EnrollStudentsAsync(string classCrn, List<string> studentIds)
        {
            var query = "IF NOT EXISTS (SELECT 1 FROM Enrollments WHERE StudentID = @StudentID AND ClassCRN = @ClassCRN) " +
                        "INSERT INTO Enrollments (StudentID, ClassCRN) VALUES (@StudentID, @ClassCRN)";
            using var connection = _context.CreateConnection();
            var enrollments = studentIds.Select(id => new { StudentID = id, ClassCRN = classCrn });
            await connection.ExecuteAsync(query, enrollments);
            return true;
        }

        public async Task<IEnumerable<string>> GetEnrolledStudentsAsync(string classCrn)
        {
            var query = "SELECT StudentID FROM Enrollments WHERE ClassCRN = @ClassCRN ORDER BY StudentID";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<string>(query, new { ClassCRN = classCrn });
        }

        public async Task<bool> RemoveStudentAsync(string classCrn, string studentId)
        {
            var query = "DELETE FROM Enrollments WHERE ClassCRN = @ClassCRN AND StudentID = @StudentID";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, new { ClassCRN = classCrn, StudentID = studentId }) > 0;
        }

        public async Task<bool> IsStudentEnrolledAsync(string classCrn, string studentId)
        {
            var query = "SELECT COUNT(1) FROM Enrollments WHERE ClassCRN = @ClassCRN AND StudentID = @StudentID";
            using var connection = _context.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(query, new { ClassCRN = classCrn, StudentID = studentId });
            return count > 0;
        }
    }
}