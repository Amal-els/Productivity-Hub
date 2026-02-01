using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TeamProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();

        public string? EmailPassword { get; set; }

        // Navigation properties
        public virtual ICollection<CalendarEvent> CalendarEvents { get; set; }
        public virtual ICollection<EventParticipant> EventParticipations { get; set; }
        public virtual ICollection<Meeting> OrganizedMeetings { get; set; }
    }
}
