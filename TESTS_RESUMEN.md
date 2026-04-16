# Tests Unitarios de Usuario - TDD Implementation

## Resumen

Se han implementado **31 tests unitarios** para la clase `Usuario` siguiendo Test-Driven Development (TDD) y cubriendo todos los requisitos especificados.

## Archivos Creados/Modificados

### 1. **WorldCupPlanner.Domain/Usuario.cs** (Implementación)
Clase principal con las siguientes funcionalidades:

#### Información del Usuario
- **Propiedades:**
  - `Nombre`: string (requerido, no vacío)
  - `Apellido`: string (requerido, no vacío)
  - `Email`: string (requerido, validación RFC básica)
  - `FechaNacimiento`: DateTime (no puede ser futura)
  - `ContraseñaCifrada`: string? (solo lectura, solo acceso a través del getter)

#### Gestión de Contraseña
- **Método `EstablecerContraseña(string)`**: Establece y cifra la contraseña
  - ✅ Mínimo 8 caracteres
  - ✅ Debe incluir mayúscula (A-Z)
  - ✅ Debe incluir minúscula (a-z)
  - ✅ Debe incluir número (0-9)
  - ✅ Debe incluir carácter especial (@, #, $, ., !, &, *, -, _, +, =, ?)
  - ✅ Cifrada con SHA256 y Base64 (no texto plano)

- **Método `VerificarContraseña(string)`**: Valida si una contraseña es correcta
  - Realiza hash de la contraseña ingresada
  - Compara con la contraseña cifrada almacenada
  - Sensible a mayúsculas/minúsculas

#### Gestión de Roles (No Excluyentes)
- **Método `AgregarRol(Rol)`**: Agrega un rol al usuario
  - Permite múltiples roles
  - Previene duplicados automáticamente (usando HashSet)

- **Método `RemoverRol(Rol)`**: Remueve un rol del usuario
  - No afecta otros roles

- **Método `TieneRol(Rol)`**: Verifica si el usuario tiene un rol específico
  - Retorna boolean

- **Método `ObtenerRoles()`**: Obtiene todos los roles del usuario
  - Retorna IEnumerable<Rol>

### 2. **WorldCupPlanner.Domain/Rol.cs** (Nuevo)
Enum con los roles disponibles:
```csharp
public enum Rol
{
    AdministradorDelSistema,  // Gestión de usuarios, equipos, estadios, logs
    Editor                     // Gestión de fixtures, partidos, importación
}
```

### 3. **WorldCupPlanner.Tests/UsuarioTests.cs** (Tests Unitarios)
31 tests organizados en 3 secciones:

#### Sección 1: Información del Usuario (6 tests)
- ✅ `CrearUsuario_ConDatosValidos_DebeCrearExitosamente`
- ✅ `CrearUsuario_ConNombreVacio_DebeLanzarExcepcion`
- ✅ `CrearUsuario_ConApellidoVacio_DebeLanzarExcepcion`
- ✅ `CrearUsuario_ConEmailInvalido_DebeLanzarExcepcion`
- ✅ `CrearUsuario_ConEmailVacio_DebeLanzarExcepcion`
- ✅ `CrearUsuario_ConFechaNacimientoFutura_DebeLanzarExcepcion`

#### Sección 2: Gestión de Contraseña (17 tests)
- ✅ `EstablecerContraseña_ConContraseñaValida_DebeEstablecerseExitosamente`
- ✅ `EstablecerContraseña_ConMenosDe8Caracteres_DebeLanzarExcepcion`
- ✅ `EstablecerContraseña_SinMayúsculas_DebeLanzarExcepcion`
- ✅ `EstablecerContraseña_SinMinúsculas_DebeLanzarExcepcion`
- ✅ `EstablecerContraseña_SinNúmeros_DebeLanzarExcepcion`
- ✅ `EstablecerContraseña_SinCaracteresEspeciales_DebeLanzarExcepcion`
- ✅ `EstablecerContraseña_ConContraseñaVacia_DebeLanzarExcepcion`
- ✅ `EstablecerContraseña_DebeAlmacenarCifrada_NoEnTextoPlano`
- ✅ `VerificarContraseña_ConContraseñaCorrecta_DebeRetornarVerdadero`
- ✅ `VerificarContraseña_ConContraseñaIncorrecta_DebeRetornarFalso`
- ✅ `VerificarContraseña_ConContraseñaCasoDiferente_DebeRetornarFalso`
- ✅ `EstablecerContraseña_ConCaracteresEspecialesValidos_DebeAceptarlos`

#### Sección 3: Gestión de Roles (8 tests)
- ✅ `AgregarRol_ConRolAdministrador_DebeAgregarse`
- ✅ `AgregarRol_ConRolEditor_DebeAgregarse`
- ✅ `AgregarRol_ConRolesMultiples_DebePermitirRolesNoExcluyentes`
- ✅ `AgregarRol_MismoRolDosVeces_NoDebeAgregarseDuplicado`
- ✅ `RemoverRol_ConRolExistente_DebeRemoverse`
- ✅ `RemoverRol_NoAfectaOtrosRoles`
- ✅ `ObtenerRoles_ConRolesAsignados_DebeRetornarTodos`
- ✅ `TieneRol_UsuarioSinRoles_DebeRetornarFalso`

## Resultados de Ejecución

```
Pruebas totales: 31
     Correcto: 31
     Fallidas: 0
 Tiempo total: 1.56 segundos
```

## Patrones Implementados

1. **Validación en Constructor**: Todas las entradas se validan en el constructor
2. **Cifrado de Contraseña**: SHA256 + Base64 para almacenamiento seguro
3. **Validación de Contraseña**: Expresiones regulares para cada requisito
4. **Roles No Excluyentes**: HashSet para evitar duplicados y permitir múltiples roles
5. **Encapsulación**: Propiedades privadas con getters públicos
6. **Excepciones Específicas**: ArgumentException con mensajes descriptivos

## Requisitos Completados

✅ Información del Usuario: Nombre, apellido, email, fecha de nacimiento
✅ Roles no excluyentes: AdministradorDelSistema y Editor
✅ Contraseña con longitud mínima de 8 caracteres
✅ Contraseña cifrada (SHA256), no en texto plano
✅ Validación de mayúsculas, minúsculas, números y caracteres especiales
✅ Tests unitarios con patrón AAA (Arrange, Act, Assert)
✅ Cobertura completa de casos positivos y negativos

