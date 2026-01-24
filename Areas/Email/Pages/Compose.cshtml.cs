using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TeamProject.Models;
using TeamProject.Services;

namespace TeamProject.Areas.Email.Pages
{
    [Authorize] 
    public class ComposeModel : PageModel
    {
        private readonly EmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ComposeModel> _logger;

        public ComposeModel(
            EmailService emailService, 
            UserManager<ApplicationUser> userManager,
            ILogger<ComposeModel> logger)
        {
            _emailService = emailService;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        [Required(ErrorMessage = "Recipient email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string To { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Subject is required")]
        public string Subject { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Message body is required")]
        public string Body { get; set; } = string.Empty;

        [BindProperty]
        public bool IsHtml { get; set; } = true;

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("=== OnGetAsync ===");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("=== OnPostAsync START ===");
            
            // Log Request details
            _logger.LogInformation($"Request.Method: {Request.Method}");
            _logger.LogInformation($"Request.ContentType: {Request.ContentType}");
            _logger.LogInformation($"Request.HasFormContentType: {Request.HasFormContentType}");
            
            // Log Form values directly from Request
            if (Request.HasFormContentType)
            {
                _logger.LogInformation("Form values from Request.Form:");
                foreach (var key in Request.Form.Keys)
                {
                    _logger.LogInformation($"  {key}: {Request.Form[key]}");
                }
            }
            
            // Log bound property values
            _logger.LogInformation($"Bound Property - To: '{To}' (Length: {To?.Length ?? 0})");
            _logger.LogInformation($"Bound Property - Subject: '{Subject}' (Length: {Subject?.Length ?? 0})");
            _logger.LogInformation($"Bound Property - Body: '{Body}' (Length: {Body?.Length ?? 0})");
            _logger.LogInformation($"Bound Property - IsHtml: {IsHtml}");

            // Get current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("User is null");
                TempData["Error"] = "User not found. Please log in.";
                return RedirectToPage("/Account/Login");
            }

            _logger.LogInformation($"User: {user.Email}");
            _logger.LogInformation($"ModelState.IsValid: {ModelState.IsValid}");
            _logger.LogInformation($"ModelState.ErrorCount: {ModelState.ErrorCount}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState INVALID:");
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state != null && state.Errors.Count > 0)
                    {
                        foreach (var error in state.Errors)
                        {
                            _logger.LogWarning($"  {key}: {error.ErrorMessage}");
                        }
                    }
                }
                return Page();
            }

            try
            {
                _logger.LogInformation($"Sending email to: {To}");
                await _emailService.SendEmailAsync(To, Subject, Body, IsHtml);
                
                _logger.LogInformation("✅ Email sent successfully!");
                TempData["Success"] = $"Email sent successfully to {To}";
                return RedirectToPage("/Inbox", new { area = "Email" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to send email");
                TempData["Error"] = "Failed to send email: " + ex.Message;
                return Page();
            }
        }
    }
}
