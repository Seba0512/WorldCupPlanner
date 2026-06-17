# Instrucciones Para El Agente De IA

Este archivo define las instrucciones generales que debe seguir cualquier agente de IA que
colabore sobre el repositorio de **WorldCupPlanner 2026**. Su objetivo es mantener coherencia
con la arquitectura, las decisiones de diseño y el estilo de trabajo del equipo.

## Contexto Del Proyecto

WorldCupPlanner 2026 es una aplicacion C# con .NET 8 y Blazor Server para gestionar la
planificacion y seguimiento del Mundial 2026.

El sistema permite administrar equipos, estadios, usuarios, roles, configuracion del torneo,
fixture, fase de grupos, etapas eliminatorias, partidos, resultados, clasificacion, simulacion,
auditoria, notificaciones, incidencias, importacion y exportacion de informacion.

La persistencia se implementa con Entity Framework Core Code First sobre SQL Server/Azure SQL
Edge en Docker.

## Arquitectura Que Debe Respetarse

La solucion esta organizada por capas. El agente debe respetar esta separacion:

```text
WorldCupPlanner.UI
    -> WorldCupPlanner.Controlador
        -> WorldCupPlanner.Servicios
            -> WorldCupPlanner.Dominio
            -> WorldCupPlanner.AccesoADatos.Interfaces
                -> WorldCupPlanner.AccesoADatos
```

Responsabilidades principales:

- `WorldCupPlanner.Dominio`: entidades, enumeraciones, excepciones y reglas propias del dominio.
- `WorldCupPlanner.Dominio.DTO`: objetos de transferencia de datos.
- `WorldCupPlanner.AccesoADatos.Interfaces`: contratos de repositorio.
- `WorldCupPlanner.AccesoADatos`: `SqlContext`, migraciones y repositorios SQL.
- `WorldCupPlanner.Servicios`: casos de uso y reglas de aplicacion.
- `WorldCupPlanner.Controlador`: coordinacion entre UI, DTOs, comandos y servicios.
- `WorldCupPlanner.UI`: interfaz Blazor Server.

## Reglas Generales De Trabajo

- Revisar el estado actual del codigo antes de proponer o realizar cambios.
- Usar nombres reales de proyectos, namespaces, clases y archivos.
- Mantener el estilo existente del proyecto.
- Hacer cambios pequenos, incrementales y verificables.
- No realizar refactors masivos sin explicar el motivo, alcance, riesgo y plan.
- No modificar archivos no relacionados con la tarea solicitada.
- No eliminar ni revertir trabajo existente sin autorizacion explicita.
- No inventar funcionalidades, dependencias, comandos ni estructuras.
- Documentar supuestos cuando una decision no sea evidente.
- Avisar si una tarea requiere confirmar una decision del equipo.

## Dominio

- El dominio no debe depender de Blazor, Entity Framework Core, `SqlContext`, repositorios ni
  controladores.
- Las reglas propias de una entidad deben permanecer en la entidad cuando esta posee la
  informacion necesaria para resolverlas.
- Evitar trasladar validaciones del dominio a la UI o a los repositorios.
- Mantener entidades y objetos de dominio libres de atributos de infraestructura siempre que sea
  posible.
- Usar excepciones del dominio cuando representen errores de negocio.

## Servicios

- Los servicios concentran casos de uso y reglas de aplicacion.
- Los servicios pueden depender de entidades del dominio y de interfaces de repositorio.
- Los servicios no deben depender directamente de `SqlContext`, `DbContext`, `DbSet`,
  migraciones ni clases concretas de Entity Framework Core.
- Evitar que un servicio tenga demasiadas responsabilidades. Si coordina un flujo complejo,
  preferir un servicio orquestador.
- Mantener separados los servicios de reglas especificas y los servicios de coordinacion.

## Controladores

- Los controladores deben funcionar como capa intermedia entre la UI y los servicios.
- La UI debe delegar operaciones en controladores o servicios, sin duplicar reglas de negocio.
- Los controladores pueden usar DTOs, objetos comando y `MapeadorDtoDominio` cuando sea necesario.
- No colocar reglas de negocio complejas en los controladores.
- No acceder directamente a `SqlContext` desde controladores.

## Interfaz Blazor

- Las paginas y componentes Blazor deben encargarse de presentar informacion y capturar acciones
  del usuario.
- No colocar logica de negocio en `.razor`.
- No acceder directamente a repositorios ni a `SqlContext` desde la UI.
- Mantener validaciones de presentacion separadas de reglas de negocio.
- Reutilizar helpers y servicios de UI existentes cuando corresponda.

