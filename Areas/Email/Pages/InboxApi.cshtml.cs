using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using TeamProject.Models;
using TeamProject.Services;

namespace TeamProject.Areas.Email.Pages
{
    [Authorize]
    public class InboxApiModel : PageModel
    {
        private readonly UserEmailServiceFactory _emailServiceFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<InboxApiModel> _logger;

        public InboxApiModel(
            UserEmailServiceFactory emailServiceFactory,
            UserManager<ApplicationUser> userManager,
            ILogger<InboxApiModel> logger)
        {
            _emailServiceFactory = emailServiceFactory;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public int Skip { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public int Take { get; set; } = 10;

        public async Task<JsonResult> OnGetLoadEmailsAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.EmailPassword))
            {
                return new JsonResult(new { success = false, message = "User not authenticated or email password missing" });
            }

            try
            {
                var emailService = _emailServiceFactory.CreateForUser(user);
                var allMessages = await emailService.GetInboxAsync(); // get all messages

                // Apply paging
                var messages = allMessages
                    .OrderByDescending(m => m.Date)
                    .Skip(Skip)
                    .Take(Take)
                    .Select(m => new
                    {
                        From = m.From.Mailboxes.FirstOrDefault()?.ToString() ?? "Unknown",
                        Subject = m.Subject,
                        Date = m.Date.ToLocalTime().ToString("MMM dd, h:mm tt"),
                        Preview = m.TextBody != null ? m.TextBody.Substring(0, Math.Min(50, m.TextBody.Length)) : ""
                    }).ToList();

                return new JsonResult(new { success = true, messages });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch emails");
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
