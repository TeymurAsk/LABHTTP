using LABHTTP.Data;
using LABHTTP.Model.DTO;
using LABHTTP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LABHTTP.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly NoteService _service;

        public NoteController(NoteService service)
        {
            _service = service;
        }

        private Guid UserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException());

        [HttpGet]
        public Task<List<Note>> GetAll()
            => _service.GetUserNotes(UserId);

        [HttpPost]
        public async Task<IActionResult> Create(NoteRequest dto)
        {
            await _service.CreateNote(dto, UserId);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, NoteRequest dto)
        {
            await _service.UpdateNote(id, dto, UserId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteNote(id, UserId);
            return NoContent();
        }
    }
}
