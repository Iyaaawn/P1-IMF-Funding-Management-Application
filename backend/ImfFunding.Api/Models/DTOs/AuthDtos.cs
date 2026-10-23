using System.ComponentModel.DataAnnotations;

namespace ImfFunding.Api.Models.DTOs;

public class RegisterDto
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = "";

    [Required, EmailAddress, StringLength(256)]
    public string Email { get; set; } = "";

    [Required, MinLength(8)]
    public string Password { get; set; } = "";

    public int? CountryId { get; set; }
}

public class LoginDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}

public class AuthResponseDto
{
    public string Token { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
}