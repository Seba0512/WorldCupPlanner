using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioConfiguracionTorneoTests
{
    [TestMethod]
    public void ModificarConfiguracion_ConAdministrador_AplicaNuevaConfiguracion()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var nuevaConfiguracion = new ConfiguracionTorneo
        {
            SemillaFixture = 7,
            MaxPartidosPorDia = 4,
            SeparacionEntreFechas = 2
        };

        // Act
        servicioConfiguracion.ModificarConfiguracion(administrador, nuevaConfiguracion);

        // Assert
        var configuracionActual = servicioConfiguracion.ObtenerConfiguracionActual();
        Assert.AreEqual(7, configuracionActual.SemillaFixture);
        Assert.AreEqual(4, configuracionActual.MaxPartidosPorDia);
        Assert.AreEqual(2, configuracionActual.SeparacionEntreFechas);
    }

    [TestMethod]
    public void ModificarConfiguracion_ConEditor_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var editor = CrearUsuarioConRol(RolUsuario.Editor);
        var nuevaConfiguracion = new ConfiguracionTorneo { SemillaFixture = 7 };

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioConfiguracion.ModificarConfiguracion(editor, nuevaConfiguracion));
    }

    [TestMethod]
    public void ModificarConfiguracion_ConFixtureGenerado_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var configuracionUsada = new ConfiguracionTorneo { SemillaFixture = 1 };
        var nuevaConfiguracion = new ConfiguracionTorneo { SemillaFixture = 2 };
        servicioConfiguracion.RegistrarFixtureGenerado(configuracionUsada);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioConfiguracion.ModificarConfiguracion(administrador, nuevaConfiguracion));
    }

    [TestMethod]
    public void ObtenerConfiguracionUsadaParaFixture_ConFixtureGenerado_DebeDevolverConfiguracionBloqueada()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var configuracionUsada = new ConfiguracionTorneo
        {
            SemillaFixture = 3,
            SemillaCompletar = 4,
            SemillaCrucesFase = 5,
            SemillaSimulation = 6
        };

        // Act
        servicioConfiguracion.RegistrarFixtureGenerado(configuracionUsada);
        var configuracionVisible = servicioConfiguracion.ObtenerConfiguracionUsadaParaFixture();

        // Assert
        Assert.AreEqual(3, configuracionVisible.SemillaFixture);
        Assert.AreEqual(4, configuracionVisible.SemillaCompletar);
        Assert.AreEqual(5, configuracionVisible.SemillaCrucesFase);
        Assert.AreEqual(6, configuracionVisible.SemillaSimulation);
    }

    [TestMethod]
    public void ObtenerConfiguracionActual_SinModificaciones_DevuelveValoresPorDefecto()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();

        // Act
        var configuracion = servicioConfiguracion.ObtenerConfiguracionActual();

        // Assert
        Assert.AreEqual(0, configuracion.SemillaFixture);
        Assert.AreEqual(0, configuracion.SemillaCompletar);
        Assert.AreEqual(0, configuracion.SemillaCrucesFase);
        Assert.AreEqual(0, configuracion.SemillaSimulation);
        Assert.AreEqual(new DateTime(2026, 6, 1), configuracion.FechaInicioTorneo);
        Assert.AreEqual(3, configuracion.MaxPartidosPorDia);
        Assert.AreEqual(3, configuracion.SeparacionEntreFechas);
    }

    [TestMethod]
    public void ObtenerConfiguracionActual_DespuesDeModificar_DevuelveValoresActualizados()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var nuevaConfiguracion = new ConfiguracionTorneo
        {
            SemillaFixture = 42,
            SemillaSimulation = 99,
            MaxPartidosPorDia = 5,
            FechaInicioTorneo = new DateTime(2026, 7, 1)
        };

        // Act
        servicioConfiguracion.ModificarConfiguracion(administrador, nuevaConfiguracion);
        var configuracion = servicioConfiguracion.ObtenerConfiguracionActual();

        // Assert
        Assert.AreEqual(42, configuracion.SemillaFixture);
        Assert.AreEqual(99, configuracion.SemillaSimulation);
        Assert.AreEqual(5, configuracion.MaxPartidosPorDia);
        Assert.AreEqual(new DateTime(2026, 7, 1), configuracion.FechaInicioTorneo);
    }

    [TestMethod]
    public void ObtenerConfiguracionActual_DevuelveCopia_ModificarlaNoAlteraElEstadoInterno()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var configuracionOriginal = new ConfiguracionTorneo { SemillaFixture = 10 };
        servicioConfiguracion.ModificarConfiguracion(administrador, configuracionOriginal);

        // Act
        var copia = servicioConfiguracion.ObtenerConfiguracionActual();
        copia.SemillaFixture = 999;

        // Assert
        var configuracionNoMutada = servicioConfiguracion.ObtenerConfiguracionActual();
        Assert.AreEqual(10, configuracionNoMutada.SemillaFixture);
    }

    private static Usuario CrearUsuarioConRol(RolUsuario rol)
    {
        var usuario = new Usuario(new DatosUsuario
        {
            Nombre = "Seba",
            Apellido = "Tester",
            Email = $"{rol}@example.com",
            FechaNacimiento = "01-01-2000"
        });
        usuario.AgregarRol(rol);

        return usuario;
    }
}
