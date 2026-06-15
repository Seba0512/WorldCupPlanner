using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Dominio.DTO;

public class IncidenciaDTO
{
    public int Id { get; set; }
    public int? PartidoId { get; set; }
    public TipoTarjeta Tipo { get; set; }
    public int Minuto { get; set; }
    public EquipoDTO? Equipo { get; set; }
    public string? EquipoNombre { get; set; }

    public static IncidenciaDTO FromEntity(Incidencia incidencia)
    {
        ArgumentNullException.ThrowIfNull(incidencia);

        return new IncidenciaDTO
        {
            Id = incidencia.Id,
            PartidoId = incidencia.PartidoId,
            Tipo = incidencia is TarjetaRoja ? TipoTarjeta.Roja : TipoTarjeta.Amarilla,
            Minuto = incidencia.MinutoDelPartido,
            Equipo = incidencia.Equipo == null ? null : EquipoDTO.FromEntity(incidencia.Equipo),
            EquipoNombre = incidencia.Equipo?.Nombre
        };
    }
}
