using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.Servicios;

public class ServicioPartido
{
    private readonly IProveedorPartidos _partidos;
    private readonly HashSet<FaseTorneo> _fasesBloqueadas = new();

    public ServicioPartido(IProveedorPartidos partidos)
    {
        _partidos = partidos;
    }

    public void EditarFechaYEstadio(int id, DateTime fecha, Estadio estadio)
    {
        EditarFecha(id, fecha);
        EditarEstadio(id, estadio);
    }

    public void AplicarResultadoConPenales(int id, ContextoResultadoPartido contexto)
    {
        CargarResultado(id, contexto.GolesLocal, contexto.GolesVisitante, contexto.Notificacion, contexto.Periodistas);
        ResolverEmpateConPenales(id, contexto.SemillaSimulation);
    }

    public void EditarFecha(int idPartido, DateTime nuevaFecha)
    {
        if (nuevaFecha == default)
        {
            throw new EntidadInvalidaExcepcion("La fecha del partido es obligatoria.");
        }

        var partido = ObtenerPartidoPorId(idPartido);
        ValidarPuedeEditarPartido(partido);

        partido.Fecha = nuevaFecha;
        _partidos.Actualizar(partido);
    }

    public void EditarEstadio(int idPartido, Estadio nuevoEstadio)
    {
        if (nuevoEstadio == null || !nuevoEstadio.EsValido())
        {
            throw new EntidadInvalidaExcepcion("El estadio del partido no es valido.");
        }

        var partido = ObtenerPartidoPorId(idPartido);
        ValidarPuedeEditarPartido(partido);

        partido.Estadio = nuevoEstadio;
        _partidos.Actualizar(partido);
    }

    private const int MultiplicadorSemillaPenales = 523;

    public void CargarResultado(int idPartido, int golesLocal, int golesVisitante)
    {
        ValidarGoles(golesLocal, golesVisitante);

        var partido = ObtenerPartidoPorId(idPartido);
        ValidarPuedeEditarPartido(partido);

        partido.GolesLocal = golesLocal;
        partido.GolesVisitante = golesVisitante;
        partido.GanadorPorPenales = null;
        _partidos.Actualizar(partido);
    }

    public void CargarResultado(int idPartido, int golesLocal, int golesVisitante, ServicioNotificacion servicioNotificacion, IEnumerable<Usuario> periodistas)
    {
        CargarResultado(idPartido, golesLocal, golesVisitante);
        var partido = ObtenerPartidoPorId(idPartido);
        servicioNotificacion.GenerarNotificacionesResultado(partido, periodistas);
    }

    public void ResolverEmpateConPenales(int idPartido, int semilla)
    {
        var partido = ObtenerPartidoPorId(idPartido);

        if (!partido.EsEmpate() || partido.Fase == FaseTorneo.FaseDeGrupos)
            return;

        var random = new Random(unchecked(semilla * MultiplicadorSemillaPenales + idPartido));
        partido.GanadorPorPenales = random.Next(2) == 0 ? partido.EquipoLocal : partido.EquipoVisitante;
        _partidos.Actualizar(partido);
    }

    public void BloquearEdicionPartidosDeFaseDeGrupos()
    {
        BloquearEdicionFase(FaseTorneo.FaseDeGrupos);
    }

    public void RegistrarFaseGenerada(FaseTorneo faseGenerada)
    {
        BloquearEdicionFase(ObtenerFaseAnterior(faseGenerada));
    }

    public void BloquearEdicionFase(FaseTorneo fase)
    {
        _fasesBloqueadas.Add(fase);
    }

    public bool EsFaseBloqueada(FaseTorneo fase)
    {
        return HayFasePosteriorGenerada(fase, ObtenerPartidos()) ||
               _fasesBloqueadas.Contains(fase);
    }

    public Partido? BuscarPartidoPorId(int idPartido)
    {
        return _partidos.BuscarPorId(idPartido);
    }

    public List<Partido> ObtenerPartidos()
    {
        return _partidos.ObtenerTodos();
    }

