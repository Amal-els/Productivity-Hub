using Microsoft.EntityFrameworkCore;
using TeamProject.Data;
using TeamProject.Models;
using TeamProject.Services.Interfaces;

namespace TeamProject.Services.Implementations
{
    public class NoteService : INoteService
    {
        private readonly ApplicationDbContext _context;
        

        public NoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Note>> GetAllNotesForUserAsync(string userId)
        {
            return await _context.Notes
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Note> CreateNoteAsync(Note note, string userId)
        {

            note.CreatedAt = DateTime.Now;
            note.UpdatedAt = null;


            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return note;
        }
    }
}