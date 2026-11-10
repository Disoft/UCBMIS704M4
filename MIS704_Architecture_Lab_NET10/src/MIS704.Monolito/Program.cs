using System;
using System.Collections.Generic;

namespace MonolitoModularDemo
{
    // =========================================================================
    // 1. MÓDULO DE CATÁLOGO (Límites Internos Explícitos)
    // =========================================================================
    namespace Catalogo.Contratos
    {
        // API Interna Explícita del Módulo de Catálogo
        public interface ICatalogoService
        {
            bool VerificarStock(int productoId, int cantidad);
            void ReducirStock(int productoId, int cantidad);
        }

        public record ProductoDto(int Id, string Nombre, decimal Precio, int StockDisponibles);
    }

    namespace Catalogo.Internos
    {
        using Catalogo.Contratos;

        // Implementación oculta dentro del módulo (Encapsulamiento y Ocultamiento de Información)
        internal class CatalogoInternalService : ICatalogoService
        {
            private readonly Dictionary<int, (string Nombre, decimal Precio, int Stock)> _inventario = new()
            {
                { 1, ("Laptop Pro", 1200.00m, 10) },
                { 2, ("Mouse Inalámbrico", 25.50m, 50) }
            };

            public bool VerificarStock(int productoId, int cantidad)
            {
                if (_inventario.ContainsKey(productoId))
                {
                    return _inventario[productoId].Stock >= cantidad;
                }
                return false;
            }

            public void ReducirStock(int productoId, int cantidad)
            {
                if (_inventario.ContainsKey(productoId))
                {
                    var item = _inventario[productoId];
                    _inventario[productoId] = (item.Nombre, item.Precio, item.Stock - cantidad);
                    Console.WriteLine($"[Catálogo] Stock actualizado para '{item.Nombre}'. Stock restante: {item.Stock - cantidad}");
                }
            }
        }

        // Fábrica o punto de acceso controlado al módulo
        public static class CatalogoModuleApi
        {
            public static ICatalogoService ObtenerServicio() => new CatalogoInternalService();
        }
    }


    // =========================================================================
    // 2. MÓDULO DE PEDIDOS (Comunicación local mediante interfaces internas)
    // =========================================================================
    namespace Pedidos.Internos
    {
        using Catalogo.Contratos;

        public class GestorDePedidos
        {
            private readonly ICatalogoService _catalogoService;

            // El módulo de Pedidos se comunica con el de Catálogo a través de una interfaz 
            // en memoria, SIN llamadas de red, sockets ni HTTP.
            public GestorDePedidos(ICatalogoService catalogoService)
            {
                _catalogoService = catalogoService;
            }

            public void ProcesarPedido(int productoId, int cantidad)
            {
                Console.WriteLine($"[Pedidos] Iniciando procesamiento para el producto ID: {productoId}, Cantidad: {cantidad}");

                // Comunicación local / llamada en memoria
                if (_catalogoService.VerificarStock(productoId, cantidad))
                {
                    _catalogoService.ReducirStock(productoId, cantidad);
                    Console.WriteLine("[Pedidos] ¡Pedido procesado y completado exitosamente con transacción local!\n");
                }
                else
                {
                    Console.WriteLine("[Pedidos] Error: Stock insuficiente o producto no encontrado.\n");
                }
            }
        }
    }


    // =========================================================================
    // 3. PROGRAMA PRINCIPAL (Unidad Única de Despliegue - Monolito)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== INICIANDO MONOLITO MODULAR ===\n");

            // 1. Obtenemos el servicio expuesto por el módulo de Catálogo mediante su API interna
            Catalogo.Contratos.ICatalogoService catalogoService = Catalogo.Internos.CatalogoModuleApi.ObtenerServicio();

            // 2. Inicializamos el módulo de Pedidos inyectándole el servicio local
            var gestorDePedidos = new Pedidos.Internos.GestorDePedidos(catalogoService);

            // 3. Simulamos transacciones de negocio
            gestorDePedidos.ProcesarPedido(productoId: 1, cantidad: 2); // Debería tener éxito
            gestorDePedidos.ProcesarPedido(productoId: 2, cantidad: 100); // Debería fallar por stock

            Console.WriteLine("=== FIN DE LA EJECUCIÓN ===");
        }
    }
}