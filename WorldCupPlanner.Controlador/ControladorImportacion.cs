using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorImportacion
{
    private readonly ServicioImportacionLegible _servicioImportacion;
    private readonly ServicioEquipo _servicioEquipo;
    private readonly ServicioUsuario? _servicioUsuario;

    public ControladorImportacion(
        ServicioImportacionLegible servicioImportacion,
        ServicioEquipo servicioEquipo,
        ServicioUsuario? servicioUsuario = null)
    {
        _servicioImportacion = servicioImportacion;
        _servicioEquipo = servicioEquipo;
        _servicioUsuario = servicioUsuario;
    }

    public ResultadoImportacion Importar(Stream csv, UsuarioDTO? solicitante = null) =>
        _servicioImportacion.Importar(csv, _servicioEquipo, ResolverUsuario(solicitante));

    private Usuario? ResolverUsuario(UsuarioDTO? usuarioActual)
    {
        if (usuarioActual == null)
            return null;

        if (_servicioUsuario == null)
            throw new InvalidOperationException("El servicio de usuario no fue configurado.");

        return _servicioUsuario.ObtenerPorEmail(usuarioActual.Email)
               ?? throw new EntidadInexistenteExcepcion($"No existe un usuario con el email '{usuarioActual.Email}'.");
    }
}
