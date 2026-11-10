using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventDrivenDemo
{
    // =========================================================================
    // 1. EVENTO (El hecho ocurrido en el pasado)
    // =========================================================================
    public record PedidoCreadoEvent(Guid PedidoId, string Cliente, decimal Total, DateTime FechaCreacion);

    // =========================================================================
    // 2. CONTRATOS DEL BUS DE EVENTOS (Broker simulado)
    // =========================================================================
    public interface IEventBus
    {
        void Suscribir<TEvent>(Func<TEvent, Task> manejador);
        Task PublicarAsync<TEvent>(TEvent evento);
    }

    // Implementación simple de un Event Bus en memoria (Simula un Broker tipo RabbitMQ / Kafka)
    public class InMemoryEventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _suscriptores = new();

        public void Suscribir<TEvent>(Func<TEvent, Task> manejador)
        {
            var tipoEvento = typeof(TEvent);
            if (!_suscriptores.ContainsKey(tipoEvento))
            {
                _suscriptores[tipoEvento] = new List<Delegate>();
            }
            _suscriptores[tipoEvento].Add(manejador);
        }

        public async Task PublicarAsync<TEvent>(TEvent evento)
        {
            var tipoEvento = typeof(TEvent);
            if (_suscriptores.TryGetValue(tipoEvento, out var listaManejadores))
            {
                foreach (var manejador in listaManejadores)
                {
                    if (manejador is Func<TEvent, Task> accion)
                    {
                        // Ejecución asíncrona de cada consumidor interesado en el evento
                        await accion(evento);
                    }
                }
            }
        }
    }

    // =========================================================================
    // 3. CONSUMIDORES / MANEJADORES DE EVENTOS (Reactivos)
    // =========================================================================
    public class EnviarCorreoConsumer
    {
        public Task HandleAsync(PedidoCreadoEvent evento)
        {
            Console.WriteLine($"[Consumidor Correo] -> ¡Evento recibido! Enviando factura al cliente '{evento.Cliente}' por el monto de ${evento.Total}.");
            return Task.CompletedTask;
        }
    }

    public class ActualizarInventarioConsumer
    {
        public Task HandleAsync(PedidoCreadoEvent evento)
        {
            Console.WriteLine($"[Consumidor Inventario] -> ¡Evento recibido! Descontando stock para el pedido ID: {evento.PedidoId}.");
            return Task.CompletedTask;
        }
    }

    // =========================================================================
    // 4. PRODUCTOR DE EVENTOS
    // =========================================================================
    public class ProcesarPedidoService
    {
        private readonly IEventBus _eventBus;

        public ProcesarPedidoService(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async Task CrearPedidoAsync(string cliente, decimal total)
        {
            Console.WriteLine($"[Productor] Procesando y guardando pedido para {cliente}...");

            var pedidoId = Guid.NewGuid();

            // Creamos el evento que representa el hecho cumplido
            var evento = new PedidoCreadoEvent(pedidoId, cliente, total, DateTime.Now);

            Console.WriteLine($"[Productor] Publicando evento 'PedidoCreadoEvent' al Broker...\n");

            // Publicamos el evento sin conocer quién ni cuántos lo van a consumir (Desacoplamiento)
            await _eventBus.PublicarAsync(evento);
        }
    }

    // =========================================================================
    // 5. PROGRAMA PRINCIPAL (Composición y Arranque)
    // =========================================================================
    class Program
    {
        static async Task Main(string[] args)
        {
            // 1. Configuramos el Broker de Eventos central
            IEventBus eventBus = new InMemoryEventBus();

            // 2. Instanciamos los consumidores
            var correoConsumer = new EnviarCorreoConsumer();
            var inventarioConsumer = new ActualizarInventarioConsumer();

            // 3. Registramos (suscribimos) los consumidores al Bus para escuchar el evento PedidoCreadoEvent
            eventBus.Suscribir<PedidoCreadoEvent>(correoConsumer.HandleAsync);
            eventBus.Suscribir<PedidoCreadoEvent>(inventarioConsumer.HandleAsync);

            // 4. Inicializamos el productor inyectándole el Bus
            var servicioPedidos = new ProcesarPedidoService(eventBus);

            // 5. Simulamos la acción del usuario creando un pedido
            Console.WriteLine("=== INICIANDO FLUJO DE ARQUITECTURA EVENT-DRIVEN ===");
            await servicioPedidos.CrearPedidoAsync("Ana Gómez", 150.50m);
        }
    }
}