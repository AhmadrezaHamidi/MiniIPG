using System;
using System.Collections.Generic;
namespace Authentication.Application.AuthorizationCommands;


public record DeleteUserCommand(
    int userId
) : IRequest<Result<bool>>
{
    public record Handler(IUserService UserService) : IRequestHandler<DeleteUserCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken ct)
            => await UserService.DeleteAsync(request.userId);
    }
}


