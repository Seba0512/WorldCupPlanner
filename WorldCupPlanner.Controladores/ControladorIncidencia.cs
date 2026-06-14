using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorIncidencia
{
    private readonly ServicioIncidencia _servicioIncidencia;
    private readonly ServicioPersistenciaFixture? _persistenciaFixture;

    public ControladorIncidencia(ServicioIncidencia servicioIncidencia)
    {
        _servicioIncidencia = servicioIncidencia;
    }

    public ControladorIncidencia(
        ServicioIncidencia servicioIncidencia,
        ServicioPersistenciaFixture persistenciaFixture)
        : this(servicioIncidencia)
    {
        _persistenciaFixture = persistenciaFixture;
    }

    public Equipo ResolverEquipoPorRol(Partido partido, string rolEquipo) =>
        _servicioIncidencia.ResolverEquipoPorRol(partido, rolEquipo);

    public void AgregarTarjeta(int partidoId, TipoTarjeta tipo, int minuto, Equipo equipo) =>
        _servicioIncidencia.AgregarTarjeta(partidoId, tipo, minuto, equipo);

    public List<Incidencia> ObtenerIncidencias(int partidoId) =>
        _servicioIncidencia.ObtenerIncidencias(partidoId);

    public void EliminarIncidencia(int incidenciaId) =>
        _servicioIncidencia.EliminarIncidencia(incidenciaId);

    public void AgregarTarjetaAPartido(Partido partido, string rolEquipo, TipoTarjeta tipo, int minuto)
    {
        if (_persistenciaFixture == null)
            throw new InvalidOperationException("La persistencia de fixture no fue configurada.");

        _persistenciaFixture.GuardarPartidos(new[] { partido });
        var equipo = _servicioIncidencia.ResolverEquipoPorRol(partido, rolEquipo);
        _servicioIncidencia.AgregarTarjeta(partido.Id, tipo, minuto, equipo);
    }
}
