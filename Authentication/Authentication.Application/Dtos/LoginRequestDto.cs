using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


internal class LoginRequestDto
{
}
