using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Dominio.DTO;

public class EquipoDTO
{
    public string Nombre { get; set; } = string.Empty;
    public int RankingFIFA { get; set; }
    public Confederacion? Confederacion { get; set; }

    public Equipo ToEntity()
    {
        return new Equipo
        {
            Nombre = Nombre,
            RankingFIFA = RankingFIFA,
            Confederacion = Confederacion
        };
    }

    public static EquipoDTO FromEntity(Equipo equipo)
    {
        ArgumentNullException.ThrowIfNull(equipo);

        return new EquipoDTO
        {
            Nombre = equipo.Nombre,
            RankingFIFA = equipo.RankingFIFA,
            Confederacion = equipo.Confederacion
        };
    }
}