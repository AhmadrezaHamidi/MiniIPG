
namespace Authentication.Application.Dtos;


public record LoginRequestDto(
   string username, string password
   )
{
    public LoginCommand ToCommand() =>
        new LoginCommand(
            username,
            password?.Trim()
        );

    public static implicit operator LoginCommand(
        LoginRequestDto dto) => dto?.ToCommand();
}

