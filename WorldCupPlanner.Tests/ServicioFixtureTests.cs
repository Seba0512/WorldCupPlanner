using WorldCupPlanner.Domain;
using WorldCupPlanner.Services;

namespace WorldCupPlanner.Tests;

[TestClass]
public class ServicioFixtureTests
{
    [TestMethod]
    public void GenerarFixture_ConMenosDe48Equipos_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = new List<Equipo>();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo)
        );
    }

    private List<Estadio> CrearEstadiosValidos(int cantidad)
    {
        var estadios = new List<Estadio>();

        for (int i = 1; i <= cantidad; i++)
        {
            estadios.Add(new Estadio
            {
                Nombre = $"Estadio{i}",
                Ciudad = $"Ciudad{i}",
                Capacidad = 30000
            });
        }

        return estadios;
    }
    
    [TestMethod]
    public void GenerarFixture_ConMenosDe4Estadios_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidos(48);
        var estadios = CrearEstadiosValidos(3);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo)
        );
    }

    private List<Equipo> CrearEquiposValidos(int cantidad)
    {
        var equipos = new List<Equipo>();
        var confederaciones = new[]
        {
            Confederacion.UEFA,
            Confederacion.CONMEBOL,
            Confederacion.CONCACAF,
            Confederacion.CAF,
            Confederacion.AFC,
            Confederacion.OFC
        };

        for (int i = 1; i <= cantidad; i++)
        {
            equipos.Add(new Equipo
            {
                Nombre = $"Equipo{i}",
                RankingFIFA = 500 + i,
                Confederacion = confederaciones[(i - 1) % confederaciones.Length]
            });
        }

        return equipos;
    }
    
    [TestMethod]
    public void GenerarFixture_Con48EquiposY4Estadios_DebeGenerar12Grupos()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidos(48);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        Assert.AreEqual(12, fixture.Grupos.Count);
    }
    
    [TestMethod]
    public void GenerarFixture_DebeCrearGruposConEtiquetasDeAToL()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidos(48);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        var etiquetasEsperadas = new List<string>
        {
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L"
        };

        var etiquetasObtenidas = fixture.Grupos.Select(g => g.Etiqueta).ToList();

        CollectionAssert.AreEqual(etiquetasEsperadas, etiquetasObtenidas);
    }
    
    [TestMethod]
    public void GenerarFixture_DebeCrearGruposConCuatroEquiposCadaUno()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidos(48);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        Assert.IsTrue(fixture.Grupos.All(g => g.Equipos.Count == 4));
    }
}