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
        var auditoria = new ServicioAuditoria(new RepositorioAuditoriaEnMemoria());
        _servicio = new ServicioUsuario(_repositorio, auditoria);
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
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() => _servicio.AgregarUsuario(usuario2));
    }

    [TestMethod]
    public void AgregarUsuario_ConEmailDuplicadoDistintasMinusculas_LanzaExcepcion()
    {
        // Arrange
        var usuario1 = CrearUsuarioValido("Juan@Example.com");
        var usuario2 = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario1);

        // Act & Assert
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() => _servicio.AgregarUsuario(usuario2));
    }

    [TestMethod]
    public void AgregarUsuario_ConEmailDuplicado_MensajeDeErrorAmigable()
    {
        // Arrange
        var usuario1 = CrearUsuarioValido("juan@example.com");
        var usuario2 = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario1);

        // Act
        var ex = Assert.ThrowsException<EntidadYaExistenteExcepcion>(() => _servicio.AgregarUsuario(usuario2));

        // Assert
        Assert.AreEqual("El correo electrónico ingresado ya está en uso.", ex.Message);
    }

    [TestMethod]
    public void AgregarUsuario_ConEmailDuplicado_RegistraIntentoEnAuditoria()
    {
        // Arrange
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var repositorio = new RepositorioUsuarioEnMemoria();
        var servicio = new ServicioUsuario(repositorio, auditoria);
        var usuario1 = CrearUsuarioValido("juan@example.com");
        var usuario2 = CrearUsuarioValido("juan@example.com");
        servicio.AgregarUsuario(usuario1);
        repoAuditoria.Entradas.Clear();

        // Act
        try { servicio.AgregarUsuario(usuario2); } catch (EntidadYaExistenteExcepcion) { }

        // Assert
        Assert.IsTrue(repoAuditoria.Entradas.Any(e => e.Accion == "EMAIL_DUPLICADO_INTENTO"),
            "Debe registrarse en auditoría el intento de registro con email duplicado.");
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
        _servicio.ActualizarUsuario(usuario, new ActualizacionUsuario { Nombre = "Carlos" });

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
        _servicio.ActualizarUsuario(usuario, new ActualizacionUsuario { Apellido = "García" });

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
        _servicio.ActualizarUsuario(usuario, new ActualizacionUsuario { Email = "newemail@example.com" });

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
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() =>
            _servicio.ActualizarUsuario(usuario2, new ActualizacionUsuario { Email = "juan@example.com" })
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
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() =>
            _servicio.ActualizarUsuario(usuario2, new ActualizacionUsuario { Email = "juan@example.com" })
        );
    }

    [TestMethod]
    public void ActualizarUsuario_ConEmailConEspacios_SeNormalizaParaValidarDuplicados()
    {
        // Arrange
        var usuario1 = CrearUsuarioValido("juan@example.com");
        var usuario2 = CrearUsuarioValido("maria@example.com");
        _servicio.AgregarUsuario(usuario1);
        _servicio.AgregarUsuario(usuario2);

        // Act & Assert
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() =>
            _servicio.ActualizarUsuario(usuario2, new ActualizacionUsuario { Email = "  juan@example.com  " })
        );
    }

    [TestMethod]
    public void ActualizarUsuario_ConFechaNacimientoNueva_ActualizaFecha()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act
        _servicio.ActualizarUsuario(usuario, new ActualizacionUsuario { FechaNacimiento = "05-15-1995" });

        // Assert
        var usuarioActualizado = _servicio.ObtenerTodosLosUsuarios()[0];
        Assert.AreEqual(new DateTime(1995, 5, 15), usuarioActualizado.FechaNacimiento);
    }

    [TestMethod]
    public void ActualizarUsuario_ConMultiplesCampos_ActualizaTodos()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act
        _servicio.ActualizarUsuario(usuario, new ActualizacionUsuario
        {
            Nombre = "Carlos",
            Apellido = "García",
            Email = "carlos@example.com",
            FechaNacimiento = "05-15-1995"
        });

        // Assert
        var usuarioActualizado = _servicio.ObtenerTodosLosUsuarios()[0];
        Assert.AreEqual("Carlos", usuarioActualizado.Nombre);
        Assert.AreEqual("García", usuarioActualizado.Apellido);
        Assert.AreEqual("carlos@example.com", usuarioActualizado.Email);
        Assert.AreEqual(new DateTime(1995, 5, 15), usuarioActualizado.FechaNacimiento);
    }

    [TestMethod]
    public void ActualizarUsuario_ConUsuarioNoExistente_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.ActualizarUsuario(usuario, new ActualizacionUsuario()));
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
    public void CambiarContrasenia_ConMismaContraseniaActual_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        usuario.EstablecerContrasenia("MismaPass@123");

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.CambiarContrasenia(usuario, "MismaPass@123", "MismaPass@123")
        );
    }

    [TestMethod]
    public void CambiarContrasenia_ConMismaContraseniaActual_MensajeDeErrorAmigable()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        usuario.EstablecerContrasenia("MismaPass@123");

        // Act
        var ex = Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.CambiarContrasenia(usuario, "MismaPass@123", "MismaPass@123")
        );

        // Assert
        Assert.AreEqual("La nueva contraseña debe ser diferente de la contraseña actual.", ex.Message);
    }

    [TestMethod]
    public void CambiarContrasenia_ConMismaContraseniaActual_RegistraIntentoEnAuditoria()
    {
        // Arrange
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var repositorio = new RepositorioUsuarioEnMemoria();
        var servicio = new ServicioUsuario(repositorio, auditoria);
        var usuario = CrearUsuarioValido("juan@example.com");
        usuario.EstablecerContrasenia("MismaPass@123");

        // Act
        try { servicio.CambiarContrasenia(usuario, "MismaPass@123", "MismaPass@123"); } catch (InvalidOperationException) { }

        // Assert
        Assert.IsTrue(repoAuditoria.Entradas.Any(e => e.Accion == "CAMBIO_CONTRASENIA_RECHAZADO"),
            "Debe registrarse en auditoría el intento de reutilizar la contraseña actual.");
    }

    [TestMethod]
    public void CambiarContrasenia_ConMismaContraseniaActual_NoModificaLaContrasenia()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        usuario.EstablecerContrasenia("MismaPass@123");

        // Act
        try { _servicio.CambiarContrasenia(usuario, "MismaPass@123", "MismaPass@123"); } catch (InvalidOperationException) { }

        // Assert — la contraseña original debe seguir siendo válida
        Assert.IsTrue(usuario.VerificarContrasenia("MismaPass@123"),
            "La contraseña no debe haber cambiado tras un intento rechazado.");
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
            _servicio.ActualizarUsuario(usuario, new ActualizacionUsuario { Nombre = "" })
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
            _servicio.ActualizarUsuario(usuario, new ActualizacionUsuario { Apellido = "" })
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
            _servicio.ActualizarUsuario(usuario, new ActualizacionUsuario { Email = "" })
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
            _servicio.ActualizarUsuario(usuario, new ActualizacionUsuario { Email = "email_invalido" })
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
            _servicio.ActualizarUsuario(usuario, new ActualizacionUsuario { Email = "   " })
        );
    }

    // IRepositorio<Usuario>

    [TestMethod]
    public void Agregar_UsuarioValido_SeAgregaAlRepositorio()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act
        _servicio.Agregar(usuario);

        // Assert
        Assert.AreEqual(1, _servicio.ObtenerTodosLosUsuarios().Count);
    }

    [TestMethod]
    public void Agregar_EmailDuplicado_LanzaExcepcion()
    {
        // Arrange
        var usuario1 = CrearUsuarioValido("juan@example.com");
        var usuario2 = CrearUsuarioValido("juan@example.com");
        _servicio.Agregar(usuario1);

        // Act & Assert
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() => _servicio.Agregar(usuario2));
    }

    [TestMethod]
    public void Actualizar_UsuarioExistente_PersisteCambios()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);

        // Act
        _servicio.Actualizar(usuario);

        // Assert
        Assert.IsTrue(_servicio.Buscar(u => u.Email == "juan@example.com").First().TieneRol(RolUsuario.AdministradorDelSistema));
    }

    [TestMethod]
    public void Actualizar_UsuarioNoExistente_LanzaExcepcion()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => _servicio.Actualizar(usuario));
    }

    [TestMethod]
    public void Actualizar_UsuarioNulo_LanzaExcepcion()
    {
        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.Actualizar(null!));
    }

    [TestMethod]
    public void Eliminar_UsuarioExistente_SeEliminaDelRepositorio()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act
        _servicio.Eliminar(usuario);

        // Assert
        Assert.AreEqual(0, _servicio.ObtenerTodosLosUsuarios().Count);
    }

    [TestMethod]
    public void Buscar_ConPredicadoCoincidente_DevuelveUsuariosCorrespondientes()
    {
        // Arrange
        var admin = CrearUsuarioValido("admin@example.com");
        admin.AgregarRol(RolUsuario.AdministradorDelSistema);
        _servicio.AgregarUsuario(admin);
        _servicio.AgregarUsuario(CrearUsuarioValido("editor@example.com"));

        // Act
        var resultado = _servicio.Buscar(u => u.TieneRol(RolUsuario.AdministradorDelSistema));

        // Assert
        Assert.AreEqual(1, resultado.Count);
        Assert.AreSame(admin, resultado[0]);
    }

    [TestMethod]
    public void Buscar_SinCoincidencias_DevuelveListaVacia()
    {
        // Arrange
        _servicio.AgregarUsuario(CrearUsuarioValido("juan@example.com"));

        // Act
        var resultado = _servicio.Buscar(u => u.TieneRol(RolUsuario.AdministradorDelSistema));

        // Assert
        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void ActualizarUsuario_ConMismoEmail_NoLanzaExcepcionNiCambiaNada()
    {
        // Verifica la rama donde email != null pero es igual al actual
        // (la condición !email.Equals(...) es false, se omite la validación)
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        _servicio.ActualizarUsuario(usuario, new ActualizacionUsuario { Email = "juan@example.com" });

        Assert.AreEqual("juan@example.com", usuario.Email);
    }

    [TestMethod]
    public void AgregarUsuario_ConEjecutadoPorExplicito_RegistraEjecutadoPorEnAuditoria()
    {
        // Cubre el branch ejecutadoPor != null en AgregarUsuario
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var repositorio = new RepositorioUsuarioEnMemoria();
        var servicio = new ServicioUsuario(repositorio, auditoria);

        var usuario = CrearUsuarioValido("juan@example.com");
        servicio.AgregarUsuario(usuario, ejecutadoPor: "admin@example.com");

        Assert.AreEqual("admin@example.com", repoAuditoria.Entradas[0].Usuario);
    }

    [TestMethod]
    public void EliminarUsuario_ConEjecutadoPorExplicito_RegistraEjecutadoPorEnAuditoria()
    {
        // Cubre el branch ejecutadoPor != null en EliminarUsuario
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var repositorio = new RepositorioUsuarioEnMemoria();
        var servicio = new ServicioUsuario(repositorio, auditoria);

        var usuario = CrearUsuarioValido("juan@example.com");
        servicio.AgregarUsuario(usuario);
        repoAuditoria.Entradas.Clear();

        servicio.EliminarUsuario(usuario, ejecutadoPor: "admin@example.com");

        Assert.AreEqual("admin@example.com", repoAuditoria.Entradas[0].Usuario);
    }

    [TestMethod]
    public void ActualizarUsuario_ConEjecutadoPorExplicito_RegistraEjecutadoPorEnAuditoria()
    {
        // Cubre el branch ejecutadoPor != null en ActualizarUsuario
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var repositorio = new RepositorioUsuarioEnMemoria();
        var servicio = new ServicioUsuario(repositorio, auditoria);

        var usuario = CrearUsuarioValido("juan@example.com");
        servicio.AgregarUsuario(usuario);
        repoAuditoria.Entradas.Clear();

        servicio.ActualizarUsuario(usuario, new ActualizacionUsuario { Nombre = "Carlos", EjecutadoPor = "admin@example.com" });

        Assert.AreEqual("admin@example.com", repoAuditoria.Entradas[0].Usuario);
    }

    [TestMethod]
    public void ActualizarUsuario_SinCambiosDeCampos_RegistraDetalleAuditoriaSinCambios()
    {
        // Arrange
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var repositorio = new RepositorioUsuarioEnMemoria();
        var servicio = new ServicioUsuario(repositorio, auditoria);

        var usuario = CrearUsuarioValido("juan@example.com");
        servicio.AgregarUsuario(usuario);
        repoAuditoria.Entradas.Clear();

        // Act — actualizacion vacía: ningún campo cambia
        servicio.ActualizarUsuario(usuario, new ActualizacionUsuario());

        // Assert
        var detalle = repoAuditoria.Entradas[0].Detalle;
        Assert.IsTrue(detalle.StartsWith("Sin cambios efectivos en"),
            $"Detalle inesperado: '{detalle}'");
    }

    [TestMethod]
    public void ActualizarUsuario_ConCambiosDeCampos_RegistraDetalleAuditoriaConCamposModificados()
    {
        // Arrange
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var repositorio = new RepositorioUsuarioEnMemoria();
        var servicio = new ServicioUsuario(repositorio, auditoria);

        var usuario = CrearUsuarioValido("juan@example.com");
        servicio.AgregarUsuario(usuario);
        repoAuditoria.Entradas.Clear();

        // Act
        servicio.ActualizarUsuario(usuario, new ActualizacionUsuario { Nombre = "Carlos", Apellido = "García" });

        // Assert
        var detalle = repoAuditoria.Entradas[0].Detalle;
        Assert.IsTrue(detalle.StartsWith("Campos modificados en"),
            $"Detalle inesperado: '{detalle}'");
        StringAssert.Contains(detalle, "nombre: Carlos");
        StringAssert.Contains(detalle, "apellido: García");
    }

    // --- Autorización ---

    [TestMethod]
    public void AgregarUsuario_ConSolicitanteAdministrador_AgregaCorrectamente()
    {
        // Arrange
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var nuevo = CrearUsuarioValido("nuevo@example.com");

        // Act
        _servicio.AgregarUsuario(nuevo, solicitante: admin);

        // Assert
        Assert.AreEqual(1, _servicio.ObtenerTodosLosUsuarios().Count);
    }

    [TestMethod]
    public void AgregarUsuario_ConSolicitanteEditor_LanzaAccesoDenegado()
    {
        // Arrange
        var editor = CrearUsuarioConRol(RolUsuario.Editor);
        var nuevo = CrearUsuarioValido("nuevo@example.com");

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.AgregarUsuario(nuevo, solicitante: editor));
    }

    [TestMethod]
    public void AgregarUsuario_ConSolicitanteSinRoles_LanzaAccesoDenegado()
    {
        // Arrange
        var sinRoles = CrearUsuarioValido("sinroles@example.com");
        var nuevo = CrearUsuarioValido("nuevo@example.com");

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.AgregarUsuario(nuevo, solicitante: sinRoles));
    }

    [TestMethod]
    public void ActualizarUsuario_ConSolicitanteEditor_LanzaAccesoDenegado()
    {
        // Arrange
        var editor = CrearUsuarioConRol(RolUsuario.Editor);
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.ActualizarUsuario(usuario, solicitante: editor));
    }

    [TestMethod]
    public void ActualizarUsuario_ConSolicitanteAdministrador_ActualizaCorrectamente()
    {
        // Arrange
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act
        _servicio.ActualizarUsuario(usuario, new ActualizacionUsuario { Nombre = "Carlos" }, solicitante: admin);

        // Assert
        Assert.AreEqual("Carlos", _servicio.ObtenerTodosLosUsuarios()[0].Nombre);
    }

    [TestMethod]
    public void EliminarUsuario_ConSolicitanteEditor_LanzaAccesoDenegado()
    {
        // Arrange
        var editor = CrearUsuarioConRol(RolUsuario.Editor);
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.EliminarUsuario(usuario, solicitante: editor));
    }

    [TestMethod]
    public void EliminarUsuario_ConSolicitanteAdministrador_EliminaCorrectamente()
    {
        // Arrange
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var usuario = CrearUsuarioValido("juan@example.com");
        _servicio.AgregarUsuario(usuario);

        // Act
        _servicio.EliminarUsuario(usuario, solicitante: admin);

        // Assert
        Assert.AreEqual(0, _servicio.ObtenerTodosLosUsuarios().Count);
    }

    [TestMethod]
    public void ObtenerTodosLosUsuarios_ConSolicitanteEditor_LanzaAccesoDenegado()
    {
        // Arrange
        var editor = CrearUsuarioConRol(RolUsuario.Editor);

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.ObtenerTodosLosUsuarios(solicitante: editor));
    }

    [TestMethod]
    public void ObtenerTodosLosUsuarios_ConSolicitanteAdministrador_DevuelveLista()
    {
        // Arrange
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);

        // Act
        var resultado = _servicio.ObtenerTodosLosUsuarios(solicitante: admin);

        // Assert
        Assert.IsNotNull(resultado);
    }

    [TestMethod]
    public void AgregarUsuario_ConSolicitanteAmbosRoles_AgregaCorrectamente()
    {
        // Arrange
        var ambos = CrearUsuarioValido("ambos@example.com");
        ambos.AgregarRol(RolUsuario.AdministradorDelSistema);
        ambos.AgregarRol(RolUsuario.Editor);
        var nuevo = CrearUsuarioValido("nuevo@example.com");

        // Act
        _servicio.AgregarUsuario(nuevo, solicitante: ambos);

        // Assert
        Assert.AreEqual(1, _servicio.ObtenerTodosLosUsuarios().Count);
    }

    // --- GestionarRoles ---

    [TestMethod]
    public void GestionarRoles_CambiarAEditor_ActualizaRolesCorrectamente()
    {
        // Arrange — dos admins con emails distintos para evitar colisión
        var admin1 = CrearUsuarioValido("admin1@example.com");
        admin1.AgregarRol(RolUsuario.AdministradorDelSistema);
        _servicio.AgregarUsuario(admin1);
        var admin2 = CrearUsuarioValido("admin2@example.com");
        admin2.AgregarRol(RolUsuario.AdministradorDelSistema);
        _servicio.AgregarUsuario(admin2);

        // Act
        _servicio.GestionarRoles(admin1, new[] { RolUsuario.Editor });

        // Assert
        Assert.IsFalse(admin1.TieneRol(RolUsuario.AdministradorDelSistema));
        Assert.IsTrue(admin1.TieneRol(RolUsuario.Editor));
    }

    [TestMethod]
    public void GestionarRoles_RemoverUltimoAdmin_LanzaExcepcion()
    {
        // Arrange
        var admin = CrearUsuarioValido("admin@example.com");
        admin.AgregarRol(RolUsuario.AdministradorDelSistema);
        _servicio.AgregarUsuario(admin);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.GestionarRoles(admin, new[] { RolUsuario.Editor }));
    }

    [TestMethod]
    public void GestionarRoles_SinNingunRol_LanzaExcepcion()
    {
        // Arrange
        var admin = CrearUsuarioValido("admin@example.com");
        admin.AgregarRol(RolUsuario.AdministradorDelSistema);
        _servicio.AgregarUsuario(admin);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.GestionarRoles(admin, Enumerable.Empty<RolUsuario>()));
    }

    [TestMethod]
    public void GestionarRoles_RegistraCambioEnAuditoria()
    {
        // Arrange
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var repositorio = new RepositorioUsuarioEnMemoria();
        var servicio = new ServicioUsuario(repositorio, auditoria);
        var admin1 = CrearUsuarioValido("admin1@example.com");
        admin1.AgregarRol(RolUsuario.AdministradorDelSistema);
        servicio.AgregarUsuario(admin1);
        var admin2 = CrearUsuarioValido("admin2@example.com");
        admin2.AgregarRol(RolUsuario.AdministradorDelSistema);
        servicio.AgregarUsuario(admin2);
        repoAuditoria.Entradas.Clear();

        // Act
        servicio.GestionarRoles(admin1, new[] { RolUsuario.Editor });

        // Assert
        Assert.IsTrue(repoAuditoria.Entradas.Any(e => e.Accion == "CAMBIO_ROLES"));
    }

    // --- Rol por defecto ---

    [TestMethod]
    public void AgregarUsuario_SinRoles_AsignaRolEditorPorDefecto()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act
        _servicio.AgregarUsuario(usuario);

        // Assert
        Assert.IsTrue(usuario.TieneRol(RolUsuario.Editor));
    }

    [TestMethod]
    public void AgregarUsuario_SinRoles_RegistraAsignacionRolDefectoEnAuditoria()
    {
        // Arrange
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var repositorio = new RepositorioUsuarioEnMemoria();
        var servicio = new ServicioUsuario(repositorio, auditoria);
        var usuario = CrearUsuarioValido("juan@example.com");

        // Act
        servicio.AgregarUsuario(usuario);

        // Assert
        Assert.IsTrue(repoAuditoria.Entradas.Any(e => e.Accion == "ASIGNACION_ROL_DEFECTO"),
            "Debe registrarse en auditoría la asignación del rol Editor por defecto.");
    }

    [TestMethod]
    public void AgregarUsuario_ConRolExplicito_NoAgregaRolEditorAdicional()
    {
        // Arrange
        var usuario = CrearUsuarioValido("admin@example.com");
        usuario.AgregarRol(RolUsuario.AdministradorDelSistema);

        // Act
        _servicio.AgregarUsuario(usuario);

        // Assert
        Assert.IsTrue(usuario.TieneRol(RolUsuario.AdministradorDelSistema));
        Assert.IsFalse(usuario.TieneRol(RolUsuario.Editor));
    }

    // --- Último administrador ---

    [TestMethod]
    public void EliminarUsuario_UltimoAdministrador_LanzaExcepcion()
    {
        // Arrange
        var admin = CrearUsuarioValido("admin@example.com");
        admin.AgregarRol(RolUsuario.AdministradorDelSistema);
        _servicio.AgregarUsuario(admin);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.EliminarUsuario(admin));
    }

    [TestMethod]
    public void EliminarUsuario_UltimoAdministrador_MensajeDeErrorEspecifico()
    {
        // Arrange
        var admin = CrearUsuarioValido("admin@example.com");
        admin.AgregarRol(RolUsuario.AdministradorDelSistema);
        _servicio.AgregarUsuario(admin);

        // Act
        var ex = Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.EliminarUsuario(admin));

        // Assert
        Assert.AreEqual("No es posible eliminar el último administrador del sistema.", ex.Message);
    }

    [TestMethod]
    public void EliminarUsuario_UltimoAdministrador_RegistraIntentoEnAuditoria()
    {
        // Arrange
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var repositorio = new RepositorioUsuarioEnMemoria();
        var servicio = new ServicioUsuario(repositorio, auditoria);
        var admin = CrearUsuarioValido("admin@example.com");
        admin.AgregarRol(RolUsuario.AdministradorDelSistema);
        servicio.AgregarUsuario(admin);
        repoAuditoria.Entradas.Clear();

        // Act
        try { servicio.EliminarUsuario(admin); } catch (InvalidOperationException) { }

        // Assert
        Assert.IsTrue(repoAuditoria.Entradas.Any(e => e.Accion == "INTENTO_ELIMINAR_ULTIMO_ADMIN"),
            "Debe registrarse en auditoría el intento de eliminar al último administrador.");
    }

    [TestMethod]
    public void EliminarUsuario_ConMultiplesAdministradores_PermiteEliminarUno()
    {
        // Arrange
        var admin1 = CrearUsuarioValido("admin1@example.com");
        admin1.AgregarRol(RolUsuario.AdministradorDelSistema);
        _servicio.AgregarUsuario(admin1);
        var admin2 = CrearUsuarioValido("admin2@example.com");
        admin2.AgregarRol(RolUsuario.AdministradorDelSistema);
        _servicio.AgregarUsuario(admin2);

        // Act
        _servicio.EliminarUsuario(admin1);

        // Assert
        var usuarios = _servicio.ObtenerTodosLosUsuarios();
        Assert.AreEqual(1, usuarios.Count);
        Assert.IsTrue(usuarios[0].TieneRol(RolUsuario.AdministradorDelSistema));
    }

    [TestMethod]
    public void GestionarRoles_RemoverUltimoAdmin_NoCorrompeRolesDelUsuario()
    {
        // Arrange
        var admin = CrearUsuarioValido("admin@example.com");
        admin.AgregarRol(RolUsuario.AdministradorDelSistema);
        _servicio.AgregarUsuario(admin);

        // Act
        try { _servicio.GestionarRoles(admin, new[] { RolUsuario.Editor }); } catch (InvalidOperationException) { }

        // Assert — la excepción se lanzó ANTES de mutar la entidad
        Assert.IsTrue(admin.TieneRol(RolUsuario.AdministradorDelSistema),
            "El rol Administrador no debe haberse removido cuando la operación fue bloqueada.");
        Assert.IsFalse(admin.TieneRol(RolUsuario.Editor),
            "No debe haberse agregado el rol Editor cuando la operación fue bloqueada.");
    }

    [TestMethod]
    public void GestionarRoles_RemoverUltimoAdmin_RegistraIntentoEnAuditoria()
    {
        // Arrange
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var repositorio = new RepositorioUsuarioEnMemoria();
        var servicio = new ServicioUsuario(repositorio, auditoria);
        var admin = CrearUsuarioValido("admin@example.com");
        admin.AgregarRol(RolUsuario.AdministradorDelSistema);
        servicio.AgregarUsuario(admin);
        repoAuditoria.Entradas.Clear();

        // Act
        try { servicio.GestionarRoles(admin, new[] { RolUsuario.Editor }); } catch (InvalidOperationException) { }

        // Assert
        Assert.IsTrue(repoAuditoria.Entradas.Any(e => e.Accion == "INTENTO_QUITAR_ROL_ADMIN_ULTIMO"),
            "Debe registrarse en auditoría el intento de quitar el rol al último administrador.");
    }

    // --- ReiniciarContrasenia forzarCambio ---

    [TestMethod]
    public void ReiniciarContrasenia_ConForzarCambio_MarcaUsuarioParaCambioForzado()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        usuario.EstablecerContrasenia("ViejaPass@123");
        _servicio.AgregarUsuario(usuario);
        var admin = CrearUsuarioValido("admin@example.com");
        admin.AgregarRol(RolUsuario.AdministradorDelSistema);
        _servicio.AgregarUsuario(admin);

        // Act
        _servicio.ReiniciarContrasenia(usuario, admin, forzarCambio: true);

        // Assert
        Assert.IsTrue(usuario.DebeCambiarContrasenia,
            "El flag DebeCambiarContrasenia debe ser true cuando se reinicia con cambio forzado.");
    }

    [TestMethod]
    public void ReiniciarContrasenia_SinForzarCambio_NoMarcaUsuarioParaCambio()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        usuario.EstablecerContrasenia("ViejaPass@123");
        _servicio.AgregarUsuario(usuario);
        var admin = CrearUsuarioValido("admin@example.com");
        admin.AgregarRol(RolUsuario.AdministradorDelSistema);
        _servicio.AgregarUsuario(admin);

        // Act
        _servicio.ReiniciarContrasenia(usuario, admin, forzarCambio: false);

        // Assert
        Assert.IsFalse(usuario.DebeCambiarContrasenia,
            "El flag DebeCambiarContrasenia debe permanecer false cuando no se fuerza el cambio.");
    }

    [TestMethod]
    public void ReiniciarContrasenia_DevuelveContraseniaTemporalEnTextoPlano()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        usuario.EstablecerContrasenia("ViejaPass@123");
        _servicio.AgregarUsuario(usuario);
        var admin = CrearUsuarioValido("admin@example.com");
        admin.AgregarRol(RolUsuario.AdministradorDelSistema);
        _servicio.AgregarUsuario(admin);

        // Act
        var contraseniaTemporal = _servicio.ReiniciarContrasenia(usuario, admin);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(contraseniaTemporal),
            "ReiniciarContrasenia debe retornar la contraseña temporal en texto plano.");
        Assert.IsTrue(usuario.VerificarContrasenia(contraseniaTemporal),
            "La contraseña retornada debe coincidir con la almacenada en el usuario.");
    }

    [TestMethod]
    public void ReiniciarContrasenia_ConForzarCambio_RegistraDetalleEnAuditoria()
    {
        // Arrange
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var repositorio = new RepositorioUsuarioEnMemoria();
        var servicio = new ServicioUsuario(repositorio, auditoria);
        var usuario = CrearUsuarioValido("juan@example.com");
        usuario.EstablecerContrasenia("ViejaPass@123");
        servicio.AgregarUsuario(usuario);
        var admin = CrearUsuarioValido("admin@example.com");
        admin.AgregarRol(RolUsuario.AdministradorDelSistema);
        servicio.AgregarUsuario(admin);
        repoAuditoria.Entradas.Clear();

        // Act
        servicio.ReiniciarContrasenia(usuario, admin, forzarCambio: true);

        // Assert
        var entrada = repoAuditoria.Entradas.FirstOrDefault(e => e.Accion == "REINICIO_CONTRASENIA");
        Assert.IsNotNull(entrada, "Debe registrarse REINICIO_CONTRASENIA en auditoría.");
        StringAssert.Contains(entrada.Detalle, "cambio forzado",
            "El detalle de auditoría debe indicar que se forzó el cambio.");
    }

    [TestMethod]
    public void CambiarContrasenia_ConCambioForzadoPrevio_LimpiaFlagDebeCambiarContrasenia()
    {
        // Arrange
        var usuario = CrearUsuarioValido("juan@example.com");
        usuario.EstablecerContrasenia("ViejaPass@123");
        usuario.MarcarParaCambioForzado();

        // Act
        _servicio.CambiarContrasenia(usuario, "ViejaPass@123", "NuevaPass@456");

        // Assert
        Assert.IsFalse(usuario.DebeCambiarContrasenia,
            "Cambiar contraseña debe limpiar el flag de cambio forzado.");
    }

    private static Usuario CrearUsuarioConRol(RolUsuario rol)
    {
        var usuario = CrearUsuarioValido($"{rol.ToString().ToLower()}@example.com");
        usuario.AgregarRol(rol);
        return usuario;
    }

    private static Usuario CrearUsuarioValido(string email)
    {
        return new Usuario(new DatosUsuario
        {
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = email,
            FechaNacimiento = "01-01-1990"
        });
    }
}

