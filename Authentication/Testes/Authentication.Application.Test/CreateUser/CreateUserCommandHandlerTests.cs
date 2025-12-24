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

namespace Authentication.Application.Test.CreateUser;

public class CreateUserCommandHandlerTests
{
    private readonly IUserService _userService;
    private readonly CreateUserCommand.Handler _handler;

    public CreateUserCommandHandlerTests()
    {
        _userService = Substitute.For<IUserService>();
        _handler = new CreateUserCommand.Handler(_userService);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Service_Returns_Success()
    {
        // Arrange
        var dto = new UserCreateDto(
            "testuser",
            "test@test.com",
            "Test",
            "User",
            "123456",
            "123456"
        );

        var command = new CreateUserCommand(dto, "user");

        _userService.CreateAsync(dto, "user")
            .Returns(Result<int>.Success(1));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(1);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Service_Fails()
    {
        // Arrange
        var dto = new UserCreateDto();
        var command = new CreateUserCommand(dto, "user");

        _userService.CreateAsync(dto, "user")
            .Returns(Result<int>.Failure("error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}