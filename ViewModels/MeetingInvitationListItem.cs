namespace TeamProject.ViewModels
{
    public class MeetingInvitationListItem
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string OrganizerName { get; set; }
        public string ResponseStatus { get; set; }
        public string Location { get; set; }
    }
}
