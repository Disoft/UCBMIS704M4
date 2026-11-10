namespace MIS704.Others
{
    using System;

    namespace InformationHidingDemo
    {
        // ==========================================
        // 1. CONTRATO ESTABLE (Expuesto al exterior)
        // ==========================================
        // El consumidor del módulo solo conoce esta interfaz y no depende de 
        // cómo se realizan los cálculos o de qué clases concretas existen.
        public interface ICalculadoraImpuestos
        {
            decimal Calcular(decimal montoBase);
        }

        // ==========================================
        // 2. DECISIONES INTERNAS OCULTAS (Privadas / Internas)
        // ==========================================
        // Esta implementación contiene los algoritmos y reglas de negocio específicas.
        // Al ser 'internal', está oculta fuera del ensamblado o módulo, y puede cambiar 
        // en el futuro sin romper el código del cliente.
        internal class MotorImpuestosRegional2026 : ICalculadoraImpuestos
        {
            public decimal Calcular(decimal montoBase)
            {
                // Detalle de implementación interno (ej. lógica fiscal compleja o sujeta a cambios)
                decimal tasaEstatal = 0.12m;
                decimal tasaMunicipal = 0.03m;

                return montoBase * (tasaEstatal + tasaMunicipal);
            }
        }

        // ==========================================
        // 3. MÓDULO DE ACCESO / FÁBRICA (Encapsula la creación)
        // ==========================================
        public static class ModuloImpuestos
        {
            // El cliente obtiene el servicio a través de este punto de entrada controlado,
            // sin enterarse de qué clase concreta está detrás.
            public static ICalculadoraImpuestos ObtenerCalculadora()
            {
                return new MotorImpuestosRegional2026();
            }
        }

        // ==========================================
        // 4. CONSUMIDOR (Cliente del módulo)
        // ==========================================
        class Program
        {
            static void Main(string[] args)
            {
                // El código cliente únicamente interactúa con el contrato estable (ICalculadoraImpuestos)
                ICalculadoraImpuestos calculadora = ModuloImpuestos.ObtenerCalculadora();

                decimal montoTotal = 1500.00m;
                decimal impuesto = calculadora.Calcular(montoTotal);

                Console.WriteLine($"Monto base: {montoTotal}");
                Console.WriteLine($"Impuesto calculado: {impuesto}");
            }
        }
    }
}
