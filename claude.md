le cambie una linea
# Instrucciones para el Agente de IA — WorldCupPlanner 2026

## Contexto del proyecto

Este repositorio contiene el **Obligatorio 1 de Diseño de Aplicaciones 1** (ORT Uruguay, Facultad de Ingeniería). La aplicación se llama **WorldCupPlanner 2026** y gestiona la fase de grupos del Mundial de Fútbol con 48 equipos (12 grupos de 4), generación de fixture y cruces eliminatorios con sorteos determinísticos reproducibles.

- **Tecnología**: C# · .NET Core 8 · Blazor Server App
- **Testing**: MSTest · TDD (Test-First Development + Refactoring)
- **Control de versiones**: Git · Gitflow
- **CI**: GitHub Actions (pipeline que corre los tests en cada PR a `main`)
- **Almacenamiento**: En memoria (sin base de datos en esta entrega)

---

## Arquitectura y estructura del proyecto

La solución está dividida en capas. La lógica de negocio debe estar **completamente separada** de la interfaz de usuario:

```
WorldCupPlanner/
├── WorldCupPlanner.Domain/        # Entidades, excepciones, enumeraciones
├── WorldCupPlanner.Services/      # Lógica de negocio, validaciones, algoritmos
├── WorldCupPlanner.DataAccess/    # Repositorios en memoria (interfaces + implementaciones)
├── WorldCupPlanner.WebApp/        # Blazor Server App (UI únicamente)
└── WorldCupPlanner.Tests/         # Proyectos MSTest (espejo de Domain + Services)
```

- La UI **no debe contener lógica de negocio**.
- Los servicios dependen de interfaces de repositorio, nunca de implementaciones concretas.
- Usar **inyección de dependencias** a través del contenedor de .NET.

---

## Convenciones de codificación

Seguir las convenciones oficiales de C#:
https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

Reglas adicionales del proyecto:

- **Clases**: PascalCase. **Métodos**: PascalCase. **Variables locales / parámetros**: camelCase.
- **Interfaces**: prefijo `I` (ej. `IEquipoRepository`).
- **No usar abreviaturas** 
- **Un método = una responsabilidad** (Single Responsibility a nivel método).
- **Sin números mágicos**: usar constantes con nombre (`MaxEquipos = 48`, `MaxPartidosPorDia = 3`, etc.).
- **Sin comentarios que expliquen el qué**: el código debe ser autoexplicativo. Comentarios solo para el *porqué*.
- Máximo **3 parámetros por método**; usar objetos de configuración si se necesitan más.
- Métodos de más de 20 líneas son candidatos a refactor.

---

## Metodología TDD

Todo el código de dominio y servicios **debe desarrollarse con TDD**:

1. Escribir el test primero (RED).
2. Implementar el mínimo código para que pase (GREEN).
3. Refactorizar manteniendo los tests en verde (REFACTOR).

Para el requerimiento **"Generación de fixture"**, cada commit debe indicar el paso del ciclo:
- Prefijo de commit: `[RED]`, `[GREEN]` o `[REFACTOR]`.

Para el resto de la funcionalidad, hacer commit cuando los tests estén en GREEN.

**No generar código de dominio o servicios sin su test correspondiente.**

---

## Entidades del dominio y sus validaciones

### Usuario
- Nombre, Apellido (obligatorios)
- Email (obligatorio, formato válido)
- FechaNacimiento (formato MM-DD-AAAA)
- Contraseña (mínimo 8 chars, mayúscula, minúscula, número, carácter especial; almacenada cifrada, nunca en texto plano)
- Roles: `Administrador`, `Editor` (no excluyentes)

### Equipo
- Nombre (obligatorio, 1–60 chars, único en el sistema)
- Confederacion (obligatorio; enum: `AFC`, `CAF`, `CONCACAF`, `CONMEBOL`, `OFC`, `UEFA`)
- RankingFIFA (obligatorio; entero 300–2500)
- Cupos: UEFA=16, CONMEBOL=7, CONCACAF=7, CAF=9, AFC=8, OFC=1

### Estadio
- Nombre (obligatorio, 1–80 chars, único en el sistema)
- Ciudad (obligatorio, 1–60 chars)
- Descripcion (opcional, hasta 400 chars)
- CapacidadPublico (obligatorio, >= 20000)

### Partido
- Id (numérico incremental autogenerado)
- Fecha (obligatorio, ISO 8601)
- Estadio (obligatorio)
- EquipoLocal / EquipoVisitante (obligatorios, distintos entre sí)
- Grupo (etiqueta A–L)
- GolesLocal, GolesVisitante, Vencedor

---

## Algoritmos determinísticos

### Fisher-Yates con semilla
Usar `System.Random(semilla)` para garantizar reproducibilidad. Es el algoritmo **obligatorio** para:
- Resolver empates de RankingFIFA al ordenar equipos en bombos.
- Barajar listas de clasificados para cruces eliminatorios.

