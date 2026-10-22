using ImfFunding.Api.Models.DTOs;
using ImfFunding.Api.Models.EF;
using ImfFunding.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImfFunding.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FundingRequestsController : ControllerBase
{
    private readonly IFundingRequestService _service;

    public FundingRequestsController(IFundingRequestService service)
    {
        _service = service;
    }

    // GET api/fundingrequests?countryId=1&status=Submitted   (both filters optional)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FundingRequestDto>>> GetAll(
        [FromQuery] int? countryId, [FromQuery] Status? status)
    {
        return Ok(await _service.GetAllAsync(countryId, status));
    }

    // GET api/fundingrequests/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<FundingRequestDto>> GetById(int id)
    {
        var request = await _service.GetByIdAsync(id);
        if (request == null) return NotFound();
        return request;
    }

    // POST api/fundingrequests
    [HttpPost]
    public async Task<ActionResult<FundingRequestDto>> Create(CreateFundingRequestDto dto)
    {
        var result = await _service.CreateAsync(dto);
        if (!result.Succeeded) return ToErrorResponse(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);   // 201 + Location header
    }

    // PUT api/fundingrequests/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<FundingRequestDto>> Update(int id, UpdateFundingRequestDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (!result.Succeeded) return ToErrorResponse(result);
        return Ok(result.Value);
    }

    // PATCH api/fundingrequests/5/status   body: { "status": "StaffAssessment" }
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<FundingRequestDto>> ChangeStatus(int id, UpdateStatusDto dto)
    {
        var result = await _service.ChangeStatusAsync(id, dto);
        if (!result.Succeeded) return ToErrorResponse(result);
        return Ok(result.Value);
    }

    // DELETE api/fundingrequests/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result.Succeeded) return ToErrorResponse(result);
        return NoContent();
    }

    // Turns a service failure into the right HTTP status code
    private ActionResult ToErrorResponse<T>(ServiceResult<T> result) => result.Error switch
    {
        ServiceErrorType.NotFound   => NotFound(new { message = result.Message }),
        ServiceErrorType.Validation => BadRequest(new { message = result.Message }),
        ServiceErrorType.Conflict   => Conflict(new { message = result.Message }),
        _                           => StatusCode(500)
    };
}