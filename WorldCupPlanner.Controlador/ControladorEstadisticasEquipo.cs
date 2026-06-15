using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.DTO;
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

    public Dictionary<string, EstadisticasEquipo> ObtenerEstadisticasPorEquipo(List<EquipoDTO> equipos) =>
        _servicioEstadisticasEquipo.ObtenerEstadisticasPorEquipo(equipos.Select(e => e.ToEntity()).ToList());

    public EstadisticasEquipo ObtenerEstadisticas(Equipo equipo) =>
        _servicioEstadisticasEquipo.ObtenerEstadisticas(equipo);

    public EstadisticasEquipo ObtenerEstadisticas(EquipoDTO equipo) =>
        _servicioEstadisticasEquipo.ObtenerEstadisticas(equipo.ToEntity());
}
