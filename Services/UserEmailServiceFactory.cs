// Services/UserEmailServiceFactory.cs
using Microsoft.AspNetCore.Identity;
using TeamProject.Models;

namespace TeamProject.Services
{
    public class UserEmailServiceFactory
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserEmailServiceFactory(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public EmailService CreateForUser(ApplicationUser user)
        {
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.EmailPassword))
            {
                throw new InvalidOperationException("User email credentials not configured");
            }

            return new EmailService(
                user.Email,
                user.EmailPassword  
            );
        }
    }
}