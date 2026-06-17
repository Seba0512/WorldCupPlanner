# WorldCupPlanner 2026

## Descripcion

WorldCupPlanner 2026 es una aplicacion desarrollada en C# con .NET 8 y Blazor Server para
gestionar la planificacion y seguimiento del Mundial 2026.

El sistema trabaja con 48 equipos distribuidos en 12 grupos y permite administrar fase de
grupos, etapas eliminatorias, equipos, estadios, usuarios, roles, configuracion del torneo,
fixture, partidos, resultados, clasificacion, simulacion, auditoria, notificaciones,
incidencias, importacion de equipos y exportacion de informacion.

La aplicacion utiliza persistencia en base de datos mediante Entity Framework Core Code First
sobre SQL Server/Azure SQL Edge en Docker, conservando el estado del sistema durante la
operativa normal.

## Tecnologias Utilizadas

- C#
- .NET 8
- Blazor Server
- Entity Framework Core 8
- SQL Server / Azure SQL Edge
- Docker
- MSTest
- CsvHelper
- ClosedXML

## Estructura De La Solucion

| Proyecto | Responsabilidad |
|---|---|
| `WorldCupPlanner.Dominio` | Entidades, enumeraciones, excepciones y reglas propias del dominio. |
| `WorldCupPlanner.Dominio.DTO` | Objetos de transferencia de datos usados entre capas. |
| `WorldCupPlanner.AccesoADatos.Interfaces` | Contratos de repositorio usados por los servicios. |
| `WorldCupPlanner.AccesoADatos` | Persistencia con Entity Framework Core, `SqlContext`, migraciones y repositorios SQL. |
| `WorldCupPlanner.Servicios` | Casos de uso, reglas de aplicacion, simulacion, fixture, clasificacion e importacion/exportacion. |
| `WorldCupPlanner.Controlador` | Capa intermedia entre la interfaz Blazor y los servicios. |
| `WorldCupPlanner.UI` | Interfaz Blazor Server y configuracion de arranque de la aplicacion. |

### Proyectos De Test

| Proyecto | Responsabilidad |
|---|---|
| `WorldCupPlanner.DominioTests` | Pruebas unitarias de entidades, reglas y validaciones del dominio. |
| `WorldCupPlanner.ServicioTests` | Pruebas unitarias de servicios y reglas de aplicacion. |
| `WorldCupPlanner.ControladorTests` | Pruebas de la capa de controladores. |
| `WorldCupPlanner.AccesoADatosTest` | Pruebas de repositorios y acceso a datos, usando EF Core InMemory cuando corresponde. |

## Requisitos Previos

- .NET 8 SDK.
- Docker Desktop o Docker Engine.
- Herramientas de Entity Framework Core, si se quieren ejecutar migraciones manualmente.
- DBeaver u otro cliente SQL, opcional para visualizar la base.

Para instalar la herramienta de Entity Framework Core:

```bash
dotnet tool install --global dotnet-ef
```

Si ya esta instalada y se quiere actualizar:

```bash
dotnet tool update --global dotnet-ef
```

## Instalacion

El costo de instalacion de la aplicacion es minimo: alcanza con tener .NET 8, Docker y restaurar
las dependencias NuGet del repositorio.

### Ubicacion de ejecutables y librerias

- El proyecto ejecutable de la aplicacion es `WorldCupPlanner.UI`.
- En desarrollo se ejecuta directamente con `dotnet run --project WorldCupPlanner.UI`.
- Si se publica la aplicacion, los artefactos quedan bajo la carpeta generada por .NET, por
  ejemplo `WorldCupPlanner.UI/bin/Release/net8.0/publish`.
- Las librerias requeridas se restauran automaticamente desde NuGet a partir de los archivos
  `.csproj` mediante `dotnet restore` o al ejecutar `dotnet build`, `dotnet run` o `dotnet test`.

### String de conexion a modificar

La cadena de conexion se encuentra en:

```text
WorldCupPlanner.UI/appsettings.json
```

Valor usado por defecto:

```text
Server=localhost;Database=WorldCupPlanner;User Id=sa;Password=ObliDA1*;TrustServerCertificate=True;
```

