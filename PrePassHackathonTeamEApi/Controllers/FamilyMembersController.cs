using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PrePassHackathonTeamEApi.Models;

namespace PrePassHackathonTeamEApi.Controllers;

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

    [HttpGet]
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
    [HttpGet("{id}")]
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

    [HttpPost]
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

    [HttpPut("{id}")]
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

    [HttpDelete("{id}")]
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
