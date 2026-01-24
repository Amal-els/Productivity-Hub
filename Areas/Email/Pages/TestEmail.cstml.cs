using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeamProject.Services;

namespace TeamProject.Areas.Email.Pages
{
    public class TestEmailModel : PageModel
    {
        private readonly EmailService _emailService;
        private readonly ILogger<TestEmailModel> _logger;

        public TestEmailModel(EmailService emailService, ILogger<TestEmailModel> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public string? TestResult { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnGetTestConnectionAsync()
        {
            try
            {
                _logger.LogInformation("Testing email connection...");
                
                var messages = await _emailService.GetInboxAsync(5);
                
                TestResult = $"✅ Success! Connected to email.\nFound {messages.Count} messages.";
                
                if (messages.Any())
                {
                    TestResult += "\n\nFirst message:";
                    TestResult += $"\nFrom: {messages[0].From}";
                    TestResult += $"\nSubject: {messages[0].Subject}";
                    TestResult += $"\nDate: {messages[0].Date}";
                }
                
                TempData["Success"] = "Connection successful!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email connection test failed");
                TestResult = $"❌ Failed: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}";
                TempData["Error"] = ex.Message;
            }

            return Page();
        }
    }
}