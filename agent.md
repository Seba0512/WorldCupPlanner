# Agent Instructions — WorldCupPlanner 2026

## Contexto general del proyecto

Este repositorio corresponde al Obligatorio 1 de Diseño de Aplicaciones 1.  
El sistema se llama **WorldCupPlanner 2026**.

La aplicación debe gestionar la fase de grupos del Mundial 2026, con 48 equipos, 12 grupos de 4 equipos, generación automática de fixture, preparación de cruces eliminatorios, simulación de resultados, auditoría y trazabilidad mediante semillas determinísticas.

El trabajo debe desarrollarse en **C# con .NET 8 y Blazor Server App**. La lógica de negocio debe estar separada de la interfaz. No usar base de datos en esta primera entrega: toda la información se guarda en memoria mediante objetos, servicios y repositorios en memoria.

## Reglas obligatorias de desarrollo

Todas las clases que no sean UI deben desarrollarse usando **TDD con MSTest**.  

Para el requerimiento **Generación de fixture**, se debe evidenciar claramente el ciclo:

1. RED: escribir primero el test que falla.
2. GREEN: implementar el mínimo código necesario para que el test pase.
3. REFACTOR: mejorar estructura y legibilidad sin cambiar comportamiento.

En los commits del requerimiento Generación de Fixture debe indicarse claramente si el commit corresponde a RED, GREEN o REFACTOR.

Usar mensajes de commit con este estilo:

