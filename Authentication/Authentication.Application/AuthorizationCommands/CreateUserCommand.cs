
using FluentValidation;

namespace Authentication.Application.AuthorizationCommands;

public record CreateUserCommand(
        UserCreateDto user,
        string role
    ) : IRequest<Result<int>>
{
    public record Handler(IUserService UserService) : IRequestHandler<CreateUserCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(CreateUserCommand request, CancellationToken ct)
        => await UserService.CreateAsync(request.user, request.role);
    }
}

