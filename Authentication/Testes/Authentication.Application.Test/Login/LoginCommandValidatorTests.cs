using Authentication.Application.AuthorizationCommands;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Authentication.Application.Test.Login;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Should_Fail_When_Username_Is_Empty()
    {
        var command = new LoginCommand("", "123456");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.username);
    }

    [Fact]
    public void Should_Fail_When_Password_Is_Empty()
    {
        var command = new LoginCommand("test", "");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.password);
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        var command = new LoginCommand("test", "123456");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
