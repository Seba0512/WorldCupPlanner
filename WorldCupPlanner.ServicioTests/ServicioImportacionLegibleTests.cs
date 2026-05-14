using System.Text;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioImportacionLegibleTests
{
    private ServicioImportacionLegible _servicio;
    private ServicioEquipo _servicioEquipo;

    [TestInitialize]
    public void Setup()
    {
        _servicio = new ServicioImportacionLegible();
        _servicioEquipo = new ServicioEquipo(new RepositorioEquipoEnMemoria());
    }

    // ── Importación CSV ──────────────────────────────────────────────────────

    [TestMethod]
    public void Importar_CSVValido_ImportaTodosLosEquipos()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,1000",
            "España,UEFA,1200",
            "Marruecos,CAF,900");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(3, resultado.Exitosos);
        Assert.AreEqual(0, resultado.Errores.Count);
        Assert.AreEqual(3, _servicioEquipo.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Importar_CSVParcial_ImportaEquiposDisponibles()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,1000",
            "Argentina,CONMEBOL,1100");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(2, resultado.Exitosos);
        Assert.AreEqual(0, resultado.Errores.Count);
        Assert.AreEqual(2, _servicioEquipo.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Importar_CSVConFilasInvalidas_ImportaSoloLasValidas()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,1000",
            "Equipo Invalido,CONFEDERACION_FAKE,999",
            "España,UEFA,1200");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(2, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
        Assert.AreEqual(2, _servicioEquipo.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Importar_CSVConColumnasIncorrectas_ReportaErrorPorFila()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL",
            "España,UEFA,1200,CampoExtra");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(2, resultado.Errores.Count);
    }

    [TestMethod]
    public void Importar_CSVVacioSoloEncabezado_RetornaCeroExitososSinErrores()
    {
        // Arrange
        var csv = CrearCSV("Nombre,Confederacion,RankingFIFA");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(0, resultado.Errores.Count);
    }

    [TestMethod]
    public void Importar_CSVConEncabezadoIncorrecto_RetornaErrorDeEncabezado()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,Ranking",
            "Uruguay,CONMEBOL,1000");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
        StringAssert.Contains(resultado.Errores[0], "Encabezado inválido");
    }

    [TestMethod]
    public void Importar_CSVSinEncabezado_RetornaErrorDeEncabezado()
    {
        // Arrange
        var csv = new MemoryStream(Array.Empty<byte>());

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
        StringAssert.Contains(resultado.Errores[0], "Encabezado");
    }

    [TestMethod]
    public void Importar_CSVConDuplicadosInternos_SegundoRegistroFalla()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,1000",
            "Uruguay,CONMEBOL,1100");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(1, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
        StringAssert.Contains(resultado.Errores[0], "Uruguay");
    }

    [TestMethod]
    public void Importar_CSVConDuplicadosContraExistentes_ReportaError()
    {
        // Arrange
        _servicioEquipo.Agregar(new Equipo { Nombre = "Uruguay", Confederacion = Confederacion.CONMEBOL, RankingFIFA = 1000 });
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,1000");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
        StringAssert.Contains(resultado.Errores[0], "Uruguay");
    }

    [TestMethod]
    public void Importar_RankingFIFANoNumerico_ReportaErrorEspecifico()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,MIL");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
        StringAssert.Contains(resultado.Errores[0], "RankingFIFA");
    }

    [TestMethod]
    public void Importar_ConfederacionInvalida_ReportaErrorEspecifico()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONFEDERACIONFALSA,1000");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
        StringAssert.Contains(resultado.Errores[0], "confederación inválida");
    }

    [TestMethod]
    public void Importar_RankingFIFAFueraDeRango_ReportaError()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,100");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
    }

    [TestMethod]
    public void Importar_CSVConLineasVacias_LasIgnora()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "",
            "Uruguay,CONMEBOL,1000",
            "   ");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(1, resultado.Exitosos);
        Assert.AreEqual(0, resultado.Errores.Count);
    }

    // ── Estado parcial ───────────────────────────────────────────────────────

    [TestMethod]
    public void Importar_Parcial_PersistirEquiposCorrectamente()
    {
        // Arrange
        var csv1 = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,1000",
            "Argentina,CONMEBOL,1100");

        var csv2 = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "España,UEFA,1200",
            "Francia,UEFA,1300");

        // Act
        _servicio.Importar(csv1, _servicioEquipo);
        _servicio.Importar(csv2, _servicioEquipo);

        // Assert
        Assert.AreEqual(4, _servicioEquipo.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void ImportarParcial_LuegoAgregarManualmente_AmbosEquiposPresentes()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,1000");

        // Act
        _servicio.Importar(csv, _servicioEquipo);
        _servicioEquipo.Agregar(new Equipo { Nombre = "España", Confederacion = Confederacion.UEFA, RankingFIFA = 1200 });

        // Assert
        Assert.AreEqual(2, _servicioEquipo.Buscar(_ => true).Count);
        Assert.AreEqual(1, _servicioEquipo.Buscar(e => e.Nombre == "Uruguay").Count);
        Assert.AreEqual(1, _servicioEquipo.Buscar(e => e.Nombre == "España").Count);
    }

    [TestMethod]
    public void ImportarParcial_LuegoCompletarAutomaticamente_AlcanzaMaximo()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,1000",
            "España,UEFA,1200");

        // Act
        _servicio.Importar(csv, _servicioEquipo);
        _servicioEquipo.CompletarEquipos(42);

        // Assert
        Assert.AreEqual(48, _servicioEquipo.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void ImportarParcial_CompletarAutomaticamente_RespetaCuposPorConfederacion()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,1000",
            "España,UEFA,1200");

        // Act
        _servicio.Importar(csv, _servicioEquipo);
        _servicioEquipo.CompletarEquipos(42);

        // Assert
        Assert.AreEqual(16, _servicioEquipo.Buscar(e => e.Confederacion == Confederacion.UEFA).Count);
        Assert.AreEqual(7, _servicioEquipo.Buscar(e => e.Confederacion == Confederacion.CONMEBOL).Count);
        Assert.AreEqual(9, _servicioEquipo.Buscar(e => e.Confederacion == Confederacion.CAF).Count);
        Assert.AreEqual(8, _servicioEquipo.Buscar(e => e.Confederacion == Confederacion.AFC).Count);
        Assert.AreEqual(7, _servicioEquipo.Buscar(e => e.Confederacion == Confederacion.CONCACAF).Count);
        Assert.AreEqual(1, _servicioEquipo.Buscar(e => e.Confederacion == Confederacion.OFC).Count);
    }

    [TestMethod]
    public void Importar_ConCupoConfederacionLlena_ReportaError()
    {
        // Arrange — llenar OFC (cupo = 1)
        _servicioEquipo.Agregar(new Equipo { Nombre = "Nueva Zelanda", Confederacion = Confederacion.OFC, RankingFIFA = 900 });
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Australia,OFC,800");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
        StringAssert.Contains(resultado.Errores[0], "OFC");
    }

    [TestMethod]
    public void Importar_NoBloqueaImportacionesAdicionales_PermiteAgregarMasEquipos()
    {
        // Arrange
        var csv1 = CrearCSV("Nombre,Confederacion,RankingFIFA", "Uruguay,CONMEBOL,1000");
        var csv2 = CrearCSV("Nombre,Confederacion,RankingFIFA", "Argentina,CONMEBOL,1100");

        // Act
        _servicio.Importar(csv1, _servicioEquipo);
        var resultado2 = _servicio.Importar(csv2, _servicioEquipo);

        // Assert — no hay excepciones, segunda importación funciona correctamente
        Assert.AreEqual(1, resultado2.Exitosos);
        Assert.AreEqual(2, _servicioEquipo.Buscar(_ => true).Count);
    }

    // ── Auditoría ────────────────────────────────────────────────────────────

    [TestMethod]
    public void Importar_ConAuditoria_RegistraEntradaDeAuditoria()
    {
        // Arrange
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var servicio = new ServicioImportacionLegible(auditoria);
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,1000");

        // Act
        servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(1, repoAuditoria.Entradas.Count);
        Assert.AreEqual("Importacion", repoAuditoria.Entradas[0].Accion);
    }

    [TestMethod]
    public void Importar_ConUsuarioSolicitante_RegistraEmailEnAuditoria()
    {
        // Arrange
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var servicio = new ServicioImportacionLegible(auditoria);
        var usuario = CrearUsuarioConRol(RolUsuario.Editor);
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,1000");

        // Act
        servicio.Importar(csv, _servicioEquipo, usuario);

        // Assert
        Assert.AreEqual(1, repoAuditoria.Entradas.Count);
        Assert.AreEqual("test@example.com", repoAuditoria.Entradas[0].Usuario);
    }

    [TestMethod]
    public void Importar_SinUsuarioSolicitante_RegistraSistemaEnAuditoria()
    {
        // Arrange
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var servicio = new ServicioImportacionLegible(auditoria);
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,1000");

        // Act
        servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual("Sistema", repoAuditoria.Entradas[0].Usuario);
    }

    [TestMethod]
    public void Importar_SinAuditoria_NoLanzaExcepcion()
    {
        // Arrange
        var servicio = new ServicioImportacionLegible();
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,1000");

        // Act & Assert
        var resultado = servicio.Importar(csv, _servicioEquipo);
        Assert.AreEqual(1, resultado.Exitosos);
    }

    // ── Autorización ─────────────────────────────────────────────────────────

    [TestMethod]
    public void Importar_ConSolicitanteEditor_ImportaCorrectamente()
    {
        // Arrange
        var editor = CrearUsuarioConRol(RolUsuario.Editor);
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,1000");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo, editor);

        // Assert
        Assert.AreEqual(1, resultado.Exitosos);
    }

    [TestMethod]
    public void Importar_ConSolicitanteAdministrador_ImportaCorrectamente()
    {
        // Arrange
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,1000");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo, admin);

        // Assert
        Assert.AreEqual(1, resultado.Exitosos);
    }

    [TestMethod]
    public void Importar_ConSolicitanteSinRol_LanzaAccesoDenegado()
    {
        // Arrange
        var sinRol = CrearUsuarioSinRoles();
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,1000");

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.Importar(csv, _servicioEquipo, sinRol));
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static Stream CrearCSV(params string[] lineas)
    {
        var contenido = string.Join("\n", lineas);
        var bytes = Encoding.UTF8.GetBytes(contenido);
        return new MemoryStream(bytes);
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
