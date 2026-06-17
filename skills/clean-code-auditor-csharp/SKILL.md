# Clean Code Auditor — C#

Skill para auditar proyectos C# y detectar violaciones a principios de Clean Code. Analiza archivos `.cs` uno por uno o en conjunto y genera un reporte estructurado con los problemas encontrados, su ubicación exacta (archivo + línea) y cómo corregirlos.

---

## Cuándo usar esta skill

- El usuario pide revisar calidad de código en un proyecto C#
- Se detectan archivos `.cs` subidos o rutas a proyectos
- El usuario menciona: "clean code", "code review", "refactor", "deuda técnica", "convenciones"
- Se solicita detectar: números mágicos, métodos largos, parámetros excesivos, excepciones silenciadas

---

## Paso 1 — Localizar los archivos a analizar

```bash
# Si hay un path de proyecto:
find /ruta/al/proyecto -name "*.cs" \
  ! -path "*/obj/*" \
  ! -path "*/bin/*" \
  ! -path "*/Migrations/*" \
  ! -name "*.Designer.cs" \
  ! -name "*.g.cs" \
  | sort
```

Si el usuario subió archivos, están en `/mnt/user-data/uploads/`. Listarlos con:

```bash
ls /mnt/user-data/uploads/*.cs 2>/dev/null || find /mnt/user-data/uploads -name "*.cs"
```

---

## Paso 2 — Ejecutar las verificaciones automáticas

