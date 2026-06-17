# ef-repository-migration

## Objetivo

Guiar a un agente de IA para completar, corregir o revisar la migración de **WorldCupPlanner 2026** desde repositorios en memoria hacia persistencia real con **Entity Framework Core Code First**, **Azure SQL Edge en Docker** y visualización con **DBeaver**, manteniendo la arquitectura por capas y evitando que la lógica de negocio quede acoplada a la tecnología de persistencia.

La idea central de esta skill es preservar el desacoplamiento ya existente:

```text
Antes:   Servicios -> interfaces de repositorio -> repositorios en memoria -> listas
Después: Servicios -> interfaces de repositorio -> repositorios SQL -> SqlContext -> base de datos
```

Los servicios deben seguir dependiendo de contratos como `IRepositorio<T>`, `IRepositorioConId<T>`, `IRepositorioUsuario`, `IRepositorioNotificacion`, `IRepositorioIncidencia` o equivalentes. Los servicios no deben depender directamente de `SqlContext`, `DbContext`, `DbSet`, Entity Framework ni SQL Server.

---

## Cuándo usar esta skill

Usar esta skill cuando la tarea sea una de estas:

- Completar el pasaje de una entidad que todavía está en memoria a repositorio SQL.
- Crear repositorios SQL faltantes.
- Corregir `SqlContext` o sus `DbSet`.
- Ajustar mapeos de EF Core en `OnModelCreating`.
- Corregir problemas de migraciones, claves primarias, relaciones, tracking o cascadas.
- Corregir la inyección de dependencias para que no mezcle `Singleton` con dependencias `Scoped`.
- Revisar que todos los datos exigidos por el Obligatorio 2 sean persistidos en base de datos.
- Hacer que la app reconstruya el último estado guardado al reiniciar.
- Evitar que `EstadoTorneo` sea la fuente de verdad final del sistema.
- Agregar tests de acceso a datos usando EF Core InMemory o SQLite InMemory.
- Preparar comandos de migración y actualización de base de datos.
- Revisar si el pasaje a base de datos rompió reglas de la Parte 1.

No usar esta skill como guía principal para:

- Implementar ranking dinámico desde cero.
- Implementar motores de simulación intercambiables desde cero.
- Implementar exportaciones CSV/XLSX desde cero.
- Diseñar DTOs de presentación, salvo ajustes mínimos para no desincronizarlos con entidades persistidas.
- Rediseñar la UI.
- Reescribir algoritmos de fixture, cruces, clasificación o simulación.
- Crear documentación final o README.

---

## Contexto del proyecto

WorldCupPlanner 2026 es una aplicación **Blazor Server en .NET 8** para gestionar el Mundial 2026 con 48 equipos, 12 grupos de 4, fixture determinístico por semillas, simulación de resultados, clasificación, cruces eliminatorios, usuarios, roles, estadios, equipos, importación CSV, incidencias, notificaciones y auditoría.

En la Parte 1 se usaba persistencia en memoria. En la Parte 2 la letra exige que **todos los datos del sistema** sean persistidos en base de datos, que al reiniciar la aplicación se cargue el último estado guardado y que las acciones se guarden durante la operativa, no con un botón global ni solamente al cerrar.

Arquitectura esperada:

```text
WorldCupPlanner.UI
    -> WorldCupPlanner.Servicios
        -> WorldCupPlanner.AccesoADatos.Interfaces
        -> WorldCupPlanner.AccesoADatos
            -> SqlContext / repositorios SQL
        -> WorldCupPlanner.Dominio
```

Proyectos relevantes:

- `WorldCupPlanner.Dominio`
  - Entidades, enums, excepciones y reglas propias del dominio.
  - No debe depender de EF Core ni de AccesoADatos.

- `WorldCupPlanner.AccesoADatos.Interfaces`
  - Contratos como `IRepositorio<T>`, `IRepositorioConId<T>`, `IRepositorioUsuario`, `IRepositorioNotificacion`, `IRepositorioIncidencia`, `IRepositorioConClaveEditable<T>`.
  - Es el punto de desacoplamiento entre servicios y persistencia.

- `WorldCupPlanner.AccesoADatos`
  - Repositorios en memoria existentes.
  - Repositorios SQL existentes o pendientes.
  - `SqlContext` y migraciones EF Core.

- `WorldCupPlanner.Servicios`
  - Casos de uso y reglas de negocio.
  - Debe permanecer desacoplado de Entity Framework.

- `WorldCupPlanner.UI`
  - Blazor Server.
  - `Program.cs`, `appsettings.json`, páginas `.razor`, servicios de sesión y toasts.

