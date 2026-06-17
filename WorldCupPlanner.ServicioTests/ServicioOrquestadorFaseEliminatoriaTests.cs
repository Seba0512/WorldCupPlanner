using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioOrquestadorFaseEliminatoriaTests
{
    private readonly ServicioConsultasTorneo _consultasTorneo = new();

    [TestMethod]
    public void GenerarFase_FaseDeGruposNoSoportada_LanzaArgumentOutOfRangeException()
    {
        // Arrange
        var orquestador = CrearOrquestador();
        var estadoTorneo = new EstadoTorneo();

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            orquestador.GenerarFase(FaseTorneo.FaseDeGrupos, estadoTorneo));
    }

    [TestMethod]
    public void GenerarFase_PrimeraRondaEliminatoria_Establece16PartidosEnEstado()
    {
        // Arrange
        var estadios = CrearEstadiosValidos(4);
        var orquestador = CrearOrquestador(estadios);
        var estadoTorneo = new EstadoTorneo();
        estadoTorneo.EstablecerFixture(CrearFixtureFaseGruposCompleto(estadios));

        // Act
        orquestador.GenerarFase(FaseTorneo.PrimeraRondaEliminatoria, estadoTorneo);

        // Assert
        Assert.AreEqual(16, _consultasTorneo.ObtenerPartidosDeFase(estadoTorneo, FaseTorneo.PrimeraRondaEliminatoria).Count);
    }

    [TestMethod]
    public void GenerarFase_Octavos_AgregaOchoPartidosAlEstado()
    {
        // Arrange
        var orquestador = CrearOrquestador();
        var estadoTorneo = new EstadoTorneo();
        estadoTorneo.EstablecerPartidosEliminatorios(CrearPartidosPrimeraRondaJugados(16));

        // Act
        orquestador.GenerarFase(FaseTorneo.Octavos, estadoTorneo);

        // Assert
        Assert.AreEqual(8, _consultasTorneo.ObtenerPartidosDeFase(estadoTorneo, FaseTorneo.Octavos).Count);
    }

    [TestMethod]
    public void GenerarFase_Cuartos_AgregaCuatroPartidosAlEstado()
    {
        // Arrange
        var orquestador = CrearOrquestador();
        var estadoTorneo = new EstadoTorneo();
        estadoTorneo.EstablecerPartidosEliminatorios(CrearPartidosOctavosJugados(8));

        // Act
        orquestador.GenerarFase(FaseTorneo.Cuartos, estadoTorneo);

        // Assert
        Assert.AreEqual(4, _consultasTorneo.ObtenerPartidosDeFase(estadoTorneo, FaseTorneo.Cuartos).Count);
    }

    [TestMethod]
    public void GenerarFase_Semifinal_AgregaDosPartidosAlEstado()
    {
        // Arrange
        var orquestador = CrearOrquestador();
        var estadoTorneo = new EstadoTorneo();
        estadoTorneo.EstablecerPartidosEliminatorios(CrearPartidosCuartosJugados(4));

        // Act
        orquestador.GenerarFase(FaseTorneo.Semifinal, estadoTorneo);

        // Assert
        Assert.AreEqual(2, _consultasTorneo.ObtenerPartidosDeFase(estadoTorneo, FaseTorneo.Semifinal).Count);
    }

    [TestMethod]
    public void GenerarFase_Final_AgregaTercerPuestoYFinalAlEstado()
    {
        // Arrange
        var orquestador = CrearOrquestador();
        var estadoTorneo = new EstadoTorneo();
        estadoTorneo.EstablecerPartidosEliminatorios(CrearPartidosSemifinalesJugados(2));

        // Act
        orquestador.GenerarFase(FaseTorneo.Final, estadoTorneo);

        // Assert
        Assert.IsTrue(estadoTorneo.PartidosEliminatorios.Any(p => p.Fase == FaseTorneo.TercerPuesto));
        Assert.IsTrue(estadoTorneo.PartidosEliminatorios.Any(p => p.Fase == FaseTorneo.Final));
    }

    [TestMethod]
    public void GenerarFase_Octavos_PartidosConFaseOctavos()
    {
        // Arrange
        var orquestador = CrearOrquestador();
        var estadoTorneo = new EstadoTorneo();
        estadoTorneo.EstablecerPartidosEliminatorios(CrearPartidosPrimeraRondaJugados(16));

        // Act
        orquestador.GenerarFase(FaseTorneo.Octavos, estadoTorneo);

        // Assert
        Assert.IsTrue(_consultasTorneo.ObtenerPartidosDeFase(estadoTorneo, FaseTorneo.Octavos)
            .All(p => p.Fase == FaseTorneo.Octavos));
    }

    // --- Helpers ---

    private static ServicioOrquestadorFaseEliminatoria CrearOrquestador(List<Estadio>? estadios = null)
    {
        var estadiosDisponibles = estadios ?? CrearEstadiosValidos(4);
        var repoEstadio = new RepositorioEstadioOrqFake();
        foreach (var e in estadiosDisponibles)
            repoEstadio.Agregar(e);
        var repoPartido = new RepositorioPartidoOrqFake();
        var auditoria = new ServicioAuditoria(new RepositorioAuditoriaFake());
        var servicioEstadio = new ServicioEstadio(repoEstadio, auditoria);
        var persistencia = new ServicioPersistenciaFixture(
            new RepositorioGrupoOrqFake(),
            repoPartido,
            new RepositorioTorneoArchivadoOrqFake());
        return new ServicioOrquestadorFaseEliminatoria(
            new ServicioClasificacion(),
            servicioEstadio,
            new ServicioConfiguracionTorneo(),
            new ServicioCruces(),
            persistencia,
            new ServicioConsultasTorneo());
    }

    private static List<Estadio> CrearEstadiosValidos(int cantidad) =>
        Enumerable.Range(1, cantidad).Select(i => new Estadio
        {
            Nombre = $"Estadio{i}",
            Ciudad = $"Ciudad{i}",
            Capacidad = 30000
        }).ToList();

    private static Fixture CrearFixtureFaseGruposCompleto(List<Estadio>? estadios = null)
    {
        var equipos = CrearEquipos48();
        var configuracion = new ConfiguracionTorneo { SemillaFixture = 1 };
        var fixture = new ServicioFixture().GenerarFixture(equipos, estadios ?? CrearEstadiosValidos(4), configuracion);
        foreach (var partido in fixture.Partidos)
        {
            partido.GolesLocal = 2;
            partido.GolesVisitante = 0;
        }
        return fixture;
    }

    private static List<Equipo> CrearEquipos48()
    {
        var distribucion = new (Confederacion Confederacion, int Cantidad)[]
        {
            (Confederacion.UEFA, 16), (Confederacion.CONMEBOL, 7), (Confederacion.CONCACAF, 7),
            (Confederacion.CAF, 9),   (Confederacion.AFC, 8),       (Confederacion.OFC, 1)
        };
        var equipos = new List<Equipo>();
        int n = 1;
        foreach (var (conf, cantidad) in distribucion)
            for (int i = 0; i < cantidad; i++, n++)
                equipos.Add(new Equipo { Id = n, Nombre = $"Equipo{n}", RankingFIFA = 300 + n, Confederacion = conf });
        return equipos;
    }

    private static List<Partido> CrearPartidosPrimeraRondaJugados(int cantidad)
    {
        var estadio = new Estadio { Nombre = "Estadio", Ciudad = "Ciudad", Capacidad = 30000 };
        return Enumerable.Range(1, cantidad).Select(i => new Partido
        {
            Id = IdsFaseEliminatoria.UltimoDeFaseDeGrupos + i,
            EquipoLocal = new Equipo { Nombre = $"Local{i}", RankingFIFA = 1000 + i, Confederacion = Confederacion.UEFA },
            EquipoVisitante = new Equipo { Nombre = $"Visitante{i}", RankingFIFA = 900 + i, Confederacion = Confederacion.CONMEBOL },
            Estadio = estadio,
            Fecha = DateTime.Now,
            Fase = FaseTorneo.PrimeraRondaEliminatoria,
            GolesLocal = 2,
            GolesVisitante = 0
        }).ToList();
    }

    private static List<Partido> CrearPartidosOctavosJugados(int cantidad)
    {
        var estadio = new Estadio { Nombre = "Estadio", Ciudad = "Ciudad", Capacidad = 30000 };
        return Enumerable.Range(1, cantidad).Select(i => new Partido
        {
            Id = IdsFaseEliminatoria.UltimoDePrimeraRonda + i,
            EquipoLocal = new Equipo { Nombre = $"LocalOctavos{i}", RankingFIFA = 1000 + i, Confederacion = Confederacion.UEFA },
            EquipoVisitante = new Equipo { Nombre = $"VisitanteOctavos{i}", RankingFIFA = 900 + i, Confederacion = Confederacion.CONMEBOL },
            Estadio = estadio,
            Fecha = DateTime.Now,
            Fase = FaseTorneo.Octavos,
            GolesLocal = 2,
            GolesVisitante = 0
        }).ToList();
    }

    private static List<Partido> CrearPartidosCuartosJugados(int cantidad)
    {
        var estadio = new Estadio { Nombre = "Estadio", Ciudad = "Ciudad", Capacidad = 30000 };
        return Enumerable.Range(1, cantidad).Select(i => new Partido
        {
            Id = IdsFaseEliminatoria.UltimoDeOctavos + i,
            EquipoLocal = new Equipo { Nombre = $"LocalCuartos{i}", RankingFIFA = 1000 + i, Confederacion = Confederacion.UEFA },
            EquipoVisitante = new Equipo { Nombre = $"VisitanteCuartos{i}", RankingFIFA = 900 + i, Confederacion = Confederacion.CONMEBOL },
            Estadio = estadio,
            Fecha = DateTime.Now,
            Fase = FaseTorneo.Cuartos,
            GolesLocal = 2,
            GolesVisitante = 0
        }).ToList();
    }

    private static List<Partido> CrearPartidosSemifinalesJugados(int cantidad)
    {
        var estadio = new Estadio { Nombre = "Estadio", Ciudad = "Ciudad", Capacidad = 30000 };
        return Enumerable.Range(1, cantidad).Select(i => new Partido
        {
            Id = IdsFaseEliminatoria.UltimoDeCuartos + i,
            EquipoLocal = new Equipo { Nombre = $"LocalSemifinal{i}", RankingFIFA = 1000 + i, Confederacion = Confederacion.UEFA },
            EquipoVisitante = new Equipo { Nombre = $"VisitanteSemifinal{i}", RankingFIFA = 900 + i, Confederacion = Confederacion.CONMEBOL },
            Estadio = estadio,
            Fecha = DateTime.Now,
            Fase = FaseTorneo.Semifinal,
            GolesLocal = 2,
            GolesVisitante = 0
        }).ToList();
    }
}

