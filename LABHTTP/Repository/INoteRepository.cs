using LABHTTP.Data;

namespace LABHTTP.Repository
{
    public interface INoteRepository
    {
        Task<List<Note>> GetAllForUser(Guid id);
        Task<Note?> GetById(Guid id, Guid userId);
        Task Create(Note note);
        Task Update(Note note);
        Task Delete(Note note);
    }
}
