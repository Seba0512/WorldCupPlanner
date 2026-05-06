using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioUsuario
{
    private readonly RepositorioUsuarioEnMemoria _repositorio;

    public ServicioUsuario(RepositorioUsuarioEnMemoria repositorio)
    {
        _repositorio = repositorio;
    }

    public void AgregarUsuario(Usuario usuario)
    {
        ValidarEmailUnico(usuario.Email);
        _repositorio.Agregar(usuario);
    }
    
    public void ActualizarUsuario(Usuario usuario, string? nombre = null, string? apellido = null, 
        string? email = null, DateTime? fechaNacimiento = null)
    {
        var usuarioActual = _repositorio.Buscar(u => ReferenceEquals(u, usuario))
            .FirstOrDefault();

        if (usuarioActual == null)
        {
            throw new EntidadInvalidaExcepcion("El usuario no existe en el sistema.");
        }

        if (email != null && !email.Equals(usuarioActual.Email, StringComparison.OrdinalIgnoreCase))
        {
            ValidarEmailUnico(email);
            usuarioActual.ActualizarEmail(email);
        }

        if (nombre != null)
        {
            usuarioActual.ActualizarNombre(nombre);
        }

        if (apellido != null)
        {
            usuarioActual.ActualizarApellido(apellido);
        }

        if (fechaNacimiento.HasValue)
        {
            usuarioActual.ActualizarFechaNacimiento(fechaNacimiento.Value);
        }

        _repositorio.Actualizar(usuarioActual);
    }

    public void EliminarUsuario(Usuario usuario)
    {
        _repositorio.Eliminar(usuario);
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
    }

    public void CambiarContrasenia(Usuario usuario, string contraseniaAnterior, string nuevaContrasenia)
    {
        if (!usuario.VerificarContrasenia(contraseniaAnterior))
        {
            throw new InvalidOperationException("La contraseña actual es incorrecta.");
        }

        usuario.EstablecerContrasenia(nuevaContrasenia);
    }

    public Usuario Login(string email, string contrasenia)
    {
        var usuario = _repositorio.ObtenerPorEmail(email);
        if (usuario == null)
        {
            throw new InvalidOperationException("Email o contraseña incorrectos.");
        }

        if (!usuario.VerificarContrasenia(contrasenia))
        {
            throw new InvalidOperationException("Email o contraseña incorrectos.");
        }

        return usuario;
    }
}

