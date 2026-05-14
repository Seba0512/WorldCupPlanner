using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Dominio.Entidades;

public static class ReglasConfederacion
{
    public static readonly IReadOnlyDictionary<Confederacion, int> CuposPorConfederacion =
        new Dictionary<Confederacion, int>
        {
            { Confederacion.UEFA,     16 },
            { Confederacion.CONMEBOL,  7 },
            { Confederacion.CONCACAF,  7 },
            { Confederacion.CAF,       9 },
            { Confederacion.AFC,       8 },
            { Confederacion.OFC,       1 },
        };
}
