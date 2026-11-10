# MIS704 — Architecture Lab (.NET 10)

Solución didáctica para **Arquitectura de Aplicaciones y Diseño de Sistemas**, basada en el sistema universitario utilizado en clase.

## Requisitos
- SDK **.NET 10** (`dotnet --version`)
- Visual Studio 2026 con soporte .NET 10, VS Code o terminal.
- Internet solo para restaurar paquetes NuGet del proyecto de pruebas.

## Primer uso (Windows PowerShell)
```powershell
cd MIS704_Architecture_Lab_NET10
powershell -ExecutionPolicy Bypass -File scripts/create-solution.ps1
dotnet run --project src/MIS704.Demo
dotnet run --project src/MIS704.WebApi --urls http://localhost:5080
dotnet test MIS704.ArchitectureLab.sln
```
En Linux/macOS: `bash scripts/create-solution.sh`.

## Probar la API (otra terminal PowerShell)
```powershell
Invoke-RestMethod http://localhost:5080/
$body = @{ studentId=[guid]::NewGuid().ToString(); courseId=[guid]::NewGuid().ToString(); credits=3 } | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri http://localhost:5080/api/enrollments -ContentType 'application/json' -Body $body
Invoke-RestMethod -Uri http://localhost:5080/api/enrollments
$payment = @{ idempotencyKey='demo-001'; amount=100 } | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri http://localhost:5080/api/payments -ContentType 'application/json' -Body $payment
```

## Organización
- `MIS704.Domain`: entidades, invariantes y proyección Event Sourcing.
- `MIS704.Application`: puertos, comandos, consultas y casos de uso.
- `MIS704.Infrastructure`: adaptadores en memoria, Pipeline, Microkernel.
- `MIS704.WebApi`: endpoints HTTP y composition root.
- `MIS704.Demo`: comparaciones **Before / After** y demostraciones por consola.
- `MIS704.Tests`: pruebas unitarias con xUnit.
- `docs/GUIA_DOCENTE_POR_DIAPOSITIVA.md`: correspondencia diapositiva → código → explicación.
- `docs/adr`: decisiones arquitectónicas.
- `docs/diagrams`: diagramas Mermaid.

## Advertencias pedagógicas
1. No se afirma que todos los estilos de las diapositivas estén implementados completamente: para Space-Based, SOA, microservicios reales, Serverless, MongoDB, Redis, brokers, Graph DB y seguridad real se incluyen guías conceptuales, no servicios productivos.
2. El repositorio en memoria no es durable; el publisher a consola no implementa mensajería confiable.
3. El endpoint de calificaciones responde **501** deliberadamente: no debe presentarse como autenticación/autorización real.
4. La idempotencia es local al proceso y no cubre caídas, reinicios o transacciones concurrentes distribuidas.
5. La ruta `/api/demo/latency` mide una sola solicitud, no percentiles bajo carga.
6. El código no ha podido compilarse aquí porque este entorno no tiene instalado el SDK .NET. Ejecutar `dotnet build` y `dotnet test` en un equipo con .NET 10.
