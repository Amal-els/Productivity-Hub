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
    public class SentModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserEmailServiceFactory _emailServiceFactory;
        private readonly ILogger<SentModel> _logger;
        public SentModel(
            UserManager<ApplicationUser> _userManager,
            UserEmailServiceFactory _emailServiceFactory,
            ILogger<SentModel> _logger)
        {
            this._userManager = _userManager;
            this._emailServiceFactory = _emailServiceFactory;
            this._logger = _logger;
        }
        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.EmailPassword))
                return RedirectToPage("/Login");
            return Page();
        }
        
        public async Task<JsonResult> OnGetLoadSentEmailsAsync(int skip = 0, int take = 5)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.EmailPassword))
                return new JsonResult(new { success = false, message = "Unauthorized" });

            try
            {
                var emailService = _emailServiceFactory.CreateForUser(user);

                // Use the generic folder batch method, specifying Sent folder
                var batchWithUid = await emailService.GetFolderBatchAsync(MyFolder.Sent, skip, take);

                var batch = batchWithUid.Select(x => new
                {
                    uid = x.Uid.ToString(),
                    to = x.Message.To.Mailboxes.FirstOrDefault()?.Name 
                        ?? x.Message.To.Mailboxes.FirstOrDefault()?.Address 
                        ?? "Unknown",
                    subject = x.Message.Subject ?? "(No Subject)",
                    preview = x.Message.TextBody != null 
                            ? x.Message.TextBody.Substring(0, Math.Min(100, x.Message.TextBody.Length)) 
                            : "",
                    date = x.Message.Date.ToLocalTime().ToString("MMM dd, h:mm tt"),
                    hasAttachments = x.Message.Attachments.Any()
                }).ToList();

                return new JsonResult(new { success = true, messages = batch });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load sent emails via AJAX");
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }


        public async Task<JsonResult> OnGetEmailAsync(string uid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.EmailPassword))
                return new JsonResult(new { success = false, message = "Unauthorized" });

            try
            {
                var emailService = _emailServiceFactory.CreateForUser(user);
                var message = await emailService.GetEmailByUidAsync(uid, MyFolder.Sent);

                if (message == null)
                    return new JsonResult(new { success = false, message = "Email not found" });

                var attachments = message.Attachments
                    .OfType<MimePart>()
                    .Select(a => new
                    {
                        fileName = a.ContentDisposition?.FileName ?? "attachment",
                        size = a.Content?.Stream?.Length ?? 0
                    }).ToList();

                return new JsonResult(new
                {
                    success = true,
                    email = new
                    {
                        to = message.To?.ToString() ?? "Unknown",
                        subject = message.Subject ?? "(No Subject)",
                        date = message.Date.ToLocalTime().ToString("MMM dd, yyyy h:mm tt"),
                        body = message.TextBody ?? "",
                        htmlBody = message.HtmlBody ?? "",
                        hasAttachments = attachments.Any(),
                        attachments = attachments
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load sent email by UID");
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
