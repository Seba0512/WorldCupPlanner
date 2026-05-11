using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

internal class RepositorioAuditoriaEstadioFake : IRepositorioConId<Auditoria>
{
    private int _nextId = 1;
    public List<Auditoria> Entradas { get; } = [];

    public int SiguienteId() => _nextId++;
    public void Agregar(Auditoria entidad) => Entradas.Add(entidad);
    public void Actualizar(Auditoria entidad) { }
    public void Eliminar(Auditoria entidad) => Entradas.Remove(entidad);
    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado) => Entradas.Where(predicado).ToList();
}

[TestClass]
public class ServicioEstadioTests
{
    private ServicioEstadio _servicio;

    [TestInitialize]
    public void Setup()
    {
        _servicio = new ServicioEstadio(new RepositorioEstadioEnMemoria());
    }

    private static (ServicioEstadio servicio, RepositorioAuditoriaEstadioFake repoAuditoria) CrearServicioConAuditoria()
    {
        var repoAuditoria = new RepositorioAuditoriaEstadioFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var servicio = new ServicioEstadio(new RepositorioEstadioEnMemoria(), auditoria);
        return (servicio, repoAuditoria);
    }

    private static Estadio CrearEstadio(string nombre = "Estadio Centenario", string ciudad = "Montevideo", int capacidad = 60000) => new()
    {
        Nombre = nombre,
        Ciudad = ciudad,
        Capacidad = capacidad
    };

    [TestMethod]
    public void AgregarEstadio_EstadioValido_SeAgregaCorrectamente()
    {
        _servicio.AgregarEstadio(CrearEstadio());

        Assert.AreEqual(1, _servicio.ObtenerEstadios().Count);
    }

