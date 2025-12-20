namespace Authentication.Infrastructure.Services;

public interface ITokenInfoService
{
    Task<TokenDto> GenerateToken(User user);
    long GetCurrentUserId();
    TokenValidationParameters GetValidationParameters();
}
