/* =============================================================
   Control de concurrencia optimista en la tabla Nota.
   Para bases de datos creadas antes de este cambio. Se puede ejecutar varias veces.
   ============================================================= */
USE GestionNotas;
GO

IF COL_LENGTH('dbo.Nota', 'RowVersion') IS NULL
    ALTER TABLE dbo.Nota ADD RowVersion ROWVERSION NOT NULL;
GO
