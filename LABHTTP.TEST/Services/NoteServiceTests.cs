using LABHTTP.Data;
using LABHTTP.Model.DTO;
using LABHTTP.Repository;
using LABHTTP.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LABHTTP.TEST.Services
{
    public class NoteServiceTests
    {
        private readonly Mock<INoteRepository> _repo;
        private readonly NoteService _service;

        public NoteServiceTests()
        {
            _repo = new Mock<INoteRepository>();
            _service = new NoteService(_repo.Object);
        }
        [Fact]
        public void Note_WithEmptyTitle_IsInvalid()
        {
            var note = new Note { Title = "" };
            var ctx = new ValidationContext(note);
            var results = new List<ValidationResult>();

            var valid = Validator.TryValidateObject(note, ctx, results, true);

            Assert.False(valid);
        }
        [Fact]
        public async Task GetUserNotes_Returns_Only_User_Notes()
        {
            var userId = Guid.NewGuid();

            _repo.Setup(r => r.GetAllForUser(userId))
                 .ReturnsAsync(new List<Note>
                 {
                 new Note { UserId = userId, Title = "My note" }
                 });

            var result = await _service.GetUserNotes(userId);

            Assert.Single(result);
            Assert.Equal(userId, result[0].UserId);
        }

        [Fact]
        public async Task CreateNote_Assigns_UserId()
        {
            var userId = Guid.NewGuid();
            var dto = new NoteRequest
            {
                Title = "Test",
                Content = "Hello"
            };

            await _service.CreateNote(dto, userId);

            _repo.Verify(r => r.Create(It.Is<Note>(
                n => n.UserId == userId &&
                     n.Title == dto.Title &&
                     n.Content == dto.Content
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateNote_Throws_When_Note_Not_Found()
        {
            _repo.Setup(r => r.GetById(It.IsAny<Guid>(), It.IsAny<Guid>()))
                 .ReturnsAsync((Note?)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.UpdateNote(Guid.NewGuid(), new NoteRequest(), Guid.NewGuid())
            );
        }

        [Fact]
        public async Task UpdateNote_Updates_Only_User_Own_Note()
        {
            var userId = Guid.NewGuid();
            var noteId = Guid.NewGuid();

            var note = new Note
            {
                Id = noteId,
                UserId = userId,
                Title = "Old",
                Content = "Old"
            };

            _repo.Setup(r => r.GetById(noteId, userId))
                 .ReturnsAsync(note);

            var dto = new NoteRequest
            {
                Title = "New",
                Content = "Updated"
            };

            await _service.UpdateNote(noteId, dto, userId);

            _repo.Verify(r => r.Update(It.Is<Note>(
                n => n.Title == "New" &&
                     n.Content == "Updated"
            )), Times.Once);
        }

        [Fact]
        public async Task DeleteNote_Throws_When_User_Tries_To_Delete_Others_Note()
        {
            _repo.Setup(r => r.GetById(It.IsAny<Guid>(), It.IsAny<Guid>()))
                 .ReturnsAsync((Note?)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.DeleteNote(Guid.NewGuid(), Guid.NewGuid())
            );
        }

        [Fact]
        public async Task DeleteNote_Deletes_User_Own_Note()
        {
            var userId = Guid.NewGuid();
            var note = new Note
            {
                Id = Guid.NewGuid(),
                UserId = userId
            };

            _repo.Setup(r => r.GetById(note.Id, userId))
                 .ReturnsAsync(note);

            await _service.DeleteNote(note.Id, userId);

            _repo.Verify(r => r.Delete(note), Times.Once);
        }

    }
}
