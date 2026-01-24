using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using TeamProject.Services;

namespace TeamProject.Areas.Email.Pages
{
    public class IndexModel : PageModel
    {
        private readonly EmailService _emailService;

        public IndexModel(EmailService emailService)
        {
            _emailService = emailService;
        }

        public int UnreadCount { get; set; }
        public int SentTodayCount { get; set; }
        public List<MimeMessage> RecentMessages { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                // Fetch recent messages
                RecentMessages = await _emailService.GetInboxAsync(20);
                
                // Count unread (this is simplified - you'd need to check message flags)
                UnreadCount = RecentMessages.Count;
                
                // Count sent today (placeholder - you'd fetch from Sent folder)
                SentTodayCount = 0;
            }
            catch (Exception ex)
            {
                // Log error
                RecentMessages = new List<MimeMessage>();
                UnreadCount = 0;
                SentTodayCount = 0;
            }
        }
    }
}