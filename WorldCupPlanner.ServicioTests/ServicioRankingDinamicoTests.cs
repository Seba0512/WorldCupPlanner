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
        // Assert — era favorito (prob > 0.5), empate baja levemente
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
        // Assert — mismo resultado pero multiplicado x1.5 → diferencia mayor
        Assert.IsTrue(rankingElim > rankingGrupos);
    }

    [TestMethod]
    public void ActualizarRanking_CuandoFaseDeGrupos_AplicaMultiplicador1punto0()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act — victoria con rankings iguales: K*1*1.0 = 30*0.5 = 15
        var rankingNuevo = servicio.ActualizarRanking(1000, 1000, 1.0, FaseTorneo.FaseDeGrupos);
        // Assert
        Assert.AreEqual(1015, rankingNuevo);
    }

    [TestMethod]
    public void ActualizarRanking_CuandoResultadoSupera2500_RetornaClampeadoA2500()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act — rankings iguales: delta=+15, 2490+15=2505 → clamp 2500
        var rankingNuevo = servicio.ActualizarRanking(2490, 2490, 1.0, FaseTorneo.FaseDeGrupos);
        // Assert
        Assert.AreEqual(2500, rankingNuevo);
    }

    [TestMethod]
    public void ActualizarRanking_CuandoResultadoBajaDe300_RetornaClampeadoA300()
    {
        // Arrange
        var servicio = new ServicioRankingDinamico();
        // Act — rankings iguales: delta=-15, 310-15=295 → clamp 300
        var rankingNuevo = servicio.ActualizarRanking(310, 310, 0.0, FaseTorneo.FaseDeGrupos);
        // Assert
        Assert.AreEqual(300, rankingNuevo);
    }

    [TestMethod]
    public void ActualizarRanking_CuandoDecimalEsExactamentePunto5_RedondeaHaciaArriba()
    {
        // Arrange — buscamos un caso donde 30*(resultado-prob)*mult = X.5
        // Rankings iguales: prob=0.5, resultado=1.0 → 30*0.5*1.0 = 15 (entero, no aplica)
        // Usamos eliminatoria con rankings iguales: 30*0.5*1.5 = 22.5 → debe ser 23
        var servicio = new ServicioRankingDinamico();
        // Act
        var rankingNuevo = servicio.ActualizarRanking(1000, 1000, 1.0, FaseTorneo.Octavos);
        // Assert
        Assert.AreEqual(1023, rankingNuevo);
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
        // Assert — un log por equipo con formato "NombreEquipo rankingAntes -> rankingDespues"
        var logs = repoAuditoria.Buscar(_ => true);
        Assert.AreEqual(2, logs.Count);
        Assert.IsTrue(logs.Any(l => l.Detalle.Contains("LocalFC") && l.Detalle.Contains("->")));
        Assert.IsTrue(logs.Any(l => l.Detalle.Contains("VisitanteFC") && l.Detalle.Contains("->")));
    }
}

file sealed class RepositorioEquipoRankingStub : IRepositorio<Equipo>
{
    private readonly List<Equipo> _equipos = new();

    public void Agregar(Equipo equipo) => _equipos.Add(equipo);

    public void Actualizar(Equipo equipo)
    {
        var indice = _equipos.FindIndex(e => ReferenceEquals(e, equipo));
        if (indice != -1)
            _equipos[indice] = equipo;
    }

    public void Eliminar(Equipo equipo) => _equipos.Remove(equipo);

    public List<Equipo> Buscar(Func<Equipo, bool> predicado)
        => _equipos.Where(predicado).ToList();
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