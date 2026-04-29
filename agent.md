# Agent Instructions — WorldCupPlanner 2026

## Contexto general del proyecto

Este repositorio corresponde al Obligatorio 1 de Diseño de Aplicaciones 1.  
El sistema se llama **WorldCupPlanner 2026**.

La aplicación debe gestionar la fase de grupos del Mundial 2026, con 48 equipos, 12 grupos de 4 equipos, generación automática de fixture, preparación de cruces eliminatorios, simulación de resultados, auditoría y trazabilidad mediante semillas determinísticas.

El trabajo debe desarrollarse en **C# con .NET 8 y Blazor Server App**. La lógica de negocio debe estar separada de la interfaz. No usar base de datos en esta primera entrega: toda la información se guarda en memoria mediante listas, objetos y servicios.

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
```

No agregar funcionalidad nueva durante un refactor.
No cambiar tests durante GREEN salvo que un test anterior contradiga explícitamente la letra del obligatorio. Si eso ocurre, explicar por qué se ajusta el test.

## Git, Gitflow y GitHub Actions

El código fuente debe gestionarse en GitHub utilizando metodología gitflow.

Se debe configurar un pipeline de GitHub Actions que ejecute los tests cada vez que se realice un pull request hacia la rama principal (`main`).

Aunque no sea obligatorio que los tests aprueben para concretar el merge, el pipeline debe ejecutarse siempre y reportar los resultados de manera visible.

Los commits de Generación de Fixture deben indicar claramente si corresponden a RED, GREEN o REFACTOR.

## Arquitectura actual esperada

El proyecto está organizado al menos en estos namespaces/proyectos:

- WorldCupPlanner.Domain
- WorldCupPlanner.Services
- WorldCupPlanner.Tests

La lógica de negocio debe ir en Domain y Services.
Los tests unitarios deben ir en Tests.
No poner lógica de negocio en la UI.

## Clases relevantes del dominio

### Equipo

Debe tener:

- Nombre
- RankingFIFA
- Confederacion

Reglas:

- Nombre obligatorio, 1 a 60 caracteres.
- Confederación obligatoria.
- Ranking FIFA obligatorio entre 300 y 2500.
- Nombre único a nivel del sistema.
- Cupos globales por confederación:
  - UEFA: 16
  - CONMEBOL: 7
  - CONCACAF: 7
  - CAF: 9
  - AFC: 8
  - OFC: 1

### Confederacion

Enum con:

- UEFA
- CONMEBOL
- CONCACAF
- CAF
- AFC
- OFC

### Estadio

Debe tener:

- Nombre
- Ciudad
- Descripcion
- Capacidad

Reglas:

- Nombre obligatorio, 1 a 80 caracteres.
- Ciudad obligatoria, 1 a 60 caracteres.
- Descripción opcional, máximo 400 caracteres.
- Capacidad mínima: 20000.
- Nombre único a nivel del sistema.

### Grupo

Debe tener:

- Etiqueta
- Equipos
- Partidos

Reglas:

- Etiqueta entre A y L.
- Exactamente 4 equipos para ser válido.
- Todos los equipos deben ser válidos.
- UEFA puede repetirse como máximo 2 veces en un grupo.
- Las demás confederaciones no pueden repetirse dentro de un grupo.
- Cada grupo debe terminar con 6 partidos de fase de grupos.

### Partido

Debe tener:

- Id
- Fecha
- Estadio
- EquipoLocal
- EquipoVisitante
- Grupo
- Fase
- GolesLocal
- GolesVisitante
- Vencedor, que puede implementarse como propiedad almacenada o como resultado calculado, siempre que el sistema pueda consultar claramente el ganador cuando corresponda.
  - En fase de grupos puede haber empate, por lo que `Vencedor` puede ser null o equivalente si el partido termina empatado.
  - En fases eliminatorias debe existir un vencedor.

Reglas:

- Id numérico incremental autogenerado.
- Fecha obligatoria y válida.
- Estadio obligatorio.
- Equipo local obligatorio.
- Equipo visitante obligatorio y distinto del local.
- Grupo obligatorio para fase de grupos, con etiqueta A-L.
- Debe poder cargarse resultado.
- Una vez generados los cruces de segunda fase, no se deben poder editar partidos de fase anterior.

### Fixture

Debe contener como mínimo:

- Lista de grupos.
- Eventualmente lista global de partidos para facilitar consultas por fecha, estadio, grupo y fase.

### ConfiguracionTorneo

Debe contener parámetros configurables para reproducibilidad:

- SemillaFixture
- SemillaCompletar
- SemillaCrucesFase
- SemillaSimulation
- FechaInicioTorneo, por defecto 2026-06-01.
- MaxPartidosPorDia, por defecto 3.
- SeparacionEntreFechas, por defecto 3.
- HoraInicioPartidos, por defecto 14:00.
- SeparacionHorasEntrePartidos, por defecto 4.


## Reglas exactas de Generación de Fixture

La generación de fixture debe cumplir:

### Validaciones previas
- Para generar el fixture deben existir exactamente 48 equipos cargados.
- Si hay menos de 48 equipos, no se debe generar directamente el fixture; primero debe ejecutarse la funcionalidad de completar automáticamente equipos faltantes, respetando cupos por confederación.
- Si hay más de 48 equipos, el fixture tampoco debe generarse porque el formato esperado es exactamente 48 equipos.
- Además, debe haber al menos 4 estadios.

### Estructura de grupos
- Se deben generar 12 grupos: A, B, C, D, E, F, G, H, I, J, K, L.
- Cada grupo debe tener 4 equipos.

### Ordenamiento y distribución
- Los equipos deben ordenarse por RankingFIFA descendente.
- Si hay empates de ranking, se debe usar Fisher-Yates determinístico con SemillaFixture.
- La distribución debe ser A→L en round-robin sobre la lista ordenada.

### Restricciones de confederaciones
- UEFA puede repetirse hasta 2 veces por grupo.
- Las demás confederaciones no pueden repetirse.

### Partidos por grupo
Cada grupo debe tener 6 partidos:
- Jornada 1: E1 vs E4, E2 vs E3.
- Jornada 2: E1 vs E3, E2 vs E4.
- Jornada 3: E1 vs E2, E3 vs E4.

### Asignación de fechas y horarios
- Fecha de inicio por defecto: 2026-06-01.
- Separación entre jornadas dentro de un grupo: 3 días.
- Hora inicial: 14:00.
- Separación entre partidos del mismo día: 4 horas.
- La última jornada de cada grupo debe jugarse el mismo día y a la misma hora.
- No se debe superar MaxPartidosPorDia, por defecto 3.

### Asignación de estadios
- Ordenar estadios por nombre normalizado ascendente.
- Asignación circular: Partido #n → Estadio[(n-1) mod CantEstadios].

### Ids de partidos
- Cada partido generado debe tener Id incremental autogenerado:
  - Primer partido: Id 1.
  - Último partido de fase de grupos: Id 72.

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

- Puntos.
- Diferencia de goles.
- Goles a favor.
- Desempate determinístico usando SemillaCrucesFase.

Luego:

- se toman los 12 primeros;
- los 12 segundos;
- los 8 mejores terceros;
- se generan 32 clasificados.

### Estructura de cruces

- Los 8 mejores primeros se cruzan con los 8 terceros: A1 a A8.
- Los 4 primeros restantes se cruzan con los 4 segundos de menor puntaje: B1 a B4.
- Los 8 segundos restantes se cruzan entre sí: B5 a B8.
- Para cruces, usar Fisher-Yates con SemillaCrucesFase.
- Evitar emparejar equipos del mismo grupo cuando sea posible.
- Luego generar las siguientes fases eliminatorias en orden:
  - Round of 32 / Primera ronda eliminatoria.
  - Octavos de final.
  - Cuartos de final.
  - Semifinales.
  - Partido por el tercer puesto.
  - Final.

## Auditoría

Debe registrarse log con timestamp ISO-8601 y usuario para:

- altas, ediciones y eliminaciones de usuarios;
- altas, ediciones y eliminaciones de estadios;
- altas, ediciones y eliminaciones de equipos;
- generación automática de equipos;
- generación de fixture;
- modificación de partidos;
- realización de sorteo para cruces;
- importación de equipos, éxitos y errores.

Cada uso de una semilla debe quedar registrado para trazabilidad.

## Generación automática de equipos faltantes

Si hay menos de 48 equipos cargados, el sistema debe permitir completar automáticamente hasta llegar a 48.

Reglas:

- Se deben respetar los cupos globales por confederación:
  - UEFA: 16
  - CONMEBOL: 7
  - CONCACAF: 7
  - CAF: 9
  - AFC: 8
  - OFC: 1
- Los equipos ficticios deben tener nombres determinísticos, por ejemplo:
  - UEFA_01
  - CONMEBOL_02
  - etc.
- El RankingFIFA de los equipos generados debe ser reproducible usando `SemillaCompletar`.
- Se debe auditar:
  - cantidad completada por confederación;
  - semilla usada;
  - rangos asignados.

## Importación de equipos

El sistema debe permitir importar un CSV de equipos.

Reglas:

- El archivo debe tener exactamente los encabezados:
  - Nombre
  - Confederación
  - RankingFIFA
- Se debe validar:
  - nombre único;
  - confederación válida;
  - RankingFIFA dentro del rango permitido;
  - cupos por confederación.
- La importación debe registrar casos de éxito y errores.
- La importación debe quedar auditada.

**Nota de diseño importante**: Para este requerimiento se deben generar con IA tres alternativas de implementación, cada una priorizando un criterio distinto:

1. Extensibilidad: diseño preparado para agregar nuevos formatos y validadores fácilmente.
2. Bajo acoplamiento y alta cohesión: separación clara de responsabilidades entre lectura, validación e inserción.
3. Facilidad de comprensión: código legible y documentado para nuevos integrantes del equipo.

Luego el equipo debe seleccionar una implementación, justificando claramente la decisión basado en el análisis crítico de las alternativas propuestas.

## Simulación de resultados

La simulación de resultados debe:

- aplicarse a partidos de fase de grupos y fases restantes;
- usar `SemillaSimulation` para que sea reproducible;
- basarse en el ranking actual de los equipos.

**Nota de diseño**: Si el dominio solo tiene `RankingFIFA`, el equipo debe decidir si:

- se reutiliza `RankingFIFA` como ranking actual;
- o se agrega una propiedad/mecanismo específico para `RankingActual`.

Esa decisión debe ser consistente, estar documentada y justificada en el código.

## Pantallas mínimas esperadas

La aplicación debe incluir, como mínimo:

1. **Login**: acceso seguro al sistema.
2. **Usuarios ABM**: alta, baja, modificación, solo para usuarios con rol Administrador.
3. **Equipos**: 
   - listado, alta, edición y eliminación.
   - incluye generador automático para completar los 48 requeridos.
4. **Estadios**: listado, alta, edición y eliminación (solo para Administrador).
5. **Fixture y cruces**:
   - generar fixture (botón, validaciones y confirmación);
   - mostrar los 12 grupos y sus equipos;
   - al acceder a un grupo, mostrar participantes, estadio y fecha de cada partido;
   - permitir generar cruces de segunda fase una vez completada la primera fase;
   - mostrar el avance del torneo (fase actual, equipos vivos, ganadores).
6. **Listado de partidos**:
   - organizado por fecha;
   - filtrable por fecha, estadio, grupo y fase.
7. **Edición de partido**:
   - modificar fecha;
   - modificar estadio;
   - cargar resultado (goles local, goles visitante).
8. **Visualización de log**: acceso a la auditoría del sistema (solo para Administrador).
9. **Importar equipos**: carga de CSV con validación y reporte de errores.

## Documentación obligatoria

Además del código y tests, el equipo debe preparar documentación formal:

Debe incluir:

- descripción general del sistema;
- funcionalidades no implementadas o errores conocidos, si los hay;
- **diagramas de paquetes**: mostrando WorldCupPlanner.Domain, Services, Tests, WebApp y sus dependencias;
- **diagramas de clases del dominio**: Equipo, Confederación, Estadio, Grupo, Partido, Fixture, ConfiguracionTorneo;
- **diagramas de clases de servicios**: ServicioEquipo, ServicioEstadio, ServicioFixture, ServicioAuditoria, etc.;
- **tabla de responsabilidades por clase**: qué responsabilidad tiene cada clase, integrante responsable;
- **explicación de mecanismos generales**:
  - interacción UI/dominio (cómo Blazor invoca servicios);
  - almacenamiento en memoria (estructura de colecciones, inicialización);
  - manejo de errores y excepciones (qué excepciones lanza cada servicio);
  - uso de polimorfismo, herencia o interfaces, si corresponde.
- **análisis detallado de dependencias para Generación de Fixture**:
  - clases involucradas (qué genera qué y en qué orden);
  - dependencias explícitas e implícitas;
  - evaluación de cohesión (¿cada clase hace una sola cosa bien?);
  - evaluación de acoplamiento (¿hay acoplamiento innecesario?).
- **explicación del algoritmo de generación de fixture**: pasos en pseudocódigo, justificación de decisiones, complejidad.
- **reporte de cobertura con JetBrains dotCover**: a nivel de clase, mostrando líneas cubiertas y no cubiertas;
  - si alguna funcionalidad no alcanza 90% de cobertura de líneas, justificar por qué (ej: manejo de excepciones raras, código defensivo, etc.).

## Uso de IA Generativa

El equipo puede usar IA como apoyo (GitHub Copilot, ChatGPT, Claude, etc.), pero todo el código generado debe ser **entendido, revisado y verificado** por los integrantes antes de comitear.

Se debe documentar:

- **herramienta utilizada**: nombre y versión.
- **contexto de uso**: qué parte del código se asistió con IA (ej: "implementación de algoritmo Fisher-Yates", "estructura de tests").
- **qué partes fueron asistidas por IA**: ser específico y honesto.
- **análisis crítico de la calidad del código generado**:
  - ¿el código respeta Clean Code?
  - ¿cumple con cohesión y bajo acoplamiento?
  - ¿la lógica es correcta?
  - ¿hay mejoras posibles?
- **skills o archivos de instrucciones utilizados**: si se usaron custom instructions (como este agent.md), documentarlos.
- **ejemplos concretos de uso**: mostrar fragmentos de código y el prompt que se usó.

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

- simples;
- legibles;
- independientes;
- repetibles;
- autocontenidos;
- con datos determinísticos.

Evitar tests gigantes.
Si una regla es compleja, dividirla en varios TDD pequeños.

## Clean Code

Aplicar Clean Code:

- nombres claros;
- métodos cortos;
- una responsabilidad por método;
- constantes para números mágicos;
- no dejar código muerto;
- no dejar código comentado;
- no agregar comentarios innecesarios;
- no cambiar comportamiento durante refactor;
- usar excepciones cuando corresponda;
- evitar acoplamiento innecesario;
- mantener alta cohesión.

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

- No usar base de datos.
- No usar Entity Framework todavía.
- No agregar dependencias externas salvo que estén justificadas.
- No meter lógica de negocio en Blazor.
- No implementar funcionalidades fuera del alcance del TDD actual.
- No reescribir masivamente clases que ya funcionan si no es necesario.
- No usar herencia o interfaces "porque sí"; solo si simplifica diseño o reduce acoplamiento.

---

## Organización del trabajo por integrante

El trabajo está dividido entre tres integrantes. Copilot debe respetar esta división para no mezclar responsabilidades ni modificar clases que no correspondan sin necesidad.

### Seba

Seba trabaja principalmente en:

- `Grupo`
- `Fixture`
- `ConfiguracionTorneo`
- `ServicioFixture`
- `GrupoTests`
- `ConfiguracionTorneoTests`
- `ServicioFixtureTests`

Responsabilidades principales de Seba:

- validación de grupos;
- generación de grupos A-L;
- distribución de equipos en grupos;
- reglas de confederación dentro de grupos;
- generación del fixture de fase de grupos;
- generación de partidos de fase de grupos;
- fechas, horarios y calendario de partidos;
- asignación de estadios por rotación;
- uso de `SemillaFixture`;
- lógica relacionada con parámetros de configuración del torneo.

Seba puede necesitar usar clases como `Equipo`, `Estadio` y `Partido`, pero no necesariamente es el responsable principal de implementarlas, salvo que sea necesario para integrar su parte.

### Nacho

Nacho trabaja principalmente en:

- `Equipo`
- `Estadio`
- `Partido`
- `Normalizar` o `NormalizadorTexto`
- `EquipoTests`
- `EstadioTests`
- `PartidoTests`
- `NormalizarTests`
- `ServicioEquipo`
- `ServicioEstadio`
- `ServicioEquipoTests`
- `ServicioEstadioTests`

Responsabilidades principales de Nacho:

- validaciones de equipos;
- cupos y reglas propias de equipos;
- validaciones de estadios;
- alta, edición y eliminación de equipos;
- alta, edición y eliminación de estadios;
- normalización de texto;
- datos y validaciones de partidos;
- carga de resultados de partidos;
- cálculo de ganador o empate;
- restricciones propias de partido.

Las clases de Nacho pueden ser usadas por `ServicioFixture`, pero Copilot no debe modificar su lógica salvo que sea estrictamente necesario para integración o para cumplir una regla de la letra.

### Juan Manuel / Ferre

Juan Manuel, también identificado como Ferre, trabaja principalmente en:

- `Usuario`
- `Rol`
- `LoginAuditoria` o `AuditoriaLog`
- `TipoAccion`
- `ServicioUsuario`
- `ServicioAuditoria`
- pipeline de GitHub Actions
- tests relacionados con usuarios, roles, auditoría y CI/CD

Responsabilidades principales de Juan Manuel / Ferre:

- gestión de usuarios;
- validación de datos de usuario;
- contraseñas seguras y cifradas;
- roles no excluyentes;
- auditoría de acciones del sistema;
- registro de generación de fixture;
- registro de uso de semillas;
- registro de modificaciones de partidos;
- configuración del pipeline de GitHub Actions;
- automatización de tests en CI.

Copilot debe tener en cuenta que la generación de fixture deberá integrarse más adelante con auditoría, pero no debe implementar auditoría dentro de `ServicioFixture` si todavía no se está trabajando ese TDD.

---

## Nota personal para el Agent

Este archivo es tu brújula. Consúltalo en cada cambio de código. Cualquier duda sobre estructura, nombres, comportamiento esperado o restricciones, refúgiate aquí. Seba (o el usuario) contará con que conozcas estas reglas y las apliques consistentemente.

**IMPORTANTE**: Respeta siempre la división de trabajo entre Seba, Nacho y Ferre. No modifiques código de otra persona sin una razón explícita que lo requiera. Si necesitas ajustar algo que no es de tu responsabilidad, consulta primero o hace el cambio mínimo necesario con justificación clara.

## Regla final

Si existe conflicto entre este archivo y la letra oficial del obligatorio, prevalece siempre la letra oficial del PDF.

Este archivo es una guía operativa para ayudar al desarrollo, pero no sustituye el documento oficial del obligatorio.