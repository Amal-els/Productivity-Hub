using TeamProject.Controllers;

namespace TeamProject.ViewModels
{
    public class MeetingsIndexViewModel
    {
        public List<MeetingDetailsViewModel> Meetings { get; set; }
        public string CurrentFilter { get; set; }
        public int UpcomingCount { get; set; }
        public int PendingCount { get; set; }
        public int OrganizedCount { get; set; }
        public int InvitationsCount { get; set; }
    }
}
