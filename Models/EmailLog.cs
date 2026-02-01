using System.ComponentModel.DataAnnotations;

namespace TeamProject.Models
{
    public class EmailLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string RecipientEmail { get; set; }

        [Required]
        [MaxLength(300)]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        // Invitation, Update, Cancellation, Reminder, Response
        [MaxLength(50)]
        public string EmailType { get; set; }

        public int? RelatedEventId { get; set; }

        public bool IsSent { get; set; }

        public DateTime? SentAt { get; set; }

        public string? ErrorMessage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
