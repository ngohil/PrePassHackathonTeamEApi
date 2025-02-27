using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PrePassHackathonTeamEApi.Models;

namespace PrePassHackathonTeamEApi.Controllers;

/// <summary>
/// Controller for managing family members
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FamilyMembersController : ControllerBase
{
    private readonly IMemoryCache _memoryCache;
    private const string CACHE_KEY = "FamilyMembers";

    private List<FamilyMember> familyMembers = new List<FamilyMember>();

    public FamilyMembersController(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// Retrieves all family members
    /// </summary>
    /// <returns>A list of all family members</returns>
    /// <response code="200">Returns the list of family members</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<FamilyMember>> GetAll()
    {      
        return Ok(GetAllMembers());
    }

    private List<FamilyMember> GetAllMembers()
    {
     
        if (!_memoryCache.TryGetValue(CACHE_KEY, out List<FamilyMember> familyMembers))
        {
            familyMembers = new List<FamilyMember>
            {
                new FamilyMember { Id = 1, Name = "Mom", Role = "Mother" },
                new FamilyMember { Id = 2, Name = "Dad", Role = "Father" },
                new FamilyMember { Id = 3, Name = "Lucy", Role = "Daughter" },
                new FamilyMember { Id = 4, Name = "Joe", Role = "Son" }
            };

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));

            _memoryCache.Set(CACHE_KEY, familyMembers, cacheOptions);

            return familyMembers;
        }
        

        return new List<FamilyMember>();
    }

    /// <summary>
    /// Retrieves a specific family member by ID
    /// </summary>
    /// <param name="id">The ID of the family member to retrieve</param>
    /// <returns>The requested family member</returns>
    /// <response code="200">Returns the requested family member</response>
    /// <response code="404">If the family member is not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<FamilyMember> GetById(int id)
    {
        var result = GetAll().Result as OkObjectResult;
        if (result == null)
            return NotFound();

        var familyMembers = result.Value as List<FamilyMember>;
        var familyMember = familyMembers.FirstOrDefault(f => f.Id == id);
        if (familyMember == null)
            return NotFound();

        return Ok(familyMember);
    }

    /// <summary>
    /// Creates a new family member
    /// </summary>
    /// <param name="familyMember">The family member to create</param>
    /// <returns>The created family member</returns>
    /// <response code="201">Returns the newly created family member</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<FamilyMember> Create(FamilyMember familyMember)
    {
        var result = GetAll().Result as OkObjectResult;
        if (result == null)
            return BadRequest();

        var familyMembers = result.Value as List<FamilyMember>;
        familyMember.Id = familyMembers.Count + 1;
        familyMembers.Add(familyMember);
        _memoryCache.Remove(CACHE_KEY); // Invalidate cache
        return CreatedAtAction(nameof(GetById), new { id = familyMember.Id }, familyMember);
    }

    /// <summary>
    /// Updates an existing family member
    /// </summary>
    /// <param name="id">The ID of the family member to update</param>
    /// <param name="familyMember">The updated family member information</param>
    /// <returns>No content</returns>
    /// <response code="204">If the update was successful</response>
    /// <response code="404">If the family member is not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, FamilyMember familyMember)
    {
        var result = GetAll().Result as OkObjectResult;
        if (result == null)
            return NotFound();

        var familyMembers = result.Value as List<FamilyMember>;
        var existing = familyMembers.FirstOrDefault(f => f.Id == id);
        if (existing == null)
            return NotFound();

        existing.Name = familyMember.Name;
        existing.Role = familyMember.Role;

        _memoryCache.Remove(CACHE_KEY); // Invalidate cache
        return NoContent();
    }

    /// <summary>
    /// Deletes a specific family member
    /// </summary>
    /// <param name="id">The ID of the family member to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">If the deletion was successful</response>
    /// <response code="404">If the family member is not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var result = GetAll().Result as OkObjectResult;
        if (result == null)
            return NotFound();

        var familyMembers = result.Value as List<FamilyMember>;
        var familyMember = familyMembers.FirstOrDefault(f => f.Id == id);
        if (familyMember == null)
            return NotFound();

        familyMembers.Remove(familyMember);
        _memoryCache.Remove(CACHE_KEY); // Invalidate cache
        return NoContent();
    }
}
