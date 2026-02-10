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

        public async Task<IActionResult> Index()
        {
            var devEmail = "dev@local.com";
            var user = await _userManager.FindByEmailAsync(devEmail);

            if (user == null)
                return Problem("Dev user not found");

            var notes = await _noteService.GetAllNotesForUserAsync(user.Id);

            ViewData["CurrentTab"] = "Notes";
            return View(notes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["CurrentTab"] = "Notes";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Note note)
        {
            if (!ModelState.IsValid)
                return View(note);

            var devEmail = "dev@local.com";
            var user = await _userManager.FindByEmailAsync(devEmail);

            if (user == null)
                return Problem("Dev user not found");

            await _noteService.CreateNoteAsync(note, user.Id);

            return RedirectToAction(nameof(Index));
        }
    }
}