```csharp
// Ejemplo de implementación esperada
public static void FisherYatesShuffle<T>(List<T> lista, int semilla)
{
    var random = new Random(semilla);
    for (int i = lista.Count - 1; i > 0; i--)
    {
        int j = random.Next(i + 1);
        (lista[i], lista[j]) = (lista[j], lista[i]);
    }
}
```

### Parámetros configurables (por UI o appsettings)
| Parámetro             | Valor por defecto | Descripción                                              |
|-----------------------|-------------------|----------------------------------------------------------|
| `SemillaFixture`      | (configurable)    | Semilla para orden de equipos en bombos y fixture        |
| `SemillaCompletar`    | (configurable)    | Semilla para generación automática de equipos ficticios  |
| `SemillaCrucesFase`   | (configurable)    | Semilla para sorteo de cruces eliminatorios              |
| `SemillaSimulation`   | (configurable)    | Semilla para simulación de resultados                    |
| `FechaInicioTorneo`   | 2026-06-01        | Fecha base del calendario                                |
| `MaxPartidosPorDia`   | 3                 | Tope de partidos por jornada                             |
| `SeparacionEntreFechas` | 3 días          | Intervalo entre jornadas dentro de un grupo              |

---

## Normalización de strings

Cuando se necesite comparar o normalizar texto (nombres de estadios, equipos, etc.):
1. Convertir a minúsculas.
2. Eliminar tildes / diacríticos.
3. Reemplazar caracteres no alfanuméricos por espacio.
4. Colapsar múltiples espacios en uno.
5. Aplicar `Trim()`.

---

## Auditoría (log)

Registrar **siempre** con timestamp ISO-8601 y usuario autenticado:
- Altas/ediciones de usuarios
- Altas, ediciones y eliminaciones de estadios
- Altas, ediciones y eliminaciones de equipos
- Generación automática de equipos (incluir semilla usada)
- Generación de fixture (incluir semilla usada)
- Modificación de partidos
- Realización de sorteo para cruces (incluir semilla usada)
- Importación de equipos (éxitos y errores)

---

## Testing — lineamientos

- Framework: **MSTest**
- Cobertura mínima esperada: **90% de líneas** por clase (medida con JetBrains dotCover).
- Cada test debe tener nombre descriptivo: `Metodo_Escenario_ResultadoEsperado`.
- Usar el patrón **Arrange / Act / Assert** con comentarios separadores.
- Los tests **no deben depender entre sí** ni compartir estado global.
- Mockear dependencias externas (repositorios) con dobles de prueba o implementaciones en memoria.
- No testear la UI de Blazor con MSTest.

```csharp
// Ejemplo de nombre de test
[TestMethod]
public void AgregarEquipo_CuandoNombreYaExiste_LanzaExcepcion()
{
    // Arrange
    ...
    // Act & Assert
    Assert.ThrowsException<NombreDuplicadoException>(() => servicio.AgregarEquipo(equipo));
}
```

---

## Gitflow y commits

- Rama principal: `main`
- Rama de desarrollo: `develop`
- Features: `feature/<nombre-descriptivo>`
- Ramas para alternativas de importación: `feature/importacion-extensibilidad`, `feature/importacion-bajo-acoplamiento`, `feature/importacion-legibilidad`
- **No hacer push directo a `main`**; siempre a través de Pull Request.
- El pipeline de GitHub Actions debe ejecutarse en cada PR y reportar resultados visibles.

Formato de mensaje de commit:
```
[RED|GREEN|REFACTOR|FEAT|FIX|REFACTOR|DOCS]: descripción breve en español

Cuerpo opcional explicando el porqué del cambio.
```

---

## Roles y permisos — resumen rápido

| Acción                          | Administrador | Editor |
|---------------------------------|:---:|:---:|
| Gestión de usuarios             | ✅  | ❌  |
| Gestión de equipos y estadios   | ✅  | ❌  |
| Ver logs                        | ✅  | ❌  |
| Generar y consultar fixture      | ✅  | ✅  |
| Editar partidos (fecha/estadio/resultado) | ✅ | ✅ |
| Simular resultados              | ✅  | ✅  |
| Sorteo de cruces (2da fase)     | ✅  | ✅  |
| Importar CSV                    | ✅  | ✅  |

---

## Lo que el agente NO debe hacer

- Agregar lógica de negocio en componentes de Blazor (`.razor`).
- Usar `Console.WriteLine` para logging; usar el servicio de auditoría.
- Guardar contraseñas en texto plano.
- Generar código sin test previo (salvo UI).
- Usar `Random` sin semilla donde se requiera determinismo.
- Ignorar los cupos por confederación al agregar o generar equipos.
- Permitir editar partidos de la fase de grupos una vez generados los cruces de segunda fase.