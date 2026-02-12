using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamProject.Models;
using TeamProject.Models.Dtos;
using TeamProject.Data;

[Authorize]
public class PomodoroController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PomodoroController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var today = DateTime.Today;

        var todaySessions = await _context.PomodoroSessions
            .Where(s => s.UserId == userId &&
                        s.StartTime.Date == today)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync();

        return View(todaySessions);
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> SaveSession([FromBody] PomodoroSessionDto dto)
    {
        if (dto == null)
            return BadRequest("DTO is null");

        var userId = _userManager.GetUserId(User);

        // Specify that incoming times are UTC
        var startUtc = DateTime.SpecifyKind(dto.StartTime, DateTimeKind.Utc);
        var endUtc = DateTime.SpecifyKind(dto.EndTime, DateTimeKind.Utc);

        var session = new PomodoroSession
        {
            UserId = userId,
            StartTime = startUtc.ToLocalTime(), 
            EndTime = endUtc.ToLocalTime(),
            Type = Enum.Parse<PomodoroType>(dto.Type),
            IsCompleted = true
        };

        _context.PomodoroSessions.Add(session);
        await _context.SaveChangesAsync();

        return Json(new
        {
            type = session.Type,
            start = session.StartTime.ToString("HH:mm"),
            end = session.EndTime.ToString("HH:mm"),
            duration = (session.EndTime - session.StartTime).ToString(@"mm\:ss")
        });
    }



}
