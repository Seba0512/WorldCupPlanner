using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorClasificacion
{
    private readonly ServicioClasificacion _servicioClasificacion;
    private readonly ServicioConfiguracionTorneo? _servicioConfiguracionTorneo;

    public ControladorClasificacion(
        ServicioClasificacion servicioClasificacion,
        ServicioConfiguracionTorneo? servicioConfiguracionTorneo = null)
    {
        _servicioClasificacion = servicioClasificacion;
        _servicioConfiguracionTorneo = servicioConfiguracionTorneo;
    }

    public List<PosicionEquipo> CalcularTabla(Grupo grupo) =>
        _servicioClasificacion.CalcularTabla(grupo);

    public List<PosicionEquipoDTO> CalcularTablaDTO(Grupo grupo) =>
        CalcularTabla(grupo).Select(PosicionEquipoDTO.FromEntity).ToList();

    public List<PosicionEquipo> CalcularTabla(Grupo grupo, ConfiguracionTorneo configuracionTorneo) =>
        _servicioClasificacion.CalcularTabla(grupo, configuracionTorneo);

    public List<PosicionEquipoDTO> CalcularTablaDTO(Grupo grupo, ConfiguracionTorneo configuracionTorneo) =>
        CalcularTabla(grupo, configuracionTorneo).Select(PosicionEquipoDTO.FromEntity).ToList();

    public List<PosicionEquipoDTO> CalcularTablaDTO(EstadoTorneo estadoTorneo, string grupoEtiqueta)
    {
        var grupo = estadoTorneo.Fixture?.Grupos.FirstOrDefault(g => g.Etiqueta == grupoEtiqueta);
        if (grupo == null)
            return new List<PosicionEquipoDTO>();

        var configuracion = _servicioConfiguracionTorneo?.ObtenerConfiguracionActual();
        return configuracion == null
            ? CalcularTablaDTO(grupo)
            : CalcularTablaDTO(grupo, configuracion);
    }

    public List<PosicionEquipoDTO> CalcularTablaAnteriorDTO(EstadoTorneo estadoTorneo, string grupoEtiqueta)
    {
        var grupo = estadoTorneo.FixtureAnterior?.Grupos.FirstOrDefault(g => g.Etiqueta == grupoEtiqueta);
        return grupo == null
            ? new List<PosicionEquipoDTO>()
            : CalcularTablaDTO(grupo);
    }

    public ClasificadosFaseGrupos ObtenerClasificadosAFaseEliminatoria(
        Fixture fixture,
        ConfiguracionTorneo configuracionTorneo) =>
        _servicioClasificacion.ObtenerClasificadosAFaseEliminatoria(fixture, configuracionTorneo);

    public ClasificadosFaseGruposDTO ObtenerClasificadosAFaseEliminatoriaDTO(
        Fixture fixture,
        ConfiguracionTorneo configuracionTorneo) =>
        ClasificadosFaseGruposDTO.FromEntity(ObtenerClasificadosAFaseEliminatoria(fixture, configuracionTorneo));

    public Equipo? ObtenerCampeon(Partido? partidoFinal) =>
        _servicioClasificacion.ObtenerCampeon(partidoFinal);

    public EquipoDTO? ObtenerCampeonDTO(Partido? partidoFinal)
    {
        var campeon = ObtenerCampeon(partidoFinal);
        return campeon == null ? null : EquipoDTO.FromEntity(campeon);
    }
}
