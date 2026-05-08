using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioSimulacionTests
{
    [TestMethod]
    public void SimularResultados_ConPartidosSinResultado_DebeAsignarGoles()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();
        var partidos = CrearPartidosSinResultado(5);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaSimulation = 1
        };

        // Act
        servicioSimulacion.SimularResultados(partidos, configuracionTorneo);

        // Assert
        Assert.IsTrue(partidos.All(p => p.GolesLocal.HasValue));
        Assert.IsTrue(partidos.All(p => p.GolesVisitante.HasValue));
    }

    private List<Partido> CrearPartidosSinResultado(int cantidad)
    {
        var partidos = new List<Partido>();

        for (int i = 1; i <= cantidad; i++)
        {
            partidos.Add(new Partido
            {
                Id = i,
                EquipoLocal = CrearEquipo($"Local{i}"),
                EquipoVisitante = CrearEquipo($"Visitante{i}"),
                Estadio = CrearEstadio(),
                Fecha = DateTime.Now,
                Fase = FaseTorneo.FaseDeGrupos
            });
        }

        return partidos;
    }

    private Equipo CrearEquipo(string nombre)
    {
        return new Equipo
        {
            Nombre = nombre,
            RankingFIFA = 1000,
            Confederacion = Confederacion.UEFA
        };
    }

    private Estadio CrearEstadio()
    {
        return new Estadio
        {
            Nombre = "Estadio",
            Ciudad = "Ciudad",
            Capacidad = 30000
        };
    }
    
    private List<string> ObtenerResultados(List<Partido> partidos)
    {
        return partidos
            .Select(p => $"{p.GolesLocal}-{p.GolesVisitante}")
            .ToList();
    }
    
    [TestMethod]
    public void SimularResultados_ConMismaSemilla_DebeGenerarMismosResultados()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();

        var partidos1 = CrearPartidosSinResultado(10);
        var partidos2 = CrearPartidosSinResultado(10);

        var configuracionTorneo1 = new ConfiguracionTorneo
        {
            SemillaSimulation = 7
        };

        var configuracionTorneo2 = new ConfiguracionTorneo
        {
            SemillaSimulation = 7
        };

        // Act
        servicioSimulacion.SimularResultados(partidos1, configuracionTorneo1);
        servicioSimulacion.SimularResultados(partidos2, configuracionTorneo2);

        // Assert
        var resultados1 = ObtenerResultados(partidos1);
        var resultados2 = ObtenerResultados(partidos2);

        CollectionAssert.AreEqual(resultados1, resultados2);
    }
}