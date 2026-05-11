using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public sealed class InicializadorDatosTests
{
    private RepositorioUsuarioEnMemoria _repositorio = null!;
    private ServicioUsuario _servicioUsuario = null!;
    private InicializadorDatos _inicializador = null!;

    [TestInitialize]
    public void Setup()
    {
        _repositorio = new RepositorioUsuarioEnMemoria();
        var auditoria = new ServicioAuditoria(new RepositorioAuditoriaEnMemoria());
        _servicioUsuario = new ServicioUsuario(_repositorio, auditoria);
        _inicializador = new InicializadorDatos(_servicioUsuario);
    }

    // --- Creación automática ---

    [TestMethod]
    public void InicializarUsuariosSemilla_CreaUsuarioAdministrador()
    {
        // Act
        _inicializador.InicializarUsuariosSemilla();

        // Assert
        var usuario = _servicioUsuario.ObtenerPorEmail(InicializadorDatos.EmailAdministrador);
        Assert.IsNotNull(usuario);
    }

    [TestMethod]
    public void InicializarUsuariosSemilla_CreaUsuarioEditor()
    {
        // Act
        _inicializador.InicializarUsuariosSemilla();

        // Assert
        var usuario = _servicioUsuario.ObtenerPorEmail(InicializadorDatos.EmailEditor);
        Assert.IsNotNull(usuario);
    }

    [TestMethod]
    public void InicializarUsuariosSemilla_CreaUsuarioAdminEditor()
    {
        // Act
        _inicializador.InicializarUsuariosSemilla();

        // Assert
        var usuario = _servicioUsuario.ObtenerPorEmail(InicializadorDatos.EmailAdminEditor);
        Assert.IsNotNull(usuario);
    }

    [TestMethod]
    public void InicializarUsuariosSemilla_CreaTresUsuariosEnTotal()
    {
        // Act
        _inicializador.InicializarUsuariosSemilla();

        // Assert
        var usuarios = _servicioUsuario.ObtenerTodosLosUsuarios();
        Assert.AreEqual(3, usuarios.Count);
    }

    // --- Asignación correcta de roles ---

    [TestMethod]
    public void InicializarUsuariosSemilla_AdministradorTieneSoloRolAdministrador()
    {
        // Arrange
        _inicializador.InicializarUsuariosSemilla();

        // Act
        var usuario = _servicioUsuario.ObtenerPorEmail(InicializadorDatos.EmailAdministrador)!;

        // Assert
        Assert.IsTrue(usuario.TieneRol(RolUsuario.AdministradorDelSistema));
        Assert.IsFalse(usuario.TieneRol(RolUsuario.Editor));
    }

    [TestMethod]
    public void InicializarUsuariosSemilla_EditorTieneSoloRolEditor()
    {
        // Arrange
        _inicializador.InicializarUsuariosSemilla();

        // Act
        var usuario = _servicioUsuario.ObtenerPorEmail(InicializadorDatos.EmailEditor)!;

        // Assert
        Assert.IsTrue(usuario.TieneRol(RolUsuario.Editor));
        Assert.IsFalse(usuario.TieneRol(RolUsuario.AdministradorDelSistema));
    }

    [TestMethod]
    public void InicializarUsuariosSemilla_AdminEditorTieneAmbosRoles()
    {
        // Arrange
        _inicializador.InicializarUsuariosSemilla();

        // Act
        var usuario = _servicioUsuario.ObtenerPorEmail(InicializadorDatos.EmailAdminEditor)!;

        // Assert
        Assert.IsTrue(usuario.TieneRol(RolUsuario.AdministradorDelSistema));
        Assert.IsTrue(usuario.TieneRol(RolUsuario.Editor));
    }

    // --- Prevención de duplicados ---

    [TestMethod]
    public void InicializarUsuariosSemilla_LlamadaDobleVeces_NoDuplicaUsuarios()
    {
        // Act
        _inicializador.InicializarUsuariosSemilla();
        _inicializador.InicializarUsuariosSemilla();

        // Assert
        var usuarios = _servicioUsuario.ObtenerTodosLosUsuarios();
        Assert.AreEqual(3, usuarios.Count);
    }

    [TestMethod]
    public void InicializarUsuariosSemilla_LlamadaMultiplesVeces_NoCreaAdministradorDuplicado()
    {
        // Act
        _inicializador.InicializarUsuariosSemilla();
        _inicializador.InicializarUsuariosSemilla();
        _inicializador.InicializarUsuariosSemilla();

        // Assert
        var administradores = _servicioUsuario.Buscar(u =>
            u.Email.Equals(InicializadorDatos.EmailAdministrador, StringComparison.OrdinalIgnoreCase));
        Assert.AreEqual(1, administradores.Count);
    }

    [TestMethod]
    public void InicializarUsuariosSemilla_UsuarioYaExiste_NoLanzaExcepcion()
    {
        // Arrange
        _inicializador.InicializarUsuariosSemilla();

        // Act & Assert
        _inicializador.InicializarUsuariosSemilla();
    }

    // --- Autenticación con credenciales configuradas ---

    [TestMethod]
    public void InicializarUsuariosSemilla_AdministradorPuedeAutenticarse()
    {
        // Arrange
        _inicializador.InicializarUsuariosSemilla();

        // Act
        var usuario = _servicioUsuario.Login(
            InicializadorDatos.EmailAdministrador,
            InicializadorDatos.ContrasenaAdministrador);

        // Assert
        Assert.IsNotNull(usuario);
        Assert.AreEqual(InicializadorDatos.EmailAdministrador, usuario.Email);
    }

    [TestMethod]
    public void InicializarUsuariosSemilla_EditorPuedeAutenticarse()
    {
        // Arrange
        _inicializador.InicializarUsuariosSemilla();

        // Act
        var usuario = _servicioUsuario.Login(
            InicializadorDatos.EmailEditor,
            InicializadorDatos.ContrasenaEditor);

        // Assert
        Assert.IsNotNull(usuario);
        Assert.AreEqual(InicializadorDatos.EmailEditor, usuario.Email);
    }

    [TestMethod]
    public void InicializarUsuariosSemilla_AdminEditorPuedeAutenticarse()
    {
        // Arrange
        _inicializador.InicializarUsuariosSemilla();

        // Act
        var usuario = _servicioUsuario.Login(
            InicializadorDatos.EmailAdminEditor,
            InicializadorDatos.ContrasenaAdminEditor);

        // Assert
        Assert.IsNotNull(usuario);
        Assert.AreEqual(InicializadorDatos.EmailAdminEditor, usuario.Email);
    }

    // --- Combinación correcta de permisos ---

    [TestMethod]
    public void InicializarUsuariosSemilla_AdminEditorPuedeEjecutarAccionesDeAdministrador()
    {
        // Arrange
        _inicializador.InicializarUsuariosSemilla();
        var adminEditor = _servicioUsuario.ObtenerPorEmail(InicializadorDatos.EmailAdminEditor)!;
        var nuevoUsuario = new Usuario(new DatosUsuario
        {
            Nombre = "Nuevo",
            Apellido = "Usuario",
            Email = "nuevo@example.com",
            FechaNacimiento = "01-01-1990"
        });

        // Act — acción exclusiva de administrador
        _servicioUsuario.AgregarUsuario(nuevoUsuario, solicitante: adminEditor);

        // Assert
        Assert.IsNotNull(_servicioUsuario.ObtenerPorEmail("nuevo@example.com"));
    }

    [TestMethod]
    public void InicializarUsuariosSemilla_SoloEditorNoTienePermisoDeAdministrador()
    {
        // Arrange
        _inicializador.InicializarUsuariosSemilla();
        var editor = _servicioUsuario.ObtenerPorEmail(InicializadorDatos.EmailEditor)!;
        var nuevoUsuario = new Usuario(new DatosUsuario
        {
            Nombre = "Nuevo",
            Apellido = "Usuario",
            Email = "nuevo@example.com",
            FechaNacimiento = "01-01-1990"
        });

        // Act & Assert — editor no puede agregar usuarios
        Assert.ThrowsException<WorldCupPlanner.Dominio.Excepciones.AccesoDenegadoExcepcion>(() =>
            _servicioUsuario.AgregarUsuario(nuevoUsuario, solicitante: editor));
    }
}
