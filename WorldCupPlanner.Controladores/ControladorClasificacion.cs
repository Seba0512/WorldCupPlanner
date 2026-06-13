using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorClasificacion
{
    private readonly ServicioClasificacion _servicioClasificacion;

    public ControladorClasificacion(ServicioClasificacion servicioClasificacion)
    {
        _servicioClasificacion = servicioClasificacion;
    }

    public List<PosicionEquipo> CalcularTabla(Grupo grupo) =>
        _servicioClasificacion.CalcularTabla(grupo);

    public List<PosicionEquipo> CalcularTabla(Grupo grupo, ConfiguracionTorneo configuracionTorneo) =>
        _servicioClasificacion.CalcularTabla(grupo, configuracionTorneo);

    public ClasificadosFaseGrupos ObtenerClasificadosAFaseEliminatoria(
        Fixture fixture,
        ConfiguracionTorneo configuracionTorneo) =>
        _servicioClasificacion.ObtenerClasificadosAFaseEliminatoria(fixture, configuracionTorneo);

    public Equipo? ObtenerCampeon(Partido? partidoFinal) =>
        _servicioClasificacion.ObtenerCampeon(partidoFinal);
}
