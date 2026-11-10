namespace MIS704.CouplingApp
{
    using System;

    public class PedidoController
    {
        public string ProcesarPedido(string cliente, decimal total)
        {
            // 1. El controlador crea directamente el cliente de base de datos
            var dbClient = new SqlDatabaseClient("Server=mi-servidor;Database=tienda;");
            dbClient.EjecutarSql($"INSERT INTO Pedidos (Cliente, Total) VALUES ('{cliente}', {total})");

            // 2. El controlador llama directamente a un proveedor externo de pagos
            var stripeClient = new StripeDirectProvider("api_key_secreta");
            bool pagoExitoso = stripeClient.Cobrar(total);

            if (!pagoExitoso)
            {
                throw new Exception("El pago falló.");
            }

            return "Pedido procesado con éxito.";
        }
    }

    // Clases simuladas de infraestructura real
    public class SqlDatabaseClient { public SqlDatabaseClient(string conn) { } public void EjecutarSql(string sql) { } }
    public class StripeDirectProvider { public StripeDirectProvider(string apiKey) { } public bool Cobrar(decimal monto) => true; }
}
