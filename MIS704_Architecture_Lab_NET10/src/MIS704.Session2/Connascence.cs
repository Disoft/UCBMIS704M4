using System;

namespace ConnascenceDemo
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    Console.WriteLine("=== DEMOSTRACIÓN DE TIPOS DE CONNASCENCE ===\n");

        //    // =====================================================================
        //    // 4. CONNASCENCE DE POSICIÓN (Position)
        //    // =====================================================================
        //    // El orden de los parámetros es crítico. Cambiar el orden en la definición
        //    // obliga a cambiarlo en todas las llamadas de forma estricta.
        //    ProcesarUsuarioPosicional("Ana Pérez", 28, "Administrador");

        //    // =====================================================================
        //    // 3. CONNASCENCE DE SIGNIFICADO / VALORES (Meaning / Values)
        //    // =====================================================================
        //    // Dependencia de un número mágico (1 = Activo, 0 = Inactivo). 
        //    // Si el significado o valor numérico cambia, todos los puntos deben ajustarse.
        //    ManejarEstadoUsuario(1);

        //    // =====================================================================
        //    // 1. CONNASCENCE DE NOMBRE (Name) & 2. CONNASCENCE DE TIPO (Type)
        //    // =====================================================================
        //    var servicio = new FacturacionServicio();

        //    // Nombre: El método se llama 'CalcularImpuesto'. Si cambia el nombre, 
        //    // la llamada debe renombrarse.
        //    // Tipo: Los argumentos y el valor de retorno son 'decimal'. Si cambian a 'double',
        //    // el código cliente debe cambiar su tipo.
        //    decimal total = servicio.CalcularImpuesto(1000.00m, 0.15m);
        //    Console.WriteLine($"[Nombre y Tipo] Impuesto calculado: {total}");

        //    // =====================================================================
        //    // 5. CONNASCENCE DE ALGORITMO (Algorithm)
        //    // =====================================================================
        //    // Dos partes del sistema deben ejecutar exactamente la misma lógica matemática 
        //    // o de transformación para que el resultado sea compatible.
        //    string tokenOriginal = "seguridad2026";
        //    string tokenGenerado = servicio.GenerarHashFirma(tokenOriginal);

        //    // Simulación de validación externa que usa el mismo algoritmo
        //    bool esValido = ValidadorExterno.VerificarHashFirma(tokenOriginal, tokenGenerado);
        //    Console.WriteLine($"[Algoritmo] ¿El token coincide con el algoritmo esperado?: {esValido}");
        //}

        // --- Método para Connascence de Posición ---
        static void ProcesarUsuarioPosicional(string nombre, int edad, string rol)
        {
            Console.WriteLine($"[Posición] Procesando -> Nombre: {nombre}, Edad: {edad}, Rol: {rol}");
        }

        // --- Método para Connascence de Significado ---
        static void ManejarEstadoUsuario(int codigoEstado)
        {
            // El valor '1' tiene un significado contractual compartido
            if (codigoEstado == 1)
            {
                Console.WriteLine("[Significado] Estado: Usuario ACTIVO (Basado en convención numérica).");
            }
            else
            {
                Console.WriteLine("[Significado] Estado: Usuario INACTIVO.");
            }
        }
    }

    public class FacturacionServicio
    {
        // 1. Connascence de Nombre: 'CalcularImpuesto'
        // 2. Connascence de Tipo: Parámetros 'decimal' y retorno 'decimal'
        public decimal CalcularImpuesto(decimal montoBase, decimal tasa)
        {
            return montoBase * tasa;
        }

        // 5. Connascence de Algoritmo: Lógica de transformación específica
        public string GenerarHashFirma(string input)
        {
            // Algoritmo estricto: Inversión de caracteres combinada con mayúsculas
            char[] array = input.ToCharArray();
            Array.Reverse(array);
            return new string(array).ToUpper();
        }
    }

    // Otro componente separado que comparte la Connascence de Algoritmo
    public static class ValidadorExterno
    {
        public static bool VerificarHashFirma(string inputOriginal, string hashAComparar)
        {
            // Debe replicar EXACTAMENTE el mismo algoritmo de FacturacionServicio.
            // Si el algoritmo cambia en uno y no en el otro, el sistema falla.
            char[] array = inputOriginal.ToCharArray();
            Array.Reverse(array);
            string hashCalculado = new string(array).ToUpper();

            return hashCalculado == hashAComparar;
        }
    }
}