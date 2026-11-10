# ADR-001 — Repositorio en memoria para laboratorio

**Estado:** Aceptado para demostración, no para producción.

## Contexto
Se requiere mostrar modularidad, DIP, puertos/adaptadores y pruebas sin instalar SQL Server.

## Alternativas
1. SQL Server real. 2. Repositorio en memoria.

## Decisión
Usar adaptador en memoria detrás de `IEnrollmentRepository`.

## Consecuencias
+ Inicio rápido, tests aislados. - Sin durabilidad, transacciones ni consistencia entre instancias.

## Revisión
Reemplazar por adaptador EF Core / SQL Server al enseñar persistencia real.
