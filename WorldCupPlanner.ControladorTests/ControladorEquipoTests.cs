using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Controladores;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ControladorTests;

[TestClass]
public class ControladorEquipoTests
{
    [TestMethod]
    public void Listar_DevuelveEquiposDelServicio()
    {
        var repositorio = new RepositorioEquipoControladorFake();
        repositorio.Agregar(CrearEquipo("Equipo A", Confederacion.UEFA));
        repositorio.Agregar(CrearEquipo("Equipo B", Confederacion.CONMEBOL));
        var controlador = CrearControlador(repositorio);

        var equipos = controlador.Listar();

        Assert.AreEqual(2, equipos.Count);
        Assert.IsTrue(equipos.Any(e => e.Nombre == "Equipo A"));
        Assert.IsTrue(equipos.Any(e => e.Nombre == "Equipo B"));
    }

    [TestMethod]
    public void Crear_DelegaAltaConDtoEnServicio()
    {
        var repositorio = new RepositorioEquipoControladorFake();
        var controlador = CrearControlador(repositorio);

        controlador.Crear(new NuevoEquipoDto
        {
            Nombre = "Nuevo",
            Confederacion = Confederacion.UEFA,
            RankingFIFA = 1500
        });

        var creado = repositorio.Buscar(_ => true).Single();
        Assert.AreEqual("Nuevo", creado.Nombre);
        Assert.AreEqual(1500, creado.RankingFIFA);
        Assert.AreEqual(1500, creado.RankingInicial);
    }

    [TestMethod]
    public void Actualizar_DelegaEdicionEnServicio()
    {
        var repositorio = new RepositorioEquipoControladorFake();
        repositorio.Agregar(CrearEquipo("Original", Confederacion.UEFA));
        var controlador = CrearControlador(repositorio);
        var actualizado = CrearEquipo("Original", Confederacion.UEFA);
        actualizado.RankingFIFA = 1700;

        controlador.Actualizar(actualizado, "Original");

        var persistido = controlador.Buscar(e => e.Nombre == "Original").Single();
        Assert.AreEqual(1700, persistido.RankingFIFA);
        Assert.AreEqual(1700, persistido.RankingInicial);
    }

    [TestMethod]
    public void Eliminar_DelegaBajaEnServicio()
    {
        var repositorio = new RepositorioEquipoControladorFake();
        var equipo = CrearEquipo("A eliminar", Confederacion.CAF);
        repositorio.Agregar(equipo);
        var controlador = CrearControlador(repositorio);

        controlador.Eliminar(equipo);

        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void ObtenerParaEdicion_DevuelveCopiaEditable()
    {
        var repositorio = new RepositorioEquipoControladorFake();
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        repositorio.Agregar(equipo);
        var controlador = CrearControlador(repositorio);

        var copia = controlador.ObtenerParaEdicion("Uruguay");

        Assert.AreEqual(equipo.Nombre, copia.Nombre);
        Assert.AreEqual(equipo.RankingFIFA, copia.RankingFIFA);
        Assert.AreNotSame(equipo, copia);
    }

    [TestMethod]
    public void Helpers_DelegaDatosDeCuposYComplecion()
    {
        var controlador = CrearControlador(new RepositorioEquipoControladorFake());

        Assert.AreEqual(48, controlador.ObtenerMaximoEquipos());
        Assert.AreEqual(46, controlador.ObtenerCantidadFaltante(2));
        Assert.AreEqual(16, controlador.ObtenerCuposMaximos()[Confederacion.UEFA]);
        Assert.AreEqual("Se generaron 2 equipos ficticios correctamente.", controlador.ObtenerMensajeComplecion(2));
    }

    private static ControladorEquipo CrearControlador(IRepositorio<Equipo> repositorio)
    {
        var servicio = new ServicioEquipo(repositorio);
        return new ControladorEquipo(servicio);
    }

    private static Equipo CrearEquipo(string nombre, Confederacion confederacion) => new()
    {
        Nombre = nombre,
        Confederacion = confederacion,
        RankingFIFA = 1200,
        RankingInicial = 1200
    };
}

file sealed class RepositorioEquipoControladorFake : IRepositorio<Equipo>
{
    private readonly List<Equipo> _equipos = new();

    public void Agregar(Equipo equipo) => _equipos.Add(equipo);

    public void Actualizar(Equipo equipo)
    {
        var indice = _equipos.FindIndex(e =>
            ReferenceEquals(e, equipo) ||
            string.Equals(e.Nombre, equipo.Nombre, StringComparison.OrdinalIgnoreCase));
        if (indice >= 0)
            _equipos[indice] = equipo;
    }

    public void Eliminar(Equipo equipo) => _equipos.Remove(equipo);

    public List<Equipo> Buscar(Func<Equipo, bool> predicado) =>
        _equipos.Where(predicado).ToList();
}

