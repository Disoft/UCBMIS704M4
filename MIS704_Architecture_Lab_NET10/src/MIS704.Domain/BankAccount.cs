namespace MIS704.Domain;
public abstract record AccountEvent;
public sealed record AccountOpened(Guid AccountId) : AccountEvent;
public sealed record MoneyDeposited(decimal Amount) : AccountEvent;
public sealed record MoneyWithdrawn(decimal Amount) : AccountEvent;
public static class AccountProjection
{
    public static decimal Replay(IEnumerable<AccountEvent> events)
    {
        decimal balance = 0;
        foreach (var evt in events)
        {
            balance = evt switch
            {
                AccountOpened => 0,
                MoneyDeposited e when e.Amount > 0 => balance + e.Amount,
                MoneyWithdrawn e when e.Amount > 0 && e.Amount <= balance => balance - e.Amount,
                _ => throw new InvalidOperationException("Evento inválido o saldo insuficiente")
            };
        }
        return balance;
    }
}
