using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeamProject.Models;
using System.Reflection.Emit;
namespace TeamProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
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
        public DbSet<toDoTask> Task { get; set; }
        public DbSet<toDoList> todolist { get; set; }
        public DbSet<Note> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {base.OnModelCreating(modelBuilder);
        
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.doList)
                .WithOne(l => l.User)
                .HasForeignKey<ApplicationUser>(u => u.toDoListId)
                .OnDelete(DeleteBehavior.Cascade);
                

            modelBuilder.Entity<toDoTask>()
                .HasOne(t => t.ToDoList)
                .WithMany(l => l.Tasks)
                .HasForeignKey(t => t.ToDoListId);
           
            modelBuilder.Entity<Note>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
           
            modelBuilder.Entity<CalendarEvent>()
                .HasOne(e => e.User)
                .WithMany(u => u.CalendarEvents)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CalendarEvent>()
                .HasOne(e => e.Meeting)
                .WithOne(m => m.CalendarEvent)
                .HasForeignKey<Meeting>(m => m.CalendarEventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Meeting>()
                .HasOne(m => m.Organizer)
                .WithMany(u => u.OrganizedMeetings)
                .HasForeignKey(m => m.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventParticipant>()
                .HasOne(ep => ep.CalendarEvent)
                .WithMany(e => e.Participants)
                .HasForeignKey(ep => ep.CalendarEventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventParticipant>()
                .HasOne(ep => ep.User)
                .WithMany(u => u.EventParticipations)
                .HasForeignKey(ep => ep.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CalendarEvent>()
                .HasIndex(e => e.UserId);

            modelBuilder.Entity<CalendarEvent>()
                .HasIndex(e => e.StartTime);

            modelBuilder.Entity<EventParticipant>()
                .HasIndex(ep => new { ep.CalendarEventId, ep.UserId })
                .IsUnique();

            modelBuilder.Entity<EmailLog>()
                .HasIndex(e => e.RelatedEventId);

            modelBuilder.Entity<PomodoroSession>();
        }

        }
    }

