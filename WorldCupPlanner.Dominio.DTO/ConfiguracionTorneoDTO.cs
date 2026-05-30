using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Dominio.DTO;

public class ConfiguracionTorneoDTO
{
    public int SemillaFixture { get; set; }
    public int SemillaCompletar { get; set; }
    public int SemillaCrucesFase { get; set; }
    public int SemillaSimulation { get; set; }
    public DateTime FechaInicioTorneo { get; set; }
    public int MaxPartidosPorDia { get; set; }
    public int SeparacionEntreFechas { get; set; }
    public TimeSpan HoraInicioPartidos { get; set; }
    public int SeparacionHorasEntrePartidos { get; set; }

    public ConfiguracionTorneo ToEntity()
    {
        return new ConfiguracionTorneo
        {
            SemillaFixture = SemillaFixture,
            SemillaCompletar = SemillaCompletar,
            SemillaCrucesFase = SemillaCrucesFase,
            SemillaSimulation = SemillaSimulation,
            FechaInicioTorneo = FechaInicioTorneo,
            MaxPartidosPorDia = MaxPartidosPorDia,
            SeparacionEntreFechas = SeparacionEntreFechas,
            HoraInicioPartidos = HoraInicioPartidos,
            SeparacionHorasEntrePartidos = SeparacionHorasEntrePartidos
        };
    }

    public static ConfiguracionTorneoDTO FromEntity(ConfiguracionTorneo configuracion)
    {
        ArgumentNullException.ThrowIfNull(configuracion);

        return new ConfiguracionTorneoDTO
        {
            SemillaFixture = configuracion.SemillaFixture,
            SemillaCompletar = configuracion.SemillaCompletar,
            SemillaCrucesFase = configuracion.SemillaCrucesFase,
            SemillaSimulation = configuracion.SemillaSimulation,
            FechaInicioTorneo = configuracion.FechaInicioTorneo,
            MaxPartidosPorDia = configuracion.MaxPartidosPorDia,
            SeparacionEntreFechas = configuracion.SeparacionEntreFechas,
            HoraInicioPartidos = configuracion.HoraInicioPartidos,
            SeparacionHorasEntrePartidos = configuracion.SeparacionHorasEntrePartidos
        };
    }
}
