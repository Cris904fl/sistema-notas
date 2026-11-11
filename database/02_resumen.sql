/* =============================================================
   Objetos para el tablero de resumen (vistas y procedimiento)
   Se puede ejecutar varias veces: usa CREATE OR ALTER.
   ============================================================= */
USE GestionNotas;
GO

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
