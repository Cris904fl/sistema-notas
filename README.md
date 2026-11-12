# Sistema de Notas

![CI](https://github.com/Cris904fl/sistema-notas/actions/workflows/ci.yml/badge.svg)

Aplicación web para registrar estudiantes, profesores y notas. La desarrollé como prueba técnica para AudiSoft Consulting: una API REST en .NET 10 con SQL Server y una interfaz en Angular.

## Cómo ejecutarlo

Con Docker Desktop instalado, desde la raíz del repositorio:

```bash
docker compose up -d --build
```

| Qué | Dónde |
|---|---|
| Aplicación | http://localhost:4200 |
| Swagger | http://localhost:5080/swagger |
| Estado de la API | http://localhost:5080/salud |

La primera vez tarda unos minutos mientras descarga las imágenes y compila. Cada vez que se levanta, la base de datos se recrea con datos de ejemplo.

### Sin Docker para la API y el frontend

Si prefiere ejecutar cada parte por separado (necesita .NET 10, Node.js 22 o superior y Angular CLI):

```bash
# 1. Solo la base de datos
docker compose up -d sqlserver sqlinit

# 2. API
cd backend
dotnet run --project src/GestionNotas.Api

# 3. Frontend (en otra terminal)
cd frontend
npm install
ng serve
```

También puede usar un SQL Server propio: ejecute `database/script.sql` y ajuste la cadena de conexión en `backend/src/GestionNotas.Api/appsettings.json`.

### Pruebas

```bash
cd backend
dotnet test
```

## Qué incluye

**Lo que pedía la prueba:** las tablas Estudiante, Profesor y Nota con sus llaves foráneas, una API REST con el CRUD de cada una, y un frontend con menú, tablas para crear, editar y eliminar, paginación y alertas.

**Lo que agregué:**

- Paginación, búsqueda y orden resueltos en SQL Server, no en memoria.
- Un tablero de inicio cuyos indicadores calcula un procedimiento almacenado (`sp_ResumenGeneral`) y dos vistas.
- Validación del valor de la nota entre 0,0 y 5,0, tanto en el formulario como en la API y en la base de datos.
- No se puede eliminar un estudiante o un profesor que tenga notas: la API responde 409 con un mensaje claro.
- Control de edición simultánea con `ROWVERSION`: si dos personas editan la misma nota, la segunda recibe un aviso en vez de sobrescribir el cambio.
- Exportación de notas a CSV, respetando los filtros y lista para abrir en Excel.
- Errores con formato `ProblemDetails` (RFC 9457) y un endpoint de salud que verifica la conexión con la base de datos.
- Interfaz adaptable a celular y tema claro u oscuro.
- 36 pruebas unitarias con xUnit y un flujo de GitHub Actions que compila y prueba cada cambio.

## Estructura

```
database/        Scripts de SQL Server (tablas, vistas, procedimiento y datos)
backend/
  src/
    GestionNotas.Domain/          Entidades
    GestionNotas.Application/     DTOs, interfaces, paginación, excepciones y exportación CSV
    GestionNotas.Infrastructure/  Entity Framework Core y servicios
    GestionNotas.Api/             Controladores, manejo de errores, salud y Swagger
  tests/GestionNotas.Tests/       Pruebas unitarias
frontend/        Aplicación Angular
docker-compose.yml
```

## Decisiones técnicas

- **Capas separadas.** Application y Domain no dependen de Entity Framework ni de ASP.NET, así que la lógica se puede probar sin base de datos real.
- **Orden antes de proyectar.** En las consultas, el `OrderBy` se aplica sobre la entidad y luego se proyecta al DTO, para que Entity Framework lo traduzca completo a SQL.
- **Procedimiento y vistas para el tablero.** Los cálculos se hacen en SQL Server y solo viajan los resultados. El procedimiento se ejecuta tal cual; las vistas se consultan desde LINQ con filtro, orden y `TOP` en la base de datos.
- **Restrict en las llaves foráneas.** Preferí no borrar notas en cascada: la regla se valida en el servicio y el usuario recibe una explicación.

## Notas

La contraseña de SQL Server que aparece en `docker-compose.yml` y `appsettings.json` es solo para el entorno local de la prueba.

---

Cristian David Flórez Jiménez · [github.com/Cris904fl](https://github.com/Cris904fl)
