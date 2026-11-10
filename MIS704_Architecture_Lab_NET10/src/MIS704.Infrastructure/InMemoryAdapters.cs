using System.Collections.Concurrent;
using MIS704.Application;
using MIS704.Domain;
namespace MIS704.Infrastructure;
// Demo solamente: persistencia efímera, sin garantías ACID.
public sealed class InMemoryEnrollmentRepository : IEnrollmentRepository
{
    private readonly ConcurrentDictionary<Guid, Enrollment> _items = new();
    public Task AddAsync(Enrollment item, CancellationToken ct = default)
    { _items[item.Id] = item; return Task.CompletedTask; }
    public Task<Enrollment?> FindAsync(Guid id, CancellationToken ct = default)
    { _items.TryGetValue(id, out var item); return Task.FromResult(item); }
    public Task<IReadOnlyList<Enrollment>> ListAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Enrollment>>(_items.Values.ToList());
}
public sealed class ConsoleEventPublisher : IEventPublisher
{
    public Task PublishAsync<T>(T evt, CancellationToken ct = default)
    { Console.WriteLine($"EVENT => {evt}"); return Task.CompletedTask; }
}
// Simula idempotencia en memoria. NO es un sustituto de una transacción durable.
public sealed class DemoPaymentGateway : IPaymentGateway
{
    private readonly ConcurrentDictionary<string, Guid> _processed = new();
    public Task<PaymentResult> ChargeAsync(string key, decimal amount, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key) || amount <= 0) throw new ArgumentException("Pago inválido");
        var id = Guid.NewGuid();
        var stored = _processed.GetOrAdd(key, id);
        return Task.FromResult(new PaymentResult(stored, stored != id));
    }
}
