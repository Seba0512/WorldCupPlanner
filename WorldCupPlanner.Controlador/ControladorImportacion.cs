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
        _servicioImportacion.Importar(csv, _servicioEquipo, _servicioUsuario.ResolverUsuario(solicitante));
}