```text
[RED] test: descripcion del test
[GREEN] feat: descripcion del comportamiento implementado
[REFACTOR] refactor: descripcion de la mejora interna
````

Importante:

Solo el requerimiento de **Generación de Fixture** exige evidenciar explícitamente el ciclo RED / GREEN / REFACTOR en commits separados o claramente identificables.

Para el resto de funcionalidades, también se debe trabajar con TDD porque la letra exige TDD para las clases no UI, pero no es necesario evidenciar cada fase con commits separados. En esos casos se puede hacer RED, GREEN y REFACTOR dentro del mismo flujo o commit, siempre que el código final tenga tests y respete la lógica de TDD.

## Git, Gitflow y GitHub Actions

El código fuente debe gestionarse en GitHub utilizando metodología gitflow.

Se debe configurar un pipeline de GitHub Actions que ejecute los tests cada vez que se realice un pull request hacia la rama principal (`main`).

Aunque no sea obligatorio que los tests aprueben para concretar el merge, el pipeline debe ejecutarse siempre y reportar los resultados de manera visible.

Los commits de Generación de Fixture deben indicar claramente si corresponden a RED, GREEN o REFACTOR.

## Arquitectura actual esperada

La solución actual está organizada en estos proyectos:

* `WorldCupPlanner.Dominio`: entidades, enumeraciones y lógica básica de dominio.
* `WorldCupPlanner.Servicios`: servicios de aplicación y lógica de negocio.
* `WorldCupPlanner.AccesoADatos`: repositorios en memoria para esta primera entrega.
* `WorldCupPlanner.DominioTest`: tests unitarios de dominio con MSTest.
* `WorldCupPlanner.ServicioTests`: tests unitarios de servicios y repositorios con MSTest.
* `WorldCupPlanner.UI`: interfaz Blazor Server.

La lógica de negocio debe ir en `WorldCupPlanner.Dominio` y `WorldCupPlanner.Servicios`.

La persistencia en esta primera entrega debe ser en memoria, mediante repositorios simples en `WorldCupPlanner.AccesoADatos`.

Los tests unitarios deben ir en `WorldCupPlanner.DominioTest` o `WorldCupPlanner.ServicioTests`, según la capa que cubran.

No poner lógica de negocio en la UI.

Pueden existir carpetas viejas del proceso de reestructuración, como `WorldCupPlanner.Domain`, `WorldCupPlanner.Services`, `WorldCupPlanner.DataAccess`, `WorldCupPlanner.Tests` o `WorldCupPlanner.WebApp`. No crear archivos nuevos allí si esos proyectos no están cargados en `WorldCupPlanner.sln`. Tomar como fuente de verdad los proyectos referenciados por la solución actual.

## Persistencia en memoria / AccesoADatos

Para esta primera entrega no se debe usar base de datos.

Actualmente existe un proyecto `WorldCupPlanner.AccesoADatos` con repositorios en memoria:

* `RepositorioEquipoEnMemoria`
* `RepositorioEstadioEnMemoria`
* `RepositorioGrupoEnMemoria`
* `RepositorioPartidoEnMemoria`

Cada repositorio expone operaciones básicas:

* `Agregar`
* `Actualizar`
* `Eliminar`
* `ObtenerTodos`

Los servicios deben usar estos repositorios cuando corresponda, preferentemente mediante inyección por constructor.

Ejemplos actuales:

* `ServicioEquipo` depende de `IRepositorioEquipo` y por defecto instancia `RepositorioEquipoEnMemoria`.
* `ServicioEstadio` usa `RepositorioEstadioEnMemoria`.

No volver a implementar listas internas en servicios si ya existe un repositorio en memoria para esa entidad.

Los repositorios no deben tener reglas de negocio complejas. Las validaciones de negocio deben permanecer en `WorldCupPlanner.Dominio` o `WorldCupPlanner.Servicios`.

## Clases relevantes del dominio

### Equipo

Debe tener:

* Nombre
* RankingFIFA
* Confederacion

Reglas:

* Nombre obligatorio, 1 a 60 caracteres.
* Confederación obligatoria.
* Ranking FIFA obligatorio entre 300 y 2500.
* Nombre único a nivel del sistema.
* Cupos globales por confederación:

  * UEFA: 16
  * CONMEBOL: 7
  * CONCACAF: 7
  * CAF: 9
  * AFC: 8
  * OFC: 1

### Confederacion

Enum con:

* UEFA
* CONMEBOL
* CONCACAF
* CAF
* AFC
* OFC

### Estadio

Debe tener:

* Nombre
* Ciudad
* Descripcion
* Capacidad

Reglas:

* Nombre obligatorio, 1 a 80 caracteres.
* Ciudad obligatoria, 1 a 60 caracteres.
* Descripción opcional, máximo 400 caracteres.
* Capacidad mínima: 20000.
* Nombre único a nivel del sistema.

### Grupo

Debe tener:

* Etiqueta
* Equipos
* Partidos

Reglas:

* Etiqueta entre A y L.
* Exactamente 4 equipos para ser válido.
* Todos los equipos deben ser válidos.
* UEFA puede repetirse como máximo 2 veces en un grupo.
* Las demás confederaciones no pueden repetirse dentro de un grupo.
* Cada grupo debe terminar con 6 partidos de fase de grupos.

### Partido

Debe tener:

* Id
* Fecha
* Estadio
* EquipoLocal
* EquipoVisitante
* Grupo
* Fase
* GolesLocal
* GolesVisitante
* Vencedor, que puede implementarse como propiedad almacenada o como resultado calculado, siempre que el sistema pueda consultar claramente el ganador cuando corresponda.

  * En fase de grupos puede haber empate, por lo que `Vencedor` puede ser null o equivalente si el partido termina empatado.
  * En fases eliminatorias debe existir un vencedor.

Reglas:

* Id numérico incremental autogenerado.
* Fecha obligatoria y válida.
* Estadio obligatorio.
* Equipo local obligatorio.
* Equipo visitante obligatorio y distinto del local.
* Grupo obligatorio para fase de grupos, con etiqueta A-L.
* Debe poder cargarse resultado.
* Una vez generados los cruces de segunda fase, no se deben poder editar partidos de fase anterior.

### Fixture

Debe contener como mínimo:

* Lista de grupos.
* Eventualmente lista global de partidos para facilitar consultas por fecha, estadio, grupo y fase.

Recomendación de diseño:

* Si se agrega una lista global de partidos, preferir una propiedad calculada a partir de los partidos de los grupos para evitar inconsistencias.
* Ejemplo válido:

  * `Partidos => Grupos.SelectMany(g => g.Partidos).ToList();`

### ConfiguracionTorneo

Debe contener parámetros configurables para reproducibilidad:

* SemillaFixture
* SemillaCompletar
* SemillaCrucesFase
* SemillaSimulation
* FechaInicioTorneo, por defecto 2026-06-01.
* MaxPartidosPorDia, por defecto 3.
* SeparacionEntreFechas, por defecto 3.
* HoraInicioPartidos, por defecto 14:00.
* SeparacionHorasEntrePartidos, por defecto 4.

## Reglas exactas de Generación de Fixture

La generación de fixture debe cumplir:

### Validaciones previas

* Para generar el fixture deben existir exactamente 48 equipos cargados.
* Si hay menos de 48 equipos, no se debe generar directamente el fixture; primero debe ejecutarse la funcionalidad de completar automáticamente equipos faltantes, respetando cupos por confederación.
* Si hay más de 48 equipos, el fixture tampoco debe generarse porque el formato esperado es exactamente 48 equipos.
* Además, debe haber al menos 4 estadios.
* Los equipos recibidos deben ser válidos.
* Los estadios recibidos deben ser válidos.

### Estructura de grupos

* Se deben generar 12 grupos: A, B, C, D, E, F, G, H, I, J, K, L.
* Cada grupo debe tener 4 equipos.

### Ordenamiento y distribución

* Los equipos deben ordenarse por RankingFIFA descendente.
* Si hay empates de ranking, se debe usar Fisher-Yates determinístico con SemillaFixture.
* La distribución debe ser A→L en round-robin sobre la lista ordenada.

### Restricciones de confederaciones

* UEFA puede repetirse hasta 2 veces por grupo.
* Las demás confederaciones no pueden repetirse.

### Partidos por grupo

Cada grupo debe tener 6 partidos:

* Jornada 1: E1 vs E4, E2 vs E3.
* Jornada 2: E1 vs E3, E2 vs E4.
* Jornada 3: E1 vs E2, E3 vs E4.

Cada equipo debe jugar exactamente 3 partidos en su grupo.

No deben repetirse enfrentamientos dentro del mismo grupo.

### Asignación de fechas y horarios

* Fecha de inicio por defecto: 2026-06-01.
* Separación entre jornadas dentro de un grupo: 3 días.
* Hora inicial: 14:00.
* Separación entre partidos del mismo día: 4 horas.
* La última jornada de cada grupo debe jugarse el mismo día y a la misma hora.
* No se debe superar MaxPartidosPorDia, por defecto 3.

### Asignación de estadios

* Ordenar estadios por nombre normalizado ascendente.
* Asignación circular: Partido #n → Estadio[(n-1) mod CantEstadios].

### Ids de partidos

* Cada partido generado debe tener Id incremental autogenerado:

  * Primer partido: Id 1.
  * Último partido de fase de grupos: Id 72.

### Estado actual de Generación de Fixture

Actualmente `ServicioFixture` ya implementa:

* validación de cantidad exacta de 48 equipos;
* validación de mínimo 4 estadios;
* validación de equipos inválidos;
* validación de estadios inválidos;
* creación de grupos A-L;
* 4 equipos por grupo;
* ordenamiento por ranking FIFA descendente;
* Fisher-Yates determinístico con `SemillaFixture`;
* distribución A-L respetando confederaciones;
* generación de 6 partidos por grupo;
* patrón de jornadas;
* fechas y horarios;
* simultaneidad de última jornada;
* rotación de estadios por nombre normalizado;
* Ids incrementales de partidos del 1 al 72.

No reescribir masivamente `ServicioFixture` si no es necesario. Preferir TDD pequeños y refactors controlados.

## Normalización

La normalización debe seguir esta regla:

1. convertir a minúsculas;
2. quitar tildes;
3. reemplazar caracteres no alfanuméricos por espacio;
4. colapsar espacios múltiples;
5. aplicar trim.

Usar esta normalización para ordenar estadios por nombre normalizado.

## Segunda fase / cruces

Al finalizar la fase de grupos, se debe determinar el orden de equipos usando:

* Puntos.
* Diferencia de goles.
* Goles a favor.
* Desempate determinístico usando SemillaCrucesFase.

Luego:

* se toman los 12 primeros;
* los 12 segundos;
* los 8 mejores terceros;
* se generan 32 clasificados.

### Estructura de cruces

* Los 8 mejores primeros se cruzan con los 8 terceros: A1 a A8.
* Los 4 primeros restantes se cruzan con los 4 segundos de menor puntaje: B1 a B4.
* Los 8 segundos restantes se cruzan entre sí: B5 a B8.
* Para cruces, usar Fisher-Yates con SemillaCrucesFase.
* Evitar emparejar equipos del mismo grupo cuando sea posible.
* Luego generar las siguientes fases eliminatorias en orden:

  * Round of 32 / Primera ronda eliminatoria.
  * Octavos de final.
  * Cuartos de final.
  * Semifinales.
  * Partido por el tercer puesto.
  * Final.

## Auditoría

Debe registrarse log con timestamp ISO-8601 y usuario para:

* altas, ediciones y eliminaciones de usuarios;
* altas, ediciones y eliminaciones de estadios;
* altas, ediciones y eliminaciones de equipos;
* generación automática de equipos;
* generación de fixture;
* modificación de partidos;
* realización de sorteo para cruces;
* importación de equipos, éxitos y errores.

Cada uso de una semilla debe quedar registrado para trazabilidad.

## Generación automática de equipos faltantes

Si hay menos de 48 equipos cargados, el sistema debe permitir completar automáticamente hasta llegar a 48.

Reglas:

* Se deben respetar los cupos globales por confederación:

  * UEFA: 16
  * CONMEBOL: 7
  * CONCACAF: 7
  * CAF: 9
  * AFC: 8
  * OFC: 1
* Los equipos ficticios deben tener nombres determinísticos, por ejemplo:

  * UEFA_01
  * CONMEBOL_02
  * etc.
* El RankingFIFA de los equipos generados debe ser reproducible usando `SemillaCompletar`.
* Se debe auditar:

  * cantidad completada por confederación;
  * semilla usada;
  * rangos asignados.

## Importación de equipos

El sistema debe permitir importar un CSV de equipos.

Reglas:

* El archivo debe tener exactamente los encabezados:

  * Nombre
  * Confederación
  * RankingFIFA
* Se debe validar:

  * nombre único;
  * confederación válida;
  * RankingFIFA dentro del rango permitido;
  * cupos por confederación.
* La importación debe registrar casos de éxito y errores.
* La importación debe quedar auditada.

**Nota de diseño importante**: Para este requerimiento se deben generar con IA tres alternativas de implementación, cada una priorizando un criterio distinto:

1. Extensibilidad: diseño preparado para agregar nuevos formatos y validadores fácilmente.
2. Bajo acoplamiento y alta cohesión: separación clara de responsabilidades entre lectura, validación e inserción.
3. Facilidad de comprensión: código legible y documentado para nuevos integrantes del equipo.

Luego el equipo debe seleccionar una implementación, justificando claramente la decisión basado en el análisis crítico de las alternativas propuestas.

## Simulación de resultados

La simulación de resultados debe:

* aplicarse a partidos de fase de grupos y fases restantes;
* usar `SemillaSimulation` para que sea reproducible;
* basarse en el ranking actual de los equipos.

**Nota de diseño**: Si el dominio solo tiene `RankingFIFA`, el equipo debe decidir si:

* se reutiliza `RankingFIFA` como ranking actual;
* o se agrega una propiedad/mecanismo específico para `RankingActual`.

Esa decisión debe ser consistente, estar documentada y justificada en el código.

## Pantallas mínimas esperadas

La aplicación debe incluir, como mínimo:

1. **Login**: acceso seguro al sistema.
2. **Usuarios ABM**: alta, baja, modificación, solo para usuarios con rol Administrador.
3. **Equipos**:

   * listado, alta, edición y eliminación.
   * incluye generador automático para completar los 48 requeridos.
4. **Estadios**: listado, alta, edición y eliminación, solo para Administrador.
5. **Fixture y cruces**:

   * generar fixture, con botón, validaciones y confirmación;
   * mostrar los 12 grupos y sus equipos;
   * al acceder a un grupo, mostrar participantes, estadio y fecha de cada partido;
   * permitir generar cruces de segunda fase una vez completada la primera fase;
   * mostrar el avance del torneo, fase actual, equipos vivos y ganadores.
6. **Listado de partidos**:

   * organizado por fecha;
   * filtrable por fecha, estadio, grupo y fase.
7. **Edición de partido**:

   * modificar fecha;
   * modificar estadio;
   * cargar resultado, goles local y goles visitante.
8. **Visualización de log**: acceso a la auditoría del sistema, solo para Administrador.
9. **Importar equipos**: carga de CSV con validación y reporte de errores.

## Estado actual de UI

El proyecto activo de interfaz es `WorldCupPlanner.UI`, pero actualmente está prácticamente como plantilla Blazor Server.

Contiene:

* `WeatherForecast`
* `WeatherForecastService`
* `Error`
* `Program.cs` con registro de Razor Pages, Server Side Blazor y `WeatherForecastService`.

Todavía no está integrada la lógica real de `WorldCupPlanner.Dominio`, `WorldCupPlanner.Servicios` ni `WorldCupPlanner.AccesoADatos`.

Cuando se trabaje en Blazor:

* no poner lógica de negocio en componentes;
* registrar servicios y repositorios en `Program.cs`;
* usar servicios para acceder a equipos, estadios, fixture, partidos y usuarios;
* mantener la UI como capa de presentación;
* no usar Entity Framework ni base de datos en esta entrega.

## Documentación obligatoria

Además del código y tests, el equipo debe preparar documentación formal.

Debe incluir:

* descripción general del sistema;
* funcionalidades no implementadas o errores conocidos, si los hay;
* **diagramas de paquetes**: mostrando `WorldCupPlanner.Dominio`, `WorldCupPlanner.Servicios`, `WorldCupPlanner.AccesoADatos`, `WorldCupPlanner.DominioTest`, `WorldCupPlanner.ServicioTests`, `WorldCupPlanner.UI` y sus dependencias;
* **diagramas de clases del dominio**: Equipo, Confederación, Estadio, Grupo, Partido, Fixture, ConfiguracionTorneo;
* **diagramas de clases de servicios**: ServicioEquipo, ServicioEstadio, ServicioFixture, ServicioAuditoria, etc.;
* **diagramas o descripción de repositorios en memoria**: RepositorioEquipoEnMemoria, RepositorioEstadioEnMemoria, RepositorioGrupoEnMemoria, RepositorioPartidoEnMemoria;
* **tabla de responsabilidades por clase**: qué responsabilidad tiene cada clase, integrante responsable;
* **explicación de mecanismos generales**:

  * interacción UI/dominio, cómo Blazor invoca servicios;
  * almacenamiento en memoria, estructura de colecciones, repositorios e inicialización;
  * manejo de errores y excepciones, qué excepciones lanza cada servicio;
  * uso de polimorfismo, herencia o interfaces, si corresponde.
* **análisis detallado de dependencias para Generación de Fixture**:

  * clases involucradas, qué genera qué y en qué orden;
  * dependencias explícitas e implícitas;
  * evaluación de cohesión, si cada clase hace una sola cosa bien;
  * evaluación de acoplamiento, si hay acoplamiento innecesario.
* **explicación del algoritmo de generación de fixture**: pasos en pseudocódigo, justificación de decisiones, complejidad.
* **reporte de cobertura con JetBrains dotCover**: a nivel de clase, mostrando líneas cubiertas y no cubiertas;

  * si alguna funcionalidad no alcanza 90% de cobertura de líneas, justificar por qué.

## Uso de IA Generativa

El equipo puede usar IA como apoyo, por ejemplo GitHub Copilot, ChatGPT, Claude, etc., pero todo el código generado debe ser **entendido, revisado y verificado** por los integrantes antes de comitear.

Se debe documentar:

* **herramienta utilizada**: nombre y versión.
* **contexto de uso**: qué parte del código se asistió con IA, por ejemplo "implementación de algoritmo Fisher-Yates" o "estructura de tests".
* **qué partes fueron asistidas por IA**: ser específico y honesto.
* **análisis crítico de la calidad del código generado**:

  * ¿el código respeta Clean Code?
  * ¿cumple con cohesión y bajo acoplamiento?
  * ¿la lógica es correcta?
  * ¿hay mejoras posibles?
* **skills o archivos de instrucciones utilizados**: si se usaron custom instructions, como este `agent.md`, documentarlos.
* **ejemplos concretos de uso**: mostrar fragmentos de código y el prompt que se usó.

**El desconocimiento del código generado por IA puede penalizar al equipo.**

## Tests y estilo

Usar MSTest.

Estilo de tests:

```csharp
[TestMethod]
public void NombreDelMetodo_Escenario_ResultadoEsperado()
{
    // Arrange

    // Act

    // Assert
}
```

Los tests deben ser:

* simples;
* legibles;
* independientes;
* repetibles;
* autocontenidos;
* con datos determinísticos.

Evitar tests gigantes.

Si una regla es compleja, dividirla en varios TDD pequeños.

## Clean Code

Aplicar Clean Code:

* nombres claros;
* métodos cortos;
* una responsabilidad por método;
* constantes para números mágicos;
* no dejar código muerto;
* no dejar código comentado;
* no agregar comentarios innecesarios;
* no cambiar comportamiento durante refactor;
* usar excepciones cuando corresponda;
* evitar acoplamiento innecesario;
* mantener alta cohesión.

Si Rider marca un método privado como "is never used" y efectivamente no se usa, eliminarlo en etapa REFACTOR.

## Instrucciones al modificar código

Antes de cambiar código:

1. Identificar qué regla de la letra se quiere cubrir.
2. Escribir o modificar primero el test.
3. Confirmar que el test falla si corresponde.
4. Implementar el mínimo código para pasarlo.
5. Correr todos los tests.
6. Refactorizar solo si el comportamiento ya está verde.
7. No romper tests anteriores.
8. Si un test anterior contradice una regla nueva de la letra, explicar el conflicto y ajustar el test.

## Restricciones importantes

* No usar base de datos.
* No usar Entity Framework todavía.
* No agregar dependencias externas salvo que estén justificadas.
* No meter lógica de negocio en Blazor.
* No implementar funcionalidades fuera del alcance del TDD actual.
* No reescribir masivamente clases que ya funcionan si no es necesario.
* No usar herencia o interfaces "porque sí"; solo si simplifica diseño o reduce acoplamiento.
* No volver a listas internas en servicios si ya existe un repositorio en memoria correspondiente.
* No mezclar responsabilidades entre dominio, servicios, data access y UI.

---

## Organización del trabajo por integrante

El trabajo está dividido entre tres integrantes. Copilot debe respetar esta división para no mezclar responsabilidades ni modificar clases que no correspondan sin necesidad.

---

## Limpiezas técnicas pendientes conocidas

Actualmente existen algunas limpiezas menores pendientes:

* `WorldCupPlanner.UI` todavía contiene código de plantilla Blazor (`WeatherForecast`, `WeatherForecastService`, `Counter`, `FetchData`, `SurveyPrompt`) y no está integrado con los servicios reales.
* `Auditoria` existe en `WorldCupPlanner.Dominio.Entidades`, pero todavía está vacía. Mantenerla solo si se va a implementar auditoría mediante TDD próximamente.
* Existen carpetas viejas de la estructura anterior que no forman parte de `WorldCupPlanner.sln`; evitar modificarlas o usarlas como destino de archivos nuevos.
* Mantener los tests de normalización en `WorldCupPlanner.ServicioTests/NormalizarTest.cs` y no volver a mezclarlos dentro de `ServicioEquipoTests`.

Estas limpiezas deben hacerse como refactors pequeños y controlados, sin cambiar comportamiento.

Si existe conflicto entre este archivo y la letra oficial del obligatorio, prevalece siempre la letra oficial del PDF.

Este archivo es una guía operativa para ayudar al desarrollo, pero no sustituye el documento oficial del obligatorio.
