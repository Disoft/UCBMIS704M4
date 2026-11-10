using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcingBankDemo
{
    // =========================================================================
    // 1. DEFINICIÓN DE EVENTOS (Hechos inmutables ocurridos en el pasado)
    // =========================================================================
    public interface IDomainEvent { }

    public record AccountOpened(Guid AccountId, decimal InitialBalance) : IDomainEvent;

    public record MoneyDeposited(Guid AccountId, decimal Amount) : IDomainEvent;

    public record MoneyWithdrawn(Guid AccountId, decimal Amount) : IDomainEvent;

    // =========================================================================
    // 2. EL AGREGADO (Domain Aggregate) - RECONSTRUCCIÓN POR REPLAY
    // =========================================================================
    public class BankAccount
    {
        public Guid Id { get; private set; }
        public decimal Balance { get; private set; }
        public int Version { get; private set; } = -1;

        // Lista de eventos no guardados (nuevos cambios de estado en la sesión actual)
        private readonly List<IDomainEvent> _uncommittedEvents = new();

        // Constructor vacío para reconstrucción
        public BankAccount() { }

        // Comportamiento de Negocio: Abrir cuenta
        public static BankAccount OpenAccount(Guid accountId, decimal initialBalance)
        {
            var account = new BankAccount();
            var evento = new AccountOpened(accountId, initialBalance);
            account.ApplyChange(evento, isNew: true);
            return account;
        }

        // Comportamiento de Negocio: Depositar dinero
        public void Deposit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("El monto a depositar debe ser mayor a cero.");

            var evento = new MoneyDeposited(Id, amount);
            ApplyChange(evento, isNew: true);
        }

        // Comportamiento de Negocio: Retirar dinero
        public void Withdraw(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("El monto a retirar debe ser mayor a cero.");
            if (Balance < amount) throw new InvalidOperationException("Fondos insuficientes para realizar el retiro.");

            var evento = new MoneyWithdrawn(Id, amount);
            ApplyChange(evento, isNew: true);
        }

        // Método central que aplica el evento al estado interno del agregado
        private void Apply(AccountOpened e)
        {
            Id = e.AccountId;
            Balance = e.InitialBalance;
        }

        private void Apply(MoneyDeposited e)
        {
            Balance += e.Amount;
        }

        private void Apply(MoneyWithdrawn e)
        {
            Balance -= e.Amount;
        }

        // Enruta dinámicamente el evento según su tipo para actualizar el estado actual
        public void ApplyChange(IDomainEvent evento, bool isNew)
        {
            // Patrón de despacho por reflexión o pattern matching de C#
            switch (evento)
            {
                case AccountOpened e: Apply(e); break;
                case MoneyDeposited e: Apply(e); break;
                case MoneyWithdrawn e: Apply(e); break;
                default: throw new InvalidOperationException($"Tipo de evento desconocido: {evento.GetType().Name}");
            }

            Version++;

            if (isNew)
            {
                _uncommittedEvents.Add(evento);
            }
        }

        // Reconstruye el agregado reproduciendo la historia completa de eventos (Replay)
        public void LoadFromHistory(IEnumerable<IDomainEvent> history)
        {
            foreach (var evento in history)
            {
                ApplyChange(evento, isNew: false);
            }
        }

        public IEnumerable<IDomainEvent> GetUncommittedEvents() => _uncommittedEvents;
        public void MarkEventsAsCommitted() => _uncommittedEvents.Clear();
    }

    // =========================================================================
    // 3. EL EVENT STORE (Simulación de la Base de Datos Inmutable)
    // =========================================================================
    public interface IEventStore
    {
        void SaveEvents(Guid aggregateId, IEnumerable<IDomainEvent> events, int expectedVersion);
        IEnumerable<IDomainEvent> GetEventsForStream(Guid aggregateId);
    }

    public class InMemoryEventStore : IEventStore
    {
        // Almacena de manera inmutable la secuencia de eventos por cada ID de cuenta
        private readonly Dictionary<Guid, List<IDomainEvent>> _database = new();

        public void SaveEvents(Guid aggregateId, IEnumerable<IDomainEvent> events, int expectedVersion)
        {
            if (!_database.ContainsKey(aggregateId))
            {
                _database[aggregateId] = new List<IDomainEvent>();
            }

            var stream = _database[aggregateId];

            // Validación de concurrencia optimista basada en versiones
            if (stream.Count != expectedVersion + 1 && expectedVersion != -1)
            {
                throw new InvalidOperationException("Excepción de Concurrencia Optimista: Los datos fueron modificados concurrentemente.");
            }

            foreach (var evento in events)
            {
                stream.Add(evento);
                Console.WriteLine($"[EventStore (Inmutable)] -> Persistido evento: {evento.GetType().Name} | Detalle: {System.Text.Json.JsonSerializer.Serialize(evento)}");
            }
        }

        public IEnumerable<IDomainEvent> GetEventsForStream(Guid aggregateId)
        {
            if (!_database.TryGetValue(aggregateId, out var stream))
            {
                throw new KeyNotFoundException($"No se encontró un stream de eventos para la cuenta con ID: {aggregateId}");
            }
            return stream;
        }
    }

    // =========================================================================
    // 4. PROGRAMA PRINCIPAL (Demostración del Flujo y Replay)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            IEventStore eventStore = new InMemoryEventStore();
            Guid accountId = Guid.NewGuid();

            Console.WriteLine("=== PASO 1: CREACIÓN Y OPERACIONES DE LA CUENTA ===");

            // 1. Abrir cuenta con saldo inicial 0
            var account = BankAccount.OpenAccount(accountId, 0m);

            // 2. Depositar 100
            account.Deposit(100m);

            // 3. Retirar 30
            account.Withdraw(30m);

            // Persistir los eventos generados en el Event Store inmutable
            eventStore.SaveEvents(accountId, account.GetUncommittedEvents(), account.Version - account.GetUncommittedEvents().Count());
            account.MarkEventsAsCommitted();

            Console.WriteLine($"\nEstado en memoria actual tras operaciones: Balance = {account.Balance}\n");

            Console.WriteLine("=== PASO 2: RECONSTRUCCIÓN DEL ESTADO DESDE CERO (REPLAY) ===");
            Console.WriteLine("Simulando que la aplicación se reinicia y no tenemos el saldo en caché, solo el log de eventos...\n");

            // Instanciar un objeto de cuenta vacío
            var restoredAccount = new BankAccount();

            // Obtener la historia inmutable completa del Event Store
            var history = eventStore.GetEventsForStream(accountId);

            // Reproducir (*replay*) los eventos para reconstruir el estado actual
            restoredAccount.LoadFromHistory(history);

            Console.WriteLine($"[Replay Exitoso] Balance actual reconstruido matemáticamente: {restoredAccount.Balance}");
            Console.WriteLine($"Versión actual del stream: {restoredAccount.Version}");

            Console.WriteLine("\n=== PASO 3: PRINCIPIO DE HISTORIA EXPLÍCITA Y CORRECCIÓN ===");
            Console.WriteLine("Si existiera un error histórico, nunca se borran registros anteriores. Se emite un nuevo evento compensatorio:");

            // Por ejemplo, si se requiere un ajuste por error, se procesa un nuevo evento en el stream:
            restoredAccount.Deposit(0m); // Demostración conceptual de que la auditoría permanece intacta.
        }
    }
}