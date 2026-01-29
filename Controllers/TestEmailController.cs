using Microsoft.AspNetCore.Mvc;
using TeamProject.Services;

namespace TeamProject.Controllers
{
    public class TestEmailController : Controller
    {
        private readonly IEmailNotifService _emailService;

        public TestEmailController(IEmailNotifService emailService)
        {
            _emailService = emailService;
        }

        public async Task<IActionResult> SendTest()
        {
            await _emailService.SendTestEmailAsync("bensalahghaida1@gmail.com");
            return Content("Test email sent. Check inbox & EmailLogs table.");
        }
    }

}
