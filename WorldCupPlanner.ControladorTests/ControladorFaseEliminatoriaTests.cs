using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Controladores;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ControladorTests;

[TestClass]
public class ControladorFaseEliminatoriaTests
{
    private readonly ServicioConsultasTorneo _consultasTorneo = new();

    [TestMethod]
    public void GenerarFase_PrimeraRondaEliminatoria_DelegaEnOrquestador()
    {
        var contexto = CrearContexto();
        var controlador = new ControladorFaseEliminatoria(contexto.Orquestador);
        var estadoTorneo = new EstadoTorneo();
        estadoTorneo.EstablecerFixture(CrearFixtureFaseGruposCompleto(contexto.Estadios));

        controlador.GenerarFase(FaseTorneo.PrimeraRondaEliminatoria, estadoTorneo);

        var primeraRonda = _consultasTorneo.ObtenerPartidosDeFase(
            estadoTorneo,
            FaseTorneo.PrimeraRondaEliminatoria);
        Assert.AreEqual(16, primeraRonda.Count);
        Assert.AreEqual(16, contexto.RepositorioPartidos.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void GenerarFase_Final_DelegaGeneracionDeFinalYTercerPuesto()
    {
        var contexto = CrearContexto();
        var controlador = new ControladorFaseEliminatoria(contexto.Orquestador);
        var estadoTorneo = new EstadoTorneo();
        estadoTorneo.EstablecerPartidosEliminatorios(CrearPartidosSemifinalesJugados());

        controlador.GenerarFase(FaseTorneo.Final, estadoTorneo);

        Assert.IsTrue(estadoTorneo.PartidosEliminatorios.Any(p => p.Fase == FaseTorneo.Final));
        Assert.IsTrue(estadoTorneo.PartidosEliminatorios.Any(p => p.Fase == FaseTorneo.TercerPuesto));
        Assert.AreEqual(2, contexto.RepositorioPartidos.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void GenerarFase_FaseNoSoportada_PropagaValidacionDelOrquestador()
    {
        var contexto = CrearContexto();
        var controlador = new ControladorFaseEliminatoria(contexto.Orquestador);

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            controlador.GenerarFase(FaseTorneo.FaseDeGrupos, new EstadoTorneo()));
    }

    private static ContextoFaseEliminatoriaControlador CrearContexto()
    {
        var estadios = CrearEstadiosValidos(4);
        var repositorioEstadio = new RepositorioEstadioControladorFaseFake();
        foreach (var estadio in estadios)
        {
            repositorioEstadio.Agregar(estadio);
        }

        var repositorioPartido = new RepositorioPartidoControladorFaseFake();
        var auditoria = new ServicioAuditoria(new RepositorioAuditoriaControladorFaseFake());
        var servicioEstadio = new ServicioEstadio(repositorioEstadio, auditoria);
        var persistencia = new ServicioPersistenciaFixture(
            new RepositorioGrupoControladorFaseFake(),
            repositorioPartido,
            new RepositorioTorneoArchivadoControladorFaseFake());
        var orquestador = new ServicioOrquestadorFaseEliminatoria(
            new ServicioClasificacion(),
            servicioEstadio,
            new ServicioConfiguracionTorneo(),
            new ServicioCruces(),
            persistencia,
            new ServicioConsultasTorneo());

        return new ContextoFaseEliminatoriaControlador(orquestador, estadios, repositorioPartido);
    }

    private static Fixture CrearFixtureFaseGruposCompleto(List<Estadio> estadios)
    {
        var fixture = new ServicioFixture().GenerarFixture(
            CrearEquiposValidos(),
            estadios,
            new ConfiguracionTorneo { SemillaFixture = 1 });

        foreach (var partido in fixture.Partidos)
        {
            partido.GolesLocal = 2;
            partido.GolesVisitante = 0;
        }

        return fixture;
    }

    private static List<Equipo> CrearEquiposValidos()
    {
        var distribucion = new (Confederacion Confederacion, int Cantidad)[]
        {
            (Confederacion.UEFA, 16),
            (Confederacion.CONMEBOL, 7),
            (Confederacion.CONCACAF, 7),
            (Confederacion.CAF, 9),
            (Confederacion.AFC, 8),
            (Confederacion.OFC, 1)
        };

        var equipos = new List<Equipo>();
        var contador = 1;

        foreach (var (confederacion, cantidad) in distribucion)
        {
            for (var i = 0; i < cantidad; i++)
            {
                equipos.Add(new Equipo
                {
                    Id = contador,
                    Nombre = $"Equipo{contador:00}",
                    Confederacion = confederacion,
                    RankingFIFA = 2500 - contador,
                    RankingInicial = 2500 - contador
                });
                contador++;
            }
        }

        return equipos;
    }

    private static List<Estadio> CrearEstadiosValidos(int cantidad) =>
        Enumerable.Range(1, cantidad).Select(i => new Estadio
        {
            Nombre = $"Estadio{i}",
            Ciudad = $"Ciudad{i}",
            Capacidad = 30000
        }).ToList();

    private static List<Partido> CrearPartidosSemifinalesJugados()
    {
        var estadio = new Estadio { Nombre = "Estadio", Ciudad = "Ciudad", Capacidad = 30000 };
        return Enumerable.Range(1, 2).Select(i => new Partido
        {
            Id = 100 + i,
            EquipoLocal = CrearEquipo($"LocalSemifinal{i}", 1000 + i),
            EquipoVisitante = CrearEquipo($"VisitanteSemifinal{i}", 900 + i),
            Estadio = estadio,
            Fecha = new DateTime(2026, 7, i, 14, 0, 0),
            Fase = FaseTorneo.Semifinal,
            GolesLocal = 2,
            GolesVisitante = 0
        }).ToList();
    }

    private static Equipo CrearEquipo(string nombre, int ranking) => new()
    {
        Nombre = nombre,
        RankingFIFA = ranking,
        Confederacion = Confederacion.UEFA
    };
}

internal sealed record ContextoFaseEliminatoriaControlador(
    ServicioOrquestadorFaseEliminatoria Orquestador,
    List<Estadio> Estadios,
    RepositorioPartidoControladorFaseFake RepositorioPartidos);

internal sealed class RepositorioPartidoControladorFaseFake : IRepositorio<Partido>
{
    private readonly List<Partido> _partidos = new();

    public void Agregar(Partido partido) => _partidos.Add(partido);

    public void Actualizar(Partido partido)
    {
    }

    public void Eliminar(Partido partido) => _partidos.Remove(partido);

    public List<Partido> Buscar(Func<Partido, bool> predicado) =>
        _partidos.Where(predicado).ToList();
}

file sealed class RepositorioEstadioControladorFaseFake : IRepositorio<Estadio>
{
    private readonly List<Estadio> _estadios = new();

    public void Agregar(Estadio estadio) => _estadios.Add(estadio);

    public void Actualizar(Estadio estadio)
    {
    }

    public void Eliminar(Estadio estadio) => _estadios.Remove(estadio);

    public List<Estadio> Buscar(Func<Estadio, bool> predicado) =>
        _estadios.Where(predicado).ToList();
}

file sealed class RepositorioGrupoControladorFaseFake : IRepositorio<Grupo>
{
    private readonly List<Grupo> _grupos = new();

    public void Agregar(Grupo grupo) => _grupos.Add(grupo);

    public void Actualizar(Grupo grupo)
    {
    }

    public void Eliminar(Grupo grupo) => _grupos.Remove(grupo);

    public List<Grupo> Buscar(Func<Grupo, bool> predicado) =>
        _grupos.Where(predicado).ToList();
}

file sealed class RepositorioAuditoriaControladorFaseFake : IRepositorioConId<Auditoria>
{
    private readonly List<Auditoria> _auditorias = new();
    private int _siguienteId = 1;

    public int SiguienteId() => _siguienteId++;

    public void Agregar(Auditoria auditoria) => _auditorias.Add(auditoria);

    public void Actualizar(Auditoria auditoria)
    {
    }

    public void Eliminar(Auditoria auditoria) => _auditorias.Remove(auditoria);

    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado) =>
        _auditorias.Where(predicado).ToList();
}

file sealed class RepositorioTorneoArchivadoControladorFaseFake : IRepositorioTorneoArchivado
{
    private TorneoArchivado? _torneoArchivado;

    public void Guardar(TorneoArchivado torneoArchivado) => _torneoArchivado = torneoArchivado;

    public TorneoArchivado? Obtener() => _torneoArchivado;
}

