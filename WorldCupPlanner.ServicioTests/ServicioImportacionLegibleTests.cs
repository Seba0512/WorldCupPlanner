using System.Text;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioImportacionLegibleTests
{
    private ServicioImportacionLegible _servicio = null!;
    private ServicioEquipo _servicioEquipo = null!;

    [TestInitialize]
    public void Setup()
    {
        _servicio = new ServicioImportacionLegible();
        _servicioEquipo = new ServicioEquipo(new RepositorioEquipoStub());
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
    public void Importar_NombreVacio_ReportaErrorEspecifico()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            ",CONMEBOL,1000");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
        StringAssert.Contains(resultado.Errores[0], "Nombre");
    }

    [TestMethod]
    public void Importar_RankingFIFAPorEncimaDelMaximo_ReportaError()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA",
            "Uruguay,CONMEBOL,99999");

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

    // ── Columna Bandera ──────────────────────────────────────────────────────

    [TestMethod]
    public void Importar_CSVConEncabezadoBanderaYDatosCompletos_ImportaEquiposConBandera()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA,Bandera",
            $"Uruguay,CONMEBOL,1000,{SvgDePrueba.DataUri}",
            $"Argentina,CONMEBOL,1100,{SvgDePrueba.DataUri}");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(2, resultado.Exitosos);
        Assert.AreEqual(0, resultado.Errores.Count);
        var uruguay = _servicioEquipo.Buscar(e => e.Nombre == "Uruguay")[0];
        Assert.AreEqual(SvgDePrueba.DataUri, uruguay.Bandera);
    }

    [TestMethod]
    public void Importar_CSVConBanderaParcialmenteCompletada_ImportaTodosLosEquipos()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA,Bandera",
            $"Uruguay,CONMEBOL,1000,{SvgDePrueba.DataUri}",
            "Argentina,CONMEBOL,1100,",
            $"Brasil,CONMEBOL,800,{SvgDePrueba.DataUri}");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(3, resultado.Exitosos);
        Assert.AreEqual(0, resultado.Errores.Count);
        var argentina = _servicioEquipo.Buscar(e => e.Nombre == "Argentina")[0];
        Assert.AreEqual("", argentina.Bandera);
    }

    [TestMethod]
    public void Importar_CSVConBanderaVaciaEnTodasLasFilas_ImportaSinErrores()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA,Bandera",
            "Uruguay,CONMEBOL,1000,",
            "España,UEFA,1200,");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(2, resultado.Exitosos);
        Assert.AreEqual(0, resultado.Errores.Count);
    }

    [TestMethod]
    public void Importar_CSVSinColumnaBandera_MantieneCompatibilidadRetroactiva()
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
    }

    [TestMethod]
    public void Importar_CSVConEncabezadoBandera_FilaConTresColumnas_ReportaError()
    {
        // Arrange — el encabezado declara Bandera pero la fila no la tiene
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA,Bandera",
            "Uruguay,CONMEBOL,1000");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
        StringAssert.Contains(resultado.Errores[0], "4 columnas");
    }

    [TestMethod]
    public void Importar_CSVConEncabezadoBandera_SoloUnaFilaSinBandera_ImportaRestoyReportaError()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA,Bandera",
            $"Uruguay,CONMEBOL,1000,{SvgDePrueba.DataUri}",
            "Argentina,CONMEBOL,1100",
            $"Brasil,CONMEBOL,800,{SvgDePrueba.DataUri}");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(2, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
    }

    [TestMethod]
    public void Importar_CasoCritico_CincoEquiposBanderaMixta_ImportaTodos()
    {
        // Arrange — caso crítico: mayoría con bandera, algunos sin; deben coexistir sin rechazos
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA,Bandera",
            $"Uruguay,CONMEBOL,1000,{SvgDePrueba.DataUri}",
            "Argentina,CONMEBOL,1100,",
            $"Brasil,CONMEBOL,800,{SvgDePrueba.DataUri}",
            $"Chile,CONMEBOL,900,{SvgDePrueba.DataUri}",
            "Paraguay,CONMEBOL,1200,");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(5, resultado.Exitosos);
        Assert.AreEqual(0, resultado.Errores.Count);
        Assert.AreEqual(5, _servicioEquipo.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Importar_CSVConBanderaEnFormatoInvalido_ReportaErrorEspecifico()
    {
        // Arrange — valor que no es Data URI Base64
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA,Bandera",
            "Uruguay,CONMEBOL,1000,uruguay.png");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
        StringAssert.Contains(resultado.Errores[0], "Bandera");
        StringAssert.Contains(resultado.Errores[0], "Base64");
    }

    [TestMethod]
    public void Importar_CSVConBanderaConPrefijoDataPeroSinMarcadorBase64_ReportaError()
    {
        // Arrange — tiene prefijo "data:" pero falta ";base64,"
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA,Bandera",
            "Uruguay,CONMEBOL,1000,data:image/png,dXJ1Z3VheQ==");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
        StringAssert.Contains(resultado.Errores[0], "Bandera");
    }

    [TestMethod]
    public void Importar_BanderaFaltante_AlmacenaValorPorDefecto()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA,Bandera",
            "Uruguay,CONMEBOL,1000,",
            "Argentina,CONMEBOL,1100,");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert — valor por defecto para bandera faltante
        Assert.AreEqual(2, resultado.Exitosos);
        Assert.AreEqual(0, resultado.Errores.Count);
        var uruguay = _servicioEquipo.Buscar(e => e.Nombre == "Uruguay")[0];
        var argentina = _servicioEquipo.Buscar(e => e.Nombre == "Argentina")[0];
        Assert.AreEqual(string.Empty, uruguay.Bandera);
        Assert.AreEqual(string.Empty, argentina.Bandera);
    }

    [TestMethod]
    public void Importar_CSVConBanderaQueSuperaLongitudMaxima_ReportaErrorEspecifico()
    {
        // Arrange — bandera de 2.800.001 caracteres (supera el límite de 2.800.000)
        var prefijo = "data:image/svg+xml;base64,";
        var banderaGigante = prefijo + new string('A', 2_800_001 - prefijo.Length);
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA,Bandera",
            $"Uruguay,CONMEBOL,1000,{banderaGigante}");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
        StringAssert.Contains(resultado.Errores[0], "Bandera");
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

    // ── Bandera: URL SVG ─────────────────────────────────────────────────────

    [TestMethod]
    public void Importar_CSVConBanderaComoUrlSvg_DescargaYAlmacenaEnBase64()
    {
        // Arrange
        var descargador = new DescargadorStub(SvgDePrueba.Contenido);
        var servicio = new ServicioImportacionLegible(descargador: descargador);
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA,Bandera",
            "Uruguay,CONMEBOL,1000,https://flagcdn.com/uy.svg");

        // Act
        var resultado = servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(1, resultado.Exitosos);
        Assert.AreEqual(0, resultado.Errores.Count);
        var uruguay = _servicioEquipo.Buscar(e => e.Nombre == "Uruguay")[0];
        Assert.AreEqual(SvgDePrueba.DataUri, uruguay.Bandera);
    }

    [TestMethod]
    public void Importar_CSVConBanderaUrlInaccesible_ReportaError()
    {
        // Arrange
        var descargador = new DescargadorQueSimulaError();
        var servicio = new ServicioImportacionLegible(descargador: descargador);
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA,Bandera",
            "Uruguay,CONMEBOL,1000,https://flagcdn.com/uy.svg");

        // Act
        var resultado = servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
        StringAssert.Contains(resultado.Errores[0], "Bandera");
    }

    [TestMethod]
    public void Importar_CSVConBanderaUrlQueNoEsSvg_ReportaError()
    {
        // Arrange
        var descargador = new DescargadorStub("<html><body>not an svg</body></html>");
        var servicio = new ServicioImportacionLegible(descargador: descargador);
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA,Bandera",
            "Uruguay,CONMEBOL,1000,https://example.com/imagen.png");

        // Act
        var resultado = servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
        StringAssert.Contains(resultado.Errores[0], "Bandera");
        StringAssert.Contains(resultado.Errores[0], "SVG");
    }

    [TestMethod]
    public void Importar_CSVConBanderaUrlSinDescargadorConfigurado_ReportaError()
    {
        // Arrange — sin descargador: las URLs no pueden resolverse
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA,Bandera",
            "Uruguay,CONMEBOL,1000,https://flagcdn.com/uy.svg");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
        StringAssert.Contains(resultado.Errores[0], "Bandera");
    }

    // ── Bandera: Base64 puro ─────────────────────────────────────────────────

    [TestMethod]
    public void Importar_CSVConBanderaBase64PuroSvgValido_AlmacenaComoDataUri()
    {
        // Arrange
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA,Bandera",
            $"Uruguay,CONMEBOL,1000,{SvgDePrueba.Base64}");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(1, resultado.Exitosos);
        Assert.AreEqual(0, resultado.Errores.Count);
        var uruguay = _servicioEquipo.Buscar(e => e.Nombre == "Uruguay")[0];
        Assert.AreEqual(SvgDePrueba.DataUri, uruguay.Bandera);
    }

    [TestMethod]
    public void Importar_CSVConBanderaBase64PuroQueNoEsSvg_ReportaError()
    {
        // Arrange — Base64 de contenido HTML (no SVG)
        var base64Html = Convert.ToBase64String(Encoding.UTF8.GetBytes("<html></html>"));
        var csv = CrearCSV(
            "Nombre,Confederacion,RankingFIFA,Bandera",
            $"Uruguay,CONMEBOL,1000,{base64Html}");

        // Act
        var resultado = _servicio.Importar(csv, _servicioEquipo);

        // Assert
        Assert.AreEqual(0, resultado.Exitosos);
        Assert.AreEqual(1, resultado.Errores.Count);
        StringAssert.Contains(resultado.Errores[0], "Bandera");
        StringAssert.Contains(resultado.Errores[0], "SVG");
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

// SVG mínimo de prueba y sus representaciones derivadas.
// Todos los tests que necesiten un SVG válido deben usar estas constantes.
file static class SvgDePrueba
{
    internal const string Contenido = "<svg xmlns=\"http://www.w3.org/2000/svg\"></svg>";
    internal static readonly string Base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(Contenido));
    internal static string DataUri => "data:image/svg+xml;base64," + Base64;
}

file sealed class DescargadorStub : IDescargadorContenido
{
    private readonly string _contenido;
    public DescargadorStub(string contenido) => _contenido = contenido;
    public string Descargar(string url) => _contenido;
}

file sealed class DescargadorQueSimulaError : IDescargadorContenido
{
    public string Descargar(string url) =>
        throw new InvalidOperationException($"No se pudo conectar a \"{url}\".");
}

file sealed class RepositorioEquipoStub : IRepositorio<Equipo>
{
    private readonly List<Equipo> _equipos = new();

    public void Agregar(Equipo equipo) => _equipos.Add(equipo);

    public void Actualizar(Equipo equipo)
    {
        var indice = _equipos.FindIndex(e => ReferenceEquals(e, equipo));
        if (indice == -1)
            throw new EntidadInexistenteExcepcion("El equipo no existe.");
        _equipos[indice] = equipo;
    }

    public void Eliminar(Equipo equipo) => _equipos.Remove(equipo);

    public List<Equipo> Buscar(Func<Equipo, bool> predicado)
        => _equipos.Where(predicado).ToList();
}
