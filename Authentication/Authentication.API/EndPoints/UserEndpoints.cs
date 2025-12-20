using Authentication.API.EndPoints.Constants;
using Authentication.Application.AuthorizationCommands;
using Authentication.Application.AuthorizationQueries;
using Authentication.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.EndPoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var userGroup = endpoints.MapGroup(UserConstants.Routes.BaseRoute)
            .WithTags("User")
            .WithApiVersionSet(); // اگر از ApiVersioning استفاده می‌کنی

        userGroup.MapGet(UserConstants.Routes.Login, Login)
            .WithName(UserConstants.Names.Login)
            .WithSummary(UserConstants.Docs.Login.Summary)
            .WithDescription(UserConstants.Docs.Login.Description)
            .AllowAnonymous()
            .WithOpenApi(operation =>
            {
                operation.Responses["200"] = new() { Description = "Login successful" };
                operation.Responses["401"] = new() { Description = "Unauthorized" };
                return operation;
            });

        // RefreshToken - AllowAnonymous
        userGroup.MapGet(UserConstants.Routes.RefreshToken, RefreshToken)
            .WithName(UserConstants.Names.RefreshToken)
            .WithSummary(UserConstants.Docs.RefreshToken.Summary)
            .WithDescription(UserConstants.Docs.RefreshToken.Description)
            .AllowAnonymous()
            .WithOpenApi();

        // GetAll - Requires admin role
        userGroup.MapGet(UserConstants.Routes.GetAll, GetAll)
            .WithName(UserConstants.Names.GetAll)
            .WithSummary(UserConstants.Docs.GetAll.Summary)
            .WithDescription(UserConstants.Docs.GetAll.Description)
            .RequireAuthorization("admin");

        // GetById - Requires admin role
        userGroup.MapGet(UserConstants.Routes.GetById, GetById)
            .WithName(UserConstants.Names.GetById)
            .WithSummary(UserConstants.Docs.GetById.Summary)
            .WithDescription(UserConstants.Docs.GetById.Description)
            .RequireAuthorization("admin");

        // Register - AllowAnonymous
        userGroup.MapPost(UserConstants.Routes.Register, Register)
            .WithName(UserConstants.Names.Register)
            .WithSummary(UserConstants.Docs.Register.Summary)
            .WithDescription(UserConstants.Docs.Register.Description)
            .AllowAnonymous()
            .WithOpenApi();

        // RegisterAdminUser - Requires admin role
        userGroup.MapPost(UserConstants.Routes.RegisterAdminUser, RegisterAdminUser)
            .WithName(UserConstants.Names.RegisterAdminUser)
            .WithSummary(UserConstants.Docs.RegisterAdminUser.Summary)
            .WithDescription(UserConstants.Docs.RegisterAdminUser.Description)
            .RequireAuthorization("admin");

        // Update - Requires authentication (احتمالاً admin یا خود کاربر)
        userGroup.MapPut(UserConstants.Routes.Update, Update)
            .WithName(UserConstants.Names.Update)
            .WithSummary(UserConstants.Docs.Update.Summary)
            .WithDescription(UserConstants.Docs.Update.Description)
            .RequireAuthorization();

        // Delete - Requires admin role
        userGroup.MapDelete(UserConstants.Routes.Delete, Delete)
            .WithName(UserConstants.Names.Delete)
            .WithSummary(UserConstants.Docs.Delete.Summary)
            .WithDescription(UserConstants.Docs.Delete.Description)
            .RequireAuthorization("admin");
    }

    private static Task<IResult> Login(
        IMediator mediator,
        [FromBody] LoginRequestDto requestDto,
        CancellationToken cancellationToken)
        => MinimalApiExtensions.SendRequest(requestDto.ToCommand(), mediator, cancellationToken);



    private static Task<IResult> RefreshToken(
        IMediator mediator,
        [FromBody] RefreshTokenRequestDto requestDto,
        CancellationToken cancellationToken)
        => MinimalApiExtensions.SendRequest(requestDto.ToCommand(), mediator, cancellationToken);


    // GetAll
    private static Task<IResult> GetAll(
        IMediator mediator,
        CancellationToken cancellationToken)
        => MinimalApiExtensions.SendRequest(new GetAllUsersQuery(), mediator, cancellationToken);

    // GetById
    private static Task<IResult> GetById(
        int id,
        IMediator mediator,
        CancellationToken cancellationToken)
        => MinimalApiExtensions.SendRequest(new GetByIdQuery(id), mediator, cancellationToken);

    // Register (normal user)
    private static Task<IResult> Register(
        UserCreateDto model,
        IMediator mediator,
        CancellationToken cancellationToken)
        => MinimalApiExtensions.SendRequest(model.ToCommand("user"), mediator, cancellationToken);

    // RegisterAdminUser
    private static Task<IResult> RegisterAdminUser(
        UserCreateDto model,
        IMediator mediator,
        CancellationToken cancellationToken)
        => MinimalApiExtensions.SendRequest(model.ToCommand("admin"), mediator, cancellationToken);

    // Update
    private static Task<IResult> Update(
        UserUpdateDto model,
        IMediator mediator,
        CancellationToken cancellationToken)
        => MinimalApiExtensions.SendRequest(model.ToCommand(), mediator, cancellationToken);

    // Delete
    private static Task<IResult> Delete(
        int userId,
        IMediator mediator,
        CancellationToken cancellationToken)
        => MinimalApiExtensions.SendRequest(new DeleteUserCommand(userId), mediator, cancellationToken);
}

