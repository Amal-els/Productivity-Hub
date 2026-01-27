using System.ComponentModel.DataAnnotations;

namespace TeamProject.ViewModels
{
    public class CalendarEventViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan StartClock { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan EndClock { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        public bool IsAllDay { get; set; }

        [StringLength(50)]
        public string Color { get; set; } = "#3788d8";

        [Required]
        public string EventType { get; set; } = "Personal";

        public bool HasReminder { get; set; }

        [Range(5, 1440, ErrorMessage = "Reminder must be between 5 and 1440 minutes")]
        public int ReminderMinutesBefore { get; set; }

        // For meetings
        public string? Agenda { get; set; }
        public string? MeetingLink { get; set; }
        
        public List<string> ParticipantEmails { get; set; } = new List<string>();
    }
}
