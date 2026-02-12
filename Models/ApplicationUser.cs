using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace TeamProject.Models;
    public class ApplicationUser : IdentityUser
    {
        public string? EmailPassword { get; set; }
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();
<<<<<<< ours

        public string? EmailPassword { get; set; }

        public string? ContactsSheetId{ get; set; }
        // Navigation properties
=======
        public Guid? toDoListId { get; set; }
        public virtual toDoList? doList { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
>>>>>>> theirs
        public virtual ICollection<CalendarEvent> CalendarEvents { get; set; }
        public virtual ICollection<EventParticipant> EventParticipations { get; set; }
        public virtual ICollection<Meeting> OrganizedMeetings { get; set; }
    }
    