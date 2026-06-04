using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
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
    public void MarcarComoLeida_CuandoNotificacionExiste_ActualizaEsLeidaATrue()
    {
        // Arrange
        var repositorio = new RepositorioNotificacionEnMemoria();
        var servicio = new ServicioNotificacion(repositorio);
        var usuario = CrearPeriodista("leida@test.com");
        var notificacion = new Notificacion { UsuarioDestinatarioId = usuario.Id };
        repositorio.Agregar(notificacion);

        // Act
        servicio.MarcarComoLeida(notificacion.Id, usuario.Id);

        // Assert
        Assert.IsTrue(repositorio.ObtenerPorUsuario(usuario.Id).First().EsLeida);
    }

    [TestMethod]
    public void MarcarComoLeida_CuandoNotificacionNoPertenecAlUsuario_LanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioNotificacionEnMemoria();
        var servicio = new ServicioNotificacion(repositorio);
        var usuario = CrearPeriodista("duenio@test.com");
        var otroUsuario = CrearPeriodista("otro@test.com");
        var notificacion = new Notificacion { UsuarioDestinatarioId = usuario.Id };
        repositorio.Agregar(notificacion);

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            servicio.MarcarComoLeida(notificacion.Id, otroUsuario.Id));
    }

    [TestMethod]
    public void MarcarComoLeida_CuandoNotificacionNoExiste_LanzaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioNotificacionEnMemoria();
        var servicio = new ServicioNotificacion(repositorio);

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() =>
            servicio.MarcarComoLeida(999, 1));
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
        var editor = new Usuario(new DatosUsuario { Nombre = "Sebita", Apellido = "Luciardi", Email = "carlos@test.com", FechaNacimiento = "01-01-1990" });
        editor.AgregarRol(RolUsuario.Editor);

        // Act
        servicio.GenerarNotificacionesResultado(partido, new List<Usuario> { editor });

        // Assert
        Assert.AreEqual(0, repositorio.ObtenerPorUsuario(editor.Id).Count);
    }

    private static Usuario CrearPeriodista(string email)
    {
        var usuario = new Usuario(new DatosUsuario { Nombre = "Test", Apellido = "Test", Email = email, FechaNacimiento = "01-01-1990" });
        usuario.AgregarRol(RolUsuario.Periodista);
        return usuario;
    }
}