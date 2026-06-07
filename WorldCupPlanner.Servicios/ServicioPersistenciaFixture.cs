using System.Text.Json;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioPersistenciaFixture
{
    private readonly IRepositorio<Grupo> _repositorioGrupo;
    private readonly IRepositorio<Partido> _repositorioPartido;
    private readonly IRepositorioTorneoArchivado _repositorioTorneoArchivado;

    public ServicioPersistenciaFixture(
        IRepositorio<Grupo> repositorioGrupo,
        IRepositorio<Partido> repositorioPartido,
        IRepositorioTorneoArchivado repositorioTorneoArchivado)
    {
        _repositorioGrupo = repositorioGrupo;
        _repositorioPartido = repositorioPartido;
        _repositorioTorneoArchivado = repositorioTorneoArchivado;
    }

    public void GuardarFixture(Fixture fixture)
    {
        GuardarGrupos(fixture.Grupos);
        GuardarPartidos(fixture.Partidos);
    }

    public void GuardarPartidos(IEnumerable<Partido> partidos)
    {
        var existentesPorId = _repositorioPartido.Buscar(_ => true)
            .ToDictionary(partido => partido.Id);

        foreach (var partido in partidos)
        {
            if (existentesPorId.TryGetValue(partido.Id, out var existente))
            {
                CopiarDatosPartido(partido, existente);
                _repositorioPartido.Actualizar(existente);
                continue;
            }

            _repositorioPartido.Agregar(partido);
            existentesPorId[partido.Id] = partido;
        }
    }

    public void LimpiarFixture()
    {
        var partidos = _repositorioPartido.Buscar(_ => true);
        foreach (var partido in partidos)
            _repositorioPartido.Eliminar(partido);

        var grupos = _repositorioGrupo.Buscar(_ => true);
        foreach (var grupo in grupos)
            _repositorioGrupo.Eliminar(grupo);
    }

    public void ArchivarTorneoActual()
    {
        var grupos = _repositorioGrupo.Buscar(_ => true);
        if (!grupos.Any()) return;

        var partidosEliminatorios = _repositorioPartido
            .Buscar(p => p.Fase != FaseTorneo.FaseDeGrupos);

        var torneoArchivado = new TorneoArchivado
        {
            GruposJson = JsonSerializer.Serialize(grupos.Select(GrupoADto).ToList()),
            PartidosEliminatoriosJson = JsonSerializer.Serialize(
                partidosEliminatorios.Select(PartidoADto).ToList())
        };

        _repositorioTorneoArchivado.Guardar(torneoArchivado);
        LimpiarFixture();
    }

    public void CargarEnEstadoTorneo(EstadoTorneo estadoTorneo)
    {
        var grupos = _repositorioGrupo.Buscar(_ => true);
        if (grupos.Any())
        {
            estadoTorneo.Fixture = new Fixture { Grupos = grupos };
            estadoTorneo.PartidosEliminatorios = _repositorioPartido
                .Buscar(p => p.Fase != FaseTorneo.FaseDeGrupos);
            estadoTorneo.TorneoFinalizado = estadoTorneo.PartidosEliminatorios
                .Any(p => p.Fase == FaseTorneo.Final && p.EstaJugado());
        }

        CargarTorneoAnteriorEnEstado(estadoTorneo);
    }

    private void CargarTorneoAnteriorEnEstado(EstadoTorneo estadoTorneo)
    {
        var archivo = _repositorioTorneoArchivado.Obtener();
        if (archivo == null) return;

        var gruposDto = JsonSerializer.Deserialize<List<GrupoDto>>(archivo.GruposJson);
        var partidosDto = JsonSerializer.Deserialize<List<PartidoDto>>(archivo.PartidosEliminatoriosJson);

        if (gruposDto == null || partidosDto == null) return;

        estadoTorneo.FixtureAnterior = new Fixture { Grupos = gruposDto.Select(DtoAGrupo).ToList() };
        estadoTorneo.PartidosEliminatoriosAnteriores = partidosDto.Select(DtoAPartido).ToList();
    }

    private void GuardarGrupos(IEnumerable<Grupo> grupos)
    {
        var etiquetasExistentes = _repositorioGrupo.Buscar(_ => true)
            .Select(grupo => grupo.Etiqueta)
            .ToHashSet();

        foreach (var grupo in grupos)
        {
            if (etiquetasExistentes.Contains(grupo.Etiqueta))
            {
                continue;
            }

            _repositorioGrupo.Agregar(grupo);
            etiquetasExistentes.Add(grupo.Etiqueta);
        }
    }

    private static void CopiarDatosPartido(Partido origen, Partido destino)
    {
        destino.Fecha = origen.Fecha;
        destino.Estadio = origen.Estadio;
        destino.EquipoLocal = origen.EquipoLocal;
        destino.EquipoVisitante = origen.EquipoVisitante;
        destino.Fase = origen.Fase;
        destino.GolesLocal = origen.GolesLocal;
        destino.GolesVisitante = origen.GolesVisitante;
        destino.Grupo = origen.Grupo;
        destino.GanadorPorPenales = origen.GanadorPorPenales;
    }

    private static GrupoDto GrupoADto(Grupo grupo) => new(
        grupo.Etiqueta,
        grupo.Equipos.Select(EquipoADto).ToList(),
        grupo.Partidos.Select(PartidoADto).ToList());

    private static EquipoDto EquipoADto(Equipo equipo) => new(
        equipo.Id,
        equipo.Nombre,
        equipo.RankingFIFA,
        (int)(equipo.Confederacion ?? 0));

    private static EstadioDto? EstadioADto(Estadio? estadio) =>
        estadio == null ? null : new(estadio.Nombre);

    private static PartidoDto PartidoADto(Partido partido) => new(
        partido.Id,
        partido.Fecha,
        partido.EquipoLocal != null ? EquipoADto(partido.EquipoLocal) : null,
        partido.EquipoVisitante != null ? EquipoADto(partido.EquipoVisitante) : null,
        EstadioADto(partido.Estadio),
        (int)partido.Fase,
        partido.GolesLocal,
        partido.GolesVisitante,
        partido.GanadorPorPenales != null ? EquipoADto(partido.GanadorPorPenales) : null);

    private static Grupo DtoAGrupo(GrupoDto dto)
    {
        var equiposPorNombre = dto.Equipos.Select(DtoAEquipo).ToDictionary(e => e.Nombre);
        var grupo = new Grupo(dto.Etiqueta, equiposPorNombre.Values.ToList());

        foreach (var pDto in dto.Partidos)
        {
            var equipoLocal = pDto.EquipoLocal != null
                ? equiposPorNombre.GetValueOrDefault(pDto.EquipoLocal.Nombre) : null;
            var equipoVisitante = pDto.EquipoVisitante != null
                ? equiposPorNombre.GetValueOrDefault(pDto.EquipoVisitante.Nombre) : null;

            var partido = new Partido
            {
                Id = pDto.Id,
                Fecha = pDto.Fecha,
                EquipoLocal = equipoLocal,
                EquipoVisitante = equipoVisitante,
                Estadio = pDto.Estadio != null ? DtoAEstadio(pDto.Estadio) : null,
                Fase = (FaseTorneo)pDto.Fase,
                GolesLocal = pDto.GolesLocal,
                GolesVisitante = pDto.GolesVisitante,
                GanadorPorPenales = ObtenerGanadorPorPenales(pDto.GanadorPorPenales, equipoLocal, equipoVisitante)
            };
            grupo.Partidos.Add(partido);
        }

        return grupo;
    }

    private static Partido DtoAPartido(PartidoDto dto)
    {
        var equipoLocal = dto.EquipoLocal != null ? DtoAEquipo(dto.EquipoLocal) : null;
        var equipoVisitante = dto.EquipoVisitante != null ? DtoAEquipo(dto.EquipoVisitante) : null;

        return new Partido
        {
            Id = dto.Id,
            Fecha = dto.Fecha,
            EquipoLocal = equipoLocal,
            EquipoVisitante = equipoVisitante,
            Estadio = dto.Estadio != null ? DtoAEstadio(dto.Estadio) : null,
            Fase = (FaseTorneo)dto.Fase,
            GolesLocal = dto.GolesLocal,
            GolesVisitante = dto.GolesVisitante,
            GanadorPorPenales = ObtenerGanadorPorPenales(dto.GanadorPorPenales, equipoLocal, equipoVisitante)
        };
    }

    private static Equipo DtoAEquipo(EquipoDto dto) => new()
    {
        Id = dto.Id,
        Nombre = dto.Nombre,
        RankingFIFA = dto.RankingFIFA,
        Confederacion = (Confederacion)dto.Confederacion
    };

    private static Estadio DtoAEstadio(EstadioDto dto) => new() { Nombre = dto.Nombre };

    private static Equipo? ObtenerGanadorPorPenales(EquipoDto? dto, Equipo? equipoLocal, Equipo? equipoVisitante)
    {
        if (dto == null) return null;
        if (equipoLocal?.Nombre == dto.Nombre) return equipoLocal;
        if (equipoVisitante?.Nombre == dto.Nombre) return equipoVisitante;
        return null;
    }

    private record EquipoDto(int Id, string Nombre, int RankingFIFA, int Confederacion);
    private record EstadioDto(string Nombre);
    private record PartidoDto(int Id, DateTime Fecha, EquipoDto? EquipoLocal, EquipoDto? EquipoVisitante,
        EstadioDto? Estadio, int Fase, int? GolesLocal, int? GolesVisitante, EquipoDto? GanadorPorPenales);
    private record GrupoDto(string Etiqueta, List<EquipoDto> Equipos, List<PartidoDto> Partidos);
}
