using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Dominio.DTO;

public class ConfiguracionTorneoDTO
{
    public int SemillaFixture { get; set; }
    public int SemillaCompletar { get; set; }
    public int SemillaCrucesFase { get; set; }
    public int SemillaSimulation { get; set; }
    public MotorSimulacion MotorSimulacion { get; set; } = MotorSimulacion.RankingActualYSemilla;
    public DateTime FechaInicioTorneo { get; set; }
    public int MaxPartidosPorDia { get; set; }
    public int SeparacionEntreFechas { get; set; }
    public TimeSpan HoraInicioPartidos { get; set; }
    public int SeparacionHorasEntrePartidos { get; set; }

    public static ConfiguracionTorneoDTO FromEntity(ConfiguracionTorneo configuracion)
    {
        ArgumentNullException.ThrowIfNull(configuracion);

        return new ConfiguracionTorneoDTO
        {
            SemillaFixture = configuracion.SemillaFixture,
            SemillaCompletar = configuracion.SemillaCompletar,
            SemillaCrucesFase = configuracion.SemillaCrucesFase,
            SemillaSimulation = configuracion.SemillaSimulation,
            MotorSimulacion = configuracion.MotorSimulacion,
            FechaInicioTorneo = configuracion.FechaInicioTorneo,
            MaxPartidosPorDia = configuracion.MaxPartidosPorDia,
            SeparacionEntreFechas = configuracion.SeparacionEntreFechas,
            HoraInicioPartidos = configuracion.HoraInicioPartidos,
            SeparacionHorasEntrePartidos = configuracion.SeparacionHorasEntrePartidos
        };
    }
}
