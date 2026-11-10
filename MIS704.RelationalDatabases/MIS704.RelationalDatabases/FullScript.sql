-- =============================================================================
-- PROYECTO: Sistema de Gestión Transaccional (Modelo Relacional)
-- MOTOR: Microsoft SQL Server (T-SQL)
-- DESCRIPCIÓN: Implementación de modelo tabular, constraints, transacciones ACID,
--              integridad referencial y consultas relacionales (Joins).
-- =============================================================================

-- 0. Configuración inicial y creación de la base de datos
USE master;
GO

IF  DB_ID(N'MIS704') IS NULL
BEGIN
    CREATE DATABASE MIS704;
END
GO

USE MIS704;
GO

-- Limpieza previa de tablas si ya existen (para pruebas limpias)
IF OBJECT_ID('dbo.DetallesPedido', 'U') IS NOT NULL DROP TABLE dbo.DetallesPedido;
IF OBJECT_ID('dbo.Pedidos', 'U') IS NOT NULL DROP TABLE dbo.Pedidos;
IF OBJECT_ID('dbo.Clientes', 'U') IS NOT NULL DROP TABLE dbo.Clientes;
GO

-- =============================================================================
-- 1. MODELO TABULAR, CLAVES Y RESTRICCIONES (Constraints)
-- =============================================================================

-- Creación de la tabla Clientes
CREATE TABLE dbo.Clientes (
    cliente_id INT IDENTITY(1,1) NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    correo VARCHAR(150) NOT NULL,
    limite_credito DECIMAL(10,2) NOT NULL,
    fecha_registro DATETIME DEFAULT GETDATE(),
    
    -- Restricciones (Constraints)
    CONSTRAINT PK_Clientes PRIMARY KEY CLUSTERED (cliente_id),
    CONSTRAINT UQ_Clientes_Correo UNIQUE (correo),
    CONSTRAINT CK_Clientes_LimiteCredito CHECK (limite_credito >= 0)
);
GO

-- Creación de la tabla Pedidos
CREATE TABLE dbo.Pedidos (
    pedido_id INT IDENTITY(1,1) NOT NULL,
    cliente_id INT NOT NULL,
    fecha_pedido DATETIME NOT NULL DEFAULT GETDATE(),
    monto_total DECIMAL(10,2) NOT NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'PENDIENTE',

    -- Restricciones y Claves Foráneas (Integridad Referencial)
    CONSTRAINT PK_Pedidos PRIMARY KEY CLUSTERED (pedido_id),
    CONSTRAINT CK_Pedidos_MontoTotal CHECK (monto_total > 0),
    CONSTRAINT CK_Pedidos_Estado CHECK (estado IN ('PENDIENTE', 'PAGADO', 'CANCELADO')),
    
    CONSTRAINT FK_Pedidos_Clientes 
        FOREIGN KEY (cliente_id) 
        REFERENCES dbo.Clientes(cliente_id)
        ON DELETE NO ACTION -- Protege al cliente de ser borrado si tiene pedidos
        ON UPDATE CASCADE   -- Actualiza en cascada si cambia la PK del cliente
);
GO

-- Creación de la tabla DetallesPedido (Relación de 1 a Muchos con Pedidos)
CREATE TABLE dbo.DetallesPedido (
    detalle_id INT IDENTITY(1,1) NOT NULL,
    pedido_id INT NOT NULL,
    producto VARCHAR(100) NOT NULL,
    cantidad INT NOT NULL,
    precio_unitario DECIMAL(10,2) NOT NULL,

    CONSTRAINT PK_DetallesPedido PRIMARY KEY CLUSTERED (detalle_id),
    CONSTRAINT CK_Detalles_Cantidad CHECK (cantidad > 0),
    CONSTRAINT CK_Detalles_Precio CHECK (precio_unitario > 0),

    CONSTRAINT FK_Detalles_Pedidos 
        FOREIGN KEY (pedido_id) 
        REFERENCES dbo.Pedidos(pedido_id)
        ON DELETE CASCADE -- Si se elimina el pedido, se eliminan sus detalles automáticamente
        ON UPDATE CASCADE
);
GO


