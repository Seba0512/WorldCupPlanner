using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

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

    [TestMethod]
    public void GenerarPrimeraRondaEliminatoria_DebeEvitarCrucesDelMismoGrupoCuandoSeaPosible()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var clasificados = CrearClasificadosConGruposDeOrigen();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

        // Act
        var partidos = servicioCruces.GenerarPrimeraRondaEliminatoria(
            clasificados,
            estadios,
            configuracionTorneo
        );

        // Assert
        foreach (var partido in partidos)
        {
            var grupoLocal = clasificados.ObtenerGrupoDeOrigen(partido.EquipoLocal!);
            var grupoVisitante = clasificados.ObtenerGrupoDeOrigen(partido.EquipoVisitante!);

            Assert.AreNotSame(grupoLocal, grupoVisitante);
        }
    }

    private List<string> ObtenerCruces(List<Partido> partidos)
    {
        return partidos
            .Select(p => $"{p.EquipoLocal.Nombre}-{p.EquipoVisitante.Nombre}")
            .ToList();
    }

    private ClasificadosFaseGrupos CrearClasificadosConGruposDeOrigen()
    {
        var clasificados = new ClasificadosFaseGrupos();
        var grupos = CrearGruposDeOrigen();

        clasificados.Primeros = CrearEquiposConOrigen("Primero", 12, grupos, clasificados);
        clasificados.Segundos = CrearEquiposConOrigen("Segundo", 12, grupos, clasificados);
        clasificados.MejoresTerceros = CrearEquiposConOrigen("Tercero", 8, grupos, clasificados);

        return clasificados;
    }

    private List<Grupo> CrearGruposDeOrigen()
    {
        var grupos = new List<Grupo>();

        for (char etiqueta = 'A'; etiqueta <= 'L'; etiqueta++)
        {
            grupos.Add(new Grupo(etiqueta.ToString()));
        }

        return grupos;
    }

    private List<Equipo> CrearEquiposConOrigen(
        string prefijo,
        int cantidad,
        List<Grupo> grupos,
        ClasificadosFaseGrupos clasificados)
    {
        var equipos = CrearEquipos(prefijo, cantidad);

        for (int i = 0; i < equipos.Count; i++)
        {
            clasificados.GruposDeOrigen[equipos[i]] = grupos[i];
        }

        return equipos;
    }
    
    private List<Partido> CrearPartidosPrimeraRondaJugados(int cantidad)
    {
        var partidos = new List<Partido>();
        var estadio = new Estadio
        {
            Nombre = "Estadio",
            Ciudad = "Ciudad",
            Capacidad = 30000
        };

        for (int i = 1; i <= cantidad; i++)
        {
            partidos.Add(new Partido
            {
                Id = i,
                EquipoLocal = new Equipo
                {
                    Nombre = $"Local{i}",
                    RankingFIFA = 1000 + i,
                    Confederacion = Confederacion.UEFA
                },
                EquipoVisitante = new Equipo
                {
                    Nombre = $"Visitante{i}",
                    RankingFIFA = 900 + i,
                    Confederacion = Confederacion.CONMEBOL
                },
                Estadio = estadio,
                Fecha = DateTime.Now,
                Fase = FaseTorneo.PrimeraRondaEliminatoria,
                GolesLocal = 2,
                GolesVisitante = 0
            });
        }

        return partidos;
    }
    
    [TestMethod]
    public void GenerarOctavos_Con16PartidosDePrimeraRondaJugados_DebeGenerar8Partidos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados(16);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo
        {
            FechaInicioTorneo = new DateTime(2026, 6, 1),
            SeparacionEntreFechas = 3
        };

        // Act
        var octavos = servicioCruces.GenerarOctavos(
            partidosPrimeraRonda,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(8, octavos.Count);
        Assert.IsTrue(octavos.All(p => p.Fase == FaseTorneo.Octavos));
    }
    
    private List<Partido> CrearPartidosPrimeraRondaJugados()
    {
        var partidos = new List<Partido>();

        for (int i = 1; i <= 16; i++)
        {
            var equipoLocal = new Equipo
            {
                Nombre = $"Ganador{i}",
                RankingFIFA = 1000 + i,
                Confederacion = Confederacion.UEFA
            };

            var equipoVisitante = new Equipo
            {
                Nombre = $"Perdedor{i}",
                RankingFIFA = 800 + i,
                Confederacion = Confederacion.CONMEBOL
            };

            partidos.Add(new Partido
            {
                Id = i,
                EquipoLocal = equipoLocal,
                EquipoVisitante = equipoVisitante,
                GolesLocal = 2,
                GolesVisitante = 0,
                Estadio = new Estadio
                {
                    Nombre = $"Estadio{i}",
                    Ciudad = $"Ciudad{i}",
                    Capacidad = 30000
                },
                Fecha = DateTime.Now,
                Fase = FaseTorneo.PrimeraRondaEliminatoria
            });
        }

        return partidos;
    }
    
    [TestMethod]
    public void GenerarOctavos_DebeUsarGanadoresDePrimeraRonda()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var octavos = servicioCruces.GenerarOctavos(
            partidosPrimeraRonda,
            estadios,
            configuracionTorneo
        );

        // Assert
        var ganadoresPrimeraRonda = partidosPrimeraRonda
            .Select(p => p.Ganador())
            .ToList();

        var equiposEnOctavos = octavos
            .SelectMany(p => new[] { p.EquipoLocal, p.EquipoVisitante })
            .ToList();

        CollectionAssert.AreEquivalent(ganadoresPrimeraRonda, equiposEnOctavos);
    }

    [TestMethod]
    public void GenerarOctavos_DebeCruzarGanadoresAContraGanadoresB()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var octavos = servicioCruces.GenerarOctavos(
            partidosPrimeraRonda,
            estadios,
            configuracionTorneo
        );

        // Assert
        for (int i = 0; i < 8; i++)
        {
            Assert.AreEqual(partidosPrimeraRonda[i].Ganador(), octavos[i].EquipoLocal);
            Assert.AreEqual(partidosPrimeraRonda[i + 8].Ganador(), octavos[i].EquipoVisitante);
        }
    }
    
    [TestMethod]
    public void GenerarOctavos_DebeCrearPartidosConFaseOctavos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var octavos = servicioCruces.GenerarOctavos(
            partidosPrimeraRonda,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.IsTrue(octavos.All(p => p.Fase == FaseTorneo.Octavos));
    }
    
    [TestMethod]
    public void GenerarOctavos_DebeAsignarIdsIncrementalesLuegoDePrimeraRonda()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var octavos = servicioCruces.GenerarOctavos(
            partidosPrimeraRonda,
            estadios,
            configuracionTorneo
        );

        // Assert
        var idsEsperados = Enumerable.Range(17, 8).ToList();
        var idsObtenidos = octavos.Select(p => p.Id).ToList();

        CollectionAssert.AreEqual(idsEsperados, idsObtenidos);
    }

    [TestMethod]
    public void GenerarOctavos_DebeAsignarEstadiosPorRotacion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados(16);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var octavos = servicioCruces.GenerarOctavos(
            partidosPrimeraRonda,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(estadios[0], octavos[0].Estadio);
        Assert.AreEqual(estadios[1], octavos[1].Estadio);
        Assert.AreEqual(estadios[2], octavos[2].Estadio);
        Assert.AreEqual(estadios[3], octavos[3].Estadio);
        Assert.AreEqual(estadios[0], octavos[4].Estadio);
    }
    
    [TestMethod]
    public void GenerarOctavos_ConPartidoPrimeraRondaNoJugado_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados(16);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        partidosPrimeraRonda[0].GolesLocal = null;
        partidosPrimeraRonda[0].GolesVisitante = null;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarOctavos(
                partidosPrimeraRonda,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarOctavos_ConCantidadDistintaA16Partidos_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados(15);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarOctavos(
                partidosPrimeraRonda,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarOctavos_ConPartidoQueNoEsDePrimeraRonda_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados(16);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        partidosPrimeraRonda[0].Fase = FaseTorneo.FaseDeGrupos;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarOctavos(
                partidosPrimeraRonda,
                estadios,
                configuracionTorneo
            )
        );
    }
}
