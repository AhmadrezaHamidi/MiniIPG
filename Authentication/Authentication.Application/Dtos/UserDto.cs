namespace Authentication.Application.Dtos;

public record UserDto(int Id, string Username , string FirstName, string LastName, string roles )
{
}