- Proyectos de tests
  - MSTest para dominio, servicios y acceso a datos.

---

## Estado actual detectado en `develop`

Antes de modificar código, verificar si este estado sigue vigente. En la rama `develop` analizada ya existe una migración parcial a EF y algunas funcionalidades de la Parte 2.

### Infraestructura EF ya iniciada

- Existe `WorldCupPlanner.AccesoADatos/SqlContext.cs`.
- Existe `docker-compose.yml` con Azure SQL Edge.
- Existe `appsettings.json` con `ConnectionStrings:DefaultConnection`.
- Existen migraciones en `WorldCupPlanner.AccesoADatos/Migraciones`.
- `WorldCupPlanner.AccesoADatos.csproj` referencia:
  - `Microsoft.EntityFrameworkCore`
  - `Microsoft.EntityFrameworkCore.SqlServer`
  - `Microsoft.EntityFrameworkCore.Design`

### DbSets actuales

`SqlContext` ya tiene `DbSet` para:

- `Usuario`
- `Equipo`
- `Estadio`
- `Grupo`
- `Partido`
- `Auditoria`
- `Notificacion`
- `Incidencia`

Pero todavía revisar si falta persistir explícitamente:

- `ConfiguracionTorneo`
- estado activo del torneo
- configuración usada para fixture
- motor de simulación elegido
- bloqueo de fases
- torneo finalizado
- fixture anterior, si sigue siendo parte funcional del sistema

### Repositorios SQL existentes

Ya existen repositorios SQL para:

- `RepositorioEquipoSql`
- `RepositorioEstadioSql`
- `RepositorioUsuarioSql`
- `RepositorioAuditoriaSql`
- `RepositorioNotificacionSql`
- `RepositorioIncidenciaSql`

### Repositorios todavía en memoria

En `Program.cs` de la rama analizada todavía aparecen:

```csharp
builder.Services.AddSingleton<IRepositorio<Grupo>, RepositorioGrupoEnMemoria>();
builder.Services.AddSingleton<IRepositorio<Partido>, RepositorioPartidoEnMemoria>();
```

Esto puede ser aceptable solamente como estado temporal de desarrollo. Para la entrega final, si `Grupo` y `Partido` quedan solo en memoria, la migración no cumple completamente la letra.

### Uso de `EstadoTorneo`

`EstadoTorneo` sigue registrado como `Singleton` y muchas páginas lo usan como fuente principal para:

- fixture activo
- partidos eliminatorios
- fixture anterior
- torneo finalizado

Esto debe revisarse. `EstadoTorneo` puede existir como cache o estado transitorio de UI, pero **no puede ser la fuente de verdad final**. La fuente de verdad debe ser la base de datos.

La aplicación debe poder reconstruir el fixture, grupos, partidos, resultados, incidencias, cruces, notificaciones y demás datos persistidos desde SQL al reiniciar.

### Lifetimes actuales a revisar

En `Program.cs` existen servicios `Singleton` junto a repositorios SQL `Scoped`. Cualquier servicio que dependa directa o indirectamente de repositorios SQL o de `SqlContext` debe ser `Scoped`.

Revisar especialmente:

- `ServicioFixture`
- `ServicioClasificacion`
- `ServicioCruces`
- `ServicioSimulacion`
- `ServicioConfiguracionTorneo`
- `ServicioPartido`
- `EstadoTorneo`

No cambiar un servicio a `Scoped` mecánicamente sin revisar su uso, pero evitar cualquier dependencia `Singleton -> Scoped`.

### Funcionalidades de Parte 2 ya iniciadas

La rama analizada ya contiene avances en:

- `Equipo.Bandera`
- `RolUsuario.Periodista`
- `Notificacion`
- `ServicioNotificacion`
- `Incidencia`
- `TarjetaAmarilla`
- `TarjetaRoja`
- `ServicioIncidencia`
- UI de tarjetas en edición de partido
- repositorios SQL de notificación e incidencia
- proyecto `WorldCupPlanner.Dominio.DTO`

Puntos a revisar por esta skill solo si afectan persistencia:

- `EquipoDTO` puede estar desactualizado respecto de `Id` y `Bandera`.
- No parece haber DTOs de `Notificacion` e `Incidencia`.
- `Partido.Incidencias` está ignorado en EF y las incidencias se relacionan por `PartidoId`; esto es válido si está documentado y los servicios consultan incidencias por repositorio.
- `Partido.Id` se genera por identidad SQL en el contexto actual, pero el proyecto usa IDs determinísticos para partidos. Esto debe revisarse: si los servicios asignan IDs 1-72, 73+, EF no debería reemplazarlos silenciosamente.

