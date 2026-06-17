# readme-writer

## Objetivo

Guiar a un agente de IA para crear o actualizar el archivo `README.md` del repositorio de
**WorldCupPlanner 2026**, respetando el estado real del proyecto y la estructura solicitada por
la consigna del obligatorio.

El README debe servir como primera guia para entender la solucion, conocer las tecnologias
utilizadas, identificar la estructura de proyectos y ejecutar la aplicacion correctamente.

---

## Cuando usar esta skill

Usar esta skill cuando la tarea sea una de estas:

- Crear el archivo `README.md` desde cero.
- Actualizar el `README.md` luego de cambios importantes en el proyecto.
- Corregir instrucciones de ejecucion, Docker, migraciones o tests.
- Documentar tecnologias usadas por la solucion.
- Explicar la estructura de proyectos del repositorio.
- Preparar el README final para la entrega del obligatorio.

No usar esta skill para:

- Modificar codigo productivo.
- Crear migraciones.
- Corregir tests.
- Refactorizar clases.
- Cambiar arquitectura.

Si durante la revision se detecta un problema en el codigo, mencionarlo como observacion, pero
no modificar archivos distintos de `README.md` salvo que el usuario lo pida explicitamente.

---

## Contexto del proyecto

WorldCupPlanner 2026 es una aplicacion desarrollada en C# con .NET 8 y Blazor Server para
gestionar la planificacion y seguimiento del Mundial 2026.

El sistema permite administrar equipos, estadios, usuarios, configuracion del torneo, fixture,
partidos, resultados, clasificacion, cruces eliminatorios, simulacion, importacion de equipos,
auditoria, notificaciones, incidencias y exportacion de informacion.

La solucion esta organizada por capas:

```text
WorldCupPlanner.UI
    -> WorldCupPlanner.Controlador
        -> WorldCupPlanner.Servicios
            -> WorldCupPlanner.Dominio
            -> WorldCupPlanner.AccesoADatos.Interfaces
                -> WorldCupPlanner.AccesoADatos
```

La persistencia se implementa con Entity Framework Core Code First sobre SQL Server/Azure SQL
Edge en Docker. Los servicios deben depender de interfaces de repositorio y no directamente de
`SqlContext`.

---

## Archivos que el agente debe revisar antes de escribir

Antes de crear o actualizar el README, revisar el estado real del repositorio. Como minimo:

1. `WorldCupPlanner.sln`
2. `docker-compose.yml`
3. `WorldCupPlanner.UI/Program.cs`
4. `WorldCupPlanner.UI/appsettings.json`, si existe.
5. Archivos `.csproj` de los proyectos principales.
6. `WorldCupPlanner.AccesoADatos/SqlContext.cs`
7. Carpeta `WorldCupPlanner.AccesoADatos/Migraciones`
8. Proyectos de tests existentes.
9. README actual, si ya existe.

No asumir nombres de proyectos, comandos o tecnologias sin verificarlos en el repositorio.

---

## Contenido obligatorio del README

El README debe incluir, como minimo, estas secciones:

1. Titulo del proyecto.
2. Breve descripcion de la solucion.
3. Tecnologias utilizadas.
4. Estructura de la solucion.
5. Requisitos previos.
6. Instrucciones para levantar la base de datos.
7. Instrucciones para aplicar migraciones.
8. Instrucciones para ejecutar la aplicacion.
9. Instrucciones para ejecutar tests.
10. Notas utiles para la entrega o el desarrollo.

---

## Formato recomendado

Usar esta estructura:

```md
# WorldCupPlanner 2026

## Descripcion

Breve descripcion del objetivo del sistema y problema que resuelve.

## Tecnologias Utilizadas

- .NET 8
- C#
- Blazor Server
- Entity Framework Core
- SQL Server / Azure SQL Edge
- Docker
- MSTest

## Estructura De La Solucion

Tabla o lista con cada proyecto y su responsabilidad.

## Requisitos Previos

- .NET 8 SDK
- Docker Desktop
- Herramientas de Entity Framework Core
- DBeaver u otro cliente SQL, opcional

## Configuracion De Base De Datos

Comandos para levantar Docker y aplicar migraciones.

## Ejecucion De La Aplicacion

Comandos para ejecutar el proyecto Blazor.

## Ejecucion De Tests

Comandos para ejecutar los tests.

## Notas

Consideraciones sobre datos, migraciones, usuarios o limitaciones conocidas.
```

---

## Descripcion de la solucion

La descripcion debe ser breve y concreta. Debe mencionar que:

