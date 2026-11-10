using System;
using System.Collections.Generic;
using System.Linq;

namespace ArquitecturaDominioUniversidad
{
    // =========================================================================
    // 1. CAPA DE DOMINIO (Lógica de negocio pura, entidades con comportamiento)
    // =========================================================================

    public class Materia
    {
        public string Id { get; private set; }
        public string Nombre { get; private set; }

        private readonly List<string> _prerrequisitos = new();
        public IReadOnlyCollection<string> Prerrequisitos => _prerrequisitos.AsReadOnly();

        public Materia(string id, string nombre, IEnumerable<string> prerrequisitos = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));

            if (prerrequisitos != null)
            {
                _prerrequisitos.AddRange(prerrequisitos);
            }
        }
    }

    public class Estudiante
    {
        public string Id { get; private set; }
        public string Nombre { get; private set; }

        private readonly List<string> _materiasAprobadas = new();
        public IReadOnlyCollection<string> MateriasAprobadas => _materiasAprobadas.AsReadOnly();

        private readonly List<Matricula> _matriculasActivas = new();
        public IReadOnlyCollection<Matricula> MatriculasActivas => _matriculasActivas.AsReadOnly();

        public Estudiante(string id, string nombre)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        }

        public void AprobarMateria(string materiaId)
        {
            if (!_materiasAprobadas.Contains(materiaId))
            {
                _materiasAprobadas.Add(materiaId);
            }
        }

        // COMPORTAMIENTO Y REGLAS DE NEGOCIO ESTRICTAS
        public Matricula InscribirEnMateria(Materia materia)
        {
            if (materia == null) throw new ArgumentNullException(nameof(materia));

            // 1. Regla: Verificar si ya está inscrito
            if (_matriculasActivas.Any(m => m.MateriaId == materia.Id))
            {
                throw new DomainException($"El estudiante ya se encuentra inscrito en la materia '{materia.Nombre}'.");
            }

            // 2. Regla de Dominio Clave: Impedir inscripción si no cumple con los prerrequisitos
            foreach (var prerrequisitoId in materia.Prerrequisitos)
            {
                if (!_materiasAprobadas.Contains(prerrequisitoId))
                {
                    throw new DomainException(
                        $"No se puede matricular en '{materia.Nombre}'. El estudiante no ha aprobado el prerrequisito obligatorio con ID: '{prerrequisitoId}'."
                    );
                }
            }

            // 3. Si se cumplen las reglas, se crea la matrícula de forma válida
            var nuevaMatricula = new Matricula(this.Id, materia.Id);
            _matriculasActivas.Add(nuevaMatricula);

            return nuevaMatricula;
        }
    }

    public class Matricula
    {
        public Guid Id { get; private set; }
        public string EstudianteId { get; private set; }
        public string MateriaId { get; private set; }
        public DateTime FechaInscripcion { get; private set; }
        public string Estado { get; private set; }

        public Matricula(string estudianteId, string materiaId)
        {
            Id = Guid.NewGuid();
            EstudianteId = estudianteId;
            MateriaId = materiaId;
            FechaInscripcion = DateTime.UtcNow;
            Estado = "Activa";
        }
    }

    // Excepción específica del dominio para manejar violaciones de reglas de negocio
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }

    // =========================================================================
    // 2. PUERTOS / CONTRATOS (Interfaces de Repositorio)
    // =========================================================================
    public interface IEstudianteRepository
    {
        Estudiante ObtenerPorId(string id);
        void Guardar(Estudiante estudiante);
    }

    public interface IMateriaRepository
    {
        Materia ObtenerPorId(string id);
    }

    // =========================================================================
    // 3. CAPA DE APLICACIÓN / CASOS DE USO (Orquestadores)
    // =========================================================================
    public class InscribirMateriaService
    {
        private readonly IEstudianteRepository _estudianteRepository;
        private readonly IMateriaRepository _materiaRepository;

        public InscribirMateriaService(IEstudianteRepository estudianteRepository, IMateriaRepository materiaRepository)
        {
            _estudianteRepository = estudianteRepository;
            _materiaRepository = materiaRepository;
        }

        public void Ejecutar(string estudianteId, string materiaId)
        {
            // Obtener entidades de los repositorios
            var estudiante = _estudianteRepository.ObtenerPorId(estudianteId)
                             ?? throw new Exception("Estudiante no encontrado.");

            var materia = _materiaRepository.ObtenerPorId(materiaId)
                          ?? throw new Exception("Materia no encontrada.");

            // Delegar la ejecución y validación estrictamente al dominio
            estudiante.InscribirEnMateria(materia);

            // Persistir cambios
            _estudianteRepository.Guardar(estudiante);

            Console.WriteLine($"[Caso de Uso] Éxito: Estudiante '{estudiante.Nombre}' matriculado en '{materia.Nombre}'.");
        }
    }

    // =========================================================================
    // 4. PROGRAMA PRINCIPAL (Simulación de Pruebas de Comportamiento)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== DEMOSTRACIÓN DE ARQUITECTURA CENTRADA EN EL DOMINIO ===\n");

            // Configurar datos de prueba en memoria
            var materiasData = new Dictionary<string, Materia>
            {
                { "MAT-101", new Materia("MAT-101", "Cálculo I") },
                { "MAT-102", new Materia("MAT-102", "Cálculo II", prerrequisitos: new[] { "MAT-101" }) }
            };

            var estudiante = new Estudiante("EST-001", "Carlos Mendoza");
            // Nota: Carlos aún no ha aprobado Cálculo I.

            var estudianteRepo = new MockEstudianteRepository(estudiante);
            var materiaRepo = new MockMateriaRepository(materiasData);

            var servicioInscripcion = new InscribirMateriaService(estudianteRepo, materiaRepo);

            // -------------------------------------------------------------
            // CASO A: Intento de inscripción sin cumplir prerrequisitos
            // -------------------------------------------------------------
            try
            {
                Console.WriteLine("--- Intento 1: Inscribir en Cálculo II sin Cálculo I ---");
                servicioInscripcion.Ejecutar("EST-001", "MAT-102");
            }
            catch (DomainException ex)
            {
                Console.WriteLine($"[Regla de Negocio Bloqueada]: {ex.Message}\n");
            }

            // -------------------------------------------------------------
            // CASO B: Cumplir el prerrequisito y reintentar
            // -------------------------------------------------------------
            Console.WriteLine("--- Acción: Carlos aprueba 'Cálculo I' ---");
            estudiante.AprobarMateria("MAT-101");

            try
            {
                Console.WriteLine("--- Intento 2: Inscribir en Cálculo II con el prerrequisito aprobado ---");
                servicioInscripcion.Ejecutar("EST-001", "MAT-102");
            }
            catch (DomainException ex)
            {
                Console.WriteLine($"[Error Inesperado]: {ex.Message}");
            }
        }
    }

    // Dobles de prueba (Mocks en memoria) para simular la infraestructura
    public class MockEstudianteRepository : IEstudianteRepository
    {
        private readonly Estudiante _estudiante;
        public MockEstudianteRepository(Estudiante estudiante) => _estudiante = estudiante;
        public Estudiante ObtenerPorId(string id) => _estudiante.Id == id ? _estudiante : null;
        public void Guardar(Estudiante estudiante) { /* Simula persistencia */ }
    }

    public class MockMateriaRepository : IMateriaRepository
    {
        private readonly Dictionary<string, Materia> _materias;
        public MockMateriaRepository(Dictionary<string, Materia> materias) => _materias = materias;
        public Materia ObtenerPorId(string id) => _materias.TryGetValue(id, out var mat) ? mat : null;
    }
}