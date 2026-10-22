

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ImfFunding.Api.Models.EF;

[Index(nameof(CodeName), IsUnique = true)]
public class LendingFacility
{
    public int Id{get;set;}

    [Required]
    [StringLength(3)]
    public string CodeName {get;set;} = null!;
    
    [Required]
    [StringLength(50)]
    public string FullName {get;set;} = null!;

    [Required]
    [StringLength(1000)]
    public string Description {get;set;} = null!;
    
    public int MaxTermMonths{get;set;}   

}