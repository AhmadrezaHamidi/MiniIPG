using System.Threading;
using System.Threading.Tasks;
using Authentication.Application.AuthorizationCommands;
using Authentication.Application.Dtos;
using Authentication.Application.Services;
using FluentAssertions;
using NSubstitute;
using Shared;
using Xunit;

namespace Authentication.Application.Tests.UpdateUser
{
	public class UpdateUserCommandHandlerTests
	{
		private readonly IUserService _userService;
		private readonly UpdateUserCommand.Handler _handler;

		public UpdateUserCommandHandlerTests()
		{
			_userService = Substitute.For<IUserService>();
			_handler = new UpdateUserCommand.Handler(_userService);
		}

		[Fact]
		public async Task Handle_Should_Return_Success_When_Service_Returns_Success()
		{
			// Arrange
			var dto = new UserUpdateDto
			{
				Id = 1,
				Email = "updated@test.com",
				FirstName = "UpdatedFirst",
				LastName = "UpdatedLast"
			};

			var command = new UpdateUserCommand(dto);

			_userService.UpdateAsync(dto)
				.Returns(Result<bool>.Success(true));

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			result.IsSuccess.Should().BeTrue();
			result.Data.Should().BeTrue();
		}

		[Fact]
		public async Task Handle_Should_Return_Failure_When_Service_Returns_Failure()
		{
			// Arrange
			var dto = new UserUpdateDto { Id = 1 };
			var command = new UpdateUserCommand(dto);

			_userService.UpdateAsync(dto)
				.Returns(Result<bool>.Failure("کاربر مورد نظر یافت نشد"));

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			result.IsSuccess.Should().BeFalse();
			result.Message.Should().Contain("کاربر مورد نظر یافت نشد");
		}
	}
}