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

    public ClasificadosFaseGrupos ToEntity()
    {
        var equiposPorNombre = new Dictionary<string, Equipo>();

        Equipo MaterializarEquipo(EquipoClasificadoDTO equipoDto)
        {
            if (equiposPorNombre.TryGetValue(equipoDto.Nombre, out var existente))
                return existente;

            var equipo = equipoDto.ToEntity();
            equiposPorNombre[equipo.Nombre] = equipo;
            return equipo;
        }

        var clasificados = new ClasificadosFaseGrupos
        {
            Primeros = Primeros.Select(MaterializarEquipo).ToList(),
            Segundos = Segundos.Select(MaterializarEquipo).ToList(),
            MejoresTerceros = MejoresTerceros.Select(MaterializarEquipo).ToList()
        };

        foreach (var origen in GruposDeOrigen)
        {
            var equipo = MaterializarEquipo(origen.Equipo);
            clasificados.GruposDeOrigen[equipo] = origen.Grupo.ToEntity(equiposPorNombre);
        }

        return clasificados;
    }

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

    public Equipo ToEntity()
    {
        return new Equipo
        {
            Nombre = Nombre,
            RankingFIFA = RankingFIFA,
            Confederacion = Confederacion
        };
    }

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

    public Grupo ToEntity(Dictionary<string, Equipo>? equiposPorNombre = null)
    {
        var equipos = Equipos.Select(equipoDto =>
        {
            if (equiposPorNombre != null &&
                equiposPorNombre.TryGetValue(equipoDto.Nombre, out var existente))
                return existente;

            var equipo = equipoDto.ToEntity();
            equiposPorNombre?.TryAdd(equipo.Nombre, equipo);
            return equipo;
        }).ToList();

        return new Grupo(Etiqueta, equipos);
    }

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
