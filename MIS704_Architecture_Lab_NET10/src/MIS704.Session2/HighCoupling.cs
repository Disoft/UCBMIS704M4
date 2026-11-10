using System;

namespace AltoAcoplamientoDemo
{
    // =========================================================================
    // 1. CLASE CONCRETA DE CORREO (Dependencia directa y rígida)
    // =========================================================================
    public class ServidorEmailDirecto
    {
        public void ConectarSMTP()
        {
            Console.WriteLine("Conectando al servidor SMTP externo...");
        }

        public void EnviarCorreo(string destinatario, string contenido)
        {
            Console.WriteLine($"Enviando Email a {destinatario}: {contenido}");
        }
    }

    // =========================================================================
    // 2. MÓDULO DE ALTO NIVEL CON ALTO ACOPLAMIENTO
    // =========================================================================
    public class GestorDeFacturacion
    {
        // ALTO ACOPLAMIENTO 1: Dependencia directa de una clase concreta ('new ServidorEmailDirecto')
        // No hay interfaces de por medio. Si ServidorEmailDirecto cambia sus métodos,
        // esta clase se rompe inmediatamente.
        private readonly ServidorEmailDirecto _emailService = new ServidorEmailDirecto();

        public void ProcesarFactura(string cliente, decimal monto)
        {
            Console.WriteLine($"Procesando factura para {cliente} por un monto de ${monto}.");

            // ALTO ACOPLAMIENTO 2: Conexión rígida a detalles de bajo nivel dentro de la regla de negocio
            _emailService.ConectarSMTP();
            _emailService.EnviarCorreo(cliente, "Su factura ha sido generada con éxito.");
        }
    }

    // =========================================================================
    // 3. PROGRAMA PRINCIPAL
    // =========================================================================
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        GestorDeFacturacion facturacion = new GestorDeFacturacion();
    //        facturacion.ProcesarFactura("Ana Pérez", 450.00m);
    //    }
    //}
}