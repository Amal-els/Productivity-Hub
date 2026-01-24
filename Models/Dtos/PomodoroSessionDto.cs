namespace TeamProject.Models.Dtos
{
    public class PomodoroSessionDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public PomodoroType Type { get; set; }
    }
}