namespace WorldCupPlanner.Dominio;

public class ClasificadosFaseGrupos
{
    public List<Equipo> Primeros { get; set; } = new List<Equipo>();
    public List<Equipo> Segundos { get; set; } = new List<Equipo>();
    public List<Equipo> MejoresTerceros { get; set; } = new List<Equipo>();

    public List<Equipo> Todos
    {
        get
        {
            return Primeros
                .Concat(Segundos)
                .Concat(MejoresTerceros)
                .ToList();
        }
    }
}