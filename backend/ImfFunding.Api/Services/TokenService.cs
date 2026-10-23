using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ImfFunding.Api.Models.EF;
using Microsoft.IdentityModel.Tokens;

namespace ImfFunding.Api.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public (string Token, DateTime ExpiresAt) CreateToken(User user)
    {
        // Claims = facts about the user, stored in the token's payload (readable, NOT secret)
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),          // "subject" = user id
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("name", user.FullName),
            new("role", user.Role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())    // unique token id
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:ExpiryMinutes"));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: credentials);   // the SIGNATURE is what makes it tamper-proof

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}