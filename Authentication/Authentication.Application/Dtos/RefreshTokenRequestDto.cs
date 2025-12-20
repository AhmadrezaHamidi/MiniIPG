namespace Authentication.Application.Dtos;


public record RefreshTokenRequestDto(string RefreshToken)
{
    public RefreshTokenCommand ToCommand() => new(RefreshToken);
}
