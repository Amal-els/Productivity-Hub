using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamProject.Models
{
    public class EventParticipant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CalendarEventId { get; set; }

        [ForeignKey("CalendarEventId")]
        public virtual CalendarEvent CalendarEvent { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        // Response status: Pending, Accepted, Declined, Tentative
        [Required]
        [MaxLength(20)]
        public string ResponseStatus { get; set; } = "Pending";

        public DateTime? ResponseDate { get; set; }

        public bool IsOrganizer { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}