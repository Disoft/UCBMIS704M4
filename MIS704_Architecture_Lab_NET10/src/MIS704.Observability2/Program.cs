using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
// Namespace estándar para Actividades y Trazabilidad en .NET
using System.Diagnostics.Metrics;

namespace MIS704.Observability2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // =========================================================================
            // 1. CONFIGURACIÓN DE OPENTELEMETRY
            // =========================================================================
            // Nota en un entorno real: Instalarías los paquetes NuGet:
            // - OpenTelemetry.Extensions.Hosting
            // - OpenTelemetry.Instrumentation.AspNetCore
            // - OpenTelemetry.Exporter.Console (o Console/Jaeger/OTLP)

            // Definición de un medidor personalizado para métricas con OpenTelemetry
            var meter = new Meter("ObservabilityDemo.Api", "1.0.0");
            var requestCounter = meter.CreateCounter<long>("app.requests.total", description: "Número total de solicitudes HTTP");

            builder.Services.AddEndpointsApiExplorer();

            // Ejemplo de configuración en producción para exportar métricas y trazas:
            /*
            builder.Services.AddOpenTelemetry()
                .WithTracing(tracing => tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter()) // Envía trazas a un colector OTLP (Grafana Tempo, Jaeger, etc.)
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddPrometheusExporter()); // Expone métricas para que Prometheus las consulte
            */

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

          //  app.UseAuthorization();

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

            // =========================================================================
            // 2. MIDDLEWARE DE CORRELATION ID Y TELEMETRÍA
            // =========================================================================
            app.Use(async (context, next) =>
            {
                var sw = Stopwatch.StartNew();

                // Gestión del Correlation ID (Propagación entre servicios)
                string correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();
                if (string.IsNullOrEmpty(correlationId))
                {
                    correlationId = $"corr-{Guid.NewGuid().ToString()[..8]}";
                }
                context.Response.Headers["X-Correlation-ID"] = correlationId;

                // Actividad de OpenTelemetry (Traces) para rastrear la petición actual
                using var activity = new Activity("HttpServer.Request");
                activity.SetTag("correlation.id", correlationId);
                activity.Start();

                // Incremento de la métrica de OpenTelemetry
                requestCounter.Add(1, new KeyValuePair<string, object?>("http.method", context.Request.Method));

                Console.WriteLine($"[LOG] [{correlationId}] INFO: Inicio de petición {context.Request.Method} {context.Request.Path}");

                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    activity.SetStatus(ActivityStatusCode.Error, ex.Message);
                    Console.WriteLine($"[LOG] [{correlationId}] ERROR: {ex.Message}");
                    throw;
                }
                finally
                {
                    sw.Stop();
                    activity.SetTag("http.status_code", context.Response.StatusCode);

                    // Registro de traza/métrica final
                    Console.WriteLine($"[METRIC] app.requests.duration_ms: {sw.ElapsedMilliseconds}ms | Path: {context.Request.Path}");
                    Console.WriteLine($"[LOG] [{correlationId}] INFO: Fin de petición. Duración: {sw.ElapsedMilliseconds}ms\n");
                }
            });

            // =========================================================================
            // 3. ENDPOINT DE EJEMPLO
            // =========================================================================
            app.MapPost("/api/checkout", async (CheckoutRequest request, HttpContext context) =>
            {
                string correlationId = context.Response.Headers["X-Correlation-ID"];

                Console.WriteLine($"[TRACE] [{correlationId}] Ejecutando lógica de negocio para: {request.Producto}");

                // Simulación de procesamiento interno
                await Task.Delay(200);

                return Results.Ok(new
                {
                    Estado = "Procesado con OpenTelemetry",
                    CorrelationId = correlationId,
                    Producto = request.Producto,
                    Timestamp = DateTime.UtcNow
                });
            });

            app.Run();
        }

        public record CheckoutRequest(string Producto, decimal Precio);
    }
}
