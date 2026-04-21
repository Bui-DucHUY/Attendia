using Attendia.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendia.Repositories
{
    public interface IClassroomRepository
    {
        Task<IEnumerable<Classroom>> GetClassroomsByInstructorAsync(int instructorId);
        Task<bool> CreateClassroomAsync(Classroom classroom);
        Task<bool> UpdateClassroomAsync(Classroom classroom);
        Task<bool> DeleteClassroomAsync(string classCrn, int instructorId);

        // Roster Management
        Task<bool> EnrollStudentsAsync(string classCrn, List<string> studentIds);
        Task<IEnumerable<string>> GetEnrolledStudentsAsync(string classCrn);
        Task<bool> RemoveStudentAsync(string classCrn, string studentId);
        Task<bool> IsStudentEnrolledAsync(string classCrn, string studentId);
    }
}