namespace TeamProject.Models.Dtos
{
    public class PomodoroSessionDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Type { get; set; }
    }
}