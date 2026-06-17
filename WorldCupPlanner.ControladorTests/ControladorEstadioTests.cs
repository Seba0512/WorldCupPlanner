using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Controladores;
using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ControladorTests;

[TestClass]
public class ControladorEstadioTests
{
    [TestMethod]
    public void Listar_DevuelveEstadiosDelServicio()
    {
        var repositorio = new RepositorioEstadioControladorFake();
        repositorio.Agregar(CrearEstadio("Estadio A"));
        repositorio.Agregar(CrearEstadio("Estadio B"));
        var controlador = CrearControlador(repositorio);

        var estadios = controlador.Listar();

        Assert.AreEqual(2, estadios.Count);
        Assert.IsTrue(estadios.Any(e => e.Nombre == "Estadio A"));
        Assert.IsTrue(estadios.Any(e => e.Nombre == "Estadio B"));
    }

    [TestMethod]
    public void ListarDTO_DevuelveEstadiosComoDTO()
    {
        var repositorio = new RepositorioEstadioControladorFake();
        repositorio.Agregar(CrearEstadio("Estadio A"));
        repositorio.Agregar(CrearEstadio("Estadio B"));
        var controlador = CrearControlador(repositorio);

        var estadios = controlador.ListarDTO();

        Assert.AreEqual(2, estadios.Count);
        Assert.IsTrue(estadios.Any(e => e.Nombre == "Estadio A"));
        Assert.IsTrue(estadios.Any(e => e.Nombre == "Estadio B"));
    }

    [TestMethod]
    public void Crear_DelegaAltaEnServicio()
    {
        var repositorio = new RepositorioEstadioControladorFake();
        var controlador = CrearControlador(repositorio);

        controlador.Crear(CrearEstadio("Nuevo Estadio"));

        Assert.AreEqual(1, repositorio.Buscar(_ => true).Count);
        Assert.AreEqual("Nuevo Estadio", repositorio.Buscar(_ => true)[0].Nombre);
    }

    [TestMethod]
    public void Crear_DesdeEstadioDTO_DelegaAltaEnServicio()
    {
        var repositorio = new RepositorioEstadioControladorFake();
        var controlador = CrearControlador(repositorio);

        controlador.Crear(new EstadioDTO
        {
            Nombre = "Nuevo Estadio DTO",
            Ciudad = "Montevideo",
            Capacidad = 45000,
            Descripcion = "Descripcion"
        });

        var creado = repositorio.Buscar(_ => true).Single();
        Assert.AreEqual("Nuevo Estadio DTO", creado.Nombre);
        Assert.AreEqual("Descripcion", creado.Descripcion);
    }

    [TestMethod]
    public void Actualizar_DelegaEdicionEnServicio()
    {
        var repositorio = new RepositorioEstadioControladorFake();
        repositorio.Agregar(CrearEstadio("Original"));
        var controlador = CrearControlador(repositorio);
        var actualizado = CrearEstadio("Original");
        actualizado.Ciudad = "Ciudad Actualizada";
        actualizado.Capacidad = 45000;

        controlador.Actualizar("Original", actualizado);

        var persistido = controlador.ObtenerPorNombre("Original");
        Assert.AreEqual("Ciudad Actualizada", persistido.Ciudad);
        Assert.AreEqual(45000, persistido.Capacidad);
    }

    [TestMethod]
    public void Actualizar_DesdeEstadioDTO_DelegaEdicionEnServicio()
    {
        var repositorio = new RepositorioEstadioControladorFake();
        repositorio.Agregar(CrearEstadio("Original"));
        var controlador = CrearControlador(repositorio);

        controlador.Actualizar("Original", new EstadioDTO
        {
            Nombre = "Original",
            Ciudad = "Ciudad DTO",
            Capacidad = 50000,
            Descripcion = "Nueva descripcion"
        });

        var persistido = controlador.ObtenerPorNombre("Original");
        Assert.AreEqual("Ciudad DTO", persistido.Ciudad);
        Assert.AreEqual(50000, persistido.Capacidad);
        Assert.AreEqual("Nueva descripcion", persistido.Descripcion);
    }

    [TestMethod]
    public void Eliminar_DelegaBajaEnServicio()
    {
        var repositorio = new RepositorioEstadioControladorFake();
        var estadio = CrearEstadio("A eliminar");
        repositorio.Agregar(estadio);
        var controlador = CrearControlador(repositorio);

        controlador.Eliminar(estadio);

        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Eliminar_PorNombre_DelegaBajaEnServicio()
    {
        var repositorio = new RepositorioEstadioControladorFake();
        repositorio.Agregar(CrearEstadio("A eliminar"));
        var controlador = CrearControlador(repositorio);

        controlador.Eliminar("A eliminar");

        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void ObtenerParaEdicion_DevuelveCopiaEditable()
    {
        var repositorio = new RepositorioEstadioControladorFake();
        var estadio = CrearEstadio("Centenario");
        repositorio.Agregar(estadio);
        var controlador = CrearControlador(repositorio);

        var copia = controlador.ObtenerParaEdicion("Centenario");

        Assert.AreEqual(estadio.Nombre, copia.Nombre);
        Assert.AreEqual(estadio.Ciudad, copia.Ciudad);
        Assert.AreNotSame(estadio, copia);
    }

    [TestMethod]
    public void ObtenerParaEdicionDTO_DevuelveCopiaComoDTO()
    {
        var repositorio = new RepositorioEstadioControladorFake();
        var estadio = CrearEstadio("Centenario");
        repositorio.Agregar(estadio);
        var controlador = CrearControlador(repositorio);

        var copia = controlador.ObtenerParaEdicionDTO("Centenario");

        Assert.AreEqual(estadio.Nombre, copia.Nombre);
        Assert.AreEqual(estadio.Ciudad, copia.Ciudad);
        Assert.AreEqual(estadio.Capacidad, copia.Capacidad);
    }

    private static ControladorEstadio CrearControlador(IRepositorio<Estadio> repositorio)
    {
        var servicio = new ServicioEstadio(repositorio);
        return new ControladorEstadio(servicio);
    }

    private static Estadio CrearEstadio(string nombre) =>
        FabricaEntidades.CrearEstadio(nombre, capacidad: FabricaEntidades.CapacidadEstadioMinima);
}

file sealed class RepositorioEstadioControladorFake : IRepositorio<Estadio>
{
    private readonly List<Estadio> _estadios = new();

    public void Agregar(Estadio estadio) => _estadios.Add(estadio);

    public void Actualizar(Estadio estadio)
    {
        var indice = _estadios.FindIndex(e =>
            ReferenceEquals(e, estadio) ||
            string.Equals(e.Nombre, estadio.Nombre, StringComparison.OrdinalIgnoreCase));
        if (indice >= 0)
            _estadios[indice] = estadio;
    }

    public void Eliminar(Estadio estadio) => _estadios.Remove(estadio);

    public List<Estadio> Buscar(Func<Estadio, bool> predicado) =>
        _estadios.Where(predicado).ToList();
}

