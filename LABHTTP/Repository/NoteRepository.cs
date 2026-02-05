using LABHTTP.Data;
using Microsoft.EntityFrameworkCore;

namespace LABHTTP.Repository
{
    public class NoteRepository : INoteRepository
    {
        private readonly AppDbContext _context;

        public NoteRepository(AppDbContext context)
        {
            _context = context;
        }
        public Task<List<Note>> GetAllForUser(Guid userId)
        {
            return _context.Notes
                .Where(n => n.UserId == userId)
                .ToListAsync();
        }

        public Task<Note?> GetById(Guid id, Guid userId)
        {
            return _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        }

        public async Task Create(Note note)
        {
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Note note)
        {
            _context.Notes.Update(note);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Note note)
        {
            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
        }
    }
}
