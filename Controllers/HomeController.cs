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
            var todayStart = DateTime.Today;
            var todayEnd = DateTime.Today.AddDays(1);

            var meetingsToday = await _context.CalendarEvents
                .Include(e => e.Meeting)
                .Where(e => e.UserId == userId && 
                        e.StartTime >= todayStart && 
                        e.StartTime < todayEnd &&
                        e.Meeting != null)
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

              var user = await _userManager.Users
                .Include(u => u.doList)
                    .ThenInclude(l => l.Tasks)
                .FirstOrDefaultAsync(u => u.Id == userId);
            
            // Tasks Statistics
             var userTodoList = await _context.Set<toDoList>()
                .Include(l => l.Tasks)
                .FirstOrDefaultAsync(l => l.UserId == userId);
            
            if (userTodoList != null && userTodoList.Tasks != null)
            {
                // Tasks due TODAY that are NOT finished
                ViewBag.TasksDueToday = userTodoList.Tasks
                    .Where(t => t.Acheived != true && 
                            t.dateOfCompletion.HasValue && 
                            t.dateOfCompletion.Value.Date == DateTime.Today)
                    .Count();

                ViewBag.TotalTasks = userTodoList.Tasks
                    .Where(t => t.Acheived != true)
                    .Count();

                // Weekly graph: finished tasks whose due date falls in the last 7 days
                var weeklyTasksData = new int[7];
                for (int i = 0; i < 7; i++)
                {
                    var date = DateTime.Today.AddDays(-6 + i);
                    weeklyTasksData[i] = userTodoList.Tasks
                        .Where(t => t.Acheived == true && 
                                t.dateOfCompletion.HasValue &&
                                t.dateOfCompletion.Value.Date == date)
                        .Count();
                }
                ViewBag.WeeklyTasksData = weeklyTasksData;
            }
            else
            {
                ViewBag.TotalTasks = 0;
                ViewBag.TasksDueToday = 0;
                ViewBag.WeeklyTasksData = new int[7];
            }

            // Get all notes for the user
            var allNotes = await _context.Notes
                .Where(n => n.UserId == userId)
                .ToListAsync();
            
            // Total notes count
            var notesCount = allNotes.Count;
            
            // Notes created this week (last 7 days)
            var notesThisWeek = allNotes
                .Where(n => n.CreatedAt >= weekAgo && n.CreatedAt < today.AddDays(1))
                .Count();
            
            // Weekly notes data for chart (if you want to add a chart later)
            
            // Pass data to view
            ViewBag.TodayPomodoros = todayPomodoros;
            ViewBag.PomodoroIncrease = pomodoroIncrease;
           // ViewBag.TasksDueToday = tasksDueToday;
            ViewBag.MeetingsToday = meetingsToday;
            ViewBag.NotesCount = notesCount;
            ViewBag.NotesThisWeek = notesThisWeek;
            ViewBag.WeeklyPomodoroData = weeklyData.PomodoroSessions;
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