---

## Entradas esperadas

Cuando esta skill se use, el agente puede recibir:

- Estructura de carpetas.
- Código de entidades de dominio.
- Interfaces de repositorios.
- Repositorios en memoria.
- Repositorios SQL existentes.
- `SqlContext.cs`.
- Migraciones actuales.
- `Program.cs`.
- `appsettings.json`.
- `.csproj`.
- `docker-compose.yml`.
- Tests existentes.
- Errores de compilación, runtime o migración.
- Requerimientos de la letra del obligatorio.
- Decisiones de diseño del equipo.

Antes de proponer cambios, el agente debe revisar:

1. Nombres reales de proyectos y namespaces.
2. Interfaces de repositorio existentes.
3. Constructores de servicios.
4. Registro actual de DI en `Program.cs`.
5. Entidades que ya tienen `Id`.
6. Entidades con propiedades privadas, backing fields o constructores privados.
7. Entidades con listas o relaciones.
8. Entidades con propiedades calculadas.
9. Repositorios SQL ya implementados.
10. Migraciones y snapshot actual.
11. Tests existentes para repositorios y servicios.
12. Si hay código productivo que todavía depende de repositorios en memoria.
13. Si hay código productivo que todavía depende de `EstadoTorneo` como estado principal.
14. Si la aplicación puede reconstruirse desde la base al reiniciar.

---

## Reglas obligatorias

### Arquitectura

- Mantener separación entre `Dominio`, `Servicios`, `AccesoADatos`, `AccesoADatos.Interfaces` y `UI`.
- `Dominio` no debe depender de EF Core.
- `Servicios` no debe depender de `DbContext`, `SqlContext`, `DbSet`, migraciones ni clases concretas de repositorio.
- `UI` no debe contener reglas de negocio.
- Repositorios SQL deben implementar las interfaces existentes.
- La lógica de negocio debe seguir en Dominio o Servicios.
- Los repositorios solo resuelven persistencia y consultas.
- No duplicar validaciones de servicios en repositorios.
- No cambiar firmas públicas si no es imprescindible.
- Si se cambia una interfaz, justificar el motivo y actualizar todos los implementadores.

### Persistencia exigida por la letra

- Todos los datos del sistema deben persistirse en base de datos.
- Los datos deben guardarse durante la operativa de la solución.
- No implementar un botón global de guardar todo.
- No depender solamente de `EstadoTorneo` para conservar datos.
- Al reiniciar la app debe recuperarse el último estado guardado.
- Si una entidad queda solo en memoria, registrar como pendiente crítico.
- La base productiva debe correr en Azure SQL Edge con Docker.
- Deben existir scripts o respaldos: uno vacío y uno con datos de prueba.

### Entity Framework Core

- `SqlContext` debe estar en `WorldCupPlanner.AccesoADatos` o equivalente de acceso a datos.
- Usar EF Core Code First.
- Usar `DbSet<T>` para entidades persistibles.
- Configurar relaciones complejas con Fluent API en `OnModelCreating`.
- No usar atributos EF en entidades de dominio si se puede resolver con Fluent API.
- No llamar `Database.Migrate()` desde el constructor del `DbContext`.
- La migración puede ejecutarse desde `Program.cs` durante el arranque si el equipo lo decide y lo documenta.
- No usar EF InMemory en producción.
- Evitar cascadas peligrosas. Preferir `DeleteBehavior.Restrict` o `NoAction` cuando borrar una entidad pueda eliminar datos del torneo accidentalmente.
- Revisar el snapshot después de cada migración.
- No editar migraciones o snapshots manualmente salvo correcciones entendidas y justificadas.

### IDs y claves

- Revisar si cada entidad usa ID generado por SQL o ID asignado por el dominio/servicio.
- Para `Partido`, tener especial cuidado: la Parte 1 usa IDs determinísticos para grupos y fases eliminatorias. Si se mantiene esa regla, configurar `ValueGeneratedNever()` en `Partido.Id`.
- Para `Auditoria`, si ya no se usa `SiguienteId()` porque SQL genera la clave, documentarlo y ajustar el repositorio de forma clara.
- Si `Grupo` usa etiqueta A-L como identidad conceptual, decidir si se usa `Id` técnico más índice único por `Etiqueta`.
- Evitar duplicados por claves naturales como nombre de equipo, nombre de estadio o email de usuario.

### Repositorios SQL

