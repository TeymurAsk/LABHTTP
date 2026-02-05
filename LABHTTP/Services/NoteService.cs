using LABHTTP.Data;
using LABHTTP.Model.DTO;
using LABHTTP.Repository;

namespace LABHTTP.Services
{
    public class NoteService
    {
        private readonly INoteRepository _repository;
        public NoteService(INoteRepository repo)
        {
            _repository = repo;
        }
        public Task<List<Note>> GetUserNotes(Guid userId)
        => _repository.GetAllForUser(userId);


        public async Task<Note> GetNote(Guid id, Guid userId)
        {
            var note = await _repository.GetById(id, userId);
            if (note == null)
                throw new UnauthorizedAccessException("Access denied");

            return note;
        }
        public async Task CreateNote(NoteRequest dto, Guid userId)
        {
            var note = new Note
            {
                Title = dto.Title,
                Content = dto.Content,
                UserId = userId
            };

            await _repository.Create(note);
        }

        public async Task UpdateNote(Guid id, NoteRequest dto, Guid userId)
        {
            var note = await GetNote(id, userId);

            note.Title = dto.Title;
            note.Content = dto.Content;

            await _repository.Update(note);
        }

        public async Task DeleteNote(Guid id, Guid userId)
        {
            var note = await GetNote(id, userId);
            await _repository.Delete(note);
        }
    }
}
