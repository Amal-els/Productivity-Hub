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

        // Get all notes for a specific user
        public async Task<IEnumerable<Note>> GetAllNotesForUserAsync(string userId)
        {
            return await _context.Notes
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        // Create a note for a specific user
        public async Task<Note> CreateNoteAsync(Note note, string userId)
        {
            note.UserId = userId;         // assign logged-in user
            note.CreatedAt = DateTime.Now;
            note.UpdatedAt = null;

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return note;
        }
        public async Task<Note?> GetNoteByIdAsync(int id)
        {
            return await _context.Notes.FindAsync(id);
        }

        public async Task DeleteNoteAsync(Note note)
        {
            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateNoteAsync(Note note)
        {
            note.UpdatedAt = DateTime.Now;

            _context.Notes.Update(note);
            await _context.SaveChangesAsync();
        }
    }
}