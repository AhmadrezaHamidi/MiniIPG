using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Application.Test.GetUserById;

using Authentication.Application.AuthorizationQueries;
using Authentication.Application.Dtos;
using Authentication.Application.Services;
using FluentAssertions;
using NSubstitute;
using Shared;
using Xunit;

public class GetByIdQueryHandlerTests
{
    private readonly IUserService _userService;
    private readonly GetByIdQueryHandler _handler;

    public GetByIdQueryHandlerTests()
    {
        _userService = Substitute.For<IUserService>();
        _handler = new GetByIdQueryHandler(_userService);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_User_Exists()
    {
        // Arrange
        var userId = 1;
        var expectedUser = new UserDto(1, "testuser", "John", "Doe", "Admin,User");

        _userService.GetByIdAsync(userId)
            .Returns(Result<UserDto>.Success(expectedUser));

        var query = new GetByIdQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEquivalentTo(expectedUser);
        result.Data.Id.Should().Be(userId);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_User_Not_Found()
    {
        // Arrange
        var userId = 999;

        _userService.GetByIdAsync(userId)
            .Returns(Result<UserDto>.Failure("کاربر مورد نظر یافت نشد"));

        var query = new GetByIdQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("کاربر مورد نظر یافت نشد");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var userId = 1;

        _userService.GetByIdAsync(userId)
            .Returns(Result<UserDto>.Failure("خطا در دریافت اطلاعات کاربر"));

        var query = new GetByIdQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("خطا در دریافت اطلاعات کاربر");
    }

    [Fact]
    public async Task Handle_Should_Call_UserService_With_Correct_Id()
    {
        // Arrange
        var userId = 5;
        var expectedUser = new UserDto(5, "user5", "Test", "User", "User");

        _userService.GetByIdAsync(userId)
            .Returns(Result<UserDto>.Success(expectedUser));

        var query = new GetByIdQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _userService.Received(1).GetByIdAsync(userId);
        result.IsSuccess.Should().BeTrue();
    }
}
