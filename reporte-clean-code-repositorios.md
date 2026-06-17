# Reporte de Clean Code — WorldCupPlanner.AccesoADatos/Repositorios

**Fecha:** 2026-06-17  
**Archivos analizados:** 10  
**Total de violaciones:** 14

---

## Resumen Ejecutivo

| Categoría                        | 🔴 Críticas | 🟡 Moderadas | 🟢 Sugerencias |
|----------------------------------|:-----------:|:------------:|:--------------:|
| Números mágicos                  | —           | 2            | —              |
| Métodos largos                   | —           | 1            | —              |
| Duplicación de lógica (DRY)      | —           | 4            | —              |
| Método stub sin documentar       | —           | 1            | —              |
| Responsabilidad de método (SRP)  | —           | 1            | —              |
| Parámetros excesivos             | —           | —            | —              |
| Excepciones silenciadas          | —           | —            | —              |
| Convenciones de nombres          | —           | —            | —              |
| Código comentado / TODOs         | —           | —            | —              |
| **Total**                        | **0**       | **9**        | **0**          |

> **Diagnóstico general:** El código es prolijo, sigue bien las convenciones de nombres, no tiene excepciones silenciadas ni métodos excesivamente largos. Las violaciones son todas moderadas y se concentran en duplicación estructural (mismo patrón copiado entre repositorios) y dos constantes implícitas sin nombre.

---

## Violaciones Moderadas 🟡

> Importantes pero no bloquean el merge. Resolver en el próximo sprint o en el refactor de la capa de acceso a datos.

---

### 1. `DesvincularInstanciaLocal` duplicada en 6 repositorios

**Archivos:** `RepositorioAuditoriaSql.cs:54`, `RepositorioEquipoSql.cs:78`, `RepositorioEstadioSql.cs:70`, `RepositorioIncidenciaSql.cs:65`, `RepositorioNotificacionSql.cs:56`, `RepositorioUsuarioSql.cs:74`

El método privado `DesvincularInstanciaLocal` es textualmente idéntico en los 6 archivos. Solo cambia el tipo y el DbSet accedido.

```csharp
// ❌ Repetido en cada repositorio
private void DesvincularInstanciaLocal(int id, Equipo entidad)
{
    var local = _contexto.Equipos.Local.FirstOrDefault(e => e.Id == id);
    if (local != null && !ReferenceEquals(local, entidad))
        _contexto.Entry(local).State = EntityState.Detached;
}
```

```csharp
// ✅ Extraer a método de extensión genérico sobre DbContext o a clase base RepositorioBase<T>
protected void DesvincularInstanciaLocal<T>(DbSet<T> conjunto, int id, T entidad)
    where T : class, IEntidadConId
{
    var local = conjunto.Local.FirstOrDefault(e => e.Id == id);
    if (local != null && !ReferenceEquals(local, entidad))
        _contexto.Entry(local).State = EntityState.Detached;
}
```

---

### 2. Métodos `ObtenerEquipoId` / `ObtenerEstadioId` / `ObtenerGrupoId` con estructura idéntica

**Archivo:** `RepositorioPartidoSql.cs:88`, `RepositorioPartidoSql.cs:100`, `RepositorioPartidoSql.cs:112`

Los tres métodos siguen exactamente el mismo patrón: si nulo retorna nulo, si `Id > 0` retorna el Id, si no busca en BD por clave alternativa.

```csharp
// ❌ Tres métodos con la misma estructura — solo cambia el DbSet y la propiedad de búsqueda
private int? ObtenerEquipoId(Equipo? equipo) { ... }
private int? ObtenerEstadioId(Estadio? estadio) { ... }
private int? ObtenerGrupoId(Grupo? grupo) { ... }
```

```csharp
// ✅ Extraer a método genérico privado con un selector de clave alternativa
private int? ObtenerIdDesdeContexto<T>(T? entidad, Func<T, int> selectId,
    Func<SqlContext, T, int?> buscarEnBd) where T : class
{
    if (entidad == null) return null;
    var id = selectId(entidad);
    return id > 0 ? id : buscarEnBd(_contexto, entidad);
}
```

O simplemente tres métodos privados auxiliares sin lógica duplicada (extraer la query a un método `BuscarIdPorNombre`).

---

### 3. Patrón `ObtenerXxxPersistido` duplicado entre repositorios

**Archivos:** `RepositorioEquipoSql.cs:70`, `RepositorioEstadioSql.cs:62`, `RepositorioUsuarioSql.cs:66`

Los tres métodos resuelven la misma lógica: "si tiene Id busca por Id, si no por clave alternativa". La estructura es idéntica.