Guardar los scripts de la sección [Scripts de verificación](#scripts-de-verificación) como archivos `.sh` y ejecutarlos sobre cada archivo encontrado.

```bash
# Para un archivo individual:
bash check_magic_numbers.sh archivo.cs
bash check_long_methods.sh archivo.cs
bash check_excessive_params.sh archivo.cs
bash check_silent_exceptions.sh archivo.cs
bash check_naming_conventions.sh archivo.cs
bash check_misc_patterns.sh archivo.cs
```

Para analizar múltiples archivos:

```bash
for f in $(find . -name "*.cs" ! -path "*/obj/*" ! -path "*/bin/*"); do
  echo "=== $f ==="
  bash check_magic_numbers.sh "$f"
  bash check_long_methods.sh "$f"
  bash check_excessive_params.sh "$f"
  bash check_silent_exceptions.sh "$f"
  bash check_naming_conventions.sh "$f"
  bash check_misc_patterns.sh "$f"
done
```

---

## Paso 3 — Análisis semántico complementario

Los scripts detectan patrones mecánicos. Después de correrlos, **leer los archivos** para detectar problemas que requieren comprensión:

- Responsabilidades múltiples en una clase (violación SRP)
- Nombres que no expresan intención (`data`, `info`, `temp`, `obj`, `result` sin contexto)
- Comentarios que explican *qué* hace el código en vez de *por qué*
- Código comentado que debería eliminarse
- Lógica duplicada entre métodos
- Retornos `null` donde se podría usar el patrón Null Object o `Optional`

Ver la sección [Reglas de Clean Code](#reglas-de-clean-code) para la lista completa con ejemplos.

---

## Paso 4 — Generar el reporte en Markdown

Escribir el reporte como archivo `.md` en `/mnt/user-data/outputs/reporte-clean-code.md` y llamar a `present_files` para que el usuario pueda descargarlo.

Estructura del reporte:

```markdown
# Reporte de Clean Code — [Nombre del Proyecto]

**Fecha:** [fecha]  
**Archivos analizados:** N  
**Total de violaciones:** N  

---

## Resumen Ejecutivo

| Categoría                | 🔴 Críticas | 🟡 Moderadas | 🟢 Sugerencias |
|--------------------------|:-----------:|:------------:|:--------------:|
| Números mágicos          | N           | —            | —              |
| Métodos largos           | N           | —            | —              |
| Parámetros excesivos     | N           | —            | —              |
| Excepciones silenciadas  | N           | —            | —              |
| Convenciones de nombres  | N           | N            | —              |
| Otros patrones           | —           | N            | N              |
| **Total**                | **N**       | **N**        | **N**          |

---

## Violaciones Críticas 🔴

> Deben corregirse antes de mergear.

### `Ruta/Al/Archivo.cs`

| Línea | Problema | Corrección sugerida |
|-------|----------|---------------------|
| 42 | Número mágico `3000` en `Thread.Sleep` | Extraer como `const int ApiRetryDelayMs = 3000;` |
| 87 | Catch vacío — excepción silenciada | Agregar `throw;` o manejar el error apropiadamente |

---

## Violaciones Moderadas 🟡

> Importantes pero no bloquean el merge. Resolver antes del próximo sprint.

### `Ruta/Al/Archivo.cs`

| Línea | Problema | Corrección sugerida |
|-------|----------|---------------------|
| 15 | Campo privado `orderRepository` sin prefijo `_` | Renombrar a `_orderRepository` |
| 103 | TODO sin número de ticket | Agregar referencia: `TODO(#1234): ...` |

---

## Sugerencias de Mejora 🟢

- **`Archivo.cs` Línea N:** Comentario describe qué hace el código en vez de por qué → eliminar o explicar la razón de negocio.

---

## Detalle por Archivo

### `Ruta/Al/Archivo.cs`

**Línea N** 🔴 `[Categoría]` — Descripción del problema.

```csharp
// ❌ Código actual
Thread.Sleep(3000);

// ✅ Corrección sugerida
private const int ApiRetryDelayMs = 3000;
Thread.Sleep(ApiRetryDelayMs);
```
```

---

## Umbrales por defecto (configurables)

| Métrica                     | Umbral       | Severidad |
|-----------------------------|:------------:|:---------:|
| Líneas por método           | > 20         | 🔴        |
| Parámetros por método       | > 3          | 🔴        |
| Líneas por clase            | > 300        | 🟡        |
| Nivel de anidación          | > 3          | 🟡        |
| Números mágicos             | cualquiera   | 🔴        |
| Excepciones silenciadas     | cualquiera   | 🔴        |
| Comentarios TODO sin ticket | cualquiera   | 🟡        |

Si el usuario indica umbrales distintos, usarlos en lugar de estos.

---

## Scripts de Verificación

### `check_magic_numbers.sh`

Detecta literales numéricos embebidos en lógica de negocio sin una constante con nombre.

```bash
#!/usr/bin/env bash
# check_magic_numbers.sh — Detecta números mágicos en archivos C#
# Uso: bash check_magic_numbers.sh <archivo.cs>

FILE="$1"

if [[ -z "$FILE" || ! -f "$FILE" ]]; then
  echo "Uso: $0 <archivo.cs>"
  exit 1
fi

FOUND=0

while IFS= read -r line; do
  LINE_NUM="${line%%:*}"
  LINE_CONTENT="${line#*:}"

  trimmed=$(echo "$LINE_CONTENT" | sed 's/^[[:space:]]*//')

  # Saltar comentarios
  if [[ "$trimmed" == //* || "$trimmed" == "/*"* || "$trimmed" == "*"* ]]; then
    continue
  fi

  # Saltar declaraciones de constantes
  if echo "$LINE_CONTENT" | grep -qE '\b(const|readonly)\b'; then
    continue
  fi

  # Saltar atributos de ASP.NET
  if echo "$LINE_CONTENT" | grep -qE '^\s*\['; then
    continue
  fi

  # Buscar números mágicos (excluye 0, 1, -1)
  if echo "$LINE_CONTENT" | grep -qE '\b([2-9][0-9]*|[0-9]{2,})\b|[0-9]+\.[0-9]+[mMfFdD]?'; then
    if echo "$LINE_CONTENT" | grep -qvE '(version|Version|StatusCode|Http[0-9])'; then
      echo "  🔴 Línea $LINE_NUM: Posible número mágico → $LINE_CONTENT"
      FOUND=$((FOUND + 1))
    fi
  fi

done < <(grep -n "" "$FILE")

if [[ $FOUND -eq 0 ]]; then
  echo "  ✅ Sin números mágicos detectados en: $FILE"
else
  echo "  ⚠️  $FOUND instancia(s) encontrada(s) en: $FILE"
fi

exit 0
```

---

### `check_long_methods.sh`

Detecta métodos que superan el umbral de líneas. Cuenta solo líneas con código real (ignora blancos, comentarios y llaves solas).

```bash
#!/usr/bin/env bash
# check_long_methods.sh — Detecta métodos largos en C#
# Uso: bash check_long_methods.sh <archivo.cs> [--max-lines 20]

FILE="$1"
MAX_LINES="${3:-20}"

if [[ -z "$FILE" || ! -f "$FILE" ]]; then
  echo "Uso: $0 <archivo.cs> [--max-lines N]"
  exit 1
fi

if [[ "$2" == "--max-lines" && -n "$3" ]]; then
  MAX_LINES="$3"
fi

FOUND=0
in_method=false
method_name=""
method_start=0
brace_depth=0
code_line_count=0

while IFS= read -r raw; do
  LINE_NUM="${raw%%:*}"
  LINE="${raw#*:}"
  trimmed=$(echo "$LINE" | sed 's/^[[:space:]]*//')

  if [[ "$in_method" == false ]]; then
    if echo "$trimmed" | grep -qE '^(public|private|protected|internal|static|async|override|virtual|sealed|abstract)\s'; then
      if echo "$trimmed" | grep -qE '\w+\s*\('; then
        if echo "$trimmed" | grep -qvE '^\s*(class|interface|struct|enum|record)\s'; then
          if echo "$trimmed" | grep -qvE '\{[^}]*\}\s*$'; then
            if echo "$trimmed" | grep -qvE '=>'; then
              method_name=$(echo "$trimmed" | grep -oE '\w+\s*\(' | head -1 | tr -d '(')
              method_start="$LINE_NUM"
              code_line_count=0
              in_method=true
              brace_depth=0
            fi
          fi
        fi
      fi
    fi
  fi

  if [[ "$in_method" == true ]]; then
    opens=$(echo "$LINE" | tr -cd '{' | wc -c)
    closes=$(echo "$LINE" | tr -cd '}' | wc -c)
    brace_depth=$((brace_depth + opens - closes))

    if [[ -n "$trimmed" && "$trimmed" != "{" && "$trimmed" != "}" && \
          "$trimmed" != "//"* && "$trimmed" != "/*"* && "$trimmed" != "*"* ]]; then
      code_line_count=$((code_line_count + 1))
    fi

    if [[ $brace_depth -le 0 && $code_line_count -gt 0 ]]; then
      if [[ $code_line_count -gt $MAX_LINES ]]; then
        echo "  🔴 Línea $method_start: Método '$method_name' tiene $code_line_count líneas (máx. $MAX_LINES)"
        FOUND=$((FOUND + 1))
      fi
      in_method=false
      code_line_count=0
      brace_depth=0
    fi
  fi

done < <(grep -n "" "$FILE")

if [[ $FOUND -eq 0 ]]; then
  echo "  ✅ Sin métodos largos en: $FILE (umbral: $MAX_LINES líneas)"
else
  echo "  ⚠️  $FOUND método(s) largo(s) en: $FILE"
fi

exit 0
```

---

### `check_excessive_params.sh`

Detecta métodos con más de 3 parámetros. Soporta firmas multi-línea.

```bash
#!/usr/bin/env bash
# check_excessive_params.sh — Detecta parámetros excesivos en C#
# Uso: bash check_excessive_params.sh <archivo.cs> [--max-params 3]

FILE="$1"
MAX_PARAMS="${3:-3}"

if [[ -z "$FILE" || ! -f "$FILE" ]]; then
  echo "Uso: $0 <archivo.cs> [--max-params N]"
  exit 1
fi

if [[ "$2" == "--max-params" && -n "$3" ]]; then
  MAX_PARAMS="$3"
fi

FOUND=0
in_signature=false
signature_buffer=""
signature_start=0

count_params() {
  local params="$1"
  local lineno="$2"
  local full_sig="$3"

  inner=$(echo "$params" | sed 's/^(//;s/)$//')

  if [[ -z "$inner" || "$inner" =~ ^this ]]; then
    return
  fi

  sanitized=$(echo "$inner" | sed 's/<[^>]*>//g')
  comma_count=$(echo "$sanitized" | tr -cd ',' | wc -c)
  param_count=$((comma_count + 1))

  if [[ $param_count -gt $MAX_PARAMS ]]; then
    method_name=$(echo "$full_sig" | grep -oE '\w+\s*\(' | head -1 | tr -d '(')
    echo "  🔴 Línea $lineno: Método '$method_name' tiene $param_count parámetros (máx. $MAX_PARAMS) → usar objeto de parámetros"
    FOUND=$((FOUND + 1))
  fi
}

while IFS= read -r raw; do
  LINE_NUM="${raw%%:*}"
  LINE="${raw#*:}"
  trimmed=$(echo "$LINE" | sed 's/^[[:space:]]*//')

  if [[ "$in_signature" == false ]]; then
    if echo "$trimmed" | grep -qE '^(public|private|protected|internal|static|async|override|virtual|abstract|sealed)\s'; then
      if echo "$trimmed" | grep -qE '\w+\s*\('; then
        if echo "$trimmed" | grep -qvE '^\s*(class|interface|struct|enum|record)\s'; then
          signature_buffer="$trimmed"
          signature_start="$LINE_NUM"

          if echo "$trimmed" | grep -qE '\)'; then
            params=$(echo "$trimmed" | grep -oE '\([^)]*\)' | head -1)
            count_params "$params" "$signature_start" "$trimmed"
          else
            in_signature=true
          fi
        fi
      fi
    fi
  else
    signature_buffer="$signature_buffer $trimmed"
    if echo "$trimmed" | grep -qE '\)'; then
      in_signature=false
      params=$(echo "$signature_buffer" | grep -oE '\([^)]*\)' | head -1)
      count_params "$params" "$signature_start" "$signature_buffer"
    fi
  fi

done < <(grep -n "" "$FILE")

if [[ $FOUND -eq 0 ]]; then
  echo "  ✅ Sin parámetros excesivos en: $FILE (umbral: $MAX_PARAMS)"
else
  echo "  ⚠️  $FOUND método(s) con demasiados parámetros en: $FILE"
fi

exit 0
```

---

### `check_silent_exceptions.sh`

Detecta catch vacíos, catch que solo loguean sin re-lanzar, y captura genérica de `Exception`.

```bash
#!/usr/bin/env bash
# check_silent_exceptions.sh — Detecta excepciones silenciadas en C#
# Uso: bash check_silent_exceptions.sh <archivo.cs>

FILE="$1"

if [[ -z "$FILE" || ! -f "$FILE" ]]; then
  echo "Uso: $0 <archivo.cs>"
  exit 1
fi

FOUND=0
in_catch=false
catch_start=0
catch_brace_depth=0
catch_lines=()
catch_has_throw=false
catch_has_return=false
catch_has_content=false
catch_type=""

while IFS= read -r raw; do
  LINE_NUM="${raw%%:*}"
  LINE="${raw#*:}"
  trimmed=$(echo "$LINE" | sed 's/^[[:space:]]*//')

  if [[ "$in_catch" == false ]]; then
    if echo "$trimmed" | grep -qE '^\s*catch\s*(\(.*\))?\s*\{?\s*$'; then
      in_catch=true
      catch_start="$LINE_NUM"
      catch_brace_depth=0
      catch_lines=()
      catch_has_throw=false
      catch_has_return=false
      catch_has_content=false
      catch_type=$(echo "$trimmed" | grep -oE 'catch\s*\([^)]*\)' | head -1)

      opens=$(echo "$LINE" | tr -cd '{' | wc -c)
      closes=$(echo "$LINE" | tr -cd '}' | wc -c)
      catch_brace_depth=$((catch_brace_depth + opens - closes))
    fi
  else
    opens=$(echo "$LINE" | tr -cd '{' | wc -c)
    closes=$(echo "$LINE" | tr -cd '}' | wc -c)
    catch_brace_depth=$((catch_brace_depth + opens - closes))

    if [[ -n "$trimmed" && "$trimmed" != "{" && "$trimmed" != "}" ]]; then
      catch_has_content=true
      catch_lines+=("$LINE_NUM: $trimmed")

      if echo "$trimmed" | grep -qE '^\s*throw\b'; then
        catch_has_throw=true
      fi
      if echo "$trimmed" | grep -qE '^\s*return\b'; then
        catch_has_return=true
      fi
    fi

    if [[ $catch_brace_depth -le 0 ]]; then
      in_catch=false

      if [[ "$catch_has_content" == false ]]; then
        echo "  🔴 Línea $catch_start: Catch vacío — la excepción es ignorada completamente"
        FOUND=$((FOUND + 1))

      elif [[ "$catch_has_throw" == false && "$catch_has_return" == false ]]; then
        only_logs=true
        for catch_line in "${catch_lines[@]}"; do
          content="${catch_line#*: }"
          if echo "$content" | grep -qvE '(_logger|log|Log|logger|Logger|Console\.|Debug\.|Trace\.|//|/\*)'; then
            only_logs=false
            break
          fi
        done

        if [[ "$only_logs" == true ]]; then
          echo "  🔴 Línea $catch_start: Catch solo loguea sin re-lanzar (excepción efectivamente silenciada)"
          echo "       Tipo capturado: ${catch_type:-'catch all'}"
          echo "       → Agregar 'throw;' o retornar un error apropiado"
          FOUND=$((FOUND + 1))
        fi

        if echo "$catch_type" | grep -qE 'catch\s*\(\s*Exception\s+\w*\s*\)'; then
          echo "  🟡 Línea $catch_start: Captura genérica 'Exception' sin re-throw — preferir excepciones específicas"
          FOUND=$((FOUND + 1))
        fi
      fi
    fi
  fi

done < <(grep -n "" "$FILE")

if [[ $FOUND -eq 0 ]]; then
  echo "  ✅ Sin excepciones silenciadas en: $FILE"
else
  echo "  ⚠️  $FOUND instancia(s) de manejo incorrecto de excepciones en: $FILE"
fi

exit 0
```

---

### `check_naming_conventions.sh`

Verifica convenciones de nombres de C#: PascalCase, camelCase, prefijo `_`, prefijo `I`, sufijo `Async`.

```bash
#!/usr/bin/env bash
# check_naming_conventions.sh — Verifica convenciones de nombres de C#
# Uso: bash check_naming_conventions.sh <archivo.cs>

FILE="$1"

if [[ -z "$FILE" || ! -f "$FILE" ]]; then
  echo "Uso: $0 <archivo.cs>"
  exit 1
fi

FOUND=0

while IFS= read -r raw; do
  LINE_NUM="${raw%%:*}"
  LINE="${raw#*:}"
  trimmed=$(echo "$LINE" | sed 's/^[[:space:]]*//')

  if [[ "$trimmed" == //* || "$trimmed" == "/*"* || "$trimmed" == "*"* || -z "$trimmed" ]]; then
    continue
  fi

  # Interfaces sin prefijo I
  if echo "$trimmed" | grep -qE '^\s*interface\s+[^I][A-Za-z]'; then
    name=$(echo "$trimmed" | grep -oE 'interface\s+\w+' | awk '{print $2}')
    echo "  🔴 Línea $LINE_NUM: Interfaz '$name' sin prefijo 'I' → 'I${name}'"
    FOUND=$((FOUND + 1))
  fi

  # Clases/Records/Structs no PascalCase
  if echo "$trimmed" | grep -qE '^\s*(public|internal|private|protected)?\s*(abstract|sealed|static)?\s*(class|record|struct)\s+[a-z]'; then
    name=$(echo "$trimmed" | grep -oE '(class|record|struct)\s+\w+' | awk '{print $2}')
    echo "  🔴 Línea $LINE_NUM: Tipo '$name' no está en PascalCase"
    FOUND=$((FOUND + 1))
  fi

  # Constantes en SCREAMING_SNAKE_CASE
  if echo "$trimmed" | grep -qE '\bconst\b.*\b[A-Z][A-Z_]+[A-Z]\b'; then
    echo "  🔴 Línea $LINE_NUM: Constante en SCREAMING_SNAKE_CASE → usar PascalCase"
    echo "       $trimmed"
    FOUND=$((FOUND + 1))
  fi

  # Campos privados sin prefijo _
  if echo "$trimmed" | grep -qE '^\s*private\s+(readonly\s+)?\w+[\w<>, ]+\s+[a-z][a-zA-Z]*\s*;'; then
    if echo "$trimmed" | grep -qvE '\bconst\b'; then
      field=$(echo "$trimmed" | grep -oE '[a-z][a-zA-Z0-9]*\s*;' | head -1 | tr -d '; ')
      echo "  🟡 Línea $LINE_NUM: Campo privado '$field' sin prefijo '_' → '_${field}'"
      FOUND=$((FOUND + 1))
    fi
  fi

  # Métodos públicos no PascalCase
  if echo "$trimmed" | grep -qE '^\s*(public|protected)\s+(static\s+|async\s+|override\s+|virtual\s+)*(void|Task|Task<|IEnumerable|IQueryable|string|int|bool|List|Dictionary|IAction)\w*\s+[a-z][a-z]'; then
    method=$(echo "$trimmed" | grep -oE '\s+[a-z]\w+\s*\(' | head -1 | tr -d '( ')
    if [[ -n "$method" ]]; then
      echo "  🔴 Línea $LINE_NUM: Método público '$method' no está en PascalCase"
      FOUND=$((FOUND + 1))
    fi
  fi

  # Métodos async sin sufijo Async
  if echo "$trimmed" | grep -qE '\basync\b' && echo "$trimmed" | grep -qvE 'Async\s*\('; then
    if echo "$trimmed" | grep -qE '(public|private|protected|internal)\s+async'; then
      method=$(echo "$trimmed" | grep -oE '\w+\s*(<[^>]*>)?\s*\(' | head -1 | tr -d '( ')
      echo "  🟡 Línea $LINE_NUM: Método async '$method' debería terminar en 'Async'"
      FOUND=$((FOUND + 1))
    fi
  fi

  # Variables con nombres genéricos sin contexto
  if echo "$trimmed" | grep -qE '\b(var|string|int|bool|object)\s+(data|info|temp|obj|result|value|item|thing|stuff)\s*[=;]'; then
    varname=$(echo "$trimmed" | grep -oE '\b(data|info|temp|obj|result|value|item|thing|stuff)\b' | head -1)
    echo "  🟡 Línea $LINE_NUM: Variable '$varname' con nombre genérico → usar nombre descriptivo"
    FOUND=$((FOUND + 1))
  fi

done < <(grep -n "" "$FILE")

if [[ $FOUND -eq 0 ]]; then
  echo "  ✅ Convenciones de nombres correctas en: $FILE"
else
  echo "  ⚠️  $FOUND violación(es) de nombres en: $FILE"
fi

exit 0
```

---

### `check_misc_patterns.sh`

Detecta código comentado, TODOs sin ticket, anidación excesiva, `IDisposable` sin `using`, variables de una letra y clases demasiado grandes.

```bash
#!/usr/bin/env bash
# check_misc_patterns.sh — Detecta patrones varios de deuda técnica en C#
# Uso: bash check_misc_patterns.sh <archivo.cs>

FILE="$1"
MAX_NESTING="${3:-3}"
MAX_CLASS_LINES="${5:-300}"

if [[ -z "$FILE" || ! -f "$FILE" ]]; then
  echo "Uso: $0 <archivo.cs>"
  exit 1
fi

FOUND=0
total_lines=$(wc -l < "$FILE")
current_indent_level=0

while IFS= read -r raw; do
  LINE_NUM="${raw%%:*}"
  LINE="${raw#*:}"
  trimmed=$(echo "$LINE" | sed 's/^[[:space:]]*//')

  # Código comentado
  if echo "$trimmed" | grep -qE '^\s*//\s*(if|for|foreach|while|return|var |int |string |bool |public|private|protected|\w+\.\w+\()'; then
    echo "  🟡 Línea $LINE_NUM: Código comentado detectado → eliminar o restaurar"
    echo "       $trimmed"
    FOUND=$((FOUND + 1))
  fi

  # TODOs sin ticket
  if echo "$trimmed" | grep -qiE '//\s*(TODO|FIXME|HACK|XXX|BUG)\s*[^(#]'; then
    tag=$(echo "$trimmed" | grep -oiE '(TODO|FIXME|HACK|XXX|BUG)' | head -1)
    echo "  🟡 Línea $LINE_NUM: $tag sin número de ticket → ej: $tag(#1234): ..."
    FOUND=$((FOUND + 1))
  fi

  # Anidación excesiva
  opens=$(echo "$LINE" | tr -cd '{' | wc -c)
  closes=$(echo "$LINE" | tr -cd '}' | wc -c)
  current_indent_level=$((current_indent_level + opens - closes))
  if [[ $current_indent_level -gt $MAX_NESTING ]]; then
    if echo "$trimmed" | grep -qE '^\s*(if|else|for|foreach|while|switch|catch|using\s*\()'; then
      echo "  🟡 Línea $LINE_NUM: Anidación nivel $current_indent_level (máx. $MAX_NESTING) → extraer a método o usar early return"
      FOUND=$((FOUND + 1))
    fi
  fi
  if [[ $current_indent_level -lt 0 ]]; then current_indent_level=0; fi

  # IDisposable sin using
  if echo "$trimmed" | grep -qE '\bnew\s+(SqlConnection|SqlCommand|FileStream|StreamReader|StreamWriter|HttpClient|MemoryStream|BinaryReader|BinaryWriter|SqlDataAdapter|OleDbConnection)\s*\('; then
    if echo "$trimmed" | grep -qvE '^\s*using\s+(var\s+|[A-Za-z])'; then
      type=$(echo "$trimmed" | grep -oE '(SqlConnection|SqlCommand|FileStream|StreamReader|StreamWriter|HttpClient|MemoryStream|BinaryReader|BinaryWriter|SqlDataAdapter|OleDbConnection)' | head -1)
      echo "  🔴 Línea $LINE_NUM: '$type' es IDisposable — usar 'using var' para liberar recursos"
      FOUND=$((FOUND + 1))
    fi
  fi

  # Variables de una sola letra fuera de loops
  if echo "$trimmed" | grep -qE '\b(var|int|string|bool|double|decimal|float|long)\s+[a-z]\s*='; then
    varname=$(echo "$trimmed" | grep -oE '\b[a-z]\b' | head -1)
    context=$(sed -n "$((LINE_NUM > 5 ? LINE_NUM - 5 : 1)),${LINE_NUM}p" "$FILE" | grep -cE '\b(for|foreach|while)\b')
    if [[ $context -eq 0 ]]; then
      echo "  🟡 Línea $LINE_NUM: Variable de una letra '$varname' fuera de loop → usar nombre descriptivo"
      FOUND=$((FOUND + 1))
    fi
  fi

  # Comentarios obvios
  if echo "$trimmed" | grep -qE '^\s*//\s*(Increment|Get|Set|Return|Create|Delete|Update|Check|Validate|Calculate|Process)\s+\w+'; then
    echo "  🟢 Línea $LINE_NUM: Comentario describe el qué, no el por qué → eliminar o explicar razón de negocio"
    FOUND=$((FOUND + 1))
  fi

done < <(grep -n "" "$FILE")

# Clases muy largas
class_count=$(grep -cE '^\s*(public|internal|private)\s+(abstract\s+|sealed\s+|static\s+)?(class|record|struct)\s+' "$FILE" 2>/dev/null || echo 0)
if [[ $total_lines -gt $MAX_CLASS_LINES && $class_count -le 1 ]]; then
  echo "  🟡 Archivo con $total_lines líneas y $class_count clase(s) → posible violación de SRP (máx. ~$MAX_CLASS_LINES líneas)"
  FOUND=$((FOUND + 1))
fi

if [[ $FOUND -eq 0 ]]; then
  echo "  ✅ Sin patrones problemáticos en: $FILE"
else
  echo "  ⚠️  $FOUND patrón(es) encontrado(s) en: $FILE"
fi

exit 0
```

---

## Reglas de Clean Code

### 1. Números Mágicos 🔴

Literales numéricos embebidos directamente en el código sin una constante con nombre.

```csharp
// ❌ Incorrecto
if (user.Age >= 18)
    ApplyDiscount(price * 0.15m);
Thread.Sleep(3000);

// ✅ Correcto
private const int LegalAge = 18;
private const decimal SeniorDiscountRate = 0.15m;
private const int ApiRetryDelayMs = 3000;

if (user.Age >= LegalAge)
    ApplyDiscount(price * SeniorDiscountRate);
Thread.Sleep(ApiRetryDelayMs);
```

> **Excepciones permitidas:** `0`, `1`, `-1` en contextos aritméticos obvios. Números en pruebas unitarias con contexto claro.

---

### 2. Métodos Largos 🔴

Métodos que superan **20 líneas** de código real. Indican que el método hace demasiado.

```csharp
// ❌ Incorrecto — un método que valida, calcula, persiste y notifica
public void ProcessOrder(Order order)
{
    // 40+ líneas mezclando responsabilidades
}

// ✅ Correcto — un método que orquesta
public void ProcessOrder(Order order)
{
    ValidateOrder(order);
    var total = CalculateTotal(order);
    SaveOrder(order, total);
    NotifyCustomer(order);
}
```

---

### 3. Parámetros Excesivos 🔴

Métodos con más de **3 parámetros**. Los parámetros deben agruparse en un objeto.

```csharp
// ❌ Incorrecto
public void CreateUser(string firstName, string lastName, string email,
                        string phone, DateTime birthDate, string role, bool isActive)

// ✅ Correcto
public void CreateUser(CreateUserRequest request)

public record CreateUserRequest(
    string FirstName, string LastName, string Email,
    string Phone, DateTime BirthDate, string Role, bool IsActive
);
```

---

### 4. Excepciones Silenciadas 🔴

Bloques `catch` vacíos o que solo loguean sin re-lanzar el error.

```csharp
// ❌ Incorrecto — catch vacío
try { SendEmail(user); }
catch (Exception) { }

// ❌ Incorrecto — solo loguea, el flujo continúa como si nada
try { ProcessPayment(order); }
catch (Exception ex) { _logger.LogError(ex, "Error"); }

// ✅ Correcto — captura específica y re-lanza o maneja el error
try { SendEmail(user); }
catch (SmtpException ex)
{
    _logger.LogWarning(ex, "Email fallido para usuario {UserId}", user.Id);
    throw;
}
```

---

### 5. Convenciones de Nombres C# 🟡

| Elemento | Convención | Ejemplo |
|----------|-----------|---------|
| Clases, Interfaces, Enums, Records | PascalCase | `OrderService`, `PaymentStatus` |
| Métodos y Propiedades | PascalCase | `ProcessOrder()`, `CustomerName` |
| Variables locales y parámetros | camelCase | `orderTotal`, `firstName` |
| Campos privados | `_camelCase` | `_orderRepository` |
| Constantes | PascalCase | `MaxRetryCount` (**no** `MAX_RETRY_COUNT`) |
| Interfaces | Prefijo `I` | `IOrderRepository` |
| Métodos async | Sufijo `Async` | `GetOrderAsync()` |

```csharp
// ❌ Incorrecto
private readonly IOrderRepository orderRepository;
private const int MAX_RETRY = 3;
public async Task<Order> GetOrder(int id) { }

// ✅ Correcto
private readonly IOrderRepository _orderRepository;
private const int MaxRetry = 3;
public async Task<Order> GetOrderAsync(int id) { }
```

---

### 6. Responsabilidad Única — SRP 🟡

Una clase debe tener una sola razón para cambiar.

```csharp
// ❌ Incorrecto — una clase que hace todo
public class UserService
{
    public void RegisterUser(User user) { /* valida, guarda, envía email, genera PDF */ }
    public void SendWelcomeEmail(User user) { /* lógica SMTP */ }
    public byte[] GenerateReport(int userId) { /* genera PDF */ }
}

// ✅ Correcto — responsabilidades separadas
public class UserRegistrationService { }
public class EmailNotificationService { }
public class UserReportGenerator { }
```

---

### 7. Código Comentado 🟡

El código comentado es ruido. El historial existe en git.

```csharp
// ❌ Incorrecto
// var oldPrice = price * 1.21m;
var price = CalculatePrice(order);
// if (user.IsAdmin) price = 0;

// ✅ Correcto — solo el código activo; si hace falta contexto, explicar el por qué
var price = CalculatePrice(order);
// El descuento para administradores se gestiona en AdminPricingPolicy (ver PR #892)
```

---

### 8. Anidación Excesiva 🟡

Más de 3 niveles de anidación. Usar guard clauses y early return.

```csharp
// ❌ Incorrecto — 5 niveles de anidación
public void ProcessOrder(Order order)
{
    if (order != null)
        if (order.IsValid)
            if (order.Items.Count > 0)
                foreach (var item in order.Items)
                    if (item.IsInStock) { /* lógica — nivel 5 */ }
}

// ✅ Correcto — guard clauses + extracción de método
public void ProcessOrder(Order order)
{
    if (order is null) throw new ArgumentNullException(nameof(order));
    if (!order.IsValid) return;
    if (order.Items.Count == 0) return;

    foreach (var item in order.Items)
        ProcessItemIfAvailable(item);
}
```

---

### 9. Retorno Null 🟡

Retornar `null` propaga NullReferenceExceptions silenciosas. Preferir excepciones o nullable explícito.

```csharp
// ❌ Incorrecto
public Order GetOrder(int id) => _db.Orders.Find(id); // puede ser null sin advertencia

// ✅ Opción 1 — lanzar si no encontrar es un error
public Order GetOrder(int id) =>
    _db.Orders.Find(id) ?? throw new OrderNotFoundException(id);

// ✅ Opción 2 — nullable explícito con nombre claro
public Order? FindOrder(int id) => _db.Orders.Find(id);
```

---

### 10. Variables con Nombres Pobres 🟡

Nombres de una letra, abreviaciones crípticas, o nombres genéricos sin contexto.

```csharp
// ❌ Incorrecto
var x = GetUsers();
var temp = x.Where(u => u.IsActive);
string s = "pending";

// ✅ Correcto
var allUsers = GetUsers();
var activeUsers = allUsers.Where(u => u.IsActive);
string orderStatus = "pending";
```

> **Excepciones:** `i`, `j`, `k` en loops y lambdas cortas como `x => x.Id` son aceptables.

---

### 11. TODOs sin Seguimiento 🟡

Los comentarios `TODO` sin ticket crean deuda técnica invisible.

```csharp
// ❌ Incorrecto
// TODO: optimizar esta query
// FIXME: esto a veces falla

// ✅ Correcto
// TODO(#1234): Optimizar con índice compuesto en OrderDate + CustomerId
// FIXME(#1235): Race condition en procesamiento concurrente de pagos
```

---

### 12. Duplicación de Lógica 🟡

El mismo bloque aparece en más de un lugar. Viola el principio DRY.

```csharp
// ❌ Incorrecto — misma lógica en OrderService e InvoiceService
var taxRate = country == "UY" ? 0.22m : 0.0m;
var total = subtotal + subtotal * taxRate;

// ✅ Correcto — extraído a TaxCalculator
public decimal Calculate(decimal amount, string country) =>
    amount * GetTaxRate(country);

private decimal GetTaxRate(string country) =>
    country == "UY" ? UruguayTaxRate : 0.0m;
```

---

### 13. IDisposable sin `using` 🔴

Los objetos `IDisposable` deben liberarse con `using` para garantizar el cierre de recursos.

```csharp
// ❌ Incorrecto — si lanza excepción, la conexión nunca se cierra
var connection = new SqlConnection(connectionString);
connection.Open();
var result = new SqlCommand(sql, connection).ExecuteScalar();

// ✅ Correcto
using var connection = new SqlConnection(connectionString);
await connection.OpenAsync();
using var command = new SqlCommand(sql, connection);
var result = await command.ExecuteScalarAsync();
```

---

### 14. Uso de `var` 🟢

Usar `var` cuando el tipo es obvio por el contexto. Evitarlo cuando oscurece el tipo retornado.

```csharp
// ❌ Ambiguo
var result = GetData();
var x = ProcessOrder(order);

// ✅ Obvio por contexto
var order = new Order();
var orders = _repository.GetAllActive();
List<Order> filtered = GetFilteredOrders(filter); // explícito cuando ayuda
```
