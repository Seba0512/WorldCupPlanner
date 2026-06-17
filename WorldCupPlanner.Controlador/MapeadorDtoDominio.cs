using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Controladores;

internal static class MapeadorDtoDominio
{
    public static Equipo AEquipo(EquipoDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new Equipo
        {
            Nombre = dto.Nombre,
            RankingFIFA = dto.RankingFIFA,
            Confederacion = dto.Confederacion,
            Bandera = dto.Bandera
        };
    }

    public static Estadio AEstadio(EstadioDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new Estadio
        {
            Nombre = dto.Nombre,
            Ciudad = dto.Ciudad,
            Capacidad = dto.Capacidad,
            Descripcion = dto.Descripcion
        };
    }

    public static ActualizacionUsuario AActualizacionUsuario(ActualizacionUsuarioDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new ActualizacionUsuario
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Email = dto.Email,
            FechaNacimiento = dto.FechaNacimiento,
            EjecutadoPor = dto.EjecutadoPor
        };
    }

    public static Auditoria AAuditoria(AuditoriaDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return string.IsNullOrWhiteSpace(dto.Timestamp)
            ? new Auditoria(dto.Id, dto.Usuario, dto.Accion, dto.Detalle)
            : new Auditoria(dto.Id, dto.Timestamp, dto.Usuario, dto.Accion, dto.Detalle);
    }

    public static ConfiguracionTorneo AConfiguracionTorneo(ConfiguracionTorneoDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new ConfiguracionTorneo
        {
            SemillaFixture = dto.SemillaFixture,
            SemillaCompletar = dto.SemillaCompletar,
            SemillaCrucesFase = dto.SemillaCrucesFase,
            SemillaSimulation = dto.SemillaSimulation,
            MotorSimulacion = dto.MotorSimulacion,
            FechaInicioTorneo = dto.FechaInicioTorneo,
            MaxPartidosPorDia = dto.MaxPartidosPorDia,
            SeparacionEntreFechas = dto.SeparacionEntreFechas,
            HoraInicioPartidos = dto.HoraInicioPartidos,
            SeparacionHorasEntrePartidos = dto.SeparacionHorasEntrePartidos
        };
    }

    public static Fixture AFixture(FixtureDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var equiposPorNombre = new Dictionary<string, Equipo>();

        return new Fixture
        {
            Grupos = dto.Grupos
                .Select(grupoDto => AGrupo(grupoDto, equiposPorNombre))
                .ToList()
        };
    }

    private static Grupo AGrupo(GrupoDTO dto, Dictionary<string, Equipo> equiposPorNombre)
    {
        var equipos = dto.Equipos
            .Select(equipoDto => MaterializarEquipo(equipoDto, equiposPorNombre))
            .OfType<Equipo>()
            .ToList();
        var grupo = new Grupo(dto.Etiqueta, equipos);

        grupo.Partidos.AddRange(dto.Partidos.Select(partidoDto => APartido(partidoDto, equiposPorNombre, grupo)));

        return grupo;
    }

    public static Partido APartido(PartidoDTO dto, Dictionary<string, Equipo>? equiposPorNombre = null, Grupo? grupo = null)
    {
        ArgumentNullException.ThrowIfNull(dto);
        equiposPorNombre ??= new Dictionary<string, Equipo>();

        return new Partido
        {
            Id = dto.Id,
            Fecha = dto.Fecha,
            Estadio = dto.Estadio == null ? null : AEstadio(dto.Estadio),
            EquipoLocal = MaterializarEquipo(dto.EquipoLocal, equiposPorNombre),
            EquipoVisitante = MaterializarEquipo(dto.EquipoVisitante, equiposPorNombre),
            Fase = dto.Fase,
            GolesLocal = dto.GolesLocal,
            GolesVisitante = dto.GolesVisitante,
            GanadorPorPenales = MaterializarEquipo(dto.GanadorPorPenales, equiposPorNombre),
            Grupo = grupo ?? CrearGrupoDesdeEtiqueta(dto.GrupoEtiqueta),
            Incidencias = MapearIncidencias(dto.Incidencias, equiposPorNombre)
        };
    }

    private static Equipo? MaterializarEquipo(EquipoDTO? dto, Dictionary<string, Equipo> equiposPorNombre)
    {
        if (dto == null)
            return null;

        if (equiposPorNombre.TryGetValue(dto.Nombre, out var existente))
            return existente;

        var equipo = AEquipo(dto);
        equiposPorNombre[equipo.Nombre] = equipo;
        return equipo;
    }

    private static List<Incidencia> MapearIncidencias(
        IEnumerable<IncidenciaDTO> dtos,
        Dictionary<string, Equipo> equiposPorNombre) =>
        dtos.Select(dto => AIncidencia(dto, equiposPorNombre)).ToList();

    private static Incidencia AIncidencia(IncidenciaDTO dto, Dictionary<string, Equipo> equiposPorNombre)
    {
        Incidencia incidencia = dto.Tipo == TipoTarjeta.Roja
            ? new TarjetaRoja()
            : new TarjetaAmarilla();

        incidencia.Id = dto.Id;
        incidencia.MinutoDelPartido = dto.Minuto;
        incidencia.PartidoId = dto.PartidoId;
        incidencia.Equipo = MaterializarEquipo(dto.Equipo, equiposPorNombre);

        return incidencia;
    }

    private static Grupo? CrearGrupoDesdeEtiqueta(string? etiqueta) =>
        string.IsNullOrWhiteSpace(etiqueta)
            ? null
            : new Grupo(etiqueta);
}
