using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.ControladorTests;

internal static class FabricaEntidades
{
    internal const int RankingFifaPorDefecto = 1200;
    internal const int RankingFifaMaximo = 2500;
    internal const int CapacidadEstadioGrande = 60000;
    internal const int CapacidadEstadioMinima = 30000;

    public static Equipo CrearEquipo(
        string nombre,
        Confederacion confederacion = Confederacion.UEFA,
        int rankingFifa = RankingFifaPorDefecto,
        int id = 0,
        string? bandera = null) => new()
    {
        Id = id,
        Nombre = nombre,
        Confederacion = confederacion,
        RankingFIFA = rankingFifa,
        RankingInicial = rankingFifa,
        Bandera = bandera
    };

    public static Estadio CrearEstadio(
        string nombre,
        string ciudad = "Montevideo",
        int capacidad = CapacidadEstadioGrande) => new()
    {
        Nombre = nombre,
        Ciudad = ciudad,
        Capacidad = capacidad
    };

    public static List<Equipo> CrearEquiposValidos()
    {
        var distribucion = new (Confederacion Confederacion, int Cantidad)[]
        {
            (Confederacion.UEFA, 16),
            (Confederacion.CONMEBOL, 7),
            (Confederacion.CONCACAF, 7),
            (Confederacion.CAF, 9),
            (Confederacion.AFC, 8),
            (Confederacion.OFC, 1)
        };

        var equipos = new List<Equipo>();
        var contador = 1;

        foreach (var (confederacion, cantidad) in distribucion)
        {
            for (var i = 0; i < cantidad; i++)
            {
                equipos.Add(new Equipo
                {
                    Id = contador,
                    Nombre = $"Equipo{contador:00}",
                    Confederacion = confederacion,
                    RankingFIFA = RankingFifaMaximo - contador,
                    RankingInicial = RankingFifaMaximo - contador
                });
                contador++;
            }
        }

        return equipos;
    }
}
