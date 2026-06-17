using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public record ComandoRegenerarFixture(
    List<Equipo> Equipos,
    Fixture FixtureActual,
    List<Estadio> Estadios,
    ConfiguracionTorneo Configuracion,
    IReadOnlyList<Partido>? PartidosEliminatorios = null);

public record ComandoSimularPartidos(
    List<Partido> Partidos,
    ConfiguracionTorneo Configuracion,
    EstadoTorneo EstadoTorneo,
    UsuarioDTO? Usuario = null);

public record ComandoSimularJornada(
    FaseTorneo EtapaActiva,
    int JornadaActiva,
    EstadoTorneo EstadoTorneo,
    UsuarioDTO? Usuario = null);

public record ComandoGuardarPartido(
    int PartidoId,
    DateTime NuevaFecha,
    string NombreEstadio,
    int? GolesLocal,
    int? GolesVisitante,
    EstadoTorneo EstadoTorneo,
    UsuarioDTO? Usuario = null);

public record ComandoTarjeta(TipoTarjeta Tipo, int Minuto);
