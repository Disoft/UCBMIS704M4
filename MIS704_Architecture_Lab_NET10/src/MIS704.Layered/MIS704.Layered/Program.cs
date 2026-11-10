using System;
using System.Collections.Generic;

namespace ArquitecturaEnCapasDemo
{
    // =========================================================================
    // 1. CAPA DE ACCESO A DATOS (Data Access Layer - DAL)
    // =========================================================================
    // Responsabilidad: Persistencia, consultas y comunicación directa con la fuente de datos.
    public interface IProductoRepository
    {
        string ObtenerNombrePorId(int id);
    }

    public class ProductoRepositorySql : IProductoRepository
    {
        public string ObtenerNombrePorId(int id)
        {
            // Simulación de consulta a base de datos relacional
            // En un caso real, aquí usarías Entity Framework, Dapper o ADO.NET
            return id == 1 ? "Laptop de Gama Alta" : "Producto Desconocido";
        }
    }

    // =========================================================================
    // 2. CAPA DE NEGOCIO (Business Logic Layer - BLL)
    // =========================================================================
    // Responsabilidad: Reglas de negocio, casos de uso y coordinación.
    public class ProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public string ObtenerDetalleFormateado(int id)
        {
            // Regla de negocio / validación simple
            if (id <= 0)
            {
                throw new ArgumentException("El ID del producto debe ser mayor a cero.");
            }

            string nombre = _productoRepository.ObtenerNombrePorId(id);

            // Lógica de presentación de negocio
            return $"[Verificado] Producto: {nombre.ToUpper()}";
        }
    }

    // =========================================================================
    // 3. CAPA DE PRESENTACIÓN (Presentation Layer)
    // =========================================================================
    // Responsabilidad: Interfaz de usuario, captura de entrada y renderizado de salida.
    public class ProductoController
    {
        private readonly ProductoService _productoService;

        public ProductoController(ProductoService productoService)
        {
            _productoService = productoService;
        }

        public void MostrarVistaProducto(int id)
        {
            Console.WriteLine("--- Capa de Presentación (Iniciando Solicitud) ---");
            try
            {
                string resultado = _productoService.ObtenerDetalleFormateado(id);
                Console.WriteLine($"Vista Web/API - Resultado: {resultado}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Vista Web/API - Error mostrado al usuario: {ex.Message}");
            }
        }
    }

    // =========================================================================
    // 4. PROGRAMA PRINCIPAL / ARRANQUE (Composición de Capas)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            // Configuración de dependencias (Wiring up)
            IProductoRepository repository = new ProductoRepositorySql();
            ProductoService service = new ProductoService(repository);
            ProductoController controller = new ProductoController(service);

            // Simulación de interacción del usuario
            controller.MostrarVistaProducto(1);
            Console.WriteLine();
            controller.MostrarVistaProducto(-5); // Dispara la validación de negocio
        }
    }
}