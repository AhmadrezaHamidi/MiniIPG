using Authentication.API.EndPoints.Constants;

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
        [AsParameters] LoginRequestDto requestDto,
        CancellationToken cancellationToken)
        => MinimalApiExtensions.SendRequest(() => userService.GetToken(requestDto.Username, requestDto.Password), cancellationToken);

    private static Task<IResult> RefreshToken(
        [AsParameters] RefreshTokenRequestDto requestDto,
        IUserService userService,
        CancellationToken cancellationToken)
        => MinimalApiExtensions.SendRequest(() => userService.GetRefreshToken(requestDto.RefreshToken), cancellationToken);

    private static Task<IResult> GetAll(IUserService userService, CancellationToken cancellationToken)
        => MinimalApiExtensions.SendRequest(() => userService.GetAll(), cancellationToken);

    private static Task<IResult> GetById(
        int id,
        IUserService userService,
        CancellationToken cancellationToken)
        => MinimalApiExtensions.SendRequest(() => userService.GetById(id), cancellationToken);

    // Register
    private static Task<IResult> Register(
        UserCreateDto model,
        IUserService userService,
        CancellationToken cancellationToken)
        => MinimalApiExtensions.SendAsync(() => userService.Create(model, "user"), cancellationToken);

    // RegisterAdminUser
    private static Task<IResult> RegisterAdminUser(
        UserCreateDto model,
        IUserService userService,
        CancellationToken cancellationToken)
        => MinimalApiExtensions.SendAsync(() => userService.Create(model, "admin"), cancellationToken);

    // Update
    private static Task<IResult> Update(
        UserUpdateDto model,
        IUserService userService,
        CancellationToken cancellationToken)
        => MinimalApiExtensions.SendAsync(() => userService.Update(model), cancellationToken);

    // Delete
    private static Task<IResult> Delete(
        int userId,
        IUserService userService,
        CancellationToken cancellationToken)
        => MinimalApiExtensions.SendAsync(() => userService.Delete(userId), cancellationToken);
}

