using Newtonsoft.Json;

namespace PrePassHackathonTeamEApi.Models
{
    /// <summary>
    /// Represents a calendar event in the system
    /// </summary>
    public class CalendarEvent
    {
        //[JsonProperty]
        //public int? Id { get; set; }

        /// <summary>
        /// Unique identifier for the calendar event
        /// </summary>
        [JsonProperty]
        public string? EventId { get; set; }

        /// <summary>
        /// Title of the calendar event
        /// </summary>
        [JsonProperty]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the calendar event
        /// </summary>
        [JsonProperty]
        public string? Description { get; set; }

        /// <summary>
        /// Start time of the calendar event
        /// </summary>
        [JsonProperty]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// End time of the calendar event
        /// </summary>
        [JsonProperty]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Location where the event takes place
        /// </summary>
        [JsonProperty]
        public string? Location { get; set; }

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
