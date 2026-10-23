//the contract. 
// The controller depends on this interface, not the class.
// That’s the D in SOLID.

using ImfFunding.Api.Models.DTOs;
using ImfFunding.Api.Models.EF;

namespace ImfFunding.Api.Services;

public interface IFundingRequestService
{
    Task<IEnumerable<FundingRequestDto>> GetAllAsync(int? countryId, Status? status);
    Task<FundingRequestDto?> GetByIdAsync(int id);
    Task<ServiceResult<FundingRequestDto>> CreateAsync(CreateFundingRequestDto dto, int requestedById);
    Task<ServiceResult<FundingRequestDto>> UpdateAsync(int id, UpdateFundingRequestDto dto);
    Task<ServiceResult<FundingRequestDto>> ChangeStatusAsync(int id, UpdateStatusDto dto);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}