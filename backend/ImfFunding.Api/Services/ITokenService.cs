using ImfFunding.Api.Models.EF;

namespace ImfFunding.Api.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(User user);
}