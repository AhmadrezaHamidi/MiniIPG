

namespace Authentication.Application.AuthorizationCommands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<TokenDto>>
{
    public string RefreshToken { get; init; } = RefreshToken;

    public record Handler(IUserService UserService) : IRequestHandler<RefreshTokenCommand, Result<TokenDto>>
    {
        public async Task<Result<TokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
            => await UserService.GetRefreshTokenAsync(request.RefreshToken);
    }
}