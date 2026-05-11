using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioAuditoriaTests
{
    private ServicioAuditoria _servicio = null!;

    [TestInitialize]
    public void Setup()
    {
        _servicio = new ServicioAuditoria(new RepositorioAuditoriaEnMemoria());
    }

    [TestMethod]
    public void Registrar_ConDatosValidos_AgregaEntrada()
    {
        // Act
        _servicio.Registrar("admin@example.com", "ALTA_EQUIPO", "Equipo: Uruguay");

        // Assert
        var entradas = _servicio.ObtenerTodos();
        Assert.AreEqual(1, entradas.Count);
    }

    [TestMethod]
    public void ObtenerTodos_SinEntradas_DevuelveListaVacia()
    {
        // Act
        var entradas = _servicio.ObtenerTodos();

        // Assert
        Assert.AreEqual(0, entradas.Count);
    }

    [TestMethod]
    public void ObtenerTodos_ConVariasEntradas_DevuelveTodas()
    {
        // Arrange
        _servicio.Registrar("u1@example.com", "ALTA", "detalle1");
        _servicio.Registrar("u2@example.com", "BAJA", "detalle2");
        _servicio.Registrar("u3@example.com", "EDICION", "detalle3");

        // Act
        var entradas = _servicio.ObtenerTodos();

        // Assert
        Assert.AreEqual(3, entradas.Count);
    }

    [TestMethod]
    public void ObtenerTodos_DevuelveEnOrdenDescendentePorTimestamp()
    {
        // Arrange
        _servicio.Registrar("u@example.com", "ACCION_1", "");
        System.Threading.Thread.Sleep(5);
        _servicio.Registrar("u@example.com", "ACCION_2", "");

        // Act
        var entradas = _servicio.ObtenerTodos();

        // Assert — la más reciente primero
        Assert.AreEqual("ACCION_2", entradas[0].Accion);
    }

    // --- Autorización ---

    [TestMethod]
    public void ObtenerTodos_ConSolicitanteAdministrador_DevuelveLista()
    {
        // Arrange
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        _servicio.Registrar("sistema", "ACCION", "detalle");

        // Act
        var entradas = _servicio.ObtenerTodos(solicitante: admin);

        // Assert
        Assert.AreEqual(1, entradas.Count);
    }

    [TestMethod]
    public void ObtenerTodos_ConSolicitanteEditor_LanzaAccesoDenegado()
    {
        // Arrange
        var editor = CrearUsuarioConRol(RolUsuario.Editor);

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.ObtenerTodos(solicitante: editor));
    }

    [TestMethod]
    public void ObtenerTodos_ConSolicitanteSinRoles_LanzaAccesoDenegado()
    {
        // Arrange
        var sinRoles = CrearUsuarioSinRoles();

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.ObtenerTodos(solicitante: sinRoles));
    }

    private static Usuario CrearUsuarioConRol(RolUsuario rol)
    {
        var usuario = CrearUsuarioSinRoles();
        usuario.AgregarRol(rol);
        return usuario;
    }

    private static Usuario CrearUsuarioSinRoles()
    {
        return new Usuario(new DatosUsuario
        {
            Nombre = "Test",
            Apellido = "User",
            Email = "test@example.com",
            FechaNacimiento = "01-01-1990"
        });
    }
}
