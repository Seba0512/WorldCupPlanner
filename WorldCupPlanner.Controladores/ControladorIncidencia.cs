using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorIncidencia
{
    private readonly ServicioIncidencia _servicioIncidencia;

    public ControladorIncidencia(ServicioIncidencia servicioIncidencia)
    {
        _servicioIncidencia = servicioIncidencia;
    }

    public Equipo ResolverEquipoPorRol(Partido partido, string rolEquipo) =>
        _servicioIncidencia.ResolverEquipoPorRol(partido, rolEquipo);

    public void AgregarTarjeta(int partidoId, TipoTarjeta tipo, int minuto, Equipo equipo) =>
        _servicioIncidencia.AgregarTarjeta(partidoId, tipo, minuto, equipo);

    public List<Incidencia> ObtenerIncidencias(int partidoId) =>
        _servicioIncidencia.ObtenerIncidencias(partidoId);

    public void EliminarIncidencia(int incidenciaId) =>
        _servicioIncidencia.EliminarIncidencia(incidenciaId);
}
