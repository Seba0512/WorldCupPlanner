using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioUsuario
{
    private readonly IRepositorioUsuario _repositorio;
    private readonly ServicioAuditoria _auditoria;
    private readonly ServicioAutorizacion _autorizacion;

    public ServicioUsuario(IRepositorioUsuario repositorio, ServicioAuditoria auditoria, ServicioAutorizacion? autorizacion = null)
    {
        _repositorio = repositorio;
        _auditoria = auditoria;
        _autorizacion = autorizacion ?? new ServicioAutorizacion();
    }

    public void AgregarUsuario(Usuario usuario, string? ejecutadoPor = null, Usuario? solicitante = null)
    {
        if (solicitante != null)
            _autorizacion.VerificarEsAdministrador(solicitante);
        ValidarEmailUnico(usuario.Email, ejecutadoPor);
        AsignarRolEditorSiNoTieneRoles(usuario, ejecutadoPor);
        _repositorio.Agregar(usuario);
        _auditoria.Registrar(ejecutadoPor ?? "sistema", "ALTA_USUARIO", $"Usuario creado: {usuario.Email}");
    }

    private void AsignarRolEditorSiNoTieneRoles(Usuario usuario, string? ejecutadoPor)
    {
        if (usuario.ObtenerRoles().Any()) return;
        usuario.AgregarRol(RolUsuario.Editor);
        _auditoria.Registrar(ejecutadoPor ?? "sistema", "ASIGNACION_ROL_DEFECTO",
            $"Rol Editor asignado por defecto a: {usuario.Email}");
    }

    public void ActualizarUsuario(Usuario usuario, ActualizacionUsuario? actualizacion = null, Usuario? solicitante = null)
    {
        if (solicitante != null)
            _autorizacion.VerificarEsAdministrador(solicitante);
        actualizacion ??= new ActualizacionUsuario();
        var usuarioActual = BuscarUsuarioExistenteOLanzarExcepcion(usuario);
        var campos = AplicarActualizacionesDeCampos(usuarioActual, actualizacion);
        _repositorio.Actualizar(usuarioActual);
        _auditoria.Registrar(
            actualizacion.EjecutadoPor ?? usuarioActual.Email,
            "EDICION_USUARIO",
            ConstruirDetalleAuditoria(usuarioActual.Email, campos));
    }

    private Usuario BuscarUsuarioExistenteOLanzarExcepcion(Usuario usuario)
    {
        var usuarioActual = _repositorio.Buscar(u => ReferenceEquals(u, usuario)).FirstOrDefault();
        if (usuarioActual == null)
            throw new EntidadInvalidaExcepcion("El usuario no existe en el sistema.");
        return usuarioActual;
    }

    private List<string> AplicarActualizacionesDeCampos(Usuario usuarioActual, ActualizacionUsuario actualizacion)
    {
        var campos = new List<string>();

        if (actualizacion.Email != null)
        {
            var emailNuevo = actualizacion.Email.Trim();
            if (!emailNuevo.Equals(usuarioActual.Email, StringComparison.OrdinalIgnoreCase))
            {
                ValidarEmailUnico(emailNuevo, actualizacion.EjecutadoPor);
                usuarioActual.ActualizarEmail(emailNuevo);
                campos.Add($"email: {emailNuevo}");
            }
        }

        if (actualizacion.Nombre != null)
        {
            usuarioActual.ActualizarNombre(actualizacion.Nombre);
            campos.Add($"nombre: {actualizacion.Nombre}");
        }

        if (actualizacion.Apellido != null)
        {
            usuarioActual.ActualizarApellido(actualizacion.Apellido);
            campos.Add($"apellido: {actualizacion.Apellido}");
        }

        if (actualizacion.FechaNacimiento != null)
        {
            usuarioActual.ActualizarFechaNacimiento(actualizacion.FechaNacimiento);
            campos.Add($"fechaNacimiento: {actualizacion.FechaNacimiento}");
        }

        return campos;
    }

    private static string ConstruirDetalleAuditoria(string email, List<string> campos)
    {
        return campos.Count > 0
            ? $"Campos modificados en {email}: {string.Join(", ", campos)}"
            : $"Sin cambios efectivos en {email}";
    }

    public void EliminarUsuario(Usuario usuario, string? ejecutadoPor = null, Usuario? solicitante = null)
    {
        if (solicitante != null)
            _autorizacion.VerificarEsAdministrador(solicitante);
        ValidarNoEsUltimoAdministrador(usuario, ejecutadoPor);
        _repositorio.Eliminar(usuario);
        _auditoria.Registrar(ejecutadoPor ?? "sistema", "BAJA_USUARIO", $"Usuario eliminado: {usuario.Email}");
    }

    private void ValidarNoEsUltimoAdministrador(Usuario usuario, string? ejecutadoPor = null)
    {
        if (!usuario.TieneRol(RolUsuario.AdministradorDelSistema)) return;
        var hayOtroAdmin = _repositorio.Buscar(u =>
            !ReferenceEquals(u, usuario) &&
            u.TieneRol(RolUsuario.AdministradorDelSistema)).Any();
        if (!hayOtroAdmin)
        {
            _auditoria.Registrar(ejecutadoPor ?? "sistema", "INTENTO_ELIMINAR_ULTIMO_ADMIN",
                $"Intento de eliminar al último administrador: {usuario.Email}");
            throw new InvalidOperationException("No es posible eliminar el último administrador del sistema.");
        }
    }

    public List<Usuario> ObtenerTodosLosUsuarios(Usuario? solicitante = null)
    {
        if (solicitante != null)
            _autorizacion.VerificarEsAdministrador(solicitante);
        return _repositorio.Buscar(_ => true);
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        return _repositorio.ObtenerPorEmail(email);
    }

    public void GestionarRoles(Usuario usuario, IEnumerable<RolUsuario> nuevosRoles, string? ejecutadoPor = null)
    {
        var rolesDeseados = nuevosRoles.ToList();
        if (!rolesDeseados.Any())
            throw new InvalidOperationException("El usuario debe tener al menos un rol.");

        ValidarQuedaAlMenosUnAdministrador(usuario, rolesDeseados, ejecutadoPor);

        var rolesActuales = usuario.ObtenerRoles().ToList();
        foreach (var rol in rolesDeseados.Where(r => !rolesActuales.Contains(r)))
            usuario.AgregarRol(rol);

        foreach (var rol in rolesActuales.Where(r => !rolesDeseados.Contains(r)))
            usuario.RemoverRol(rol);

        _repositorio.Actualizar(usuario);
        _auditoria.Registrar(ejecutadoPor ?? usuario.Email, "CAMBIO_ROLES",
            $"Roles actualizados para {usuario.Email}: {string.Join(", ", rolesDeseados)}");
    }

    private void ValidarQuedaAlMenosUnAdministrador(Usuario usuarioAfectado, List<RolUsuario> rolesNuevos, string? ejecutadoPor = null)
    {
        if (!usuarioAfectado.TieneRol(RolUsuario.AdministradorDelSistema)) return;
        if (rolesNuevos.Contains(RolUsuario.AdministradorDelSistema)) return;
        var hayOtroAdmin = _repositorio.Buscar(u =>
            !ReferenceEquals(u, usuarioAfectado) &&
            u.TieneRol(RolUsuario.AdministradorDelSistema)).Any();
        if (!hayOtroAdmin)
        {
            _auditoria.Registrar(ejecutadoPor ?? usuarioAfectado.Email, "INTENTO_QUITAR_ROL_ADMIN_ULTIMO",
                $"Intento de quitar rol Administrador al último administrador: {usuarioAfectado.Email}");
            throw new InvalidOperationException("Debe existir al menos un usuario con rol Administrador.");
        }
    }

    private void ValidarEmailUnico(string email, string? ejecutadoPor = null)
    {
        var emailNormalizado = email.Trim();
        var usuarioExistente = _repositorio.ObtenerPorEmail(emailNormalizado);
        if (usuarioExistente != null)
        {
            _auditoria.Registrar(ejecutadoPor ?? emailNormalizado, "EMAIL_DUPLICADO_INTENTO",
                $"Intento de uso de email ya registrado: {emailNormalizado}");
            throw new EntidadYaExistenteExcepcion("El correo electrónico ingresado ya está en uso.");
        }
    }

    private const int AnioBaseMundial = 2026;

    public void GenerarContraseniaDefecto(Usuario usuario, int seed = 0)
    {
        var contraseniaDefecto = $"TempPass@{AnioBaseMundial + seed}";
        usuario.EstablecerContrasenia(contraseniaDefecto);
    }

    public void ReiniciarContrasenia(Usuario usuario, Usuario administrador)
    {
        if (!administrador.TieneRol(RolUsuario.AdministradorDelSistema))
        {
            throw new InvalidOperationException("Solo un administrador puede reiniciar contraseñas.");
        }

        GenerarContraseniaDefecto(usuario);
        _auditoria.Registrar(administrador.Email, "REINICIO_CONTRASENIA", $"Contraseña reiniciada para: {usuario.Email}");
    }

    public void CambiarContrasenia(Usuario usuario, string contraseniaAnterior, string nuevaContrasenia)
    {
        if (!usuario.VerificarContrasenia(contraseniaAnterior))
        {
            throw new InvalidOperationException("La contraseña actual es incorrecta.");
        }

        usuario.EstablecerContrasenia(nuevaContrasenia);
        _auditoria.Registrar(usuario.Email, "CAMBIO_CONTRASENIA", $"Contraseña cambiada para: {usuario.Email}");
    }

    public Usuario Login(string email, string contrasenia)
    {
        var usuario = _repositorio.ObtenerPorEmail(email);

        if (usuario == null || !usuario.VerificarContrasenia(contrasenia))
        {
            _auditoria.Registrar(email, "LOGIN_FALLIDO", $"Intento fallido para: {email}");
            throw new InvalidOperationException("Email o contraseña incorrectos.");
        }

        _auditoria.Registrar(usuario.Email, "LOGIN_EXITOSO", $"Login exitoso: {email}");
        return usuario;
    }

    public void Agregar(Usuario entidad) => AgregarUsuario(entidad);

    public void Actualizar(Usuario entidad)
    {
        if (entidad == null)
            throw new EntidadInvalidaExcepcion("El usuario no puede ser nulo.");
        var existente = _repositorio.Buscar(u => ReferenceEquals(u, entidad)).FirstOrDefault();
        if (existente == null)
            throw new EntidadInexistenteExcepcion("El usuario no existe en el sistema.");
        _repositorio.Actualizar(existente);
    }

    public void Eliminar(Usuario entidad) => EliminarUsuario(entidad);

    public List<Usuario> Buscar(Func<Usuario, bool> predicado) => _repositorio.Buscar(predicado);
}

