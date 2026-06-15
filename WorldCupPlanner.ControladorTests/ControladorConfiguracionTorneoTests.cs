using WorldCupPlanner.Controladores;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ControladorTests;

[TestClass]
public class ControladorConfiguracionTorneoTests
{
    [TestMethod]
    public void ObtenerConfiguracionActual_DevuelveConfiguracionDelServicio()
    {
        var servicio = new ServicioConfiguracionTorneo();
        var controlador = new ControladorConfiguracionTorneo(servicio);

        var configuracion = controlador.ObtenerConfiguracionActual();

        Assert.AreEqual(0, configuracion.SemillaFixture);
        Assert.AreEqual(0, configuracion.SemillaCompletar);
        Assert.AreEqual(3, configuracion.MaxPartidosPorDia);
    }

    [TestMethod]
    public void ModificarConfiguracion_DelegaEnServicioYMantieneComportamiento()
    {
        var servicio = new ServicioConfiguracionTorneo();
        var controlador = new ControladorConfiguracionTorneo(servicio);
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);

        controlador.ModificarConfiguracion(administrador, new ConfiguracionTorneo
        {
            SemillaFixture = 12,
            SemillaCompletar = 4,
            SemillaCrucesFase = 8,
            SemillaSimulation = 16,
            MaxPartidosPorDia = 4,
            SeparacionEntreFechas = 2
        });

        var configuracion = servicio.ObtenerConfiguracionActual();
        Assert.AreEqual(12, configuracion.SemillaFixture);
        Assert.AreEqual(4, configuracion.SemillaCompletar);
        Assert.AreEqual(8, configuracion.SemillaCrucesFase);
        Assert.AreEqual(16, configuracion.SemillaSimulation);
        Assert.AreEqual(4, configuracion.MaxPartidosPorDia);
        Assert.AreEqual(2, configuracion.SeparacionEntreFechas);
    }

    [TestMethod]
    public void ModificarConfiguracion_ConUsuarioNoAdministrador_MantieneValidacionDelServicio()
    {
        var servicio = new ServicioConfiguracionTorneo();
        var controlador = new ControladorConfiguracionTorneo(servicio);
        var editor = CrearUsuarioConRol(RolUsuario.Editor);

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            controlador.ModificarConfiguracion(editor, new ConfiguracionTorneo()));
    }

    [TestMethod]
    public void RegistrarFixtureGenerado_BloqueaConfiguracionYPermiteObtenerUsada()
    {
        var servicio = new ServicioConfiguracionTorneo();
        var controlador = new ControladorConfiguracionTorneo(servicio);
        var configuracionUsada = new ConfiguracionTorneo
        {
            SemillaFixture = 5,
            SemillaCompletar = 6
        };

        controlador.RegistrarFixtureGenerado(configuracionUsada);

        Assert.IsFalse(controlador.EsConfiguracionModificable);
        var obtenida = controlador.ObtenerConfiguracionUsadaParaFixture();
        Assert.AreEqual(5, obtenida.SemillaFixture);
        Assert.AreEqual(6, obtenida.SemillaCompletar);
    }

    [TestMethod]
    public void PuedeModificarConfiguracion_DelegaConsultaDelServicio()
    {
        var servicio = new ServicioConfiguracionTorneo();
        var controlador = new ControladorConfiguracionTorneo(servicio);
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var periodista = CrearUsuarioConRol(RolUsuario.Periodista);

        Assert.IsTrue(controlador.PuedeModificarConfiguracion(administrador));
        Assert.IsFalse(controlador.PuedeModificarConfiguracion(periodista));
        Assert.IsFalse(controlador.PuedeModificarConfiguracion(null));
    }

    private static Usuario CrearUsuarioConRol(RolUsuario rol)
    {
        var usuario = new Usuario(new DatosUsuario
        {
            Nombre = "Usuario",
            Apellido = "Prueba",
            Email = $"{rol.ToString().ToLowerInvariant()}@test.com",
            FechaNacimiento = "01-01-1990"
        });
        usuario.AgregarRol(rol);
        return usuario;
    }
}

