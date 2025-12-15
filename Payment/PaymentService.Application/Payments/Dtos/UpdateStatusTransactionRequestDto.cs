using PaymentService.Application.Payments.Commands.UpdateStatusTransaction;

namespace PaymentService.Application.Payments.Dtos;

public record UpdateStatusTransactionRequestDto(
    string Token,
    bool IsSuccess,
    string? Rrn
)
{
    public UpdateStatusTransactionCommand ToCommand() =>
        new UpdateStatusTransactionCommand(
            Token?.Trim(),
            IsSuccess,
            Rrn?.Trim()
        );

    public static implicit operator UpdateStatusTransactionCommand(
        UpdateStatusTransactionRequestDto dto) => dto?.ToCommand();
}
