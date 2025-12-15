using PaymentService.Application.Payments.Commands.CreateTransaction;

namespace PaymentService.Application.Payments.Dtos;

public record CreateTransactionRequestDto(
   string TerminalNo,
   decimal Amount,
   string RedirectUrl,
   string ReservationNumber,
   string PhoneNumber
   )
{
    public CreateTransactionCommand ToCommand() =>
        new CreateTransactionCommand(
            TerminalNo?.Trim(),
            Amount,
            RedirectUrl?.Trim(),
            ReservationNumber?.Trim(),
            PhoneNumber?.Trim()
        );

    public static implicit operator CreateTransactionCommand(
        CreateTransactionRequestDto dto) => dto?.ToCommand();
}
