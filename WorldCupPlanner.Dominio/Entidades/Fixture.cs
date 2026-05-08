namespace WorldCupPlanner.Dominio.Entidades;

public class Fixture
{
    public List<Grupo> Grupos { get; set; } = new List<Grupo>();

    public List<Partido> Partidos
    {
        get
        {
            var partidos = new List<Partido>();

            foreach (var grupo in Grupos)
            {
                partidos.AddRange(grupo.Partidos);
            }

            return partidos;
        }
    }
}
