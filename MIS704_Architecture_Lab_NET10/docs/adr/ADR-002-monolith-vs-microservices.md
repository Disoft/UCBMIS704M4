### ADR-002: Monolith vs Microservices

#### Estado
Propuesto

#### Contexto
El equipo de desarrollo necesita definir la arquitectura base para iniciar el desarrollo de la nueva plataforma de servicios universitarios con equipos de tamaño reducido y alta presión de plazos iniciales.

#### Drivers
* Velocidad de entrega inicial (Time-to-Market)
* Complejidad operacional y de despliegue
* Independencia de escalabilidad por dominios de negocio
* Habilidades actuales del equipo técnico

#### Alternativas
1. Arquitectura Monolítica Modular
2. Arquitectura de Microservicios Distribuidos

#### Decisión
Adoptar inicialmente una Arquitectura Monolítica Modular para concentrar el esfuerzo en el dominio de negocio, migrando selectivamente a microservicios conforme crezca la escala y la autonomía de los equipos.

#### Consecuencias
* Despliegues iniciales simplificados y facilidad para depurar errores de forma centralizada.
* Menor sobrecarga de red y gestión de infraestructura en las primeras etapas del proyecto.
* Riesgo de acoplamiento interno si no se mantienen estrictos los límites entre módulos de código.

#### Validación
Revisiones periódicas de arquitectura, métricas de acoplamiento de código y pruebas de rendimiento del despliegue.