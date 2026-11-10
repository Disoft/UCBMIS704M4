using System;
using System.Collections.Generic;
using System.Threading;

namespace KeyValueStorageDemo
{
    // =========================================================================
    // 1. MODELO DE DATOS Y ESTRUCTURA CLAVE-VALOR
    // =========================================================================
    public class CacheItem
    {
        public object Value { get; set; }
        public DateTime ExpirationTime { get; set; }

        public bool IsExpired => DateTime.UtcNow > ExpirationTime;
    }

    // =========================================================================
    // 2. MOTOR DE ALMACENAMIENTO CLAVE-VALOR CON SOPORTE DE TTL
    // =========================================================================
    public class KeyValueStore
    {
        private readonly Dictionary<string, CacheItem> _store = new();
        private readonly object _lock = new();

        // Operación PUT / Set con TTL opcional
        public void Set(string key, object value, TimeSpan? ttl = null)
        {
            lock (_lock)
            {
                var expiration = ttl.HasValue
                    ? DateTime.UtcNow.Add(ttl.Value)
                    : DateTime.UtcNow.AddYears(100); // Valor persistente por defecto

                _store[key] = new CacheItem
                {
                    Value = value,
                    ExpirationTime = expiration
                };
            }
        }

        // Operación GET (Lookup muy eficiente por clave)
        public T Get<T>(string key)
        {
            lock (_lock)
            {
                if (!_store.TryGetValue(key, out var item))
                {
                    return default;
                }

                // Verificar si el TTL ha expirado
                if (item.IsExpired)
                {
                    _store.Remove(key);
                    Console.WriteLine($"[Store] La clave '{key}' ha expirado y fue eliminada (TTL).");
                    return default;
                }

                return (T)item.Value;
            }
        }

        // Operación de Contador Atómico / Incremento
        public long Increment(string key, long value = 1)
        {
            lock (_lock)
            {
                long currentVal = 0;
                if (_store.TryGetValue(key, out var item) && !item.IsExpired)
                {
                    currentVal = Convert.ToInt64(item.Value);
                }

                currentVal += value;
                _store[key] = new CacheItem
                {
                    Value = currentVal,
                    ExpirationTime = DateTime.UtcNow.AddYears(100)
                };

                return currentVal;
            }
        }
    }

    // =========================================================================
    // 3. PROGRAMA PRINCIPAL / CASOS DE USO (Caché, Sesiones, Contadores)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== INICIANDO DEMOSTRACIÓN DE KEY-VALUE STORE EN C# ===\n");
            var kvStore = new KeyValueStore();

            // --- CASO DE USO 1: Caché de Datos Frecuentes ---
            Console.WriteLine("--- 1. Caso de Uso: Caché ---");
            kvStore.Set("product:xyz:details", "{ \"nombre\": \"Laptop\", \"precio\": 1200 }", TimeSpan.FromMinutes(10));
            var cachedProduct = kvStore.Get<string>("product:xyz:details");
            Console.WriteLine($"[Lookup Caché] product:xyz:details -> {cachedProduct}\n");

            // --- CASO DE USO 2: Gestión de Sesiones con TTL ---
            Console.WriteLine("--- 2. Caso de Uso: Sesiones con TTL ---");
            string sessionKey = "session:user:123";
            var sessionValue = "{ \"userId\": 123, \"role\": \"Student\" }";

            // Asignamos un TTL corto de 2 segundos para la demostración
            kvStore.Set(sessionKey, sessionValue, TimeSpan.FromSeconds(2));

            Console.WriteLine($"[Sesión Creada] {sessionKey} leída inmediatamente: {kvStore.Get<string>(sessionKey)}");

            Console.WriteLine("[Espera] Esperando 3 segundos para que el TTL expire...");
            Thread.Sleep(3000);

            // Intento de lectura tras expirar el TTL
            var expiredSession = kvStore.Get<string>(sessionKey);
            Console.WriteLine($"[Sesión Expirada] {sessionKey} tras TTL: {(expiredSession == null ? "NULL (Eliminado automáticamente)" : expiredSession)}\n");

            // --- CASO DE USO 3: Contadores en Tiempo Real ---
            Console.WriteLine("--- 3. Caso de Uso: Contadores ---");
            string counterKey = "stats:page_views";
            Console.WriteLine($"Incremento 1 -> Valor actual: {kvStore.Increment(counterKey)}");
            Console.WriteLine($"Incremento 2 -> Valor actual: {kvStore.Increment(counterKey)}");
            Console.WriteLine($"Incremento por 5 -> Valor actual: {kvStore.Increment(counterKey, 5)}\n");

            // --- NOTA SOBRE CONSISTENCIA E INVALIDACIÓN ---
            Console.WriteLine("=== NOTA ARQUITECTÓNICA == /");
            Console.WriteLine("Nota: El TTL limita la vigencia temporal del dato para liberar memoria de forma automática,");
            Console.WriteLine("pero no resuelve por sí solo los problemas complejos de invalidación inmediata o consistencia global");
            Console.WriteLine("cuando los datos principales cambian en la base de datos relacional subyacente.");
        }
    }
}