file sealed class RepositorioEstadioOrqFake : IRepositorio<Estadio>
{
    private readonly List<Estadio> _estadios = new();
    public void Agregar(Estadio estadio) => _estadios.Add(estadio);
    public void Actualizar(Estadio estadio) { }
    public void Eliminar(Estadio estadio) => _estadios.Remove(estadio);
    public List<Estadio> Buscar(Func<Estadio, bool> predicado) => _estadios.Where(predicado).ToList();
}

file sealed class RepositorioPartidoOrqFake : IRepositorio<Partido>
{
    private readonly List<Partido> _partidos = new();
    public void Agregar(Partido partido) => _partidos.Add(partido);
    public void Actualizar(Partido partido) { }
    public void Eliminar(Partido partido) => _partidos.Remove(partido);
    public List<Partido> Buscar(Func<Partido, bool> predicado) => _partidos.Where(predicado).ToList();
}

file sealed class RepositorioGrupoOrqFake : IRepositorio<Grupo>
{
    private readonly List<Grupo> _grupos = new();
    public void Agregar(Grupo grupo) => _grupos.Add(grupo);
    public void Actualizar(Grupo grupo) { }
    public void Eliminar(Grupo grupo) => _grupos.Remove(grupo);
    public List<Grupo> Buscar(Func<Grupo, bool> predicado) => _grupos.Where(predicado).ToList();
}

file sealed class RepositorioTorneoArchivadoOrqFake : IRepositorioTorneoArchivado
{
    private TorneoArchivado? _archivo;
    public void Guardar(TorneoArchivado torneoArchivado) => _archivo = torneoArchivado;
    public TorneoArchivado? Obtener() => _archivo;
}
