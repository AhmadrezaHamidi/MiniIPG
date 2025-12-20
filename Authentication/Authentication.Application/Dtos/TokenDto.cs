
namespace Authentication.Application.Dtos;

public  record TokenDto(string AccessToken ,string RefreshToken, int UserId , string Email , string Roles)
{
}
