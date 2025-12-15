using MediatR;
using Shared;

namespace PaymentService.API.Abstraction;

public static class MinimalApiExtensions
{
    public static async Task<IResult> SendRequest<T>(
        IRequest<Result<T>> request,
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await mediator.Send(request, cancellationToken);

            return result.IsSuccess
                ? Results.Ok(ApiResponse<T>.Success(result.Data))
                : Results.BadRequest(ApiResponse<T>.Failure(result.Message));
        }
        //catch (ValidationException ex)
        //{
        //    return Results.BadRequest(ApiResponse<object>.ValidationFailure(ex.Message));
        //}
        catch (Exception ex)
        {
            return Results.Problem(
                detail: "خطای داخلی سرور",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error");
        }
    }
}
