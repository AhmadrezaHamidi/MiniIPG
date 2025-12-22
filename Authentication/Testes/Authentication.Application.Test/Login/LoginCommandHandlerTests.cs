using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Authentication.Application.AuthorizationCommands;
using Authentication.Application.Dtos;
using Authentication.Application.Services;
using FluentAssertions;
using NSubstitute;
using Shared;

namespace Authentication.Application.Test.Login;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Should_Return_Success_When_Login_Is_Valid()
    {
        // Arrange
        var userService = Substitute.For<IUserService>();

        var token = new TokenDto(
            AccessToken: "token",
            RefreshToken: "refresh-token",
            UserId: 1,
            Email: "test@test.com",
            Roles: "User"
        );

        userService
            .GetTokenAsync("test", "123456")
            .Returns(Task.FromResult(Result<TokenDto>.Success(token)));

        var handler = new LoginCommand.Handler(userService);
        var command = new LoginCommand("test", "123456");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Return_Failure_When_Login_Is_Invalid()
    {
        // Arrange
        var userService = Substitute.For<IUserService>();

        userService
            .GetTokenAsync("test", "wrong-pass")
            .Returns(Result<TokenDto>.Failure("نام کاربری یا رمز عبور اشتباه است"));

        var handler = new LoginCommand.Handler(userService);
        var command = new LoginCommand("test", "wrong-pass");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("نام کاربری یا رمز عبور اشتباه است");
    }



    [Fact]
    public async Task Should_Call_UserService_With_Correct_Parameters()
    {
        // Arrange
        var userService = Substitute.For<IUserService>();

        userService
            .GetTokenAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Result<TokenDto>.Failure("error"));

        var handler = new LoginCommand.Handler(userService);
        var command = new LoginCommand("test", "123456");

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await userService.Received(1)
            .GetTokenAsync("test", "123456");
    }
}
