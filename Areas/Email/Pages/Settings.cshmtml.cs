using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TeamProject.Models;

namespace TeamProject.Areas.Email.Pages
{
    [Authorize]
    [ValidateAntiForgeryToken]
    public class SettingsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SettingsModel> _logger;

        public SettingsModel(UserManager<ApplicationUser> userManager, ILogger<SettingsModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [PasswordPropertyText]
            [Display(Name = "Email Password")]
            public string EmailPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("OnGetAsync called");
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null) 
            {
                _logger.LogWarning("User not found");
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            Input = new InputModel
            {
                EmailPassword = user.EmailPassword ?? string.Empty
            };

            _logger.LogInformation($"User {user.Email} loaded settings page");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("OnPostAsync called - Form submitted");
            _logger.LogInformation($"Input object is null: {Input == null}");
            
            if (Input != null)
            {
                _logger.LogInformation($"EmailPassword received: {(string.IsNullOrEmpty(Input.EmailPassword) ? "EMPTY" : $"HAS VALUE (length: {Input.EmailPassword.Length})")}");
            }
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found in OnPost");
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            if (Input == null || string.IsNullOrWhiteSpace(Input.EmailPassword))
            {
                _logger.LogWarning("Email password is empty");
                StatusMessage = "Email password cannot be empty.";
                
                // Re-initialize Input so the page can render
                Input = new InputModel
                {
                    EmailPassword = user.EmailPassword ?? string.Empty
                };
                
                return Page();
            }

            _logger.LogInformation($"Updating email password for user {user.Email}");
            user.EmailPassword = Input.EmailPassword;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogError("Failed to update user");
                StatusMessage = "Failed to save email password.";
                return Page();
            }

            _logger.LogInformation("Email password saved successfully, redirecting to Inbox");
            TempData["Success"] = "Email password saved successfully!";
            return Redirect("~/Email/Index");
        }
    }
}