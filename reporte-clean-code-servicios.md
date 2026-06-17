# Reporte de Clean Code — WorldCupPlanner.Servicios

**Fecha:** 2026-06-17
**Archivos analizados:** 45 (`.cs`, excluyendo `obj/` y `bin/`)
**Total de violaciones:** 14 (1 🔴 · 9 🟡 · 4 🟢)

> Nota: los scripts `.sh` de la skill están pensados para Linux (`/mnt/user-data/...`) y vienen
> con placeholders rotos (`FILE="la"`). Las mismas verificaciones se ejecutaron con las
> herramientas nativas (Grep/Read) sobre Windows.

---

## Resumen Ejecutivo

| Categoría                | 🔴 Críticas | 🟡 Moderadas | 🟢 Sugerencias |
|--------------------------|:-----------:|:------------:|:--------------:|
| Números mágicos          | 1           | 1            | 2              |
| Métodos largos           | —           | 1            | —              |
| Parámetros excesivos     | 0           | —            | —              |
| Excepciones silenciadas  | 0           | 1            | —              |
| Convenciones de nombres  | 0           | 0            | —              |
| SRP / clases grandes     | —           | 2            | —              |
| Duplicación (DRY)        | —           | 1            | —              |
| Otros patrones           | —           | 0            | 2              |
| **Total**                | **1**       | **6**        | **4**          |

**Veredicto general:** código de alta calidad. No se detectaron `catch` vacíos, campos sin
prefijo `_`, constantes en `SCREAMING_SNAKE_CASE`, interfaces sin `I`, `IDisposable` sin `using`,
ni `TODO`/`FIXME` sin ticket. La mayoría de los servicios ya están bien factorizados en métodos
cortos con constantes nombradas. Los hallazgos se concentran en pocos archivos.

---

## Violaciones Críticas 🔴

> Deben corregirse antes de mergear.

### `WorldCupPlanner.Servicios/ServicioSimulacion.cs`

La clase define ~17 constantes nombradas, pero los métodos de *tuning* del motor "Ranking + Forma"
contienen literales numéricos embebidos sin nombre — exactamente el tipo de número mágico que la
guía del proyecto prohíbe (pesos y umbrales de ajuste deportivo).

| Línea | Problema | Corrección sugerida |
|-------|----------|---------------------|
| 161 | Umbral `0.70` en `GenerarGolesPorFuerza` | `const double UmbralFuerzaAlta = 0.70;` |
| 166 | `0.18` y `0.55` (ventaja / probabilidad bonus) | `VentajaParaGolExtra`, `ProbabilidadGolPorVentaja` |
| 171 | `-0.24` y `0.45` (desventaja / probabilidad) | `DesventajaParaGolMenos`, `ProbabilidadGolPorDesventaja` |
| 176 | `0.30` y `0.30` (fuerza baja / probabilidad) | `UmbralFuerzaBaja`, `ProbabilidadGolPorFuerzaBaja` |
| 234, 237 | `3.0` (puntos máximos por partido) | `const double PuntosMaximosPorPartido = 3.0;` |
| 236 | `+ 3` y `6.0` (normalización de diferencia de goles) | `RangoDiferenciaGoles` / constante con nombre |
| 239 | `0.55`, `0.30`, `0.15` (pesos de forma) | `PesoPuntos`, `PesoDiferencia`, `PesoGoles` |
| 248 | `3` (puntos por victoria) | `const int PuntosPorVictoria = 3;` |

```csharp
// ❌ Actual (líneas 161-181, 239)
if (fuerzaPropia >= 0.70) goles++;
if (ventaja >= 0.18 && random.NextDouble() < 0.55) goles++;
...
return ratioPuntos * 0.55 + ratioDiferencia * 0.30 + ratioGoles * 0.15;

// ✅ Sugerido
if (fuerzaPropia >= UmbralFuerzaAlta) goles++;
if (ventaja >= VentajaParaGolExtra && random.NextDouble() < ProbabilidadGolPorVentaja) goles++;
...
return ratioPuntos * PesoPuntos + ratioDiferencia * PesoDiferencia + ratioGoles * PesoGoles;
```

> Excepciones aceptadas y **no** marcadas: `random.Next(2)` (líneas 305, 317) representa la
> elección entre dos equipos; `+ 0.5` de redondeo en `ServicioRankingDinamico` (línea 308).

---

## Violaciones Moderadas 🟡

> Importantes pero no bloquean el merge. Resolver antes del próximo sprint.

### `WorldCupPlanner.Servicios/ServicioFixture.cs` — 675 líneas, 1 clase

