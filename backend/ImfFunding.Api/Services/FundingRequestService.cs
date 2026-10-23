using ImfFunding.Api.Data;
using ImfFunding.Api.Models.DTOs;
using ImfFunding.Api.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace ImfFunding.Api.Services;

public class FundingRequestService : IFundingRequestService
{
    // Business rule: a request can't exceed this percent of the country's IMF quota.
    // (Modeled on the IMF's normal annual access limit; approximate.)
    private const decimal AccessLimitPercentOfQuota = 200m;

    // Business rule: the IMF review workflow. Each status lists where it can go next.
    private static readonly Dictionary<Status, Status[]> AllowedTransitions = new()
    {
        [Status.Submitted]       = new[] { Status.StaffAssessment, Status.Rejected },
        [Status.StaffAssessment] = new[] { Status.BoardReview, Status.Rejected },
        [Status.BoardReview]     = new[] { Status.Approved, Status.Rejected },
        [Status.Approved]        = new[] { Status.Disbursing },
        [Status.Disbursing]      = new[] { Status.Completed },
        [Status.Rejected]        = Array.Empty<Status>(),
        [Status.Completed]       = Array.Empty<Status>()
    };

    private readonly AppDbContext _context;

    public FundingRequestService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FundingRequestDto>> GetAllAsync(int? countryId, Status? status)
    {
        IQueryable<FundingRequest> query = _context.FundingRequests
            .AsNoTracking()
            .Include(r => r.Country)
            .Include(r => r.LendingFacility);

        // Optional filters: each Where becomes part of the SQL WHERE clause
        if (countryId.HasValue)
            query = query.Where(r => r.CountryId == countryId.Value);
        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        var requests = await query.OrderByDescending(r => r.SubmittedDate).ToListAsync();
        return requests.Select(ToDto);
    }

    public async Task<FundingRequestDto?> GetByIdAsync(int id)
    {
        var request = await FindWithDetailsAsync(id);
        return request == null ? null : ToDto(request);
    }

    public async Task<ServiceResult<FundingRequestDto>> CreateAsync(CreateFundingRequestDto dto, int requestedById)
    {
        var country = await _context.Countries.FindAsync(dto.CountryId);
        if (country == null)
            return ServiceResult<FundingRequestDto>.Invalid($"Country {dto.CountryId} does not exist.");

        var facility = await _context.LendingFacilities.FindAsync(dto.LendingFacilityId);
        if (facility == null)
            return ServiceResult<FundingRequestDto>.Invalid($"Lending facility {dto.LendingFacilityId} does not exist.");

        var limitError = CheckAccessLimit(country, dto.RequestAmountSdrMillions);
        if (limitError != null)
            return ServiceResult<FundingRequestDto>.Invalid(limitError);

        var request = new FundingRequest
        {
            CountryId = country.Id,
            Country = country,
            LendingFacilityId = facility.Id,
            LendingFacility = facility,
            Title = dto.Title.Trim(),
            Purpose = dto.Purpose.Trim(),
            RequestAmountSdrMillions = dto.RequestAmountSdrMillions,
            Status = Status.Submitted,          // always starts here; the client can't choose
            SubmittedDate = DateTime.UtcNow,    // set by the server, not the client
            LastUpdated = DateTime.UtcNow,
            RequestedById = requestedById
        };

        _context.FundingRequests.Add(request);
        await _context.SaveChangesAsync();
        return ServiceResult<FundingRequestDto>.Ok(ToDto(request));
    }

    public async Task<ServiceResult<FundingRequestDto>> UpdateAsync(int id, UpdateFundingRequestDto dto)
    {
        var request = await FindWithDetailsAsync(id);
        if (request == null)
            return ServiceResult<FundingRequestDto>.NotFound($"Funding request {id} not found.");

        if (request.Status != Status.Submitted)
            return ServiceResult<FundingRequestDto>.Conflict(
                $"Only Submitted requests can be edited. This one is {request.Status}.");

        if (dto.LendingFacilityId != request.LendingFacilityId)
        {
            var facility = await _context.LendingFacilities.FindAsync(dto.LendingFacilityId);
            if (facility == null)
                return ServiceResult<FundingRequestDto>.Invalid($"Lending facility {dto.LendingFacilityId} does not exist.");
            request.LendingFacilityId = facility.Id;
            request.LendingFacility = facility;
        }

        var limitError = CheckAccessLimit(request.Country, dto.RequestAmountSdrMillions);
        if (limitError != null)
            return ServiceResult<FundingRequestDto>.Invalid(limitError);

        request.Title = dto.Title.Trim();
        request.Purpose = dto.Purpose.Trim();
        request.RequestAmountSdrMillions = dto.RequestAmountSdrMillions;
        request.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ServiceResult<FundingRequestDto>.Ok(ToDto(request));
    }

    public async Task<ServiceResult<FundingRequestDto>> ChangeStatusAsync(int id, UpdateStatusDto dto)
    {
        var request = await FindWithDetailsAsync(id);
        if (request == null)
            return ServiceResult<FundingRequestDto>.NotFound($"Funding request {id} not found.");

        var allowed = AllowedTransitions[request.Status];
        if (!allowed.Contains(dto.Status))
        {
            var next = allowed.Length == 0 ? "none (this is a final status)" : string.Join(", ", allowed);
            return ServiceResult<FundingRequestDto>.Conflict(
                $"Can't move a request from {request.Status} to {dto.Status}. Allowed next: {next}.");
        }

        request.Status = dto.Status;
        request.LastUpdated = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return ServiceResult<FundingRequestDto>.Ok(ToDto(request));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var request = await _context.FundingRequests.FindAsync(id);
        if (request == null)
            return ServiceResult<bool>.NotFound($"Funding request {id} not found.");

        if (request.Status != Status.Submitted && request.Status != Status.Rejected)
            return ServiceResult<bool>.Conflict(
                "Requests that have moved into review are part of the official record and can't be deleted.");

        _context.FundingRequests.Remove(request);
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    // ---------- private helpers ----------

    private Task<FundingRequest?> FindWithDetailsAsync(int id) =>
        _context.FundingRequests
            .Include(r => r.Country)
            .Include(r => r.LendingFacility)
            .FirstOrDefaultAsync(r => r.Id == id);

    private static string? CheckAccessLimit(Country country, decimal amount)
    {
        var limit = country.QuotaAmountSdrMillions * AccessLimitPercentOfQuota / 100m;
        return amount > limit
            ? $"{country.Name} can request at most {limit:N1} million SDR ({AccessLimitPercentOfQuota}% of its quota)."
            : null;
    }

    private static FundingRequestDto ToDto(FundingRequest r) => new()
    {
        Id = r.Id,
        CountryId = r.CountryId,
        CountryName = r.Country.Name,
        LendingFacilityId = r.LendingFacilityId,
        FacilityCode = r.LendingFacility.CodeName,
        FacilityName = r.LendingFacility.FullName,
        Title = r.Title,
        Purpose = r.Purpose,
        RequestAmountSdrMillions = r.RequestAmountSdrMillions,
        Status = r.Status,
        SubmittedDate = r.SubmittedDate,
        LastUpdated = r.LastUpdated
    };
}