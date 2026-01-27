using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamProject.Models
{
    public class CalendarEvent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [MaxLength(100)]
        public string? Location { get; set; }

        public bool IsAllDay { get; set; }

        [MaxLength(50)]
        public string Color { get; set; } = "#3788d8";

        // Event Type: Personal or Meeting
        [Required]
        [MaxLength(50)]
        public string EventType { get; set; } = "Personal";

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public bool HasReminder { get; set; }
        public int ReminderMinutesBefore { get; set; } = 15;
        public bool ReminderSent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<EventParticipant> Participants { get; set; }
        public virtual Meeting Meeting { get; set; }
    }
}
