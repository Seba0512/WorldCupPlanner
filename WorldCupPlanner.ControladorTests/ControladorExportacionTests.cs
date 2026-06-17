using System.Text;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Controladores;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ControladorTests;

[TestClass]
public class ControladorExportacionTests
{
    [TestMethod]
    public void ExportarAuditoriasCsv_DelegaEnServicio()
    {
        var repo = new RepositorioAuditoriaFake();
        repo.Agregar(new Auditoria(1, new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc).ToString("o"),
            "admin", "Alta", "Dentro del rango"));
        repo.Agregar(new Auditoria(2, new DateTime(2026, 6, 3, 10, 0, 0, DateTimeKind.Utc).ToString("o"),
            "admin", "Baja", "Fuera del rango"));
        var controlador = CrearControlador(repo);

        var contenido = Encoding.UTF8.GetString(controlador.ExportarAuditoriasCsv(
            new DateTime(2026, 6, 1), new DateTime(2026, 6, 1), "Periodista"));

        StringAssert.Contains(contenido, "Dentro del rango");
        Assert.IsFalse(contenido.Contains("Fuera del rango"));
    }

    [TestMethod]
    public void ExportarAuditoriasCsv_ConRangoInvertido_MantieneValidacionDelServicio()
    {
        var controlador = CrearControlador(new RepositorioAuditoriaFake());

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            controlador.ExportarAuditoriasCsv(new DateTime(2026, 6, 2), new DateTime(2026, 6, 1), "admin"));
    }

    [TestMethod]
    public void ExportarAuditoriasXlsx_DelegaGeneracionExcel()
    {
        var repo = new RepositorioAuditoriaFake();
        repo.Agregar(new Auditoria(1, new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc).ToString("o"),
            "admin", "Alta", "Detalle"));
        var controlador = CrearControlador(repo);

        var bytes = controlador.ExportarAuditoriasXlsx(
            new DateTime(2026, 6, 1), new DateTime(2026, 6, 1), "Administrador");

        Assert.IsTrue(bytes.Length > 0);
        // Firma "PK" identifica el formato ZIP (base de XLSX)
        Assert.AreEqual((byte)'P', bytes[0]);
        Assert.AreEqual((byte)'K', bytes[1]);
    }

    [TestMethod]
    public void ExportarFixtureCsv_DelegaEnServicio()
    {
        var controlador = CrearControlador(new RepositorioAuditoriaFake(), CrearEstadoConFixture());

        var contenido = Encoding.UTF8.GetString(controlador.ExportarFixtureCsv("Administrador"));

        StringAssert.Contains(contenido, "Uruguay");
        StringAssert.Contains(contenido, "Final");
    }

    [TestMethod]
    public void ExportarFixtureXlsx_DelegaGeneracionExcel()
    {
        var controlador = CrearControlador(new RepositorioAuditoriaFake(), CrearEstadoConFixture());

        var bytes = controlador.ExportarFixtureXlsx("Administrador");

        Assert.IsTrue(bytes.Length > 0);
        // Firma "PK" identifica el formato ZIP (base de XLSX)
        Assert.AreEqual((byte)'P', bytes[0]);
        Assert.AreEqual((byte)'K', bytes[1]);
    }

    private static ControladorExportacion CrearControlador(
        IRepositorioConId<Auditoria> repositorio,
        EstadoTorneo? estadoTorneo = null)
    {
        var auditoria = new ServicioAuditoria(repositorio);
        var servicio = new ServicioExportacion(
            repositorio,
            estadoTorneo ?? new EstadoTorneo(),
            auditoria,
            new ServicioConsultasTorneo());
        return new ControladorExportacion(servicio);
    }

    private static EstadoTorneo CrearEstadoConFixture()
    {
        var uruguay = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        var francia = CrearEquipo("Francia", Confederacion.UEFA);
        var japon = CrearEquipo("Japon", Confederacion.AFC);
        var marruecos = CrearEquipo("Marruecos", Confederacion.CAF);
        var estadio = FabricaEntidades.CrearEstadio("Centenario");
        var grupo = new Grupo("A", [uruguay, francia, japon, marruecos]);
        grupo.Partidos.Add(new Partido
        {
            Id = 1,
            Fecha = new DateTime(2026, 6, 1, 14, 0, 0),
            Fase = FaseTorneo.FaseDeGrupos,
            Grupo = grupo,
            EquipoLocal = uruguay,
            EquipoVisitante = francia,
            Estadio = estadio,
            GolesLocal = 2,
            GolesVisitante = 1
        });

        return new EstadoTorneo
        {
            Fixture = new Fixture { Grupos = { grupo } },
            PartidosEliminatorios =
            [
                new Partido
                {
                    Id = 2,
                    Fecha = new DateTime(2026, 7, 19, 14, 0, 0),
                    Fase = FaseTorneo.Final,
                    EquipoLocal = uruguay,
                    EquipoVisitante = japon,
                    Estadio = estadio
                }
            ]
        };
    }

    private static Equipo CrearEquipo(string nombre, Confederacion confederacion) =>
        FabricaEntidades.CrearEquipo(nombre, confederacion, rankingFifa: 1000);
}



