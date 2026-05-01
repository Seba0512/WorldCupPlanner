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

        var locales = partidosPrimerosRestantesContraSegundos
            .Select(p => p.EquipoLocal)
            .ToList();

        var visitantes = partidosPrimerosRestantesContraSegundos
            .Select(p => p.EquipoVisitante)
            .ToList();

        CollectionAssert.AreEquivalent(primerosRestantes, locales);
        CollectionAssert.AreEquivalent(segundosDeMenorRendimiento, visitantes);
    }
    
    [TestMethod]
    public void GenerarPrimeraRondaEliminatoria_DebeCruzarSegundosRestantesEntreSi()
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
        var partidosSegundosRestantes = partidos
            .Skip(12)
            .Take(4)
            .ToList();

        var segundosRestantes = clasificados.Segundos
            .Take(8)
            .ToList();

        for (int i = 0; i < partidosSegundosRestantes.Count; i++)
        {
            Assert.IsTrue(segundosRestantes.Contains(partidosSegundosRestantes[i].EquipoLocal));
            Assert.IsTrue(segundosRestantes.Contains(partidosSegundosRestantes[i].EquipoVisitante));
            Assert.AreNotEqual(
                partidosSegundosRestantes[i].EquipoLocal,
                partidosSegundosRestantes[i].EquipoVisitante
            );
        }

        var equiposUsados = partidosSegundosRestantes
            .SelectMany(p => new[] { p.EquipoLocal, p.EquipoVisitante })
            .ToList();

        CollectionAssert.AreEquivalent(segundosRestantes, equiposUsados);
    }

    [TestMethod]
    public void GenerarPrimeraRondaEliminatoria_ConMismaSemilla_DebeGenerarMismosCruces()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var clasificados = CrearClasificadosCon32Equipos();
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo1 = new ConfiguracionTorneo { SemillaCrucesFase = 7 };
        var configuracionTorneo2 = new ConfiguracionTorneo { SemillaCrucesFase = 7 };

        // Act
        var partidos1 = servicioCruces.GenerarPrimeraRondaEliminatoria(
            clasificados,
            estadios,
            configuracionTorneo1
        );

        var partidos2 = servicioCruces.GenerarPrimeraRondaEliminatoria(
            clasificados,
            estadios,
            configuracionTorneo2
        );

        // Assert
        var cruces1 = ObtenerCruces(partidos1);
        var cruces2 = ObtenerCruces(partidos2);

        CollectionAssert.AreEqual(cruces1, cruces2);
    }

    [TestMethod]
    public void GenerarPrimeraRondaEliminatoria_ConDistintasSemillas_DebePermitirCrucesDistintos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var clasificados = CrearClasificadosCon32Equipos();
        var estadios = CrearEstadiosValidos(4);

        var configuracionReferencia = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

        // Act
        var partidosReferencia = servicioCruces.GenerarPrimeraRondaEliminatoria(
            clasificados,
            estadios,
            configuracionReferencia
        );

        var crucesReferencia = ObtenerCruces(partidosReferencia);
        var existeCruceDistinto = false;

        for (int semilla = 2; semilla <= 20; semilla++)
        {
            var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = semilla };

            var partidos = servicioCruces.GenerarPrimeraRondaEliminatoria(
                clasificados,
                estadios,
                configuracionTorneo
            );

            var cruces = ObtenerCruces(partidos);

            if (!cruces.SequenceEqual(crucesReferencia))
            {
                existeCruceDistinto = true;
                break;
            }
        }

        // Assert
        Assert.IsTrue(existeCruceDistinto);
    }

    private List<string> ObtenerCruces(List<Partido> partidos)
    {
        return partidos
            .Select(p => $"{p.EquipoLocal.Nombre}-{p.EquipoVisitante.Nombre}")
            .ToList();
    }
}
