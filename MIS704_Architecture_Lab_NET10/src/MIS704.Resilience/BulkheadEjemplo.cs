using System;
using System.Collections.Generic;
using System.Text;

namespace MIS704.Resilience
{
    public class BulkheadEjemplo
    {
        // Limita el acceso concurrente a un máximo de 2 hilos para este recurso específico
        private readonly SemaphoreSlim _semaforo = new SemaphoreSlim(2, 2);

        public async Task ProcesarPeticionAsync(int idCliente)
        {
            // Intenta adquirir un espacio de inmediato (timeout de 0)
            if (!await _semaforo.WaitAsync(0))
            {
                Console.WriteLine($"[Bulkhead] Recursos agotados. Petición {idCliente} rechazada para aislar el sistema.");
                return;
            }

            try
            {
                Console.WriteLine($"[Bulkhead] Petición {idCliente} en ejecución...");
                await Task.Delay(1000); // Simular procesamiento
            }
            finally
            {
                _semaforo.Release(); // Libera el espacio asignado
            }
        }
    }
}
