using MIS704.CleanArchitecture.Domain.Entities;
using MIS704.CleanArchitecture.Domain.Repositories;

namespace MIS704.CleanArch.Infrastructure.Persistence
{
    // Implementación concreta del repositorio definido en el Dominio
    public class SqlPedidoRepository : IPedidoRepository
    {
        private readonly Dictionary<Guid, Pedido> _baseDeDatosSimulada = new();

        public void Guardar(Pedido pedido)
        {
            _baseDeDatosSimulada[pedido.Id] = pedido;
            Console.WriteLine($"[Infraestructura] Pedido {pedido.Id} guardado en SQL Server exitosamente.");
        }

        public Pedido? ObtenerPorId(Guid id)
        {
            return _baseDeDatosSimulada.TryGetValue(id, out var pedido) ? pedido : null;
        }
    }
}