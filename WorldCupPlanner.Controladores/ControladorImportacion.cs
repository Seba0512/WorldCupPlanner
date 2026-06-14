using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorImportacion
{
    private readonly ServicioImportacionLegible _servicioImportacion;
    private readonly ServicioEquipo _servicioEquipo;

    public ControladorImportacion(
        ServicioImportacionLegible servicioImportacion,
        ServicioEquipo servicioEquipo)
    {
        _servicioImportacion = servicioImportacion;
        _servicioEquipo = servicioEquipo;
    }

    public ResultadoImportacion Importar(Stream csv, Usuario? solicitante = null) =>
        _servicioImportacion.Importar(csv, _servicioEquipo, solicitante);
}
