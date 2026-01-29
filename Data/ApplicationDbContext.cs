using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TeamProject.Models;

namespace TeamProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<EventParticipant> EventParticipants { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<PomodoroSession> PomodoroSessions { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CalendarEvent>()
                .HasOne(e => e.User)
                .WithMany(u => u.CalendarEvents)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CalendarEvent>()
                .HasOne(e => e.Meeting)
                .WithOne(m => m.CalendarEvent)
                .HasForeignKey<Meeting>(m => m.CalendarEventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Meeting>()
                .HasOne(m => m.Organizer)
                .WithMany(u => u.OrganizedMeetings)
                .HasForeignKey(m => m.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EventParticipant>()
                .HasOne(ep => ep.CalendarEvent)
                .WithMany(e => e.Participants)
                .HasForeignKey(ep => ep.CalendarEventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EventParticipant>()
                .HasOne(ep => ep.User)
                .WithMany(u => u.EventParticipations)
                .HasForeignKey(ep => ep.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CalendarEvent>()
                .HasIndex(e => e.UserId);

            builder.Entity<CalendarEvent>()
                .HasIndex(e => e.StartTime);

            builder.Entity<EventParticipant>()
                .HasIndex(ep => new { ep.CalendarEventId, ep.UserId })
                .IsUnique();

            builder.Entity<EmailLog>()
                .HasIndex(e => e.RelatedEventId);

            builder.Entity<PomodoroSession>();

        }
    }
}

