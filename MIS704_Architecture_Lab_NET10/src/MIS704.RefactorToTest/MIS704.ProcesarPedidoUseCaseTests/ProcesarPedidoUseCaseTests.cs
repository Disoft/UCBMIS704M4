namespace MIS704.ProcesarPedidoUseCaseTests
{
    using MIS704.CleanArchitecture;
    using Moq;
    using Xunit;


    public class ProcesarPedidoUseCaseTests
    {
        [Fact]
        public void Ejecutar_CuandoElPagoEsExitoso_DebeGuardarElPedidoYRetornarTrue()
        {
            // Arrange (Preparar los Fakes / Mocks)
            var repositorioMock = new Mock<IPedidoRepository>();
            var proveedorPagoMock = new Mock<IProveedorPagoService>();

            // Configuramos el comportamiento simulado sin tocar infraestructura real
            proveedorPagoMock.Setup(p => p.ProcesarCobro(It.IsAny<decimal>())).Returns(true);
            repositorioMock.Setup(r => r.Guardar(It.IsAny<string>(), It.IsAny<decimal>()));

            // Inyectamos los mocks en el caso de uso
            var casoDeUso = new ProcesarPedidoUseCase(repositorioMock.Object, proveedorPagoMock.Object);

            // Act (Actuar)
            bool resultado = casoDeUso.Ejecutar("María Gómez", 250.00m);

            // Assert (Afirmar)
            Assert.True(resultado);

            // Verificamos que las interacciones ocurrieron exactamente como se esperaba
            proveedorPagoMock.Verify(p => p.ProcesarCobro(250.00m), Times.Once);
            repositorioMock.Verify(r => r.Guardar("María Gómez", 250.00m), Times.Once);
        }
    }
}