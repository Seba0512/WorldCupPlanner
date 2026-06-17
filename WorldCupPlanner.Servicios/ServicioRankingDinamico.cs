using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioRankingDinamico
{
    private const int FactorK = 30;
    private const int RankingMinimo = 300;
    private const int RankingMaximo = 2500;
    private const double MultiplicadorFaseDeGrupos = 1.0;
    private const double MultiplicadorEliminatoria = 1.5;
    private const double BaseElo = 10.0;
    private const double DivisorElo = 1000.0;

    private readonly IRepositorio<Equipo>? _repositorioEquipo;
    private readonly IRepositorio<Partido>? _repositorioPartido;
    private readonly ServicioAuditoria? _auditoria;
    private readonly ServicioConsultasTorneo _consultasTorneo;

    public ServicioRankingDinamico()
    {
        _consultasTorneo = new ServicioConsultasTorneo();
    }

    public ServicioRankingDinamico(IRepositorio<Equipo>? repositorioEquipo)
    {
        _repositorioEquipo = repositorioEquipo;
        _consultasTorneo = new ServicioConsultasTorneo();
    }

    public ServicioRankingDinamico(IRepositorio<Equipo>? repositorioEquipo, ServicioAuditoria? auditoria)
    {
        _repositorioEquipo = repositorioEquipo;
        _auditoria = auditoria;
        _consultasTorneo = new ServicioConsultasTorneo();
    }

    public ServicioRankingDinamico(
        IRepositorio<Equipo>? repositorioEquipo,
        IRepositorio<Partido>? repositorioPartido,
        ServicioAuditoria? auditoria,
        ServicioConsultasTorneo? consultasTorneo = null)
    {
        _repositorioEquipo = repositorioEquipo;
        _repositorioPartido = repositorioPartido;
        _auditoria = auditoria;
        _consultasTorneo = consultasTorneo ?? new ServicioConsultasTorneo();
    }

    public double CalcularProbabilidad(int rankingPropio, int rankingRival)
    {
        return 1.0 / (1.0 + Math.Pow(BaseElo, (rankingRival - rankingPropio) / DivisorElo));
    }

    public int ActualizarRanking(int rankingActual, int rankingRival, double resultado, FaseTorneo fase)
    {
        var probabilidad = CalcularProbabilidad(rankingActual, rankingRival);
        var multiplicador = fase == FaseTorneo.FaseDeGrupos ? MultiplicadorFaseDeGrupos : MultiplicadorEliminatoria;
        var rankingSinRedondear = rankingActual + FactorK * (resultado - probabilidad) * multiplicador;
        return Math.Clamp(RedondearRanking(rankingSinRedondear), RankingMinimo, RankingMaximo);
    }

    public void FijarRankingBaseDelTorneo(Usuario? usuario = null)
    {
        if (_repositorioEquipo == null)
            return;

        var equipos = _repositorioEquipo.Buscar(_ => true);
        var actualizados = 0;

        foreach (var equipo in equipos)
        {
            if (equipo.RankingInicial == equipo.RankingFIFA)
                continue;

            equipo.RankingInicial = equipo.RankingFIFA;
            _repositorioEquipo.Actualizar(equipo);
            actualizados++;
        }

        if (actualizados > 0)
        {
            _auditoria?.Registrar(
                ObtenerNombreUsuario(usuario),
                "RankingBaseTorneo",
                $"Ranking base fijado para {actualizados} equipos.");
        }
    }

    public List<CambioRanking> RecalcularRankings(Usuario? usuario = null, int? partidoReferenciaId = null)
    {
        if (_repositorioEquipo == null || _repositorioPartido == null)
            return new List<CambioRanking>();

        var equipos = _repositorioEquipo.Buscar(_ => true);
        var rankingPersistidoAntes = equipos.ToDictionary(ClaveEquipo, e => e.RankingFIFA);
        var rankingInicialPersistidoAntes = equipos.ToDictionary(ClaveEquipo, e => e.RankingInicial);
        var rankingAntesDelPartidoReferencia = new Dictionary<string, int>();

        foreach (var equipo in equipos)
        {
            equipo.RankingInicial = ObtenerRankingInicial(equipo);
            equipo.RankingFIFA = equipo.RankingInicial;
        }

        var partidosJugados = _repositorioPartido.Buscar(p => p.EstaJugado())
            .OrderBy(p => p.Fecha)
            .ThenBy(p => p.Id)
            .ToList();

        foreach (var partido in partidosJugados)
        {
            var local = ResolverEquipo(partido.EquipoLocal, equipos);
            var visitante = ResolverEquipo(partido.EquipoVisitante, equipos);
            if (local == null || visitante == null)
                continue;

            var rankingLocalOriginal = local.RankingFIFA;
            var rankingVisitanteOriginal = visitante.RankingFIFA;
            if (partidoReferenciaId.HasValue && partido.Id == partidoReferenciaId.Value)
            {
                rankingAntesDelPartidoReferencia[ClaveEquipo(local)] = rankingLocalOriginal;
                rankingAntesDelPartidoReferencia[ClaveEquipo(visitante)] = rankingVisitanteOriginal;
            }

            var resultadoLocal = DeterminarResultado(partido.GolesLocal!.Value, partido.GolesVisitante!.Value);

            local.RankingFIFA = ActualizarRanking(rankingLocalOriginal, rankingVisitanteOriginal, resultadoLocal, partido.Fase);
            visitante.RankingFIFA = ActualizarRanking(rankingVisitanteOriginal, rankingLocalOriginal, 1.0 - resultadoLocal, partido.Fase);
        }

        var cambios = new List<CambioRanking>();

        foreach (var equipo in equipos)
        {
            var clave = ClaveEquipo(equipo);
            var rankingAntes = rankingPersistidoAntes[clave];
            var rankingInicialAntes = rankingInicialPersistidoAntes[clave];

            var cambioPersistido = rankingAntes != equipo.RankingFIFA || rankingInicialAntes != equipo.RankingInicial;
            if (cambioPersistido)
                _repositorioEquipo.Actualizar(equipo);

            var rankingAnteriorAuditado = rankingAntesDelPartidoReferencia.GetValueOrDefault(clave, rankingAntes);
            if (cambioPersistido && rankingAnteriorAuditado != equipo.RankingFIFA)
                cambios.Add(new CambioRanking(equipo.Nombre, rankingAnteriorAuditado, equipo.RankingFIFA));
        }

        RegistrarAuditoria(cambios, usuario);
        return cambios;
    }

    public List<CambioRanking> ActualizarRankingPartido(Partido partido, Usuario? usuario = null)
    {
        if (!partido.EstaJugado() || partido.EquipoLocal == null || partido.EquipoVisitante == null)
            return new List<CambioRanking>();

        var rankingLocalOriginal = partido.EquipoLocal.RankingFIFA;
        var rankingVisitanteOriginal = partido.EquipoVisitante.RankingFIFA;
        var resultadoLocal = DeterminarResultado(partido.GolesLocal!.Value, partido.GolesVisitante!.Value);

        partido.EquipoLocal.RankingFIFA = ActualizarRanking(
            rankingLocalOriginal,
            rankingVisitanteOriginal,
            resultadoLocal,
            partido.Fase);
        partido.EquipoVisitante.RankingFIFA = ActualizarRanking(
            rankingVisitanteOriginal,
            rankingLocalOriginal,
            1.0 - resultadoLocal,
            partido.Fase);

        _repositorioEquipo?.Actualizar(partido.EquipoLocal);
        _repositorioEquipo?.Actualizar(partido.EquipoVisitante);

        var cambios = new List<CambioRanking>
        {
            new(partido.EquipoLocal.Nombre, rankingLocalOriginal, partido.EquipoLocal.RankingFIFA),
            new(partido.EquipoVisitante.Nombre, rankingVisitanteOriginal, partido.EquipoVisitante.RankingFIFA)
        };
        RegistrarAuditoria(cambios.Where(c => c.RankingAnterior != c.RankingNuevo), usuario);
        return cambios;
    }

    public void RecalcularRankingsEnMemoria(EstadoTorneo estado)
    {
        var equipos = _consultasTorneo.ObtenerTodosLosEquipos(estado).ToList();
        if (equipos.Count == 0)
            return;

        var rankingBase = ConstruirRankingBase(equipos);
        var rankingActual = ReproducirHistorial(estado, rankingBase);
        AplicarRankings(equipos, rankingBase, rankingActual);
    }

    private static Dictionary<string, int> ConstruirRankingBase(List<Equipo> equipos)
    {
        var rankingBase = new Dictionary<string, int>();
        foreach (var equipo in equipos)
        {
            var clave = ClaveEquipo(equipo);
            if (!rankingBase.ContainsKey(clave))
                rankingBase[clave] = ObtenerRankingInicial(equipo);
        }

        return rankingBase;
    }

    private Dictionary<string, int> ReproducirHistorial(
        EstadoTorneo estado,
        Dictionary<string, int> rankingBase)
    {
        var rankingActual = new Dictionary<string, int>(rankingBase);
        var partidosJugados = _consultasTorneo.ObtenerTodosLosPartidos(estado)
            .Where(p => p.EstaJugado())
            .OrderBy(p => p.Fecha)
            .ThenBy(p => p.Id);

        foreach (var partido in partidosJugados)
            AplicarPartidoEnHistorial(partido, rankingActual);

        return rankingActual;
    }

    private void AplicarPartidoEnHistorial(Partido partido, Dictionary<string, int> rankingActual)
    {
        if (partido.EquipoLocal == null || partido.EquipoVisitante == null)
            return;

        var claveLocal = ClaveEquipo(partido.EquipoLocal);
        var claveVisitante = ClaveEquipo(partido.EquipoVisitante);
        if (!rankingActual.ContainsKey(claveLocal) || !rankingActual.ContainsKey(claveVisitante))
            return;

        var rankingLocal = rankingActual[claveLocal];
        var rankingVisitante = rankingActual[claveVisitante];
        var resultadoLocal = DeterminarResultado(partido.GolesLocal!.Value, partido.GolesVisitante!.Value);

        rankingActual[claveLocal] = ActualizarRanking(rankingLocal, rankingVisitante, resultadoLocal, partido.Fase);
        rankingActual[claveVisitante] = ActualizarRanking(rankingVisitante, rankingLocal, 1.0 - resultadoLocal, partido.Fase);
    }

    private static void AplicarRankings(
        List<Equipo> equipos,
        Dictionary<string, int> rankingBase,
        Dictionary<string, int> rankingActual)
    {
        foreach (var equipo in equipos)
        {
            var clave = ClaveEquipo(equipo);
            equipo.RankingInicial = rankingBase[clave];
            equipo.RankingFIFA = rankingActual[clave];
        }
    }

    public void SincronizarRankingsEnEstado(EstadoTorneo estado)
    {
        if (_repositorioEquipo == null)
            return;

        var equiposActualizados = _repositorioEquipo.Buscar(_ => true);
        foreach (var equipo in _consultasTorneo.ObtenerTodosLosEquipos(estado))
        {
            var actualizado = ResolverEquipo(equipo, equiposActualizados);
            if (actualizado == null)
                continue;
            equipo.RankingFIFA    = actualizado.RankingFIFA;
            equipo.RankingInicial = actualizado.RankingInicial;
        }
    }

    private static int RedondearRanking(double ranking)
    {
        return (int)Math.Floor(ranking + 0.5);
    }

    private static int ObtenerRankingInicial(Equipo equipo)
    {
        return equipo.RankingInicial is >= RankingMinimo and <= RankingMaximo
            ? equipo.RankingInicial
            : equipo.RankingFIFA;
    }

    private static Equipo? ResolverEquipo(Equipo? referencia, List<Equipo> equipos)
    {
        if (referencia == null)
            return null;

        if (referencia.Id > 0)
        {
            var porId = equipos.FirstOrDefault(e => e.Id == referencia.Id);
            if (porId != null)
                return porId;
        }

        return equipos.FirstOrDefault(e =>
            string.Equals(e.Nombre, referencia.Nombre, StringComparison.OrdinalIgnoreCase));
    }

    private static string ClaveEquipo(Equipo equipo)
    {
        return equipo.Id > 0 ? $"id:{equipo.Id}" : $"nombre:{equipo.Nombre.ToUpperInvariant()}";
    }

    private static double DeterminarResultado(int golesLocal, int golesVisitante)
    {
        if (golesLocal > golesVisitante) return 1.0;
        if (golesVisitante > golesLocal) return 0.0;
        return 0.5;
    }

    private void RegistrarAuditoria(IEnumerable<CambioRanking> cambios, Usuario? usuario)
    {
        if (_auditoria == null)
            return;

        var cambiosReales = cambios
            .Where(c => c.RankingAnterior != c.RankingNuevo)
            .ToList();
        if (cambiosReales.Count == 0)
            return;

        var detalle = string.Join("; ", cambiosReales.Take(8)
            .Select(c => $"{c.NombreEquipo}: {c.RankingAnterior} -> {c.RankingNuevo}"));
        if (cambiosReales.Count > 8)
            detalle += $"; +{cambiosReales.Count - 8} cambios";

        _auditoria.Registrar(
            ObtenerNombreUsuario(usuario),
            "RankingDinamico",
            detalle);
    }

    private static string ObtenerNombreUsuario(Usuario? usuario)
    {
        return usuario == null ? "Sistema" : $"{usuario.Nombre} {usuario.Apellido}";
    }
}

public record CambioRanking(string NombreEquipo, int RankingAnterior, int RankingNuevo);
