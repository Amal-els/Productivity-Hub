namespace TeamProject.ViewModels
{
    public class MeetingDetailsViewModel
    {
        public int MeetingId { get; set; }
        public int EventId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public string OrganizerName { get; set; }
        public string OrganizerEmail { get; set; }
        public bool IsOrganizer { get; set; }
        public int ParticipantCount { get; set; }
        public string ResponseStatus { get; set; }
        public int AcceptedCount { get; set; }
        public string MeetingLink { get; set; }
    }
}
