using TeamProject.Models;

namespace TeamProject.Services.Interfaces
{
    public interface INoteService
    {
        Task<IEnumerable<Note>> GetAllNotesForUserAsync(string userId);
        Task<Note> CreateNoteAsync(Note note, string userId);
    }
}