- Implementar las mismas interfaces que los repositorios en memoria.
- Mantener nombres en español, por ejemplo `RepositorioPartidoSql`.
- Usar `SaveChanges()` en `Agregar`, `Actualizar` y `Eliminar`, salvo que el proyecto incorpore explícitamente Unit of Work.
- Usar `AsNoTracking()` para consultas de solo lectura.
- No usar `AsNoTracking()` si el objeto se va a modificar dentro del mismo contexto sin adjuntarlo correctamente.
- Si se usa `Update(entidad)` con entidades desconectadas, manejar posibles conflictos de tracking.
- Si se necesita actualizar por clave anterior, usar `IRepositorioConClaveEditable<T>` o método específico ya existente.
- Evitar cargar toda la base a memoria innecesariamente.
- Si `IRepositorio<T>.Buscar(Func<T,bool>)` obliga a evaluar en memoria, mantenerlo solo como transición y recomendar mejora futura con `Expression<Func<T,bool>>` o métodos de consulta específicos.

### Inyección de dependencias

- `DbContext` se registra con `AddDbContext` y lifetime `Scoped`.
- Repositorios que dependen de `DbContext` deben ser `Scoped`, no `Singleton`.
- Servicios que dependan de repositorios SQL `Scoped` también deben ser `Scoped`.
- No mezclar un `Singleton` que dependa directa o indirectamente de un servicio `Scoped`.
- Si un servicio sigue siendo `Singleton`, verificar que no dependa de repositorios SQL ni de objetos `Scoped`.
- `SesionServicio` y `ToastServicio` pueden seguir siendo `Scoped`.

### Tests

- Mantener MSTest.
- No depender de Docker ni SQL Server real en tests automatizados.
- Para tests de repositorios SQL usar EF Core InMemory o SQLite InMemory.
- Agregar `Microsoft.EntityFrameworkCore.InMemory` al proyecto de tests si se usa InMemory.
- Cada test debe crear una base con nombre único o limpiar su estado.
- Los tests deben seguir Arrange / Act / Assert.
- Los tests deben ser rápidos, independientes, repetibles y autovalidados.

### Funcionalidades de la Parte 1

No romper:

- Gestión de usuarios.
- Gestión de equipos.
- Gestión de estadios.
- Configuración de torneo.
- Completar equipos.
- Generación de fixture.
- Simulación de resultados.
- Clasificación.
- Cruces eliminatorios.
- Edición de partidos.
- Importación CSV.
- Auditoría.
- Login y roles existentes.

---

## Definition of Done de la migración

La migración a base de datos se considera terminada solamente si se cumplen todos estos puntos:

- No queda ningún repositorio en memoria registrado en `Program.cs` para datos productivos del sistema.
- `Equipo`, `Estadio`, `Usuario`, `Grupo`, `Partido`, `Fixture` o su estado equivalente, resultados, eliminatorias, incidencias, notificaciones, auditorías y configuración del torneo se pueden persistir y recuperar desde base de datos.
- `ConfiguracionTorneo` o su equivalente persistido guarda tanto la configuración vigente como la configuración usada al generar el fixture, si el sistema necesita diferenciarlas.
- `EstadoTorneo`, si sigue existiendo, no es la fuente de verdad final; como máximo funciona como cache, estado de navegación o representación transitoria reconstruida desde SQL.
- La aplicación puede cerrarse, reiniciarse y volver a mostrar el último estado guardado: equipos, estadios, usuarios, fixture, grupos, partidos, resultados, incidencias, notificaciones y auditorías.
- Las acciones de usuario guardan sus cambios durante la operativa normal, sin depender de un botón global de guardar todo ni de lógica ejecutada solamente al cerrar la aplicación.
- No existen dependencias `Singleton -> Scoped`.
- Los servicios de negocio no dependen de `SqlContext`, `DbContext`, `DbSet`, migraciones ni clases concretas de EF Core.
- Los repositorios SQL implementan las interfaces existentes o interfaces nuevas justificadas.
- Las migraciones fueron generadas, revisadas y aplicadas correctamente.
- La base productiva se puede levantar con Docker/Azure SQL Edge y visualizar en DBeaver.
- Existen scripts o respaldos para base vacía y base con datos de prueba.
- Existen tests para los repositorios SQL y para la reconstrucción del estado relevante, usando EF Core InMemory o SQLite InMemory cuando corresponda.
- Las funcionalidades de Parte 1 siguen funcionando luego del pasaje a base de datos.

Si falta cualquiera de estos puntos, la skill debe tratarlo como **pendiente explícito**. Si el pendiente afecta `Grupo`, `Partido`, fixture, resultados, eliminatorias o `EstadoTorneo`, debe marcarlo como **pendiente crítico**.

---

## Regla de seguridad ante cambios masivos