    [TestMethod]
    public void AgregarEstadio_NombreDuplicado_LanzaExcepcion()
    {
        _servicio.AgregarEstadio(CrearEstadio());

        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() => _servicio.AgregarEstadio(CrearEstadio(capacidad: 40000)));
    }

    [TestMethod]
    public void AgregarEstadio_EstadioInvalido_LanzaExcepcion()
    {
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.AgregarEstadio(CrearEstadio(nombre: "")));
    }

    [TestMethod]
    public void EliminarEstadio_EstadioExistente_SeEliminaCorrectamente()
    {
        var estadio = CrearEstadio();
        _servicio.AgregarEstadio(estadio);

        _servicio.EliminarEstadio(estadio);

        Assert.AreEqual(0, _servicio.ObtenerEstadios().Count);
    }

    [TestMethod]
    public void EliminarEstadio_EstadioUsadoEnFixture_DebeLanzarExcepcion()
    {
        var estadio = CrearEstadio();
        var fixture = CrearFixtureConEstadio(estadio);
        _servicio.AgregarEstadio(estadio);
        _servicio.RegistrarFixtureGenerado(fixture);

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            _servicio.EliminarEstadio(estadio));
    }

    [TestMethod]
    public void EditarEstadio_EstadioValido_SeEditaCorrectamente()
    {
        var estadio = CrearEstadio();
        _servicio.AgregarEstadio(estadio);
        string nombreOriginal = estadio.Nombre;
        estadio.Nombre = "Estadio Olimpico";

        _servicio.EditarEstadio(nombreOriginal, estadio);

        Assert.AreEqual("Estadio Olimpico", _servicio.ObtenerEstadios().First().Nombre);
    }

    [TestMethod]
    public void EditarEstadio_EstadioNoExistente_LanzaExcepcion()
    {
        var estadio = CrearEstadio();

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            _servicio.EditarEstadio("EstadioQueNoExiste", estadio));
    }

    [TestMethod]
    public void EditarEstadio_ConNuevoObjetoYNombreOriginal_SeEditaCorrectamente()
    {
        // Cubre la rama del predicado donde ReferenceEquals es false
        // pero e.Nombre == nombreOriginal es true (busca por nombre, no por referencia)
        var estadio = CrearEstadio();
        _servicio.AgregarEstadio(estadio);

        var estadioActualizado = new Estadio
        {
            Nombre = "Estadio Olimpico",
            Ciudad = "Buenos Aires",
            Capacidad = 70000
        };

        _servicio.EditarEstadio("Estadio Centenario", estadioActualizado);

        Assert.AreEqual("Estadio Olimpico", _servicio.ObtenerEstadios().First().Nombre);
    }

    // --- Auditoría ---

    [TestMethod]
    public void AgregarEstadio_EstadioValido_RegistraEntradaDeAuditoria()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();

        servicio.AgregarEstadio(CrearEstadio());

        Assert.AreEqual(1, repoAuditoria.Entradas.Count);
        Assert.AreEqual("Alta", repoAuditoria.Entradas[0].Accion);
        StringAssert.Contains(repoAuditoria.Entradas[0].Detalle, "Estadio Centenario");
    }

    [TestMethod]
    public void AgregarEstadio_EstadioInvalido_NoRegistraAuditoria()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => servicio.AgregarEstadio(CrearEstadio(nombre: "")));

        Assert.AreEqual(0, repoAuditoria.Entradas.Count);
    }

    [TestMethod]
    public void EditarEstadio_EstadioValido_RegistraEntradaDeAuditoria()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        var estadio = CrearEstadio();
        servicio.AgregarEstadio(estadio);
        repoAuditoria.Entradas.Clear();

        estadio.Nombre = "Estadio Olimpico";
        servicio.EditarEstadio("Estadio Centenario", estadio);

        Assert.AreEqual(1, repoAuditoria.Entradas.Count);
        Assert.AreEqual("Edicion", repoAuditoria.Entradas[0].Accion);
        StringAssert.Contains(repoAuditoria.Entradas[0].Detalle, "Estadio Centenario");
        StringAssert.Contains(repoAuditoria.Entradas[0].Detalle, "Estadio Olimpico");
    }

    [TestMethod]
    public void EditarEstadio_EstadioInvalido_NoRegistraAuditoria()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        servicio.AgregarEstadio(CrearEstadio());
        repoAuditoria.Entradas.Clear();

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicio.EditarEstadio("Estadio Centenario", CrearEstadio(nombre: "")));

        Assert.AreEqual(0, repoAuditoria.Entradas.Count);
    }

    [TestMethod]
    public void EliminarEstadio_EstadioExistente_RegistraEntradaDeAuditoria()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        var estadio = CrearEstadio();
        servicio.AgregarEstadio(estadio);
        repoAuditoria.Entradas.Clear();

        servicio.EliminarEstadio(estadio);

        Assert.AreEqual(1, repoAuditoria.Entradas.Count);
        Assert.AreEqual("Baja", repoAuditoria.Entradas[0].Accion);
        StringAssert.Contains(repoAuditoria.Entradas[0].Detalle, "Estadio Centenario");
    }

    [TestMethod]
    public void AgregarEstadio_SinAuditoria_NoLanzaExcepcion()
    {
        var servicio = new ServicioEstadio(new RepositorioEstadioEnMemoria());

        servicio.AgregarEstadio(CrearEstadio());

        Assert.AreEqual(1, servicio.ObtenerEstadios().Count);
    }

    [TestMethod]
    public void Auditoria_RegistraUsuarioSistema()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();

        servicio.AgregarEstadio(CrearEstadio());

        Assert.AreEqual("Sistema", repoAuditoria.Entradas[0].Usuario);
    }

    private static Fixture CrearFixtureConEstadio(Estadio estadio)
    {
        var equipoLocal = new Equipo
        {
            Nombre = "Uruguay",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        };
        var equipoVisitante = new Equipo
        {
            Nombre = "Francia",
            RankingFIFA = 1000,
            Confederacion = Confederacion.UEFA
        };
        var grupo = new Grupo("A", new List<Equipo>
        {
            equipoLocal,
            equipoVisitante,
            new Equipo
            {
                Nombre = "Marruecos",
                RankingFIFA = 1000,
                Confederacion = Confederacion.CAF
            },
            new Equipo
            {
                Nombre = "Japon",
                RankingFIFA = 1000,
                Confederacion = Confederacion.AFC
            }
        });

        grupo.Partidos.Add(new Partido
        {
            Id = 1,
            Fecha = new DateTime(2026, 6, 1),
            Estadio = estadio,
            EquipoLocal = equipoLocal,
            EquipoVisitante = equipoVisitante,
            Fase = FaseTorneo.FaseDeGrupos,
            Grupo = grupo
        });

        return new Fixture
        {
            Grupos = { grupo }
        };
    }
}
