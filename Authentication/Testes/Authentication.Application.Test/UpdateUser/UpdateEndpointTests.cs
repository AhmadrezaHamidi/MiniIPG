using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Application.Test.UpdateUser
{
    using System.Net;
    using System.Net.Http.Json;
    using FluentAssertions;
    using global::Authentication.API.EndPoints.Constants;
    using global::Authentication.Application.AuthorizationCommands;
    using global::Authentication.Application.Dtos;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using NSubstitute;
    using Shared;
    using Xunit;

    namespace Authentication.Application.Tests.UpdateUser
    {
        public class UpdateEndpointTests
        {
            private readonly HttpClient _client;
            private readonly IMediator _mediator;

            public UpdateEndpointTests()
            {
                _mediator = Substitute.For<IMediator>();

                var builder = new WebHostBuilder()
                    .ConfigureServices(services =>
                    {
                        services.AddRouting();
                        services.AddSingleton(_mediator);
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapPut("/update", async context =>
                            {
                                var dto = await context.Request.ReadFromJsonAsync<UserUpdateDto>();
                                var mediator = context.RequestServices.GetRequiredService<IMediator>();
                                var result = await MinimalApiExtensions.SendRequest(
                                    dto!.ToCommand(),
                                    mediator,
                                    context.RequestAborted
                                );
                                await result.ExecuteAsync(context);
                            });
                        });
                    });

                var server = new TestServer(builder);
                _client = server.CreateClient();
            }

            [Fact]
            public async Task Update_Should_Return_OK_When_Command_Succeeds()
            {
                // Arrange
                _mediator.Send(Arg.Any<UpdateUserCommand>(), Arg.Any<CancellationToken>())
                    .Returns(Result<bool>.Success(true));

                var dto = new UserUpdateDto
                {
                    Id = 1,
                    Email = "new@test.com",
                    FirstName = "NewFirst",
                    LastName = "NewLast"
                };

                // Act
                var response = await _client.PutAsJsonAsync("/update", dto);

                // Assert
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }

            [Fact]
            public async Task Update_Should_Return_BadRequest_When_Command_Fails()
            {
                // Arrange
                _mediator.Send(Arg.Any<UpdateUserCommand>(), Arg.Any<CancellationToken>())
                    .Returns(Result<bool>.Failure("کاربر مورد نظر یافت نشد"));

                var dto = new UserUpdateDto { Id = 999 }; // کاربر ناموجود

                // Act
                var response = await _client.PutAsJsonAsync("/update", dto);

                // Assert
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }

            [Fact]
            public async Task Update_Should_Return_BadRequest_When_Model_Is_Invalid()
            {
                // Arrange
                var invalidDto = new UserUpdateDto
                {
                    Id = 1,
                    Email = "invalid-email",
                    FirstName = "",
                    LastName = ""
                };

                // Act
                var response = await _client.PutAsJsonAsync("/update", invalidDto);

                // Assert
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }
    }
}
