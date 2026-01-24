using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TeamProject.Models
{
    public enum PomodoroType
    {
        Work,
        Break,
        LongBreak
    }

    public class PomodoroSession
    {
        [Key]
        public int SessionId { get; set; }  

        [Required]
        public string UserId { get; set; }  

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }  

        [Required]
        public DateTime StartTime { get; set; }  

        [Required]
        public DateTime EndTime { get; set; }    

        [Required]
        public PomodoroType Type { get; set; }   

        [Required]
        public bool IsCompleted { get; set; }    

        [NotMapped]
        public TimeSpan Duration => EndTime - StartTime; 
    }
}
