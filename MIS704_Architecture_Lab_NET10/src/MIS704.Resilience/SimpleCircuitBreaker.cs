using System;
using System.Collections.Generic;
using System.Text;

namespace MIS704.Resilience
{
    public class SimpleCircuitBreaker
    {
        private int _fallosConsecutivos = 0;
        private const int LimiteFallos = 3;
        private bool _circuitoAbierto = false;

        public void EjecutarLlamada()
        {
            if (_circuitoAbierto)
            {
                Console.WriteLine("[Circuit Breaker] ABIERTO. Llamada bloqueada rápidamente para proteger el sistema.");
                return;
            }

            try
            {
                // Simular llamada a un servicio externo caído
                throw new Exception("Servicio no disponible.");
            }
            catch
            {
                _fallosConsecutivos++;
                Console.WriteLine($"[Circuit Breaker] Fallo detectado ({_fallosConsecutivos}/{LimiteFallos}).");

                if (_fallosConsecutivos >= LimiteFallos)
                {
                    _circuitoAbierto = true;
                    Console.WriteLine("[Circuit Breaker] ¡Umbral superado! El circuito pasa a estado ABIIERTO.");
                }
            }
        }
    }
}
