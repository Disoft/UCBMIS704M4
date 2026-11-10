namespace MIS704.Demo;
// Código intencionalmente problemático: usar SOLO para análisis de clase.
public static class Before
{
    // Connascence of position: amount y discount comparten tipo.
    public static void ProcessOrder(int id, string customer, decimal amount, string currency, decimal discount)
        => Console.WriteLine($"BEFORE {id} {customer} {amount} {currency} {discount}");
}
public sealed record OrderRequest(int Id, string Customer, decimal Amount, string Currency, decimal Discount);
public static class After
{
    public static void ProcessOrder(OrderRequest request)
        => Console.WriteLine($"AFTER {request.Id} {request.Customer} {request.Amount} {request.Currency} {request.Discount}");
}
// Mala separación de responsabilidades, baja cohesión y alto acoplamiento.
// Ejemplo NO utilizado por la API principal.
public sealed class BadEnrollmentManager
{
    public void EnrollAndNotify(Guid studentId, Guid courseId)
    {
        Console.WriteLine("Validar matrícula");
        Console.WriteLine("INSERT INTO Enrollments ... (SQL acoplado a negocio)");
        Console.WriteLine("SMTP: enviar correo (infraestructura acoplada)");
    }
}
