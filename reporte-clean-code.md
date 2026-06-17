# Reporte de Clean Code — WorldCupPlanner 2026

**Fecha:** 2026-06-17
**Alcance:** `AccesoADatos/Repositorios`, `AccesoADatos.Interfaces`, `AccesoADatosTest`, `Controlador`
**Archivos analizados:** 44 (`.cs`, excluyendo `obj/` y generados)
**Total de violaciones:** 0 críticas · 3 moderadas · 2 sugerencias

---

## Resumen Ejecutivo

| Categoría                | 🔴 Críticas | 🟡 Moderadas | 🟢 Sugerencias |
|--------------------------|:-----------:|:------------:|:--------------:|
| Números mágicos          | 0           | —            | —              |
| Métodos largos (>20)     | 0           | —            | —              |
| Parámetros excesivos (>3)| 0           | —            | —              |
| Excepciones silenciadas  | 0           | —            | —              |
| Convenciones de nombres  | 0           | 0            | —              |
| Retorno `null`           | —           | 1            | —              |
| Constructores telescópicos| —          | 2            | —              |
| Otros patrones           | —           | 0            | 2              |
| **Total**                | **0**       | **3**        | **2**          |

El código revisado cumple de forma consistente con las convenciones del proyecto: campos privados con `_`, interfaces con `I`, PascalCase/camelCase correctos, métodos cortos de una sola responsabilidad, parámetros agrupados en `record` de comando (`ComandoSimularPartidos`, `ComandoGuardarPartido`, etc.), constantes con nombre (`IdNoAsignado`, `IdConfiguracionActual`) y tests con patrón Arrange/Act/Assert y nombres `Metodo_Escenario_Resultado`. No se detectaron números mágicos, métodos largos, catch silenciados ni código comentado.

---

## Violaciones Críticas 🔴

> Ninguna. No hay bloqueantes para merge.

---

## Violaciones Moderadas 🟡

### `WorldCupPlanner.Controlador/MapeadorDtoDominio.cs`

| Línea | Problema | Corrección sugerida |
|-------|----------|---------------------|
| 126 | `MaterializarEquipo` retorna `return null!;` (regla #9 — Retorno Null). El `null!` suprime la advertencia del compilador y propaga un posible `NullReferenceException` a `Partido.EquipoLocal/Visitante/GanadorPorPenales`. | Documentar el porqué (los penales pueden no existir) o exponer el tipo de retorno como `Equipo?` y dejar que el llamador decida, evitando el `null!`. |

### Constructores telescópicos (acoplamiento por composición opcional)

| Archivo | Líneas | Problema | Corrección sugerida |
|---------|--------|----------|---------------------|
| `Controlador/ControladorFixture.cs` | 18–58 | 4 constructores encadenados con servicios opcionales (`ServicioOrquestadorFixture?`, `ServicioPersistenciaFixture?`, etc.) que luego obligan a chequeos `if (_x == null) throw` dispersos. | Considerar un único objeto de dependencias o separar responsabilidades en colaboradores enfocados; reduce las combinaciones de constructores y los `InvalidOperationException` defensivos. |
| `Controlador/ControladorPartido.cs` | 15–28 | 2 constructores con `ServicioOrquestadorPartido?` opcional + chequeo `if (_orquestadorPartido == null) throw` en `GuardarCambiosPartido` (línea 115). | Igual que arriba: la opcionalidad del orquestador sugiere dos roles que podrían vivir en clases separadas. |

> Nota: el patrón es defendible (permite reutilizar el controlador con distinto grado de orquestación) y es coherente en todo el módulo. Se marca como deuda de diseño, no como error.

---

## Sugerencias de Mejora 🟢

- **`AccesoADatos/Repositorios/RepositorioAuditoriaSql.cs` línea 12-15:** `SiguienteId()` retorna el literal `0`. Es correcto (el `Id` lo autogenera EF), pero el `0` "pelado" no expresa intención. → Reutilizar la constante `IdNoAsignado` ya existente en `RepositorioBase` o agregar un comentario del *porqué* (la BD asigna el id).
- **Consistencia de inicialización de listas vacías:** conviven `new List<PosicionEquipoDTO>()` (`ControladorClasificacion.cs` líneas 37, 49) y la sintaxis de colección `[]` (`ControladorFixture.cs` líneas 114, 117). → Unificar a `[]` para mantener un solo estilo en la capa de controladores.

---

## Detalle por archivo (hallazgos)

### `WorldCupPlanner.Controlador/MapeadorDtoDominio.cs`

**Línea 123-134** 🟡 `[Retorno Null]` — `null!` propaga nulabilidad oculta.

```csharp
// ❌ Actual
private static Equipo MaterializarEquipo(EquipoDTO? dto, Dictionary<string, Equipo> equiposPorNombre)
{
    if (dto == null)
        return null!;
    ...
}

// ✅ Sugerencia — expresar la nulabilidad en la firma
private static Equipo? MaterializarEquipo(EquipoDTO? dto, Dictionary<string, Equipo> equiposPorNombre)
{
    if (dto == null)
        return null;
    ...
}
```

### `WorldCupPlanner.AccesoADatos/Repositorios/RepositorioAuditoriaSql.cs`

**Línea 12-15** 🟢 `[Intención]` — literal `0` sin nombre.

```csharp
// ❌ Actual
public int SiguienteId()
{
    return 0;
}

// ✅ Sugerencia — la BD autogenera el Id; reutilizar la constante existente
public int SiguienteId() => IdNoAsignado; // el Id real lo asigna EF al persistir
```

---

## Conclusión

Los cuatro módulos auditados están en muy buen estado de Clean Code. No hay violaciones críticas ni moderadas que bloqueen el merge; los puntos listados son mejoras menores de legibilidad y diseño. Las capas mantienen una separación correcta (repositorios sin lógica de negocio, controladores como fachada delgada hacia servicios, interfaces minimalistas) y los tests son independientes y descriptivos.
