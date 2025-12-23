using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Authentication.Application.AuthorizationCommands;
using Authentication.Application.Dtos;
using Authentication.Application.Services;
using Authentication.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using Shared;

namespace Authentication.Application.Test.RefreshToken;


public class RefreshTokenHandlerTests
{
    [Fact]
    public async Task Should_Return_Success_When_RefreshToken_Is_Valid()
    {
        var userService = Substitute.For<IUserService>();

        var existingUser = new User
        {
            Id = 1,
            UserName = "testuser",
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            RefreshToken = "valid-refresh-token",  
            CreatedAt = DateTime.UtcNow
        };

        var newToken = new TokenDto(
            AccessToken: "new-access-token",
            RefreshToken: "new-refresh-token",
            UserId: 1,
            Email: "test@test.com",
            Roles: "User"
        );

        userService
            .GetRefreshTokenAsync("valid-refresh-token")
            .Returns(Task.FromResult(Result<TokenDto>.Success(newToken)));

        var handler = new RefreshTokenCommand.Handler(userService);

        var command = new RefreshTokenCommand("valid-refresh-token");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.AccessToken.Should().Be("new-access-token");
        result.Data.RefreshToken.Should().Be("new-refresh-token");
        result.Data.UserId.Should().Be(1);
        result.Data.Email.Should().Be("test@test.com");
        result.Data.Roles.Should().Be("User");

        // Service metodu doğru parametr ilə çağırılıb mı?
        await userService
            .Received(1)
            .GetRefreshTokenAsync("valid-refresh-token");
    }

    [Fact]
    public async Task Should_Return_Failure_When_RefreshToken_Is_Invalid()
    {
        // Arrange
        var userService = Substitute.For<IUserService>();

        userService
            .GetRefreshTokenAsync("invalid-refresh-token")
            .Returns(Task.FromResult(Result<TokenDto>.Failure("توکن نامعتبر است")));

        var handler = new RefreshTokenCommand.Handler(userService);
        var command = new RefreshTokenCommand("invalid-refresh-token");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("توکن نامعتبر است");

        await userService
            .Received(1)
            .GetRefreshTokenAsync("invalid-refresh-token");
    }
}
