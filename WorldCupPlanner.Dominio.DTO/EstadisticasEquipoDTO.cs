using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Dominio.DTO;

public class EstadisticasEquipoDTO
{
    public int PartidosJugados { get; set; }
    public int GolesAFavor { get; set; }
    public int GolesEnContra { get; set; }
    public int DiferenciaGoles { get; set; }
    public int Puntos { get; set; }

    public static EstadisticasEquipoDTO FromEntity(EstadisticasEquipo estadisticas)
    {
        ArgumentNullException.ThrowIfNull(estadisticas);

        return new EstadisticasEquipoDTO
        {
            PartidosJugados = estadisticas.PartidosJugados,
            GolesAFavor = estadisticas.GolesAFavor,
            GolesEnContra = estadisticas.GolesEnContra,
            DiferenciaGoles = estadisticas.DiferenciaGoles,
            Puntos = estadisticas.Puntos
        };
    }
}