    private Partido ObtenerPartidoPorId(int idPartido)
    {
        var partido = BuscarPartidoPorId(idPartido);

        if (partido == null)
        {
            throw new EntidadInexistenteExcepcion("El partido no existe.");
        }

        return partido;
    }

    private void ValidarPuedeEditarPartido(Partido partido)
    {
        if (EsFaseBloqueada(partido.Fase))
        {
            throw new EntidadInvalidaExcepcion(
                "No se pueden editar partidos de una fase anterior una vez generada la fase siguiente.");
        }
    }

    private void ValidarGoles(int golesLocal, int golesVisitante)
    {
        if (golesLocal < 0 || golesVisitante < 0)
        {
            throw new EntidadInvalidaExcepcion("Los goles del partido no pueden ser negativos.");
        }
    }

    public IEnumerable<string> ObtenerEstadiosDisponibles(IEnumerable<Partido> partidos) =>
        partidos.Select(p => p.Estadio?.Nombre).Where(n => n != null).Distinct().OrderBy(n => n)!;

    public IEnumerable<string> ObtenerGruposDisponibles(IEnumerable<Partido> partidos) =>
        partidos.Select(p => p.Grupo?.Etiqueta).Where(e => e != null).Distinct().OrderBy(e => e)!;

    public IEnumerable<FaseTorneo> ObtenerFasesDisponibles(IEnumerable<Partido> partidos) =>
        partidos.Select(p => p.Fase).Distinct().OrderBy(f => (int)f);

    public IEnumerable<Partido> FiltrarPartidos(IEnumerable<Partido> partidos, FiltroPartidos filtro)
    {
        return partidos
            .Where(p => filtro.Fecha == null || p.Fecha.Date == filtro.Fecha.Value.Date)
            .Where(p => string.IsNullOrEmpty(filtro.Estadio) || p.Estadio?.Nombre == filtro.Estadio)
            .Where(p => string.IsNullOrEmpty(filtro.Grupo) || p.Grupo?.Etiqueta == filtro.Grupo)
            .Where(p => string.IsNullOrEmpty(filtro.Fase) || p.Fase.ToString() == filtro.Fase);
    }

    private FaseTorneo ObtenerFaseAnterior(FaseTorneo faseGenerada)
    {
        return faseGenerada switch
        {
            FaseTorneo.PrimeraRondaEliminatoria => FaseTorneo.FaseDeGrupos,
            FaseTorneo.Octavos => FaseTorneo.PrimeraRondaEliminatoria,
            FaseTorneo.Cuartos => FaseTorneo.Octavos,
            FaseTorneo.Semifinal => FaseTorneo.Cuartos,
            FaseTorneo.TercerPuesto => FaseTorneo.Semifinal,
            FaseTorneo.Final => FaseTorneo.Semifinal,
            _ => throw new EntidadInvalidaExcepcion("La fase generada no tiene una fase anterior para bloquear.")
        };
    }

    private static bool HayFasePosteriorGenerada(FaseTorneo fase, IEnumerable<Partido> partidos)
    {
        return fase switch
        {
            FaseTorneo.FaseDeGrupos => partidos.Any(p => p.Fase != FaseTorneo.FaseDeGrupos),
            FaseTorneo.PrimeraRondaEliminatoria => partidos.Any(p =>
                p.Fase is FaseTorneo.Octavos or FaseTorneo.Cuartos or FaseTorneo.Semifinal or FaseTorneo.TercerPuesto or FaseTorneo.Final),
            FaseTorneo.Octavos => partidos.Any(p =>
                p.Fase is FaseTorneo.Cuartos or FaseTorneo.Semifinal or FaseTorneo.TercerPuesto or FaseTorneo.Final),
            FaseTorneo.Cuartos => partidos.Any(p =>
                p.Fase is FaseTorneo.Semifinal or FaseTorneo.TercerPuesto or FaseTorneo.Final),
            FaseTorneo.Semifinal => partidos.Any(p =>
                p.Fase is FaseTorneo.TercerPuesto or FaseTorneo.Final),
            _ => false
        };
    }
}
