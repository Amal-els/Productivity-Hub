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
    [IgnoreAntiforgeryToken] // TEMPORARY - just for testing
    public class ComposeModel : PageModel
    {
        private readonly UserEmailServiceFactory _emailServiceFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ComposeModel> _logger;

        public ComposeModel(
            UserEmailServiceFactory emailServiceFactory,
            UserManager<ApplicationUser> userManager,
            ILogger<ComposeModel> logger)
        {
            _emailServiceFactory = emailServiceFactory;
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
                _logger.LogWarning("User not found. Redirecting to login.");
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("=== OnPostAsync START ===");

            // Log Request info
            _logger.LogInformation($"Request Method: {Request.Method}");
            _logger.LogInformation($"ContentType: {Request.ContentType}");
            _logger.LogInformation($"HasFormContentType: {Request.HasFormContentType}");

            // Log all form fields
            if (Request.HasFormContentType)
            {
                _logger.LogInformation("Form data received:");
                foreach (var key in Request.Form.Keys)
                {
                    _logger.LogInformation($"  {key}: {Request.Form[key]}");
                }
            }

            // Log bound properties
            _logger.LogInformation($"Bound Property - To: '{To}'");
            _logger.LogInformation($"Bound Property - Subject: '{Subject}'");
            _logger.LogInformation($"Bound Property - Body: '{Body}'");
            _logger.LogInformation($"Bound Property - IsHtml: {IsHtml}");

            // Validate model state
            _logger.LogInformation($"ModelState.IsValid: {ModelState.IsValid}");
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        _logger.LogWarning($"ModelState error - {key}: {error.ErrorMessage}");
                    }
                }
                return Page();
            }

            // Get user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("User not found. Redirecting to login.");
                TempData["Error"] = "User not found. Please log in.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            if (string.IsNullOrEmpty(user.EmailPassword))
            {
                TempData["Error"] = "Your Gmail app password is not configured. Please set it in settings.";
                return RedirectToPage("/Email/Settings");
            }

            try
            {
                _logger.LogInformation($"Sending email to {To}...");
                
                var emailService = _emailServiceFactory.CreateForUser(user);
                
                await emailService.SendEmailAsync(
                    fromEmail: user.Email,
                    appPassword: user.EmailPassword,
                    to: To,
                    subject: Subject,
                    body: Body,
                    isHtml: IsHtml,
                    fromName: user.DisplayName
                );
                
                _logger.LogInformation("Email sent successfully.");
                TempData["Success"] = $"Email sent successfully to {To}";
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email");
                TempData["Error"] = "Failed to send email: " + ex.Message;
                return Page();
            }
        }
    }
}