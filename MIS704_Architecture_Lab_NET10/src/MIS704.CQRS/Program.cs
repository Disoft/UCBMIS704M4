using System;
using System.Collections.Generic;
using System.Linq;

namespace CqrsDemo
{
    // =========================================================================
    // 1. MODELO DE DATOS / PERSISTENCIA
    // =========================================================================
    public class ProductoEntidad
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }

    // Base de datos simulada
    public static class BaseDeDatos
    {
        public static readonly List<ProductoEntidad> Productos = new();
    }

    // =========================================================================
    // 2. COMANDOS (Commands & Handlers) - Modifican el Estado
    // =========================================================================

    // Command: Expresa la intención explícita de un cambio
    public record CrearProductoCommand(string Nombre, decimal Precio, int StockInicial);
    public record ActualizarStockCommand(Guid ProductoId, int CantidadAumentar);

    // Command Handler: Procesa las reglas de negocio y persiste los cambios
    public class ProductoCommandHandler
    {
        public Guid Handle(CrearProductoCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Nombre))
                throw new ArgumentException("El nombre del producto no puede estar vacío.");

            if (command.Precio <= 0)
                throw new ArgumentException("El precio debe ser mayor a cero.");

            var nuevoProducto = new ProductoEntidad
            {
                Id = Guid.NewGuid(),
                Nombre = command.Nombre,
                Precio = command.Precio,
                Stock = command.StockInicial
            };

            BaseDeDatos.Productos.Add(nuevoProducto);
            Console.WriteLine($"[Command] Producto '{nuevoProducto.Nombre}' creado con ID: {nuevoProducto.Id}");

            return nuevoProducto.Id;
        }

        public void Handle(ActualizarStockCommand command)
        {
            var producto = BaseDeDatos.Productos.FirstOrDefault(p => p.Id == command.ProductoId)
                ?? throw new KeyNotFoundException("Producto no encontrado.");

            producto.Stock += command.CantidadAumentar;
            Console.WriteLine($"[Command] Stock de '{producto.Nombre}' actualizado a: {producto.Stock}");
        }
    }

    // =========================================================================
    // 3. CONSULTAS (Queries, DTOs & Handlers) - Solo Lectura
    // =========================================================================

    // DTO de Lectura (Optimizado para vistas/consultas)
    public record ProductoReadModel(Guid Id, string Nombre, decimal Precio, bool EnStock);

    // Queries: Solicitudes de datos sin efectos secundarios
    public record ObtenerProductoPorIdQuery(Guid Id);
    public record ListarProductosDisponiblesQuery();

    // Query Handler: Recupera datos sin modificar el estado
    public class ProductoQueryHandler
    {
        public ProductoReadModel Handle(ObtenerProductoPorIdQuery query)
        {
            var producto = BaseDeDatos.Productos.FirstOrDefault(p => p.Id == query.Id);
            if (producto == null) return null;

            // Proyección al modelo de lectura
            return new ProductoReadModel(
                producto.Id,
                producto.Nombre,
                producto.Precio,
                producto.Stock > 0
            );
        }

        public IEnumerable<ProductoReadModel> Handle(ListarProductosDisponiblesQuery query)
        {
            return BaseDeDatos.Productos
                .Where(p => p.Stock > 0)
                .Select(p => new ProductoReadModel(p.Id, p.Nombre, p.Precio, true))
                .ToList();
        }
    }

    // =========================================================================
    // 4. PROGRAMA PRINCIPAL
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== DEMOSTRACIÓN DE CQRS EN C# ===\n");

            var commandHandler = new ProductoCommandHandler();
            var queryHandler = new ProductoQueryHandler();

            // --- EJECUCIÓN DE COMMANDS (Escritura) ---
            Console.WriteLine("--- 1. Ejecutando Comandos (Modificación de Estado) ---");

            var cmdCrear1 = new CrearProductoCommand("Laptop Gamer", 1200.00m, 5);
            Guid productoId1 = commandHandler.Handle(cmdCrear1);

            var cmdCrear2 = new CrearProductoCommand("Mouse Inalámbrico", 25.50m, 0);
            Guid productoId2 = commandHandler.Handle(cmdCrear2);

            var cmdStock = new ActualizarStockCommand(productoId1, 3);
            commandHandler.Handle(cmdStock);

            Console.WriteLine();

            // --- EJECUCIÓN DE QUERIES (Lectura) ---
            Console.WriteLine("--- 2. Ejecutando Consultas (Lectura Sin Efectos Secundarios) ---");

            var queryDetalle = new ObtenerProductoPorIdQuery(productoId1);
            var detalle = queryHandler.Handle(queryDetalle);
            Console.WriteLine($"[Query Output] Producto: {detalle.Nombre} | Precio: ${detalle.Precio} | Disponible: {detalle.EnStock}");

            Console.WriteLine("\nListado de productos con stock disponible:");
            var queryLista = new ListarProductosDisponiblesQuery();
            foreach (var prod in queryHandler.Handle(queryLista))
            {
                Console.WriteLine($" -> {prod.Nombre} - ${prod.Precio}");
            }
        }
    }
}