
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

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.username)
            .NotEmpty().WithMessage("نام کاربری الزامی است")
            .MaximumLength(100);

        RuleFor(x => x.password)
            .NotEmpty().WithMessage("رمز عبور الزامی است")
            .MinimumLength(6).WithMessage("رمز عبور حداقل باید ۶ کاراکتر باشد")
            .MaximumLength(100);
    }
}