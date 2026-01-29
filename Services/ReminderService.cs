using Microsoft.EntityFrameworkCore;
using TeamProject.Data;

namespace TeamProject.Services
{
    public class ReminderService : IReminderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailNotifService _emailService;
        private readonly ILogger<ReminderService> _logger;

        public ReminderService(
            ApplicationDbContext context,
            IEmailNotifService emailService,
            ILogger<ReminderService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task ProcessPendingRemindersAsync()
        {
            var now = DateTime.Now;

            var eventsToRemind = await _context.CalendarEvents
                .Include(e => e.User)
                .Include(e => e.Participants)
                .ThenInclude(p => p.User)
                .Where(e => e.HasReminder
                    && !e.ReminderSent
                    && e.StartTime > now)
                .ToListAsync();

            foreach (var evt in eventsToRemind)
            {
                var reminderTime = evt.StartTime.AddMinutes(-evt.ReminderMinutesBefore);

                if (now >= reminderTime)
                {
                    // Send to owner
                    await _emailService.SendEventReminderAsync(evt.Id, evt.User.Email);

                    // Send to all participants if it's a meeting
                    if (evt.EventType == "Meeting")
                    {
                        foreach (var participant in evt.Participants.Where(p => p.ResponseStatus == "Accepted"))
                        {
                            await _emailService.SendEventReminderAsync(evt.Id, participant.User?.Email ?? participant.Email);
                        }
                    }

                    evt.ReminderSent = true;
                    _logger.LogInformation($"Reminder sent for event {evt.Id}: {evt.Title}");
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}

