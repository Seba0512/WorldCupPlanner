using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;

namespace WorldCupPlanner.ServicioTests;


[TestClass]
public class UsuarioTests
{
    
    [TestMethod]
    public void CrearUsuario_ConDatosValidos_DebeCrearExitosamente()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "Pérez";
        var email = "juan.perez@example.com";
        var fechaNacimiento = new DateTime(1990, 5, 15);

        // Act
        var usuario = new Usuario(nombre, apellido, email, fechaNacimiento);

        // Assert
        Assert.AreEqual(nombre, usuario.Nombre);
        Assert.AreEqual(apellido, usuario.Apellido);
        Assert.AreEqual(email, usuario.Email);
        Assert.AreEqual(fechaNacimiento, usuario.FechaNacimiento);
    }

    [TestMethod]
    public void CrearUsuario_ConNombreVacio_DebeLanzarExcepcion()
    {
        // Arrange
        var apellido = "Pérez";
        var email = "juan.perez@example.com";
        var fechaNacimiento = new DateTime(1990, 5, 15);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(string.Empty, apellido, email, fechaNacimiento)
        );
    }

    [TestMethod]
    public void CrearUsuario_ConApellidoVacio_DebeLanzarExcepcion()
    {
        // Arrange
        var nombre = "Juan";
        var email = "juan.perez@example.com";
        var fechaNacimiento = new DateTime(1990, 5, 15);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(nombre, string.Empty, email, fechaNacimiento)
        );
    }

    [TestMethod]
    public void CrearUsuario_ConEmailInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "Pérez";
        var email = "email-invalido";
        var fechaNacimiento = new DateTime(1990, 5, 15);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(nombre, apellido, email, fechaNacimiento)
        );
    }

    [TestMethod]
    public void CrearUsuario_ConEmailVacio_DebeLanzarExcepcion()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "Pérez";
        var email = string.Empty;
        var fechaNacimiento = new DateTime(1990, 5, 15);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(nombre, apellido, email, fechaNacimiento)
        );
    }

    [TestMethod]
    public void CrearUsuario_ConFechaNacimientoFutura_DebeLanzarExcepcion()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "Pérez";
        var email = "juan.perez@example.com";
        var fechaNacimiento = DateTime.Now.AddDays(1);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(nombre, apellido, email, fechaNacimiento)
        );
    }



    [TestMethod]
    public void EstablecerContrasenia_ConContrasesiaValida_DebeEstablecerseExitosamente()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "MyPassword123!";

        // Act
        usuario.EstablecerContrasenia(contrasenia);

        // Assert
        Assert.IsTrue(usuario.VerificarContrasenia(contrasenia));
    }

    [TestMethod]
    public void EstablecerContrasenia_ConMenosDe8Caracteres_DebeLanzarExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "Pass12!"; // 7 caracteres

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            usuario.EstablecerContrasenia(contrasenia)
        );
    }

    [TestMethod]
    public void EstablecerContrasenia_SinMayúsculas_DebeLanzarExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "password123!"; // Sin mayúsculas

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            usuario.EstablecerContrasenia(contrasenia)
        );
    }

    [TestMethod]
    public void EstablecerContrasenia_SinMinúsculas_DebeLanzarExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "PASSWORD123!"; // Sin minúsculas

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            usuario.EstablecerContrasenia(contrasenia)
        );
    }

    [TestMethod]
    public void EstablecerContrasenia_SinNúmeros_DebeLanzarExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "PasswordAbc!"; // Sin números

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            usuario.EstablecerContrasenia(contrasenia)
        );
    }

    [TestMethod]
    public void EstablecerContrasenia_SinCaracteresEspeciales_DebeLanzarExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "Password1234"; // Sin caracteres especiales

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            usuario.EstablecerContrasenia(contrasenia)
        );
    }

    [TestMethod]
    public void EstablecerContrasenia_ConContrasesiaVacia_DebeLanzarExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            usuario.EstablecerContrasenia(string.Empty)
        );
    }

    [TestMethod]
    public void EstablecerContrasenia_DebeAlmacenarCifrada_NoEnTextoPlano()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "MyPassword123!";

        // Act
        usuario.EstablecerContrasenia(contrasenia);

        // Assert
        Assert.IsFalse(usuario.ContraseniaCifrada == contrasenia,
            "La contrasenia no debe almacenarse en texto plano");
        Assert.IsNotNull(usuario.ContraseniaCifrada);
        Assert.IsTrue(usuario.ContraseniaCifrada.Length > 0);
    }

    [TestMethod]
    public void VerificarContrasenia_ConContrasesiaCorrecta_DebeRetornarVerdadero()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "MyPassword123!";
        usuario.EstablecerContrasenia(contrasenia);

        // Act
        var resultado = usuario.VerificarContrasenia(contrasenia);

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void VerificarContrasenia_ConContrasesiaIncorrecta_DebeRetornarFalso()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "MyPassword123!";
        usuario.EstablecerContrasenia(contrasenia);

        // Act
        var resultado = usuario.VerificarContrasenia("WrongPassword123!");

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void VerificarContrasenia_ConContrasesiaCasoDiferente_DebeRetornarFalso()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenia = "MyPassword123!";
        usuario.EstablecerContrasenia(contrasenia);

        // Act - Intenta con caso diferente
        var resultado = usuario.VerificarContrasenia("mypassword123!");

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EstablecerContrasenia_ConCaracteresEspecialesValidos_DebeAceptarlos()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var contrasenias = new[]
        {
            "Password123@",
            "Password123#",
            "Password123$",
            "Password123."
        };

        // Act & Assert
        foreach (var contrasenia in contrasenias)
        {
            usuario.EstablecerContrasenia(contrasenia);
            Assert.IsTrue(usuario.VerificarContrasenia(contrasenia),
                $"Debería aceptar contrasenia con carácter especial: {contrasenia}");
        }
    }



    [TestMethod]
    public void AgregarRol_ConRolAdministrador_DebeAgregarse()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);

        // Assert
        Assert.IsTrue(usuario.TieneRol(RolUsuario.AdministradorDelSistema));
    }

    [TestMethod]
    public void AgregarRol_ConRolEditor_DebeAgregarse()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act
        usuario.AgregarRol(RolUsuario.Editor);

        // Assert
        Assert.IsTrue(usuario.TieneRol(RolUsuario.Editor));
    }

    [TestMethod]
    public void AgregarRol_ConRolesMultiples_DebePermitirRolesNoExcluyentes()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);
        usuario.AgregarRol(RolUsuario.Editor);

        // Assert
        Assert.IsTrue(usuario.TieneRol(RolUsuario.AdministradorDelSistema));
        Assert.IsTrue(usuario.TieneRol(RolUsuario.Editor));
    }

    [TestMethod]
    public void AgregarRol_MismoRolDosVeces_NoDebeAgregarseDuplicado()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);

        // Act
        var rolesAntes = usuario.ObtenerRoles().Count();
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);
        var rolesDepues = usuario.ObtenerRoles().Count();

        // Assert
        Assert.AreEqual(rolesAntes, rolesDepues);
    }

    [TestMethod]
    public void RemoverRol_ConRolExistente_DebeRemoverse()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);

        // Act
        usuario.RemoverRol(RolUsuario.AdministradorDelSistema);

        // Assert
        Assert.IsFalse(usuario.TieneRol(RolUsuario.AdministradorDelSistema));
    }

    [TestMethod]
    public void RemoverRol_NoAfectaOtrosRoles()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);
        usuario.AgregarRol(RolUsuario.Editor);

        // Act
        usuario.RemoverRol(RolUsuario.AdministradorDelSistema);

        // Assert
        Assert.IsFalse(usuario.TieneRol(RolUsuario.AdministradorDelSistema));
        Assert.IsTrue(usuario.TieneRol(RolUsuario.Editor));
    }

    [TestMethod]
    public void ObtenerRoles_ConRolesAsignados_DebeRetornarTodos()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);
        usuario.AgregarRol(RolUsuario.Editor);

        // Act
        var roles = usuario.ObtenerRoles();

        // Assert
        Assert.AreEqual(2, roles.Count());
        Assert.IsTrue(roles.Contains(RolUsuario.AdministradorDelSistema));
        Assert.IsTrue(roles.Contains(RolUsuario.Editor));
    }

    [TestMethod]
    public void TieneRol_UsuarioSinRoles_DebeRetornarFalso()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act & Assert
        Assert.IsFalse(usuario.TieneRol(RolUsuario.AdministradorDelSistema));
        Assert.IsFalse(usuario.TieneRol(RolUsuario.Editor));
    }



    private Usuario CrearUsuarioValido()
    {
        return new Usuario(
            "Juan",
            "Pérez",
            "juan.perez@example.com",
            new DateTime(1990, 5, 15)
        );
    }

}
