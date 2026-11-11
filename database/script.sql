/* =============================================================
   Prueba Técnica .NET – AudiSoft Consulting
   Base de datos: GestionNotas (SQL Server)
   Autor: Cristian David Flórez Jiménez
   ============================================================= */

IF DB_ID('GestionNotas') IS NULL
    CREATE DATABASE GestionNotas;
GO

USE GestionNotas;
GO

/* Se eliminan en orden inverso por las llaves foráneas,
   para que el script se pueda ejecutar varias veces. */
IF OBJECT_ID('dbo.Nota', 'U') IS NOT NULL DROP TABLE dbo.Nota;
IF OBJECT_ID('dbo.Estudiante', 'U') IS NOT NULL DROP TABLE dbo.Estudiante;
IF OBJECT_ID('dbo.Profesor', 'U') IS NOT NULL DROP TABLE dbo.Profesor;
GO

/* ---------- Tabla Estudiante ---------- */
CREATE TABLE dbo.Estudiante
(
    Id     INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(100)     NOT NULL,
    CONSTRAINT PK_Estudiante PRIMARY KEY (Id),
    CONSTRAINT CK_Estudiante_Nombre CHECK (LEN(LTRIM(RTRIM(Nombre))) > 0)
);
GO

/* ---------- Tabla Profesor ---------- */
CREATE TABLE dbo.Profesor
(
    Id     INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(100)     NOT NULL,
    CONSTRAINT PK_Profesor PRIMARY KEY (Id),
    CONSTRAINT CK_Profesor_Nombre CHECK (LEN(LTRIM(RTRIM(Nombre))) > 0)
);
GO

/* ---------- Tabla Nota ---------- */
CREATE TABLE dbo.Nota
(
    Id           INT IDENTITY(1,1) NOT NULL,
    Nombre       NVARCHAR(100)     NOT NULL,   -- Ej.: "Parcial 1", "Taller de SQL"
    IdProfesor   INT               NOT NULL,
    IdEstudiante INT               NOT NULL,
    Valor        DECIMAL(3,1)      NOT NULL,   -- Escala colombiana de 0.0 a 5.0
    CONSTRAINT PK_Nota PRIMARY KEY (Id),
    CONSTRAINT FK_Nota_Profesor   FOREIGN KEY (IdProfesor)   REFERENCES dbo.Profesor (Id),
    CONSTRAINT FK_Nota_Estudiante FOREIGN KEY (IdEstudiante) REFERENCES dbo.Estudiante (Id),
    CONSTRAINT CK_Nota_Valor  CHECK (Valor BETWEEN 0 AND 5),
    CONSTRAINT CK_Nota_Nombre CHECK (LEN(LTRIM(RTRIM(Nombre))) > 0)
);
GO

/* Índices sobre las llaves foráneas: aceleran los JOIN
   y la validación al eliminar estudiantes o profesores. */
CREATE INDEX IX_Nota_IdProfesor   ON dbo.Nota (IdProfesor);
CREATE INDEX IX_Nota_IdEstudiante ON dbo.Nota (IdEstudiante);
GO

/* ---------- Datos iniciales ---------- */
INSERT INTO dbo.Profesor (Nombre) VALUES
    (N'Ana María Rodríguez'),
    (N'Carlos Andrés Gómez'),
    (N'Luisa Fernanda Torres');

INSERT INTO dbo.Estudiante (Nombre) VALUES
    (N'Juan Pablo Martínez'),
    (N'Valentina Herrera'),
    (N'Santiago López'),
    (N'Camila Ramírez'),
    (N'Andrés Felipe Castro');

INSERT INTO dbo.Nota (Nombre, IdProfesor, IdEstudiante, Valor) VALUES
    (N'Parcial 1',          1, 1, 4.5),
    (N'Parcial 1',          1, 2, 3.8),
    (N'Taller de SQL',      2, 1, 5.0),
    (N'Taller de SQL',      2, 3, 2.9),
    (N'Proyecto final',     3, 4, 4.2),
    (N'Proyecto final',     3, 5, 3.5),
    (N'Quiz de algoritmos', 2, 2, 4.0);
GO

/* Consulta de verificación */
SELECT n.Id, n.Nombre AS Nota, e.Nombre AS Estudiante, p.Nombre AS Profesor, n.Valor
FROM dbo.Nota n
JOIN dbo.Estudiante e ON e.Id = n.IdEstudiante
JOIN dbo.Profesor   p ON p.Id = n.IdProfesor
ORDER BY n.Id;
GO

/* ---------- Objetos del tablero de resumen ---------- */

/* Promedio y cantidad de notas por estudiante */
CREATE OR ALTER VIEW dbo.vw_PromedioEstudiante
AS
SELECT e.Id                              AS IdEstudiante,
       e.Nombre                          AS Estudiante,
       COUNT(n.Id)                       AS CantidadNotas,
       CAST(AVG(n.Valor) AS DECIMAL(3,1)) AS Promedio
FROM dbo.Estudiante e
LEFT JOIN dbo.Nota n ON n.IdEstudiante = e.Id
GROUP BY e.Id, e.Nombre;
GO

/* Promedio de las notas registradas por cada profesor */
CREATE OR ALTER VIEW dbo.vw_PromedioProfesor
AS
SELECT p.Id                              AS IdProfesor,
       p.Nombre                          AS Profesor,
       COUNT(n.Id)                       AS CantidadNotas,
       CAST(AVG(n.Valor) AS DECIMAL(3,1)) AS Promedio
FROM dbo.Profesor p
LEFT JOIN dbo.Nota n ON n.IdProfesor = p.Id
GROUP BY p.Id, p.Nombre;
GO

/* Indicadores generales del tablero (una sola fila).
   La nota mínima aprobatoria se recibe como parámetro (por defecto 3,0). */
CREATE OR ALTER PROCEDURE dbo.sp_ResumenGeneral
    @NotaAprobatoria DECIMAL(3,1) = 3.0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        (SELECT COUNT(*) FROM dbo.Estudiante)              AS TotalEstudiantes,
        (SELECT COUNT(*) FROM dbo.Profesor)                AS TotalProfesores,
        COUNT(n.Id)                                         AS TotalNotas,
        CAST(AVG(n.Valor) AS DECIMAL(3,1))                  AS PromedioGeneral,
        CAST(100.0 * SUM(CASE WHEN n.Valor >= @NotaAprobatoria THEN 1 ELSE 0 END)
             / NULLIF(COUNT(n.Id), 0) AS DECIMAL(5,1))      AS PorcentajeAprobacion,
        MAX(n.Valor)                                        AS NotaMaxima,
        MIN(n.Valor)                                        AS NotaMinima
    FROM dbo.Nota n;
END;
GO
