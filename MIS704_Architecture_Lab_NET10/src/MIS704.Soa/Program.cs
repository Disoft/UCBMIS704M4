using SoaUniversidadDemo;
using System;
using System.Collections.Generic;

namespace SoaUniversidadDemo
{
    // =========================================================================
    // 1. CONTRATOS DE SERVICIO (Interfaces explícitas de capacidad empresarial)
    // =========================================================================
    public interface IAcademicService
    {
        bool ValidarEstudianteMatriculado(string codigoEstudiante, string codigoCurso);
    }

    public interface IFinancialService
    {
        void RegistrarPagoMatricula(string codigoEstudiante, decimal monto);
    }

    public interface IHumanResourcesService
    {
        void RegistrarAsignacionDocente(string codigoCurso, string idDocente);
    }

    // =========================================================================
    // 2. IMPLEMENTACIONES DE SERVICIOS HETEROGÉNEOS (Sistemas independientes)
    // =========================================================================

    // Simula el Sistema Académico (Ej. Desarrollado en tecnología Legacy PHP o similar)
    public class AcademicSystemAdapter : IAcademicService
    {
        public bool ValidarEstudianteMatriculado(string codigoEstudiante, string codigoCurso)
        {
            Console.WriteLine($"[Sistema Académico] Validando estatus del estudiante {codigoEstudiante} para el curso {codigoCurso}...");
            // Lógica simulada de validación en base de datos académica
            return true;
        }
    }

    // Simula el Sistema Financiero (Ej. Desarrollado en Java / .NET con base de datos propia)
    public class FinancialSystemAdapter : IFinancialService
    {
        public void RegistrarPagoMatricula(string codigoEstudiante, decimal monto)
        {
            Console.WriteLine($"[Sistema Financiero] Procesando pago de matrícula por ${monto} para el estudiante {codigoEstudiante}...");
            // Lógica de registro financiero transaccional
        }
    }

    // Simula el Sistema de Recursos Humanos (Ej. Sistema SaaS Externo de Nómina)
    public class HumanResourcesSystemAdapter : IHumanResourcesService
    {
        public void RegistrarAsignacionDocente(string codigoCurso, string idDocente)
        {
            Console.WriteLine($"[Sistema RRHH] Registrando contrato y asignación del docente {idDocente} al curso {codigoCurso}...");
            // Lógica de gestión de personal y cargas horarias
        }
    }

    // =========================================================================
    // 3. INFRAESTRUCTURA DE INTEGRACIÓN / BUS DE SERVICIOS (ESB) O ORQUESTADOR
    // =========================================================================
    public class UniversidadServiceBusOrchestrator
    {
        private readonly IAcademicService _academicService;
        private readonly IFinancialService _financialService;
        private readonly IHumanResourcesService _hrService;

        public UniversidadServiceBusOrchestrator(
            IAcademicService academicService,
            IFinancialService financialService,
            IHumanResourcesService hrService)
        {
            _academicService = academicService;
            _financialService = financialService;
            _hrService = hrService;
        }

        // Operación de negocio de grano grueso expuesta a través de contratos de servicio
        public void MatricularEstudianteYAsignarRecursos(string estudianteId, string cursoId, decimal costoMatricula, string docenteId)
        {
            Console.WriteLine("=== INICIANDO TRANSACCIÓN COMPUESTA SOA (UNIVERSIDAD) ===");
            try
            {
                // 1. Invoca capacidad del sistema académico
                bool esValido = _academicService.ValidarEstudianteMatriculado(estudianteId, cursoId);

                if (esValido)
                {
                    // 2. Invoca capacidad del sistema financiero
                    _financialService.RegistrarPagoMatricula(estudianteId, costoMatricula);

                    // 3. Invoca capacidad de recursos humanos
                    _hrService.RegistrarAsignacionDocente(cursoId, docenteId);

                    Console.WriteLine("=== PROCESO INSTITUCIONAL INTEGRADO CON ÉXITO ===\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error de Integración SOA] {ex.Message}\n");
            }
        }
    }

    // =========================================================================
    // 4. PROGRAMA PRINCIPAL (Composition Root / Cliente SOA)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            // Inicialización de los adaptadores de servicios heterogéneos
            IAcademicService sistemaAcademico = new AcademicSystemAdapter();
            IFinancialService sistemaFinanciero = new FinancialSystemAdapter();
            IHumanResourcesService sistemaRrHH = new HumanResourcesSystemAdapter();

            // Configuración del Bus / Orquestador SOA
            var busOrquestador = new UniversidadServiceBusOrchestrator(
                sistemaAcademico,
                sistemaFinanciero,
                sistemaRrHH
            );

            // Simulación de una transacción institucional integrada
            busOrquestador.MatricularEstudianteYAsignarRecursos(
                estudianteId: "EST-2026-99",
                cursoId: "ARQ-SOFT-101",
                costoMatricula: 450.00m,
                docenteId: "DOC-DR-GARCIA"
            );
        }
    }
}