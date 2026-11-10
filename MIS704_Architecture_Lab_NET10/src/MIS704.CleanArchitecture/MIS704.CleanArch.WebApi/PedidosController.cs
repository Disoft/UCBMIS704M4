using Microsoft.AspNetCore.Mvc;
using MIS704.CleanArch.Application.UseCases.CrearPedido;

namespace MIS704.CleanArch.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly CrearPedidoHandler _crearPedidoHandler;

        public PedidosController(CrearPedidoHandler crearPedidoHandler)
        {
            _crearPedidoHandler = crearPedidoHandler;
        }

        [HttpPost]
        public IActionResult Crear([FromBody] CrearPedidoCommand command)
        {
            try
            {
                var id = _crearPedidoHandler.Ejecutar(command);
                return Ok(new { Mensaje = "Pedido creado correctamente", Id = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

    }
}
