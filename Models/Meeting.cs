using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamProject.Models
{
    public class Meeting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CalendarEventId { get; set; }

        [ForeignKey("CalendarEventId")]
        public virtual CalendarEvent CalendarEvent { get; set; }

        [MaxLength(500)]
        public string? MeetingLink { get; set; }

        [Required]
        public string OrganizerId { get; set; }

        [ForeignKey("OrganizerId")]
        public virtual ApplicationUser Organizer { get; set; }

        public bool RequiresResponse { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

