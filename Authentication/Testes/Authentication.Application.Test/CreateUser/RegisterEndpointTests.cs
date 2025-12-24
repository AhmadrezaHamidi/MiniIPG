using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Authentication.API.EndPoints.Constants;
using Authentication.Application.AuthorizationCommands;
using Authentication.Application.Dtos;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shared;

namespace Authentication.Application.Test.CreateUser;

public class RegisterEndpointTests
{
    private readonly HttpClient _client;
    private readonly IMediator _mediator;

    public RegisterEndpointTests()
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
                    endpoints.MapPost("/register", async context =>
                    {
                        var dto = await context.Request.ReadFromJsonAsync<UserCreateDto>();
                        var mediator = context.RequestServices.GetRequiredService<IMediator>();

                        var result = await MinimalApiExtensions.SendRequest(
                            dto!.ToCommand("user"),
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
    public async Task Register_Should_Return_OK_When_Command_Succeeds()
    {
        // Arrange
        _mediator.Send(Arg.Any<CreateUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1));

        var dto = new UserCreateDto(
            "testuser",
            "test@test.com",
            "Test",
            "User",
            "123456",
            "123456"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/register", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_Should_Return_BadRequest_When_Command_Fails()
    {
        // Arrange
        _mediator.Send(Arg.Any<CreateUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<int>.Failure("error"));

        // Act
        var response = await _client.PostAsJsonAsync("/register", new UserCreateDto());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}