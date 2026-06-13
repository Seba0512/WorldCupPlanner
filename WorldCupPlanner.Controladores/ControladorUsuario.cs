using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorUsuario
{
    private readonly ServicioUsuario _servicioUsuario;

    public ControladorUsuario(ServicioUsuario servicioUsuario)
    {
        _servicioUsuario = servicioUsuario;
    }

    public List<UsuarioDTO> Listar(Usuario? solicitante = null) =>
        _servicioUsuario.ObtenerTodosLosUsuarios(solicitante)
            .Select(UsuarioDTO.FromEntity)
            .ToList();

    public UsuarioDTO? ObtenerPorEmail(string email)
    {
        var usuario = _servicioUsuario.ObtenerPorEmail(email);
        return usuario == null ? null : UsuarioDTO.FromEntity(usuario);
    }

    public void Crear(NuevoUsuarioDto usuario, string? ejecutadoPor = null, Usuario? solicitante = null) =>
        _servicioUsuario.CrearUsuario(usuario, ejecutadoPor, solicitante);

    public void Actualizar(string emailActual, ActualizacionUsuario actualizacion, Usuario? solicitante = null)
    {
        var usuario = ObtenerEntidadPorEmailOExcepcion(emailActual);
        _servicioUsuario.ActualizarUsuario(usuario, actualizacion, solicitante);
    }

    public void Eliminar(string email, string? ejecutadoPor = null, Usuario? solicitante = null)
    {
        var usuario = ObtenerEntidadPorEmailOExcepcion(email);
        _servicioUsuario.EliminarUsuario(usuario, ejecutadoPor, solicitante);
    }

    public void GestionarRoles(
        string email,
        IEnumerable<RolUsuario> nuevosRoles,
        string? ejecutadoPor = null)
    {
        var usuario = ObtenerEntidadPorEmailOExcepcion(email);
        _servicioUsuario.GestionarRoles(usuario, nuevosRoles, ejecutadoPor);
    }

    public bool EsUltimoAdministrador(string email)
    {
        var usuario = ObtenerEntidadPorEmailOExcepcion(email);
        return _servicioUsuario.EsUltimoAdministrador(usuario);
    }

    public UsuarioDTO Login(string email, string contrasenia) =>
        UsuarioDTO.FromEntity(_servicioUsuario.Login(email, contrasenia));

    public void CambiarContrasenia(string email, CambiarContraseniaDto datos)
    {
        var usuario = ObtenerEntidadPorEmailOExcepcion(email);
        _servicioUsuario.CambiarContrasenia(usuario, datos);
    }

    public string ReiniciarContrasenia(
        string email,
        Usuario administrador,
        bool forzarCambio = false)
    {
        var usuario = ObtenerEntidadPorEmailOExcepcion(email);
        return _servicioUsuario.ReiniciarContrasenia(usuario, administrador, forzarCambio);
    }

    private Usuario ObtenerEntidadPorEmailOExcepcion(string email) =>
        _servicioUsuario.ObtenerPorEmail(email)
        ?? throw new EntidadInexistenteExcepcion($"No existe un usuario con el email '{email}'.");
}
