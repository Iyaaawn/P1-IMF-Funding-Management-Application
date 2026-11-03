using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ImfFunding.Api.Models.EF;

[Index(nameof(IsoCode), IsUnique = true)]
public class Country
{
    
    public int Id {get; set;}

    [Required]
    [StringLength(50)]
    public string Name {get; set;} = null!;

    [Required]
    [StringLength(3)]
    public string IsoCode {get; set;} = null!;

    [Precision(18,2)]
    [Range(0.01, 1_000_000)]
    public decimal QuotaAmountSdrMillions { get; set; }


}