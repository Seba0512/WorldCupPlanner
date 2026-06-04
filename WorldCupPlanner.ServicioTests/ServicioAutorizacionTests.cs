using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioAutorizacionTests
{
    private ServicioAutorizacion _servicio = null!;

    [TestInitialize]
    public void Setup()
    {
        _servicio = new ServicioAutorizacion();
    }

    // --- VerificarEsAdministrador ---

    [TestMethod]
    public void VerificarEsAdministrador_UsuarioAdministrador_NoLanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);

        // Act & Assert
        _servicio.VerificarEsAdministrador(usuario);
    }

    [TestMethod]
    public void VerificarEsAdministrador_UsuarioEditor_LanzaAccesoDenegado()
    {
        // Arrange
        var usuario = CrearUsuarioConRol(RolUsuario.Editor);

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.VerificarEsAdministrador(usuario));
    }

    [TestMethod]
    public void VerificarEsAdministrador_UsuarioSinRoles_LanzaAccesoDenegado()
    {
        // Arrange
        var usuario = CrearUsuarioSinRoles();

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.VerificarEsAdministrador(usuario));
    }

    [TestMethod]
    public void VerificarEsAdministrador_UsuarioConAmbosRoles_NoLanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioSinRoles();
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);
        usuario.AgregarRol(RolUsuario.Editor);

        // Act & Assert
        _servicio.VerificarEsAdministrador(usuario);
    }

    // --- VerificarEsEditorOAdministrador ---

    [TestMethod]
    public void VerificarEsEditorOAdministrador_UsuarioEditor_NoLanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioConRol(RolUsuario.Editor);

        // Act & Assert
        _servicio.VerificarEsEditorOAdministrador(usuario);
    }

    [TestMethod]
    public void VerificarEsEditorOAdministrador_UsuarioAdministrador_NoLanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);

        // Act & Assert
        _servicio.VerificarEsEditorOAdministrador(usuario);
    }

    [TestMethod]
    public void VerificarEsEditorOAdministrador_UsuarioConAmbosRoles_NoLanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioSinRoles();
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);
        usuario.AgregarRol(RolUsuario.Editor);

        // Act & Assert
        _servicio.VerificarEsEditorOAdministrador(usuario);
    }

    [TestMethod]
    public void VerificarEsEditorOAdministrador_UsuarioSinRoles_LanzaAccesoDenegado()
    {
        // Arrange
        var usuario = CrearUsuarioSinRoles();

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.VerificarEsEditorOAdministrador(usuario));
    }

    

    [TestMethod]
    public void VerificarEsPeriodista_CuandoUsuarioEsPeriodista_NoLanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioConRol(RolUsuario.Periodista);

        // Act & Assert
        _servicio.VerificarEsPeriodista(usuario);
    }

    [TestMethod]
    public void VerificarEsPeriodista_CuandoUsuarioEsEditor_LanzaAccesoDenegado()
    {
        // Arrange
        var usuario = CrearUsuarioConRol(RolUsuario.Editor);

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.VerificarEsPeriodista(usuario));
    }

    // --- VerificarPermiso (método base) ---

    [TestMethod]
    public void VerificarPermiso_UsuarioTieneRolRequerido_NoLanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioConRol(RolUsuario.Editor);

        // Act & Assert
        _servicio.VerificarPermiso(usuario, RolUsuario.Editor);
    }

    [TestMethod]
    public void VerificarPermiso_UsuarioNoTieneRolRequerido_LanzaAccesoDenegado()
    {
        // Arrange
        var usuario = CrearUsuarioConRol(RolUsuario.Editor);

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.VerificarPermiso(usuario, RolUsuario.AdministradorDelSistema));
    }

    [TestMethod]
    public void VerificarPermiso_MensajeExcepcionContieneRol()
    {
        // Arrange
        var usuario = CrearUsuarioSinRoles();

        // Act
        var excepcion = Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.VerificarPermiso(usuario, RolUsuario.AdministradorDelSistema));

        // Assert
        StringAssert.Contains(excepcion.Message, "AdministradorDelSistema");
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
