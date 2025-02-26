using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PrePassHackathonTeamEApi.Models;

[ApiController]
[Route("api/[controller]")]
public class CalendarEventsController : ControllerBase
{
    private readonly List<CalendarEvent> _events = new();  // Temporary in-memory storage
    private readonly IMemoryCache _cache;
    private const string CacheKey = "CalendarEvents";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromDays(30);

    public CalendarEventsController(IMemoryCache cache)
    {
        _cache = cache;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CalendarEvent>> GetAll([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var events = _cache.GetOrCreate(CacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return _events;
        });

        var query = events.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(e => e.StartTime >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(e => e.EndTime <= endDate.Value);

        return Ok(query.ToList());
    }

    //[HttpGet("family-member/{familyMemberId}")]
    //public ActionResult<IEnumerable<CalendarEvent>> GetByFamilyMember(int familyMemberId)
    //{
    //    var events = _events.Where(e => e.FamilyMemberId == familyMemberId).ToList();
    //    return Ok(events);
    //}

    private void SetCache(List<CalendarEvent> calendarEvents)
    {

        List<CalendarEvent> cachedEvents = GetCache();
        cachedEvents.AddRange(calendarEvents);
        var cacheOptions = new MemoryCacheEntryOptions()
               .SetSlidingExpiration(TimeSpan.FromDays(10));       

        _cache.Set(CacheKey, cachedEvents, cacheOptions);
      
    }


    private List<CalendarEvent> GetCache()
    {
        if (!_cache.TryGetValue(CacheKey, out List<CalendarEvent>? events))
        {          
            if (events == null)
            {
                events = new List<CalendarEvent>();
            } 
            return events;
        }

        return new List<CalendarEvent>();
    }

    [HttpPost]
    public ActionResult<CalendarEvent> Create(CalendarEvent calendarEvent)
    {
        calendarEvent.Id = _events.Count + 1;
        _events.Add(calendarEvent);
        SetCache(new List<CalendarEvent> { calendarEvent});     
        return CreatedAtAction(nameof(GetById), new { id = calendarEvent.Id }, calendarEvent);
    }

    [HttpGet("{id}")]
    public ActionResult<CalendarEvent> GetById(int id)
    {
       
       List<CalendarEvent> events = GetCache();
        var calendarEvent = events.FirstOrDefault(e => e.Id == id);
        if (calendarEvent == null)
            return NotFound();

        return Ok(calendarEvent);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, CalendarEvent calendarEvent)
    {
        List<CalendarEvent> events = GetCache();
        var existing = events.FirstOrDefault(e => e.Id == id);
        if (existing == null)
            return NotFound();

        existing.Title = calendarEvent.Title;
        existing.Description = calendarEvent.Description;
        existing.StartTime = calendarEvent.StartTime;
        existing.EndTime = calendarEvent.EndTime;
        existing.Priority = calendarEvent.Priority;
        existing.IsRecurring = calendarEvent.IsRecurring;       
        existing.RecurrencePattern = calendarEvent.RecurrencePattern;

        SetCache(events);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        List<CalendarEvent> events = GetCache();
        var calendarEvent = events.FirstOrDefault(e => e.Id == id);
        if (calendarEvent == null)
            return NotFound();

        _events.Remove(calendarEvent);
        _cache.Remove(CacheKey); // Invalidate cache
        return NoContent();
    }
} 