using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TeamProject.Models;
using TeamProject.Services.Interfaces;

namespace TeamProject.Controllers
{
    public class NoteController : Controller
    {
        private readonly INoteService _noteService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NoteController(
            INoteService noteService,
            UserManager<ApplicationUser> userManager)
        {
            _noteService = noteService;
            _userManager = userManager;
        }

        // GET: Notes/Index
        public async Task<IActionResult> Index()
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge(); // redirect to login if not authenticated

            var notes = await _noteService.GetAllNotesForUserAsync(user.Id);

            ViewData["CurrentTab"] = "Notes";
            return View(notes);
        }

        // GET: Notes/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["CurrentTab"] = "Notes";
            return View();
        }

        // POST: Notes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Note note)
        {
            if (!ModelState.IsValid)
                return View(note);

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge(); // redirect to login if not authenticated

            await _noteService.CreateNoteAsync(note, user.Id);

            return RedirectToAction(nameof(Index));
        }
   

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var note = await _noteService.GetNoteByIdAsync(id);

            if (note == null || note.UserId != user.Id)
                return NotFound();

            await _noteService.DeleteNoteAsync(note);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null)
                return NotFound();

            return View(note);
        }

        // --- EDIT POST ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Note note)
        {
            if (id != note.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(note);

            var existingNote = await _noteService.GetNoteByIdAsync(id);
            if (existingNote == null)
                return NotFound();

            existingNote.Title = note.Title;
            existingNote.Content = note.Content;

            await _noteService.UpdateNoteAsync(existingNote);

            return RedirectToAction(nameof(Index));
        }
    }
}
    