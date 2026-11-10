### ADR-001: PostgreSQL vs MongoDB

#### Estado
Propuesto

#### Contexto
La plataforma empresarial de comercio electrónico debe gestionar órdenes, pagos y perfiles de usuario garantizando consistencia financiera y flexibilidad en ciertos atributos de productos.

#### Drivers
* Consistencia transaccional estricta (ACID)
* Experiencia y familiaridad del equipo de desarrollo
* Capacidad de consultas complejas y reportes
* Rendimiento bajo concurrencia moderada

#### Alternativas
1. PostgreSQL (Base de datos relacional)
2. MongoDB (Base de datos NoSQL orientada a documentos)

#### Decisión
Seleccionar PostgreSQL para el núcleo transaccional de órdenes y pagos debido a la necesidad crítica de integridad de datos y consistencia ACID.

#### Consecuencias
* Garantía de integridad mediante transacciones y restricciones relacionales estrictas.
* Modelo relacional explícito que requiere migraciones controladas de esquemas.
* Necesidad de planificar estrategias de indexación y réplicas de lectura para escalar el rendimiento.

#### Validación
Pruebas de concurrencia, integridad transaccional bajo carga y validación de consultas complejas.