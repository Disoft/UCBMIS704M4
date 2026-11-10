using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesArchitectureDemo
{
    // =========================================================================
    // 1. MICROSERVICIO DE ÓRDENES (Orders Service)
    // Tiene su propia base de datos aislada y despliegue independiente.
    // =========================================================================
    public class OrderDatabase
    {
        public int Id { get; set; }
        public string Producto { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
    }

    public class OrdersMicroservice
    {
        // Base de datos privada y exclusiva del microservicio de órdenes
        private readonly Dictionary<int, OrderDatabase> _databaseOrdenes = new();

        public async Task<int> CrearOrdenAsync(string producto, decimal total)
        {
            Console.WriteLine("[Microservice - Orders] Procesando solicitud HTTP POST /orders...");
            await Task.Delay(100); // Simula latencia de red

            var orderId = new Random().Next(1000, 9999);
            var nuevaOrden = new OrderDatabase
            {
                Id = orderId,
                Producto = producto,
                Total = total,
                Estado = "Pendiente de Pago"
            };

            _databaseOrdenes[orderId] = nuevaOrden;
            Console.WriteLine($"[Microservice - Orders] Orden {orderId} guardada en su BD dedicada. Estado: {nuevaOrden.Estado}");

            return orderId;
        }

        public async Task ActualizarEstadoOrdenAsync(int orderId, string nuevoEstado)
        {
            await Task.Delay(50);
            if (_databaseOrdenes.TryGetValue(orderId, out var orden))
            {
                orden.Estado = nuevoEstado;
                Console.WriteLine($"[Microservice - Orders] BD de Órdenes actualizada: Orden {orderId} ahora está '{nuevoEstado}'.");
            }
        }
    }

    // =========================================================================
    // 2. MICROSERVICIO DE PAGOS (Payments Service)
    // Base de datos propia y separada. No accede a la BD de órdenes directamente.
    // =========================================================================
    public class PaymentDatabase
    {
        public Guid TransaccionId { get; set; }
        public int OrderId { get; set; }
        public decimal Monto { get; set; }
        public bool Exitoso { get; set; }
    }

    public class PaymentsMicroservice
    {
        // Base de datos privada y exclusiva del microservicio de pagos
        private readonly Dictionary<Guid, PaymentDatabase> _databasePagos = new();

        public async Task<bool> ProcesarPagoAsync(int orderId, decimal monto, string tarjeta)
        {
            Console.WriteLine("[Microservice - Payments] Procesando solicitud HTTP POST /payments...");
            await Task.Delay(150); // Simula latencia de red con pasarela externa

            if (string.IsNullOrWhiteSpace(tarjeta))
            {
                Console.WriteLine("[Microservice - Payments] Error: Tarjeta inválida.");
                return false;
            }

            var transaccionId = Guid.NewGuid();
            var pago = new PaymentDatabase
            {
                TransaccionId = transaccionId,
                OrderId = orderId,
                Monto = monto,
                Exitoso = true
            };

            _databasePagos[transaccionId] = pago;
            Console.WriteLine($"[Microservice - Payments] Pago exitoso registrado en BD de Pagos. Transacción ID: {transaccionId}");

            return true;
        }
    }

    // =========================================================================
    // 3. API GATEWAY / ORQUESTADOR DISTRIBUIDO
    // Coordina la comunicación de red entre microservicios independientes.
    // =========================================================================
    public class ApiGatewayOrchestrator
    {
        private readonly OrdersMicroservice _ordersService;
        private readonly PaymentsMicroservice _paymentsService;

        public ApiGatewayOrchestrator(OrdersMicroservice ordersService, PaymentsMicroservice paymentsService)
        {
            _ordersService = ordersService;
            _paymentsService = paymentsService;
        }

        public async Task EjecutarCheckoutDistribuidoAsync(string producto, decimal precio, string tarjeta)
        {
            Console.WriteLine("=== INICIANDO FLUJO DISTRIBUIDO (MICROSERVICIOS) ===");
            try
            {
                // 1. Llamada de red al Microservicio de Órdenes
                int orderId = await _ordersService.CrearOrdenAsync(producto, precio);

                // 2. Llamada de red independiente al Microservicio de Pagos
                bool pagoExitoso = await _paymentsService.ProcesarPagoAsync(orderId, precio, tarjeta);

                if (pagoExitoso)
                {
                    // 3. Coordinación posterior (consistencia eventual / actualización)
                    await _ordersService.ActualizarEstadoOrdenAsync(orderId, "Pagada y Confirmada");
                }
                else
                {
                    await _ordersService.ActualizarEstadoOrdenAsync(orderId, "Pago Fallido");
                }

                Console.WriteLine("=== FLUJO DE MICROSERVICIOS FINALIZADO ===\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error de Comunicación Distribuida] {ex.Message}\n");
            }
        }
    }

    // =========================================================================
    // 4. PROGRAMA PRINCIPAL (Simulación de Entorno)
    // =========================================================================
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== DEMOSTRACIÓN DE ARQUITECTURA DE MICROSERVICIOS EN C# ===\n");

            // Instanciación de los microservicios independientes (simulan procesos separados)
            var ordersMicroservice = new OrdersMicroservice();
            var paymentsMicroservice = new PaymentsMicroservice();

            // Puerta de enlace que rutea las peticiones
            var apiGateway = new ApiGatewayOrchestrator(ordersMicroservice, paymentsMicroservice);

            // Simulación de transacciones de clientes
            await apiGateway.EjecutarCheckoutDistribuidoAsync("Laptop Gamer", 1500.00m, "4111-XXXX-XXXX-1234");
            await apiGateway.EjecutarCheckoutDistribuidoAsync("Mouse Inalámbrico", 45.00m, "5555-XXXX-XXXX-5678");
        }
    }
}