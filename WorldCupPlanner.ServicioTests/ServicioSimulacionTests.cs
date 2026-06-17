using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.AccesoADatos.Interfaces;
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

    [TestMethod]
    public void SimularResultados_ConPartidoSinResultado_DebeGenerarTarjetasSimuladas()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();
        var partido = CrearPartidosSinResultado(1)[0];
        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaSimulation = 8
        };

        // Act
        servicioSimulacion.SimularResultados(new List<Partido> { partido }, configuracionTorneo);

        // Assert
        Assert.IsTrue(partido.Incidencias.Any());
        Assert.IsTrue(partido.Incidencias.All(i => i is TarjetaAmarilla || i is TarjetaRoja));
        Assert.IsTrue(partido.Incidencias.All(i => i.PartidoId == partido.Id));
        Assert.IsTrue(partido.Incidencias.All(i => i.MinutoDelPartido is >= 1 and <= 90));
        Assert.IsTrue(partido.Incidencias.All(i =>
            ReferenceEquals(i.Equipo, partido.EquipoLocal) ||
            ReferenceEquals(i.Equipo, partido.EquipoVisitante)));
    }

    [TestMethod]
    public void SimularResultados_ConPartidoYaJugado_NoDebeAgregarTarjetasSimuladas()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();
        var partido = CrearPartidosSinResultado(1)[0];
        partido.GolesLocal = 1;
        partido.GolesVisitante = 0;

        // Act
        servicioSimulacion.SimularResultados(
            new List<Partido> { partido },
            new ConfiguracionTorneo { SemillaSimulation = 8 });

        // Assert
        Assert.IsFalse(partido.Incidencias.Any());
    }

    [TestMethod]
    public void SimularResultados_ConAuditoria_RegistraSemillaYCantidadDePartidos()
    {
        // Arrange
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var servicioSimulacion = new ServicioSimulacion(auditoria);
        var partidos = CrearPartidosSinResultado(2);
        var usuario = new Usuario(new DatosUsuario
        {
            Nombre = "Admin",
            Apellido = "Sistema",
            Email = "admin@test.com",
            FechaNacimiento = "01-01-1990"
        });
        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaSimulation = 17
        };

        // Act
        servicioSimulacion.SimularResultados(partidos, configuracionTorneo, usuario);

        // Assert
        var log = repoAuditoria.Buscar(_ => true).Single();
        Assert.AreEqual("Simulacion", log.Accion);
        StringAssert.Contains(log.Usuario, "admin@test.com");
        StringAssert.Contains(log.Detalle, "17");
        StringAssert.Contains(log.Detalle, "2/2");
    }

    [TestMethod]
    public void ObtenerMotoresDisponibles_DebeIncluirHistoricoRankingYFormaYAleatorioPuro()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();

        // Act
        var motores = servicioSimulacion.ObtenerMotoresDisponibles();

        // Assert
        Assert.IsTrue(motores.Any(m => m.Tipo == MotorSimulacion.RankingActualYSemilla && m.Nombre == "Ranking actual + semilla"));
        Assert.IsTrue(motores.Any(m => m.Tipo == MotorSimulacion.RankingYForma && m.Nombre == "Ranking + Forma"));
        Assert.IsTrue(motores.Any(m => m.Tipo == MotorSimulacion.AleatorioPuro && m.Nombre == "Aleatorio Puro"));
        Assert.AreEqual(3, motores.Count);
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

    private List<Partido> CrearContextoDeForma(string nombreLocal, string nombreVisitante, bool localEnForma)
    {
        var local = CrearEquipo(nombreLocal, 1200);
        var visitante = CrearEquipo(nombreVisitante, 1200);
        var partidos = new List<Partido>();

        for (var i = 1; i <= 3; i++)
        {
            partidos.Add(new Partido
            {
                Id = 100 + i,
                EquipoLocal = local,
                EquipoVisitante = visitante,
                Estadio = CrearEstadio(),
                Fecha = DateTime.Now.AddDays(-i),
                Fase = FaseTorneo.FaseDeGrupos,
                GolesLocal = localEnForma ? 3 : 0,
                GolesVisitante = localEnForma ? 0 : 3
            });
        }

        return partidos;
    }

    private List<Partido> CrearPartidosMismosEquipos(
        int cantidad,
        string nombreLocal,
        string nombreVisitante,
        int rankingLocal,
        int rankingVisitante)
    {
        var local = CrearEquipo(nombreLocal, rankingLocal);
        var visitante = CrearEquipo(nombreVisitante, rankingVisitante);
        var partidos = new List<Partido>();

        for (var i = 1; i <= cantidad; i++)
        {
            partidos.Add(new Partido
            {
                Id = i,
                EquipoLocal = local,
                EquipoVisitante = visitante,
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
    public void SimularResultados_ConPartidosEliminatorios_TodosDienenGanador()
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
    public void SimularResultados_ConMotorAleatorioPuro_NoDebeUsarRanking()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();
        var partidoLocalFuerte = CrearPartidosConRankingLocalYVisitante(1, 2500, 300)[0];
        var partidoLocalDebil = CrearPartidosConRankingLocalYVisitante(1, 300, 2500)[0];
        var configuracion = new ConfiguracionTorneo
        {
            SemillaSimulation = 15,
            MotorSimulacion = MotorSimulacion.AleatorioPuro
        };

        // Act
        servicioSimulacion.SimularResultados(new List<Partido> { partidoLocalFuerte }, configuracion);
        servicioSimulacion.SimularResultados(new List<Partido> { partidoLocalDebil }, configuracion);

        // Assert
        Assert.AreEqual(partidoLocalFuerte.GolesLocal, partidoLocalDebil.GolesLocal);
        Assert.AreEqual(partidoLocalFuerte.GolesVisitante, partidoLocalDebil.GolesVisitante);
    }

    [TestMethod]
    public void SimularResultados_ConMotorRankingYForma_DebeConsiderarRendimientoDelTorneo()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();
        var partidosLocalEnForma = CrearPartidosMismosEquipos(30, "Local1", "Visitante1", 1200, 1200);
        var partidosLocalSinForma = CrearPartidosMismosEquipos(30, "Local1", "Visitante1", 1200, 1200);
        var contextoLocalEnForma = CrearContextoDeForma("Local1", "Visitante1", localEnForma: true);
        var contextoLocalSinForma = CrearContextoDeForma("Local1", "Visitante1", localEnForma: false);
        var configuracion = new ConfiguracionTorneo
        {
            SemillaSimulation = 3,
            MotorSimulacion = MotorSimulacion.RankingYForma
        };

        // Act
        servicioSimulacion.SimularResultados(partidosLocalEnForma, configuracion, contextoPartidos: contextoLocalEnForma);
        servicioSimulacion.SimularResultados(partidosLocalSinForma, configuracion, contextoPartidos: contextoLocalSinForma);

        // Assert
        var golesLocalEnForma = partidosLocalEnForma.Sum(p => p.GolesLocal!.Value);
        var golesLocalSinForma = partidosLocalSinForma.Sum(p => p.GolesLocal!.Value);
        Assert.IsTrue(golesLocalEnForma > golesLocalSinForma);
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
    public void SimularResultados_ConEquiposDeRankingAltoEliminatorio_TodosTienenGanador()
    {
        // Con partidos eliminatorios de equipos ranking 2500 (bonus 2),
        // todos los posibles empates deben resolverse por penales asignando GanadorPorPenales.
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

            Assert.IsTrue(partidos.All(p => p.Ganador() != null), $"Semilla {semilla} dejó un partido sin ganador");
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
    public void SimularResultados_ConPartidoEliminatorioEmpatado_AsignaGanadorPorPenales()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();
        var config = new ConfiguracionTorneo { SemillaSimulation = 999 };

        // Buscar semilla que produzca empate en fase eliminatoria
        Partido? partidoConEmpate = null;
        for (int semilla = 1; semilla <= 200; semilla++)
        {
            var partidos = CrearPartidosEliminatoriosSinResultado(5);
            servicioSimulacion.SimularResultados(partidos, new ConfiguracionTorneo { SemillaSimulation = semilla });
            partidoConEmpate = partidos.FirstOrDefault(p => p.FueDefinidoPorPenales());
            if (partidoConEmpate != null) break;
        }

        // Assert — al menos una semilla produjo un partido decidido por penales
        Assert.IsNotNull(partidoConEmpate, "Ninguna semilla produjo un partido decidido por penales");
        Assert.IsNotNull(partidoConEmpate!.GanadorPorPenales);
        Assert.AreEqual(partidoConEmpate.GolesLocal, partidoConEmpate.GolesVisitante);
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
    public void DesempatarPartido_AsignaGanadorPorPenalesYMantieneGoles()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();
        var partido = CrearPartidosEliminatoriosSinResultado(1)[0];
        partido.GolesLocal = 2;
        partido.GolesVisitante = 2;
        var random = new Random(1);

        // Act
        servicioSimulacion.DesempatarPartido(partido, random);

        // Assert
        Assert.IsTrue(
            ReferenceEquals(partido.GanadorPorPenales, partido.EquipoLocal) ||
            ReferenceEquals(partido.GanadorPorPenales, partido.EquipoVisitante));
        Assert.AreEqual(2, partido.GolesLocal);
        Assert.AreEqual(2, partido.GolesVisitante);
    }

    [TestMethod]
    public void DesempatarPartido_ConSemillaFija_EsDeterministico()
    {
        // Arrange
        var servicioSimulacion = new ServicioSimulacion();
        var partido1 = CrearPartidosEliminatoriosSinResultado(1)[0];
        var partido2 = CrearPartidosEliminatoriosSinResultado(1)[0];
        partido1.GolesLocal = partido2.GolesLocal = 1;
        partido1.GolesVisitante = partido2.GolesVisitante = 1;

        // Act — misma semilla, mismo equipo debe ganar
        servicioSimulacion.DesempatarPartido(partido1, new Random(42));
        servicioSimulacion.DesempatarPartido(partido2, new Random(42));

        // Assert
        Assert.IsNotNull(partido1.GanadorPorPenales);
        Assert.IsNotNull(partido2.GanadorPorPenales);
        Assert.AreEqual(partido1.GanadorPorPenales!.Nombre, partido2.GanadorPorPenales!.Nombre);
    }

}

