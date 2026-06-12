using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioRankingDinamicoTests
{
    [TestMethod]
    public void CalcularProbabilidad_CuandoRankingsIguales_RetornaCincoPorciento()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act
        var resultado = servicio.CalcularProbabilidad(1000, 1000);
        // Assert
        Assert.AreEqual(0.5, resultado, 0.001);
    }

    [TestMethod]
    public void CalcularProbabilidad_CuandoEquipoTieneMayorRanking_RetornaMayorA0punto5()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act
        var resultado = servicio.CalcularProbabilidad(1500, 1000);
        // Assert
        Assert.IsTrue(resultado > 0.5);
    }

    [TestMethod]
    public void CalcularProbabilidad_CuandoEquipoTieneMenorRanking_RetornaMenorA0punto5()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act
        var resultado = servicio.CalcularProbabilidad(1000, 1500);
        // Assert
        Assert.IsTrue(resultado < 0.5);
    }

    [TestMethod]
    public void ActualizarRanking_CuandoGanaEquipoDebil_AumentaRanking()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act
        var rankingNuevo = servicio.ActualizarRanking(1200, 1500, 1.0, FaseTorneo.FaseDeGrupos);
        // Assert
        Assert.IsTrue(rankingNuevo > 1200);
    }

    [TestMethod]
    public void ActualizarRanking_CuandoPierdeEquipoFuerte_DisminuyeRanking()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act
        var rankingNuevo = servicio.ActualizarRanking(1500, 1200, 0.0, FaseTorneo.FaseDeGrupos);
        // Assert
        Assert.IsTrue(rankingNuevo < 1500);
    }

    [TestMethod]
    public void ActualizarRanking_CuandoEmpate_AjustaSegunProbabilidad()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act
        var rankingNuevo = servicio.ActualizarRanking(1500, 1200, 0.5, FaseTorneo.FaseDeGrupos);
        // Assert 
        Assert.IsTrue(rankingNuevo < 1500);
    }

    [TestMethod]
    public void ActualizarRanking_CuandoFaseEliminatoria_AplicaMultiplicador1punto5()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act
        var rankingGrupos = servicio.ActualizarRanking(1000, 1000, 1.0, FaseTorneo.FaseDeGrupos);
        var rankingElim = servicio.ActualizarRanking(1000, 1000, 1.0, FaseTorneo.Octavos);
        // Assert 
        Assert.IsTrue(rankingElim > rankingGrupos);
    }

    [TestMethod]
    public void ActualizarRanking_CuandoFaseDeGrupos_AplicaMultiplicador1punto0()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act 
        var rankingNuevo = servicio.ActualizarRanking(1000, 1000, 1.0, FaseTorneo.FaseDeGrupos);
        // Assert
        Assert.AreEqual(1015, rankingNuevo);
    }

    [TestMethod]
    public void ActualizarRanking_CuandoResultadoSupera2500_RetornaClampeadoA2500()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act 
        var rankingNuevo = servicio.ActualizarRanking(2490, 2490, 1.0, FaseTorneo.FaseDeGrupos);
        // Assert
        Assert.AreEqual(2500, rankingNuevo);
    }

    [TestMethod]
    public void ActualizarRanking_CuandoResultadoBajaDe300_RetornaClampeadoA300()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act 
        var rankingNuevo = servicio.ActualizarRanking(310, 310, 0.0, FaseTorneo.FaseDeGrupos);
        // Assert
        Assert.AreEqual(300, rankingNuevo);
    }

    [TestMethod]
    public void ActualizarRanking_CuandoDecimalEsExactamentePunto5_RedondeaHaciaArriba()
    {
        var servicio = new ServicioRankingDinamico();
        // Act
        var rankingNuevo = servicio.ActualizarRanking(1000, 1000, 1.0, FaseTorneo.Octavos);
        // Assert
        Assert.AreEqual(1023, rankingNuevo);
    }

    [TestMethod]
    public void ActualizarRanking_CuandoDecimalNoLlegaAPunto5_RedondeaAlEnteroMasCercano()
    {
        var servicio = new ServicioRankingDinamico();

        var rankingNuevo = servicio.ActualizarRanking(1500, 1501, 0.5, FaseTorneo.FaseDeGrupos);

        Assert.AreEqual(1500, rankingNuevo);
    }

    [TestMethod]
    public void ActualizarRankingPartido_CuandoPartidoJugado_ActualizaRankingDeAmbosEquipos()
    {
        // Arrange
        var local = new Equipo { Nombre = "LocalFC", RankingFIFA = 1200, Confederacion = Confederacion.UEFA };
        var visitante = new Equipo { Nombre = "VisitanteFC", RankingFIFA = 1200, Confederacion = Confederacion.UEFA };
        var partido = new Partido { EquipoLocal = local, EquipoVisitante = visitante, GolesLocal = 2, GolesVisitante = 1, Fase = FaseTorneo.FaseDeGrupos };
        var repo = new RepositorioEquipoRankingStub();
        repo.Agregar(local);
        repo.Agregar(visitante);
        var servicio = new ServicioRankingDinamico(repo);
        // Act
        servicio.ActualizarRankingPartido(partido);
        // Assert
        Assert.AreNotEqual(1200, local.RankingFIFA);
        Assert.AreNotEqual(1200, visitante.RankingFIFA);
    }

    [TestMethod]
    public void ActualizarRankingPartido_CuandoPartidoNoJugado_NoActualizaRanking()
    {
        // Arrange
        var local = new Equipo { Nombre = "LocalFC", RankingFIFA = 1200, Confederacion = Confederacion.UEFA };
        var visitante = new Equipo { Nombre = "VisitanteFC", RankingFIFA = 1200, Confederacion = Confederacion.UEFA };
        var partido = new Partido { EquipoLocal = local, EquipoVisitante = visitante, Fase = FaseTorneo.FaseDeGrupos };
        var repo = new RepositorioEquipoRankingStub();
        repo.Agregar(local);
        repo.Agregar(visitante);
        var servicio = new ServicioRankingDinamico(repo);
        // Act
        servicio.ActualizarRankingPartido(partido);
        // Assert
        Assert.AreEqual(1200, local.RankingFIFA);
        Assert.AreEqual(1200, visitante.RankingFIFA);
    }

    [TestMethod]
    public void ActualizarRankingPartido_CuandoPartidoJugado_RegistraAuditoriaConValoresAntesYDespues()
    {
        // Arrange
        var local = new Equipo { Nombre = "LocalFC", RankingFIFA = 1200, Confederacion = Confederacion.UEFA };
        var visitante = new Equipo { Nombre = "VisitanteFC", RankingFIFA = 1200, Confederacion = Confederacion.UEFA };
        var partido = new Partido { EquipoLocal = local, EquipoVisitante = visitante, GolesLocal = 2, GolesVisitante = 1, Fase = FaseTorneo.FaseDeGrupos };
        var repoEquipo = new RepositorioEquipoRankingStub();
        repoEquipo.Agregar(local);
        repoEquipo.Agregar(visitante);
        var repoAuditoria = new RepositorioAuditoriaRankingStub();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var servicio = new ServicioRankingDinamico(repoEquipo, auditoria);
        // Act
        servicio.ActualizarRankingPartido(partido);
        // Assert 
        var logs = repoAuditoria.Buscar(_ => true);
        Assert.AreEqual(1, logs.Count);
        Assert.IsTrue(logs[0].Detalle.Contains("LocalFC") && logs[0].Detalle.Contains("->"));
        Assert.IsTrue(logs[0].Detalle.Contains("VisitanteFC") && logs[0].Detalle.Contains("->"));
    }

    [TestMethod]
    public void RecalcularRankings_CuandoSeCorrigePartidoAnterior_RecalculaPosterioresEnCascada()
    {
        var equipoA = CrearEquipo(1, "EquipoA", 1500);
        var equipoB = CrearEquipo(2, "EquipoB", 1200);
        var equipoC = CrearEquipo(3, "EquipoC", 1000);
        var repoEquipo = new RepositorioEquipoRankingStub();
        repoEquipo.Agregar(equipoA);
        repoEquipo.Agregar(equipoB);
        repoEquipo.Agregar(equipoC);
        var partidoUno = new Partido
        {
            Id = 1,
            Fecha = new DateTime(2026, 6, 1),
            EquipoLocal = equipoA,
            EquipoVisitante = equipoB,
            GolesLocal = 1,
            GolesVisitante = 0,
            Fase = FaseTorneo.FaseDeGrupos
        };
        var partidoDos = new Partido
        {
            Id = 2,
            Fecha = new DateTime(2026, 6, 2),
            EquipoLocal = equipoA,
            EquipoVisitante = equipoC,
            GolesLocal = 1,
            GolesVisitante = 0,
            Fase = FaseTorneo.FaseDeGrupos
        };
        var repoPartido = new RepositorioPartidoRankingStub();
        repoPartido.Agregar(partidoUno);
        repoPartido.Agregar(partidoDos);
        var servicio = new ServicioRankingDinamico(repoEquipo, repoPartido, null);

        servicio.RecalcularRankings();
        partidoUno.GolesLocal = 0;
        partidoUno.GolesVisitante = 1;
        servicio.RecalcularRankings();

        var rankingAtrasPrimerPartidoA = servicio.ActualizarRanking(1500, 1200, 0.0, FaseTorneo.FaseDeGrupos);
        var rankingEsperadoB = servicio.ActualizarRanking(1200, 1500, 1.0, FaseTorneo.FaseDeGrupos);
        var rankingEsperadoA = servicio.ActualizarRanking(rankingAtrasPrimerPartidoA, 1000, 1.0, FaseTorneo.FaseDeGrupos);
        var rankingEsperadoC = servicio.ActualizarRanking(1000, rankingAtrasPrimerPartidoA, 0.0, FaseTorneo.FaseDeGrupos);

        Assert.AreEqual(rankingEsperadoA, equipoA.RankingFIFA);
        Assert.AreEqual(rankingEsperadoB, equipoB.RankingFIFA);
        Assert.AreEqual(rankingEsperadoC, equipoC.RankingFIFA);
    }

    [TestMethod]
    public void RecalcularRankings_CuandoSeEditaPartidoYaJugado_AuditaDesdeRankingPrevioAlPartido()
    {
        var caf02 = CrearEquipo(1, "CAF_02", 2458);
        var caf07 = CrearEquipo(2, "CAF_07", 1860);
        var repoEquipo = new RepositorioEquipoRankingStub();
        repoEquipo.Agregar(caf02);
        repoEquipo.Agregar(caf07);

        var partido = new Partido
        {
            Id = 263,
            Fecha = new DateTime(2026, 7, 29, 14, 0, 0),
            EquipoLocal = caf02,
            EquipoVisitante = caf07,
            GolesLocal = 5,
            GolesVisitante = 4,
            Fase = FaseTorneo.TercerPuesto
        };
        var repoPartido = new RepositorioPartidoRankingStub();
        repoPartido.Agregar(partido);
        var repoAuditoria = new RepositorioAuditoriaRankingStub();
        var servicio = new ServicioRankingDinamico(
            repoEquipo,
            repoPartido,
            new ServicioAuditoria(repoAuditoria));

        servicio.RecalcularRankings(null, partido.Id);
        var rankingLocalTrasResultadoOriginal = caf02.RankingFIFA;

        partido.GolesLocal = 4;
        partido.GolesVisitante = 4;
        partido.GanadorPorPenales = caf07;
        servicio.RecalcularRankings(null, partido.Id);

        var rankingEsperadoLocal = servicio.ActualizarRanking(2458, 1860, 0.5, FaseTorneo.TercerPuesto);
        var rankingEsperadoVisitante = servicio.ActualizarRanking(1860, 2458, 0.5, FaseTorneo.TercerPuesto);
        var ultimoLog = repoAuditoria.Buscar(_ => true).Last();

        Assert.AreEqual(rankingEsperadoLocal, caf02.RankingFIFA);
        Assert.AreEqual(rankingEsperadoVisitante, caf07.RankingFIFA);
        StringAssert.Contains(ultimoLog.Detalle, $"CAF_02: 2458 -> {rankingEsperadoLocal}");
        Assert.IsFalse(ultimoLog.Detalle.Contains($"CAF_02: {rankingLocalTrasResultadoOriginal} ->"));
    }

    private static Equipo CrearEquipo(int id, string nombre, int ranking) => new()
    {
        Id = id,
        Nombre = nombre,
        RankingFIFA = ranking,
        RankingInicial = ranking,
        Confederacion = Confederacion.UEFA
    };
}

