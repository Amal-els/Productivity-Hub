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
    public class InboxModel : PageModel
    {
        private readonly UserEmailServiceFactory _emailServiceFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<InboxModel> _logger;

        public InboxModel(
            UserEmailServiceFactory emailServiceFactory,
            UserManager<ApplicationUser> userManager,
            ILogger<InboxModel> logger)
        {
            _emailServiceFactory = emailServiceFactory;
            _userManager = userManager;
            _logger = logger;
        }

        public List<MimeMessage> Messages { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found";
                return;
            }

            try
            {
                var emailService = _emailServiceFactory.CreateForUser(user);
                //Messages = await emailService.GetInboxAsync(50);
                Messages = await emailService.GetMessagesAsync(MyFolder.All, 50);
                _logger.LogInformation($"Loaded {Messages.Count} messages for {user.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load inbox");
                Messages = new List<MimeMessage>();
                TempData["Error"] = "Failed to load inbox: " + ex.Message;
            }
        }
    }
}