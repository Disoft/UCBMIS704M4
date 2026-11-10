using System;
using System.Collections.Generic;

namespace ServiceBasedArchitectureDemo
{
    // =========================================================================
    // 1. BASE DE DATOS COMPARTIDA Y MODELOS DE DATOS
    // Comparten un mismo motor, pero con zonas estrictas de ownership lógico.
    // =========================================================================
    public class ProductoDbModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
    }

    public class FacturaDbModel
    {
        public Guid FacturaId { get; set; }
        public string Detalle { get; set; }
        public decimal Total { get; set; }
    }

    // Simulación de base de datos compartida
    public static class SharedDatabase
    {
        // Tabla de propiedad del Servicio de Ventas y Pagos
        public static Dictionary<int, ProductoDbModel> TablaProductos = new()
        {
            { 1, new ProductoDbModel { Id = 1, Nombre = "Laptop Empresarial", Precio = 1200.00m } },
            { 2, new ProductoDbModel { Id = 2, Nombre = "Teclado Mecánico", Precio = 85.50m } }
        };

        // Tabla de propiedad del Servicio de Facturación
        public static Dictionary<Guid, FacturaDbModel> TablaFacturas = new();
    }

    // =========================================================================
    // 2. SERVICIO DE GRANO GRUESO: VENTAS Y PAGOS (Coarse-grained Service)
    // Agrupa múltiples capacidades (Catálogo, Carrito, Pagos) en una sola unidad.
    // =========================================================================
    public class VentasYPagosService
    {
        // Ownership de escritura: Solo este servicio modifica TablaProductos y procesa pagos
        public ProductoDbModel ConsultarProducto(int id)
        {
            return SharedDatabase.TablaProductos.TryGetValue(id, out var producto) ? producto : null;
        }

        public void ProcesarCheckoutYPago(int productoId, string tarjetaCredito)
        {
            Console.WriteLine("[Servicio Ventas y Pagos] Iniciando flujo de grano grueso (Catálogo -> Carrito -> Pago)...");

            var producto = ConsultarProducto(productoId);
            if (producto == null)
            {
                throw new InvalidOperationException("El producto no existe en el catálogo.");
            }

            if (string.IsNullOrWhiteSpace(tarjetaCredito))
            {
                throw new ArgumentException("La información de pago es inválida.");
            }

            Console.WriteLine($"[Servicio Ventas y Pagos] Pago procesado con éxito por {producto.Precio} usando la tarjeta terminada en ****.");
        }
    }

    // =========================================================================
    // 3. SERVICIO DE GRANO GRUESO: FACTURACIÓN Y REPORTES
    // Otro servicio independiente que comparte la base pero con su propia disciplina.
    // =========================================================================
    public class FacturacionService
    {
        // Ownership de escritura: Solo este servicio genera y escribe facturas
        public Guid GenerarFactura(string detalle, decimal total)
        {
            var facturaId = Guid.NewGuid();
            var factura = new FacturaDbModel
            {
                FacturaId = facturaId,
                Detalle = detalle,
                Total = total
            };

            SharedDatabase.TablaFacturas[facturaId] = factura;
            Console.WriteLine($"[Servicio Facturación] Factura {facturaId} generada y guardada en la base de datos compartida.");
            return facturaId;
        }
    }

    // =========================================================================
    // 4. CAPA DE ENTRADA / ORQUESTACIÓN (API Gateway o Controlador)
    // =========================================================================
    public class ECommerceOrchestratorController
    {
        private readonly VentasYPagosService _ventasService;
        private readonly FacturacionService _facturacionService;

        public ECommerceOrchestratorController(VentasYPagosService ventasService, FacturacionService facturacionService)
        {
            _ventasService = ventasService;
            _facturacionService = facturacionService;
        }

        public void EjecutarCompraCompleta(int productoId, string tarjeta)
        {
            Console.WriteLine("=== INICIANDO TRANSACCIÓN SERVICE-BASED ===");
            try
            {
                // 1. Validar y procesar mediante el servicio de grano grueso de ventas
                var producto = _ventasService.ConsultarProducto(productoId);
                _ventasService.ProcesarCheckoutYPago(productoId, tarjeta);

                // 2. Coordinar con el servicio de facturación
                _facturacionService.GenerarFactura($"Compra de: {producto.Nombre}", producto.Precio);

                Console.WriteLine("=== COMPRA FINALIZADA EXITOSAMENTE ===\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error en Orquestación] {ex.Message}\n");
            }
        }
    }

    // =========================================================================
    // 5. PROGRAMA PRINCIPAL (Composition Root)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            // Inicialización de los servicios de grano grueso
            var ventasYPagosService = new VentasYPagosService();
            var facturacionService = new FacturacionService();

            // Inicialización de la capa de entrada
            var controller = new ECommerceOrchestratorController(ventasYPagosService, facturacionService);

            // Simulación de solicitudes de usuario
            controller.EjecutarCompraCompleta(1, "4111-2222-3333-4444");

            // Simulación de error por producto inexistente
            controller.EjecutarCompraCompleta(99, "4111-2222-3333-4444");
        }
    }
}