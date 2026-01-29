// Models/ComposeEmailViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace TeamProject.Models
{
    public class ComposeEmailViewModel
    {
        [Required(ErrorMessage = "Recipient email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string To { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject is required")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message body is required")]
        public string Body { get; set; } = string.Empty;

        public bool IsHtml { get; set; } = true;
    }
}