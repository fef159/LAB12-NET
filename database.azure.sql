-- Ejecutar conectado directamente a la Azure SQL Database de la aplicación.
-- La base ya debe existir; este archivo crea el esquema y los datos de ejemplo.

IF OBJECT_ID(N'dbo.Categorias', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categorias (
        Id INT IDENTITY(1,1) CONSTRAINT PK_Categorias PRIMARY KEY,
        Nombre NVARCHAR(80) NOT NULL CONSTRAINT UQ_Categorias_Nombre UNIQUE,
        Descripcion NVARCHAR(250) NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.Productos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Productos (
        Id INT IDENTITY(1,1) CONSTRAINT PK_Productos PRIMARY KEY,
        Nombre NVARCHAR(120) NOT NULL,
        Sku NVARCHAR(30) NOT NULL CONSTRAINT UQ_Productos_Sku UNIQUE,
        Precio DECIMAL(12,2) NOT NULL CONSTRAINT CK_Productos_Precio CHECK (Precio > 0),
        Stock INT NOT NULL CONSTRAINT CK_Productos_Stock CHECK (Stock >= 0),
        CategoriaId INT NOT NULL,
        CONSTRAINT FK_Productos_Categorias FOREIGN KEY (CategoriaId)
            REFERENCES dbo.Categorias(Id) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Categorias)
BEGIN
    INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES
    (N'Computadores', N'Equipos de escritorio y portátiles'),
    (N'Accesorios', N'Periféricos y complementos');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Productos)
BEGIN
    INSERT INTO dbo.Productos (Nombre, Sku, Precio, Stock, CategoriaId)
    SELECT N'Portátil empresarial 14 pulgadas', N'PORT-001', 3299000, 8, c.Id
    FROM dbo.Categorias AS c WHERE c.Nombre = N'Computadores'
    UNION ALL
    SELECT N'Monitor IPS 24 pulgadas', N'MON-001', 749900, 15, c.Id
    FROM dbo.Categorias AS c WHERE c.Nombre = N'Accesorios'
    UNION ALL
    SELECT N'Teclado mecánico', N'TEC-001', 289900, 20, c.Id
    FROM dbo.Categorias AS c WHERE c.Nombre = N'Accesorios';
END;
GO
