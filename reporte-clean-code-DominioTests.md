# Reporte de Clean Code — WorldCupPlanner.DominioTests

**Fecha:** 2026-06-17
**Archivos analizados:** 14
**Total de violaciones:** 9 (0 críticas · 6 moderadas · 3 sugerencias)

> Contexto: es un proyecto de **tests MSTest**. La skill permite explícitamente
> *"números en pruebas unitarias con contexto claro"*, por lo que los literales
> de dominio (`300`, `2500`, `48`, `16`, `60000`, etc.) **no** se cuentan como
> números mágicos. Tampoco hay métodos largos (>20 líneas de código real),
> excepciones silenciadas, `IDisposable` sin `using` ni anidación excesiva.

---

## Resumen Ejecutivo

| Categoría                       | 🔴 Críticas | 🟡 Moderadas | 🟢 Sugerencias |
|---------------------------------|:-----------:|:------------:|:--------------:|
| Números mágicos                 | —           | —            | —              |
| Métodos largos                  | —           | —            | —              |
| Parámetros excesivos            | —           | —            | —              |
| Excepciones silenciadas         | —           | —            | —              |
| Duplicación de lógica (DRY)     | —           | 1            | 1              |
| Convenciones / nombres          | —           | 4            | —              |
| Números mágicos repetidos       | —           | 1            | —              |
| Comentarios que explican el qué | —           | —            | 2              |
| **Total**                       | **0**       | **6**        | **3**          |

---

## Violaciones Críticas 🔴

Ninguna. El código de test cumple los umbrales bloqueantes de la skill.

---

## Violaciones Moderadas 🟡

> Importantes pero no bloquean el merge.

### `PartidoTests.cs` — Duplicación de construcción de `Partido`

| Línea | Problema | Corrección sugerida |
|-------|----------|---------------------|
| 19–408 | El objeto `Partido` se construye in-line con un inicializador casi idéntico en ~18 tests (mismo `Estadio`, mismos `Equipo` Uruguay/Argentina, misma `Fecha`). Viola DRY. A diferencia de `FixtureTests`, `GrupoTests`, `PosicionEquipoTests` y `ClasificadosFaseGruposTests`, esta clase **no** tiene método fábrica. | Extraer un helper `CrearPartido(...)` con valores por defecto y solo sobreescribir goles/fase/penales por test. |

```csharp
// ✅ Sugerencia
private static Partido CrearPartido(
    int golesLocal = 0, int golesVisitante = 0,
    FaseTorneo fase = FaseTorneo.FaseDeGrupos, Equipo? ganadorPenales = null)
{
    return new Partido
    {
        Fecha = DateTime.Now,
        Estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 },
        EquipoLocal = new Equipo { Nombre = "Uruguay", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL },
        EquipoVisitante = new Equipo { Nombre = "Argentina", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL },
        Fase = fase, GolesLocal = golesLocal, GolesVisitante = golesVisitante, GanadorPorPenales = ganadorPenales
    };
}
```

### Nombres de test poco expresivos (typos que afectan la intención)

| Archivo | Línea | Problema | Corrección |
|---------|-------|----------|-----------|
| `ClasificadosFaseGruposTests.cs` | 10 | `..._IncluseLosTresListados` — "Incluse" no es palabra | `..._IncluyeLosTresListados` |
| `UsuarioTests.cs` | 358 | variable `rolesDepues` — typo | `rolesDespues` |
| `UsuarioTests.cs` | 642 | `EstablecerContrasenia_LimpieFlag...` — "Limpie" rompe el patrón `Metodo_Escenario_Resultado` | `..._LimpiaFlagDebeCambiarContrasenia` |

### `UsuarioTests.cs` — Identificadores con tildes en nombres de método

| Línea | Problema | Corrección |
|-------|----------|-----------|
| 158, 171, 184 | `EstablecerContrasenia_SinMayúsculas`, `_SinMinúsculas`, `_SinNúmeros` usan caracteres acentuados en el nombre del método. Las convenciones de C# (y el resto del proyecto) usan identificadores ASCII. | `_SinMayusculas`, `_SinMinusculas`, `_SinNumeros` |

### Helpers de fábrica inconsistentes (static vs instancia)

| Archivo | Línea | Problema | Corrección |
|---------|-------|----------|-----------|
| `FixtureTests.cs` | 49 | `private Partido CrearPartido(...)` de instancia | hacerlo `static` |
| `GrupoTests.cs` | 298, 311 | `CrearGrupoValido` / `CrearEquipoValido` de instancia | hacerlos `static` (igual que en `ClasificadosFaseGruposTests` y `PosicionEquipoTests`) |

### `EquipoTests.cs` — Número mágico repetido (no es dato de dominio único)

| Línea | Problema | Corrección |
|-------|----------|-----------|
| 204, 217, 224 | El límite `2_800_000` del tamaño de bandera aparece 3 veces y se recalcula (`2_800_000 - prefijo.Length`). No es un valor "obvio" de dominio sino un umbral técnico. | Extraer `private const int MaxLongitudBandera = 2_800_000;` |

---

## Sugerencias de Mejora 🟢

- **`ValidadorContraseniaTests.cs` / `UsuarioTests.cs`** (líneas 149, 162, 175, 188, 201): comentarios como `// 7 caracteres`, `// Sin mayúsculas`, `// Sin números` describen *qué* es el dato. En tests son tolerables, pero el nombre del test ya lo expresa → se pueden eliminar para reducir ruido.
- **`EquipoTests.cs` línea 64** (`EstadioTests.cs`): el dato `"Estadio Centenaario"` tiene un typo en el valor de prueba; no afecta el assert pero conviene corregirlo por prolijidad.
- **DRY de datos de equipo**: `new Equipo { ... RankingFIFA = 500/1000, Confederacion = ... }` se repite en muchos archivos. Si crece el suite, considerar un builder/fábrica de test compartido en el proyecto de tests.

---

## Detalle por archivo (sin hallazgos)

`AuditoriaTests.cs`, `ConfiguracionTorneoTests.cs`, `ExcepcionesTests.cs`,
`ActualizacionUsuarioTests.cs`, `EstadioTests.cs`, `PosicionEquipoTests.cs`,
`ReglasConfederacionTests.cs` — ✅ limpios bajo los umbrales de la skill
(AAA correcto, nombres `Metodo_Escenario_Resultado`, sin estado compartido).
