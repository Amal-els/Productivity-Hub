using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;

namespace TeamProject.Areas.Email.Pages
{
    public class SentModel : PageModel
    {
        public List<MimeMessage> Messages { get; set; } = new();

        public void OnGet()
        {
            // TODO: Implement sent messages retrieval
        }
    }
}