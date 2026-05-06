using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public sealed class RepositorioUsuarioEnMemoriaTests
{
    private RepositorioUsuarioEnMemoria _repositorio = null!;

    public RepositorioUsuarioEnMemoriaTests()
    {
        _repositorio = new RepositorioUsuarioEnMemoria();
    }

    [TestMethod]
    public void Agregar_ConUsuarioValido_AgregoUnUsuario()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act
        _repositorio.Agregar(usuario);

        // Assert
        var usuarios = _repositorio.ObtenerTodos();
        Assert.AreEqual(1, usuarios.Count);
        Assert.AreSame(usuario, usuarios[0]);
    }

    [TestMethod]
    public void Agregar_ConVariosUsuarios_AgregoTodosLosUsuarios()
    {
        // Arrange
        var usuario1 = CrearUsuarioValido("juan@example.com");
        var usuario2 = CrearUsuarioValido("maria@example.com");
        var usuario3 = CrearUsuarioValido("pedro@example.com");

        // Act
        _repositorio.Agregar(usuario1);
        _repositorio.Agregar(usuario2);
        _repositorio.Agregar(usuario3);

        // Assert
        var usuarios = _repositorio.ObtenerTodos();
        Assert.AreEqual(3, usuarios.Count);
    }

    [TestMethod]
    public void Actualizar_ConUsuarioExistente_ActualizaElUsuario()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _repositorio.Agregar(usuario);
        
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);

        // Act
        _repositorio.Actualizar(usuario);

        // Assert
        var usuarioActualizado = _repositorio.ObtenerTodos()[0];
        Assert.IsTrue(usuarioActualizado.TieneRol(RolUsuario.AdministradorDelSistema));
    }

    [TestMethod]
    public void ActualizarNombre_ConNombreValido_ActualizaElNombre()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _repositorio.Agregar(usuario);

        // Act
        usuario.ActualizarNombre("Carlos");

        // Assert
        Assert.AreEqual("Carlos", usuario.Nombre);
    }

    [TestMethod]
    public void ActualizarNombre_ConNombreVacio_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => usuario.ActualizarNombre(""));
    }

    [TestMethod]
    public void ActualizarApellido_ConApellidoValido_ActualizaElApellido()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _repositorio.Agregar(usuario);

        // Act
        usuario.ActualizarApellido("García");

        // Assert
        Assert.AreEqual("García", usuario.Apellido);
    }

    [TestMethod]
    public void ActualizarEmail_ConEmailValido_ActualizaElEmail()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _repositorio.Agregar(usuario);

        // Act
        usuario.ActualizarEmail("newemail@example.com");

        // Assert
        Assert.AreEqual("newemail@example.com", usuario.Email);
    }

    [TestMethod]
    public void ActualizarEmail_ConEmailInvalido_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => usuario.ActualizarEmail("email_invalido"));
    }

    [TestMethod]
    public void ActualizarFechaNacimiento_ConFechaValida_ActualizaLaFecha()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _repositorio.Agregar(usuario);
        var nuevaFecha = new DateTime(1995, 5, 15);

        // Act
        usuario.ActualizarFechaNacimiento(nuevaFecha);

        // Assert
        Assert.AreEqual(nuevaFecha, usuario.FechaNacimiento);
    }

    [TestMethod]
    public void ActualizarFechaNacimiento_ConFechaFutura_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        var fechaFutura = DateTime.Now.AddYears(1);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => usuario.ActualizarFechaNacimiento(fechaFutura));
    }

    [TestMethod]
    public void Actualizar_ConUsuarioNoExistente_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _repositorio.Actualizar(usuario));
    }

    [TestMethod]
    public void Eliminar_ConUsuarioExistente_EliminaElUsuario()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _repositorio.Agregar(usuario);

        // Act
        _repositorio.Eliminar(usuario);

        // Assert
        var usuarios = _repositorio.ObtenerTodos();
        Assert.AreEqual(0, usuarios.Count);
    }

    [TestMethod]
    public void Eliminar_ConUsuarioNoExistente_NoLanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act & Assert (no debe lanzar excepción)
        _repositorio.Eliminar(usuario);
    }

    [TestMethod]
    public void ObtenerTodos_SinUsuarios_DevuelveListaVacia()
    {
        // Act
        var usuarios = _repositorio.ObtenerTodos();

        // Assert
        Assert.AreEqual(0, usuarios.Count);
    }

    [TestMethod]
    public void ObtenerTodos_DevuelveCopia_NoReferencia()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _repositorio.Agregar(usuario);

        // Act
        var usuarios1 = _repositorio.ObtenerTodos();
        var usuarios2 = _repositorio.ObtenerTodos();

        // Assert
        Assert.AreNotSame(usuarios1, usuarios2);
        Assert.AreEqual(usuarios1.Count, usuarios2.Count);
    }

    [TestMethod]
    public void ObtenerPorEmail_ConEmailExistente_DevuelveElUsuario()
    {
        // Arrange
        var email = "juan@example.com";
        var usuario = CrearUsuarioValido(email);
        _repositorio.Agregar(usuario);

        // Act
        var usuarioEncontrado = _repositorio.ObtenerPorEmail(email);

        // Assert
        Assert.IsNotNull(usuarioEncontrado);
        Assert.AreSame(usuario, usuarioEncontrado);
    }

    [TestMethod]
    public void ObtenerPorEmail_ConEmailNoExistente_DevuelveNull()
    {
        // Act
        var usuarioEncontrado = _repositorio.ObtenerPorEmail("noexiste@example.com");

        // Assert
        Assert.IsNull(usuarioEncontrado);
    }

    [TestMethod]
    public void ObtenerPorEmail_SinDiferenciarCase_DevuelveElUsuario()
    {
        // Arrange
        var usuario = CrearUsuarioValido("Juan@Example.com");
        _repositorio.Agregar(usuario);

        // Act
        var usuarioEncontrado = _repositorio.ObtenerPorEmail("juan@example.com");

        // Assert
        Assert.IsNotNull(usuarioEncontrado);
        Assert.AreSame(usuario, usuarioEncontrado);
    }

    private Usuario CrearUsuarioValido(string email)
    {
        return new Usuario("Juan", "Pérez", email, new DateTime(1990, 1, 1));
    }
}

