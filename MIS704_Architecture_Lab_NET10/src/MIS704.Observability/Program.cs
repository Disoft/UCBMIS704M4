using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ObservabilityDemo
{
    // =========================================================================
    // 0. COMPONENTE DE CONTEXTO Y CORRELACIÓN (Correlation ID)
    // =========================================================================
    public static class TraceContext
    {
        // Permite mantener el Correlation ID vivo durante el hilo de ejecución actual
        private static readonly AsyncLocal<string> _correlationId = new();

        public static string CurrentCorrelationId
        {
            get => _correlationId.Value ?? string.Empty;
            set => _correlationId.Value = value;
        }
    }

    // =========================================================================
    // 1. SISTEMA DE MÉTRICAS SIMULADO (Metrics)
    // =========================================================================
    public static class MetricsCollector
    {
        private static int _totalSolicitudes = 0;
        private static int _totalErrores = 0;
        private static readonly Dictionary<string, long> _latenciasPorServicio = new();

        public static void RegistrarSolicitud(string servicio)
        {
            _totalSolicitudes++;
            Console.WriteLine($"[METRIC] Counter -> requests_total( {{service=\"{servicio}\"}} ) Count: {_totalSolicitudes}");
        }

        public static void RegistrarLatencia(string servicio, long ms)
        {
            _latenciasPorServicio[servicio] = ms;
            Console.WriteLine($"[METRIC] Histogram -> request_duration_ms( {{service=\"{servicio}\"}} ) Latency: {ms}ms");
        }

        public static void RegistrarError(string servicio)
        {
            _totalErrores++;
            Console.WriteLine($"[METRIC] Alert -> error_rate_high( {{service=\"{servicio}\"}} ) Total Errors: {_totalErrores}");
        }
    }

    // =========================================================================
    // 2. MICROSERVICIO DE ÓRDENES (Orders Service)
    // =========================================================================
    public class OrdersService
    {
        public async Task<int> ProcesarOrdenAsync(string producto, decimal precio)
        {
            var sw = Stopwatch.StartNew();
            MetricsCollector.RegistrarSolicitud("OrdersService");

            string correlationId = TraceContext.CurrentCorrelationId;
            Console.WriteLine($"[LOG] [{correlationId}] [OrdersService] INFO: Iniciando creación de orden para '{producto}' por ${precio}");

            // Simulación de procesamiento de base de datos
            await Task.Delay(100); 

            int orderId = new Random().Next(1000, 9999);
            Console.WriteLine($"[LOG] [{correlationId}] [OrdersService] INFO: Orden {orderId} creada exitosamente con estado 'Pendiente'");

            sw.Stop();
            MetricsCollector.RegistrarLatencia("OrdersService", sw.ElapsedMilliseconds);
            return orderId;
        }
    }

    // =========================================================================
    // 3. MICROSERVICIO DE PAGOS (Payments Service)
    // =========================================================================
    public class PaymentsService
    {
        public async Task<bool> ProcesarPagoAsync(int orderId, decimal monto)
        {
            var sw = Stopwatch.StartNew();
            MetricsCollector.RegistrarSolicitud("PaymentsService");

            string correlationId = TraceContext.CurrentCorrelationId;
            Console.WriteLine($"[LOG] [{correlationId}] [PaymentsService] INFO: Procesando pago de ${monto} para la Orden #{orderId}");

            // Simulación de latencia de red con pasarela externa (ej. 1.2 segundos para evidenciar SLO/latencia alta)
            await Task.Delay(1200);

            Console.WriteLine($"[LOG] [{correlationId}] [PaymentsService] INFO: Pago aprobado con éxito para la Orden #{orderId}");

            sw.Stop();
            MetricsCollector.RegistrarLatencia("PaymentsService", sw.ElapsedMilliseconds);
            return true;
        }
    }

    // =========================================================================
    // 4. MICROSERVICIO DE INVENTARIO (Inventory Service)
    // =========================================================================
    public class InventoryService
    {
        public async Task<bool> ReservarStockAsync(string producto)
        {
            var sw = Stopwatch.StartNew();
            MetricsCollector.RegistrarSolicitud("InventoryService");

            string correlationId = TraceContext.CurrentCorrelationId;
            Console.WriteLine($"[LOG] [{correlationId}] [InventoryService] INFO: Verificando y reservando stock para '{producto}'");

            await Task.Delay(150);

            Console.WriteLine($"[LOG] [{correlationId}] [InventoryService] INFO: Stock reservado correctamente para '{producto}'");

            sw.Stop();
            MetricsCollector.RegistrarLatencia("InventoryService", sw.ElapsedMilliseconds);
            return true;
        }
    }

    // =========================================================================
    // 5. API GATEWAY / ORQUESTADOR DISTRIBUIDO (Traces y Correlation ID)
    // =========================================================================
    public class ApiGatewayOrchestrator
    {
        private readonly OrdersService _ordersService = new();
        private readonly PaymentsService _paymentsService = new();
        private readonly InventoryService _inventoryService = new();

        public async Task EjecutarCheckoutDistribuidoAsync(string producto, decimal precio)
        {
            // Generar o propagar el Correlation ID que conecta toda la traza distribuida
            string correlationId = $"corr-{Guid.NewGuid().ToString()[..8]}";
            TraceContext.CurrentCorrelationId = correlationId;

            var traceStopwatch = Stopwatch.StartNew();
            Console.WriteLine($"\n==================================================================");
            Console.WriteLine($"[TRACE INICIO] CorrelationID: {correlationId} | Solicitud de Checkout");
            Console.WriteLine($"==================================================================");

            try
            {
                // 1. Servicio de Órdenes
                int orderId = await _ordersService.ProcesarOrdenAsync(producto, precio);

                // 2. Servicio de Pagos
                bool pagoExitoso = await _paymentsService.ProcesarPagoAsync(orderId, precio);

                // 3. Servicio de Inventario
                if (pagoExitoso)
                {
                    await _inventoryService.ReservarStockAsync(producto);
                }
                else
                {
                    MetricsCollector.RegistrarError("PaymentsService");
                    Console.WriteLine($"[LOG] [{correlationId}] [Gateway] ERROR: El pago falló, abortando inventario.");
                }

                traceStopwatch.Stop();
                long duracionTotal = traceStopwatch.ElapsedMilliseconds;

                Console.WriteLine($"\n[TRACE FIN] CorrelationID: {correlationId} | Duración Total: {duracionTotal}ms");

                // Validación de SLO (Ejemplo: SLO establecido en < 1000ms)
                if (duracionTotal > 1000)
                {
                    Console.WriteLine($"[SLO ALERTA] La solicitud superó el umbral de rendimiento (Latencia real: {duracionTotal}ms > SLO: 1000ms).");
                }
                Console.WriteLine($"==================================================================\n");
            }
            catch (Exception ex)
            {
                MetricsCollector.RegistrarError("ApiGateway");
                Console.WriteLine($"[LOG] [{correlationId}] [Gateway] CRITICAL: Excepción no controlada: {ex.Message}");
            }
        }
    }

    // =========================================================================
    // 6. PROGRAMA PRINCIPAL
    // =========================================================================
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== DEMOSTRACIÓN DE OBSERVABILIDAD EN C# (LOGS, METRICS, TRACES) ===");

            var gateway = new ApiGatewayOrchestrator();

            // Simular una solicitud de cliente que toma más de lo esperado
            await gateway.EjecutarCheckoutDistribuidoAsync("Laptop Gamer Pro", 1850.00m);
        }
    }
}