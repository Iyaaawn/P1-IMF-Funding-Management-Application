
using System.Net.Http.Headers;
using ImfFunding.Api.Data;
using ImfFunding.Api.Models.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; //

namespace ImfFunding.Api.Controllers;


/*
    ApiController vs MVC-Controller


*/
[ApiController]
[Route("api/[controller]")]
public class CountriesController : ControllerBase
{

    /*
        Dependency injection is when a class receives the objects it depends on
        from the outside, usually through its constructor, instead of creating them itself.
        
        This is an example of dependency injection because CountriesController
        receives its DbContext from ASP.NET Core's builder.Services that you specified in Program.cs
        rather than creating it with "new".
    */ 
    private readonly AppDbContext _context; 
    //we use readonly because once _contexts value is declared at runtime, we dont it to changed or declared.

    //Contrsuctor Injection?
    public CountriesController( AppDbContext context)
    {
        _context = context; // FYI: the lifetime of this context ends when the HTTP Request is fufilled. 
    }

    // GET: api/Countries
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
    {
        return await _context.Countries.ToListAsync();
    }

    // GET: api/Countries/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Country>> GetCountry(int id)
    {
        
        var country = await _context.Countries.FindAsync(id);

        if (country == null)
        {
            return NotFound(); //Http Status Code: 404
        }

        return country;
    }

    // POST 
    [HttpPost]
    public async Task<ActionResult<Country>> PostCountry(Country country)
    {
        // 1) Check first: reject duplicates before touching the database
        if (await CountryExists(country.IsoCode))
        {
            return Conflict("A country with this ISO code already exists.");   // 409
        }

        // 2) Add and save
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();

        // 3) 201, with a Location header pointing at GET api/countries/{id}
        return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, country);
    }

    // PUT: api/countries/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCountry(int id, Country country)
    {
        // The id in the URL and the id in the body must agree
        if (id != country.Id)
        {
            return BadRequest("The id in the URL does not match the id in the body.");   // 400
        }

        var existing = await _context.Countries.FindAsync(id);
        if (existing == null)
        {
            return NotFound("This Country doesn't exist.");                                   // 404
        }

        // If the ISO code is changing, make sure no *other* country already uses it
        bool isoTaken = await _context.Countries
            .AnyAsync(c => c.IsoCode == country.IsoCode && c.Id != id);
        if (isoTaken)
        {
            return Conflict("Another country already uses this ISO code.");    // 409
        }

        // Copy the new values onto the tracked entity; EF detects what changed
        existing.Name = country.Name;
        existing.IsoCode = country.IsoCode;
        existing.QuotaAmountSdrMillions = country.QuotaAmountSdrMillions;

        await _context.SaveChangesAsync();                       // EF generates an UPDATE

        return NoContent();                                      // 204
    }

    // DELETE: api/countries/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        var country = await _context.Countries.FindAsync(id);
        if (country == null)
        {
            return NotFound();                                   // 404
        }

        // Don't allow deleting a country that still has funding requests
        bool hasRequests = await _context.FundingRequests.AnyAsync(r => r.CountryId == id);
        if (hasRequests)
        {
            return Conflict("This country has funding requests and cannot be deleted.");   // 409
        }

        _context.Countries.Remove(country);
        await _context.SaveChangesAsync();                       // EF generates a DELETE

        return NoContent();                                      // 204
    }

    private async Task<bool> CountryExists(string isoCode)
    {
        
        return await _context.Countries.AnyAsync(e => e.IsoCode == isoCode);
    }
}
