
using Xunit;
using Moq;
using System;

namespace MIS704.Testable
{
    // =========================================================================
    // CONTRATO Y DOMINIO (Clases bajo prueba)
    // =========================================================================
    public interface IArancelRepository
    {
        decimal ObtenerCostoBaseMatricula();
        bool EstudiantePoseeBeca(int estudianteId);
    }

    public class CalculadoraMatricula
    {
        private readonly IArancelRepository _arancelRepository;

        public CalculadoraMatricula(IArancelRepository arancelRepository)
        {
            _arancelRepository = arancelRepository ?? throw new ArgumentNullException(nameof(arancelRepository));
        }

        public decimal CalcularImporteFinal(int estudianteId)
        {
            decimal costoBase = _arancelRepository.ObtenerCostoBaseMatricula();
            bool tieneBeca = _arancelRepository.EstudiantePoseeBeca(estudianteId);

            // Regla de negocio: Si tiene beca, aplica un 30% de descuento
            if (tieneBeca)
            {
                return costoBase * 0.70m;
            }

            return costoBase;
        }
    }

    // =========================================================================
    // CLASE DE PRUEBAS UNITARIAS CON XUNIT
    // =========================================================================
    public class CalculadoraMatriculaTests
    {
        [Fact]
        public void CalcularImporteFinal_CuandoEstudianteTieneBeca_DebeAplicarDescuentoCorrectamente()
        {
            // Arrange (Preparar): Creamos un Mock del contrato utilizando Moq
            var repositorioMock = new Mock<IArancelRepository>();

            // Configuramos el comportamiento simulado (Sin tocar SQL Server)
            repositorioMock.Setup(repo => repo.ObtenerCostoBaseMatricula()).Returns(1000.00m);
            repositorioMock.Setup(repo => repo.EstudiantePoseeBeca(It.IsAny<int>())).Returns(true);

            // Inyectamos el mock en la lógica de dominio
            var calculadora = new CalculadoraMatricula(repositorioMock.Object);

            // Act (Actuar): Ejecutamos la regla de negocio
            decimal resultado = calculadora.CalcularImporteFinal(estudianteId: 123);

            // Assert (Afirmar): Validamos que el resultado sea el esperado (1000 con 30% de descuento = 700)
            Assert.Equal(700.00m, resultado);
        }

        [Fact]
        public void CalcularImporteFinal_CuandoEstudianteNoTieneBeca_DebeRetornarCostoBaseCompleto()
        {
            // Arrange (Preparar)
            var repositorioMock = new Mock<IArancelRepository>();

            repositorioMock.Setup(repo => repo.ObtenerCostoBaseMatricula()).Returns(1200.00m);
            repositorioMock.Setup(repo => repo.EstudiantePoseeBeca(It.IsAny<int>())).Returns(false);

            var calculadora = new CalculadoraMatricula(repositorioMock.Object);

            // Act (Actuar)
            decimal resultado = calculadora.CalcularImporteFinal(estudianteId: 456);

            // Assert (Afirmar): Sin beca debe retornar los 1200.00 intactos
            Assert.Equal(1200.00m, resultado);
        }
    }
}