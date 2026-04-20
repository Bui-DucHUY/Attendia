using Attendia.Models;

namespace Attendia.Repositories
{
    public interface ISessionRepository
    {
        Task<IEnumerable<Session>> GetSessionsByClassAsync(string classCrn);
        Task<Guid> CreateSessionAsync(Session session);
    }
}