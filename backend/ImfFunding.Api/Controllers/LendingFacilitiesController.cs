

using ImfFunding.Api.Data;
using ImfFunding.Api.Models.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImfFunding.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LendingFacilitiesController : ControllerBase
{
    private readonly AppDbContext _context;

    public LendingFacilitiesController( AppDbContext context)
    {
        _context = context;
    }

    // GET: all available lending facilites
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LendingFacility>>> GetFacilities()
    {
        return await _context.LendingFacilities.OrderBy(f => f.FullName).ToListAsync();
    }

    // GET: one specific lending facility
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LendingFacility>> GetFacility(int id)
    {
        var facility = await _context.LendingFacilities.FindAsync(id);

        if (facility == null)
        {
            return NotFound();
        }

        return facility;
    }
}