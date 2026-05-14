using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;

namespace WorldCupPlanner.DominioTests;

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
        var fechaNacimiento = "05-15-1990";

        // Act
        var usuario = new Usuario(new DatosUsuario
        {
            Nombre = nombre,
            Apellido = apellido,
            Email = email,
            FechaNacimiento = fechaNacimiento
        });

        // Assert
        Assert.AreEqual(nombre, usuario.Nombre);
        Assert.AreEqual(apellido, usuario.Apellido);
        Assert.AreEqual(email, usuario.Email);
        Assert.AreEqual(new DateTime(1990, 5, 15), usuario.FechaNacimiento);
    }

    [TestMethod]
    public void CrearUsuario_ConNombreVacio_DebeLanzarExcepcion()
    {
        // Arrange
        var apellido = "Pérez";
        var email = "juan.perez@example.com";
        var fechaNacimiento = "05-15-1990";

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(new DatosUsuario { Nombre = string.Empty, Apellido = apellido, Email = email, FechaNacimiento = fechaNacimiento })
        );
    }

    [TestMethod]
    public void CrearUsuario_ConApellidoVacio_DebeLanzarExcepcion()
    {
        // Arrange
        var nombre = "Juan";
        var email = "juan.perez@example.com";
        var fechaNacimiento = "05-15-1990";

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(new DatosUsuario { Nombre = nombre, Apellido = string.Empty, Email = email, FechaNacimiento = fechaNacimiento })
        );
    }

    [TestMethod]
    public void CrearUsuario_ConEmailInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "Pérez";
        var email = "email-invalido";
        var fechaNacimiento = "05-15-1990";

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(new DatosUsuario { Nombre = nombre, Apellido = apellido, Email = email, FechaNacimiento = fechaNacimiento })
        );
    }

    [TestMethod]
    public void CrearUsuario_ConEmailVacio_DebeLanzarExcepcion()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "Pérez";
        var email = string.Empty;
        var fechaNacimiento = "05-15-1990";

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(new DatosUsuario { Nombre = nombre, Apellido = apellido, Email = email, FechaNacimiento = fechaNacimiento })
        );
    }

    [TestMethod]
    public void CrearUsuario_ConFechaNacimientoFutura_DebeLanzarExcepcion()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "Pérez";
        var email = "juan.perez@example.com";
        var fechaNacimiento = DateTime.Now.AddDays(1).ToString("MM-dd-yyyy");

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(new DatosUsuario { Nombre = nombre, Apellido = apellido, Email = email, FechaNacimiento = fechaNacimiento })
        );
    }

    [TestMethod]
    public void CrearUsuario_ConFormatoFechaInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "Pérez";
        var email = "juan.perez@example.com";

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(new DatosUsuario { Nombre = nombre, Apellido = apellido, Email = email, FechaNacimiento = "1990-05-15" })
        );
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(new DatosUsuario { Nombre = nombre, Apellido = apellido, Email = email, FechaNacimiento = "15/05/1990" })
        );
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(new DatosUsuario { Nombre = nombre, Apellido = apellido, Email = email, FechaNacimiento = "05/15/1990" })
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
        // Arrange — el usuario necesita al menos dos roles para poder remover uno
        var usuario = CrearUsuarioValido();
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);
        usuario.AgregarRol(RolUsuario.Editor);

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
    public void RemoverRol_CuandoEsElUnicoRol_DebeLanzarExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        usuario.AgregarRol(RolUsuario.Editor);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            usuario.RemoverRol(RolUsuario.Editor)
        );
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

    [TestMethod]
    public void ActualizarNombre_ConNombreValido_ActualizaElNombre()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act
        usuario.ActualizarNombre("Carlos");

        // Assert
        Assert.AreEqual("Carlos", usuario.Nombre);
    }

    [TestMethod]
    public void ActualizarNombre_ConNombreVacio_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => usuario.ActualizarNombre(string.Empty));
    }

    [TestMethod]
    public void ActualizarApellido_ConApellidoValido_ActualizaElApellido()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act
        usuario.ActualizarApellido("García");

        // Assert
        Assert.AreEqual("García", usuario.Apellido);
    }

    [TestMethod]
    public void ActualizarEmail_ConEmailValido_ActualizaElEmail()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act
        usuario.ActualizarEmail("newemail@example.com");

        // Assert
        Assert.AreEqual("newemail@example.com", usuario.Email);
    }

    [TestMethod]
    public void ActualizarEmail_ConEmailInvalido_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => usuario.ActualizarEmail("email_invalido"));
    }

    [TestMethod]
    public void ActualizarFechaNacimiento_ConFechaValida_ActualizaLaFecha()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act
        usuario.ActualizarFechaNacimiento("05-15-1995");

        // Assert
        Assert.AreEqual(new DateTime(1995, 5, 15), usuario.FechaNacimiento);
    }

    [TestMethod]
    public void ActualizarFechaNacimiento_ConFechaFutura_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        var fechaFutura = DateTime.Now.AddYears(1).ToString("MM-dd-yyyy");

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => usuario.ActualizarFechaNacimiento(fechaFutura));
    }

    [TestMethod]
    public void ActualizarFechaNacimiento_ConFormatoInvalido_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => usuario.ActualizarFechaNacimiento("1995-05-15"));
        Assert.ThrowsException<ArgumentException>(() => usuario.ActualizarFechaNacimiento("15/05/1995"));
    }

    [TestMethod]
    public void VerificarContrasenia_SinContraseniaEstablecida_DebeRetornarFalso()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act
        var resultado = usuario.VerificarContrasenia("CualquierContrasenia1!");

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void CrearUsuario_ConFechaNacimientoVacia_DebeLanzarExcepcion()
    {
        // Arrange & Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(new DatosUsuario
            {
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "juan.perez@example.com",
                FechaNacimiento = string.Empty
            })
        );
    }

    [TestMethod]
    public void CrearUsuario_ConFechaInvalidaEnCalendario_DebeLanzarExcepcion()
    {
        // El formato coincide con el patrón pero el día 30 de febrero no existe
        Assert.ThrowsException<ArgumentException>(() =>
            new Usuario(new DatosUsuario
            {
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "juan.perez@example.com",
                FechaNacimiento = "02-30-2020"
            })
        );
    }

    [TestMethod]
    public void ActualizarApellido_ConApellidoVacio_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => usuario.ActualizarApellido(string.Empty));
    }

    [TestMethod]
    public void ActualizarEmail_ConEmailVacio_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => usuario.ActualizarEmail(string.Empty));
    }

    [TestMethod]
    public void DatosUsuario_ExpresionWith_CreaCopiaConCampoModificado()
    {
        // Arrange
        var original = new DatosUsuario
        {
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan.perez@example.com",
            FechaNacimiento = "05-15-1990"
        };

        // Act — invoca el constructor de copia generado por el record
        var copia = original with { Nombre = "Carlos" };

        // Assert
        Assert.AreEqual("Carlos", copia.Nombre);
        Assert.AreEqual(original.Apellido, copia.Apellido);
        Assert.AreEqual(original.Email, copia.Email);
        Assert.AreEqual(original.FechaNacimiento, copia.FechaNacimiento);
    }

    // --- DebeCambiarContrasenia ---

    [TestMethod]
    public void DebeCambiarContrasenia_NuevoUsuario_EsFalsoPorDefecto()
    {
        // Arrange & Act
        var usuario = CrearUsuarioValido();

        // Assert
        Assert.IsFalse(usuario.DebeCambiarContrasenia);
    }

    [TestMethod]
    public void MarcarParaCambioForzado_EstableceFlagEnTrue()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act
        usuario.MarcarParaCambioForzado();

        // Assert
        Assert.IsTrue(usuario.DebeCambiarContrasenia);
    }

    [TestMethod]
    public void EstablecerContrasenia_LimpieFlagDebeCambiarContrasenia()
    {
        // Arrange
        var usuario = CrearUsuarioValido();
        usuario.MarcarParaCambioForzado();

        // Act
        usuario.EstablecerContrasenia("NuevaPass@123");

        // Assert
        Assert.IsFalse(usuario.DebeCambiarContrasenia);
    }

    [TestMethod]
    public void MarcarParaCambioForzado_LuegoEstablecerContrasenia_FlagQuedaFalso()
    {
        // Arrange
        var usuario = CrearUsuarioValido();

        // Act
        usuario.MarcarParaCambioForzado();
        usuario.EstablecerContrasenia("NuevaPass@123");

        // Assert
        Assert.IsFalse(usuario.DebeCambiarContrasenia,
            "Establecer contraseña debe limpiar el flag de cambio forzado.");
    }

    private static Usuario CrearUsuarioValido()
    {
        return new Usuario(new DatosUsuario
        {
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan.perez@example.com",
            FechaNacimiento = "05-15-1990"
        });
    }
}
