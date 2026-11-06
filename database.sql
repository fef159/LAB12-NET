IF OBJECT_ID(N'dbo.Mascotas', N'U') IS NOT NULL DROP TABLE dbo.Mascotas;
IF OBJECT_ID(N'dbo.Propietarios', N'U') IS NOT NULL DROP TABLE dbo.Propietarios;

CREATE TABLE dbo.Propietarios (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Propietarios PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL,
    Telefono NVARCHAR(20) NULL,
    CONSTRAINT UQ_Propietarios_Email UNIQUE (Email)
);

CREATE TABLE dbo.Mascotas (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Mascotas PRIMARY KEY,
    Nombre NVARCHAR(80) NOT NULL,
    Especie NVARCHAR(50) NOT NULL,
    Edad INT NOT NULL CONSTRAINT CK_Mascotas_Edad CHECK (Edad BETWEEN 0 AND 40),
    PropietarioId INT NOT NULL,
    CONSTRAINT FK_Mascotas_Propietarios FOREIGN KEY (PropietarioId)
        REFERENCES dbo.Propietarios(Id)
);

INSERT INTO dbo.Propietarios (Nombre, Email, Telefono) VALUES
(N'Ana Torres', N'ana.torres@example.com', N'+51 987 111 222'),
(N'Luis Mendoza', N'luis.mendoza@example.com', N'+51 986 333 444');

INSERT INTO dbo.Mascotas (Nombre, Especie, Edad, PropietarioId) VALUES
(N'Luna', N'Perro', 4, 1),
(N'Milo', N'Gato', 2, 1),
(N'Kira', N'Perro', 6, 2);
