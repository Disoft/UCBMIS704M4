using MIS704.CleanArchitecture.Domain.Entities;

namespace MIS704.CleanArchitecture.Domain.Repositories
{
    public interface IPedidoRepository
    {
        void Guardar(Pedido pedido);
        Pedido? ObtenerPorId(Guid id);
    }
}
