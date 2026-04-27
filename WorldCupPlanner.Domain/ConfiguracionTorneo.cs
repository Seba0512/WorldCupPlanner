namespace WorldCupPlanner.Domain;

public class ConfiguracionTorneo
{
    public int SemillaFixture { get; set; }
    public int SemillaCompletar { get; set; }
    public int SemillaCrucesFase { get; set; }
    public int SemillaSimulation { get; set; }

    public DateTime FechaInicioTorneo { get; set; } = new DateTime(2026, 6, 1);
    public int MaxPartidosPorDia { get; set; } = 3;
    public int SeparacionEntreFechas { get; set; } = 3;
    public TimeSpan HoraInicioPartidos { get; set; } = new TimeSpan(14, 0, 0);
    public int SeparacionHorasEntrePartidos { get; set; } = 4;
}