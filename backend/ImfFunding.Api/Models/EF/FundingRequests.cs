
using System;
using System.ComponentModel.DataAnnotations;
using ImfFunding.Api.Models.EF;
using Microsoft.EntityFrameworkCore;

namespace ImfFunding.Api.Models.EF;

public class FundingRequest
{
    public int Id {get;set;}

    //Foreign Keys
    public int CountryId {get;set;}
    public int LendingFacilityId {get;set;}
    public int? RequestedById {get;set;}

    [Required]
    [StringLength(100)]
    public string Title {get;set;} = null!;

    [Required]
    [StringLength(100)]
    public string Purpose{get;set;} = null!;

    [Precision(18,2)]
    [Range(0.01, 1_000_000)]
    public decimal RequestAmountSdrMillions { get; set; }
    
    public Status Status{get;set;} = Status.Submitted;

    public DateTime SubmittedDate{get;set;}
    public DateTime LastUpdated{get;set;}



    public Country Country{get;set;} = null!;
    public LendingFacility LendingFacility{get;set;} = null!;

    // public virtual User? RequestedById{get;set;}

}
