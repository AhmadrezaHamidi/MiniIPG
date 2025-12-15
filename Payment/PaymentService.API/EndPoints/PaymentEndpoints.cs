using System.ComponentModel.DataAnnotations;
using System.Data;
using Azure.Core;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentService.API.Abstraction;
using PaymentService.API.EndPoints.Constants;
using PaymentService.Application.Payments.Commands.UpdateStatusTransaction;
using PaymentService.Application.Payments.Commands.VerifyTransaction;
using PaymentService.Application.Payments.Dtos;
using Refit;
using Shared;

namespace PaymentService.API.EndPoints;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var paymentGroup = endpoints.MapGroup(PaymentConstants.Routes.BaseRoute)
            .WithApiVersionSet()
            .WithTags("Payment")
            .RequireAuthorization(); // اگر نیاز به احراز هویت دارد

        // Get Token
        paymentGroup.MapPost(PaymentConstants.Routes.GetToken, GetToken)
            .WithName(PaymentConstants.Names.GetToken)
            .WithSummary(PaymentConstants.Docs.GetToken.Summary)
            .WithDescription(PaymentConstants.Docs.GetToken.Description)
            .WithOpenApi(operation =>
            {
                operation.Responses["200"] = new() { Description = "Token created successfully" };
                operation.Responses["400"] = new() { Description = "Bad request" };
                return operation;
            });

        // Verify Token
        paymentGroup.MapPost(PaymentConstants.Routes.Verify, VerifyToken)
            .WithName(PaymentConstants.Names.Verify)
            .WithSummary(PaymentConstants.Docs.Verify.Summary)
            .WithDescription(PaymentConstants.Docs.Verify.Description)
            .WithOpenApi();

        // Update Status
        paymentGroup.MapPost(PaymentConstants.Routes.UpdateStatus, UpdateStatus)
            .WithName(PaymentConstants.Names.UpdateStatus)
            .WithSummary(PaymentConstants.Docs.UpdateStatus.Summary)
            .WithDescription(PaymentConstants.Docs.UpdateStatus.Description)
            .WithOpenApi();
    }
    private static Task<IResult> GetToken(
     CreateTransactionRequestDto requestDto,
     IMediator mediator,
     CancellationToken cancellationToken)
     => MinimalApiExtensions.SendRequest(requestDto.ToCommand(), mediator, cancellationToken);

    private static Task<IResult> VerifyToken(
        VerifyTransactionRequestDto requestDto,
        IMediator mediator,
        CancellationToken cancellationToken)
        => MinimalApiExtensions.SendRequest(requestDto.ToCommand(), mediator, cancellationToken);

    private static Task<IResult> UpdateStatus(
        UpdateStatusTransactionRequestDto requestDto,
        IMediator mediator,
        CancellationToken cancellationToken)
        => MinimalApiExtensions.SendRequest(requestDto.ToCommand(), mediator, cancellationToken);
}