using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrePassHackathonTeamEApi;
using PrePassHackathonTeamEApi.Models;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CalendarEventsController : ControllerBase
{  
    private readonly FileDataService _fileDataService;

    public CalendarEventsController(FileDataService fileDataService)
    {        
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

    [HttpPost]
    public ActionResult<CalendarEvent> Create(CalendarEvent calendarEvent)
    {     
        
        calendarEvent.EventId = Guid.NewGuid().ToString();      
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

    [HttpPut()]
    public ActionResult<CalendarEvent> Update(CalendarEvent calendarEvent)
    {
        if(calendarEvent == null || string.IsNullOrEmpty(calendarEvent.EventId ) )
            return BadRequest();
        List<CalendarEvent> events = _fileDataService.ReadItemsAsync().GetAwaiter().GetResult();
        var existing = events.FirstOrDefault(e => e.EventId == calendarEvent.EventId);
        if (existing == null)
            return NoContent();       

        events.Remove(existing);
        events.Add(calendarEvent);

        _fileDataService.WriteItemsAsync(events).GetAwaiter().GetResult();

         return GetById(calendarEvent.EventId!);
    }

    [HttpDelete("{eventid}")]
    public IActionResult Delete(string eventid)
    {
        List<CalendarEvent> events = _fileDataService.ReadItemsAsync().GetAwaiter().GetResult();
        var calendarEvent = events.FirstOrDefault(e => e.EventId == eventid);
        if (calendarEvent == null)
            return NoContent();

        events.Remove(calendarEvent);
       _fileDataService.WriteItemsAsync(events).GetAwaiter().GetResult(); // Invalidate cache
        return Ok();
    }
} 