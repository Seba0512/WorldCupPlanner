using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorConsultasTorneo
{
    private readonly ServicioConsultasTorneo _servicioConsultasTorneo;

    public ControladorConsultasTorneo(ServicioConsultasTorneo servicioConsultasTorneo)
    {
        _servicioConsultasTorneo = servicioConsultasTorneo;
    }

    public List<Partido> ObtenerTodosLosPartidos(EstadoTorneo estado) =>
        _servicioConsultasTorneo.ObtenerTodosLosPartidos(estado);

    public IEnumerable<Equipo> ObtenerTodosLosEquipos(EstadoTorneo estado) =>
        _servicioConsultasTorneo.ObtenerTodosLosEquipos(estado);

    public List<Partido> ObtenerPartidosDeFase(EstadoTorneo estado, FaseTorneo fase) =>
        _servicioConsultasTorneo.ObtenerPartidosDeFase(estado, fase);

    public int ContarTotalPartidos(EstadoTorneo estado) =>
        _servicioConsultasTorneo.ContarTotalPartidos(estado);

    public int ContarPartidosJugados(EstadoTorneo estado) =>
        _servicioConsultasTorneo.ContarPartidosJugados(estado);

    public bool EquipoParticipaEnFixture(EstadoTorneo estado, Equipo equipo) =>
        _servicioConsultasTorneo.EquipoParticipaEnFixture(estado, equipo);

    public bool EquipoParticipaEnFixture(EstadoTorneo estado, EquipoDTO equipo) =>
        EquipoParticipaEnFixture(estado, equipo.ToEntity());

    public bool EquipoParticipaEnFixture(EstadoTorneo estado, string nombreEquipo) =>
        EquipoParticipaEnFixture(estado, new Equipo { Nombre = nombreEquipo });

    public bool EstadioParticipaEnFixture(EstadoTorneo estado, Estadio estadio) =>
        _servicioConsultasTorneo.EstadioParticipaEnFixture(estado, estadio);

    public bool EstadioParticipaEnFixture(EstadoTorneo estado, EstadioDTO estadio) =>
        EstadioParticipaEnFixture(estado, estadio.ToEntity());

    public bool EstadioParticipaEnFixture(EstadoTorneo estado, string nombreEstadio) =>
        EstadioParticipaEnFixture(estado, new Estadio { Nombre = nombreEstadio });

    public FaseConfiguracionTorneo ObtenerFaseConfiguracion(EstadoTorneo estado, int cantidadEquiposRegistrados) =>
        _servicioConsultasTorneo.ObtenerFaseConfiguracion(estado, cantidadEquiposRegistrados);
}
