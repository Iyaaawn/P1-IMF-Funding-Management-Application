using System.ComponentModel.DataAnnotations;
using ImfFunding.Api.Models.EF;

namespace ImfFunding.Api.Models.DTOs;

// What the API SENDS back. Flat, with names instead of nested objects (no JSON cycles).
public class FundingRequestDto
{
    public int Id { get; set; }
    public int CountryId { get; set; }
    public string CountryName { get; set; } = "";
    public int LendingFacilityId { get; set; }
    public string FacilityCode { get; set; } = "";
    public string FacilityName { get; set; } = "";
    public string Title { get; set; } = "";
    public string Purpose { get; set; } = "";
    public decimal RequestAmountSdrMillions { get; set; }
    public Status Status { get; set; }
    public DateTime SubmittedDate { get; set; }
    public DateTime LastUpdated { get; set; }
}

// What the caller SENDS to create one. No Id, Status, or dates: the server controls those.
public class CreateFundingRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "CountryId is required.")]
    public int CountryId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "LendingFacilityId is required.")]
    public int LendingFacilityId { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = "";

    [Required, StringLength(2000)]
    public string Purpose { get; set; } = "";

    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal RequestAmountSdrMillions { get; set; }
}

// Editing a request. The country can't change after submission.
public class UpdateFundingRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "LendingFacilityId is required.")]
    public int LendingFacilityId { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = "";

    [Required, StringLength(2000)]
    public string Purpose { get; set; } = "";

    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal RequestAmountSdrMillions { get; set; }
}

// Moving a request through the workflow (admin action).
public class UpdateStatusDto
{
    [EnumDataType(typeof(Status))]
    public Status Status { get; set; }
}