using MIS704.CleanArchitecture.Domain.Entities;
using MIS704.CleanArchitecture.Domain.Repositories;

namespace MIS704.CleanArch.Application.UseCases.CrearPedido
{
    public class CrearPedidoHandler
    {
        private readonly IPedidoRepository _pedidoRepository;

        // Inyección de la abstracción del dominio (DIP)
        public CrearPedidoHandler(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public Guid Ejecutar(CrearPedidoCommand command)
        {
            // Aplicar regla de negocio del dominio
            var pedido = new Pedido(command.Cliente, command.Total);

            // Persistir usando el puerto del dominio
            _pedidoRepository.Guardar(pedido);

            return pedido.Id;
        }
    }
}
