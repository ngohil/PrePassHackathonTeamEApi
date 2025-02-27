using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrePassHackathonTeamEApi;
using PrePassHackathonTeamEApi.Models;

namespace PrePassHackathonTeamEApi.Controllers;

/// <summary>
/// Controller for managing calendar events with file-based persistence
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CalendarEventsController : ControllerBase
{  
    private readonly FileDataService _fileDataService;

    /// <summary>
    /// Initializes a new instance of the CalendarEventsController
    /// </summary>
    /// <param name="fileDataService">Service for file-based data operations</param>
    public CalendarEventsController(FileDataService fileDataService)
    {        
        _fileDataService = fileDataService;   
    }

    /// <summary>
    /// Retrieves all calendar events with optional date filtering
    /// </summary>
    /// <param name="startDate">Optional start date to filter events</param>
    /// <param name="endDate">Optional end date to filter events</param>
    /// <returns>List of calendar events matching the criteria</returns>
    /// <response code="200">Returns the list of calendar events</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

    /// <summary>
    /// Creates a new calendar event
    /// </summary>
    /// <param name="calendarEvent">The calendar event to create</param>
    /// <returns>The newly created calendar event</returns>
    /// <response code="200">Returns the created calendar event</response>
    /// <response code="400">If the calendar event data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<CalendarEvent> Create(CalendarEvent calendarEvent)
    {     
        
        calendarEvent.EventId = Guid.NewGuid().ToString();      
      _fileDataService.AppendItemAsync(calendarEvent).GetAwaiter().GetResult();

        return GetById(calendarEvent.EventId);
    }

    /// <summary>
    /// Retrieves a specific calendar event by its ID
    /// </summary>
    /// <param name="eventid">The unique identifier of the calendar event</param>
    /// <returns>The requested calendar event</returns>
    /// <response code="200">Returns the requested calendar event</response>
    /// <response code="204">If the calendar event is not found</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpGet("{eventid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<CalendarEvent> GetById(string eventid)
    {

        List<CalendarEvent> events = _fileDataService.ReadItemsAsync().GetAwaiter().GetResult();
        var calendarEvent = events.FirstOrDefault(e => e.EventId == eventid);
        if (calendarEvent == null)
            return NoContent();

        return Ok(calendarEvent);
    }    

    /// <summary>
    /// Updates an existing calendar event
    /// </summary>
    /// <param name="calendarEvent">The updated calendar event information</param>
    /// <returns>The updated calendar event</returns>
    /// <response code="200">Returns the updated calendar event</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="204">If the calendar event is not found</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpPut()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

    /// <summary>
    /// Deletes a specific calendar event
    /// </summary>
    /// <param name="eventid">The unique identifier of the calendar event to delete</param>
    /// <returns>Success status</returns>
    /// <response code="200">If the deletion was successful</response>
    /// <response code="204">If the calendar event was not found</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpDelete("{eventid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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