```csharp
// ❌ RepositorioEquipoSql.cs:70
private Equipo? ObtenerEquipoPersistido(Equipo equipo)
{
    if (equipo.Id > 0)
        return _contexto.Equipos.FirstOrDefault(e => e.Id == equipo.Id);
    return _contexto.Equipos.FirstOrDefault(e => e.Nombre == equipo.Nombre);
}

// ❌ RepositorioEstadioSql.cs:62 — misma estructura
// ❌ RepositorioUsuarioSql.cs:66 — misma estructura
```

Solución: clase base genérica `RepositorioBase<T>` con el método `ObtenerPersistido` parametrizable por el selector de clave alternativa.

---

### 4. `ResolverEquipoExistente` (RepositorioIncidenciaSql) repite lógica de `ObtenerEquipoParaRelacion` (RepositorioGrupoSql)

**Archivos:** `RepositorioIncidenciaSql.cs:72`, `RepositorioGrupoSql.cs:77`

Ambos métodos resuelven un equipo desde el change tracker o desde la BD (primero por Id, luego por Nombre). La diferencia es solo el comportamiento final (crear nuevo vs. marcar como Unchanged).

```csharp
// ❌ Lógica de búsqueda duplicada en dos repositorios distintos
// RepositorioGrupoSql.cs:79-89  y  RepositorioIncidenciaSql.cs:74-91
// ambos hacen: buscar local por Id → buscar BD por Id → buscar local por Nombre → buscar BD por Nombre
```

Solución: Extraer la búsqueda del equipo a un método de extensión en `SqlContext` o a un servicio de resolución interno `ResolvedorEntidades`.

---

### 5. Número mágico implícito: `0` como centinela de "Id no asignado"

**Archivos:** `RepositorioEquipoSql.cs:72`, `RepositorioGrupoSql.cs:71,80,85`, `RepositorioEstadioSql.cs:64`, `RepositorioIncidenciaSql.cs:74`, `RepositorioPartidoSql.cs:91,103,115`, `RepositorioUsuarioSql.cs:68`

La comparación `entidad.Id > 0` para detectar si una entidad es nueva o tiene Id asignado aparece 10 veces en 5 archivos. No hay constante que explique el significado de `0`.

```csharp
// ❌ Significado implícito de 0 repetido sin nombre
if (equipo.Id > 0) return _contexto.Equipos...
```

```csharp
// ✅ Opción 1 — constante en clase base o en dominio
private const int IdNoAsignado = 0;
if (equipo.Id > IdNoAsignado) ...

// ✅ Opción 2 — propiedad en la entidad base
public bool EsNuevo => Id == 0;
if (!equipo.EsNuevo) ...
```

---

### 6. Número mágico: `equipo.RankingInicial == 0`

**Archivos:** `RepositorioEquipoSql.cs:87`, `RepositorioGrupoSql.cs:95`

El `0` se usa como valor centinela de "ranking no asignado" en dos lugares distintos.

```csharp
// ❌ RepositorioEquipoSql.cs:87
if (equipo.RankingInicial == 0)
    equipo.RankingInicial = equipo.RankingFIFA;

// ❌ RepositorioGrupoSql.cs:95
RankingInicial = equipo.RankingInicial == 0 ? equipo.RankingFIFA : equipo.RankingInicial,
```

```csharp
// ✅ Mover esta lógica a la entidad Equipo (donde pertenece) o usar constante
private const int RankingNoAsignado = 0;
```

La lógica `"si RankingInicial == 0, usar RankingFIFA"` está duplicada entre el repositorio de equipos y el de grupos. Además pertenece a la entidad o al servicio de dominio, no a la capa de acceso a datos.

---

### 7. `GuardarConfiguracion` supera las 20 líneas efectivas

**Archivo:** `RepositorioConfiguracionTorneoSql.cs:44`

El método tiene ~23 líneas de código real. El cuerpo principal es la copia manual de todas las propiedades (12 propiedades en el inicializador del objeto), lo que alarga el método innecesariamente.

```csharp
// ❌ 23 líneas — supera el umbral
private void GuardarConfiguracion(ConfiguracionTorneo configuracion, int id) { ... }
```

```csharp
// ✅ Extraer la copia de propiedades a un método auxiliar
private void GuardarConfiguracion(ConfiguracionTorneo configuracion, int id)
{
    var entidad = CopiarConId(configuracion, id);
    DesvincularConfiguracionLocal(id);
    ActualizarOAgregar(entidad, id);
    _contexto.SaveChanges();
}

private ConfiguracionTorneo CopiarConId(ConfiguracionTorneo origen, int id) => new()
{
    Id = id,
    SemillaFixture = origen.SemillaFixture,
    // ...
};
```

---

### 8. `SiguienteId()` retorna `0` sin explicación