Si para cumplir una tarea el agente detecta que necesita modificar muchas capas, reescribir varios servicios, alterar algoritmos de fixture/cruces/simulación/clasificación, cambiar interfaces públicas ampliamente o tocar muchas páginas `.razor`, debe detenerse antes de aplicar cambios y responder con:

1. diagnóstico del problema;
2. archivos afectados;
3. motivo por el cual el cambio es riesgoso;
4. plan incremental propuesto;
5. primer paso pequeño y verificable;
6. tests necesarios antes y después.

No aceptar ni generar un diff masivo sin explicación. La migración debe hacerse en pasos chicos, revisables y con pruebas.

---


## Orden recomendado de ejecución

La migración debe realizarse en etapas pequeñas, verificables y con pruebas. No intentar resolver todo en un único cambio grande.

Orden recomendado:

1. **Diagnóstico sin modificar código**
   - Revisar `Program.cs`, `SqlContext`, repositorios, servicios y usos de `EstadoTorneo`.
   - Identificar qué datos siguen dependiendo de memoria.
   - Listar repositorios SQL faltantes y relaciones EF pendientes.

2. **Revisar registros en memoria y lifetimes**
   - Detectar registros como `RepositorioGrupoEnMemoria`, `RepositorioPartidoEnMemoria` u otros repositorios productivos en memoria.
   - Detectar servicios `Singleton` que dependan directa o indirectamente de repositorios SQL o `SqlContext`.
   - No cambiar todo de golpe: primero registrar los riesgos y luego ajustar por entidad migrada.

3. **Migrar `Partido` a SQL**
   - Crear o completar `RepositorioPartidoSql`.
   - Asegurar persistencia de fecha, estadio, equipos, fase, grupo, goles, ganador por penales y demás datos necesarios.
   - Cuidar especialmente `Partido.Id`, porque el sistema usa IDs determinísticos para partidos de fase de grupos y eliminatorias.
   - Agregar tests de repositorio SQL para guardar, buscar y actualizar partidos.

4. **Migrar `Grupo` a SQL**
   - Crear o completar `RepositorioGrupoSql`.
   - Persistir etiqueta, equipos del grupo y partidos asociados.
   - Evitar duplicar grupos por etiqueta.
   - Agregar tests para recuperar grupos con equipos y partidos.

5. **Persistir `ConfiguracionTorneo`**
   - Guardar configuración actual y configuración usada al generar fixture, si el sistema necesita diferenciarlas.
   - Persistir semillas, fechas, máximos de partidos por día y parámetros horarios.
   - Mantener bloqueo de modificación luego de generado el fixture.

6. **Persistir estado del torneo / fixture activo / fases generadas**
   - Definir cómo se sabe desde BD si existe fixture activo, si ya hay eliminatorias, si el torneo finalizó y qué fases están bloqueadas.
   - Persistir el motor de simulación elegido cuando ese requisito esté implementado.
   - Puede crearse una entidad específica como `Torneo`, `EstadoTorneoPersistido` o equivalente, si el diseño del equipo lo justifica.

7. **Hacer que `EstadoTorneo` deje de ser fuente de verdad**
   - No eliminarlo automáticamente.
   - Convertirlo en cache, estado de navegación o representación reconstruida desde BD.
   - La app debe poder cerrar, reiniciar y reconstruir fixture, grupos, partidos, resultados, eliminatorias, incidencias y notificaciones desde SQL.

8. **Ajustar inyección de dependencias**
   - Registrar `SqlContext`, repositorios SQL y servicios dependientes como `Scoped`.
   - Eliminar registros productivos en memoria cuando exista reemplazo SQL probado.
   - Verificar que no quede ninguna dependencia `Singleton -> Scoped`.

9. **Generar y revisar migraciones EF**
   - Generar migraciones con nombres descriptivos.
   - Revisar la migración y el `SqlContextModelSnapshot`.
   - Confirmar que no se crean columnas duplicadas, relaciones incorrectas ni cascadas peligrosas.

10. **Agregar tests de persistencia**
    - Usar MSTest con EF Core InMemory o SQLite InMemory.
    - Probar guardado, actualización, eliminación y recuperación tras recrear el contexto.
    - Probar especialmente fixture, grupos, partidos, resultados, incidencias y notificaciones.

11. **Prueba manual de reinicio**
    - Levantar Docker y aplicar migraciones.
    - Crear datos, generar fixture, cargar o simular resultados e incidencias.
    - Cerrar la app, reiniciar y verificar que el último estado se reconstruye desde la base.

