using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ImfFunding.Api.Models.EF;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string FullName { get; set; } = "";

    [Required, StringLength(256)]
    public string Email { get; set; } = "";

    [Required]
    public string PasswordHash { get; set; } = "";     // never the real password

    [Required, StringLength(20)]
    public string Role { get; set; } = Roles.Requester;

    public int? CountryId { get; set; }                 // optional: which country the user represents
    public Country? Country { get; set; }
}