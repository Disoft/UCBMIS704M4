using System.Diagnostics;
using MIS704.Application;
using MIS704.Infrastructure;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IEnrollmentRepository, InMemoryEnrollmentRepository>();
builder.Services.AddSingleton<IEventPublisher, ConsoleEventPublisher>();
builder.Services.AddSingleton<IPaymentGateway, DemoPaymentGateway>();
builder.Services.AddScoped<EnrollStudentHandler>();
builder.Services.AddScoped<GetEnrollmentHandler>();
builder.Services.AddHttpClient("ExternalProvider", client => client.Timeout = TimeSpan.FromSeconds(3));
var app = builder.Build();
app.MapGet("/", () => Results.Ok(new { Name = "MIS704 Architecture Lab", Version = "net10.0" }));
app.MapPost("/api/enrollments", async (EnrollStudentCommand command, EnrollStudentHandler handler, CancellationToken ct) =>
{
    try { var item = await handler.Handle(command, ct); return Results.Created($"/api/enrollments/{item.Id}", item); }
    catch (ArgumentException e) { return Results.BadRequest(new { error = e.Message }); }
});
app.MapGet("/api/enrollments/{id:guid}", async (Guid id, GetEnrollmentHandler handler, CancellationToken ct) =>
{
    var item = await handler.Handle(new GetEnrollmentQuery(id), ct);
    return item is null ? Results.NotFound() : Results.Ok(item);
});
app.MapGet("/api/enrollments", async (IEnrollmentRepository repository, CancellationToken ct) => Results.Ok(await repository.ListAsync(ct)));
app.MapPost("/api/payments", async (PaymentRequest request, IPaymentGateway gateway, CancellationToken ct) =>
{
    try { return Results.Ok(await gateway.ChargeAsync(request.IdempotencyKey, request.Amount, ct)); }
    catch (ArgumentException e) { return Results.BadRequest(new { error = e.Message }); }
});
// Para ilustrar latencia; NO es un benchmark ni mide p95.
app.MapGet("/api/demo/latency", async (CancellationToken ct) =>
{
    var watch = Stopwatch.StartNew();
    await Task.Delay(50, ct);
    return Results.Ok(new { processingMs = watch.ElapsedMilliseconds });
});
// Endpoint protegido a nivel de objeto mediante comprobación didáctica del identificador.
// En producción el ID debe obtenerse de claims validados por middleware de autenticación.
app.MapGet("/api/demo/student/{id:guid}/grades", (Guid id, HttpContext http) =>
{
    return Results.StatusCode(StatusCodes.Status501NotImplemented);
});
app.Run();
public sealed record PaymentRequest(string IdempotencyKey, decimal Amount);
public partial class Program { }