12. **Verificar DBeaver y respaldos/scripts**
    - Confirmar que las tablas y datos se ven correctamente en DBeaver.
    - Preparar script/respaldo vacío y script/respaldo con datos de prueba.

Si en cualquier etapa aparece la necesidad de modificar muchas capas o reescribir lógica de negocio, aplicar la sección **Regla de seguridad ante cambios masivos** antes de continuar.

---

## Procedimiento paso a paso

### 1. Analizar la tarea concreta

Clasificar la tarea:

- Crear repositorio SQL faltante.
- Corregir repositorio SQL existente.
- Corregir `SqlContext`.
- Corregir DI.
- Corregir migración.
- Persistir estado de torneo.
- Persistir configuración.
- Reconstruir estado desde base.
- Agregar tests.
- Diagnosticar error.

Responder primero con un plan incremental. No reescribir toda la aplicación.

### 2. Revisar estado actual del código

Inspeccionar:

- `SqlContext.cs`
- `Program.cs`
- repositorios SQL
- repositorios en memoria todavía usados
- interfaces
- entidades
- servicios que usan esas entidades
- páginas que dependen de `EstadoTorneo`
- migraciones y snapshot

### 3. Identificar datos que aún no persisten correctamente

Revisar especialmente:

- `Grupo`
- `Partido`
- `ConfiguracionTorneo`
- fixture activo
- cruces eliminatorios
- resultados simulados o manuales
- incidencias
- notificaciones
- auditorías
- estado de torneo finalizado
- configuración usada al generar fixture
- motor de simulación elegido

### 4. Crear o completar repositorios SQL faltantes

Prioridad recomendada:

1. `RepositorioPartidoSql`
2. `RepositorioGrupoSql`
3. repositorio para `ConfiguracionTorneo`
4. repositorio/servicio para estado persistido del torneo, si el diseño lo requiere

Los repositorios deben implementar interfaces existentes o una interfaz específica justificada.

### 5. Ajustar `SqlContext`

Verificar:

- claves primarias
- índices únicos
- relaciones equipo-partido
- relación grupo-partido
- relación grupo-equipos
- relación partido-incidencias
- relación notificación-usuario, si corresponde
- configuración de enum como string o entero, manteniendo consistencia
- `Partido.Id` determinístico
- `Fixture.Partidos` calculado, si corresponde ignorarlo

### 6. Ajustar inyección de dependencias

Reemplazar registros en memoria por SQL cuando el repositorio SQL exista y esté probado.

Ejemplo esperado al terminar:

```csharp
builder.Services.AddScoped<IRepositorio<Grupo>, RepositorioGrupoSql>();
builder.Services.AddScoped<IRepositorio<Partido>, RepositorioPartidoSql>();
```

No dejar para entrega final:

```csharp
builder.Services.AddSingleton<IRepositorio<Grupo>, RepositorioGrupoEnMemoria>();
builder.Services.AddSingleton<IRepositorio<Partido>, RepositorioPartidoEnMemoria>();
```

### 7. Revisar `EstadoTorneo`

No eliminarlo automáticamente. Decidir su rol:

- cache transitoria de UI
- estado de navegación
- representación en memoria reconstruida desde SQL

Pero no debe ser el único lugar donde viven los datos del torneo.

Si se mantiene, crear mecanismo de carga desde BD al iniciar o al abrir páginas:

```text
BD -> Servicio de consulta/persistencia -> EstadoTorneo reconstruido -> UI
```

### 8. Ajustar servicios solo lo necesario

No mover lógica de negocio a repositorios. Ajustar servicios únicamente para:

- usar repositorios ya inyectados
- persistir cambios cuando corresponde
- dejar de depender exclusivamente de `EstadoTorneo`
- reconstruir datos desde repositorios

### 9. Crear migraciones

Usar nombres descriptivos:

```bash
dotnet ef migrations add AgregarRepositorioPartidosSql --project WorldCupPlanner.AccesoADatos --startup-project WorldCupPlanner.UI

dotnet ef database update --project WorldCupPlanner.AccesoADatos --startup-project WorldCupPlanner.UI
```

Ajustar nombres reales si el proyecto cambió.

### 10. Agregar tests

Agregar tests de repositorio SQL para las entidades migradas:

- agrega y recupera
- actualiza sin duplicar
- elimina
- carga relaciones necesarias
- no duplica claves naturales
- persiste resultados de partido
- persiste incidencias de partido
- persiste notificaciones leídas

Usar EF InMemory o SQLite InMemory. No depender de Docker para tests unitarios.

### 11. Verificar manualmente

Checklist manual mínimo:

