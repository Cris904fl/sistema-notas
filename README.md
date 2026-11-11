# Sistema de Notas

![CI](https://github.com/Cris904fl/sistema-notas/actions/workflows/ci.yml/badge.svg)

Prueba técnica .NET para **AudiSoft Consulting**: aplicación web para gestionar estudiantes, profesores y notas, con una API REST en .NET y un frontend en Angular.

## Tecnologías

| Capa | Tecnología |
|---|---|
| Base de datos | SQL Server 2022 (Docker) |
| Backend | .NET 10, ASP.NET Core Web API, Entity Framework Core, Swagger |
| Frontend | Angular (componentes standalone, signals), TypeScript, CSS propio |

## Estructura

```
PruebaAudisoft/
├── database/script.sql        # Tablas, llaves foráneas, restricciones y datos iniciales
├── docker-compose.yml         # SQL Server con el script aplicado automáticamente
├── backend/                   # Solución .NET por capas
│   ├── src/GestionNotas.Domain          # Entidades
│   ├── src/GestionNotas.Application     # DTOs, interfaces, paginación y excepciones
│   ├── src/GestionNotas.Infrastructure  # EF Core y servicios
│   ├── src/GestionNotas.Api             # Controladores, middleware de errores y Swagger
│   └── tests/GestionNotas.Tests         # Pruebas unitarias
└── frontend/                  # Aplicación Angular
```

## Requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Node.js 22 o superior](https://nodejs.org/) y Angular CLI (`npm install -g @angular/cli`)

## Instalación y ejecución

**1. Base de datos**

```bash
docker compose up -d
```

Levanta SQL Server en el puerto 1433 y ejecuta `database/script.sql` automáticamente (crea la base `GestionNotas` con datos de ejemplo).

**2. API**

```bash
cd backend
dotnet run --project src/GestionNotas.Api
```

La API queda en `http://localhost:5080` y la documentación Swagger en `http://localhost:5080/swagger`.

**3. Frontend**

```bash
cd frontend
npm install
ng serve
```

La aplicación queda en `http://localhost:4200`.

> La cadena de conexión de desarrollo está en `backend/src/GestionNotas.Api/appsettings.json` y apunta al contenedor de Docker. La contraseña es solo para el entorno local.

## Funcionalidades

- CRUD de estudiantes, profesores y notas.
- Llaves foráneas de `Nota` hacia `Estudiante` y `Profesor`, y validación del valor entre 0,0 y 5,0.
- Paginación, búsqueda y ordenamiento resueltos en la base de datos.
- Alertas al crear, actualizar y eliminar, y confirmación antes de eliminar.
- No se permite eliminar un estudiante o profesor con notas registradas (respuesta 409 con un mensaje claro).
- Promedio de cada estudiante y nombres del estudiante y el profesor en cada nota.
- Manejo de errores uniforme con `ProblemDetails` (RFC 9457).
- Diseño responsive: en celular, las tablas se muestran como tarjetas.
- Tema claro y oscuro seleccionable.

## Autor

Cristian David Flórez Jiménez · [github.com/Cris904fl](https://github.com/Cris904fl)
