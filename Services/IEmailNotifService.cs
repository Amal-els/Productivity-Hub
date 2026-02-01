namespace TeamProject.Services
{
    public interface IEmailNotifService
    {
        Task SendTestEmailAsync(string recipientEmail);
        Task SendMeetingInvitationAsync(int eventId, List<string> recipientEmails);
        Task SendMeetingUpdateAsync(int eventId);
        Task SendMeetingCancellationAsync(int eventId);
        Task SendEventReminderAsync(int eventId, string recipientEmail);
        Task SendResponseNotificationAsync(int eventId, string participantName, string response);
    }
}
