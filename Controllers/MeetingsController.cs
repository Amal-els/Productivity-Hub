using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamProject.Data;
using TeamProject.Models;
using TeamProject.ViewModels;

namespace TeamProject.Controllers
{
    [Authorize]
    public class MeetingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<MeetingsController> _logger;

        public MeetingsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<MeetingsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Meetings
        public async Task<IActionResult> Index(string filter = "all")
        {
            ViewData["CurrentTab"] = "Meetings";
            var userId = _userManager.GetUserId(User);
            var now = DateTime.Now;

            // Get all meetings where user is organizer or participant
            var meetingsQuery = _context.Meetings
                .Include(m => m.CalendarEvent)
                    .ThenInclude(e => e.Participants)
                        .ThenInclude(p => p.User)
                .Include(m => m.Organizer)
                .Where(m => m.OrganizerId == userId ||
                           m.CalendarEvent.Participants.Any(p => p.UserId == userId));

            // Apply filters
            switch (filter.ToLower())
            {
                case "upcoming":
                    meetingsQuery = meetingsQuery.Where(m =>
                        m.CalendarEvent.StartTime >= now &&
                        (
                            m.OrganizerId == userId ||
                            m.CalendarEvent.Participants.Any(p =>
                                p.UserId == userId && p.ResponseStatus != "Declined")
                        ));
                    break;

                case "pending":
                    meetingsQuery = meetingsQuery.Where(m =>
                        m.CalendarEvent.Participants.Any(p =>
                            p.UserId == userId && p.ResponseStatus == "Pending"));
                    break;

                case "organized":
                    meetingsQuery = meetingsQuery.Where(m =>
                        m.OrganizerId == userId);
                    break;

                case "invitations":
                    meetingsQuery = meetingsQuery.Where(m =>
                        m.OrganizerId != userId &&
                        m.CalendarEvent.Participants.Any(p => p.UserId == userId) &&
                        m.CalendarEvent.StartTime >= now);
                    break;
            }


            var meetings = await meetingsQuery
                .OrderBy(m => m.CalendarEvent.StartTime)
                .Select(m => new MeetingDetailsViewModel
                {
                    MeetingId = m.Id,
                    EventId = m.CalendarEventId,
                    Title = m.CalendarEvent.Title,
                    Description = m.CalendarEvent.Description,
                    StartTime = m.CalendarEvent.StartTime,
                    EndTime = m.CalendarEvent.EndTime,
                    Location = m.CalendarEvent.Location,
                    OrganizerName = m.Organizer.FullName,
                    OrganizerEmail = m.Organizer.Email,
                    IsOrganizer = m.OrganizerId == userId,
                    ParticipantCount = m.CalendarEvent.Participants.Count,
                    ResponseStatus = m.CalendarEvent.Participants
                        .FirstOrDefault(p => p.UserId == userId).ResponseStatus,
                    AcceptedCount = m.CalendarEvent.Participants
                        .Count(p => p.ResponseStatus == "Accepted"),
                    MeetingLink = m.MeetingLink,
                })
                .ToListAsync();

            var viewModel = new MeetingsIndexViewModel
            {
                Meetings = meetings,
                CurrentFilter = filter,
                UpcomingCount = await _context.Meetings
                    .Where(m => m.OrganizerId == userId ||
                               m.CalendarEvent.Participants.Any(p => p.UserId == userId && p.ResponseStatus != "Declined"))
                    .Where(m => m.CalendarEvent.StartTime >= now)
                    .CountAsync(),

                PendingCount = await _context.Meetings
                    .Where(m => m.CalendarEvent.Participants
                        .Any(p => p.UserId == userId && p.ResponseStatus == "Pending"))
                    .CountAsync(),
                OrganizedCount = await _context.Meetings
                    .Where(m => m.OrganizerId == userId)
                    .CountAsync(),
                InvitationsCount = await _context.Meetings
                    .Where(m => m.OrganizerId != userId &&
                               m.CalendarEvent.Participants.Any(p => p.UserId == userId) &&
                               m.CalendarEvent.StartTime >= now)
                    .CountAsync()
            };

            return View(viewModel);
        }

        // GET: Meetings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            ViewData["CurrentTab"] = "Meetings";
            var userId = _userManager.GetUserId(User);

            var meeting = await _context.Meetings
                .Include(m => m.CalendarEvent)
                    .ThenInclude(e => e.Participants)
                        .ThenInclude(p => p.User)
                .Include(m => m.Organizer)
                .Where(m => (m.Id == id || m.CalendarEventId == id) &&
                    (m.OrganizerId == userId ||
                     m.CalendarEvent.Participants.Any(p => p.UserId == userId)))
                .OrderBy(m => m.Id == id ? 0 : 1) // Prioritize m.Id matches
                .FirstOrDefaultAsync();

            if (meeting == null)
            {
                return NotFound();
            }

            var viewModel = new MeetingInvitationViewModel
            {
                EventId = meeting.CalendarEventId,
                Title = meeting.CalendarEvent.Title,
                Description = meeting.CalendarEvent.Description,
                StartTime = meeting.CalendarEvent.StartTime.ToLocalTime(),
                EndTime = meeting.CalendarEvent.EndTime.ToLocalTime(),
                Location = meeting.CalendarEvent.Location,
                MeetingLink = meeting.MeetingLink,
                OrganizerName = meeting.Organizer.FullName,
                OrganizerEmail = meeting.Organizer.Email,
                CurrentResponseStatus = meeting.CalendarEvent.Participants
                    .FirstOrDefault(p => p.UserId == userId)?.ResponseStatus,
                Participants = meeting.CalendarEvent.Participants
                    .Select(p => new ParticipantInfo
                    {
                        Name = p.User != null ? p.User.FullName : (p.Email ?? "Guest"),
                        Email = p.User != null ? p.User.Email : p.Email,
                        ResponseStatus = p.ResponseStatus
                    })
                    .ToList()
            };

            return View(viewModel);
        }
    }

}
