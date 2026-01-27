using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamProject.Data;
using TeamProject.Models;
using TeamProject.Services;
using TeamProject.ViewModels;


namespace TeamProject.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<CalendarController> _logger;

        public CalendarController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            ILogger<CalendarController> logger)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        // GET: Calendar
        public async Task<IActionResult> Index(string view = "month", DateTime? date = null)
        {
            ViewData["CurrentTab"] = "Calendar";

            var userId = _userManager.GetUserId(User);
            var currentDate = date ?? DateTime.Today;

            var viewModel = new CalendarViewModel
            {
                View = view,
                CurrentDate = currentDate,
                Events = await GetUserEventsAsync(userId, view, currentDate)
            };

            return View(viewModel);
        }

        // GET: Calendar/Create
        public IActionResult Create(string date)
        {
            ViewData["CurrentTab"] = "Calendar";

            var model = new CalendarEventViewModel();

            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime selectedDate))
            {
                model.StartDate = selectedDate.Date;
                model.StartClock = new TimeSpan(9, 0, 0);
                model.EndDate = selectedDate.Date;
                model.EndClock = new TimeSpan(10, 0, 0);
            }
            else
            {
                model.StartDate = DateTime.Now.Date;
                model.StartClock = new TimeSpan(9, 0, 0);
                model.EndDate = DateTime.Now.Date;
                model.EndClock = new TimeSpan(10, 0, 0);
            }

            return View(model);
        }


        // POST: Calendar/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CalendarEventViewModel model)
        {
            ViewData["CurrentTab"] = "Calendar";

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new
                {
                    Field = x.Key,
                    Errors = x.Value.Errors.Select(e => e.ErrorMessage)
                });

                _logger.LogError("ModelState errors: {@Errors}", errors);
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.GetUserAsync(User);

            var calendarEvent = new CalendarEvent
            {
                Title = model.Title,
                Description = model.Description,
                StartTime = model.StartDate.Date + model.StartClock,
                EndTime = model.EndDate.Date + model.EndClock,
                Location = model.Location,
                IsAllDay = model.EndDate > model.StartDate ? true : model.IsAllDay,
                Color = model.Color,
                EventType = model.EventType,
                HasReminder = model.HasReminder,
                ReminderMinutesBefore = model.ReminderMinutesBefore,
                UserId = userId
            };

            _context.CalendarEvents.Add(calendarEvent);
            await _context.SaveChangesAsync();

            // If it's a meeting, create meeting record and participants
            if (model.EventType == "Meeting" && model.ParticipantEmails?.Any() == true)
            {
                var meeting = new Meeting
                {
                    CalendarEventId = calendarEvent.Id,
                    MeetingLink = model.MeetingLink,
                    OrganizerId = userId
                };

                _context.Meetings.Add(meeting);

                // Add organizer as participant
                var organizerParticipant = new EventParticipant
                {
                    CalendarEventId = calendarEvent.Id,
                    UserId = userId,
                    ResponseStatus = "Accepted",
                    IsOrganizer = true,
                    ResponseDate = DateTime.Now
                };
                _context.EventParticipants.Add(organizerParticipant);

                // Add other participants
                var validEmails = new List<string>();
                foreach (var email in model.ParticipantEmails.Where(e => !string.IsNullOrWhiteSpace(e)))
                {
                    var participant = await _userManager.FindByEmailAsync(email.Trim());

                    var eventParticipant = new EventParticipant
                    {
                        CalendarEventId = calendarEvent.Id,
                        UserId = participant?.Id, // Can be null
                        Email = email.Trim(),
                        ResponseStatus = "Pending",
                        IsOrganizer = false
                    };
                    _context.EventParticipants.Add(eventParticipant);
                    validEmails.Add(email.Trim());
                }

                await _context.SaveChangesAsync();

                // Send invitations
                if (validEmails.Any())
                {
                    await _emailService.SendMeetingInvitationAsync(calendarEvent.Id, validEmails);
                }
            }

            TempData["SuccessMessage"] = "Event created successfully!";

            return RedirectToAction(nameof(Index));
        }

        // GET: Calendar/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["CurrentTab"] = "Calendar";

            var userId = _userManager.GetUserId(User);
            var calendarEvent = await _context.CalendarEvents
                .Include(e => e.Meeting)
                .Include(e => e.Participants)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (calendarEvent == null)
            {
                return NotFound();
            }

            var model = new CalendarEventViewModel
            {
                Id = calendarEvent.Id,
                Title = calendarEvent.Title,
                Description = calendarEvent.Description,
                StartDate = calendarEvent.StartTime.Date,
                StartClock = calendarEvent.StartTime.TimeOfDay,
                EndDate = calendarEvent.EndTime.Date,
                EndClock = calendarEvent.EndTime.TimeOfDay,
                Location = calendarEvent.Location,
                IsAllDay = calendarEvent.IsAllDay,
                Color = calendarEvent.Color,
                EventType = calendarEvent.EventType,
                HasReminder = calendarEvent.HasReminder,
                ReminderMinutesBefore = calendarEvent.ReminderMinutesBefore,
                MeetingLink = calendarEvent.Meeting?.MeetingLink,
                ParticipantEmails = calendarEvent.Participants?
                .Where(p => !p.IsOrganizer)
                .Select(p => p.User != null ? p.User.Email : p.Email)
                .ToList() ?? new List<string>()
            };

            return View(model);
        }

        // POST: Calendar/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CalendarEventViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var userId = _userManager.GetUserId(User);

            var calendarEvent = await _context.CalendarEvents
                .Include(e => e.Meeting)
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (calendarEvent == null)
                return NotFound();

            calendarEvent.StartTime = model.StartDate.Date + model.StartClock;
            calendarEvent.EndTime = model.EndDate.Date + model.EndClock;

            calendarEvent.Title = model.Title;
            calendarEvent.Description = model.Description;
            calendarEvent.Location = model.Location;
            calendarEvent.Color = model.Color;
            calendarEvent.HasReminder = model.HasReminder;
            calendarEvent.ReminderMinutesBefore = model.ReminderMinutesBefore;
            calendarEvent.ReminderSent = false;
            calendarEvent.UpdatedAt = DateTime.Now;

            calendarEvent.IsAllDay =
                model.IsAllDay ||
                calendarEvent.EndTime.Date > calendarEvent.StartTime.Date;

            
            if (calendarEvent.Meeting != null)
            {
                calendarEvent.Meeting.MeetingLink = model.MeetingLink;
                calendarEvent.Meeting.UpdatedAt = DateTime.Now;

                var incomingEmails = model.ParticipantEmails
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .Select(e => e.Trim().ToLower())
                    .Distinct()
                    .ToList();

                var existingParticipants = calendarEvent.Participants
                    .Where(p => !p.IsOrganizer)
                    .ToList();

                // Remove deleted participants
                var toRemove = existingParticipants
                    .Where(p => !incomingEmails.Contains(p.Email.ToLower()))
                    .ToList();

                _context.EventParticipants.RemoveRange(toRemove);

                // Add new participants
                foreach (var email in incomingEmails)
                {
                    if (existingParticipants.Any(p => p.Email.ToLower() == email))
                        continue;

                    var user = await _userManager.FindByEmailAsync(email);

                    _context.EventParticipants.Add(new EventParticipant
                    {
                        CalendarEventId = calendarEvent.Id,
                        UserId = user?.Id,
                        Email = email,
                        ResponseStatus = "Pending",
                        IsOrganizer = false
                    });
                }
            }

            await _context.SaveChangesAsync();

            if (calendarEvent.Meeting != null)
                await _emailService.SendMeetingUpdateAsync(calendarEvent.Id);

            TempData["SuccessMessage"] = "Event updated successfully!";
            return RedirectToAction(nameof(Index));
        }


        // POST: Calendar/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var calendarEvent = await _context.CalendarEvents
                .Include(e => e.Meeting)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (calendarEvent == null)
            {
                return NotFound();
            }

            // Send cancellation emails if it's a meeting
            if (calendarEvent.Meeting != null)
            {
                await _emailService.SendMeetingCancellationAsync(calendarEvent.Id);
            }

            _context.CalendarEvents.Remove(calendarEvent);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Event deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Calendar/ViewEvent/5
        public async Task<IActionResult> ViewEvent(int id)
        {
            ViewData["CurrentTab"] = "Calendar";

            var userId = _userManager.GetUserId(User);
            var calendarEvent = await _context.CalendarEvents
                .Include(e => e.User)
                .Include(e => e.Meeting)
                .ThenInclude(m => m.Organizer)
                .Include(e => e.Participants)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(e => e.Id == id &&
                    (e.UserId == userId || e.Participants.Any(p => p.UserId == userId)));

            if (calendarEvent == null)
            {
                return NotFound();
            }

            if (calendarEvent.Meeting != null)
            {
                var viewModel = new MeetingInvitationViewModel
                {
                    EventId = calendarEvent.Id,
                    Title = calendarEvent.Title,
                    Description = calendarEvent.Description,
                    StartTime = calendarEvent.StartTime,
                    EndTime = calendarEvent.EndTime,
                    Location = calendarEvent.Location,
                    MeetingLink = calendarEvent.Meeting.MeetingLink,
                    OrganizerName = calendarEvent.Meeting.Organizer.FullName,
                    OrganizerEmail = calendarEvent.Meeting.Organizer.Email,
                    CurrentResponseStatus = calendarEvent.Participants
                        .FirstOrDefault(p => p.UserId == userId)?.ResponseStatus,
                    Participants = calendarEvent.Participants.Select(p => new ParticipantInfo
                    {
                        Name = p.User != null ? p.User.FullName : (p.Email ?? "Guest"),
                        Email = p.User != null ? p.User.Email : p.Email,
                        ResponseStatus = p.ResponseStatus
                    }).ToList()
                };

                return View("~/Views/Meetings/Details.cshtml", viewModel);
            }

            return View(calendarEvent);
        }

        // GET: Calendar/RespondToInvitation/5
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> RespondToInvitation(int id, string response, string? email)
        {
            var userId = _userManager.GetUserId(User);
            EventParticipant? participant = null;

            // Try to find by UserId if logged in
            if (userId != null)
            {
                participant = await _context.EventParticipants
                    .Include(p => p.CalendarEvent)
                    .ThenInclude(e => e.Meeting)
                    .FirstOrDefaultAsync(p => p.CalendarEventId == id && p.UserId == userId);
            }

            // If not found (or not logged in) and email is provided, try to find by email
            if (participant == null && !string.IsNullOrEmpty(email))
            {
                participant = await _context.EventParticipants
                    .Include(p => p.CalendarEvent)
                    .ThenInclude(e => e.Meeting)
                    .FirstOrDefaultAsync(p => p.CalendarEventId == id && p.Email == email);
            }

            if (participant == null)
            {
                // If user is not logged in and no email provided, ask them to login
                if (userId == null && string.IsNullOrEmpty(email))
                {
                    return Challenge();
                }
                return NotFound();
            }

            if (response != "Accepted" && response != "Declined" && response != "Tentative")
            {
                return BadRequest();
            }

            participant.ResponseStatus = response;
            participant.ResponseDate = DateTime.Now;
            await _context.SaveChangesAsync();

            // Send notification to organizer
            // Use Participant Email or User Full Name if available, otherwise "Guest"
            var participantName = participant.User?.FullName ?? participant.Email ?? "Guest";
            await _emailService.SendResponseNotificationAsync(id, participantName, response);

            TempData["SuccessMessage"] = $"You have {response.ToLower()} the meeting invitation.";

            // If user is logged in, go to Index, otherwise show a simple confirmation view or redirect to Home
            if (userId != null)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // For anonymous users, redirect to Home or a specific confirmation page
                return RedirectToAction("Index", "Home");
            }
        }

        // API endpoint for getting events (for calendar views)
        [HttpGet]
        public async Task<IActionResult> GetEvents(DateTime start, DateTime end)
        {
            var userId = _userManager.GetUserId(User);

            var events = await _context.CalendarEvents
                .Include(e => e.Participants)
                .Include(e => e.Meeting)
                .Where(e => (e.UserId == userId || e.Participants.Any(p => p.UserId == userId && p.ResponseStatus != "Declined"))
                    && e.StartTime < end
                    && e.EndTime > start)
                .Select(e => new
                {
                    id = e.Id,
                    title = e.Title,
                    start = e.StartTime,
                    end = e.IsAllDay ? e.EndTime.AddDays(1) : e.EndTime,
                    allDay = e.IsAllDay,
                    color = e.Color,
                    extendedProps = new
                    {
                        description = e.Description,
                        location = e.Location,
                        eventType = e.EventType,
                        isMeeting = e.Meeting != null,
                        responseStatus = e.Participants.FirstOrDefault(p => p.UserId == userId).ResponseStatus,
                        isOrganizer = e.UserId == userId
                    }
                })

                .ToListAsync();

            return Json(events);
        }

        private async Task<List<CalendarEventDisplayModel>> GetUserEventsAsync(string userId, string view, DateTime currentDate)
        {
            DateTime startDate, endDate;

            switch (view)
            {
                case "day":
                    startDate = currentDate.Date;
                    endDate = currentDate.Date.AddDays(1);
                    break;
                case "week":
                    startDate = currentDate.AddDays(-(int)currentDate.DayOfWeek);
                    endDate = startDate.AddDays(7);
                    break;
                case "month":
                default:
                    startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                    endDate = startDate.AddMonths(1);
                    break;
            }

            return await _context.CalendarEvents
                .Include(e => e.Participants)
                .Include(e => e.Meeting)
                .Where(e => (e.UserId == userId || e.Participants.Any(p => p.UserId == userId && p.ResponseStatus != "Declined"))
                    && e.StartTime < endDate
                    && e.EndTime > startDate)
                .Select(e => new CalendarEventDisplayModel
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    Start = e.StartTime,
                    End = e.EndTime,
                    Color = e.Color,
                    AllDay = e.IsAllDay,
                    EventType = e.EventType,
                    Location = e.Location,
                    IsMeeting = e.Meeting != null,
                    ResponseStatus = e.Participants.FirstOrDefault(p => p.UserId == userId).ResponseStatus,
                    IsOrganizer = e.UserId == userId
                })
                .OrderBy(e => e.Start)
                .ToListAsync();
        }
    }
}