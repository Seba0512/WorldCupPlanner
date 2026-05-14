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

    private List<Partido> CrearPartidosEliminatoriosSinResultado(int cantidad)
    {
        var partidos = new List<Partido>();

        for (int i = 1; i <= cantidad; i++)
        {
            partidos.Add(new Partido
            {
                Id = i,
                EquipoLocal = CrearEquipo($"LocalEliminatorio{i}"),
                EquipoVisitante = CrearEquipo($"VisitanteEliminatorio{i}"),
                Estadio = CrearEstadio(),
                Fecha = DateTime.Now,
                Fase = FaseTorneo.PrimeraRondaEliminatoria
            });
        }

        return partidos;
    }

    private List<Partido> CrearPartidosConRankingLocalYVisitante(
        int cantidad,
        int rankingLocal,
        int rankingVisitante)
    {
        var partidos = new List<Partido>();

        for (int i = 1; i <= cantidad; i++)
        {
            partidos.Add(new Partido
            {
                Id = i,
                EquipoLocal = CrearEquipo($"Local{i}", rankingLocal),
                EquipoVisitante = CrearEquipo($"Visitante{i}", rankingVisitante),
                Estadio = CrearEstadio(),
                Fecha = DateTime.Now,
                Fase = FaseTorneo.FaseDeGrupos
            });
        }

        return partidos;
    }

    private Equipo CrearEquipo(string nombre)
    {
        return CrearEquipo(nombre, 1000);
    }

    private Equipo CrearEquipo(string nombre, int rankingFifa)
    {
        return new Equipo
        {
            Nombre = nombre,
            RankingFIFA = rankingFifa,
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
    
    [TestMethod]
    public void SimularResultados_ConDistintasSemillas_DebePermitirResultadosDistintos()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();

        var partidosReferencia = CrearPartidosSinResultado(10);

        var configuracionReferencia = new ConfiguracionTorneo
        {
            SemillaSimulation = 1
        };

        servicioSimulacion.SimularResultados(
            partidosReferencia,
            configuracionReferencia
        );

        var resultadosReferencia = ObtenerResultados(partidosReferencia);

        var existeResultadoDistinto = false;

        // Act
        for (int semilla = 2; semilla <= 20; semilla++)
        {
            var partidos = CrearPartidosSinResultado(10);

            var configuracionTorneo = new ConfiguracionTorneo
            {
                SemillaSimulation = semilla
            };

            servicioSimulacion.SimularResultados(
                partidos,
                configuracionTorneo
            );

            var resultados = ObtenerResultados(partidos);

            if (!resultados.SequenceEqual(resultadosReferencia))
            {
                existeResultadoDistinto = true;
                break;
            }
        }

        // Assert
        Assert.IsTrue(existeResultadoDistinto);
    }

    [TestMethod]
    public void SimularResultados_ConPartidoYaJugado_NoDebeSobrescribirResultado()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();
        var partidos = CrearPartidosSinResultado(3);

        partidos[0].GolesLocal = 4;
        partidos[0].GolesVisitante = 2;

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaSimulation = 1
        };

        // Act
        servicioSimulacion.SimularResultados(partidos, configuracionTorneo);

        // Assert
        Assert.AreEqual(4, partidos[0].GolesLocal);
        Assert.AreEqual(2, partidos[0].GolesVisitante);

        Assert.IsTrue(partidos[1].GolesLocal.HasValue);
        Assert.IsTrue(partidos[1].GolesVisitante.HasValue);
        Assert.IsTrue(partidos[2].GolesLocal.HasValue);
        Assert.IsTrue(partidos[2].GolesVisitante.HasValue);
    }

    [TestMethod]
    public void SimularResultados_ConPartidosEliminatorios_NoDebeGenerarEmpates()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();
        var partidos = CrearPartidosEliminatoriosSinResultado(100);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaSimulation = 1
        };

        // Act
        servicioSimulacion.SimularResultados(partidos, configuracionTorneo);

        // Assert
        Assert.IsTrue(partidos.All(p => p.EstaJugado()));
        Assert.IsTrue(partidos.All(p => !p.EsEmpate()));
        Assert.IsTrue(partidos.All(p => p.Ganador() != null));
    }

    [TestMethod]
    public void SimularResultados_DebeBasarseEnRankingFifaDeLosEquipos()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();
        var partidosConLocalFuerte = CrearPartidosConRankingLocalYVisitante(30, 2500, 300);
        var partidosConLocalDebil = CrearPartidosConRankingLocalYVisitante(30, 300, 2500);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaSimulation = 1
        };

        // Act
        servicioSimulacion.SimularResultados(partidosConLocalFuerte, configuracionTorneo);
        servicioSimulacion.SimularResultados(partidosConLocalDebil, configuracionTorneo);

        // Assert
        var golesLocalesFuertes = partidosConLocalFuerte.Sum(p => p.GolesLocal!.Value);
        var golesLocalesDebiles = partidosConLocalDebil.Sum(p => p.GolesLocal!.Value);

        Assert.IsTrue(golesLocalesFuertes > golesLocalesDebiles);
    }

    [TestMethod]
    public void SimularResultados_ConEquipoLocalNulo_DebeAsignarGolesComoSiTuvieraBonusCero()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();
        var partido = new Partido
        {
            Id = 1,
            EquipoLocal = null,
            EquipoVisitante = CrearEquipo("Visitante", 300),
            Estadio = CrearEstadio(),
            Fecha = DateTime.Now,
            Fase = FaseTorneo.FaseDeGrupos
        };
        var configuracionTorneo = new ConfiguracionTorneo { SemillaSimulation = 1 };

        // Act
        servicioSimulacion.SimularResultados(new List<Partido> { partido }, configuracionTorneo);

        // Assert — se deben asignar goles sin excepción; local sin bonus no supera 3
        Assert.IsTrue(partido.GolesLocal.HasValue);
        Assert.IsTrue(partido.GolesLocal <= 3);
    }

    [TestMethod]
    public void SimularResultados_ConEquiposDeRankingAltoEliminatorio_TodosLosCasosDeDesempateQuedan()
    {
        // Con 1000 partidos eliminatorios de equipos a ranking 2500 (bonus 2),
        // la simulación debe cubrir el desempate decrementando visitante cuando local ya es 5.
        var servicioSimulacion = new ServicioSimulacion();
        for (int semilla = 1; semilla <= 100; semilla++)
        {
            var partidos = Enumerable.Range(1, 10).Select(i => new Partido
            {
                Id = i,
                EquipoLocal = CrearEquipo("Local" + i, 2500),
                EquipoVisitante = CrearEquipo("Visitante" + i, 2500),
                Estadio = CrearEstadio(),
                Fecha = DateTime.Now,
                Fase = FaseTorneo.Octavos
            }).ToList();

            servicioSimulacion.SimularResultados(partidos, new ConfiguracionTorneo { SemillaSimulation = semilla });

            Assert.IsTrue(partidos.All(p => !p.EsEmpate()), $"Semilla {semilla} dejó un empate");
            Assert.IsTrue(partidos.All(p => p.GolesLocal <= 5 && p.GolesVisitante <= 5));
        }
    }

    [TestMethod]
    public void SimularResultados_ConRankingMedio_AplicaBonusDe1()
    {
        // Equipos con ranking entre 1000 y 1799 reciben bonus 1
        var servicioSimulacion = new ServicioSimulacion();
        var partidosConRankingMedio = CrearPartidosConRankingLocalYVisitante(50, 1500, 300);
        var partidosConRankingBajo = CrearPartidosConRankingLocalYVisitante(50, 300, 300);
        var config = new ConfiguracionTorneo { SemillaSimulation = 1 };

        servicioSimulacion.SimularResultados(partidosConRankingMedio, config);
        servicioSimulacion.SimularResultados(partidosConRankingBajo, config);

        var golesLocalMedio = partidosConRankingMedio.Sum(p => p.GolesLocal!.Value);
        var golesLocalBajo = partidosConRankingBajo.Sum(p => p.GolesLocal!.Value);

        Assert.IsTrue(golesLocalMedio > golesLocalBajo, "El ranking medio debe generar más goles que el ranking bajo");
    }

    [TestMethod]
    public void SimularResultados_MismoPartidoSoloODentroDeLista_DebeGenerarMismoResultado()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();
        var partidos = CrearPartidosSinResultado(3);
        var partidoSimuladoSolo = CrearPartidosSinResultado(3)[2];
        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaSimulation = 4
        };

        // Act
        servicioSimulacion.SimularResultados(new List<Partido> { partidoSimuladoSolo }, configuracionTorneo);
        servicioSimulacion.SimularResultados(partidos, configuracionTorneo);

        // Assert
        Assert.AreEqual(partidoSimuladoSolo.GolesLocal, partidos[2].GolesLocal);
        Assert.AreEqual(partidoSimuladoSolo.GolesVisitante, partidos[2].GolesVisitante);
    }
    [TestMethod]
    public void DesempatarPartido_ConLocalMenorAlMaximo_IncrementaGolesLocal()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();
        var partido = CrearPartidosEliminatoriosSinResultado(1)[0];
        partido.GolesLocal = 3;
        partido.GolesVisitante = 3;

        var metodo = typeof(ServicioSimulacion).GetMethod(
            "DesempatarPartido",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        // Act
        metodo!.Invoke(servicioSimulacion, new object[] { partido });

        // Assert
        Assert.AreEqual(4, partido.GolesLocal);
        Assert.AreEqual(3, partido.GolesVisitante);
    }

    [TestMethod]
    public void DesempatarPartido_ConLocalEnMaximo_DecrementaGolesVisitante()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();
        var partido = CrearPartidosEliminatoriosSinResultado(1)[0];
        partido.GolesLocal = 5;
        partido.GolesVisitante = 5;

        var metodo = typeof(ServicioSimulacion).GetMethod(
            "DesempatarPartido",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        // Act
        metodo!.Invoke(servicioSimulacion, new object[] { partido });

        // Assert
        Assert.AreEqual(5, partido.GolesLocal);
        Assert.AreEqual(4, partido.GolesVisitante);
    }

}
