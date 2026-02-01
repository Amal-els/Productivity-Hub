using System.Net;
using System.Net.Mail;
using TeamProject.Data;
using TeamProject.Models;
using Microsoft.EntityFrameworkCore;

namespace TeamProject.Services
{
    public class EmailNotifService : IEmailNotifService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailNotifService> _logger;

        public EmailNotifService(
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<EmailNotifService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task SendTestEmailAsync(string to)
        {
            await SendEmailAsync(
                to,
                "Test Email",
                "<h2>✅ Email service works!</h2><p>This is a test email.</p>",
                "Test",
                null
            );
        }

        public async Task SendMeetingInvitationAsync(int eventId, List<string> recipientEmails)
        {
            var calendarEvent = await _context.CalendarEvents
                .Include(e => e.Meeting)
                .ThenInclude(m => m.Organizer)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (calendarEvent == null || calendarEvent.Meeting == null)
                return;

            var organizer = calendarEvent.Meeting.Organizer;
            var subject = $"Meeting Invitation: {calendarEvent.Title}";

            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>You're invited to a meeting</h2>
                <h3>{calendarEvent.Title}</h3>
                <p><strong>Organizer:</strong> {organizer.FullName} ({organizer.Email})</p>
                <p><strong>Date & Time:</strong> {calendarEvent.StartTime:dddd, MMMM dd, yyyy} at {calendarEvent.StartTime:hh:mm tt} - {calendarEvent.EndTime:hh:mm tt}</p>
                <p><strong>Location:</strong> {calendarEvent.Location ?? "Not specified"}</p>
                {(string.IsNullOrEmpty(calendarEvent.Meeting.MeetingLink) ? "" : $"<p><strong>Meeting Link:</strong> <a href='{calendarEvent.Meeting.MeetingLink}'>{calendarEvent.Meeting.MeetingLink}</a></p>")}
                <p><strong>Description:</strong></p>
                <p>{calendarEvent.Description}</p>
                <p style='margin-top: 20px;'>
                    <a href='{_configuration["AppUrl"]}/Calendar/RespondToInvitation/{eventId}?response=Accepted' style='background-color: #28a745; color: white; padding: 10px 20px; text-decoration: none; margin-right: 10px;'>Accept</a>
                    <a href='{_configuration["AppUrl"]}/Calendar/RespondToInvitation/{eventId}?response=Tentative' style='background-color: #ffc107; color: black; padding: 10px 20px; text-decoration: none; margin-right: 10px;'>Tentative</a>
                    <a href='{_configuration["AppUrl"]}/Calendar/RespondToInvitation/{eventId}?response=Declined' style='background-color: #dc3545; color: white; padding: 10px 20px; text-decoration: none;'>Decline</a>
                </p>
            </body>
            </html>";

            foreach (var email in recipientEmails)
            {
                // Personalize the body only for the links
                var personalizedBody = body
                    .Replace("response=Accepted", $"response=Accepted&email={System.Net.WebUtility.UrlEncode(email)}")
                    .Replace("response=Tentative", $"response=Tentative&email={System.Net.WebUtility.UrlEncode(email)}")
                    .Replace("response=Declined", $"response=Declined&email={System.Net.WebUtility.UrlEncode(email)}");

                await SendEmailAsync(email, subject, personalizedBody, "Invitation", eventId);
            }
        }

        public async Task SendMeetingUpdateAsync(int eventId)
        {
            var calendarEvent = await _context.CalendarEvents
                .Include(e => e.Meeting)
                .ThenInclude(m => m.Organizer)
                .Include(e => e.Participants)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (calendarEvent == null || calendarEvent.Meeting == null)
                return;

            var subject = $"Meeting Updated: {calendarEvent.Title}";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Meeting has been updated</h2>
                <h3>{calendarEvent.Title}</h3>
                <p><strong>Organizer:</strong> {calendarEvent.Meeting.Organizer.FullName}</p>
                <p><strong>New Date & Time:</strong> {calendarEvent.StartTime:dddd, MMMM dd, yyyy} at {calendarEvent.StartTime:hh:mm tt} - {calendarEvent.EndTime:hh:mm tt}</p>
                <p><strong>Location:</strong> {calendarEvent.Location ?? "Not specified"}</p>
                {(string.IsNullOrEmpty(calendarEvent.Meeting.MeetingLink) ? "" : $"<p><strong>Meeting Link:</strong> <a href='{calendarEvent.Meeting.MeetingLink}'>{calendarEvent.Meeting.MeetingLink}</a></p>")}
                <p><strong>Description:</strong></p>
                <p>{calendarEvent.Description}</p>
                <p style='margin-top: 20px;'>
                    <a href='{_configuration["AppUrl"]}/Calendar/ViewEvent/{eventId}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none;'>View Details</a>
                </p>
            </body>
            </html>";

            foreach (var participant in calendarEvent.Participants.Where(p => !p.IsOrganizer))
            {
                var email = participant.User?.Email ?? participant.Email;
                if (!string.IsNullOrEmpty(email))
                {
                    await SendEmailAsync(email, subject, body, "Update", eventId);
                }
            }
        }

        public async Task SendMeetingCancellationAsync(int eventId)
        {
            var calendarEvent = await _context.CalendarEvents
                .Include(e => e.Meeting)
                .ThenInclude(m => m.Organizer)
                .Include(e => e.Participants)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (calendarEvent == null || calendarEvent.Meeting == null)
                return;

            var subject = $"Meeting Cancelled: {calendarEvent.Title}";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #dc3545;'>Meeting has been cancelled</h2>
                <h3>{calendarEvent.Title}</h3>
                <p><strong>Organizer:</strong> {calendarEvent.Meeting.Organizer.FullName}</p>
                <p><strong>Original Date & Time:</strong> {calendarEvent.StartTime:dddd, MMMM dd, yyyy} at {calendarEvent.StartTime:hh:mm tt} - {calendarEvent.EndTime:hh:mm tt}</p>
                <p>This meeting has been cancelled by the organizer.</p>
            </body>
            </html>";

            foreach (var participant in calendarEvent.Participants.Where(p => !p.IsOrganizer))
            {
                var email = participant.User?.Email ?? participant.Email;
                if (!string.IsNullOrEmpty(email))
                {
                    await SendEmailAsync(email, subject, body, "Cancellation", eventId);
                }
            }
        }

        public async Task SendEventReminderAsync(int eventId, string recipientEmail)
        {
            var calendarEvent = await _context.CalendarEvents
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (calendarEvent == null)
                return;

            var subject = $"Reminder: {calendarEvent.Title}";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>⏰ Event Reminder</h2>
                <h3>{calendarEvent.Title}</h3>
                <p><strong>Starting in {calendarEvent.ReminderMinutesBefore} minutes</strong></p>
                <p><strong>Date & Time:</strong> {calendarEvent.StartTime:dddd, MMMM dd, yyyy} at {calendarEvent.StartTime:hh:mm tt}</p>
                <p><strong>Location:</strong> {calendarEvent.Location ?? "Not specified"}</p>
                <p>{calendarEvent.Description}</p>
            </body>
            </html>";

            await SendEmailAsync(recipientEmail, subject, body, "Reminder", eventId);
        }

        public async Task SendResponseNotificationAsync(int eventId, string participantName, string response)
        {
            var calendarEvent = await _context.CalendarEvents
                .Include(e => e.Meeting)
                .ThenInclude(m => m.Organizer)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (calendarEvent == null || calendarEvent.Meeting == null)
                return;

            var subject = $"Meeting Response: {calendarEvent.Title}";
            var responseColor = response == "Accepted" ? "#28a745" : response == "Declined" ? "#dc3545" : "#ffc107";

            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Meeting Response Received</h2>
                <p><strong>{participantName}</strong> has <span style='color: {responseColor}; font-weight: bold;'>{response}</span> your meeting invitation</p>
                <h3>{calendarEvent.Title}</h3>
                <p><strong>Date & Time:</strong> {calendarEvent.StartTime:dddd, MMMM dd, yyyy} at {calendarEvent.StartTime:hh:mm tt}</p>
                <p style='margin-top: 20px;'>
                    <a href='{_configuration["AppUrl"]}/Calendar/ViewEvent/{eventId}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none;'>View Details</a>
                </p>
            </body>
            </html>";

            await SendEmailAsync(calendarEvent.Meeting.Organizer.Email, subject, body, "Response", eventId);
        }

        private async Task SendEmailAsync(string to, string subject, string body, string emailType, int? eventId)
        {
            var emailLog = new EmailLog
            {
                RecipientEmail = to,
                Subject = subject,
                Body = body,
                EmailType = emailType,
                RelatedEventId = eventId
            };

            try
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var smtpUsername = _configuration["EmailSettings:Username"];
                var smtpPassword = _configuration["EmailSettings:Password"];
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromName = _configuration["EmailSettings:FromName"];

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);

                emailLog.IsSent = true;
                emailLog.SentAt = DateTime.Now;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {to}");
                emailLog.IsSent = false;
                emailLog.ErrorMessage = ex.Message;
            }

            _context.EmailLogs.Add(emailLog);
            await _context.SaveChangesAsync();
        }

    }
}
