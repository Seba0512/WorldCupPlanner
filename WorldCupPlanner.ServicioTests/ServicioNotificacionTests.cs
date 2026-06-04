using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioNotificacionTests
{
    [TestMethod]
    public void GenerarNotificacionesResultado_CuandoHayPeriodistas_CreaUnaNotificacionPorCadaPeriodista()
    {
        // Arrange
        var repositorio = new RepositorioNotificacionEnMemoria();
        var servicio = new ServicioNotificacion(repositorio);
        var partido = new Partido();
        var periodista1 = new Usuario(new DatosUsuario { Nombre = "Juan", Apellido = "Perez", Email = "juan@test.com", FechaNacimiento = "01-01-1990" });
        periodista1.AgregarRol(RolUsuario.Periodista);
        var periodista2 = new Usuario(new DatosUsuario { Nombre = "Ana", Apellido = "Lopez", Email = "ana@test.com", FechaNacimiento = "01-01-1990" });
        periodista2.AgregarRol(RolUsuario.Periodista);
        var periodistas = new List<Usuario> { periodista1, periodista2 };

        // Act
        servicio.GenerarNotificacionesResultado(partido, periodistas);

        // Assert
        var notificaciones1 = repositorio.ObtenerPorUsuario(periodista1.Id);
        var notificaciones2 = repositorio.ObtenerPorUsuario(periodista2.Id);
        Assert.AreEqual(1, notificaciones1.Count);
        Assert.AreEqual(1, notificaciones2.Count);
    }

    [TestMethod]
    public void GenerarNotificacionesResultado_CuandoNoHayPeriodistas_NoCreaNingunNotificacion()
    {
        // Arrange
        var repositorio = new RepositorioNotificacionEnMemoria();
        var servicio = new ServicioNotificacion(repositorio);
        var partido = new Partido();

        // Act
        servicio.GenerarNotificacionesResultado(partido, new List<Usuario>());

        // Assert
        Assert.AreEqual(0, repositorio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void GenerarNotificacionesResultado_CuandoUsuarioNoEsPeriodista_NoLeGeneraNotificacion()
    {
        // Arrange
        var repositorio = new RepositorioNotificacionEnMemoria();
        var servicio = new ServicioNotificacion(repositorio);
        var partido = new Partido();
        var editor = new Usuario(new DatosUsuario { Nombre = "Carlos", Apellido = "Gomez", Email = "carlos@test.com", FechaNacimiento = "01-01-1990" });
        editor.AgregarRol(RolUsuario.Editor);

        // Act
        servicio.GenerarNotificacionesResultado(partido, new List<Usuario> { editor });

        // Assert
        Assert.AreEqual(0, repositorio.ObtenerPorUsuario(editor.Id).Count);
    }
}