-- =============================================================================
-- 2. TRANSACCIONES ACID FUERTES (Atomicidad, Consistencia, Aislamiento, Durabilidad)
-- =============================================================================

PRINT '--- EJECUTANDO TRANSACCIÓN ACID EXITOSA ---';
GO

BEGIN TRANSACTION;

BEGIN TRY
    -- 1. Insertar un nuevo cliente (Atomicidad parte 1)
    INSERT INTO dbo.Clientes (nombre, correo, limite_credito)
    VALUES ('Carlos Mendoza', 'carlos.mendoza@email.com', 3000.00);

    -- Obtener el ID generado para el cliente actual
    DECLARE @ClienteIdGenerado INT = SCOPE_IDENTITY();

    -- 2. Insertar un pedido asociado al cliente (Atomicidad parte 2)
    INSERT INTO dbo.Pedidos (cliente_id, monto_total, estado)
    VALUES (@ClienteIdGenerado, 450.00, 'PAGADO');

    DECLARE @PedidoIdGenerado INT = SCOPE_IDENTITY();

    -- 3. Insertar detalles del pedido (Atomicidad parte 3)
    INSERT INTO dbo.DetallesPedido (pedido_id, producto, cantidad, precio_unitario)
    VALUES (@PedidoIdGenerado, 'Teclado Mecánico', 1, 450.00);

    -- Si todo el bloque transcurre sin errores, se confirma (COMMIT - Durabilidad y Consistencia)
    COMMIT TRANSACTION;
    PRINT 'Transacción ACID completada con éxito. Datos persistidos de forma duradera.';

END TRY
BEGIN CATCH
    -- Si ocurre cualquier fallo, se revierte todo (ROLLBACK - Atomicidad garantizada)
    ROLLBACK TRANSACTION;
    PRINT 'Error detectado en la transacción. Se aplicó ROLLBACK completo.';
    
    -- Mostrar detalles del error
    SELECT 
        ERROR_NUMBER() AS NumeroError,
        ERROR_MESSAGE() AS MensajeError;
END CATCH;
GO


-- =============================================================================
-- 3. DEMOSTRACIÓN DE INTEGRIDAD REFERENCIAL
-- =============================================================================

PRINT '--- PROBANDO RESTRICCIÓN DE INTEGRIDAD REFERENCIAL ---';
GO

BEGIN TRY
    -- Intentamos insertar un pedido con un cliente_id que NO existe (ej. 9999)
    INSERT INTO dbo.Pedidos (cliente_id, monto_total, estado)
    VALUES (9999, 150.00, 'PENDIENTE');
    
    PRINT 'Éxito inesperado (esto no debería imprimirse).';
END TRY
BEGIN CATCH
    PRINT '¡Violación de Integridad Referencial bloqueada correctamente por SQL Server!';
    PRINT ERROR_MESSAGE();
END CATCH;
GO


-- =============================================================================
-- 4. CONSULTAS RELACIONALES (JOINS) Y REPORTES
-- =============================================================================

PRINT '--- CONSULTA RELACIONAL AVANZADA (INNER JOIN) ---';
GO

-- Consultar información unida de Clientes, Pedidos y sus Detalles
SELECT 
    c.cliente_id,
    c.nombre AS cliente_nombre,
    c.correo,
    p.pedido_id,
    p.fecha_pedido,
    p.estado AS estado_pedido,
    d.producto,
    d.cantidad,
    d.precio_unitario,
    (d.cantidad * d.precio_unitario) AS subtotal_calculado
FROM 
    dbo.Clientes c
INNER JOIN 
    dbo.Pedidos p ON c.cliente_id = p.cliente_id
INNER JOIN 
    dbo.DetallesPedido d ON p.pedido_id = d.pedido_id;
GO