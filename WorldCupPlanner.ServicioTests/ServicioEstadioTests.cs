using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
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
        _servicio = new ServicioEstadio();
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
    public void EditarEstadio_EstadioValido_SeEditaCorrectamente()
    {
        var estadio = CrearEstadio();
        _servicio.AgregarEstadio(estadio);
        string nombreOriginal = estadio.Nombre;
        estadio.Nombre = "Estadio Olimpico";

        _servicio.EditarEstadio(nombreOriginal, estadio);

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
}
