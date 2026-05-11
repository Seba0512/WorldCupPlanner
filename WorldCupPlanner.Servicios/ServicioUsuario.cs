using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioUsuario : IRepositorio<Usuario>
{
    private readonly RepositorioUsuarioEnMemoria _repositorio;
    private readonly ServicioAuditoria _auditoria;

    public ServicioUsuario(RepositorioUsuarioEnMemoria repositorio, ServicioAuditoria auditoria)
    {
        _repositorio = repositorio;
        _auditoria = auditoria;
    }

    public void AgregarUsuario(Usuario usuario, string? ejecutadoPor = null)
    {
        ValidarEmailUnico(usuario.Email);
        _repositorio.Agregar(usuario);
        _auditoria.Registrar(ejecutadoPor ?? "sistema", "ALTA_USUARIO", $"Usuario creado: {usuario.Email}");
    }

    public void ActualizarUsuario(Usuario usuario, ActualizacionUsuario? actualizacion = null)
    {
        actualizacion ??= new ActualizacionUsuario();

        var usuarioActual = _repositorio.Buscar(u => ReferenceEquals(u, usuario))
            .FirstOrDefault();

        if (usuarioActual == null)
        {
            throw new EntidadInvalidaExcepcion("El usuario no existe en el sistema.");
        }

        var campos = new List<string>();

        if (actualizacion.Email != null && !actualizacion.Email.Equals(usuarioActual.Email, StringComparison.OrdinalIgnoreCase))
        {
            ValidarEmailUnico(actualizacion.Email);
            usuarioActual.ActualizarEmail(actualizacion.Email);
            campos.Add($"email: {actualizacion.Email}");
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

        _repositorio.Actualizar(usuarioActual);

        var detalle = campos.Count > 0
            ? $"Campos modificados en {usuarioActual.Email}: {string.Join(", ", campos)}"
            : $"Sin cambios efectivos en {usuarioActual.Email}";
        _auditoria.Registrar(actualizacion.EjecutadoPor ?? usuarioActual.Email, "EDICION_USUARIO", detalle);
    }

    public void EliminarUsuario(Usuario usuario, string? ejecutadoPor = null)
    {
        _repositorio.Eliminar(usuario);
        _auditoria.Registrar(ejecutadoPor ?? "sistema", "BAJA_USUARIO", $"Usuario eliminado: {usuario.Email}");
    }

    public List<Usuario> ObtenerTodosLosUsuarios()
    {
        return _repositorio.Buscar(_ => true);
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        return _repositorio.ObtenerPorEmail(email);
    }

    private void ValidarEmailUnico(string email)
    {
        var usuarioExistente = _repositorio.ObtenerPorEmail(email);
        if (usuarioExistente != null)
        {
            throw new InvalidOperationException($"El email '{email}' ya está registrado en el sistema.");
        }
    }

    public void GenerarContraseniaDefecto(Usuario usuario, int seed = 0)
    {
        var contraseniaDefecto = $"TempPass@{2026 + seed}";
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

