namespace MIS704.Resilience
{
    

    internal class Program
    {
        public async Task OperacionConTimeoutAsync()
        {
            // Define un límite de tiempo estricto de 2 segundos
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

            try
            {
                // Simula una operación lenta que toma 5 segundos
                await Task.Delay(TimeSpan.FromSeconds(5), cts.Token);
                Console.WriteLine("Operación completada con éxito.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("[Timeout] La operación tardó demasiado y fue cancelada para liberar recursos.");
            }
        }

        public void OperacionConRetry()
        {
            int intentosMaximos = 3;
            int intento = 0;

            while (intento < intentosMaximos)
            {
                try
                {
                    intento++;
                    // Simular un fallo transitorio en los primeros intentos
                    if (intento < 3) throw new TimeoutException("Fallo temporal de red.");

                    Console.WriteLine($"Operación exitosa en el intento {intento}.");
                    break;
                }
                catch (TimeoutException ex) when (intento < intentosMaximos)
                {
                    Console.WriteLine($"[Retry] Error transitorio ({ex.Message}). Reintentando ({intento}/{intentosMaximos})...");
                    Thread.Sleep(1000); // Espera antes del siguiente intento
                }
            }
        }

        public string ObtenerDatosConFallback()
        {
            try
            {
                // Simular fallo en el servicio o base de datos principal
                throw new Exception("Base de datos principal inaccesible.");
            }
            catch (Exception)
            {
                // Mecanismo Fallback: Degradar la funcionalidad conscientemente
                Console.WriteLine("[Fallback] El servicio principal falló. Retornando datos desde la caché local.");
                return "Datos de respaldo (Caché local)";
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}