file sealed class RepositorioEquipoRankingStub : IRepositorio<Equipo>
{
    private readonly List<Equipo> _equipos = new();

    public void Agregar(Equipo equipo) => _equipos.Add(equipo);

    public void Actualizar(Equipo equipo)
    {
        var indice = _equipos.FindIndex(e =>
            ReferenceEquals(e, equipo) ||
            (equipo.Id > 0 && e.Id == equipo.Id) ||
            string.Equals(e.Nombre, equipo.Nombre, StringComparison.OrdinalIgnoreCase));
        if (indice != -1)
            _equipos[indice] = equipo;
    }

    public void Eliminar(Equipo equipo) => _equipos.Remove(equipo);

    public List<Equipo> Buscar(Func<Equipo, bool> predicado)
        => _equipos.Where(predicado).ToList();
}

file sealed class RepositorioPartidoRankingStub : IRepositorio<Partido>
{
    private readonly List<Partido> _partidos = new();

    public void Agregar(Partido partido) => _partidos.Add(partido);
    public void Actualizar(Partido partido) { }
    public void Eliminar(Partido partido) => _partidos.Remove(partido);
    public List<Partido> Buscar(Func<Partido, bool> predicado)
        => _partidos.Where(predicado).ToList();
}

file sealed class RepositorioAuditoriaRankingStub : IRepositorioConId<Auditoria>
{
    private readonly List<Auditoria> _logs = new();
    private int _contador;

    public int SiguienteId() => ++_contador;
    public void Agregar(Auditoria entidad) => _logs.Add(entidad);
    public void Actualizar(Auditoria entidad) { }
    public void Eliminar(Auditoria entidad) => _logs.Remove(entidad);
    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado)
        => _logs.Where(predicado).ToList();
}
