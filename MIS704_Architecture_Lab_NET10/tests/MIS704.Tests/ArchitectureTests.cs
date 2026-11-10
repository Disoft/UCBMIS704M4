using MIS704.Application;
using MIS704.Domain;
using MIS704.Infrastructure;
using Xunit;
namespace MIS704.Tests;
public class ArchitectureTests
{
    [Fact] 
    public void TuitionCalculationIsPure() => Assert.Equal(600m, TuitionCalculator.Calculate(3, 200m));
    
    [Fact] 
    public void InvalidCreditsAreRejected() => Assert.Throws<ArgumentOutOfRangeException>(() => new Enrollment(Guid.NewGuid(), Guid.NewGuid(), 0));
    
    [Fact] 
    public void EventSourcingReplayIsDeterministic()
        => Assert.Equal(70m, AccountProjection.Replay([new AccountOpened(Guid.NewGuid()), new MoneyDeposited(100), new MoneyWithdrawn(30)]));

    [Fact] 
    public void PipelineRemovesDuplicates()
        => Assert.Equal(["ANA", "LUIS"], StudentImportPipeline.Import([" Ana ", "Luis", "ana", " "]));

    [Fact] 
    public async Task UseCaseCanBeTestedWithoutDatabase()
    {
        var repository = new InMemoryEnrollmentRepository();
        var handler = new EnrollStudentHandler(repository, new NoOpPublisher());
        var result = await handler.Handle(new EnrollStudentCommand(Guid.NewGuid(), Guid.NewGuid(), 3));
        Assert.NotNull(await repository.FindAsync(result.Id));
    }

    [Fact] 
    public async Task IdempotencyKeyPreventsDuplicateDemoPayments()
    {
        var gateway = new DemoPaymentGateway();
        var a = await gateway.ChargeAsync("same", 100);
        var b = await gateway.ChargeAsync("same", 100);
        Assert.Equal(a.PaymentId, b.PaymentId);
    }

    private sealed class NoOpPublisher : IEventPublisher
    {
        public Task PublishAsync<T>(T evt, CancellationToken ct = default) => Task.CompletedTask;
    }
}
