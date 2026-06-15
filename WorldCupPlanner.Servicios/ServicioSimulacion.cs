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
    private const int RankingMinimo = 300;
    private const int RankingMaximo = 2500;
    private const double PesoRanking = 0.65;
    private const double PesoForma = 0.35;
    private const double FormaNeutral = 0.5;
    private const int MinutoMinimoTarjeta = 1;
    private const int MinutoMaximoTarjetaExclusivo = 91;
    private const int AmarillasMaximasExclusivo = 4;
    private const double ProbabilidadAmarillaExtra = 0.35;
    private const double ProbabilidadRoja = 0.12;
    private const double PuntosPorPartidoForma = 3.0;
    private const double OffsetDiferenciaGoles = 3.0;
    private const double RangoDiferenciaGoles = 6.0;
    private const double PesoRatioPuntos = 0.55;
    private const double PesoRatioDiferencia = 0.30;
    private const double PesoRatioGoles = 0.15;
    private readonly ServicioAuditoria? _auditoria;

    public ServicioSimulacion(ServicioAuditoria? auditoria = null)
    {
        _auditoria = auditoria;
    }

    public IReadOnlyList<MotorSimulacionDisponible> ObtenerMotoresDisponibles()
    {
        return new List<MotorSimulacionDisponible>
        {
            new(MotorSimulacion.RankingActualYSemilla, ObtenerNombreMotor(MotorSimulacion.RankingActualYSemilla)),
            new(MotorSimulacion.RankingYForma, ObtenerNombreMotor(MotorSimulacion.RankingYForma)),
            new(MotorSimulacion.AleatorioPuro, ObtenerNombreMotor(MotorSimulacion.AleatorioPuro))
        };
    }

    public string ObtenerNombreMotor(MotorSimulacion motor)
    {
        return motor switch
        {
            MotorSimulacion.RankingYForma => "Ranking + Forma",
            MotorSimulacion.AleatorioPuro => "Aleatorio Puro",
            _ => "Ranking actual + semilla"
        };
    }

    public void SimularResultados(
        List<Partido> partidos,
        ConfiguracionTorneo configuracionTorneo,
        Usuario? usuario = null,
        IEnumerable<Partido>? contextoPartidos = null)
    {
        var contexto = (contextoPartidos ?? partidos).ToList();
        var partidosSimulados = 0;

        foreach (var partido in partidos)
        {
            if (partido.EstaJugado())
            {
                continue;
            }

            var random = CrearRandomParaPartido(partido, configuracionTorneo);

            if (configuracionTorneo.MotorSimulacion == MotorSimulacion.AleatorioPuro)
            {
                SimularAleatorioPuro(partido, random);
            }
            else if (configuracionTorneo.MotorSimulacion == MotorSimulacion.RankingYForma)
            {
                SimularRankingYForma(partido, random, contexto);
            }
            else
            {
                partido.GolesLocal = GenerarGoles(partido.EquipoLocal, random);
                partido.GolesVisitante = GenerarGoles(partido.EquipoVisitante, random);
            }

            if (EsFaseEliminatoria(partido) && partido.EsEmpate())
            {
                DesempatarPartido(partido, random);
            }

            SimularIncidencias(partido, random);
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
            $"Motor: {ObtenerNombreMotor(configuracionTorneo.MotorSimulacion)}. Semilla simulacion: {configuracionTorneo.SemillaSimulation}. Partidos simulados: {partidosSimulados}/{partidosRecibidos}.");
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

    private void SimularAleatorioPuro(Partido partido, Random random)
    {
        partido.GolesLocal = random.Next(GolesMinimos, GolesMaximosExclusivo);
        partido.GolesVisitante = random.Next(GolesMinimos, GolesMaximosExclusivo);
    }

    private void SimularRankingYForma(
        Partido partido,
        Random random,
        IReadOnlyCollection<Partido> contextoPartidos)
    {
        var fuerzaLocal = CalcularFuerzaRankingYForma(partido.EquipoLocal, contextoPartidos);
        var fuerzaVisitante = CalcularFuerzaRankingYForma(partido.EquipoVisitante, contextoPartidos);

        partido.GolesLocal = GenerarGolesPorFuerza(fuerzaLocal, fuerzaVisitante, random);
        partido.GolesVisitante = GenerarGolesPorFuerza(fuerzaVisitante, fuerzaLocal, random);
    }

    private int GenerarGolesPorFuerza(double fuerzaPropia, double fuerzaRival, Random random)
    {
        var goles = random.Next(GolesMinimos, GolesBaseMaximosExclusivo);
        var ventaja = fuerzaPropia - fuerzaRival;

        if (fuerzaPropia >= 0.70)
        {
            goles++;
        }

        if (ventaja >= 0.18 && random.NextDouble() < 0.55)
        {
            goles++;
        }

        if (ventaja <= -0.24 && random.NextDouble() < 0.45)
        {
            goles--;
        }

        if (fuerzaPropia <= 0.30 && random.NextDouble() < 0.30)
        {
            goles--;
        }

        return Math.Clamp(goles, GolesMinimos, GolesMaximosExclusivo - 1);
    }

    private double CalcularFuerzaRankingYForma(
        Equipo? equipo,
        IReadOnlyCollection<Partido> contextoPartidos)
    {
        if (equipo == null)
        {
            return 0;
        }

        var ranking = NormalizarRanking(equipo.RankingFIFA);
        var forma = CalcularForma(equipo, contextoPartidos);

        return Math.Clamp(ranking * PesoRanking + forma * PesoForma, 0, 1);
    }

    private double NormalizarRanking(int ranking)
    {
        return Math.Clamp(
            (ranking - RankingMinimo) / (double)(RankingMaximo - RankingMinimo),
            0,
            1);
    }

    private double CalcularForma(Equipo equipo, IReadOnlyCollection<Partido> contextoPartidos)
    {
        var partidosJugados = contextoPartidos
            .Where(p => p.EstaJugado() && ParticipaEquipo(p, equipo))
            .ToList();

        if (!partidosJugados.Any())
            return FormaNeutral;

        var estadisticas = AcumularEstadisticas(partidosJugados, equipo);
        return CalcularRatioForma(estadisticas);
    }

    private EstadisticasForma AcumularEstadisticas(List<Partido> partidos, Equipo equipo)
    {
        var puntos = 0;
        var golesAFavor = 0;
        var golesEnContra = 0;

        foreach (var partido in partidos)
        {
            var esLocal = EsMismoEquipo(partido.EquipoLocal, equipo);
            golesAFavor  += esLocal ? partido.GolesLocal!.Value  : partido.GolesVisitante!.Value;
            golesEnContra += esLocal ? partido.GolesVisitante!.Value : partido.GolesLocal!.Value;
            puntos += CalcularPuntosDeForma(partido, equipo);
        }

        return new EstadisticasForma(puntos, golesAFavor, golesEnContra, partidos.Count);
    }

    private static double CalcularRatioForma(EstadisticasForma e)
    {
        var ratioPuntos      = e.Puntos / (e.CantidadPartidos * PuntosPorPartidoForma);
        var promedioDif      = (e.GolesAFavor - e.GolesEnContra) / (double)e.CantidadPartidos;
        var ratioDiferencia  = Math.Clamp((promedioDif + OffsetDiferenciaGoles) / RangoDiferenciaGoles, 0, 1);
        var ratioGoles       = Math.Clamp(e.GolesAFavor / (e.CantidadPartidos * PuntosPorPartidoForma), 0, 1);
        return ratioPuntos * PesoRatioPuntos + ratioDiferencia * PesoRatioDiferencia + ratioGoles * PesoRatioGoles;
    }

    private int CalcularPuntosDeForma(Partido partido, Equipo equipo)
    {
        var ganador = partido.Ganador();

        if (EsMismoEquipo(ganador, equipo))
        {
            return 3;
        }

        return partido.EsEmpate() ? 1 : 0;
    }

    private bool ParticipaEquipo(Partido partido, Equipo equipo)
    {
        return EsMismoEquipo(partido.EquipoLocal, equipo)
            || EsMismoEquipo(partido.EquipoVisitante, equipo);
    }

    private bool EsMismoEquipo(Equipo? primero, Equipo? segundo)
    {
        if (primero == null || segundo == null)
        {
            return false;
        }

        if (primero.Id > 0 && segundo.Id > 0)
        {
            return primero.Id == segundo.Id;
        }

        return string.Equals(primero.Nombre, segundo.Nombre, StringComparison.OrdinalIgnoreCase);
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

    private void SimularIncidencias(Partido partido, Random random)
    {
        var cantidadInicial = partido.Incidencias.Count;

        SimularTarjetasEquipo(partido, partido.EquipoLocal, random);
        SimularTarjetasEquipo(partido, partido.EquipoVisitante, random);

        if (partido.Incidencias.Count == cantidadInicial)
        {
            var equipo = random.Next(2) == 0 ? partido.EquipoLocal : partido.EquipoVisitante;
            AgregarTarjetaSimulada(partido, equipo, esRoja: false, random);
        }
    }

    private void SimularTarjetasEquipo(Partido partido, Equipo? equipo, Random random)
    {
        if (equipo == null)
        {
            return;
        }

        var amarillas = random.Next(AmarillasMaximasExclusivo);
        if (random.NextDouble() < ProbabilidadAmarillaExtra)
        {
            amarillas++;
        }

        for (var i = 0; i < amarillas; i++)
        {
            AgregarTarjetaSimulada(partido, equipo, esRoja: false, random);
        }

        if (random.NextDouble() < ProbabilidadRoja)
        {
            AgregarTarjetaSimulada(partido, equipo, esRoja: true, random);
        }
    }

    private void AgregarTarjetaSimulada(Partido partido, Equipo? equipo, bool esRoja, Random random)
    {
        if (equipo == null)
        {
            return;
        }

        Incidencia tarjeta = esRoja
            ? new TarjetaRoja()
            : new TarjetaAmarilla();

        tarjeta.PartidoId = partido.Id;
        tarjeta.Equipo = equipo;
        tarjeta.MinutoDelPartido = random.Next(MinutoMinimoTarjeta, MinutoMaximoTarjetaExclusivo);

        partido.Incidencias.Add(tarjeta);
    }
}

internal record EstadisticasForma(int Puntos, int GolesAFavor, int GolesEnContra, int CantidadPartidos);
