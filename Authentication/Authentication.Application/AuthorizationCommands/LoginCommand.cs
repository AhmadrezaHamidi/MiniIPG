

using FluentValidation;

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
