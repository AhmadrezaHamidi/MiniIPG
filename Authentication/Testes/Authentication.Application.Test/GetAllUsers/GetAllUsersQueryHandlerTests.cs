using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Authentication.Application.AuthorizationQueries;
using Authentication.Application.Dtos;
using Authentication.Application.Services;
using Authentication.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shared;

namespace Authentication.Application.Test.GetAllUsers;

public class GetAllUsersQueryHandlerTests
{
    private readonly IUserService _userService;
    private readonly UserManager<User> _userManager;
    private readonly GetAllUsersQueryHandler _handler;

    public GetAllUsersQueryHandlerTests()
    {
        _userService = Substitute.For<IUserService>();
        _userManager = Substitute.For<UserManager<User>>(
            Substitute.For<IUserStore<User>>(),
            null, null, null, null, null, null, null, null);

        _handler = new GetAllUsersQueryHandler(_userService);
    }


    [Fact]
    public async Task Handle_Should_Return_Success_With_UserList_When_Users_Exist()
    {
        // Arrange
        var expectedUsers = new List<UserDto>
    {
        new UserDto(1, "adminuser", "Admin", "User", "Admin,Manager"),
        new UserDto(2, "normaluser", "Normal", "User", "User")
    };

        _userService
            .GetAllAsync()
            .Returns(Result<List<UserDto>>.Success(expectedUsers));

        var query = new GetAllUsersQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data.Should().BeEquivalentTo(expectedUsers);
    }


    [Fact]
    public async Task GetAllAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        _userService
     .GetAllAsync()
     .Returns(Result<List<UserDto>>.Failure("خطا در دریافت لیست کاربران"));



        var query = new GetAllUsersQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        // Act

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("خطا در دریافت لیست کاربران");
    }

}