using Microsoft.AspNetCore.Mvc;
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

   // private readonly MemoryCacheService _cacheService;
    private readonly FileDataService _fileDataService;

    public CalendarEventsController(FileDataService fileDataService)
    {
        //_cache = cache;

        //_cacheService = memoryCacheService;
        _fileDataService = fileDataService;
   
    }

    [HttpGet]
    public ActionResult<IEnumerable<CalendarEvent>> GetAll([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        List<CalendarEvent> cachedEvents = _fileDataService.ReadItemsAsync().GetAwaiter().GetResult();  

        var query = cachedEvents.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(e => e.StartTime >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(e => e.EndTime <= endDate.Value);

        return Ok(query.ToList());
    }   

    //private void SetCache(List<CalendarEvent> calendarEvents)
    //{

    //    List<CalendarEvent> cachedEvents = _fileDataService.ReadItemsAsync().GetAwaiter().GetResult();
    //    cachedEvents.AddRange(calendarEvents);   

    //    _fileDataService.WriteItemsAsync(cachedEvents).GetAwaiter().GetResult();
    //    //var cacheOptions = new MemoryCacheEntryOptions()
    //    //    .SetAbsoluteExpiration(CacheDuration) // Expire in 10 mins
    //    //    .SetSlidingExpiration(CacheDuration); // Reset expiration if accessed   
    //    //_cacheService.Set(CacheKey, stringList, CacheDuration);
    //    //_cache.Set(CacheKey, cachedEvents, cacheOptions);   
    //}


    ////private List<CalendarEvent> GetCache()
    ////{

    ////  return   _fileDataService.ReadItemsAsync().GetAwaiter().GetResult();

    ////    //List<CalendarEvent> cacheList = new List<CalendarEvent>();
    ////    //if (!_cacheService.TryGet(CacheKey, out List<string>? events))
    ////    //{          
    ////    //    if (events == null)
    ////    //    {
    ////    //        return cacheList;
    ////    //    }
    ////    //    foreach (string item in events)
    ////    //    {
    ////    //        cacheList.Add(JsonConvert.DeserializeObject<CalendarEvent>(item));
    ////    //    }
    ////    //}

       
    ////}

    [HttpPost]
    public ActionResult<CalendarEvent> Create(CalendarEvent calendarEvent)
    {     
        
        calendarEvent.EventId = Guid.NewGuid().ToString();
        _events.Add(calendarEvent);
       // SetCache(new List<CalendarEvent> { calendarEvent});
      _fileDataService.AppendItemAsync(calendarEvent).GetAwaiter().GetResult();

        return GetById(calendarEvent.EventId);
    }

    [HttpGet("{eventid}")]
    public ActionResult<CalendarEvent> GetById(string eventid)
    {

        List<CalendarEvent> events = _fileDataService.ReadItemsAsync().GetAwaiter().GetResult();
        var calendarEvent = events.FirstOrDefault(e => e.EventId == eventid);
        if (calendarEvent == null)
            return NoContent();

        return Ok(calendarEvent);
    }    

    [HttpPut("{eventid}")]
    public IActionResult Update(string eventid, CalendarEvent calendarEvent)
    {
        List<CalendarEvent> events = _fileDataService.ReadItemsAsync().GetAwaiter().GetResult();
        var existing = events.FirstOrDefault(e => e.EventId == eventid);
        if (existing == null)
            return NotFound();

        existing.Title = calendarEvent.Title;
        existing.Description = calendarEvent.Description;
        existing.StartTime = calendarEvent.StartTime;
        existing.EndTime = calendarEvent.EndTime;
        existing.Priority = calendarEvent.Priority;
        existing.IsRecurring = calendarEvent.IsRecurring;       
        existing.RecurrencePattern = calendarEvent.RecurrencePattern;

       // SetCache(events);

        return NoContent();
    }

    [HttpDelete("{eventid}")]
    public IActionResult Delete(string eventid)
    {
        List<CalendarEvent> events = _fileDataService.ReadItemsAsync().GetAwaiter().GetResult();
        var calendarEvent = events.FirstOrDefault(e => e.EventId == eventid);
        if (calendarEvent == null)
            return NotFound();

        _events.Remove(calendarEvent);
       _fileDataService.WriteItemsAsync(events).GetAwaiter().GetResult(); // Invalidate cache
        return Ok();
    }
} 