- Es una aplicacion para gestionar el Mundial 2026.
- Trabaja con 48 equipos y 12 grupos.
- Permite gestionar fase de grupos y etapas eliminatorias.
- Incluye usuarios, roles, equipos, estadios, fixture, resultados, simulacion, auditoria,
  notificaciones, incidencias e importacion/exportacion.
- Usa persistencia en base de datos para conservar el estado del sistema.

Evitar convertir el README en la documentacion completa del obligatorio. Debe ser practico.

---

## Tecnologias

Listar solo tecnologias realmente utilizadas en el repositorio. Revisar `.csproj`,
`docker-compose.yml` y `Program.cs`.

Tecnologias esperadas:

- C#
- .NET 8
- Blazor Server
- Entity Framework Core
- SQL Server / Azure SQL Edge
- Docker
- MSTest
- CsvHelper, si sigue usado para importacion CSV.
- ClosedXML, si sigue usado para exportacion.

No incluir tecnologias no verificadas.

---

## Estructura de la solucion

Incluir una tabla similar a esta, ajustando segun el estado real:

| Proyecto | Responsabilidad |
|---|---|
| `WorldCupPlanner.Dominio` | Entidades, enumeraciones, excepciones y reglas propias del dominio. |
| `WorldCupPlanner.Dominio.DTO` | Objetos de transferencia de datos. |
| `WorldCupPlanner.AccesoADatos.Interfaces` | Contratos de repositorio. |
| `WorldCupPlanner.AccesoADatos` | Persistencia con EF Core, `SqlContext`, migraciones y repositorios SQL. |
| `WorldCupPlanner.Servicios` | Casos de uso y reglas de aplicacion. |
| `WorldCupPlanner.Controlador` | Capa intermedia entre UI y servicios. |
| `WorldCupPlanner.UI` | Interfaz Blazor Server. |

Agregar proyectos de test en una seccion breve aparte, sin mezclarlos con la arquitectura
productiva.

---

## Instrucciones de ejecucion

Las instrucciones deben estar probadas o al menos basadas en archivos reales del repositorio.

Comandos esperados, ajustando si cambia el proyecto:

```bash
docker compose up -d
```

```bash
dotnet ef database update --project WorldCupPlanner.AccesoADatos --startup-project WorldCupPlanner.UI
```

```bash
dotnet run --project WorldCupPlanner.UI
```

Si el proyecto usa otra forma de aplicar migraciones durante el arranque, explicarlo y no duplicar
pasos innecesarios.

---

## Instrucciones de tests

Agregar un comando simple:

```bash
dotnet test
```

Si hay tests que dependen de configuracion especial, mencionarlo claramente.

No afirmar que todos los tests pasan si no fueron ejecutados.

---

## Reglas de redaccion

- Escribir en espanol.
- Mantener un tono claro, tecnico y practico.
- No usar explicaciones excesivamente largas.
- No inventar funcionalidades.
- No mencionar decisiones que no esten reflejadas en el repositorio.
- Usar nombres exactos de proyectos, carpetas y comandos.
- Usar tablas cuando faciliten la lectura.
- Usar bloques de codigo para comandos.
- Evitar texto de marketing.
- Evitar repetir la documentacion completa del obligatorio.

---

## Validaciones antes de terminar

Antes de entregar el README, verificar:

- El archivo esta en la raiz del repositorio como `README.md`.
- La descripcion coincide con el sistema actual.
- Las tecnologias listadas aparecen realmente en el proyecto.
- Los comandos usan nombres reales de proyectos.
- Las instrucciones de Docker coinciden con `docker-compose.yml`.
- Las instrucciones de migraciones usan el proyecto correcto.
- Las instrucciones de ejecucion apuntan a `WorldCupPlanner.UI`, salvo que el repo haya cambiado.
- Las instrucciones de tests son correctas para la solucion.
- No se afirma que algo fue probado si no se ejecuto.

---

## Formato de respuesta esperado del agente

Cuando el agente use esta skill, debe responder con:

```md
## Analisis

Resumen breve del estado actual revisado.

## Cambios realizados

Archivos creados o modificados.

## Contenido principal

Resumen de las secciones incluidas en el README.

## Verificacion

Comandos ejecutados o aclaracion de que no se ejecutaron.

## Pendientes

Aspectos que requieren confirmacion humana, si existen.
```

---

## Definition of Done

La tarea se considera terminada cuando:

- Existe `README.md` en la raiz del repositorio.
- El README contiene descripcion, tecnologias, estructura e instrucciones de ejecucion.
- Las instrucciones incluyen Docker, migraciones, ejecucion de la app y tests cuando corresponde.
- El contenido esta alineado con el estado real del repositorio.
- No hay informacion inventada ni desactualizada.
- El texto es claro para una persona que clona el repositorio por primera vez.
