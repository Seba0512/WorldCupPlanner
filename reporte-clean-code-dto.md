# Reporte de Clean Code — WorldCupPlanner.Dominio.DTO

**Fecha:** 2026-06-17
**Archivos analizados:** 16
**Total de violaciones:** 7 (0 críticas · 3 moderadas · 4 sugerencias)

---

## Resumen Ejecutivo

| Categoría                       | 🔴 Críticas | 🟡 Moderadas | 🟢 Sugerencias |
|---------------------------------|:-----------:|:------------:|:--------------:|
| Números mágicos                 | —           | —            | —              |
| Métodos largos                  | —           | —            | —              |
| Parámetros excesivos            | —           | —            | —              |
| Excepciones silenciadas         | —           | —            | —              |
| Convenciones de nombres         | —           | —            | 1              |
| Una clase por archivo (SRP)     | —           | 2            | —              |
| Estilo de namespace inconsistente | —         | 1            | —              |
| Duplicación de lógica (DRY)     | —           | —            | 1              |
| Otros patrones                  | —           | —            | 2              |
| **Total**                       | **0**       | **3**        | **4**          |

> **Conclusión general:** Los DTOs están bien diseñados. Son inmutables en intención,
> validan `null` con `ArgumentNullException.ThrowIfNull`, usan factory methods `FromEntity`
> consistentes y no contienen lógica de negocio. No hay violaciones críticas. Las
> observaciones son de organización de archivos y consistencia de estilo.

---

## Violaciones Moderadas 🟡

> Importantes pero no bloquean el merge.

### Una clase por archivo (SRP a nivel de archivo)

| Archivo | Problema | Corrección sugerida |
|---------|----------|---------------------|
| `ReglasConfederacionDTO.cs` (líneas 6, 25) | Dos clases en un mismo archivo: `ReglasConfederacionDTO` y `CupoConfederacionDTO` | Mover `CupoConfederacionDTO` a su propio archivo `CupoConfederacionDTO.cs` |
| `ClasificadosFaseGruposDTO.cs` (líneas 6, 35, 56, 73) | Cuatro clases en un mismo archivo: `ClasificadosFaseGruposDTO`, `EquipoClasificadoDTO`, `GrupoClasificadoDTO`, `GrupoOrigenClasificadoDTO` | Separar cada clase en su propio archivo |

### Estilo de namespace inconsistente

| Archivo | Problema | Corrección sugerida |
|---------|----------|---------------------|
| `EquipoDTO.cs` (líneas 4-26) | Usa namespace con llaves (block-scoped) mientras que **todos** los demás DTOs usan file-scoped namespace (`namespace ...;`) | Convertir a `namespace WorldCupPlanner.Dominio.DTO;` para mantener consistencia |

```csharp
// ❌ EquipoDTO.cs — estilo distinto al resto del proyecto
namespace WorldCupPlanner.Dominio.DTO
{
    public class EquipoDTO { ... }
}

// ✅ Consistente con los otros 15 archivos
namespace WorldCupPlanner.Dominio.DTO;

public class EquipoDTO { ... }
```

---

## Sugerencias de Mejora 🟢

- **`ClasificadosFaseGruposDTO.cs` Línea 29:** El parámetro de lambda `par` (de `par.Key`/`par.Value`)
  tiene nombre genérico → renombrar a algo como `equipoConGrupo` para expresar intención.

- **`ActualizacionUsuarioDTO.cs` Línea 1:** Línea en blanco inicial antes del `using` → eliminar.

- **Duplicación (DRY) entre `EquipoDTO` y `EquipoClasificadoDTO`:**
  `EquipoClasificadoDTO` (`ClasificadosFaseGruposDTO.cs:35`) es estructuralmente idéntico a
  `EquipoDTO` (`EquipoDTO.cs:6`): mismas propiedades (`Nombre`, `RankingFIFA`, `Confederacion`,
  `Bandera`) y el mismo `FromEntity(Equipo)`. Evaluar reutilizar `EquipoDTO` en lugar de mantener
  dos tipos paralelos, salvo que se prevea que diverjan.

- **`RankingFIFA` (acrónimo en propiedad):** La convención C# para acrónimos de 3+ letras es
  PascalCase (`RankingFifa`). Se mantiene `RankingFIFA` porque coincide con el término de dominio
  usado en `CLAUDE.md` y en la entidad `Equipo`. **No requiere cambio**, se documenta solo por
  trazabilidad.

---

## Detalle por Archivo

| Archivo | Estado |
|---------|--------|
| `DatosUsuarioDTO.cs` | ✅ Limpio |
| `EstadisticasEquipoDTO.cs` | ✅ Limpio |
| `IncidenciaDTO.cs` | ✅ Limpio |
| `NotificacionDTO.cs` | ✅ Limpio |
| `PosicionEquipoDTO.cs` | ✅ Limpio (método `FromEntity` largo pero es mapeo plano, aceptable) |
| `ReglasConfederacionDTO.cs` | 🟡 2 clases por archivo |
| `TorneoArchivadoDTO.cs` | ✅ Limpio |
| `UsuarioDTO.cs` | ✅ Limpio |
| `ActualizacionUsuarioDTO.cs` | 🟢 Línea en blanco inicial |
| `AuditoriaDTO.cs` | ✅ Limpio |
| `ClasificadosFaseGruposDTO.cs` | 🟡 4 clases por archivo · 🟢 lambda `par` · 🟢 duplicación con EquipoDTO |
| `ConfiguracionTorneoDTO.cs` | ✅ Limpio |
| `EquipoDTO.cs` | 🟡 Namespace block-scoped inconsistente |
| `EstadioDTO.cs` | ✅ Limpio |
| `FixtureDTO.cs` | ✅ Limpio |
| `GrupoDTO.cs` | ✅ Limpio |
| `PartidoDTO.cs` | ✅ Limpio |
