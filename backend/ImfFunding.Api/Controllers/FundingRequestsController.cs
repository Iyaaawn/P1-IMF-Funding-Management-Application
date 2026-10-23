using ImfFunding.Api.Models.DTOs;
using ImfFunding.Api.Models.EF;
using ImfFunding.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImfFunding.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FundingRequestsController : ControllerBase
{
    private readonly IFundingRequestService _service;

    public FundingRequestsController(IFundingRequestService service)
    {
        _service = service;
    }

#region GET api/fundingrequests?countryId=1&status=Submitted   (both filters optional)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FundingRequestDto>>> GetAll(
        [FromQuery] int? countryId, [FromQuery] Status? status)
    {
        return Ok(await _service.GetAllAsync(countryId, status));
    }
#endregion

#region GET api/fundingrequests/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<FundingRequestDto>> GetById(int id)
    {
        var request = await _service.GetByIdAsync(id);
        if (request == null) return NotFound();
        return request;
    }
#endregion

#region POST api/fundingrequests
    [HttpPost]
    public async Task<ActionResult<FundingRequestDto>> Create(CreateFundingRequestDto dto)
    {
        var userId = int.Parse(User.FindFirst("sub")!.Value);
        var result = await _service.CreateAsync(dto, userId);
        if (!result.Succeeded) return ToErrorResponse(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
}
#endregion

#region PUT api/fundingrequests/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<FundingRequestDto>> Update(int id, UpdateFundingRequestDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (!result.Succeeded) return ToErrorResponse(result);
        return Ok(result.Value);
    }
#endregion

#region PATCH api/fundingrequests/5/status   body: { "status": "StaffAssessment" }
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<FundingRequestDto>> ChangeStatus(int id, UpdateStatusDto dto)
    {
        var result = await _service.ChangeStatusAsync(id, dto);
        if (!result.Succeeded) return ToErrorResponse(result);
        return Ok(result.Value);
    }
#endregion

#region DELETE api/fundingrequests/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result.Succeeded) return ToErrorResponse(result);
        return NoContent();
    }
#endregion

    // Turns a service failure into the right HTTP status code
    private ActionResult ToErrorResponse<T>(ServiceResult<T> result) => result.Error switch
    {
        ServiceErrorType.NotFound   => NotFound(new { message = result.Message }),
        ServiceErrorType.Validation => BadRequest(new { message = result.Message }),
        ServiceErrorType.Conflict   => Conflict(new { message = result.Message }),
        _                           => StatusCode(500)
    };
}