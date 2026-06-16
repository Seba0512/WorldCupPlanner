using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Dominio.DTO;

public class ClasificadosFaseGruposDTO
{
    public List<EquipoClasificadoDTO> Primeros { get; set; } = [];
    public List<EquipoClasificadoDTO> Segundos { get; set; } = [];
    public List<EquipoClasificadoDTO> MejoresTerceros { get; set; } = [];
    public List<GrupoOrigenClasificadoDTO> GruposDeOrigen { get; set; } = [];

    public List<EquipoClasificadoDTO> Todos =>
        Primeros
            .Concat(Segundos)
            .Concat(MejoresTerceros)
            .ToList();

    public static ClasificadosFaseGruposDTO FromEntity(ClasificadosFaseGrupos clasificados)
    {
        ArgumentNullException.ThrowIfNull(clasificados);

        return new ClasificadosFaseGruposDTO
        {
            Primeros = clasificados.Primeros.Select(EquipoClasificadoDTO.FromEntity).ToList(),
            Segundos = clasificados.Segundos.Select(EquipoClasificadoDTO.FromEntity).ToList(),
            MejoresTerceros = clasificados.MejoresTerceros.Select(EquipoClasificadoDTO.FromEntity).ToList(),
            GruposDeOrigen = clasificados.GruposDeOrigen
                .Select(par => GrupoOrigenClasificadoDTO.FromEntity(par.Key, par.Value))
                .ToList()
        };
    }
}

public class EquipoClasificadoDTO
{
    public string Nombre { get; set; } = string.Empty;
    public int RankingFIFA { get; set; }
    public Confederacion? Confederacion { get; set; }

    public static EquipoClasificadoDTO FromEntity(Equipo equipo)
    {
        ArgumentNullException.ThrowIfNull(equipo);

        return new EquipoClasificadoDTO
        {
            Nombre = equipo.Nombre,
            RankingFIFA = equipo.RankingFIFA,
            Confederacion = equipo.Confederacion
        };
    }
}

public class GrupoClasificadoDTO
{
    public string Etiqueta { get; set; } = string.Empty;
    public List<EquipoClasificadoDTO> Equipos { get; set; } = [];

    public static GrupoClasificadoDTO FromEntity(Grupo grupo)
    {
        ArgumentNullException.ThrowIfNull(grupo);

        return new GrupoClasificadoDTO
        {
            Etiqueta = grupo.Etiqueta,
            Equipos = grupo.Equipos.Select(EquipoClasificadoDTO.FromEntity).ToList()
        };
    }
}

public class GrupoOrigenClasificadoDTO
{
    public EquipoClasificadoDTO Equipo { get; set; } = new();
    public GrupoClasificadoDTO Grupo { get; set; } = new();

    public static GrupoOrigenClasificadoDTO FromEntity(Equipo equipo, Grupo grupo)
    {
        ArgumentNullException.ThrowIfNull(equipo);
        ArgumentNullException.ThrowIfNull(grupo);

        return new GrupoOrigenClasificadoDTO
        {
            Equipo = EquipoClasificadoDTO.FromEntity(equipo),
            Grupo = GrupoClasificadoDTO.FromEntity(grupo)
        };
    }
}
