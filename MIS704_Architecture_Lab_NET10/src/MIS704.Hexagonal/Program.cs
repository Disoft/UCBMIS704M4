using System;
using System.Collections.Generic;

namespace ArquitecturaHexagonalDemo
{
    // =========================================================================
    // 1. DOMINIO / CAPA CENTRAL (Lógica de negocio pura)
    // =========================================================================
    public class Pedido
    {
        public Guid Id { get; private set; }
        public string Cliente { get; private set; }
        public decimal Total { get; private set; }
        public string Estado { get; private set; }

        public Pedido(string cliente, decimal total)
        {
            Id = Guid.NewGuid();
            Cliente = cliente ?? throw new ArgumentNullException(nameof(cliente));

            if (total <= 0)
                throw new ArgumentException("El total del pedido debe ser mayor a cero.", nameof(total));

            Total = total;
            Estado = "Creado";
        }

        public void MarcarComoCompletado()
        {
            Estado = "Completado";
        }
    }

    // =========================================================================
    // 2. PUERTOS (Interfaces que definen contratos)
    // =========================================================================

    // Puerto de Entrada (Inbound Port / Use Case Contract)
    public interface ICrearPedidoUseCase
    {
        Pedido Ejecutar(CrearPedidoComando comando);
    }

    // Objeto de transferencia para la entrada (Command)
    public record CrearPedidoComando(string Cliente, decimal Total);

    // Puerto de Salida (Outbound Port / Repository Contract)
    public interface IPedidoRepository
    {
        void Guardar(Pedido pedido);
        Pedido ObtenerPorId(Guid id);
    }

    // =========================================================================
    // 3. APLICACIÓN / CASOS DE USO (Orquestación del Dominio)
    // =================================================*========================
    public class CrearPedidoService : ICrearPedidoUseCase
    {
        private readonly IPedidoRepository _pedidoRepository;

        // Inyección del puerto de salida mediante constructor
        public CrearPedidoService(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public Pedido Ejecutar(CrearPedidoComando comando)
        {
            // 1. Crear la entidad de dominio (aplica reglas de negocio)
            var pedido = new Pedido(comando.Cliente, comando.Total);

            // 2. Modificar estado opcionalmente por reglas de negocio
            pedido.MarcarComoCompletado();

            // 3. Persistir usando el puerto de salida
            _pedidoRepository.Guardar(pedido);

            // 4. Retornar el resultado
            return pedido;
        }
    }

    // =========================================================================
    // 4. ADAPTADORES DE INFRAESTRUCTURA (Detalles técnicos externos)
    // =========================================================================

    // Adaptador de Salida: Implementación de Base de Datos (Simulada en Memoria)
    public class InMemoryPedidoRepository : IPedidoRepository
    {
        private readonly Dictionary<Guid, Pedido> _baseDeDatos = new();

        public void Guardar(Pedido pedido)
        {
            _baseDeDatos[pedido.Id] = pedido;
            Console.WriteLine($"[Adaptador DB] Pedido {pedido.Id} guardado exitosamente en el almacenamiento.");
        }

        public Pedido ObtenerPorId(Guid id)
        {
            return _baseDeDatos.TryGetValue(id, out var pedido) ? pedido : null;
        }
    }

    // Adaptador de Entrada: Controlador de Consola o API (Simulado)
    public class PedidoController
    {
        private readonly ICrearPedidoUseCase _crearPedidoUseCase;

        public PedidoController(ICrearPedidoUseCase crearPedidoUseCase)
        {
            _crearPedidoUseCase = crearPedidoUseCase;
        }

        public void RealizarPedido(string cliente, decimal total)
        {
            Console.WriteLine($"[Adaptador UI/API] Recibiendo solicitud HTTP POST para cliente: {cliente}");

            var comando = new CrearPedidoComando(cliente, total);

            try
            {
                var pedidoCreado = _crearPedidoUseCase.Ejecutar(comando);
                Console.WriteLine($"[Adaptador UI/API] Respuesta 201 Created: Pedido ID {pedidoCreado.Id} - Estado: {pedidoCreado.Estado}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Adaptador UI/API] Error 400 Bad Request: {ex.Message}\n");
            }
        }
    }

    // =========================================================================
    // 5. PROGRAMA PRINCIPAL (Composición y Arranque / Composition Root)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== INICIANDO APLICACIÓN CON ARQUITECTURA HEXAGONAL ===\n");

            // 1. Instanciar los adaptadores de infraestructura
            IPedidoRepository repositoryAdapter = new InMemoryPedidoRepository();

            // 2. Instanciar los casos de uso inyectando los puertos correspondientes
            ICrearPedidoUseCase crearPedidoService = new CrearPedidoService(repositoryAdapter);

            // 3. Instanciar el adaptador de entrada inyectando el caso de uso
            var controllerAdapter = new PedidoController(crearPedidoService);

            // --- SIMULACIÓN DE FLUJOS ---

            // Flujo exitoso
            controllerAdapter.RealizarPedido("Ana Gómez", 450.75m);

            // Flujo con error de validación de negocio en el Dominio
            controllerAdapter.RealizarPedido("Carlos Ruiz", -50.00m);
        }
    }
}