namespace MIS704.CleanArch.Application.UseCases.CrearPedido
{
    public class CrearPedidoCommand
    {
        public required string Cliente { get; set; }
        public decimal Total { get; set; }
    }
}