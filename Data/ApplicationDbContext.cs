using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeamProject.Models;

namespace TeamProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<toDoTask> Task { get; set; }
        public DbSet<toDoList> todolist { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Appel de la méthode de base
            base.OnModelCreating(modelBuilder);

            // --- CONFIGURATION DES RELATIONS ---
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.doList)
                .WithOne(l => l.User)
                .HasForeignKey<ApplicationUser>(u => u.toDoListId)
                .OnDelete(DeleteBehavior.Cascade);
                

            modelBuilder.Entity<toDoTask>()
                .HasOne(t => t.ToDoList)
                .WithMany(l => l.Tasks)
                .HasForeignKey(t => t.ToDoListId);



        }
    }
}
