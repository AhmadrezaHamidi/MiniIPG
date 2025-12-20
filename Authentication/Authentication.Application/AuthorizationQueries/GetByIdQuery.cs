
namespace Authentication.Application.AuthorizationQueries;
public record GetByIdQuery(int userId) : IRequest<Result<UserDto>>;

public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, Result<UserDto>>
{
    private readonly IUserService _userService;

    public GetByIdQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<UserDto>> Handle(GetByIdQuery request, CancellationToken ct)
        => await _userService.GetByIdAsync(request.userId);
}