Si se cambia el nombre del servidor, puerto, base, usuario o contrasenia del contenedor SQL,
tambien debe actualizarse este valor.

### Base de datos y scripts

- La base de datos se crea y actualiza mediante migraciones de Entity Framework Core.
- Las migraciones se encuentran en `WorldCupPlanner.AccesoADatos/Migraciones`.
- El contexto de base de datos se encuentra en `WorldCupPlanner.AccesoADatos/SqlContext.cs`.
- El archivo `docker-compose.yml` define el contenedor `sqlserver`.
- Los datos persistidos del contenedor se guardan en la carpeta local `data`.
- Tambien se incluyen scripts SQL independientes en la carpeta `Scripts-SQL`:
  - `ScriptTablas.SQL`: definicion de tablas y relaciones.
  - `datosDePrueba.sql`: datos de prueba con usuarios, equipos, estadios, auditorias,
    partidos, incidencias y notificaciones.
  - `scriptUsuarios.sql`: usuarios de prueba.
- Para la ejecucion normal de la aplicacion se recomienda usar las migraciones EF. Los scripts
  SQL quedan disponibles como entregables auxiliares y para carga/restauracion de datos de prueba.

### Usuario inicial

La aplicacion no crea usuarios por codigo al iniciar. Para una demo o base de prueba se pueden
cargar los usuarios incluidos en `Scripts-SQL/scriptUsuarios.sql`.

Usuarios de prueba incluidos:

| Rol | Email |
|---|---|
| Administrador | `administrador@mail.com` |
| Administrador | `administrador2@mail.com` |
| Editor | `editor@mail.com` |
| Editor | `editor2@mail.com` |
| Periodista | `periodista@mail.com` |
| Periodista | `periodista2@mail.com` |
| Administrador, Editor y Periodista | `admineditorperiodista@mail.com` |

Las contrasenias estan almacenadas en el script como valores cifrados, no en texto plano.

## Configuracion De Base De Datos

La base se levanta con Docker usando Azure SQL Edge. Desde la raiz del repositorio ejecutar:

```bash
docker compose up -d
```

El contenedor expone SQL Server en el puerto `1433`. La cadena de conexion usada por la
aplicacion esta en `WorldCupPlanner.UI/appsettings.json`:

```text
Server=localhost;Database=WorldCupPlanner;User Id=sa;Password=ObliDA1*;TrustServerCertificate=True;
```

## Migraciones

La aplicacion ejecuta las migraciones automaticamente al iniciar, mediante `Database.Migrate()`
en `WorldCupPlanner.UI/Program.cs`.

Si se desea aplicar las migraciones manualmente antes de ejecutar la aplicacion, usar:

```bash
dotnet ef database update --project WorldCupPlanner.AccesoADatos --startup-project WorldCupPlanner.UI
```

## Ejecucion De La Aplicacion

Con Docker levantado, ejecutar:

```bash
dotnet run --project WorldCupPlanner.UI
```

El perfil HTTP configurado para desarrollo usa:

```text
http://localhost:5076
```

Tambien existe perfil HTTPS en:

```text
https://localhost:7129
```

## Ejecucion De Tests

Para ejecutar todos los tests de la solucion:

```bash
dotnet test
```

Tambien se puede ejecutar un proyecto de tests especifico:

```bash
dotnet test WorldCupPlanner.DominioTests
dotnet test WorldCupPlanner.ServicioTests
dotnet test WorldCupPlanner.ControladorTests
dotnet test WorldCupPlanner.AccesoADatosTest
```

## Notas Utiles

- `WorldCupPlanner.UI` es el proyecto de arranque.
- `docker-compose.yml` crea el contenedor `sqlserver` usando la imagen `mcr.microsoft.com/azure-sql-edge`.
- Los datos de SQL Server se guardan en la carpeta local `data`.
- Los servicios de negocio dependen de interfaces de repositorio, no directamente de `SqlContext`.
- Los repositorios SQL se encuentran en `WorldCupPlanner.AccesoADatos/Repositorios`.
- Las migraciones se encuentran en `WorldCupPlanner.AccesoADatos/Migraciones`.
- No se debe modificar manualmente la base sin revisar su impacto sobre las migraciones y el modelo de EF Core.
