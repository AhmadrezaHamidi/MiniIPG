using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Authentication.Application.Services;
using Authentication.Domain.Entities;
using Authentication.Infrastructure.Manageres;
using Authentication.Infrastructure.Services;
using Duende.IdentityServer.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Authentication.Application.Test.GetUserByIdz;

public class UserServiceGetByIdTests
{
    private readonly AppUserManager _userManager;
    private readonly AppSignInManager _signInManager;
    private readonly ITokenInfoService _tokenInfoService;
    private readonly UserService _userService;


    private static AppUserManager CreateAppUserManager()
    {
        var store = Substitute.For<IUserStore<User>>();

        return Substitute.For<AppUserManager>(
            store,
            null, // IOptions<IdentityOptions>
            null, // IPasswordHasher<User>
            null, // IEnumerable<IUserValidator<User>>
            null, // IEnumerable<IPasswordValidator<User>>
            null, // ILookupNormalizer
            null, // IdentityErrorDescriber
            null, // IServiceProvider
            null  // ILogger<UserManager<User>>
        );
    }
    private static AppSignInManager CreateSignInManager(UserManager<User> userManager)
    {
        return Substitute.For<AppSignInManager>(
            userManager,
            Substitute.For<IHttpContextAccessor>(),
            Substitute.For<IUserClaimsPrincipalFactory<User>>(),
            Substitute.For<IOptions<IdentityOptions>>(),
            Substitute.For<ILogger<SignInManager<User>>>(),
            Substitute.For<IAuthenticationSchemeProvider>(),
            Substitute.For<IUserConfirmation<User>>()
        );
    }
    private static ITokenInfoService CreateTokenInfoService()
    {
        return Substitute.For<ITokenInfoService>();
    }
    public UserServiceGetByIdTests()
    {
        _userManager = CreateAppUserManager();
        _signInManager = CreateSignInManager(_userManager);
        _tokenInfoService = CreateTokenInfoService();

        _userService = new UserService(
            _userManager,
            _signInManager,
            _tokenInfoService
        );
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Success_With_User_When_User_Exists()
    {
        // Arrange
        var userId = 1;
        var user = new User
        {
            Id = userId,
            UserName = "testuser",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };

        var roles = new List<string> { "Admin", "User" };

        _userManager.FindByIdAsync(userId.ToString())
            .Returns(Task.FromResult(user));

        _userManager.GetRolesAsync(user)
            .Returns(Task.FromResult<IList<string>>(roles));

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(userId);
        result.Data.Username.Should().Be("testuser");
        result.Data.FirstName.Should().Be("John");
        result.Data.LastName.Should().Be("Doe");
        result.Data.roles.Should().Be("Admin,User");

        await _userManager.Received(1).FindByIdAsync(userId.ToString());
        await _userManager.Received(1).GetRolesAsync(user);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Failure_When_User_Not_Found()
    {
        // Arrange
        var userId = 999;

        _userManager.FindByIdAsync(userId.ToString())
            .Returns(Task.FromResult((User)null));

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("کاربر مورد نظر یافت نشد");
        result.Data.Should().BeNull();

        await _userManager.Received(1).FindByIdAsync(userId.ToString());
        await _userManager.DidNotReceive().GetRolesAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Failure_When_FindByIdAsync_Throws_Exception()
    {
        // Arrange
        var userId = 1;

        _userManager.FindByIdAsync(userId.ToString())
            .Throws(new Exception("Database connection failed"));

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("خطا در دریافت اطلاعات کاربر");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Failure_When_GetRolesAsync_Throws_Exception()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId };

        _userManager.FindByIdAsync(userId.ToString())
            .Returns(Task.FromResult(user));

        _userManager.GetRolesAsync(user)
            .Throws(new Exception("Role service unavailable"));

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("خطا در دریافت اطلاعات کاربر");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_User_With_Empty_Roles_When_No_Roles_Assigned()
    {
        // Arrange
        var userId = 1;
        var user = new User
        {
            Id = userId,
            UserName = "noroles",
            FirstName = "No",
            LastName = "Roles"
        };

        var emptyRoles = new List<string>();

        _userManager.FindByIdAsync(userId.ToString())
            .Returns(Task.FromResult(user));

        _userManager.GetRolesAsync(user)
            .Returns(Task.FromResult<IList<string>>(emptyRoles));

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.roles.Should().BeEmpty();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public async Task GetByIdAsync_Should_Call_FindByIdAsync_With_Correct_String_Id(int userId)
    {
        // Arrange
        var user = new User { Id = userId };
        var roles = new List<string> { "User" };

        _userManager.FindByIdAsync(userId.ToString())
            .Returns(Task.FromResult(user));

        _userManager.GetRolesAsync(user)
            .Returns(Task.FromResult<IList<string>>(roles));

        // Act
        await _userService.GetByIdAsync(userId);

        // Assert
        await _userManager.Received(1).FindByIdAsync(userId.ToString());
    }
}
