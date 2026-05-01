namespace WorldCupPlanner.Domain;

public class Fixture
{
    public List<Grupo> Grupos { get; set; } = new List<Grupo>();

    public List<Partido> Partidos
    {
        get
        {
            return Grupos
                .SelectMany(g => g.Partidos)
                .ToList();
        }
    }
}