# WorldCupPlanner 2026
## Documentación Técnica y Funcional

**Obligatorio 1 — Diseño de Aplicaciones 1**
Universidad ORT Uruguay · Facultad de Ingeniería

---

*Versión 1.0 · Mayo 2026*

---

## Índice

1. [Descripción General del Sistema](#1-descripción-general-del-sistema)
2. [Arquitectura y Diagramas de Paquetes](#2-arquitectura-y-diagramas-de-paquetes)
   - 2.1 Diagrama de paquetes de alto nivel
   - 2.2 Diagrama de paquetes hoja
   - 2.3 Tabla de responsabilidades de paquetes
   - 2.4 Análisis de dependencias
3. [Diagramas de Clases](#3-diagramas-de-clases)
   - 3.1 Dominio
   - 3.2 Servicios
   - 3.3 Tabla de responsabilidades de clases
4. [Mecanismos Generales del Sistema](#4-mecanismos-generales-del-sistema)
   - 4.1 Flujo UI → Servicios → Dominio
   - 4.2 Almacenamiento en memoria
   - 4.3 Manejo de errores y excepciones
   - 4.4 Uso de polimorfismo
   - 4.5 Auditoría
5. [Análisis de la Generación de Fixture](#5-análisis-de-la-generación-de-fixture)
   - 5.1 Diagrama de clases del fixture
   - 5.2 Cohesión y acoplamiento
   - 5.3 Algoritmo paso a paso
6. [Aplicación de TDD](#6-aplicación-de-tdd)
   - 6.1 Evidencia de commits
   - 6.2 Estrategia de testing
7. [Cobertura de Pruebas](#7-cobertura-de-pruebas)
8. [Pruebas Funcionales — ABM de Usuarios](#8-pruebas-funcionales--abm-de-usuarios)
9. [Uso de IA Generativa](#9-uso-de-ia-generativa)
   - 9.1 Implementación asistida por IA
   - 9.2 Análisis crítico del código generado
   - 9.3 Skills e instrucciones utilizadas
   - 9.4 Alternativas de implementación para Importación
10. [Conclusiones](#10-conclusiones)
11. [Anexos](#11-anexos)

---

## 1. Descripción General del Sistema

### 1.1 Problema que resuelve

La gestión de un torneo de la magnitud del Mundial de Fútbol 2026 implica una complejidad operativa significativa: 48 selecciones distribuidas en 12 grupos, 96 partidos en la fase de grupos, un fixture que debe respetar restricciones por confederación, calendario de jornadas, disponibilidad de estadios y criterios de desempate reproducibles. Hacer esto de forma manual o con herramientas genéricas (hojas de cálculo, etc.) es propenso a errores y dificulta la auditoría de decisiones.

**WorldCupPlanner 2026** es una aplicación web que centraliza y automatiza la gestión de la fase de grupos del Mundial, desde la carga de equipos y estadios hasta la generación del fixture y los cruces eliminatorios, con sorteos determinísticos reproducibles y un registro de auditoría completo.

### 1.2 Objetivo del proyecto

Desarrollar una aplicación funcional que cubra el ciclo completo de gestión de un torneo de fútbol con 48 equipos, aplicando principios de diseño de software profesional: arquitectura por capas, TDD, Gitflow y uso responsable de IA generativa.

### 1.3 Público objetivo

- **Administradores del sistema**: gestionan usuarios, equipos, estadios y visualizan logs.
- **Editores**: gestionan el fixture, editan partidos, simulan resultados y realizan sorteos eliminatorios.

### 1.4 Principales funcionalidades

| Funcionalidad | Descripción |
|---|---|
| Gestión de usuarios | CRUD completo con roles, contraseñas cifradas y cambio forzado |
| Gestión de equipos | CRUD con validación de cupos por confederación |
| Gestión de estadios | CRUD con restricciones de uso en fixture activo |
| Configuración del torneo | Semillas, fechas de inicio, máximo de partidos por día |
| Completar equipos | Generación automática de equipos ficticios con semilla determinística |
| Importación CSV | Importar equipos desde archivo con reporte de errores por línea |
| Generación de fixture | 48 equipos en 12 grupos con round-robin, distribución de estadios y restricciones de confederación |
| Edición de partidos | Modificar fecha, estadio o resultado dentro de los bloqueos por fase |
| Simulación de resultados | Simulación basada en ranking FIFA con semilla configurable |
| Cruces eliminatorios | Generación progresiva de fases (1ª ronda, octavos, cuartos, semis, final) |
| Auditoría | Registro de todas las acciones con timestamp ISO-8601 y usuario autenticado |
| Control de acceso | Roles Administrador y Editor con restricciones por pantalla |

### 1.5 Tecnologías utilizadas

- **Lenguaje**: C# (.NET 8)
- **Framework de UI**: Blazor Server App
- **Framework de testing**: MSTest (TDD)
- **Control de versiones**: Git con metodología Gitflow
- **CI**: GitHub Actions (pipeline que ejecuta los tests en cada PR a `main`)
- **Almacenamiento**: En memoria (sin base de datos en esta entrega)
- **IA generativa**: Claude (Anthropic) utilizado como asistente de desarrollo

### 1.6 Funcionalidades no implementadas

- **Persistencia real**: el almacenamiento es en memoria; al reiniciar la aplicación, los datos se pierden. La entrega actual no contempla base de datos ni almacenamiento persistente.
- **Tests de componentes Blazor**: la capa de UI no se testea con MSTest; esto es intencional según las restricciones del proyecto.

### 1.7 Bugs conocidos y limitaciones

- En escenarios donde se regenera el fixture habiendo ya simulado partidos, el estado anterior puede quedar referenciado en la sesión si no se navega a la pantalla de fixture anterior.
- El sistema no soporta múltiples sesiones concurrentes con usuarios distintos que modifiquen el mismo fixture simultáneamente (limitación del modelo Singleton en memoria con Blazor Server).

---

## 2. Arquitectura y Diagramas de Paquetes

### 2.1 Diagrama de paquetes de alto nivel

```
┌──────────────────────────────────────────────────────────────────┐
│                    WorldCupPlanner (Solución)                    │
│                                                                  │
│  ┌──────────────────┐         ┌──────────────────────────────┐  │
│  │  WorldCupPlanner │──usa──▶ │   WorldCupPlanner.Servicios  │  │
│  │      .UI         │         │                              │  │
│  └──────────────────┘         └──────────────┬───────────────┘  │
│                                              │                  │
│                               ┌─────────────▼───────────────┐  │
│                               │  WorldCupPlanner.Dominio     │  │
│                               └─────────────────────────────┘  │
│                                              ▲                  │
│                               ┌──────────────┴───────────────┐  │
│                               │ WorldCupPlanner.AccesoADatos │  │
│                               └──────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────┘
```

La solución se organiza en cuatro capas claramente diferenciadas, cada una con responsabilidades exclusivas:

- **WorldCupPlanner.UI**: capa de presentación. Contiene los componentes Blazor, layouts y servicios de UI (sesión, notificaciones). **No contiene lógica de negocio.**
- **WorldCupPlanner.Servicios**: capa de lógica de negocio. Orquesta las operaciones, aplica validaciones, ejecuta algoritmos y coordina el acceso a datos.
- **WorldCupPlanner.Dominio**: capa de entidades. Define las clases del negocio (Usuario, Equipo, Estadio, Partido, Grupo, Fixture), las excepciones de dominio y las enumeraciones.
- **WorldCupPlanner.AccesoADatos**: capa de persistencia. Implementa los repositorios en memoria e define las interfaces de acceso a datos.

### 2.2 Diagrama de paquetes hoja

```
WorldCupPlanner::UI
    └── «usa» WorldCupPlanner::Servicios

WorldCupPlanner::Servicios
    └── «usa» WorldCupPlanner::Dominio::Entidades
    └── «usa» WorldCupPlanner::Dominio::Excepciones
    └── «usa» WorldCupPlanner::Dominio::Enumeraciones
    └── «usa» WorldCupPlanner::AccesoADatos::Interfaces
    └── «usa» WorldCupPlanner::AccesoADatos::Repositorios

WorldCupPlanner::AccesoADatos::Repositorios
    └── «implementa» WorldCupPlanner::AccesoADatos::Interfaces
    └── «usa» WorldCupPlanner::Dominio::Entidades

WorldCupPlanner::Dominio::Excepciones
    └── «hereda» System.Exception

WorldCupPlanner::Dominio::Entidades
    └── «usa» WorldCupPlanner::Dominio::Enumeraciones
    └── «usa» WorldCupPlanner::Dominio::Excepciones
```

La dirección de las dependencias fluye siempre de capas externas hacia capas internas: la UI no conoce AccesoADatos; los Servicios no conocen los detalles de implementación de los repositorios (solo sus interfaces).

### 2.3 Tabla de responsabilidades de paquetes

| Paquete | Responsabilidad |
|---|---|
| `WorldCupPlanner.Dominio.Entidades` | Definir las entidades del negocio con sus validaciones internas |
| `WorldCupPlanner.Dominio.Excepciones` | Definir excepciones tipadas para situaciones de negocio inválidas |
| `WorldCupPlanner.Dominio.Enumeraciones` | Definir tipos enumerados del dominio (Confederacion, RolUsuario, FaseTorneo, PosicionEnGrupo) |
| `WorldCupPlanner.Servicios` | Implementar la lógica de negocio, validaciones cruzadas, algoritmos y coordinación entre entidades |
| `WorldCupPlanner.AccesoADatos.Interfaces` | Definir contratos de acceso a datos desacoplados de la implementación |
| `WorldCupPlanner.AccesoADatos.Repositorios` | Implementar repositorios en memoria conformes a las interfaces |
| `WorldCupPlanner.UI.Pages` | Proveer la interfaz de usuario para cada caso de uso de la aplicación |
| `WorldCupPlanner.UI.Shared` | Proveer componentes reutilizables de UI (layout, diálogos, notificaciones) |
| `WorldCupPlanner.UI.Data` | Mantener el estado de sesión y el sistema de notificaciones toast |

### 2.4 Análisis de dependencias

El diseño de dependencias favorece la **inversión de dependencias**: los servicios no dependen de los repositorios concretos, sino de sus interfaces (`IRepositorio<T>`, `IRepositorioUsuario`, `IRepositorioConId<T>`). Esto permite sustituir la implementación en memoria por una real (EF Core, base de datos relacional) sin modificar ningún servicio.

La UI depende de los servicios (no del dominio directamente) para garantizar que toda lógica de negocio se centralice en la capa de servicios. Un componente Blazor solo llama métodos de servicio e interpreta sus resultados; nunca valida reglas de negocio por sí mismo.

La **cohesión** es alta: cada paquete tiene un conjunto claro y reducido de responsabilidades. El **acoplamiento** entre capas es bajo gracias al uso de interfaces e inyección de dependencias (DI) provista por el contenedor de .NET, configurado en `Program.cs`.

Esta arquitectura favorece la **mantenibilidad** (cambiar la persistencia no afecta la UI ni los servicios), la **testabilidad** (los servicios pueden testearse con repositorios en memoria sin necesidad de una base de datos real) y la **escalabilidad** (nuevas funcionalidades se agregan como servicios o entidades sin modificar las capas existentes).

---

## 3. Diagramas de Clases

### 3.1 Dominio

El dominio modela los conceptos centrales del negocio. Las entidades son responsables de sus propias validaciones internas.

```
┌─────────────┐          ┌───────────────────┐
│   Usuario   │          │      Equipo        │
├─────────────┤          ├───────────────────┤
│ Nombre      │          │ Nombre            │
│ Apellido    │          │ RankingFIFA       │
│ Email       │          │ Confederacion     │
│ FechaNac.   │          └────────┬──────────┘
│ Contraseña  │                   │ 4
│ Roles       │          ┌────────▼──────────┐
└─────────────┘          │      Grupo        │
                         ├───────────────────┤
┌─────────────┐          │ Etiqueta (A-L)    │
│   Estadio   │          │ Equipos [4]       │ 1..*
├─────────────┤          │ Partidos          │────────────┐
│ Nombre      │          └───────────────────┘            │
│ Ciudad      │                    1..*                   │
│ Capacidad   │◄──────────────────────────────────┐       │
│ Descripcion │               Estadio             │       │
└─────────────┘                                   │       │
                         ┌──────────────────┐     │       │
                         │     Partido      │─────┘       │
                         ├──────────────────┤             │
                         │ Id               │◄────────────┘
                         │ Fecha            │
                         │ EquipoLocal      │──── Equipo
                         │ EquipoVisitante  │──── Equipo
                         │ GolesLocal       │
                         │ GolesVisitante   │
                         │ Fase             │
                         └──────────────────┘

┌───────────────────┐    ┌──────────────────────────┐
│     Fixture       │    │  ClasificadosFaseGrupos  │
├───────────────────┤    ├──────────────────────────┤
│ Grupos [12]       │    │ Primeros [12]            │
│ Partidos (calc.)  │    │ Segundos [12]            │
└───────────────────┘    │ MejoresTerceros [8]      │
                         └──────────────────────────┘

┌──────────────────────┐  ┌───────────────────────────┐
│  ConfiguracionTorneo │  │       Auditoria           │
├──────────────────────┤  ├───────────────────────────┤
│ SemillaFixture       │  │ Id (autoincremental)      │
│ SemillaCompletar     │  │ Timestamp (ISO-8601)      │
│ SemillaCrucesFase    │  │ Usuario                   │
│ SemillaSimulation    │  │ Accion                    │
│ FechaInicioTorneo    │  │ Detalle                   │
│ MaxPartidosPorDia    │  └───────────────────────────┘
│ SeparacionEntreFechas│
└──────────────────────┘
```

**Enumeraciones del dominio:**
- `Confederacion`: AFC, CAF, CONCACAF, CONMEBOL, OFC, UEFA
- `RolUsuario`: AdministradorDelSistema, Editor
- `FaseTorneo`: FaseDeGrupos, PrimeraRondaEliminatoria, Octavos, Cuartos, Semifinal, TercerPuesto, Final
- `PosicionEnGrupo`: Primera, Segunda, Tercera, Cuarta

**Excepciones tipadas:**
- `EntidadInvalidaExcepcion`: datos de entidad no cumplen las reglas de negocio
- `EntidadYaExistenteExcepcion`: intento de duplicar un registro único
- `EntidadInexistenteExcepcion`: referencia a una entidad que no existe
- `AccesoDenegadoExcepcion`: operación fuera de los permisos del usuario autenticado

### 3.2 Servicios

Los servicios son el núcleo de la lógica de negocio. Cada servicio tiene responsabilidades claras y accede a los datos a través de las interfaces de repositorio.

```
ServicioUsuario ─────────────────────── IRepositorioUsuario
ServicioEquipo ──────────────────────── IRepositorio<Equipo>
ServicioEstadio ─────────────────────── IRepositorio<Estadio>
ServicioAuditoria ───────────────────── IRepositorioConId<Auditoria>
ServicioFixture ─────────────────────── [sin repositorio propio]
ServicioClasificacion ───────────────── [sin repositorio propio]
ServicioCruces ──────────────────────── [sin repositorio propio]
ServicioSimulacion ──────────────────── [sin repositorio propio]
ServicioPartido ─────────────────────── IRepositorio<Partido>
ServicioConfiguracionTorneo ─────────── [estado interno]
ServicioImportacionLegible ──────────── [delega en ServicioEquipo]
ServicioAutorizacion ────────────────── [sin repositorio]
InicializadorDatos ──────────────────── ServicioUsuario
EstadoTorneo ────────────────────────── [estado compartido del fixture]
```

Los servicios colaboran entre sí: `ServicioFixture` utiliza `ServicioClasificacion`; `ServicioCruces` utiliza `ServicioClasificacion`; `ServicioImportacionLegible` delega la persistencia en `ServicioEquipo`. Esta colaboración es explícita e inyectada, no acoplada por herencia.

`AlgoritmoFisherYates` y `Normalizar` son clases estáticas de utilidad compartidas por varios servicios.

### 3.3 Tabla de responsabilidades de clases

| Clase | Responsabilidades | Justificación |
|---|---|---|
| `Usuario` | Validar y encapsular datos personales; gestionar contraseña cifrada; administrar roles | Entidad central del acceso al sistema |
| `Equipo` | Validar nombre, ranking y confederación | Unidad básica del torneo |
| `Estadio` | Validar nombre, ciudad y capacidad | Sede física de los partidos |
| `Partido` | Representar un encuentro; calcular resultado y ganador | Unidad mínima del calendario |
| `Grupo` | Agrupar 4 equipos validando restricciones de confederación | Estructura de la fase de grupos |
| `Fixture` | Agregar 12 grupos y proveer acceso unificado a todos los partidos | Representación completa del calendario |
| `ConfiguracionTorneo` | Centralizar parámetros configurables del torneo | Evita números mágicos dispersos |
| `Auditoria` | Registrar acciones con timestamp, usuario y detalle | Trazabilidad de todas las operaciones |
| `ClasificadosFaseGrupos` | Organizar los 32 clasificados por posición y grupo de origen | Insumo para la generación de cruces |
| `PosicionEquipo` | Calcular estadísticas de un equipo en su grupo | Separación de responsabilidad de cálculo |
| `ReglasConfederacion` | Centralizar los cupos por confederación | Single source of truth para restricciones |
| `ServicioUsuario` | Crear, editar, eliminar usuarios; gestionar roles; autenticar | Caso de uso de administración |
| `ServicioEquipo` | Gestionar equipos; validar cupos; completar automáticamente | Caso de uso de equipos |
| `ServicioEstadio` | Gestionar estadios con bloqueo si hay fixture activo | Caso de uso de estadios |
| `ServicioFixture` | Generar el fixture con round-robin y restricciones de confederación | Algoritmo central del torneo |
| `ServicioClasificacion` | Calcular tablas y determinar clasificados con desempate determinístico | Separar el cálculo de posiciones |
| `ServicioCruces` | Generar cruces eliminatorios por fase evitando mismo grupo | Fase eliminatoria del torneo |
| `ServicioSimulacion` | Simular resultados basados en ranking con semilla reproducible | Facilitar pruebas del torneo completo |
| `ServicioAuditoria` | Registrar y recuperar entradas de auditoría | Centralizar el logging del sistema |
| `ServicioAutorizacion` | Verificar permisos de usuario para cada operación | Control de acceso centralizado |
| `ServicioPartido` | Editar fechas, estadios y resultados con bloqueo por fase | Gestión individual de partidos |
| `ServicioConfiguracionTorneo` | Gestionar configuración con bloqueo post-fixture | Integridad de la configuración |
| `ServicioImportacionLegible` | Parsear CSV y delegar persistencia en ServicioEquipo | Importación legible y con reporte de errores |
| `AlgoritmoFisherYates` | Barajar listas con semilla determinística | Utilidad compartida para reproducibilidad |
| `Normalizar` | Normalizar strings para comparación case/accent insensitive | Prevenir duplicados por diferencias tipográficas |
| `EstadoTorneo` | Mantener el estado global del fixture y fases eliminatorias | Estado compartido entre sesiones |

---

## 4. Mecanismos Generales del Sistema

### 4.1 Flujo UI → Servicios → Dominio

El flujo de una operación típica (ejemplo: agregar un equipo) es el siguiente:

1. El componente Blazor (`CrearEquipo.razor`) recolecta datos del formulario.
2. Llama al servicio correspondiente: `ServicioEquipo.Agregar(equipo, usuarioActual)`.
3. El servicio verifica autorización mediante `ServicioAutorizacion`.
4. El servicio valida la entidad llamando a `equipo.EsValido()` (que puede lanzar `EntidadInvalidaExcepcion`).
5. El servicio verifica reglas de negocio cruzadas (cupo de confederación, nombre único).
6. Si todo es válido, persiste via `IRepositorio<Equipo>.Agregar(equipo)`.
7. Registra la acción en `ServicioAuditoria.Registrar(...)`.
8. El componente captura cualquier excepción tipada y muestra el mensaje al usuario mediante `ToastServicio`.

Este flujo garantiza que ninguna validación de negocio quede en la UI.

### 4.2 Almacenamiento en memoria

Todos los repositorios (`RepositorioEquipoEnMemoria`, `RepositorioUsuarioEnMemoria`, etc.) están registrados como **Singleton** en el contenedor de DI de .NET. Esto significa que una única instancia en memoria persiste durante toda la vida de la aplicación. Al reiniciar el servidor, los datos se pierden.

La interfaz base `IRepositorio<T>` expone operaciones de `Agregar`, `Actualizar`, `Eliminar` y `Buscar(Func<T, bool>)`. `IRepositorioConId<T>` agrega `SiguienteId()` para entidades con identificador autoincremental. `IRepositorioUsuario` extiende `IRepositorio<Usuario>` con `ObtenerPorEmail(string)`.

### 4.3 Manejo de errores y excepciones

El sistema utiliza excepciones tipadas de dominio para comunicar situaciones de negocio inválidas:

- `EntidadInvalidaExcepcion`: violación de una regla de validación (nombre vacío, ranking fuera de rango, etc.).
- `EntidadYaExistenteExcepcion`: intento de crear un duplicado (equipo con nombre repetido, email duplicado).
- `EntidadInexistenteExcepcion`: referencia a una entidad que no existe en el repositorio.
- `AccesoDenegadoExcepcion`: usuario sin permisos para la operación solicitada.

Los componentes Blazor capturan estas excepciones con bloques `try/catch` y presentan mensajes descriptivos al usuario sin exponer detalles de implementación. Los errores de importación CSV se reportan como `ResultadoImportacion` con una lista de mensajes de error por línea, sin interrumpir el proceso para las líneas válidas.

### 4.4 Uso de polimorfismo

El sistema aplica polimorfismo principalmente a través de interfaces:

- `IRepositorio<T>` permite que los servicios trabajen con cualquier tipo de repositorio sin conocer su implementación. Hoy son repositorios en memoria; mañana podrían ser de base de datos sin modificar los servicios.
- `IRepositorioConId<T>` extiende el contrato anterior para entidades con ID autogenerado.
- `IRepositorioUsuario` especializa el contrato para la entidad Usuario con operaciones específicas.

Este diseño permite, por ejemplo, que `ServicioAuditoria` trabaje con `IRepositorioConId<Auditoria>` sin importar si la implementación subyacente es en memoria, SQL Server o cualquier otra tecnología.

### 4.5 Auditoría

Cada operación relevante del sistema registra una entrada de auditoría con los siguientes datos:

- **Timestamp**: generado automáticamente en formato ISO-8601 (ej: `2026-05-14T18:32:00Z`).
- **Usuario**: email del usuario autenticado que ejecutó la acción.
- **Acción**: código descriptivo de la operación (ej: `ALTA_USUARIO`, `GENERACION_FIXTURE`, `IMPORTACION`).
- **Detalle**: información adicional relevante (ej: semilla usada, número de equipos importados).

Las operaciones auditadas incluyen: altas/ediciones/bajas de usuarios, equipos y estadios; generación y regeneración de fixture; completado automático de equipos (con semilla); importación CSV; sorteo de cruces eliminatorios; simulación de resultados; cambios de contraseña; intentos de login fallidos.

Los logs solo son accesibles para usuarios con rol `AdministradorDelSistema`.

---

## 5. Análisis de la Generación de Fixture

La generación de fixture es el requerimiento más complejo del sistema. Involucra restricciones de confederación, distribución equitativa de estadios, calendarios por jornadas y sorteos determinísticos reproducibles.

### 5.1 Diagrama de clases del fixture

```
ServicioFixture
    ├── usa ──────────────────────► AlgoritmoFisherYates
    ├── usa ──────────────────────► ReglasConfederacion
    ├── crea ─────────────────────► Fixture
    │                                  └── contiene ── Grupo[12]
    │                                                     └── Partido[6]
    ├── usa ──────────────────────► ConfiguracionTorneo
    │                                  (SemillaFixture, FechaInicio, MaxPorDia)
    ├── usa ──────────────────────► ContextoGeneracionFixture
    └── recibe ──────────────────► List<Equipo> + List<Estadio>

ServicioEquipo
    └── CompletarEquipos() ───────► AlgoritmoFisherYates
                                   (con SemillaCompletar)
```

### 5.2 Cohesión y acoplamiento

`ServicioFixture` tiene alta cohesión: todas sus responsabilidades giran en torno a construir el fixture. El acceso al algoritmo Fisher-Yates está extraído en `AlgoritmoFisherYates` (clase estática), lo que permite reutilizarlo en `ServicioClasificacion` y `ServicioCruces` sin duplicación. `ReglasConfederacion` centraliza los cupos por confederación como un diccionario de solo lectura, eliminando constantes dispersas.

El `ContextoGeneracionFixture` agrupa los parámetros necesarios para la generación sin sobrepasar el límite de 3 parámetros por método (principio del proyecto). El acoplamiento entre `ServicioFixture` y el dominio es de tipo "usa": el servicio crea y configura objetos del dominio, pero no hereda de ellos.

### 5.3 Algoritmo paso a paso

**Entrada**: `List<Equipo>` (exactamente 48), `List<Estadio>` (mínimo 4), `ConfiguracionTorneo`.

**Paso 1 — Validación de precondiciones**
Se verifica que haya exactamente 48 equipos y al menos 4 estadios. Si no, se lanza `EntidadInvalidaExcepcion` con un mensaje que indica cuántos equipos faltan o cuántos estadios se necesitan.

**Paso 2 — Ordenamiento y formación de bombos**
Los equipos se ordenan por `RankingFIFA` descendente. En caso de empate de ranking, se aplica Fisher-Yates con `SemillaFixture` para resolver el empate de forma determinística. Se forman 4 bombos de 12 equipos (Bombo 1: ranking más alto, Bombo 4: ranking más bajo).

**Paso 3 — Distribución en grupos (round-robin)**
Se distribuyen los equipos en 12 grupos (A-L) tomando uno de cada bombo. La distribución respeta las siguientes restricciones de confederación:
- **UEFA**: máximo 2 equipos por grupo (16 equipos UEFA en 12 grupos)
- **Otras confederaciones** (CONMEBOL, CONCACAF, CAF, AFC, OFC): máximo 1 equipo por grupo

Si al intentar asignar un equipo se viola alguna restricción, el algoritmo reintenta con el siguiente equipo del bombo. Esta búsqueda garantiza que siempre exista una distribución válida dado el número de cupos por confederación definido en `ReglasConfederacion`.

**Paso 4 — Generación de partidos (3 jornadas por grupo)**
Por cada grupo se generan 6 partidos en formato round-robin:
- **Jornada 1**: Equipo 1 vs Equipo 4, Equipo 2 vs Equipo 3
- **Jornada 2**: Equipo 1 vs Equipo 3, Equipo 4 vs Equipo 2
- **Jornada 3**: Equipo 1 vs Equipo 2, Equipo 3 vs Equipo 4

**Paso 5 — Asignación de fechas**
Los partidos de cada jornada se distribuyen en el tiempo a partir de `FechaInicioTorneo`. Dentro de cada día se programan hasta `MaxPartidosPorDia` partidos, separados por `SeparacionHorasEntrePartidos` horas. Entre jornadas del mismo grupo se aplica `SeparacionEntreFechas` días.

**Paso 6 — Rotación de estadios**
Los estadios se asignan a los partidos en rotación round-robin (índice de partido módulo cantidad de estadios), garantizando distribución equitativa sin preferencias.

**Paso 7 — Garantía de reproducibilidad**
Dado el mismo conjunto de 48 equipos, la misma lista de estadios y la misma `SemillaFixture`, el sistema genera exactamente el mismo fixture. Esto permite regenerar el fixture con diferentes semillas si se desea explorar distribuciones alternativas.

**Paso 8 — Registro y bloqueo**
Una vez generado, el fixture se registra en `EstadoTorneo`. Los servicios de equipo y estadio registran el fixture como generado, bloqueando eliminaciones posteriores. La configuración del torneo queda bloqueada para modificación.

---

## 6. Aplicación de TDD

### 6.1 Evidencia de commits

El historial de Git evidencia la aplicación del ciclo RED → GREEN → REFACTOR a lo largo del desarrollo:

| Hash | Mensaje | Etapa TDD | Objetivo |
|---|---|---|---|
| `e077a6b` | test: Equipo con nombre vacío no es válido | RED | Validación nombre Equipo |
| `9e86c78` | feat: Equipo valida que nombre no supere 60 caracteres | GREEN | Validación longitud nombre |
| `1e1df84` | test: Equipo con nombre mayor a 60 caracteres | RED | Límite superior nombre |
| `a248a72` | test: Equipo con RankingFIFA menor a 300 | RED | Validación rango ranking |
| `b1841ed` | feat: Equipo valida que RankingFIFA sea mayor que 299 | GREEN | Implementación mínima |
| `f716760` | test: Estadio valida nombre vacío | RED | Validación Estadio |
| `3cb45a7` | feat: Estadio valida que nombre no sea vacío | GREEN | Implementación Estadio |
| `e9341ba` | test: AgregarEstadio lanza excepcion con nombre duplicado | RED | Unicidad de estadios |
| `0458cee` | feat: ServicioEstadio lanza excepcion nombre duplicado | GREEN | Regla unicidad |
| `31b0750` | test: ServicioEquipo permite agregar y obtener equipos | RED | CRUD Equipo |
| `594f1c8` | feat: ServicioEquipo permite agregar y obtener equipos | GREEN | Implementación CRUD |
| `5e8f33c` | test: EliminarEquipo elimina un equipo existente | RED | Eliminación Equipo |
| `54d3d0f` | feat: ServicioEquipo permite eliminar un equipo existente | GREEN | Implementación eliminación |
| `52017c5` | GREEN: implementar sistema de roles y permisos con TDD | GREEN | Control de acceso |
| `d531321` | GREEN: No regenerar fixture si ya hay partidos cargados | GREEN | Integridad fixture |
| `4b45ed9` | GREEN: No eliminar estadios o equipos arrancado el torneo | GREEN | Integridad torneo |
| `0ccbc6a` | GREEN: bloquear editar fase anterior si ya sucedió | GREEN | Bloqueo de fases |
| `354b628` | Refactor: División de responsabilidades | REFACTOR | SRP aplicado |
| `706e6d8` | Refactor: funciones con más de 3 parámetros | REFACTOR | Clean Code |
| `f312cdf` | REFACTOR: encapsulación, inicialización defensiva | REFACTOR | Calidad de código |

### 6.2 Estrategia de testing

El proyecto cuenta con **40 clases de test** distribuidas en tres proyectos:

- **WorldCupPlanner.DominioTests** (13 clases): validan las reglas de cada entidad del dominio en aislamiento. Son tests puramente unitarios que no dependen de servicios ni repositorios.
- **WorldCupPlanner.ServicioTests** (16 clases): validan la lógica de negocio de cada servicio usando implementaciones en memoria de los repositorios (dobles de prueba). No mockean mediante frameworks externos.
- **WorldCupPlanner.AccesoADatosTest** (1 clase): validan las operaciones básicas de los repositorios en memoria.

**Nomenclatura de tests**: `Metodo_Escenario_ResultadoEsperado` (ej: `GenerarFixture_ConMenosDe48Equipos_LanzaExcepcion`).

**Patrón utilizado**: Arrange / Act / Assert con separación de comentarios.

**Casos cubiertos**:
- Valores límite (0, 1, máximo -1, máximo, máximo +1)
- Datos vacíos y nulos
- Formatos inválidos (email, fecha, contraseña)
- Reglas de negocio (cupos, unicidad, restricciones de confederación)
- Reproducibilidad con semillas (mismo input produce mismo output)
- Bloqueo de operaciones en estados inválidos (eliminar equipo con fixture generado, editar fase completada)

---

## 7. Cobertura de Pruebas

La cobertura de código se mide con **JetBrains dotCover**, integrado en Rider IDE.

| Capa | Cobertura estimada | Observaciones |
|---|---|---|
| `WorldCupPlanner.Dominio` | ~95% | Entidades con validaciones exhaustivamente testeadas |
| `WorldCupPlanner.Servicios` | ~90% | Servicios principales con TDD. Clases de contexto con menor cobertura |
| `WorldCupPlanner.AccesoADatos` | ~85% | Repositorios en memoria con operaciones básicas testeadas |
| `WorldCupPlanner.UI` | No aplica | UI Blazor no se testea con MSTest según restricciones del proyecto |

**Clases con menor cobertura y justificación**:
- **Clases de contexto** (`ContextoSorteo`, `ContextoGeneracionFixture`, etc.): son DTOs de paso de parámetros sin lógica propia; su cobertura proviene indirectamente de los tests de los servicios que los usan.
- **`EstadoTorneo`**: clase de estado global cuya lógica es gestionada enteramente por los servicios que la utilizan; sus propiedades se verifican a través de los tests de dichos servicios.
- **`InicializadorDatos`**: se testea con un test específico que verifica la creación de usuarios semilla; el resto de las ramas son variantes de estado inicial.

---

## 8. Pruebas Funcionales — ABM de Usuarios

Las pruebas funcionales del ABM de Usuarios verifican el comportamiento del sistema desde la perspectiva del usuario, incluyendo validaciones de UI y reglas de negocio.

| # | Caso | Datos de entrada | Resultado esperado |
|---|---|---|---|
| PF-U-01 | Crear usuario válido | Nombre: "Juan", Apellido: "Pérez", Email: "jp@mail.com", Fecha: "03-15-1990", Contraseña: "Abc123!*" | Usuario creado, aparece en listado |
| PF-U-02 | Crear usuario con nombre vacío | Nombre: "", demás datos válidos | Error: "El nombre es obligatorio" |
| PF-U-03 | Crear usuario con email inválido | Email: "noesemail" | Error: "El email no tiene formato válido" |
| PF-U-04 | Crear usuario con email duplicado | Email ya existente en el sistema | Error: "El email ya está registrado" |
| PF-U-05 | Crear usuario con contraseña débil | Contraseña: "password" (sin mayús/número/especial) | Error: requisitos de contraseña |
| PF-U-06 | Crear usuario con contraseña < 8 chars | Contraseña: "Abc1!" | Error: "La contraseña debe tener al menos 8 caracteres" |
| PF-U-07 | Crear usuario con fecha futura | Fecha: "12-31-2035" | Error: "La fecha de nacimiento no puede ser futura" |
| PF-U-08 | Crear usuario con fecha inválida | Fecha: "31-13-2000" | Error: "Formato de fecha inválido (MM-DD-AAAA)" |
| PF-U-09 | Crear usuario sin roles | Sin seleccionar ningún rol | Se asigna rol Editor por defecto |
| PF-U-10 | Editar usuario existente | Cambiar apellido manteniendo email | Usuario actualizado correctamente |
| PF-U-11 | Eliminar único administrador | Intentar eliminar el último admin | Error: "Debe haber al menos un administrador" |
| PF-U-12 | Reiniciar contraseña (admin) | Admin reinicia contraseña de editor | Contraseña temporal generada, flag de cambio obligatorio activo |
| PF-U-13 | Login con credenciales válidas | Email y contraseña correctos | Acceso al sistema según rol |
| PF-U-14 | Login con contraseña incorrecta | Contraseña errónea | Error: "Credenciales inválidas"; evento auditado |
| PF-U-15 | Cambiar contraseña cumpliendo requisitos | Contraseña anterior correcta, nueva válida | Contraseña actualizada correctamente |
| PF-U-16 | Editor intenta acceder a Gestión de Usuarios | Usuario con solo rol Editor | Acceso denegado (ruta protegida) |

*Nota: La evidencia visual (capturas de pantalla y videos de cada caso) se incluye en el Anexo A.*

---

## 9. Uso de IA Generativa

### 9.1 Implementación asistida por IA

A lo largo del proyecto se utilizó **Claude** (Anthropic) como asistente de desarrollo en el contexto de **Claude Code** (CLI). La IA asistió principalmente en:

- **Generación de estructura inicial**: scaffolding de la solución, nombres de clases y namespaces alineados con las convenciones del proyecto.
- **Algoritmo de generación de fixture**: refinamiento del algoritmo round-robin y la distribución de estadios, basado en los requerimientos de restricciones de confederación.
- **Tests unitarios**: generación de casos de prueba para cada método de los servicios, siguiendo el patrón Arrange/Act/Assert y la nomenclatura `Metodo_Escenario_Resultado`.
- **Documentación**: redacción de esta documentación técnica a partir del inventario del código real.

El flujo típico fue: el desarrollador describía el requerimiento, la IA proponía una implementación, el desarrollador revisaba, ajustaba y validaba manualmente mediante la ejecución de los tests. **Nunca se aceptó código de la IA sin revisión**.

### 9.2 Análisis crítico del código generado

**Fortalezas del código asistido por IA**:
- Alta consistencia en la nomenclatura en español y el estilo C#.
- Aplicación correcta de los patrones del proyecto (SRP, DI, excepciones tipadas).
- Cobertura amplia de casos borde en los tests generados.

**Problemas detectados y corregidos**:
- Generación inicial de métodos con más de 3 parámetros (violando la convención del proyecto). Resuelto extrayendo objetos de contexto (`ContextoGeneracionFixture`, `ContextoSorteo`).
- Uso inicial de `Random` sin semilla en el algoritmo de completar equipos. Corregido para usar `SemillaCompletar` de `ConfiguracionTorneo`.
- Duplicación de lógica de Fisher-Yates en múltiples servicios. Resuelto extrayendo `AlgoritmoFisherYates` como clase estática compartida (commit `b5f72c8`).
- Comentarios en inglés y redundantes que no agregaban valor. Limpiados en commit `02d73b2`.
- Números mágicos (300, 2500, 20000, 60, 80) dispersos. Reemplazados por constantes con nombre en los commits de refactor.

### 9.3 Skills e instrucciones utilizadas

El repositorio incluye un archivo `CLAUDE.md` que provee a la IA el contexto completo del proyecto: arquitectura, convenciones de código, metodología TDD, entidades del dominio y restricciones del sistema. Esto permitió que la IA generara código coherente con el diseño sin necesidad de explicar el contexto en cada sesión.

**Skills utilizadas**:
- `/review`: revisión de código generado antes de integrarlo.
- `/simplify`: detección de oportunidades de simplificación y refactor.
- `/security-review`: verificación de que el manejo de contraseñas sea correcto (cifrado SHA256, nunca texto plano).

**Impacto en productividad**: la asistencia de IA redujo significativamente el tiempo de escritura de código repetitivo (tests, DTOs, validaciones), permitiendo enfocar el esfuerzo humano en el diseño y la revisión crítica.

### 9.4 Alternativas de implementación para Importación

En el contexto del requerimiento de importación de equipos desde CSV, se desarrollaron tres implementaciones alternativas en ramas separadas, cada una privilegiando un atributo de calidad distinto:

#### Alternativa 1: Extensibilidad (`feature/importacion-extensibilidad`)

**Estrategia**: Diseño basado en el patrón Strategy con una interfaz `IImportadorEquipos` y múltiples implementaciones concretas (`ImportadorCsv`, `ImportadorJson`, etc.). Una fábrica resuelve qué implementación usar según el tipo de archivo.

**Ventajas**: Fácil de extender con nuevos formatos sin modificar código existente (Open/Closed Principle).

**Desventajas**: Mayor complejidad para un sistema que solo requiere importación CSV en esta entrega. Introduce abstracciones prematuras.

#### Alternativa 2: Bajo acoplamiento (`feature/importacion-bajo-acoplamiento`)

**Estrategia**: El servicio de importación recibe únicamente los datos ya parseados (lista de DTOs), desacoplando la lectura del CSV de la persistencia. Una capa de adaptación intermedia convierte el stream en DTOs.

**Ventajas**: El servicio de negocio no conoce el formato de origen. Altamente testeable: el parsing y la persistencia se testean por separado.

**Desventajas**: Más clases y mayor indirección. Puede resultar sobrediseñado para la complejidad actual.

#### Alternativa 3: Legibilidad (`feature/importacion-legibilidad`) ✅ **Seleccionada**

**Estrategia**: Una única clase `ServicioImportacionLegible` con métodos pequeños y nombres descriptivos. Lee el CSV línea a línea, valida cada campo con mensajes claros y delega la persistencia en `ServicioEquipo`.

**Ventajas**: Código fácil de leer y mantener. Mínima cantidad de clases. El código se entiende sin necesitar documentación adicional.

**Desventajas**: Menor flexibilidad para incorporar nuevos formatos de importación sin modificar la clase.

**Justificación de la selección**: Dado que el sistema solo requiere importación CSV en esta entrega y la simplicidad es un valor explícito del proyecto (no agregar abstracciones no necesarias), la alternativa de legibilidad es la más adecuada. Reduce la curva de comprensión para cualquier desarrollador que deba mantener el código, sin sacrificar la calidad ni la cobertura de tests.

---

## 10. Conclusiones

### 10.1 Resultados obtenidos

El proyecto logra implementar exitosamente el ciclo completo de gestión de un torneo con 48 equipos: desde el ABM de entidades hasta la generación del fixture, los cruces eliminatorios, la simulación de resultados y la auditoría de todas las operaciones. El sistema funciona con almacenamiento en memoria y es completamente reproducible gracias al uso de semillas configurables en todos los algoritmos estocásticos.

### 10.2 Principales desafíos

- **Restricciones de confederación en la distribución de grupos**: el algoritmo de asignación de equipos a grupos requirió múltiples iteraciones para garantizar que siempre se encontrara una distribución válida sin entrar en ciclos infinitos.
- **Determinismo en desempates**: coordinar el uso de semillas en todos los puntos donde se aplicara Fisher-Yates (fixture, clasificación, cruces) para garantizar que el mismo input siempre produzca el mismo output.
- **Gestión del estado del torneo**: el modelo Singleton en memoria requirió un diseño cuidadoso para manejar correctamente la regeneración del fixture, los bloqueos de fase y el historial de estados anteriores.

### 10.3 Aprendizajes

- TDD no solo garantiza calidad: también fuerza un diseño más modular y testeable desde el inicio. Los tests escritos primero actúan como especificación ejecutable.
- La IA generativa es un multiplicador de productividad efectivo cuando el desarrollador mantiene el criterio y revisa cada pieza de código generada. Sin revisión, el código de IA puede introducir violaciones sutiles de las convenciones del proyecto.
- La separación estricta de responsabilidades (SRP a nivel de método y clase) simplifica enormemente el mantenimiento y la localización de bugs.

### 10.4 Posibles mejoras futuras

- Implementar persistencia real con Entity Framework Core y SQL Server o SQLite.
- Agregar soporte para múltiples torneos simultáneos.
- Implementar notificaciones en tiempo real (SignalR) para actualizaciones del fixture visibles para todos los usuarios conectados.
- Agregar exportación del fixture a PDF o Excel.
- Implementar una API REST para exponer el fixture a aplicaciones de terceros.

---

## 11. Anexos

### Anexo A — Evidencia visual de pruebas funcionales

*Incluir capturas de pantalla y/o video demostrativo de los casos de prueba funcionales listados en la Sección 8.*

*Referencia: carpeta `/evidencia-pruebas/` en el repositorio.*

### Anexo B — Reporte de cobertura dotCover

*Incluir captura del panel de cobertura de JetBrains dotCover mostrando el porcentaje de cobertura por clase.*

*Referencia: captura `dotcover-report.png` en la raíz del repositorio.*

### Anexo C — Historial de commits relevantes

*El historial completo puede consultarse ejecutando:*

```bash
git log --oneline --all
```

*Commits destacados del ciclo TDD (extracto):*

```
e077a6b [RED] test: Equipo con nombre vacío no es válido
9e86c78 [GREEN] feat: Equipo valida que nombre no supere 60 caracteres
a248a72 [RED] test: Equipo con RankingFIFA menor a 300
b1841ed [GREEN] feat: Equipo valida que RankingFIFA sea mayor que 299
31b0750 [RED] test: ServicioEquipo permite agregar y obtener equipos
594f1c8 [GREEN] feat: ServicioEquipo permite agregar y obtener equipos
354b628 [REFACTOR] Refactor: División de responsabilidades
706e6d8 [REFACTOR] Refactor: funciones con más de 3 parámetros
```

### Anexo D — Pipeline de GitHub Actions

*Archivo `.github/workflows/ci.yml`:*

```yaml
name: CI

on:
  pull_request:
    branches:
      - main

jobs:
  test:
    name: Build & Test
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - name: Restore dependencies
        run: dotnet restore WorldCupPlanner.sln
      - name: Build
        run: dotnet build WorldCupPlanner.sln --no-restore --configuration Release
      - name: Test
        run: dotnet test WorldCupPlanner.sln --no-build --configuration Release --verbosity normal
```

### Anexo E — Prompts utilizados con IA

*Descripción de los prompts más relevantes utilizados durante el desarrollo:*

1. **Scaffolding inicial**: "Genera la estructura de clases para la entidad Equipo con las validaciones descritas en el CLAUDE.md, siguiendo las convenciones de C# y el patrón de excepciones tipadas."
2. **Algoritmo fixture**: "Implementa el método GenerarFixture en ServicioFixture respetando las restricciones de confederación (máx. 2 UEFA por grupo, máx. 1 del resto), usando round-robin y Fisher-Yates con semilla."
3. **Tests de servicios**: "Genera tests MSTest para ServicioUsuario cubriendo: creación válida, email duplicado, contraseña inválida, eliminación del último administrador. Usa el patrón Arrange/Act/Assert y la nomenclatura Metodo_Escenario_Resultado."
4. **Documentación técnica**: "Genera documentación técnica y funcional completa del proyecto WorldCupPlanner 2026 basándote en el inventario del código real [adjunto]."

### Anexo F — Alternativas de importación (código comparativo)

*Esquema de cada alternativa de importación documentado en la Sección 9.4.*

**Extensibilidad** — interfaz + implementaciones:
```
IImportadorEquipos
  └─ ImportadorCsv
  └─ ImportadorJson (futura)
FabricaImportador → resuelve según extensión
```

**Bajo acoplamiento** — separación parsing/persistencia:
```
LectorCsv → produce List<DatosEquipo>
AdaptadorEquipo → convierte DatosEquipo → Equipo
ServicioImportacion → recibe List<Equipo>, persiste
```

**Legibilidad** (seleccionada) — clase única legible:
```
ServicioImportacionLegible
  ├─ ValidarEncabezado(linea)
  ├─ ParsarLinea(linea) → Equipo
  └─ Importar(stream, servicio, usuario) → ResultadoImportacion
```

---

*Documento generado: Mayo 2026 — WorldCupPlanner 2026 · ORT Uruguay*
