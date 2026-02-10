using TeamProject.Models;

namespace TeamProject.Services.Interfaces
{
    public interface INoteService
    {
        Task<IEnumerable<Note>> GetAllNotesForUserAsync(string userId);
        Task<Note> CreateNoteAsync(Note note, string userId);
        Task DeleteNoteAsync(Note note);
        Task<Note?> GetNoteByIdAsync(int id);
        Task UpdateNoteAsync(Note note);
    }
}