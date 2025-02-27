using Newtonsoft.Json;

namespace PrePassHackathonTeamEApi.Models
{
    public class CalendarEvent
    {
        //[JsonProperty]
        //public int? Id { get; set; }

        [JsonProperty]
        public string? EventId { get; set; }

        [JsonProperty]
        public string Title { get; set; } = string.Empty;

        [JsonProperty]
        public string Description { get; set; } = string.Empty;

        [JsonProperty]
        public DateTime StartTime { get; set; }

        [JsonProperty]
        public DateTime EndTime { get; set; }

        [JsonProperty]
        public string Attendees { get; set; } = string.Empty;
        //public int FamilyMemberId { get; set; }
        //public FamilyMember? FamilyMember { get; set; }

        [JsonProperty]
        public EventPriority Priority { get; set; }

        [JsonProperty]
        public bool IsRecurring { get; set; }

        [JsonProperty]
        public RecurrencePattern? RecurrencePattern { get; set; }
    }

    public enum EventPriority
    {
        Low,
        Medium,
        High
    }

    public class RecurrencePattern
    {
        public RecurrenceType Type { get; set; }
        public int Interval { get; set; }  // e.g., every 2 weeks
        public DateTime? EndDate { get; set; }
    }

    public enum RecurrenceType
    {
        Daily,
        Weekly,
        Monthly,
        Yearly
    }

}
