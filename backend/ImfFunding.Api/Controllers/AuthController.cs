using ImfFunding.Api.Data;
using ImfFunding.Api.Models.DTOs;
using ImfFunding.Api.Models.EF;
using ImfFunding.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImfFunding.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthController(AppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    // POST api/auth/register
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        if (await _context.Users.AnyAsync(u => u.Email == email))
            return Conflict(new { message = "An account with that email already exists." });

        if (dto.CountryId.HasValue && !await _context.Countries.AnyAsync(c => c.Id == dto.CountryId))
            return BadRequest(new { message = $"Country {dto.CountryId} does not exist." });

        var user = new User
        {
            FullName = dto.FullName.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = Roles.Requester,          // nobody can register themselves as Admin
            CountryId = dto.CountryId
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(BuildResponse(user));
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLower();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        // Same message whether the email or the password is wrong,
        // so attackers can't use login to find out which emails have accounts.
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password." });

        return Ok(BuildResponse(user));
    }

    private AuthResponseDto BuildResponse(User user)
    {
        var (token, expiresAt) = _tokenService.CreateToken(user);
        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role
        };
    }
}