- Levantar Docker.
- Ejecutar migraciones.
- Abrir DBeaver.
- Crear equipo/estadio/usuario.
- Generar fixture.
- Simular fase de grupos.
- Cerrar app.
- Reiniciar app.
- Verificar que fixture, partidos, resultados e incidencias siguen disponibles.
- Marcar notificación como leída.
- Reiniciar app.
- Verificar que la notificación leída no vuelve a aparecer.

---

## Diseño esperado

El resultado final debe mantener este flujo:

```text
UI Blazor
  -> Servicio de aplicación/caso de uso
      -> interfaz de repositorio
          -> repositorio SQL
              -> SqlContext
                  -> Azure SQL Edge
```

Ejemplo correcto:

```text
EditarPartido.razor
  -> ServicioPartido.CargarResultado
      -> IRepositorio<Partido>.Actualizar
          -> RepositorioPartidoSql
              -> SqlContext.Partidos
```

Ejemplo incorrecto:

```text
EditarPartido.razor
  -> SqlContext.Partidos.Update(...)
```

Ejemplo incorrecto:

```text
ServicioPartido
  -> new SqlContext(...)
```

---

## Entity Framework Core

### Recomendaciones generales

- Preferir Fluent API en `OnModelCreating`.
- Mantener entidades de dominio limpias de atributos EF.
- Usar índices únicos para reglas de unicidad que también existan en servicios.
- Usar `Include` cuando sea necesario recuperar objetos relacionados.
- Documentar cuando una navegación no se persiste directamente y se consulta por repositorio.

### Cuidado con propiedades calculadas

Si `Fixture.Partidos` se calcula desde los grupos, no mapearlo como colección persistida directa.

### Cuidado con incidencias

Si `Partido.Incidencias` se ignora en EF, asegurar que:

- `Incidencia.PartidoId` se persiste.
- `ServicioIncidencia.ObtenerIncidencias(partidoId)` funciona.
- La UI carga las incidencias usando el servicio, no esperando que vengan dentro de `Partido.Incidencias`.

### Cuidado con roles de usuario

Si los roles se serializan en una columna, asegurar que:

- se guardan todos los roles
- se recuperan correctamente
- `Periodista` queda incluido
- no se persiste contraseña en texto plano

---

## Repositorios SQL prioritarios pendientes

### `RepositorioPartidoSql`

Debe permitir:

- agregar partidos de grupo y eliminatorios
- buscar por ID
- actualizar fecha, estadio, resultado y ganador por penales
- recuperar equipos relacionados
- recuperar estadio
- recuperar grupo si corresponde
- preservar IDs determinísticos si aplica

### `RepositorioGrupoSql`

Debe permitir:

- agregar grupos A-L
- recuperar equipos del grupo
- recuperar partidos del grupo
- evitar duplicar grupos por etiqueta

### Persistencia de configuración

Debe permitir:

- guardar configuración actual
- guardar configuración usada para fixture
- bloquear modificación tras fixture generado
- recuperar al reiniciar

### Persistencia de estado del torneo

Definir un mecanismo claro para saber:

- si hay fixture activo
- si hay eliminatorias generadas
- si el torneo finalizó
- qué motor de simulación fue elegido
- cuál es la configuración usada

Esto puede implementarse con una entidad `Torneo`, `EstadoTorneoPersistido` o similar, si el equipo lo decide.

---

## Inyección de dependencias

Revisar `Program.cs` después de cada cambio.

Reglas:

- `SqlContext`: `Scoped`.
- Repositorios SQL: `Scoped`.
- Servicios que usan repositorios SQL: `Scoped`.
- Servicios puramente algorítmicos sin estado mutable ni dependencias scoped pueden ser `Singleton`, pero revisar caso a caso.
- Evitar servicios con estado mutable global que deberían persistirse en BD.

Cuando se migren `Grupo` y `Partido`, `ServicioPersistenciaFixture` debería recibir repositorios SQL.

---

## Migraciones

Antes de generar migraciones:

- Confirmar que el modelo compila.
- Confirmar que `SqlContext` incluye todas las entidades persistibles.
- Confirmar que `Program.cs` puede construir el contexto.
- Confirmar que el connection string existe.

Después de generar migraciones:

- Revisar el archivo `.cs` de migración.
- Revisar `SqlContextModelSnapshot`.
- Confirmar que no se crean columnas duplicadas innecesarias.
- Confirmar que no se borran tablas por accidente.
- Confirmar que relaciones y cascadas son razonables.

---

## Testing

Tests recomendados para esta skill:

### RepositorioPartidoSql

