using MIS704.Domain;
namespace MIS704.Application;
// Information Hiding / Hexagonal Ports / Dependency Inversion.
public interface IEnrollmentRepository
{
    Task AddAsync(Enrollment enrollment, CancellationToken ct = default);
    Task<Enrollment?> FindAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Enrollment>> ListAsync(CancellationToken ct = default);
}
public interface IEventPublisher
{
    Task PublishAsync<T>(T eventData, CancellationToken ct = default);
}
public interface IPaymentGateway
{
    Task<PaymentResult> ChargeAsync(string idempotencyKey, decimal amount, CancellationToken ct = default);
}
public sealed record PaymentResult(Guid PaymentId, bool AlreadyProcessed);
public sealed record EnrollmentCreated(Guid EnrollmentId, Guid StudentId, Guid CourseId);
// CQRS: comando y consulta separados, SIN exigir dos bases de datos.
public sealed record EnrollStudentCommand(Guid StudentId, Guid CourseId, int Credits);
public sealed record GetEnrollmentQuery(Guid EnrollmentId);
