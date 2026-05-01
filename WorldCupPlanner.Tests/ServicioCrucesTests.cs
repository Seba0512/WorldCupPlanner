using WorldCupPlanner.Domain;
using WorldCupPlanner.Services;

namespace WorldCupPlanner.Tests;

[TestClass]
public class ServicioCrucesTests
{
    [TestMethod]
    public void GenerarPrimeraRondaEliminatoria_Con32Clasificados_DebeGenerar16Partidos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var clasificados = CrearClasificadosCon32Equipos();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCrucesFase = 1
        };

        // Act
        var partidos = servicioCruces.GenerarPrimeraRondaEliminatoria(
            clasificados,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(16, partidos.Count);
        Assert.IsTrue(partidos.All(p => p.Fase == FaseTorneo.PrimeraRondaEliminatoria));
    }

    private ClasificadosFaseGrupos CrearClasificadosCon32Equipos()
    {
        return new ClasificadosFaseGrupos
        {
            Primeros = CrearEquipos("Primero", 12),
            Segundos = CrearEquipos("Segundo", 12),
            MejoresTerceros = CrearEquipos("Tercero", 8)
        };
    }

    private List<Equipo> CrearEquipos(string prefijo, int cantidad)
    {
        var equipos = new List<Equipo>();

        for (int i = 1; i <= cantidad; i++)
        {
            equipos.Add(new Equipo
            {
                Nombre = $"{prefijo}{i}",
                RankingFIFA = 1000 + i,
                Confederacion = Confederacion.UEFA
            });
        }

        return equipos;
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
    public void GenerarPrimeraRondaEliminatoria_DebeCruzarOchoPrimerosConOchoTerceros()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var clasificados = CrearClasificadosCon32Equipos();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCrucesFase = 1
        };

        // Act
        var partidos = servicioCruces.GenerarPrimeraRondaEliminatoria(
            clasificados,
            estadios,
            configuracionTorneo
        );

        // Assert
        var primerosOchoPartidos = partidos.Take(8).ToList();

        for (int i = 0; i < primerosOchoPartidos.Count; i++)
        {
            Assert.IsTrue(
                clasificados.Primeros.Take(8).Contains(primerosOchoPartidos[i].EquipoLocal)
            );

            Assert.IsTrue(
                clasificados.MejoresTerceros.Contains(primerosOchoPartidos[i].EquipoVisitante)
            );
        }
    }
    
    [TestMethod]
    public void GenerarPrimeraRondaEliminatoria_DebeCruzarPrimerosRestantesConSegundosDeMenorRendimiento()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var clasificados = CrearClasificadosCon32Equipos();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCrucesFase = 1
        };

        // Act
        var partidos = servicioCruces.GenerarPrimeraRondaEliminatoria(
            clasificados,
            estadios,
            configuracionTorneo
        );

        // Assert
        var partidosPrimerosRestantesContraSegundos = partidos
            .Skip(8)
            .Take(4)
            .ToList();

        var primerosRestantes = clasificados.Primeros
            .Skip(8)
            .Take(4)
            .ToList();

        var segundosDeMenorRendimiento = clasificados.Segundos
            .Skip(8)
            .Take(4)
            .ToList();

        for (int i = 0; i < partidosPrimerosRestantesContraSegundos.Count; i++)
        {
            Assert.AreEqual(primerosRestantes[i], partidosPrimerosRestantesContraSegundos[i].EquipoLocal);
            Assert.AreEqual(segundosDeMenorRendimiento[i], partidosPrimerosRestantesContraSegundos[i].EquipoVisitante);
        }
    }
}