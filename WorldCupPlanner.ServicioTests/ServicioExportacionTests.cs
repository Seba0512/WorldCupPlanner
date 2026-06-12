using System.Text;
using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioExportacionTests
{
    [TestMethod]
    public void ExportarAuditoriasCsv_FiltraRegistrosPorRangoDeFechas()
    {
        var repo = new RepositorioAuditoriaExportacionStub();
        repo.Agregar(new Auditoria(1, new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc).ToString("o"),
            "admin", "Alta", "Dentro del rango"));
        repo.Agregar(new Auditoria(2, new DateTime(2026, 6, 3, 10, 0, 0, DateTimeKind.Utc).ToString("o"),
            "admin", "Baja", "Fuera del rango"));
        var servicio = CrearServicio(repo);

        var contenido = Encoding.UTF8.GetString(servicio.ExportarAuditoriasCsv(
            new DateTime(2026, 6, 1), new DateTime(2026, 6, 1), "Periodista"));

        StringAssert.Contains(contenido, "Dentro del rango");
        Assert.IsFalse(contenido.Contains("Fuera del rango"));
    }

    [TestMethod]
    public void ExportarAuditoriasXlsx_GeneraArchivoExcel()
    {
        var repo = new RepositorioAuditoriaExportacionStub();
        repo.Agregar(new Auditoria(1, new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc).ToString("o"),
            "admin", "Alta", "Detalle"));
        var servicio = CrearServicio(repo);

        var bytes = servicio.ExportarAuditoriasXlsx(
            new DateTime(2026, 6, 1), new DateTime(2026, 6, 1), "Administrador");

        Assert.IsTrue(bytes.Length > 0);
        Assert.AreEqual((byte)'P', bytes[0]);
        Assert.AreEqual((byte)'K', bytes[1]);
    }

    [TestMethod]
    public void ExportarAuditoriasCsv_ConRangoInvertido_LanzaExcepcion()
    {
        var servicio = CrearServicio(new RepositorioAuditoriaExportacionStub());

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicio.ExportarAuditoriasCsv(new DateTime(2026, 6, 2), new DateTime(2026, 6, 1), "admin"));
    }

    [TestMethod]
    public void ExportarFixtureCsv_IncluyePartidosDeGruposYEliminatorias()
    {
        var estado = CrearEstadoConFixtureCompleto();
        var servicio = CrearServicio(new RepositorioAuditoriaExportacionStub(), estado);

        var contenido = Encoding.UTF8.GetString(servicio.ExportarFixtureCsv("Administrador"));

        StringAssert.Contains(contenido, "Uruguay");
        StringAssert.Contains(contenido, "Final");
    }

    [TestMethod]
    public void ExportarFixtureXlsx_GeneraArchivoExcel()
    {
        var estado = CrearEstadoConFixtureCompleto();
        var servicio = CrearServicio(new RepositorioAuditoriaExportacionStub(), estado);

        var bytes = servicio.ExportarFixtureXlsx("Administrador");

        Assert.IsTrue(bytes.Length > 0);
        Assert.AreEqual((byte)'P', bytes[0]);
        Assert.AreEqual((byte)'K', bytes[1]);
    }

    private static ServicioExportacion CrearServicio(
        RepositorioAuditoriaExportacionStub repo,
        EstadoTorneo? estadoTorneo = null)
    {
        var auditoria = new ServicioAuditoria(repo);
        return new ServicioExportacion(repo, estadoTorneo ?? new EstadoTorneo(), auditoria, new ServicioConsultasTorneo());
    }

    private static EstadoTorneo CrearEstadoConFixtureCompleto()
    {
        var uruguay = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        var francia = CrearEquipo("Francia", Confederacion.UEFA);
        var japon = CrearEquipo("Japon", Confederacion.AFC);
        var marruecos = CrearEquipo("Marruecos", Confederacion.CAF);
        var estadio = new Estadio { Nombre = "Centenario", Ciudad = "Montevideo", Capacidad = 60000 };
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

    private static Equipo CrearEquipo(string nombre, Confederacion confederacion) => new()
    {
        Nombre = nombre,
        RankingFIFA = 1000,
        RankingInicial = 1000,
        Confederacion = confederacion
    };
}

internal sealed class RepositorioAuditoriaExportacionStub : IRepositorioConId<Auditoria>
{
    private readonly List<Auditoria> _logs = [];
    private int _contador;

    public int SiguienteId() => ++_contador;

    public void Agregar(Auditoria entidad) => _logs.Add(entidad);

    public void Actualizar(Auditoria entidad)
    {
        var indice = _logs.FindIndex(e => e.Id == entidad.Id);
        if (indice >= 0)
            _logs[indice] = entidad;
    }

    public void Eliminar(Auditoria entidad) => _logs.Remove(entidad);

    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado) => _logs.Where(predicado).ToList();
}