## Acceso A Datos

- La persistencia debe realizarse mediante repositorios SQL e interfaces de
  `WorldCupPlanner.AccesoADatos.Interfaces`.
- Los repositorios SQL deben implementar los contratos existentes cuando sea posible.
- `SqlContext` debe permanecer en `WorldCupPlanner.AccesoADatos`.
- Configurar relaciones de Entity Framework Core preferentemente con Fluent API.
- No mover reglas de negocio a repositorios.
- Los repositorios deben resolver persistencia, consultas y actualizaciones de datos.
- Revisar migraciones y `SqlContextModelSnapshot` luego de cambios en el modelo.
- Evitar cambios manuales en migraciones salvo que el motivo este claro y justificado.

## Base De Datos Y Migraciones

- La base productiva se levanta con Docker usando Azure SQL Edge.
- La cadena de conexion principal esta en `WorldCupPlanner.UI/appsettings.json`.
- Las migraciones estan en `WorldCupPlanner.AccesoADatos/Migraciones`.
- La aplicacion ejecuta migraciones al iniciar mediante `Database.Migrate()` en
  `WorldCupPlanner.UI/Program.cs`.
- Si se agregan entidades persistibles, revisar `SqlContext`, repositorios, interfaces,
  migraciones y tests.
- No dejar datos requeridos por el sistema solamente en memoria si deben persistirse.

## Inyeccion De Dependencias

- `SqlContext` y repositorios SQL deben registrarse como `Scoped`.
- Los servicios que dependan directa o indirectamente de repositorios SQL tambien deben ser
  compatibles con ese lifetime.
- Evitar dependencias `Singleton -> Scoped`.
- Revisar `WorldCupPlanner.UI/Program.cs` cuando se agreguen servicios o repositorios.

## Tests

- El proyecto usa MSTest.
- Cuando un cambio afecte comportamiento, agregar o actualizar tests.
- Preferir tests pequenos, independientes y repetibles.
- No depender de Docker ni de SQL Server real para tests unitarios.
- Para acceso a datos se puede usar EF Core InMemory cuando corresponda.
- Mantener el patron Arrange / Act / Assert.
- No afirmar que los tests pasan si no fueron ejecutados.

## Clean Code Y Mantenibilidad

- Usar nombres claros y consistentes con el proyecto.
- Evitar numeros magicos; extraer constantes con nombres descriptivos.
- Evitar metodos demasiado largos.
- Evitar exceso de parametros; usar objetos comando o clases de contexto cuando mejore claridad.
- Evitar duplicacion de logica.
- Evitar anidacion excesiva; preferir guard clauses cuando corresponda.
- No dejar codigo comentado sin justificacion.
- No silenciar excepciones.
- Mantener alta cohesion y bajo acoplamiento.

## Uso De Skills

El repositorio incluye Skills en la carpeta `skills`. El agente debe usar la skill adecuada si la
tarea coincide con su objetivo:

- `skills/ef-repository-migration/SKILL.md`: migracion o revision de persistencia con EF Core.
- `skills/clean-code-auditor-csharp/SKILL.md`: auditoria de Clean Code en codigo C#.
- `skills/readme-writer/SKILL.md`: creacion o actualizacion del `README.md`.

Las Skills no reemplazan estas instrucciones generales. Deben complementarlas.

## Antes De Modificar Codigo

Antes de editar archivos, el agente debe:

1. Identificar la capa afectada.
2. Revisar clases, interfaces y tests relacionados.
3. Confirmar si existen patrones previos en el proyecto.
4. Evaluar si el cambio puede romper persistencia, DI, tests o reglas de dominio.
5. Proponer un cambio pequeno si el impacto es alto.

## Despues De Modificar Codigo

Luego de modificar archivos, el agente debe:

1. Resumir los cambios realizados.
2. Indicar los archivos modificados.
3. Ejecutar tests si corresponde y si el entorno lo permite.
4. Aclarar si no pudo ejecutar tests.
5. Informar riesgos, pendientes o supuestos relevantes.

## Restricciones Importantes

- No reescribir la arquitectura completa.
- No cambiar contratos publicos ampliamente sin justificacion.
- No introducir nuevas librerias sin necesidad clara.
- No mezclar logica de UI, servicios, dominio y persistencia.
- No usar IA para generar cambios grandes sin revision humana.
- No ocultar errores ni pendientes detectados.

El objetivo del agente es colaborar con el equipo manteniendo el proyecto claro, verificable y
coherente con las decisiones de diseno tomadas durante el obligatorio.
