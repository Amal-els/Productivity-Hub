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

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    ModelState.AddModelError("", "User not found");
                    return RedirectToAction("Login", "Account");
                }

                var userEmail = await _userManager.GetEmailAsync(user);

                await _emailService.SendEmailAsync(
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
