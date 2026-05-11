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

    [TestMethod]
    public void EditarPartidoDePrimeraRonda_ConOctavosGenerados_DebeLanzarExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartido(FaseTorneo.PrimeraRondaEliminatoria);
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);
        servicioPartido.RegistrarFaseGenerada(FaseTorneo.Octavos);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioPartido.CargarResultado(partido.Id, 2, 1));
    }

    [TestMethod]
    public void EditarPartidoDeOctavos_ConCuartosGenerados_DebeLanzarExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartido(FaseTorneo.Octavos);
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);
        servicioPartido.RegistrarFaseGenerada(FaseTorneo.Cuartos);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioPartido.EditarFecha(partido.Id, new DateTime(2026, 7, 1)));
    }

    [TestMethod]
    public void EditarPartidoDeCuartos_ConSemifinalesGeneradas_DebeLanzarExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartido(FaseTorneo.Cuartos);
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);
        servicioPartido.RegistrarFaseGenerada(FaseTorneo.Semifinal);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioPartido.EditarEstadio(partido.Id, CrearEstadio("Estadio Nuevo")));
    }

    [TestMethod]
    public void EditarPartidoDeSemifinal_ConFinalGenerada_DebeLanzarExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartido(FaseTorneo.Semifinal);
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);
        servicioPartido.RegistrarFaseGenerada(FaseTorneo.Final);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioPartido.CargarResultado(partido.Id, 2, 1));
    }

    [TestMethod]
    public void EsFaseBloqueada_SinFasesBloqueadas_RetornaFalso()
    {
        // Arrange
        var servicioPartido = new ServicioPartido(new RepositorioPartidoEnMemoria());

        // Act & Assert
        Assert.IsFalse(servicioPartido.EsFaseBloqueada(FaseTorneo.FaseDeGrupos));
    }

    [TestMethod]
    public void EsFaseBloqueada_ConFaseDeGruposBloqueadaExplicitamente_RetornaVerdadero()
    {
        // Arrange
        var servicioPartido = new ServicioPartido(new RepositorioPartidoEnMemoria());
        servicioPartido.BloquearEdicionPartidosDeFaseDeGrupos();

        // Act & Assert
        Assert.IsTrue(servicioPartido.EsFaseBloqueada(FaseTorneo.FaseDeGrupos));
    }

    [TestMethod]
    public void BuscarPartidoPorId_ConIdInexistente_RetornaNull()
    {
        // Arrange
        var servicioPartido = new ServicioPartido(new RepositorioPartidoEnMemoria());

        // Act
        var resultado = servicioPartido.BuscarPartidoPorId(99);

        // Assert
        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void BuscarPartidoPorId_ConPartidoExistente_RetornaPartido()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);

        // Act
        var resultado = servicioPartido.BuscarPartidoPorId(partido.Id);

        // Assert
        Assert.AreEqual(partido, resultado);
    }

    private static Partido CrearPartido()
    {
        return CrearPartido(FaseTorneo.FaseDeGrupos);
    }

    private static Partido CrearPartido(FaseTorneo fase)
    {
        return new Partido
        {
            Id = 1,
            Fecha = new DateTime(2026, 6, 1, 14, 0, 0),
            Estadio = CrearEstadio("Estadio Inicial"),
            EquipoLocal = CrearEquipo("Equipo Local"),
            EquipoVisitante = CrearEquipo("Equipo Visitante"),
            Fase = fase
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
