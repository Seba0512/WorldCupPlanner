using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Dominio.DTO;

public class FixtureDTO
{
    public List<GrupoDTO> Grupos { get; set; } = [];

    public List<PartidoDTO> Partidos =>
        Grupos
            .SelectMany(grupo => grupo.Partidos)
            .ToList();

    public static FixtureDTO FromEntity(Fixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

        return new FixtureDTO
        {
            Grupos = fixture.Grupos.Select(GrupoDTO.FromEntity).ToList()
        };
    }
}
