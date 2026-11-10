### ADR-003: REST vs Messaging

#### Estado
Propuesto

#### Contexto
Los servicios internos de la plataforma deben comunicarse entre sí, específicamente para notificar transacciones de pago completadas y activar la generación de facturas y envíos de forma asíncrona.

#### Drivers
* Desacoplamiento temporal entre servicios
* Latencia en el camino crítico del usuario
* Resiliencia ante fallos de servicios dependientes
* Trazabilidad y simplicidad de depuración

#### Alternativas
1. Comunicación síncrona basada en REST / HTTP
2. Comunicación asíncrona basada en Mensajería (Brokers de eventos / Colas)

#### Decisión
Implementar un sistema de Mensajería Asíncrona (utilizando un Broker de Eventos) para las operaciones secundarias de notificación y procesamiento posterior al pago.

#### Consecuencias
* Desacoplamiento drástico que previene fallos en cascada si un servicio receptor no está disponible.
* Mejora en la latencia percibida por el usuario al no bloquear el hilo principal de la respuesta.
* Incremento en la complejidad de rastreo (*distributed tracing*) y gestión de reintentos por mensajes fallidos.

#### Validación
Pruebas de caos simulando la caída de microservicios consumidores y medición de tiempos de respuesta en el flujo principal.