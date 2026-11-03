//the contract. 
// The controller depends on this interface, not the class.
// That’s the D in SOLID.

using ImfFunding.Api.Models.DTOs;
using ImfFunding.Api.Models.EF;

namespace ImfFunding.Api.Services;

public interface IFundingRequestService
{
    Task<IEnumerable<FundingRequestDto>> GetAllAsync(int? countryId, Status? status, int userId, bool isAdmin);
    Task<FundingRequestDto?> GetByIdAsync(int id, int userId, bool isAdmin);
    Task<ServiceResult<FundingRequestDto>> UpdateAsync(int id, UpdateFundingRequestDto dto, int userId, bool isAdmin);
    Task<ServiceResult<bool>> DeleteAsync(int id, int userId, bool isAdmin);
    Task<ServiceResult<FundingRequestDto>> ChangeStatusAsync(int id, UpdateStatusDto dto);
    Task<ServiceResult<FundingRequestDto>> CreateAsync(CreateFundingRequestDto dto, int requestedById);
}