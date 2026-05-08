using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioPartidoTests
{
    [TestMethod]
    public void EditarFecha_ConPartidoExistente_ActualizaFecha()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);
        var nuevaFecha = new DateTime(2026, 6, 15, 18, 0, 0);

        // Act
        servicioPartido.EditarFecha(partido.Id, nuevaFecha);

        // Assert
        Assert.AreEqual(nuevaFecha, partido.Fecha);
    }

    [TestMethod]
    public void EditarEstadio_ConPartidoExistente_ActualizaEstadio()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);
        var nuevoEstadio = CrearEstadio("Estadio Nuevo");

        // Act
        servicioPartido.EditarEstadio(partido.Id, nuevoEstadio);

        // Assert
        Assert.AreEqual(nuevoEstadio, partido.Estadio);
    }

    [TestMethod]
    public void CargarResultado_ConPartidoExistente_ActualizaGoles()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);

        // Act
        servicioPartido.CargarResultado(partido.Id, 2, 1);

        // Assert
        Assert.AreEqual(2, partido.GolesLocal);
        Assert.AreEqual(1, partido.GolesVisitante);
    }

    [TestMethod]
    public void EditarFecha_ConPartidoInexistente_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioPartido = new ServicioPartido(new RepositorioPartidoEnMemoria());

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() =>
            servicioPartido.EditarFecha(99, new DateTime(2026, 6, 15)));
    }

    [TestMethod]
    public void EditarEstadio_ConEstadioInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);
        var estadioInvalido = CrearEstadio("");

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioPartido.EditarEstadio(partido.Id, estadioInvalido));
    }

    [TestMethod]
    public void CargarResultado_ConGolesNegativos_DebeLanzarExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioPartido.CargarResultado(partido.Id, -1, 0));
    }

    [TestMethod]
    public void EditarPartidoDeFaseDeGrupos_ConCrucesGenerados_DebeLanzarExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);
        servicioPartido.BloquearEdicionPartidosDeFaseDeGrupos();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioPartido.CargarResultado(partido.Id, 2, 1));
    }

    private static Partido CrearPartido()
    {
        return new Partido
        {
            Id = 1,
            Fecha = new DateTime(2026, 6, 1, 14, 0, 0),
            Estadio = CrearEstadio("Estadio Inicial"),
            EquipoLocal = CrearEquipo("Equipo Local"),
            EquipoVisitante = CrearEquipo("Equipo Visitante"),
            Fase = FaseTorneo.FaseDeGrupos
        };
    }

    private static Equipo CrearEquipo(string nombre)
    {
        return new Equipo
        {
            Nombre = nombre,
            RankingFIFA = 1000,
            Confederacion = Confederacion.UEFA
        };
    }

    private static Estadio CrearEstadio(string nombre)
    {
        return new Estadio
        {
            Nombre = nombre,
            Ciudad = "Ciudad",
            Capacidad = 30000
        };
    }
}
