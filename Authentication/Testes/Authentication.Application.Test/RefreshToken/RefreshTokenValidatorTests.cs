using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Authentication.Application.AuthorizationCommands;
using FluentValidation.TestHelper;

namespace Authentication.Application.Test.RefreshToken;

public class RefreshTokenValidatorTests
{
    private readonly RefreshTokenCommandValidator _validator = new();

    [Fact]
    public void Should_Fail_When_Refresh_Is_Empty()
    {
        var command = new RefreshTokenCommand("");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }

    [Fact]
    public void Should_Fail_When_Refresh_Is_Null()
    {
        var command = new RefreshTokenCommand(string.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        var command = new RefreshTokenCommand("sdfsdffsdfsdfsdfsd");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

}
