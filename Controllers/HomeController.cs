using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamProject.Data;
using TeamProject.Models;


namespace TeamProject.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var userId = _userManager.GetUserId(User);
            var today = DateTime.Today;
            var weekAgo = today.AddDays(-6); // Last 7 days including today

            // Get Pomodoro sessions for the last 7 days
            var pomodoroSessions = await _context.PomodoroSessions
                .Where(s => s.UserId == userId && 
                           s.StartTime >= weekAgo && 
                           s.StartTime < today.AddDays(1))
                .ToListAsync();
            var meetingsToday = await _context.Meetings
                .Where(m => m.CalendarEvent.UserId == userId && 
                           m.CalendarEvent.StartTime == today)
                .CountAsync();

            

            // Calculate Pomodoro stats
            var todayPomodoros = pomodoroSessions
                .Where(s => s.StartTime.Date == today && s.Type == PomodoroType.Work)
                .Count();

            var yesterdayPomodoros = pomodoroSessions
                .Where(s => s.StartTime.Date == today.AddDays(-1) && s.Type == PomodoroType.Work)
                .Count();

            var pomodoroIncrease = todayPomodoros - yesterdayPomodoros;

            // Prepare weekly data for chart
            var weeklyData = new
            {
                PomodoroSessions = Enumerable.Range(0, 7)
                    .Select(i => {
                        var date = weekAgo.AddDays(i);
                        return pomodoroSessions
                            .Count(s => s.StartTime.Date == date && s.Type == PomodoroType.Work);
                    })
                    .ToList(),
            };

            // Pass data to view
            ViewBag.TodayPomodoros = todayPomodoros;
            ViewBag.PomodoroIncrease = pomodoroIncrease;
            //ViewBag.TotalTasks = totalTasks;
           // ViewBag.TasksDueToday = tasksDueToday;
            ViewBag.MeetingsToday = meetingsToday;
            //ViewBag.MinutesUntilNextMeeting = minutesUntilNextMeeting;
            //ViewBag.NotesCount = notesCount;
           // ViewBag.NotesThisWeek = notesThisWeek;
            ViewBag.WeeklyPomodoroData = weeklyData.PomodoroSessions;
            //ViewBag.WeeklyTasksData = weeklyData.TasksCompleted;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
