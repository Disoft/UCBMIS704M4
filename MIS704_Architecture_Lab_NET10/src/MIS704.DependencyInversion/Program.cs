using System;

namespace InversionDeDependenciasDemo
{
    // =========================================================================
    // 1. ABSTRACCIÓN (Definida por el consumidor según su necesidad)
    // =========================================================================
    public interface IPasarelaDePago
    {
        void ProcesarPago(decimal monto);
    }

    // =========================================================================
    // 2. DETALLES CONCRETOS (Infraestructura / Proveedores de bajo nivel)
    // =========================================================================
    // Implementan la interfaz, invirtiendo la dirección tradicional de la dependencia.
    public class StripePaymentService : IPasarelaDePago
    {
        public void ProcesarPago(decimal monto)
        {
            Console.WriteLine($"[Stripe] Procesando pago seguro de ${monto} mediante API externa.");
        }
    }

    public class PayPalPaymentService : IPasarelaDePago
    {
        public void ProcesarPago(decimal monto)
        {
            Console.WriteLine($"[PayPal] Procesando pago de ${monto} mediante pasarela PayPal.");
        }
    }

    // =========================================================================
    // 3. POLÍTICA DE ALTO NIVEL (Lógica de Negocio / Consumidor)
    // =========================================================================
    // Esta clase NO conoce ni depende de clases concretas (Stripe o PayPal).
    // Depende exclusivamente del contrato abstracto (IPasarelaDePago).
    public class GestorDeVentas
    {
        private readonly IPasarelaDePago _pasarelaDePago;

        // Inyección de Dependencias (DI): Mecanismo para suministrar el detalle por constructor
        public GestorDeVentas(IPasarelaDePago pasarelaDePago)
        {
            _pasarelaDePago = pasarelaDePago ?? throw new ArgumentNullException(nameof(pasarelaDePago));
        }

        public void FinalizarCompra(decimal total)
        {
            Console.WriteLine("Ejecutando reglas de negocio del pedido...");

            // Invocación a través de la abstracción
            _pasarelaDePago.ProcesarPago(total);

            Console.WriteLine("Compra finalizada con éxito.\n");
        }
    }

    // =========================================================================
    // 4. PROGRAMA PRINCIPAL (Composition Root)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            // Caso A: El negocio opera utilizando Stripe
            IPasarelaDePago pasarelaStripe = new StripePaymentService();
            GestorDeVentas ventaStripe = new GestorDeVentas(pasarelaStripe);
            ventaStripe.FinalizarCompra(150.00m);

            // Caso B: Cambiamos a PayPal sin modificar una sola línea de 'GestorDeVentas'
            IPasarelaDePago pasarelaPayPal = new PayPalPaymentService();
            GestorDeVentas ventaPayPal = new GestorDeVentas(pasarelaPayPal);
            ventaPayPal.FinalizarCompra(320.50m);
        }
    }
}