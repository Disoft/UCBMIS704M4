
using System.Diagnostics;

namespace MIS704.LactencyTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/weatherforecast", (HttpContext httpContext) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast");

            app.Use(async (context, next) =>
            {
                var sw = Stopwatch.StartNew();

                // Registrar el header justo antes de enviar la respuesta
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers["X-Latency-Ms"] = sw.ElapsedMilliseconds.ToString();
                    return Task.CompletedTask;
                });

                // Ejecuta el siguiente componente en el pipeline
                await next();

                sw.Stop();
                var elapsedMs = sw.ElapsedMilliseconds;

                // Registro de telemetría (Aquí se instrumenta la medición de rendimiento)
                Console.WriteLine($"[TELEMETRÍA] Endpoint: {context.Request.Path} | Método: {context.Request.Method} | Latencia: {elapsedMs} ms");
            });

            // =========================================================================
            // 2. ENDPOINT DEL SISTEMA DE MATRÍCULAS (Alta Concurrencia)
            // =========================================================================
            app.MapPost("/api/matriculas", (SolicitudMatricula solicitud) =>
            {
                // Simulación de la lógica de negocio para la inscripción al curso
                // (En producción aquí ocurrirían las validaciones y transacciones de base de datos)
                Thread.Sleep(45); // Simulación de procesamiento interno

                return Results.Ok(new
                {
                    Estado = "Exitoso",
                    Mensaje = $"Estudiante {solicitud.EstudianteId} matriculado correctamente en el curso {solicitud.CursoId}",
                    Timestamp = DateTime.UtcNow
                });
            });

            app.Run();
        }

        public record SolicitudMatricula(int EstudianteId, int CursoId);
    }
}
