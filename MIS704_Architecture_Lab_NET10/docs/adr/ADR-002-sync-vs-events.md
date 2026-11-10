# ADR-002 — Publicación de eventos tras matrícula

**Estado:** Propuesto; requiere validación.

## Contexto
Una matrícula genera una notificación y otros procesos de negocio.

## Alternativas
1. Llamadas síncronas. 2. Eventos asíncronos con broker.

## Decisión didáctica
Demostrar `IEventPublisher` con un adaptador de consola.

## Riesgos
La demo publica después de guardar sin transacción compartida. Puede perder eventos en fallos.

## Validación requerida para producción
Outbox transaccional, consumidores idempotentes, reintentos, DLQ y observabilidad.