| Línea | Problema | Corrección sugerida |
|-------|----------|---------------------|
| 7–675 | **SRP**: la clase mezcla *generación* del fixture (`GenerarFixture`, `Crear...RoundRobin`, asignación a grupos) con *consultas de estado de fase* (`PuedeGenerarFase`, `ObtenerMensajeCondicionFase`, `ObtenerPartidos...`, mensajes de UI). | Separar en `ServicioFixture` (generación) y `ServicioConsultaFases`/`ServicioEstadoFase` (consultas y mensajes). |
| 444–448 | Constantes (`NumeroJornadaInicial`, `PrimeraJornada`...) declaradas a mitad de archivo, después de varios métodos. | Mover al bloque de constantes del inicio (líneas 10–25). |

### `WorldCupPlanner.Servicios/ServicioCruces.cs` — 557 líneas

| Línea | Problema | Corrección sugerida |
|-------|----------|---------------------|
| 271–384 | **DRY**: `GenerarOctavos`, `GenerarCuartos` y `GenerarSemifinales` son casi idénticos (validar → obtener ganadores → loop creando partidos). | Extraer un método privado `GenerarFaseEliminatoria(partidosAnteriores, fase, cantidad, emparejamiento, ...)`. |
| 337, 338, 375, 376 | Literal `2` (`i * 2`, `i * 2 + 1`) cuando ya existe `PasoEntreParesDeEquipos = 2` sin usarse aquí. | Reutilizar la constante o nombrar el emparejamiento de pares. |

### `WorldCupPlanner.Servicios/ServicioRankingDinamico.cs`

| Línea | Problema | Corrección sugerida |
|-------|----------|---------------------|
| 357, 359, 360 | Número mágico `8` (`Take(8)`, `> 8`, `- 8`) — límite de cambios mostrados en el detalle de auditoría. | `const int MaxCambiosEnDetalleAuditoria = 8;` |
| 92–157 | **Método largo**: `RecalcularRankings` (~45 líneas de código real, > 20). Hace snapshot, reset, reproducción del historial y diff/persistencia. | Extraer `CapturarRankingPrevio`, `ReproducirPartidos`, `PersistirYCalcularCambios`. |

### `WorldCupPlanner.Servicios/ImportacionLegibilidad.cs`

| Línea | Problema | Corrección sugerida |
|-------|----------|---------------------|
| 125, 160 | Captura genérica `catch (Exception ex)`. **No** está silenciada (registra el error y continúa/retorna), lo cual es defendible para una importación masiva que reporta éxitos y errores. Aun así, conviene capturar tipos específicos. | Capturar `FormatException`/excepciones de dominio esperadas en lugar de `Exception`. |

---

## Sugerencias de Mejora 🟢

- **`ServicioFixture.cs` líneas 106, 128:** strings de error con `48` y `4` literales
  (`"...con más de 48 equipos."`, `"...menos de 4 estadios."`) duplican las constantes
  `ConstantesTorneo.CantidadEquipos` y `CantidadMinimaEstadios`. Interpolar la constante.
- **`ServicioEquipo.cs` línea 145:** `"Ya hay 48 equipos registrados."` duplica `MaximoEquipos`.
  Usar `$"Ya hay {MaximoEquipos} equipos registrados."`.
- **`ServicioRankingDinamico.cs` líneas 22–50:** cuatro constructores telescópicos. Considerar
  un único constructor con parámetros opcionales o un objeto de configuración.
- **Consistencia de códigos de auditoría:** `ServicioUsuario` usa `"ALTA_USUARIO"`/`"EDICION_USUARIO"`
  mientras `ServicioEquipo` usa `"Alta"`/`"Edicion"`. Son literales de datos (no identificadores C#,
  por eso no violan naming), pero unificar el estilo mejora la legibilidad del log.

---

## Verificaciones sin hallazgos ✅

- Excepciones silenciadas (`catch {}`): ninguna. El `catch (EntidadInvalidaExcepcion)` de
  `ServicioFixture.cs:52` es un *retry* intencional documentado que relanza al agotar intentos.
- Campos privados: todos con prefijo `_`.
- Constantes: todas en PascalCase (sin `SCREAMING_SNAKE_CASE`).
- Interfaces: todas con prefijo `I`.
- `IDisposable`: `StreamReader`/`MemoryStream`/`StreamWriter` siempre con `using var`.
- `TODO`/`FIXME`/`HACK` sin ticket: ninguno.
- Parámetros excesivos: ninguno; las firmas con muchos datos ya usan objetos de contexto
  (`ContextoSorteo`, `ContextoGeneracionCruces`, `DatosPartidoEliminatorio`, etc.).
