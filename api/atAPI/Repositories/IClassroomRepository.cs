using Attendia.Models;

namespace Attendia.Repositories
{
    public interface IClassroomRepository
    {
        Task<IEnumerable<Classroom>> GetClassroomsByInstructorAsync(int instructorId);
        Task<bool> CreateClassroomAsync(Classroom classroom);
        Task<bool> EnrollStudentsAsync(string classCrn, List<string> studentIds);
        Task<bool> UpdateClassroomAsync(Classroom classroom);
        Task<bool> DeleteClassroomAsync(string classCrn, int instructorId);
    }
}