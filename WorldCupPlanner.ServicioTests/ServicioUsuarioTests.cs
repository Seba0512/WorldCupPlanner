using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public sealed class ServicioUsuarioTests
{
    private RepositorioUsuarioEnMemoria _repositorio = null!;
    private ServicioUsuario _servicio = null!;

    public ServicioUsuarioTests()
    {
        _repositorio = new RepositorioUsuarioEnMemoria();
        _servicio = new ServicioUsuario(_repositorio);
    }

    [TestMethod]
    public void AgregarUsuario_ConEmailValido_AgregarAlSistema()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act
        _servicio.AgregarUsuario(usuario);

        // Assert
        var usuarios = _servicio.ObtenerTodosLosUsuarios();
        Assert.AreEqual(1, usuarios.Count);
        Assert.AreSame(usuario, usuarios[0]);
    }

    [TestMethod]
    public void AgregarUsuario_ConEmailDuplicado_LanzaExcepcion()
    {
        // Arrange
        var usuario1 = CrearUsuarioValido("juan@example.com");
        var usuario2 = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario1);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _servicio.AgregarUsuario(usuario2));
    }

    [TestMethod]
    public void AgregarUsuario_ConEmailDuplicadoDistintasMinusculas_LanzaExcepcion()
    {
        // Arrange
        var usuario1 = CrearUsuarioValido("Juan@Example.com");
        var usuario2 = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario1);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _servicio.AgregarUsuario(usuario2));
    }

    [TestMethod]
    public void ActualizarUsuario_ConUsuarioExistente_ActualizaEnSistema()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);

        // Act
        _servicio.ActualizarUsuario(usuario);

        // Assert
        var usuarios = _servicio.ObtenerTodosLosUsuarios();
        Assert.IsTrue(usuarios[0].TieneRol(RolUsuario.AdministradorDelSistema));
    }

    [TestMethod]
    public void ActualizarUsuario_ConNombreNuevo_ActualizaNombre()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act
        _servicio.ActualizarUsuario(usuario, nombre: "Carlos");

        // Assert
        var usuarioActualizado = _servicio.ObtenerTodosLosUsuarios()[0];
        Assert.AreEqual("Carlos", usuarioActualizado.Nombre);
    }

    [TestMethod]
    public void ActualizarUsuario_ConApellidoNuevo_ActualizaApellido()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act
        _servicio.ActualizarUsuario(usuario, apellido: "García");

        // Assert
        var usuarioActualizado = _servicio.ObtenerTodosLosUsuarios()[0];
        Assert.AreEqual("García", usuarioActualizado.Apellido);
    }

    [TestMethod]
    public void ActualizarUsuario_ConEmailNuevo_ActualizaEmail()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act
        _servicio.ActualizarUsuario(usuario, email: "newemail@example.com");

        // Assert
        var usuarioActualizado = _servicio.ObtenerTodosLosUsuarios()[0];
        Assert.AreEqual("newemail@example.com", usuarioActualizado.Email);
    }

    [TestMethod]
    public void ActualizarUsuario_ConEmailDuplicado_LanzaExcepcion()
    {
        // Arrange
        var usuario1 = CrearUsuarioValido("juan@example.com");
        var usuario2 = CrearUsuarioValido("maria@example.com");
        _servicio.AgregarUsuario(usuario1);
        _servicio.AgregarUsuario(usuario2);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.ActualizarUsuario(usuario2, email: "juan@example.com")
        );
    }

    [TestMethod]
    public void ActualizarUsuario_ConEmailDuplicadoDistintasMinusculas_LanzaExcepcion()
    {
        // Arrange
        var usuario1 = CrearUsuarioValido("Juan@Example.com");
        var usuario2 = CrearUsuarioValido("maria@example.com");
        _servicio.AgregarUsuario(usuario1);
        _servicio.AgregarUsuario(usuario2);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.ActualizarUsuario(usuario2, email: "juan@example.com")
        );
    }

    [TestMethod]
    public void ActualizarUsuario_ConFechaNacimientoNueva_ActualizaFecha()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);
        var nuevaFecha = new DateTime(1995, 5, 15);

        // Act
        _servicio.ActualizarUsuario(usuario, fechaNacimiento: nuevaFecha);

        // Assert
        var usuarioActualizado = _servicio.ObtenerTodosLosUsuarios()[0];
        Assert.AreEqual(nuevaFecha, usuarioActualizado.FechaNacimiento);
    }

    [TestMethod]
    public void ActualizarUsuario_ConMultiplesCampos_ActualizaTodos()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);
        var nuevaFecha = new DateTime(1995, 5, 15);

        // Act
        _servicio.ActualizarUsuario(usuario, nombre: "Carlos", apellido: "García", 
            email: "carlos@example.com", fechaNacimiento: nuevaFecha);

        // Assert
        var usuarioActualizado = _servicio.ObtenerTodosLosUsuarios()[0];
        Assert.AreEqual("Carlos", usuarioActualizado.Nombre);
        Assert.AreEqual("García", usuarioActualizado.Apellido);
        Assert.AreEqual("carlos@example.com", usuarioActualizado.Email);
        Assert.AreEqual(nuevaFecha, usuarioActualizado.FechaNacimiento);
    }

    [TestMethod]
    public void ActualizarUsuario_ConUsuarioNoExistente_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.ActualizarUsuario(usuario));
    }

    [TestMethod]
    public void EliminarUsuario_ConUsuarioExistente_EliminaDelSistema()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act
        _servicio.EliminarUsuario(usuario);

        // Assert
        var usuarios = _servicio.ObtenerTodosLosUsuarios();
        Assert.AreEqual(0, usuarios.Count);
    }

    [TestMethod]
    public void ObtenerTodosLosUsuarios_SinUsuarios_DevuelveListaVacia()
    {
        // Act
        var usuarios = _servicio.ObtenerTodosLosUsuarios();

        // Assert
        Assert.AreEqual(0, usuarios.Count);
    }

    [TestMethod]
    public void ObtenerTodosLosUsuarios_ConVariosUsuarios_DevuelveTodos()
    {
        // Arrange
        var usuario1 = CrearUsuarioValido("juan@example.com");
        var usuario2 = CrearUsuarioValido("maria@example.com");
        var usuario3 = CrearUsuarioValido("pedro@example.com");
        _servicio.AgregarUsuario(usuario1);
        _servicio.AgregarUsuario(usuario2);
        _servicio.AgregarUsuario(usuario3);

        // Act
        var usuarios = _servicio.ObtenerTodosLosUsuarios();

        // Assert
        Assert.AreEqual(3, usuarios.Count);
    }

    [TestMethod]
    public void ObtenerPorEmail_ConEmailExistente_DevuelveUsuario()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act
        var usuarioEncontrado = _servicio.ObtenerPorEmail("juan@example.com");

        // Assert
        Assert.IsNotNull(usuarioEncontrado);
        Assert.AreSame(usuario, usuarioEncontrado);
    }

    [TestMethod]
    public void ObtenerPorEmail_ConEmailNoExistente_DevuelveNull()
    {
        // Act
        var usuarioEncontrado = _servicio.ObtenerPorEmail("noexiste@example.com");

        // Assert
        Assert.IsNull(usuarioEncontrado);
    }

    [TestMethod]
    public void GenerarContraseniaDefecto_AsignaContraseniaAlUsuario()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act
        _servicio.GenerarContraseniaDefecto(usuario);

        // Assert
        Assert.IsNotNull(usuario.ContraseniaCifrada);
        Assert.IsTrue(usuario.VerificarContrasenia("TempPass@2026"));
    }

    [TestMethod]
    public void GenerarContraseniaDefecto_ConSeed_AsignaContraseniaConSeed()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act
        _servicio.GenerarContraseniaDefecto(usuario, seed: 42);

        // Assert
        Assert.IsNotNull(usuario.ContraseniaCifrada);
        Assert.IsTrue(usuario.VerificarContrasenia("TempPass@2068"));
    }

    [TestMethod]
    public void ReiniciarContrasenia_ConAdministrador_ReiniciaContrasenia()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        var administrador = CrearUsuarioValido("admin@example.com");
        _servicio.AgregarUsuario(usuario);
        _servicio.AgregarUsuario(administrador);
        administrador.AgregarRol(RolUsuario.AdministradorDelSistema);

        // Primer login - establecer contraseña
        _servicio.GenerarContraseniaDefecto(usuario);

        // Act
        _servicio.ReiniciarContrasenia(usuario, administrador);

        // Assert
        Assert.IsTrue(usuario.VerificarContrasenia("TempPass@2026"));
    }

    [TestMethod]
    public void ReiniciarContrasenia_ConNoAdministrador_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        var noAdmin = CrearUsuarioValido("noadmin@example.com");
        _servicio.AgregarUsuario(usuario);
        _servicio.AgregarUsuario(noAdmin);
        noAdmin.AgregarRol(RolUsuario.Editor);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _servicio.ReiniciarContrasenia(usuario, noAdmin));
    }

    [TestMethod]
    public void CambiarContrasenia_ConContraseniaActualCorrecta_CambiaLaContrasenia()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        usuario.EstablecerContrasenia("ViejaPass@123");

        // Act
        _servicio.CambiarContrasenia(usuario, "ViejaPass@123", "NuevaPass@456");

        // Assert
        Assert.IsTrue(usuario.VerificarContrasenia("NuevaPass@456"));
        Assert.IsFalse(usuario.VerificarContrasenia("ViejaPass@123"));
    }

    [TestMethod]
    public void CambiarContrasenia_ConContraseniaActualIncorrecta_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        usuario.EstablecerContrasenia("ViejaPass@123");

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.CambiarContrasenia(usuario, "IncorrectPass@999", "NuevaPass@456")
        );
    }

    [TestMethod]
    public void Login_ConEmailYContraseniaCorrectos_DevuelveUsuario()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        usuario.EstablecerContrasenia("Pass@12345");
        _servicio.AgregarUsuario(usuario);

        // Act
        var usuarioAutenticado = _servicio.Login("juan@example.com", "Pass@12345");

        // Assert
        Assert.IsNotNull(usuarioAutenticado);
        Assert.AreSame(usuario, usuarioAutenticado);
    }

    [TestMethod]
    public void Login_ConEmailNoExistente_LanzaExcepcion()
    {
        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.Login("noexiste@example.com", "Pass@12345")
        );
    }

    [TestMethod]
    public void Login_ConContraseniaIncorrecta_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        usuario.EstablecerContrasenia("Pass@12345");
        _servicio.AgregarUsuario(usuario);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.Login("juan@example.com", "IncorrectPass@999")
        );
    }

    [TestMethod]
    public void Login_CaseSensitiveEmail_NoDistingueMayusculas()
    {
        // Arrange
        var usuario = CrearUsuarioValido("Juan@Example.com");
        usuario.EstablecerContrasenia("Pass@12345");
        _servicio.AgregarUsuario(usuario);

        // Act
        var usuarioAutenticado = _servicio.Login("juan@example.com", "Pass@12345");

        // Assert
        Assert.IsNotNull(usuarioAutenticado);
        Assert.AreSame(usuario, usuarioAutenticado);
    }

    [TestMethod]
    public void ActualizarUsuario_ConNombreVacio_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            _servicio.ActualizarUsuario(usuario, nombre: "")
        );
    }

    [TestMethod]
    public void ActualizarUsuario_ConApellidoVacio_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            _servicio.ActualizarUsuario(usuario, apellido: "")
        );
    }

    [TestMethod]
    public void ActualizarUsuario_ConEmailVacio_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            _servicio.ActualizarUsuario(usuario, email: "")
        );
    }

    [TestMethod]
    public void ActualizarUsuario_ConEmailInvalido_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            _servicio.ActualizarUsuario(usuario, email: "email_invalido")
        );
    }

    [TestMethod]
    public void ActualizarUsuario_ConEmailEspacios_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            _servicio.ActualizarUsuario(usuario, email: "   ")
        );
    }

    private Usuario CrearUsuarioValido(string email)
    {
        return new Usuario("Juan", "Pérez", email, new DateTime(1990, 1, 1));
    }
}

