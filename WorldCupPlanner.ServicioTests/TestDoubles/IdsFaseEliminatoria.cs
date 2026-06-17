namespace WorldCupPlanner.ServicioTests;

// Los partidos se numeran de forma incremental y global a lo largo del torneo:
// 72 de fase de grupos (Ids 1–72), 16 de primera ronda (73–88), 8 de octavos (89–96),
// 4 de cuartos (97–100), 2 de semifinales (101–102), tercer puesto (103) y final (104).
// Estas constantes expresan el último Id de cada fase, usado como offset base de la fase siguiente.
internal static class IdsFaseEliminatoria
{
    public const int UltimoDeFaseDeGrupos = 72;
    public const int UltimoDePrimeraRonda = 88;
    public const int UltimoDeOctavos = 96;
    public const int UltimoDeCuartos = 100;
}
