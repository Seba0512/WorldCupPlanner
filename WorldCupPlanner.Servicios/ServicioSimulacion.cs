using WorldCupPlanner.Dominio.Entidades;

using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.Servicios;

public class ServicioSimulacion
{
    private const int MultiplicadorSemillaPartido = 397;
    private const int GolesMinimos = 0;
    private const int GolesMaximosExclusivo = 6;
    private const int GolesBaseMaximosExclusivo = 4;
    private const int RankingBajo = 1000;
    private const int RankingAlto = 1800;
    private const int BonusRankingMedio = 1;
    private const int BonusRankingAlto = 2;
    private readonly ServicioAuditoria? _auditoria;

    public ServicioSimulacion(ServicioAuditoria? auditoria = null)
    {
        _auditoria = auditoria;
    }

    public void SimularResultados(
        List<Partido> partidos,
        ConfiguracionTorneo configuracionTorneo,
        Usuario? usuario = null)
    {
        var partidosSimulados = 0;

        foreach (var partido in partidos)
        {
            if (partido.EstaJugado())
            {
                continue;
            }

            var random = CrearRandomParaPartido(partido, configuracionTorneo);

            partido.GolesLocal = GenerarGoles(partido.EquipoLocal, random);
            partido.GolesVisitante = GenerarGoles(partido.EquipoVisitante, random);

            if (EsFaseEliminatoria(partido) && partido.EsEmpate())
            {
                DesempatarPartido(partido, random);
            }

            partidosSimulados++;
        }

        RegistrarAuditoriaSimulacion(partidos.Count, partidosSimulados, configuracionTorneo, usuario);
    }

    private void RegistrarAuditoriaSimulacion(
        int partidosRecibidos,
        int partidosSimulados,
        ConfiguracionTorneo configuracionTorneo,
        Usuario? usuario)
    {
        if (_auditoria == null || partidosRecibidos == 0)
        {
            return;
        }

        var usuarioAuditoria = usuario == null
            ? "Sistema"
            : $"{usuario.Nombre} {usuario.Apellido} ({usuario.Email})";

        _auditoria.Registrar(
            usuarioAuditoria,
            "Simulacion",
            $"Semilla simulacion: {configuracionTorneo.SemillaSimulation}. Partidos simulados: {partidosSimulados}/{partidosRecibidos}.");
    }

    private Random CrearRandomParaPartido(
        Partido partido,
        ConfiguracionTorneo configuracionTorneo)
    {
        return new Random(ObtenerSemillaPartido(partido, configuracionTorneo));
    }

    private int ObtenerSemillaPartido(
        Partido partido,
        ConfiguracionTorneo configuracionTorneo)
    {
        return unchecked(configuracionTorneo.SemillaSimulation * MultiplicadorSemillaPartido + partido.Id);
    }

    private bool EsFaseEliminatoria(Partido partido)
    {
        return partido.Fase != FaseTorneo.FaseDeGrupos;
    }

    private int GenerarGoles(Equipo? equipo, Random random)
    {
        var goles = random.Next(GolesMinimos, GolesBaseMaximosExclusivo)
            + ObtenerBonusPorRanking(equipo);

        return Math.Min(goles, GolesMaximosExclusivo - 1);
    }

    private int ObtenerBonusPorRanking(Equipo? equipo)
    {
        if (equipo == null)
        {
            return 0;
        }

        if (equipo.RankingFIFA >= RankingAlto)
        {
            return BonusRankingAlto;
        }

        if (equipo.RankingFIFA >= RankingBajo)
        {
            return BonusRankingMedio;
        }

        return 0;
    }

    private void DesempatarPartido(Partido partido, Random random)
    {
        partido.GanadorPorPenales = random.Next(2) == 0 ? partido.EquipoLocal : partido.EquipoVisitante;
    }
}
