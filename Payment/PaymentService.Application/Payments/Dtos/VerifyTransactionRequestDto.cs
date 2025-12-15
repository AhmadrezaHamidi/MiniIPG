using PaymentService.Application.Payments.Commands.VerifyTransaction;

namespace PaymentService.Application.Payments.Dtos;

public record VerifyTransactionRequestDto(
    string Token,
    string AppCode
)
{
    public VerifyTransactionCommand ToCommand() =>
        new VerifyTransactionCommand(
            Token?.Trim(),
            AppCode?.Trim()
        );

    public static implicit operator VerifyTransactionCommand(
        VerifyTransactionRequestDto dto) => dto?.ToCommand();
}