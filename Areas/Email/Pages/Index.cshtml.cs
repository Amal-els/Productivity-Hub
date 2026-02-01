using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using TeamProject.Models;
using TeamProject.Services;
namespace TeamProject.Areas.Email.Pages;
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
    public async Task<IActionResult> OnGetAsync() 
    { 
        var user = await _userManager.GetUserAsync(User); 
        if (user == null) 
        {
            _logger.LogWarning("User not found"); 
            return RedirectToPage("/Account/Login", new { area = "Identity" }); 
        } 
            // Check if EmailPassword is null or empty 
        if (string.IsNullOrEmpty(user.EmailPassword)) 
        { 
            _logger.LogInformation($"User {user.Email} has no email password set"); 
            TempData["Info"] = "Please enter your email app password to access your inbox."; 
            return Redirect("~/Email/Settings"); 
        } 
        try 
        { 
            _logger.LogInformation($"Attempting to fetch emails for {user.Email}");
            var emailService = _emailServiceFactory.CreateForUser(user); 
            var Messages = await emailService.GetInboxAsync(20); 
            _logger.LogInformation($"Successfully loaded {Messages.Count} messages for {user.Email}"); 
        } 
        catch (MailKit.Security.AuthenticationException authEx) 
        {
            _logger.LogError(authEx, "Authentication failed - Invalid email password"); 
            var Messages = new List<MimeMessage>(); 
            TempData["Error"] = "Authentication failed. Your email app password is incorrect. Please update it in settings."; 
            TempData["ShowSettingsLink"] = true; 
        } 
        catch (Exception ex) 
        { 
            _logger.LogError(ex, "Failed to load inbox"); 
            var Messages = new List<MimeMessage>(); 
            TempData["Error"] = $"Failed to load inbox: {ex.Message}"; 
        } 
        return Page();

    }



    public async Task<IActionResult> OnPostMarkAsReadAsync([FromBody] string uid)
        {
            if (string.IsNullOrEmpty(uid))
                return new JsonResult(new { success = false, message = "UID is required" });

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var emailService = _emailServiceFactory.CreateForUser(user); 
                await emailService.MarkEmailAsReadAsync(uid, MyFolder.Inbox);
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    // AJAX HANDLER — LOAD EMAILS BATCH
    public async Task<JsonResult> OnGetLoadEmailsAsync(string folder = "Inbox", int skip = 0, int take = 5)
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null || string.IsNullOrEmpty(user.EmailPassword))
        return new JsonResult(new { success = false, message = "Unauthorized" });

    try
    {
        var emailService = _emailServiceFactory.CreateForUser(user);

        if (!Enum.TryParse<MyFolder>(folder, true, out var folderType))
            folderType = MyFolder.Inbox;

        var batchWithUid = await emailService.GetFolderBatchAsync(folderType, skip, take);

        var batch = batchWithUid.Select(x => new
        {
            uid = x.Uid.ToString(),
            from = x.Message.From.Mailboxes.FirstOrDefault()?.Name ?? x.Message.From.Mailboxes.FirstOrDefault()?.Address ?? "Unknown",
            subject = x.Message.Subject ?? "(No Subject)",
            preview = x.Message.TextBody != null ? x.Message.TextBody.Substring(0, Math.Min(100, x.Message.TextBody.Length)) : "",
            date = x.Message.Date.ToLocalTime().ToString("MMM dd, h:mm tt"),
            hasAttachments = x.Message.Attachments.Any()
        }).ToList();

        return new JsonResult(new { success = true, messages = batch });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to load emails via AJAX");
        return new JsonResult(new { success = false, message = ex.Message });
    }
}


    // AJAX HANDLER — GET SINGLE EMAIL BY UID
    public async Task<JsonResult> OnGetEmailAsync(string uid)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || string.IsNullOrEmpty(user.EmailPassword))
            return new JsonResult(new { success = false, message = "Unauthorized" });

        try
        {
            var emailService = _emailServiceFactory.CreateForUser(user);
            var message = await emailService.GetEmailByUidAsync(uid);

            if (message == null)
                return new JsonResult(new { success = false, message = "Email not found" });

            var attachments = message.Attachments.Select(a =>
            {
                var part = a as MimePart;
                return new
                {
                    fileName = part?.FileName ?? "Unknown",
                    size = part?.Content?.Stream?.Length ?? 0
                };
            }).ToList();

            return new JsonResult(new
            {
                success = true,
                email = new
                {
                    from = message.From.Mailboxes.FirstOrDefault()?.Name ?? message.From.Mailboxes.FirstOrDefault()?.Address ?? "Unknown",
                    to = string.Join(", ", message.To.Mailboxes.Select(m => m.Address)),
                    subject = message.Subject ?? "(No Subject)",
                    date = message.Date.ToLocalTime().ToString("MMM dd, h:mm tt"),
                    body = message.TextBody ?? "",
                    htmlBody = message.HtmlBody ?? "",
                    hasAttachments = attachments.Count > 0,
                    attachments
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load single email via AJAX");
            return new JsonResult(new { success = false, message = ex.Message });
        }
    }
}
