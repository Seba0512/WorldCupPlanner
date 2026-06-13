using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorEstadisticasEquipo
{
    private readonly ServicioEstadisticasEquipo _servicioEstadisticasEquipo;

    public ControladorEstadisticasEquipo(ServicioEstadisticasEquipo servicioEstadisticasEquipo)
    {
        _servicioEstadisticasEquipo = servicioEstadisticasEquipo;
    }

    public Dictionary<string, EstadisticasEquipo> ObtenerEstadisticasPorEquipo(List<Equipo> equipos) =>
        _servicioEstadisticasEquipo.ObtenerEstadisticasPorEquipo(equipos);

    public EstadisticasEquipo ObtenerEstadisticas(Equipo equipo) =>
        _servicioEstadisticasEquipo.ObtenerEstadisticas(equipo);
}
