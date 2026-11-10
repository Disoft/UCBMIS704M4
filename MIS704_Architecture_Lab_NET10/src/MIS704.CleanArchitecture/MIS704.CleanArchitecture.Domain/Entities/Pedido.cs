namespace MIS704.CleanArchitecture.Domain.Entities
{
    public class Pedido
    {
        public Guid Id { get; private set; }
        public string Cliente { get; private set; }
        public decimal Total { get; private set; }

        public Pedido(string cliente, decimal total)
        {
            Id = Guid.NewGuid();
            Cliente = !string.IsNullOrWhiteSpace(cliente)
                ? cliente
                : throw new ArgumentException("El cliente es obligatorio.");

            Total = total > 0
                ? total
                : throw new ArgumentException("El total del pedido debe ser mayor a cero.");
        }
    }
}
