using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Authentication.Application.Dtos;
using Authentication.Domain.Entities;
using Authentication.Infrastructure.Manageres;
using Authentication.Infrastructure.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Authentication.Application.Test.CreateUser
{
    public class UserServiceCreateTests
    {
        private readonly AppUserManager _userManager;
        private readonly UserService _userService;

        public UserServiceCreateTests()
        {
            _userManager = CreateUserManager();

            _userService = new UserService(
                _userManager,
                null!, // AppSignInManager در CreateAsync استفاده نمی‌شود
                null!  // ITokenInfoService در CreateAsync استفاده نمی‌شود
            );
        }

        private static AppUserManager CreateUserManager()
        {
            var store = Substitute.For<IUserStore<User>>();

            return Substitute.For<AppUserManager>(
                store,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Success_When_User_Created_And_Role_Assigned()
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

            _userManager.CreateAsync(Arg.Any<User>(), dto.Password)
                .Returns(IdentityResult.Success);

            _userManager.AddToRoleAsync(Arg.Any<User>(), "user")
                .Returns(IdentityResult.Success);

            // Act
            var result = await _userService.CreateAsync(dto, "user");

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task CreateAsync_Should_Fail_When_CreateUser_Fails()
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

            _userManager.CreateAsync(Arg.Any<User>(), Arg.Any<string>())
                .Returns(IdentityResult.Failed(
                    new IdentityError { Description = "Create failed" }
                ));

            // Act
            var result = await _userService.CreateAsync(dto, "user");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("Create failed");
        }

        [Fact]
        public async Task CreateAsync_Should_Delete_User_When_AddToRole_Fails()
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

            _userManager.CreateAsync(Arg.Any<User>(), Arg.Any<string>())
                .Returns(IdentityResult.Success);

            _userManager.AddToRoleAsync(Arg.Any<User>(), "user")
                .Returns(IdentityResult.Failed(
                    new IdentityError { Description = "Role failed" }
                ));

            _userManager.DeleteAsync(Arg.Any<User>())
                .Returns(IdentityResult.Success);

            // Act
            var result = await _userService.CreateAsync(dto, "user");

            // Assert
            result.IsSuccess.Should().BeFalse();
            await _userManager.Received(1)
                .DeleteAsync(Arg.Any<User>());
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Failure_When_Exception_Occurs()
        {
            // Arrange
            _userManager.CreateAsync(Arg.Any<User>(), Arg.Any<string>())
                .Throws(new Exception("Database error"));

            // Act
            var result = await _userService.CreateAsync(new UserCreateDto(), "user");

            // Assert
            result.IsSuccess.Should().BeFalse();
        }
    }
}