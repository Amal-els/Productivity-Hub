namespace TeamProject.ViewModels
{
    public class MeetingInvitationViewModel
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public string MeetingLink { get; set; }
        public string OrganizerName { get; set; }
        public string OrganizerEmail { get; set; }
        public string CurrentResponseStatus { get; set; }
        public List<ParticipantInfo> Participants { get; set; }
    }

    public class ParticipantInfo
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string ResponseStatus { get; set; }
    }
    
}
