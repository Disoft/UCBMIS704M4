using System;
using System.Collections.Generic;

namespace OnionArchitectureDemo
{
    // =========================================================================
    // 1. CAPA DE DOMINIO (Core / Enterprise Logic)
    // El centro exacto de la cebolla. No depende de ninguna otra capa.
    // =========================================================================
    public class Curso
    {
        public Guid Id { get; private set; }
        public string Titulo { get; private set; }
        public int CuposMaximos { get; private set; }

        public Curso(string titulo, int cuposMaximos)
        {
            Id = Guid.NewGuid();
            Titulo = !string.IsNullOrWhiteSpace(titulo)
                ? titulo
                : throw new ArgumentException("El título del curso es obligatorio.", nameof(titulo));

            CuposMaximos = cuposMaximos > 0
                ? cuposMaximos
                : throw new ArgumentException("Los cupos máximos deben ser mayores a cero.", nameof(cuposMaximos));
        }
    }

    // =========================================================================
    // 2. CAPA DE APLICACIÓN (Interfaces / Contratos y Casos de Uso)
    // Define los contratos (puertos) que la infraestructura debe implementar.
    // =========================================================================

    // Contrato de Repositorio (Puerto de Salida)
    public interface ICursoRepository
    {
        void Registrar(Curso curso);
        Curso ObtenerPorId(Guid id);
    }

    // Servicio de Aplicación / Caso de Uso
    public class RegistrarCursoService
    {
        private readonly ICursoRepository _cursoRepository;

        // Inyección de la abstracción del núcleo
        public RegistrarCursoService(ICursoRepository cursoRepository)
        {
            _cursoRepository = cursoRepository;
        }

        public Curso Ejecutar(string titulo, int cupos)
        {
            // Aplicar reglas del dominio
            var curso = new Curso(titulo, cupos);

            // Persistir mediante la interfaz interna
            _cursoRepository.Registrar(curso);

            return curso;
        }
    }

    // =========================================================================
    // 3. CAPA DE INFRAESTRUCTURA (Detalles externos y Persistencia)
    // Implementa las interfaces definidas en el núcleo interno.
    // =========================================================================
    public class SqlCursoRepository : ICursoRepository
    {
        private readonly Dictionary<Guid, Curso> _baseDeDatosSimulada = new();

        public void Registrar(Curso curso)
        {
            _baseDeDatosSimulada[curso.Id] = curso;
            Console.WriteLine($"[Infraestructura - SQL DB] Curso '{curso.Titulo}' guardado exitosamente.");
        }

        public Curso ObtenerPorId(Guid id)
        {
            return _baseDeDatosSimulada.TryGetValue(id, out var curso) ? curso : null;
        }
    }

    // =========================================================================
    // 4. CAPA DE PRESENTACIÓN / ENTRADA (UI / API / Composition Root)
    // Orquesta el arranque y recibe las solicitudes externas.
    // =========================================================================
    public class CursoController
    {
        private readonly RegistrarCursoService _registrarCursoService;

        public CursoController(RegistrarCursoService registrarCursoService)
        {
            _registrarCursoService = registrarCursoService;
        }

        public void CrearNuevoCurso(string titulo, int cupos)
        {
            Console.WriteLine($"[Presentación] Solicitando creación del curso: {titulo}");
            try
            {
                var cursoCreado = _registrarCursoService.Ejecutar(titulo, cupos);
                Console.WriteLine($"[Presentación] Respuesta Exitosa: ID {cursoCreado.Id} - Cupos: {cursoCreado.CuposMaximos}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Presentación] Error de validación: {ex.Message}\n");
            }
        }
    }

    // =========================================================================
    // PROGRAMA PRINCIPAL (Composition Root)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== INICIANDO ARQUITECTURA DE CEBOLLA (ONION) EN C# ===\n");

            // 1. Inicializar la infraestructura externa
            ICursoRepository repository = new SqlCursoRepository();

            // 2. Inicializar los servicios de aplicación inyectando las abstracciones
            var registrarService = new RegistrarCursoService(repository);

            // 3. Inicializar la capa de presentación inyectando los casos de uso
            var controller = new CursoController(registrarService);

            // --- CASOS DE PRUEBA ---
            // Flujo exitoso
            controller.CrearNuevoCurso("Arquitectura de Software Avanzada", 30);

            // Flujo con error de validación de negocio en el Dominio
            controller.CrearNuevoCurso("", -5);
        }
    }
}