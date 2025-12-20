

namespace Authentication.Application.AuthorizationCommands;

public record LoginCommand(string username, string password) : IRequest<Result<TokenDto>>
{
    public string username { get; init; } = username;
    public string password { get; init; } = password;

    public record Handler(IUserService UserService) : IRequestHandler<LoginCommand, Result<TokenDto>>
    {
        public async Task<Result<TokenDto>> Handle(LoginCommand request, CancellationToken cancellationToken) 
            => await UserService.GetTokenAsync(request.username, request.password);
    }
}
