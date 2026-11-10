using System;

namespace AcoplamientoDemo
{
    // =========================================================================
    // 1. CONTRATO ESTABLE (Interfaz que controla la dirección de la dependencia)
    // =========================================================================
    public interface INotificador
    {
        void Enviar(string mensaje);
    }

    // =========================================================================
    // 2. MÓDULOS DE BAJO NIVEL (Implementaciones concretas)
    // =========================================================================
    public class NotificadorEmail : INotificador
    {
        public void Enviar(string mensaje)
        {
            Console.WriteLine($"[EMAIL] Enviando correo: {mensaje}");
        }
    }

    public class NotificadorSMS : INotificador
    {
        public void Enviar(string mensaje)
        {
            Console.WriteLine($"[SMS] Enviando mensaje de texto: {mensaje}");
        }
    }

    // =========================================================================
    // 3. MÓDULO DE ALTO NIVEL CON BAJO ACOPLAMIENTO (Diseño Correcto)
    // =========================================================================
    // Esta clase depende de la abstracción (INotificador) y no de clases concretas.
    // Esto permite controlar la cantidad, dirección y estabilidad de las dependencias.
    public class GestorDePedidos
    {
        private readonly INotificador _notificador;

        // Inyección de dependencias por constructor: el acoplamiento es débil y controlado.
        public GestorDePedidos(INotificador notificador)
        {
            _notificador = notificador;
        }

        public void ProcesarPedido(string idPedido)
        {
            Console.WriteLine($"Procesando el pedido {idPedido}...");

            // Usamos el contrato estable sin importar qué canal de notificación real exista
            _notificador.Enviar($"Su pedido {idPedido} ha sido procesado con éxito.");
        }
    }

    // =========================================================================
    // 4. PROGRAMA PRINCIPAL (Simulación de uso)
    // =========================================================================
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        // Queremos usar notificaciones por correo electrónico:
    //        INotificador servicioEmail = new NotificadorEmail();

    //        // Inyectamos la dependencia en el gestor de pedidos
    //        GestorDePedidos gestor = new GestorDePedidos(servicioEmail);

    //        gestor.ProcesarPedido("ORD-2026-001");

    //        // Si mañana queremos cambiar a SMS, la clase GestorDePedidos NO se modifica,
    //        // demostrando que el acoplamiento es bajo y los cambios están controlados.
    //        Console.WriteLine("\n--- Cambio de canal a SMS ---");
    //        INotificador servicioSms = new NotificadorSMS();
    //        GestorDePedidos gestorSms = new GestorDePedidos(servicioSms);
    //        gestorSms.ProcesarPedido("ORD-2026-002");
    //    }
    //}
}