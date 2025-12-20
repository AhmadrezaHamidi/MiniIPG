
namespace Authentication.Application.Services;

public interface IUserService
{
    Task<Result<TokenDto>> GetTokenAsync(string username, string password);
    Task<Result<TokenDto>> GetRefreshTokenAsync(string refreshToken);
    Task<Result<UserDto>> GetByIdAsync(int id);
    Task<Result<List<UserDto>>> GetAllAsync();
    Task<Result<int>> CreateAsync(UserCreateDto user, string role);
    Task<Result<bool>> DeleteAsync(int id);
    Task<Result<bool>> UpdateAsync(UserUpdateDto model);
}
