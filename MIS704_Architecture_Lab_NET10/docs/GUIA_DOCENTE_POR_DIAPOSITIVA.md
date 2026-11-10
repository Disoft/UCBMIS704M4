# Guía docente: diapositivas → demostraciones (.NET 10)

Basada en `MIS704_Application_Architecture_System_Design(1).pptx`. Los números corresponden a la numeración visible.

## Ejecución principal

```bash
dotnet run --project src/MIS704.Demo
dotnet run --project src/MIS704.WebApi --urls http://localhost:5080
dotnet test MIS704.ArchitectureLab.sln
```

## Cómo presentar cada ejemplo

1. Mostrar primero el problema o driver.
2. Abrir el archivo indicado y explicar el código.
3. Ejecutar el Demo o la API cuando haya implementación.
4. Preguntar por trade-offs y validación.
5. No confundir analogías o ejemplos conceptuales con sistemas de producción.

## Mapa por diapositiva

| Slide | Concepto | Archivo(s) | Qué mostrar / explicar |
|---:|---|---|---|
| 40 | Modularidad | `src/MIS704.Application/Contracts.cs` | Identificar módulos Enrollment/Payments y sus contratos. |
| 41 | Separation of concerns | `src/MIS704.Demo/BeforeAfter.cs; src/MIS704.Application/UseCases.cs` | Comparar BadEnrollmentManager con EnrollStudentHandler. |
| 42 | Information hiding | `src/MIS704.Application/Contracts.cs; src/MIS704.Infrastructure/InMemoryAdapters.cs` | Sustituir implementación de repositorio sin tocar el caso de uso. |
| 43 | Cohesion | `src/MIS704.Domain/Enrollment.cs; src/MIS704.Demo/BeforeAfter.cs` | Comparar reglas de matrícula con el manager multipropósito. |
| 44 | Coupling | `src/MIS704.Application/UseCases.cs` | Localizar dependencias a puertos vs detalles concretos. |
| 45 | Connascence | `src/MIS704.Demo/BeforeAfter.cs` | Discutir cambios coordinados en llamadas y firmas. |
| 46 | Connascence of position | `src/MIS704.Demo/BeforeAfter.cs` | Before.ProcessOrder frente a After.ProcessOrder(OrderRequest). |
| 47 | Refactoring | `src/MIS704.Demo/BeforeAfter.cs; src/MIS704.Application/UseCases.cs` | Mover lógica de manager a caso de uso y adaptadores. |
| 48 | Quality attributes | `docs/adr/ADR-001-inmemory-for-class.md` | Diferenciar cualidades de la demo y de producción. |
| 49 | Atributos de calidad | `README.md` | Identificar testabilidad, mantenibilidad y fiabilidad. |
| 50 | Quality attribute scenario | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Redactar escenario medible con seis elementos. |
| 51 | Performance | `src/MIS704.WebApi/Program.cs` | GET /api/demo/latency; explicar limitaciones de la medición. |
| 52 | Availability | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Dibujar balanceador y múltiples instancias; NO simulado. |
| 53 | Security | `src/MIS704.WebApi/Program.cs` | Endpoint /grades devuelve 501; diseñar authN/authZ real. |
| 54 | Maintainability / testability | `tests/MIS704.Tests/ArchitectureTests.cs` | Prueba del caso de uso sin base de datos. |
| 55 | Trade-offs | `docs/adr/ADR-001-inmemory-for-class.md` | Comparar memoria vs SQL Server real. |
| 56 | ASR | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Formular driver de durabilidad y auditoría. |
| 57 | Identificar ASR | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Priorizar requisitos por impacto y riesgo. |
| 58 | Actividad ASR | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Completar plantilla de escenario de calidad. |
| 60 | Gestión decisiones | `docs/adr/ADR-001-inmemory-for-class.md` | Identificar, decidir, documentar, ejecutar y comunicar. |
| 61 | Anatomía decisión | `docs/adr/ADR-001-inmemory-for-class.md` | Identificar contexto, drivers, alternativas, consecuencias. |
| 62 | ADR | `docs/adr/ADR-001-inmemory-for-class.md` | Abrir ADR y analizar secciones. |
| 63 | PostgreSQL vs MongoDB | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Debate; no se implementan estos motores. |
| 64 | Monolito vs microservicios | `docs/diagrams/architecture.mmd` | Comparar una WebApi con múltiples servicios hipotéticos. |
| 65 | REST vs messaging | `src/MIS704.WebApi/Program.cs; src/MIS704.Infrastructure/InMemoryAdapters.cs` | Contrastar endpoint REST con IEventPublisher a consola. |
| 66 | Revisión ADR | `docs/adr/ADR-002-sync-vs-events.md` | Proponer ADR sucesor ante cambio de drivers. |
| 67 | Monolítico vs distribuido | `docs/diagrams/architecture.mmd` | Diferenciar despliegue único y distribución real. |
| 68 | Technical vs domain | `docs/diagrams/modules.mmd` | Comparar capas técnicas con módulo Enrollment. |
| 70 | Layered | `docs/diagrams/architecture.mmd` | Mostrar separación entre proyectos. |
| 71 | MVC | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | WebApi es Minimal API, no MVC; explicar diferencia. |
| 72 | MVVM | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | No implementado; plantear ViewModel en WPF. |
| 73 | VIPER | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | No implementado; diagrama de responsabilidades. |
| 74 | Hexagonal | `src/MIS704.Application/Contracts.cs; src/MIS704.Infrastructure/InMemoryAdapters.cs` | Puerto IEnrollmentRepository + adaptador. |
| 75 | Onion | `docs/diagrams/architecture.mmd` | Mostrar dirección de dependencias. |
| 77 | Pipeline | `src/MIS704.Infrastructure/PluginAndPipeline.cs` | Ejecutar StudentImportPipeline. |
| 78 | Microkernel | `src/MIS704.Infrastructure/PluginAndPipeline.cs` | Ejecutar ReportHost con TextReportPlugin. |
| 79 | Service-Based | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Diseño conceptual, no despliegue distribuido. |
| 80 | Event-Driven | `src/MIS704.Application/Contracts.cs; src/MIS704.Infrastructure/InMemoryAdapters.cs` | Evento EnrollmentCreated y publisher a consola. |
| 81 | OrderCreated → Shipped | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Diagramar secuencia; no está implementado. |
| 82 | SOA | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Diseñar contratos de integración. |
| 83 | Modular Monolith | `docs/diagrams/modules.mmd` | Un despliegue, módulos con límites. |
| 84 | Microservices | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Diseño conceptual; no simular independencia inexistente. |
| 85 | Distribución | `docs/adr/ADR-002-sync-vs-events.md` | Riesgo de falla entre persistir y publicar. |
| 86 | Network fallacies | `docs/adr/ADR-002-sync-vs-events.md` | Identificar reintentos, timeout e idempotencia. |
| 87 | Service boundaries | `docs/diagrams/modules.mmd` | Separar capacidades por negocio. |
| 88 | Data ownership | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Definir propiedad de datos, sin DB compartida real. |
| 89 | Síncrono | `src/MIS704.WebApi/Program.cs` | Endpoint HTTP request/response. |
| 90 | Asíncrono | `src/MIS704.Application/UseCases.cs` | Publicación de evento simulada; no broker. |
| 91 | Transacciones distribuidas | `docs/adr/ADR-002-sync-vs-events.md` | Analizar Outbox y Saga sin afirmar implementación. |
| 92 | Resiliencia | `src/MIS704.WebApi/Program.cs` | HttpClient con timeout; no circuit breaker. |
| 93 | Observabilidad | `src/MIS704.WebApi/Program.cs` | Medición de latencia; faltan trazas distribuidas. |
| 94 | Gateway / discovery | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Diseño conceptual. |
| 95 | Monolito → modular | `src/MIS704.Demo/BeforeAfter.cs; docs/diagrams/modules.mmd` | Contrastar BadEnrollmentManager con límites. |
| 96 | Modular → microservicios | `docs/adr/ADR-002-sync-vs-events.md` | Definir condiciones de extracción. |
| 97 | Resiliencia checkout | `src/MIS704.Infrastructure/InMemoryAdapters.cs` | DemoPaymentGateway con clave de idempotencia local. |
| 99 | Space-Based | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Conceptual: no hay grid distribuido. |
| 100 | Serverless | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Conceptual: no hay Function desplegada. |
| 101 | Comparación estilos | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Construir matriz de trade-offs. |
| 102 | Elegir estilo | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Ejercicio de cuatro escenarios. |
| 103 | Relacional | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Modelo SQL ilustrativo, no motor incluido. |
| 104 | Documental | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Comparar agregado documental con entidad. |
| 105 | Key-value | `src/MIS704.Infrastructure/InMemoryAdapters.cs` | ConcurrentDictionary es analogía, no Redis. |
| 106 | Object storage | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Diseñar almacenamiento multimedia. |
| 107 | Graph DB | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Modelar nodos y aristas. |
| 108 | Selección persistencia | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Comparar motores según patrón de acceso. |
| 109 | Polyglot | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Evaluar costo operativo de múltiples motores. |
| 110 | Ejercicio selección | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Sistema educativo: matriz y ADR. |
| 112 | Clean Architecture | `docs/diagrams/architecture.mmd` | Domain al centro, referencias entrantes. |
| 113 | Dependency inversion | `src/MIS704.Application/Contracts.cs` | El caso de uso depende de interfaz. |
| 114 | Domain-centric | `src/MIS704.Domain/Enrollment.cs` | Reglas de matrícula en dominio. |
| 115 | Domain boundaries | `src/MIS704.Domain/Enrollment.cs` | Identificar invariantes. |
| 116 | CQRS | `src/MIS704.Application/Contracts.cs; src/MIS704.Application/UseCases.cs` | EnrollStudentCommand y GetEnrollmentQuery. |
| 117 | Cuándo CQRS | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Discutir costo de separar lecturas y escrituras. |
| 118 | Event Sourcing | `src/MIS704.Domain/BankAccount.cs` | Replay de AccountEvent. |
| 119 | Bank Account | `src/MIS704.Domain/BankAccount.cs` | Abrir +100 -30 => 70. |
| 120 | ES vs CRUD | `src/MIS704.Domain/BankAccount.cs` | Diferenciar historial vs estado actual. |
| 121 | Testable | `tests/MIS704.Tests/ArchitectureTests.cs` | Pruebas sin SQL Server. |
| 122 | Pirámide tests | `tests/MIS704.Tests/ArchitectureTests.cs` | Solo unitarias: integración/E2E pendientes. |
| 123 | Refactoring testable | `src/MIS704.Demo/BeforeAfter.cs; tests/MIS704.Tests/ArchitectureTests.cs` | Comparar acoplamiento directo vs ports. |
| 124 | Cierre | `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md` | Defensa de decisión + trade-off + evidencia. |

## Plantilla para Quality Attribute Scenario (slides 50, 56–58)

| Campo | Ejemplo |
|---|---|
| Fuente | 500 estudiantes concurrentes |
| Estímulo | Consultar matrícula |
| Entorno | Inscripción de inicio de semestre |
| Artefacto | API de matrículas |
| Respuesta | Responder consulta correctamente |
| Medida | p95 < 200 ms bajo carga y entorno definidos |

## Preguntas para discusión

- ¿Qué se rompe si `Infrastructure` cambia de motor de datos?
- ¿Cuándo una interfaz oculta detalles y cuándo solo añade indirección?
- ¿Qué riesgo existe si la publicación de eventos falla después de guardar?
- ¿Qué evidencia justificaría separar `Enrollment` como microservicio?
- ¿Cómo validarías el escenario p95 sin confundirlo con tiempo de procesamiento?

## Límites de alcance

No se incluye infraestructura real de SQL Server, MongoDB, Redis, Kafka/RabbitMQ, Kubernetes, FaaS ni despliegue distribuido. Las diapositivas correspondientes se trabajan con diseños y ADR.
