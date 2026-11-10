namespace MIS704.CleanArchitecture
{
    using System;

    // =========================================================================
    // CONTRATOS (PUERTOS)
    // =========================================================================
    public interface IPedidoRepository
    {
        void Guardar(string cliente, decimal total);
    }

    public interface IProveedorPagoService
    {
        bool ProcesarCobro(decimal monto);
    }

    // =========================================================================
    // CASO DE USO (Lógica de negocio aislada)
    // =========================================================================
    public class ProcesarPedidoUseCase
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IProveedorPagoService _proveedorPagoService;

        // Las dependencias se inyectan a través de interfaces
        public ProcesarPedidoUseCase(IPedidoRepository pedidoRepository, IProveedorPagoService proveedorPagoService)
        {
            _pedidoRepository = pedidoRepository ?? throw new ArgumentNullException(nameof(pedidoRepository));
            _proveedorPagoService = proveedorPagoService ?? throw new ArgumentNullException(nameof(proveedorPagoService));
        }

        public bool Ejecutar(string cliente, decimal total)
        {
            // 1. Ejecutar regla de negocio usando el puerto de pago
            bool pagoAprobado = _proveedorPagoService.ProcesarCobro(total);
            if (!pagoAprobado)
            {
                throw new InvalidOperationException("El pago no pudo ser procesado.");
            }

            // 2. Persistir utilizando el puerto de repositorio
            _pedidoRepository.Guardar(cliente, total);

            return true;
        }
    }
}