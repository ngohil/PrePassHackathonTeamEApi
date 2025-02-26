using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using PrePassHackathonTeamEApi;
using PrePassHackathonTeamEApi.Models;

//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CalendarEventsController : ControllerBase
{
    private readonly List<CalendarEvent> _events = new();  // Temporary in-memory storage
   // private readonly IMemoryCache _cache;
    private const string CacheKey = "CalendarEvents";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(10);

    private readonly MemoryCacheService _cacheService;

    public CalendarEventsController(MemoryCacheService memoryCacheService)
    {
        //_cache = cache;

        _cacheService = memoryCacheService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CalendarEvent>> GetAll([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        List<CalendarEvent> cachedEvents = GetCache(); 

        var query = cachedEvents.AsQueryable();

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
              .SetAbsoluteExpiration(CacheDuration) // Expire in 10 mins
              .SetSlidingExpiration(CacheDuration); // Reset expiration if accessed   
        List<string> stringList = new();
        foreach (var calendarEvent in cachedEvents)
        {
            stringList.Add( JsonConvert.SerializeObject(calendarEvent));
        }
        _cacheService.Set(CacheKey, stringList, CacheDuration);
        //_cache.Set(CacheKey, cachedEvents, cacheOptions);   
    }


    private List<CalendarEvent> GetCache()
    {

        List<CalendarEvent> cacheList = new List<CalendarEvent>();
        if (!_cacheService.TryGet(CacheKey, out List<string>? events))
        {          
            if (events == null)
            {
                return cacheList;
            }
            foreach (string item in events)
            {
                cacheList.Add(JsonConvert.DeserializeObject<CalendarEvent>(item));
            }
        }

        return cacheList;
    }

    [HttpPost]
    public ActionResult<CalendarEvent> Create(CalendarEvent calendarEvent)
    {
        calendarEvent.Id = _events.Count + 1;
        _events.Add(calendarEvent);
        SetCache(new List<CalendarEvent> { calendarEvent});     
        return GetById(calendarEvent.Id);
    }

    [HttpGet("{id}")]
    public ActionResult<CalendarEvent> GetById(int id)
    {
       
       List<CalendarEvent> events = GetCache();
        var calendarEvent = events.FirstOrDefault(e => e.Id == id);
        if (calendarEvent == null)
            return NoContent();

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
        _cacheService.Remove(CacheKey); // Invalidate cache
        return NoContent();
    }
} 