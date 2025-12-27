using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Application.Test.UpdateUser
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using global::Authentication.Application.Dtos;
    using global::Authentication.Domain.Entities;
    using global::Authentication.Infrastructure.Manageres;
    using global::Authentication.Infrastructure.Services;
    using Microsoft.AspNetCore.Identity;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;

    namespace Authentication.Application.Tests.UpdateUser
    {
        public class UserServiceUpdateTests
        {
            private readonly AppUserManager _userManager;
            private readonly UserService _userService;

            public UserServiceUpdateTests()
            {
                _userManager = CreateUserManager();
                _userService = new UserService(_userManager, null!, null!);
            }

            private static AppUserManager CreateUserManager()
            {
                var store = Substitute.For<IUserStore<User>>();
                return Substitute.For<AppUserManager>(store, null, null, null, null, null, null, null, null);
            }

            [Fact]
            public async Task UpdateAsync_Should_Return_Success_When_No_Password_Change()
            {
                // Arrange
                var existingUser = new User { Id = 1, Email = "old@test.com", FirstName = "Old", LastName = "User" };
                _userManager.FindByIdAsync("1").Returns(existingUser);
                _userManager.UpdateAsync(existingUser).Returns(IdentityResult.Success);

                var dto = new UserUpdateDto
                {
                    Id = 1,
                    Email = "new@test.com",
                    FirstName = "NewFirst",
                    LastName = "NewLast",
                    Password = null // بدون تغییر رمز
                };

                // Act
                var result = await _userService.UpdateAsync(dto);

                // Assert
                result.IsSuccess.Should().BeTrue();
                existingUser.Email.Should().Be("new@test.com");
                existingUser.FirstName.Should().Be("NewFirst");
                existingUser.LastName.Should().Be("NewLast");
                await _userManager.DidNotReceive().GeneratePasswordResetTokenAsync(Arg.Any<User>());
            }

            [Fact]
            public async Task UpdateAsync_Should_Change_Password_And_SecurityStamp_When_Password_Provided()
            {
                // Arrange
                var user = new User { Id =1 };
                _userManager.FindByIdAsync("1").Returns(user);
                _userManager.GeneratePasswordResetTokenAsync(user).Returns("fake-token");
                _userManager.ResetPasswordAsync(user, "fake-token", "NewPass123")
                    .Returns(IdentityResult.Success);
                _userManager.UpdateAsync(user).Returns(IdentityResult.Success);

                var dto = new UserUpdateDto
                {
                    Id = 1,
                    Email = "same@test.com",
                    FirstName = "Same",
                    LastName = "User",
                    Password = "NewPass123",
                    Respassword = "NewPass123"
                };

                // Act
                var result = await _userService.UpdateAsync(dto);

                // Assert
                result.IsSuccess.Should().BeTrue();
                await _userManager.Received(1).GeneratePasswordResetTokenAsync(user);
                await _userManager.Received(1).ResetPasswordAsync(user, "fake-token", "NewPass123");
                user.SecurityStamp.Should().NotBeNull(); // تغییر کرده
            }

            [Fact]
            public async Task UpdateAsync_Should_Fail_When_User_Not_Found()
            {
                // Arrange
                _userManager.FindByIdAsync("999").Returns((User?)null);

                var dto = new UserUpdateDto { Id = 999 };

                // Act
                var result = await _userService.UpdateAsync(dto);

                // Assert
                result.IsSuccess.Should().BeFalse();
                result.Message.Should().Be("کاربر مورد نظر یافت نشد");
            }

            [Fact]
            public async Task UpdateAsync_Should_Fail_When_ResetPassword_Fails()
            {
                // Arrange
                var user = new User { Id = 1 };
                _userManager.FindByIdAsync("1").Returns(user);
                _userManager.GeneratePasswordResetTokenAsync(user).Returns("token");
                _userManager.ResetPasswordAsync(user, "token", "weak")
                    .Returns(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

                var dto = new UserUpdateDto { Id = 1, Password = "weak" };

                // Act
                var result = await _userService.UpdateAsync(dto);

                // Assert
                result.IsSuccess.Should().BeFalse();
                result.Message.Should().Contain("Password too weak");
                await _userManager.DidNotReceive().UpdateAsync(user); // آپدیت نباید انجام بشه
            }

            [Fact]
            public async Task UpdateAsync_Should_Fail_When_UpdateAsync_Fails()
            {
                // Arrange
                var user = new User { Id = 1 };
                _userManager.FindByIdAsync("1").Returns(user);
                _userManager.UpdateAsync(user)
                    .Returns(IdentityResult.Failed(new IdentityError { Description = "Concurrency failure" }));

                var dto = new UserUpdateDto { Id = 1, FirstName = "New" };

                // Act
                var result = await _userService.UpdateAsync(dto);

                // Assert
                result.IsSuccess.Should().BeFalse();
                result.Message.Should().Contain("Concurrency failure");
            }

            [Fact]
            public async Task UpdateAsync_Should_Return_Failure_On_Exception()
            {
                // Arrange
                _userManager.FindByIdAsync(Arg.Any<string>())
                    .Throws(new Exception("DB connection failed"));

                // Act
                var result = await _userService.UpdateAsync(new UserUpdateDto { Id = 1 });

                // Assert
                result.IsSuccess.Should().BeFalse();
                result.Message.Should().Be("خطا در بروزرسانی کاربر");
            }
        }
    }
}
