namespace TeamProject.ViewModels
{
    public class CalendarViewModel
    {
        public string View { get; set; } = "month"; // day, week, month
        public DateTime CurrentDate { get; set; } = DateTime.Today;
        public List<CalendarEventDisplayModel> Events { get; set; } = new List<CalendarEventDisplayModel>();
    }

    public class CalendarEventDisplayModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Color { get; set; }
        public bool AllDay { get; set; }
        public string EventType { get; set; }
        public string Location { get; set; }
        public bool IsMeeting { get; set; }
        public string ResponseStatus { get; set; }
        public bool IsOrganizer { get; set; }
    }
}
