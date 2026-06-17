using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorIncidencia
{
    private readonly ServicioIncidencia _servicioIncidencia;
    private readonly ServicioPersistenciaFixture? _persistenciaFixture;
    private readonly ServicioPartido? _servicioPartido;

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

    public ControladorIncidencia(
        ServicioIncidencia servicioIncidencia,
        ServicioPersistenciaFixture persistenciaFixture,
        ServicioPartido servicioPartido)
        : this(servicioIncidencia, persistenciaFixture)
    {
        _servicioPartido = servicioPartido;
    }

    public Equipo ResolverEquipoPorRol(Partido partido, string rolEquipo) =>
        _servicioIncidencia.ResolverEquipoPorRol(partido, rolEquipo);

    public void AgregarTarjeta(int partidoId, Equipo equipo, ComandoTarjeta comando) =>
        _servicioIncidencia.AgregarTarjeta(partidoId, comando.Tipo, comando.Minuto, equipo);

    public List<Incidencia> ObtenerIncidencias(int partidoId) =>
        _servicioIncidencia.ObtenerIncidencias(partidoId);

    public List<IncidenciaDTO> ObtenerIncidenciasDTO(int partidoId) =>
        ObtenerIncidencias(partidoId).Select(IncidenciaDTO.FromEntity).ToList();

    public void EliminarIncidencia(int incidenciaId) =>
        _servicioIncidencia.EliminarIncidencia(incidenciaId);

    public void AgregarTarjetaAPartido(Partido partido, string rolEquipo, ComandoTarjeta comando)
    {
        if (_persistenciaFixture == null)
            throw new InvalidOperationException("La persistencia de fixture no fue configurada.");

        _persistenciaFixture.GuardarPartidos(new[] { partido });
        var equipo = _servicioIncidencia.ResolverEquipoPorRol(partido, rolEquipo);
        _servicioIncidencia.AgregarTarjeta(partido.Id, comando.Tipo, comando.Minuto, equipo);
    }

    public void AgregarTarjetaAPartido(int partidoId, string rolEquipo, ComandoTarjeta comando)
    {
        if (_servicioPartido == null)
            throw new InvalidOperationException("El servicio de partido no fue configurado.");

        var partido = _servicioPartido.BuscarPartidoPorId(partidoId)
                      ?? throw new EntidadInexistenteExcepcion("El partido no existe.");
        AgregarTarjetaAPartido(partido, rolEquipo, comando);
    }
}
