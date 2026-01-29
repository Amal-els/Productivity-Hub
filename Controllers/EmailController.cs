using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TeamProject.Areas.Email.Pages;
using TeamProject.Models;
using TeamProject.Services;

namespace TeamProject.Controllers
{
    [Authorize]
    public class EmailController : Controller
    {
        private readonly EmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;


        public EmailController(UserManager<ApplicationUser> userManager, EmailService emailService)

        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<IActionResult> Inbox()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var email = await _userManager.GetEmailAsync(user);
            var messages = await _emailService.GetInboxAsync();
            return View(messages); 
        }
        
        [HttpGet]
        public async Task<IActionResult> ComposeAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new ComposeEmailViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Compose(ComposeEmailViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // Get the user's email & app password from your DB
            var fromEmail = user.Email; // Gmail address
            var appPassword = user.EmailPassword; // stored securely in DB

            if (string.IsNullOrEmpty(appPassword))
            {
                ModelState.AddModelError("", "No Gmail app password found. Please set it first.");
                return View(model);
            }

            try
            {
                await _emailService.SendEmailAsync(
                    fromEmail,    // sender email
                    appPassword,  // sender app password
                    model.To,
                    model.Subject,
                    model.Body,
                    model.IsHtml
                );

                TempData["Success"] = $"Email sent successfully to {model.To}";
                return RedirectToAction("Inbox");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to send email: " + ex.Message);
                return View(model);
            }
        }

    }
}
