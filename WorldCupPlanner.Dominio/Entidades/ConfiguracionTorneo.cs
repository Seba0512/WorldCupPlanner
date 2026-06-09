namespace WorldCupPlanner.Dominio.Entidades;

public class ConfiguracionTorneo
{
    private const int AnioInicioPorDefecto = 2026;
    private const int MesInicioPorDefecto = 6;
    private const int DiaInicioPorDefecto = 1;
    private const int MaxPartidosPorDiaPorDefecto = 3;
    private const int SeparacionEntreFechasPorDefecto = 3;
    private const int HoraInicioPorDefecto = 14;
    private const int MinutoInicioPorDefecto = 0;
    private const int SeparacionHorasEntrePartidosPorDefecto = 4;

    public int Id { get; set; }
    public int SemillaFixture { get; set; }
    public int SemillaCompletar { get; set; }
    public int SemillaCrucesFase { get; set; }
    public int SemillaSimulation { get; set; }
    public WorldCupPlanner.Dominio.Enumeraciones.MotorSimulacion MotorSimulacion { get; set; } =
        WorldCupPlanner.Dominio.Enumeraciones.MotorSimulacion.RankingActualYSemilla;

    public DateTime FechaInicioTorneo { get; set; } =
        new DateTime(AnioInicioPorDefecto, MesInicioPorDefecto, DiaInicioPorDefecto);

    public int MaxPartidosPorDia { get; set; } = MaxPartidosPorDiaPorDefecto;
    public int SeparacionEntreFechas { get; set; } = SeparacionEntreFechasPorDefecto;

    public TimeSpan HoraInicioPartidos { get; set; } =
        new TimeSpan(HoraInicioPorDefecto, MinutoInicioPorDefecto, 0);

    public int SeparacionHorasEntrePartidos { get; set; } =
        SeparacionHorasEntrePartidosPorDefecto;
}

