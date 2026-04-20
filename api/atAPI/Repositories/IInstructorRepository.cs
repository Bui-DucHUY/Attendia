using Attendia.Models;

namespace Attendia.Repositories
{
    public interface IInstructorRepository
    {
        Task<int> RegisterInstructorAsync(Instructor instructor);
        Task<Instructor?> GetInstructorByEmailAsync(string email);
    }
}