**Archivo:** `RepositorioAuditoriaSql.cs:17`

El método está en la interfaz `IRepositorioConId<T>` pero retorna siempre `0`. Si el Id lo genera la base de datos (autoincremento / identity), este método es innecesario o debería documentar por qué devuelve 0.

```csharp
// 🟡 Actualmente:
public int SiguienteId()
{
    return 0; // ¿por qué 0? ¿EF genera el Id automáticamente?
}
```

Si el Id es autogenerado por la BD, considerar:
- Eliminar el método de la interfaz si ningún consumidor lo usa.
- O retornar un valor semánticamente claro: `return IdGeneradoPorBaseDeDatos;` con `const int IdGeneradoPorBaseDeDatos = 0;`.

---

## Detalle por Archivo

### `RepositorioAuditoriaSql.cs`

| Línea | Problema | Severidad |
|-------|----------|-----------|
| 17–20 | `SiguienteId()` retorna `0` sin explicación — ¿contrato vacío o EF lo genera? | 🟡 |
| 54–59 | `DesvincularInstanciaLocal` duplicada en 6 repositorios | 🟡 |

### `RepositorioConfiguracionTorneoSql.cs`

| Línea | Problema | Severidad |
|-------|----------|-----------|
| 44–73 | `GuardarConfiguracion` tiene ~23 líneas efectivas (umbral: 20) | 🟡 |

### `RepositorioEquipoSql.cs`

| Línea | Problema | Severidad |
|-------|----------|-----------|
| 72, 73 | `equipo.Id > 0` — magic number implícito (Id no asignado = 0) | 🟡 |
| 78–83 | `DesvincularInstanciaLocal` duplicada | 🟡 |
| 87–88 | `RankingInicial == 0` duplicado en RepositorioGrupoSql; lógica que pertenece al dominio | 🟡 |

### `RepositorioEstadioSql.cs`

| Línea | Problema | Severidad |
|-------|----------|-----------|
| 64, 65 | `estadio.Id > 0` — mismo patrón sin constante | 🟡 |
| 70–75 | `DesvincularInstanciaLocal` duplicada | 🟡 |

### `RepositorioGrupoSql.cs`

| Línea | Problema | Severidad |
|-------|----------|-----------|
| 71, 74 | `grupo.Id > 0` — patrón sin constante | 🟡 |
| 77–101 | `ObtenerEquipoParaRelacion` repite lógica de búsqueda de `ResolverEquipoExistente` (IncidenciaSql) | 🟡 |
| 95 | `equipo.RankingInicial == 0` duplicado con EquipoSql | 🟡 |

### `RepositorioIncidenciaSql.cs`

| Línea | Problema | Severidad |
|-------|----------|-----------|
| 65–70 | `DesvincularInstanciaLocal` duplicada | 🟡 |
| 72–95 | `ResolverEquipoExistente` repite estructura de `ObtenerEquipoParaRelacion` | 🟡 |

### `RepositorioNotificacionSql.cs`

| Línea | Problema | Severidad |
|-------|----------|-----------|
| 56–61 | `DesvincularInstanciaLocal` duplicada | 🟡 |

### `RepositorioPartidoSql.cs`

| Línea | Problema | Severidad |
|-------|----------|-----------|
| 88–122 | `ObtenerEquipoId`, `ObtenerEstadioId`, `ObtenerGrupoId` son estructuralmente idénticos | 🟡 |
| 91, 103, 115 | `> 0` como centinela de Id no asignado (3 ocurrencias) | 🟡 |

### `RepositorioTorneoArchivadoSql.cs`

Sin violaciones detectadas. ✅

### `RepositorioUsuarioSql.cs`

| Línea | Problema | Severidad |
|-------|----------|-----------|
| 68, 69 | `usuario.Id > 0` — mismo patrón sin constante | 🟡 |
| 74–79 | `DesvincularInstanciaLocal` duplicada | 🟡 |

---

## Puntos positivos destacados

- **Cero excepciones silenciadas** — todos los `catch` ausentes; los errores se propagan correctamente con excepciones de dominio.
- **Convenciones de nombres impecables** — PascalCase, prefijo `_`, `IInterfaz`, sin abreviaciones.
- **Sin métodos de más de 20 líneas** — salvo `GuardarConfiguracion` por 3 líneas.
- **Sin números mágicos en lógica de negocio** — las constantes `IdConfiguracionActual` y `IdConfiguracionParaFixture` están bien nombradas.
- **Guard clauses consistentes** — todos los repositorios validan existencia antes de actualizar/eliminar.
- **`AsNoTracking()` bien aplicado** — en todas las consultas de lectura.
- **`ChangeTracker.Clear()` presente** — en los repositorios que lo necesitan (Grupo, Partido).