- `AgregarPartidoSql_DebePersistirPartidoConEquiposYEstadio`
- `ActualizarResultadoSql_DebeGuardarGolesYGanadorPorPenales`
- `BuscarPartidoSql_DebeIncluirEquiposEstadioYGrupo`
- `AgregarPartidoSql_NoDebeCambiarIdDeterministico`

### RepositorioGrupoSql

- `AgregarGrupoSql_DebePersistirEquiposDelGrupo`
- `BuscarGrupoSql_DebeCargarPartidos`
- `AgregarGrupoDuplicado_DebeEvitarDuplicadoPorEtiqueta`

### Persistencia de fixture

- `GuardarFixtureSql_DebePersistirGruposYPartidos`
- `GuardarFixtureSql_AlRecrearContexto_DebeRecuperarFixture`
- `GuardarPartidosSql_ConResultado_DebePersistirResultadoTrasReinicio`

### Notificaciones e incidencias

- `MarcarNotificacionLeidaSql_DebePersistirLectura`
- `AgregarIncidenciaSql_DebePersistirTipoMinutoEquipoYPartido`

---

## Errores comunes a evitar

- Dejar `Grupo` y `Partido` solo en memoria para la entrega final.
- Dejar `EstadoTorneo` como única fuente de verdad.
- Registrar repositorios SQL como `Singleton`.
- Registrar servicios `Singleton` que dependen de repositorios `Scoped`.
- Inyectar `SqlContext` en servicios de negocio.
- Acceder a `SqlContext` directamente desde páginas `.razor`.
- Mover reglas de negocio desde servicios hacia repositorios.
- Romper IDs determinísticos de partidos.
- Persistir propiedades calculadas como si fueran columnas reales.
- Ignorar relaciones necesarias y luego perder datos al reiniciar.
- Usar EF InMemory en producción.
- No revisar migraciones generadas.
- No probar con DBeaver.
- No entregar scripts/respaldos vacío y con datos.
- Dejar DTOs desactualizados respecto a entidades persistidas si se usan en el flujo.
- Hacer cambios grandes sin tests.
- Aceptar cambios masivos sin diagnóstico, plan incremental y revisión humana.

---

## Formato de respuesta esperado

Cuando una IA use esta skill para responder una tarea concreta, debe responder así:

```md
## Análisis

Breve explicación del cambio solicitado y capas afectadas.

## Estado actual relevante

Qué archivos/clases existentes influyen en el cambio.

## Plan de cambios

1. Archivo/capa a modificar.
2. Archivo/capa a crear.
3. Tests a agregar o modificar.

## Implementación propuesta

Código o instrucciones concretas.

## Migración / configuración EF

Comandos o ajustes necesarios.

## Tests

Casos de prueba sugeridos o código de tests.

## Riesgos y supuestos

Cualquier supuesto tomado o riesgo detectado.

## Checklist final

- [ ] Compila.
- [ ] Tests pasan.
- [ ] Servicios no dependen de DbContext.
- [ ] Repositorios SQL implementan interfaces existentes.
- [ ] No queda ningún dato requerido solo en memoria.
- [ ] Datos se guardan durante la operativa.
- [ ] La app puede reconstruirse desde la BD al reiniciar.
- [ ] La BD se puede ver en DBeaver.
- [ ] Si el cambio era masivo, se detuvo y propuso plan antes de modificar.
```

---

## Checklist final de migración

- [ ] `SqlContext` contiene todos los `DbSet` necesarios.
- [ ] Existen repositorios SQL para todas las entidades persistibles.
- [ ] `Grupo` no queda solo en memoria.
- [ ] `Partido` no queda solo en memoria.
- [ ] `ConfiguracionTorneo` se persiste.
- [ ] El fixture activo se reconstruye desde BD.
- [ ] Los partidos eliminatorios se reconstruyen desde BD.
- [ ] Resultados manuales se persisten.
- [ ] Resultados simulados se persisten.
- [ ] Incidencias se persisten.
- [ ] Notificaciones se persisten y su lectura también.
- [ ] Auditorías se persisten con timestamp correcto.
- [ ] `EstadoTorneo`, si existe, no es fuente de verdad final.
- [ ] Los servicios no dependen de EF Core.
- [ ] Los repositorios SQL son `Scoped`.
- [ ] No hay `Singleton` dependiendo de `Scoped`.
- [ ] Migraciones revisadas.
- [ ] Docker levanta Azure SQL Edge.
- [ ] DBeaver muestra datos esperados.
- [ ] Existen scripts/respaldos vacío y con datos de prueba.
- [ ] Tests de repositorio SQL agregados.
- [ ] Funcionalidades de Parte 1 siguen funcionando.
