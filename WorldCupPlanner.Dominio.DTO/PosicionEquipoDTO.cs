using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.Dominio.DTO;

public class PosicionEquipoDTO
{
    public EquipoDTO? Equipo { get; set; }
    public string? EquipoNombre { get; set; }
    public string? GrupoEtiqueta { get; set; }
    public int Posicion { get; set; }
    public bool Clasifica { get; set; }
    public bool EsTercero { get; set; }
    public int PartidosJugados { get; set; }
    public int PartidosGanados { get; set; }
    public int PartidosEmpatados { get; set; }
    public int PartidosPerdidos { get; set; }
    public int Puntos { get; set; }
    public int GolesAFavor { get; set; }
    public int GolesEnContra { get; set; }
    public int DiferenciaGoles { get; set; }

    public PosicionEquipo ToEntity()
    {
        return new PosicionEquipo
        {
            Equipo = Equipo?.ToEntity()!,
            Grupo = CrearGrupoDesdeEtiqueta()!,
            Posicion = Posicion,
            PartidosJugados = PartidosJugados,
            PartidosGanados = PartidosGanados,
            PartidosEmpatados = PartidosEmpatados,
            PartidosPerdidos = PartidosPerdidos,
            Puntos = Puntos,
            GolesAFavor = GolesAFavor,
            GolesEnContra = GolesEnContra
        };
    }

    public static PosicionEquipoDTO FromEntity(PosicionEquipo posicion)
    {
        ArgumentNullException.ThrowIfNull(posicion);

        return new PosicionEquipoDTO
        {
            Equipo = posicion.Equipo == null ? null : EquipoDTO.FromEntity(posicion.Equipo),
            EquipoNombre = posicion.Equipo?.Nombre,
            GrupoEtiqueta = posicion.Grupo?.Etiqueta,
            Posicion = posicion.Posicion,
            Clasifica = posicion.Clasifica,
            EsTercero = posicion.EsTercero,
            PartidosJugados = posicion.PartidosJugados,
            PartidosGanados = posicion.PartidosGanados,
            PartidosEmpatados = posicion.PartidosEmpatados,
            PartidosPerdidos = posicion.PartidosPerdidos,
            Puntos = posicion.Puntos,
            GolesAFavor = posicion.GolesAFavor,
            GolesEnContra = posicion.GolesEnContra,
            DiferenciaGoles = posicion.DiferenciaGoles
        };
    }

    private Grupo? CrearGrupoDesdeEtiqueta()
    {
        return string.IsNullOrWhiteSpace(GrupoEtiqueta)
            ? null
            : new Grupo(GrupoEtiqueta);
    }
}
