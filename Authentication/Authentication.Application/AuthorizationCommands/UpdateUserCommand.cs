
namespace Authentication.Application.AuthorizationCommands;

public record UpdateUserCommand(
    UserUpdateDto request
) : IRequest<Result<bool>>
{
    public record Handler(IUserService UserService) : IRequestHandler<UpdateUserCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateUserCommand request, CancellationToken ct)
            =>   await UserService.UpdateAsync(request.request);
    }
}
