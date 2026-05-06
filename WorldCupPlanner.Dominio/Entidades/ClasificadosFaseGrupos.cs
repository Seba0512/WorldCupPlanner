using System.Linq;

namespace WorldCupPlanner.Dominio.Entidades;

public class ClasificadosFaseGrupos
{
    public List<Equipo> Primeros { get; set; } = new List<Equipo>();
    public List<Equipo> Segundos { get; set; } = new List<Equipo>();
    public List<Equipo> MejoresTerceros { get; set; } = new List<Equipo>();
    public Dictionary<Equipo, Grupo> GruposDeOrigen { get; set; } = new Dictionary<Equipo, Grupo>();

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

    public Grupo? ObtenerGrupoDeOrigen(Equipo equipo)
    {
        return GruposDeOrigen.GetValueOrDefault(equipo);
    }
}
