IF DB_ID(N'InventarioDb') IS NULL
    CREATE DATABASE InventarioDb;
GO

USE InventarioDb;
GO

CREATE TABLE Categorias (
    Id INT IDENTITY(1,1) CONSTRAINT PK_Categorias PRIMARY KEY,
    Nombre NVARCHAR(80) NOT NULL CONSTRAINT UQ_Categorias_Nombre UNIQUE,
    Descripcion NVARCHAR(250) NULL
);
GO

CREATE TABLE Productos (
    Id INT IDENTITY(1,1) CONSTRAINT PK_Productos PRIMARY KEY,
    Nombre NVARCHAR(120) NOT NULL,
    Sku NVARCHAR(30) NOT NULL CONSTRAINT UQ_Productos_Sku UNIQUE,
    Precio DECIMAL(12,2) NOT NULL CONSTRAINT CK_Productos_Precio CHECK (Precio > 0),
    Stock INT NOT NULL CONSTRAINT CK_Productos_Stock CHECK (Stock >= 0),
    CategoriaId INT NOT NULL,
    CONSTRAINT FK_Productos_Categorias FOREIGN KEY (CategoriaId)
        REFERENCES Categorias(Id) ON DELETE NO ACTION
);
GO

INSERT INTO Categorias (Nombre, Descripcion) VALUES
(N'Computadores', N'Equipos de escritorio y portátiles'),
(N'Accesorios', N'Periféricos y complementos');

INSERT INTO Productos (Nombre, Sku, Precio, Stock, CategoriaId) VALUES
(N'Portátil empresarial 14 pulgadas', N'PORT-001', 3299000, 8, 1),
(N'Monitor IPS 24 pulgadas', N'MON-001', 749900, 15, 2),
(N'Teclado mecánico', N'TEC-001', 289900, 20, 2);
GO
