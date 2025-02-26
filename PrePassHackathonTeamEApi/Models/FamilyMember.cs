namespace PrePassHackathonTeamEApi.Models
{
    public class FamilyMember
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;  // e.g., Parent, Child, etc.
        public List<CalendarEvent> Events { get; set; } = new();
    }
}
