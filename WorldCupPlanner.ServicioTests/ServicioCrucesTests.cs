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
            new ContextoSorteo { Clasificados = clasificados, Estadios = estadios, ConfiguracionTorneo = configuracionTorneo }
        );

        // Assert
        Assert.AreEqual(16, partidos.Count);
        Assert.IsTrue(partidos.All(p => p.Fase == FaseTorneo.PrimeraRondaEliminatoria));
    }

    [TestMethod]
    public void GenerarPrimeraRondaEliminatoria_DebeAsignarIdsIncrementalesLuegoDeFaseDeGrupos()
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
            new ContextoSorteo { Clasificados = clasificados, Estadios = estadios, ConfiguracionTorneo = configuracionTorneo }
        );

        // Assert
        var idsEsperados = Enumerable.Range(73, 16).ToList();
        var idsObtenidos = partidos.Select(p => p.Id).ToList();

        CollectionAssert.AreEqual(idsEsperados, idsObtenidos);
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
            new ContextoSorteo { Clasificados = clasificados, Estadios = estadios, ConfiguracionTorneo = configuracionTorneo }
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
            new ContextoSorteo { Clasificados = clasificados, Estadios = estadios, ConfiguracionTorneo = configuracionTorneo }
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
            new ContextoSorteo { Clasificados = clasificados, Estadios = estadios, ConfiguracionTorneo = configuracionTorneo }
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
            new ContextoSorteo { Clasificados = clasificados, Estadios = estadios, ConfiguracionTorneo = configuracionTorneo1 }
        );

        var partidos2 = servicioCruces.GenerarPrimeraRondaEliminatoria(
            new ContextoSorteo { Clasificados = clasificados, Estadios = estadios, ConfiguracionTorneo = configuracionTorneo2 }
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
            new ContextoSorteo { Clasificados = clasificados, Estadios = estadios, ConfiguracionTorneo = configuracionReferencia }
        );

        var crucesReferencia = ObtenerCruces(partidosReferencia);
        var existeCruceDistinto = false;

        for (int semilla = 2; semilla <= 20; semilla++)
        {
            var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = semilla };

            var partidos = servicioCruces.GenerarPrimeraRondaEliminatoria(
                new ContextoSorteo { Clasificados = clasificados, Estadios = estadios, ConfiguracionTorneo = configuracionTorneo }
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
            new ContextoSorteo { Clasificados = clasificados, Estadios = estadios, ConfiguracionTorneo = configuracionTorneo }
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
                Id = 72 + i,
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
                Id = 72 + i,
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

    private List<Partido> CrearPartidosOctavosJugados(int cantidad, DateTime? fechaFija = null)
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
                Id = 88 + i,
                EquipoLocal = new Equipo
                {
                    Nombre = $"LocalOctavos{i}",
                    RankingFIFA = 1000 + i,
                    Confederacion = Confederacion.UEFA
                },
                EquipoVisitante = new Equipo
                {
                    Nombre = $"VisitanteOctavos{i}",
                    RankingFIFA = 900 + i,
                    Confederacion = Confederacion.CONMEBOL
                },
                Estadio = estadio,
                Fecha = fechaFija ?? DateTime.Now,
                Fase = FaseTorneo.Octavos,
                GolesLocal = 2,
                GolesVisitante = 0
            });
        }

        return partidos;
    }

    private List<Partido> CrearPartidosCuartosJugados(int cantidad, DateTime? fechaFija = null)
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
                Id = 96 + i,
                EquipoLocal = new Equipo
                {
                    Nombre = $"LocalCuartos{i}",
                    RankingFIFA = 1000 + i,
                    Confederacion = Confederacion.UEFA
                },
                EquipoVisitante = new Equipo
                {
                    Nombre = $"VisitanteCuartos{i}",
                    RankingFIFA = 900 + i,
                    Confederacion = Confederacion.CONMEBOL
                },
                Estadio = estadio,
                Fecha = fechaFija ?? DateTime.Now,
                Fase = FaseTorneo.Cuartos,
                GolesLocal = 2,
                GolesVisitante = 0
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
        var idsEsperados = Enumerable.Range(89, 8).ToList();
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

    [TestMethod]
    public void GenerarOctavos_ConPartidoPrimeraRondaEmpatado_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var partidosPrimeraRonda = CrearPartidosPrimeraRondaJugados(16);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        partidosPrimeraRonda[0].GolesLocal = 1;
        partidosPrimeraRonda[0].GolesVisitante = 1;

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
    public void GenerarCuartos_Con8OctavosJugados_DebeGenerar4Partidos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(8);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var cuartos = servicioCruces.GenerarCuartos(
            octavos,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(4, cuartos.Count);
        Assert.IsTrue(cuartos.All(p => p.Fase == FaseTorneo.Cuartos));
    }

    [TestMethod]
    public void GenerarCuartos_ConCantidadDistintaA8Octavos_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(7);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarCuartos(
                octavos,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarCuartos_ConPartidoQueNoEsOctavos_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(8);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        octavos[0].Fase = FaseTorneo.PrimeraRondaEliminatoria;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarCuartos(
                octavos,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarCuartos_ConOctavoNoJugado_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(8);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        octavos[0].GolesLocal = null;
        octavos[0].GolesVisitante = null;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarCuartos(
                octavos,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarCuartos_ConOctavoEmpatado_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(8);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        octavos[0].GolesLocal = 1;
        octavos[0].GolesVisitante = 1;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarCuartos(
                octavos,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarCuartos_DebeUsarGanadoresDeOctavos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(8);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var cuartos = servicioCruces.GenerarCuartos(
            octavos,
            estadios,
            configuracionTorneo
        );

        // Assert
        var ganadoresOctavos = octavos
            .Select(p => p.Ganador())
            .ToList();

        var equiposEnCuartos = cuartos
            .SelectMany(p => new[] { p.EquipoLocal, p.EquipoVisitante })
            .ToList();

        CollectionAssert.AreEquivalent(ganadoresOctavos, equiposEnCuartos);
    }

    [TestMethod]
    public void GenerarCuartos_DebeCruzarGanadoresDeOctavosDeADos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(8);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var cuartos = servicioCruces.GenerarCuartos(
            octavos,
            estadios,
            configuracionTorneo
        );

        // Assert
        for (int i = 0; i < 4; i++)
        {
            Assert.AreEqual(octavos[i * 2].Ganador(), cuartos[i].EquipoLocal);
            Assert.AreEqual(octavos[i * 2 + 1].Ganador(), cuartos[i].EquipoVisitante);
        }
    }

    [TestMethod]
    public void GenerarCuartos_DebeAsignarIdsIncrementalesLuegoDeOctavos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(8);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var cuartos = servicioCruces.GenerarCuartos(
            octavos,
            estadios,
            configuracionTorneo
        );

        // Assert
        var idsEsperados = Enumerable.Range(97, 4).ToList();
        var idsObtenidos = cuartos.Select(p => p.Id).ToList();

        CollectionAssert.AreEqual(idsEsperados, idsObtenidos);
    }

    [TestMethod]
    public void GenerarCuartos_DebeAsignarEstadiosPorRotacion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var octavos = CrearPartidosOctavosJugados(8);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var cuartos = servicioCruces.GenerarCuartos(
            octavos,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(estadios[0], cuartos[0].Estadio);
        Assert.AreEqual(estadios[1], cuartos[1].Estadio);
        Assert.AreEqual(estadios[2], cuartos[2].Estadio);
        Assert.AreEqual(estadios[3], cuartos[3].Estadio);
    }

    [TestMethod]
    public void GenerarCuartos_DebeAsignarFechaSegunConfiguracion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        // octavos con fecha fija para que cuartos caigan en FechaInicioTorneo + 2*SeparacionEntreFechas
        var octavos = CrearPartidosOctavosJugados(8, new DateTime(2026, 6, 4));
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            FechaInicioTorneo = new DateTime(2026, 6, 1),
            SeparacionEntreFechas = 3,
            HoraInicioPartidos = TimeSpan.Zero,
            SeparacionHorasEntrePartidos = 0,
            MaxPartidosPorDia = 4
        };

        // Act
        var cuartos = servicioCruces.GenerarCuartos(
            octavos,
            estadios,
            configuracionTorneo
        );

        // Assert
        var fechaEsperada = new DateTime(2026, 6, 7);

        Assert.IsTrue(cuartos.All(p => p.Fecha == fechaEsperada));
    }

    [TestMethod]
    public void GenerarSemifinales_Con4CuartosJugados_DebeGenerar2Partidos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(4);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var semifinales = servicioCruces.GenerarSemifinales(
            cuartos,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(2, semifinales.Count);
        Assert.IsTrue(semifinales.All(p => p.Fase == FaseTorneo.Semifinal));
    }

    [TestMethod]
    public void GenerarSemifinales_ConCantidadDistintaA4Cuartos_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(3);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarSemifinales(
                cuartos,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarSemifinales_ConPartidoQueNoEsCuartos_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(4);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        cuartos[0].Fase = FaseTorneo.Octavos;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarSemifinales(
                cuartos,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarSemifinales_ConCuartoNoJugado_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(4);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        cuartos[0].GolesLocal = null;
        cuartos[0].GolesVisitante = null;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarSemifinales(
                cuartos,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarSemifinales_ConCuartoEmpatado_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(4);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        cuartos[0].GolesLocal = 1;
        cuartos[0].GolesVisitante = 1;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarSemifinales(
                cuartos,
                estadios,
                configuracionTorneo
            )
        );
    }

    [TestMethod]
    public void GenerarSemifinales_DebeUsarGanadoresDeCuartos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(4);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var semifinales = servicioCruces.GenerarSemifinales(
            cuartos,
            estadios,
            configuracionTorneo
        );

        // Assert
        var ganadoresCuartos = cuartos
            .Select(p => p.Ganador())
            .ToList();

        var equiposEnSemifinales = semifinales
            .SelectMany(p => new[] { p.EquipoLocal, p.EquipoVisitante })
            .ToList();

        CollectionAssert.AreEquivalent(ganadoresCuartos, equiposEnSemifinales);
    }

    [TestMethod]
    public void GenerarSemifinales_DebeCruzarGanadoresDeCuartosDeADos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(4);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var semifinales = servicioCruces.GenerarSemifinales(
            cuartos,
            estadios,
            configuracionTorneo
        );

        // Assert
        for (int i = 0; i < 2; i++)
        {
            Assert.AreEqual(cuartos[i * 2].Ganador(), semifinales[i].EquipoLocal);
            Assert.AreEqual(cuartos[i * 2 + 1].Ganador(), semifinales[i].EquipoVisitante);
        }
    }
    
    [TestMethod]
    public void GenerarSemifinales_DebeAsignarIdsIncrementalesLuegoDeCuartos()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(4);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var semifinales = servicioCruces.GenerarSemifinales(
            cuartos,
            estadios,
            configuracionTorneo
        );

        // Assert
        var idsEsperados = Enumerable.Range(101, 2).ToList();
        var idsObtenidos = semifinales.Select(p => p.Id).ToList();

        CollectionAssert.AreEqual(idsEsperados, idsObtenidos);
    }
    
    [TestMethod]
    public void GenerarSemifinales_DebeAsignarEstadiosPorRotacion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var cuartos = CrearPartidosCuartosJugados(4);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var semifinales = servicioCruces.GenerarSemifinales(
            cuartos,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(estadios[0], semifinales[0].Estadio);
        Assert.AreEqual(estadios[1], semifinales[1].Estadio);
    }
    
    [TestMethod]
    public void GenerarSemifinales_DebeAsignarFechaSegunConfiguracion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        // cuartos con fecha fija para que semifinales caigan en FechaInicioTorneo + 3*SeparacionEntreFechas
        var cuartos = CrearPartidosCuartosJugados(4, new DateTime(2026, 6, 7));
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            FechaInicioTorneo = new DateTime(2026, 6, 1),
            SeparacionEntreFechas = 3,
            HoraInicioPartidos = TimeSpan.Zero,
            SeparacionHorasEntrePartidos = 0
        };

        // Act
        var semifinales = servicioCruces.GenerarSemifinales(
            cuartos,
            estadios,
            configuracionTorneo
        );

        // Assert
        var fechaEsperada = new DateTime(2026, 6, 10);

        Assert.IsTrue(semifinales.All(p => p.Fecha == fechaEsperada));
    }
    
    private List<Partido> CrearPartidosSemifinalesJugados(int cantidad, DateTime? fechaFija = null)
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
                Id = 100 + i,
                EquipoLocal = new Equipo
                {
                    Nombre = $"LocalSemifinal{i}",
                    RankingFIFA = 1000 + i,
                    Confederacion = Confederacion.UEFA
                },
                EquipoVisitante = new Equipo
                {
                    Nombre = $"VisitanteSemifinal{i}",
                    RankingFIFA = 900 + i,
                    Confederacion = Confederacion.CONMEBOL
                },
                Estadio = estadio,
                Fecha = fechaFija ?? DateTime.Now,
                Fase = FaseTorneo.Semifinal,
                GolesLocal = 2,
                GolesVisitante = 0
            });
        }

        return partidos;
    }
    
    [TestMethod]
    public void GenerarTercerPuesto_Con2SemifinalesJugadas_DebeGenerar1Partido()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var tercerPuesto = servicioCruces.GenerarTercerPuesto(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(FaseTorneo.TercerPuesto, tercerPuesto.Fase);
        Assert.IsNotNull(tercerPuesto.EquipoLocal);
        Assert.IsNotNull(tercerPuesto.EquipoVisitante);
    }
    
    [TestMethod]
    public void GenerarTercerPuesto_ConCantidadDistintaA2Semifinales_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(1);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarTercerPuesto(
                semifinales,
                estadios,
                configuracionTorneo
            )
        );
    }
    
    [TestMethod]
    public void GenerarTercerPuesto_ConPartidoQueNoEsSemifinal_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        semifinales[0].Fase = FaseTorneo.Cuartos;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarTercerPuesto(
                semifinales,
                estadios,
                configuracionTorneo
            )
        );
    }
    
    [TestMethod]
    public void GenerarTercerPuesto_ConSemifinalNoJugada_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        semifinales[0].GolesLocal = null;
        semifinales[0].GolesVisitante = null;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarTercerPuesto(
                semifinales,
                estadios,
                configuracionTorneo
            )
        );
    }
    
    [TestMethod]
    public void GenerarTercerPuesto_ConSemifinalEmpatada_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        semifinales[0].GolesLocal = 1;
        semifinales[0].GolesVisitante = 1;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarTercerPuesto(
                semifinales,
                estadios,
                configuracionTorneo
            )
        );
    }
    
    [TestMethod]
    public void GenerarTercerPuesto_DebeUsarPerdedoresDeSemifinales()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var tercerPuesto = servicioCruces.GenerarTercerPuesto(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        var perdedorSemifinal1 = semifinales[0].EquipoVisitante;
        var perdedorSemifinal2 = semifinales[1].EquipoVisitante;

        Assert.AreEqual(perdedorSemifinal1, tercerPuesto.EquipoLocal);
        Assert.AreEqual(perdedorSemifinal2, tercerPuesto.EquipoVisitante);
    }

    [TestMethod]
    public void GenerarTercerPuesto_CuandoGanaElVisitante_UsaEquipoLocalComoPerdedor()
    {
        // Cubre la rama de ObtenerPerdedor donde el ganador es el visitante
        // y el perdedor es el equipo local
        var servicioCruces = new ServicioCruces();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();
        var estadio = new Estadio { Nombre = "Estadio", Ciudad = "Ciudad", Capacidad = 30000 };

        var localSemifinal1 = new Equipo { Nombre = "Local1", RankingFIFA = 1000, Confederacion = Confederacion.UEFA };
        var visitanteSemifinal1 = new Equipo { Nombre = "Visitante1", RankingFIFA = 900, Confederacion = Confederacion.CONMEBOL };
        var localSemifinal2 = new Equipo { Nombre = "Local2", RankingFIFA = 1000, Confederacion = Confederacion.UEFA };
        var visitanteSemifinal2 = new Equipo { Nombre = "Visitante2", RankingFIFA = 900, Confederacion = Confederacion.CONMEBOL };

        // El visitante gana → perdedor es el local
        var semifinales = new List<Partido>
        {
            new Partido
            {
                Id = 101, EquipoLocal = localSemifinal1, EquipoVisitante = visitanteSemifinal1,
                GolesLocal = 0, GolesVisitante = 2,
                Estadio = estadio, Fecha = DateTime.Now, Fase = FaseTorneo.Semifinal
            },
            new Partido
            {
                Id = 102, EquipoLocal = localSemifinal2, EquipoVisitante = visitanteSemifinal2,
                GolesLocal = 0, GolesVisitante = 1,
                Estadio = estadio, Fecha = DateTime.Now, Fase = FaseTorneo.Semifinal
            }
        };

        var tercerPuesto = servicioCruces.GenerarTercerPuesto(semifinales, estadios, configuracionTorneo);

        Assert.AreEqual(localSemifinal1, tercerPuesto.EquipoLocal);
        Assert.AreEqual(localSemifinal2, tercerPuesto.EquipoVisitante);
    }
    
    [TestMethod]
    public void GenerarTercerPuesto_DebeAsignarIdLuegoDeSemifinales()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var tercerPuesto = servicioCruces.GenerarTercerPuesto(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(103, tercerPuesto.Id);
    }
    
    [TestMethod]
    public void GenerarTercerPuesto_DebeAsignarFechaSegunConfiguracion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        // semifinales con fecha fija para que tercer puesto caiga en FechaInicioTorneo + 4*SeparacionEntreFechas
        var semifinales = CrearPartidosSemifinalesJugados(2, new DateTime(2026, 6, 10));
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            FechaInicioTorneo = new DateTime(2026, 6, 1),
            SeparacionEntreFechas = 3,
            HoraInicioPartidos = TimeSpan.Zero
        };

        // Act
        var tercerPuesto = servicioCruces.GenerarTercerPuesto(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        var fechaEsperada = new DateTime(2026, 6, 13);

        Assert.AreEqual(fechaEsperada, tercerPuesto.Fecha);
    }
    
    [TestMethod]
    public void GenerarTercerPuesto_DebeAsignarPrimerEstadio()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var tercerPuesto = servicioCruces.GenerarTercerPuesto(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(estadios[0], tercerPuesto.Estadio);
    }
    
    [TestMethod]
    public void GenerarFinal_Con2SemifinalesJugadas_DebeGenerar1Partido()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var final = servicioCruces.GenerarFinal(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(FaseTorneo.Final, final.Fase);
        Assert.IsNotNull(final.EquipoLocal);
        Assert.IsNotNull(final.EquipoVisitante);
    }
    
    [TestMethod]
    public void GenerarFinal_ConCantidadDistintaA2Semifinales_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(1);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarFinal(
                semifinales,
                estadios,
                configuracionTorneo
            )
        );
    }
    
    [TestMethod]
    public void GenerarFinal_ConPartidoQueNoEsSemifinal_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        semifinales[0].Fase = FaseTorneo.Cuartos;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarFinal(
                semifinales,
                estadios,
                configuracionTorneo
            )
        );
    }
    
    [TestMethod]
    public void GenerarFinal_ConSemifinalNoJugada_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        semifinales[0].GolesLocal = null;
        semifinales[0].GolesVisitante = null;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarFinal(
                semifinales,
                estadios,
                configuracionTorneo
            )
        );
    }
    
    [TestMethod]
    public void GenerarFinal_ConSemifinalEmpatada_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        semifinales[0].GolesLocal = 1;
        semifinales[0].GolesVisitante = 1;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioCruces.GenerarFinal(
                semifinales,
                estadios,
                configuracionTorneo
            )
        );
    }
    
    [TestMethod]
    public void GenerarFinal_DebeUsarGanadoresDeSemifinales()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var final = servicioCruces.GenerarFinal(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(semifinales[0].Ganador(), final.EquipoLocal);
        Assert.AreEqual(semifinales[1].Ganador(), final.EquipoVisitante);
    }
    
    [TestMethod]
    public void GenerarFinal_DebeAsignarIdLuegoDeTercerPuesto()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var final = servicioCruces.GenerarFinal(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(104, final.Id);
    }
    
    [TestMethod]
    public void GenerarFinal_DebeAsignarFechaSegunConfiguracion()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        // semifinales con fecha fija para que la final caiga en FechaInicioTorneo + 5*SeparacionEntreFechas
        var semifinales = CrearPartidosSemifinalesJugados(2, new DateTime(2026, 6, 10));
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            FechaInicioTorneo = new DateTime(2026, 6, 1),
            SeparacionEntreFechas = 3,
            HoraInicioPartidos = TimeSpan.Zero
        };

        // Act
        var final = servicioCruces.GenerarFinal(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        var fechaEsperada = new DateTime(2026, 6, 16);

        Assert.AreEqual(fechaEsperada, final.Fecha);
    }
    
    [TestMethod]
    public void GenerarFinal_DebeAsignarPrimerEstadio()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var semifinales = CrearPartidosSemifinalesJugados(2);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var final = servicioCruces.GenerarFinal(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(estadios[0], final.Estadio);
    }
    
    private void MarcarPartidosComoJugados(List<Partido> partidos)
    {
        foreach (var partido in partidos)
        {
            partido.GolesLocal = 2;
            partido.GolesVisitante = 0;
        }
    }

    [TestMethod]
    public void GenerarPrimeraRondaEliminatoria_CuandoTodosDelMismoGrupo_CompletaSinExcepcion()
    {
        // Arrange — todos los equipos del mismo grupo obliga a BuscarVisitanteAlternativo
        // y BuscarRivalAlternativo a devolver null (fallback gracioso sin crash)
        var servicioCruces = new ServicioCruces();
        var grupoA = new Grupo("A");
        var grupoB = new Grupo("B");
        var grupoC = new Grupo("C");

        var clasificados = new ClasificadosFaseGrupos();

        // 8 primeros desde grupoA → se cruzarán contra terceros (todos grupoA)
        for (int i = 1; i <= 8; i++)
        {
            var e = new Equipo { Nombre = $"Primero{i}", RankingFIFA = 1000 + i, Confederacion = Confederacion.UEFA };
            clasificados.Primeros.Add(e);
            clasificados.GruposDeOrigen[e] = grupoA;
        }
        // 4 primeros adicionales desde grupoB
        for (int i = 9; i <= 12; i++)
        {
            var e = new Equipo { Nombre = $"Primero{i}", RankingFIFA = 1000 + i, Confederacion = Confederacion.UEFA };
            clasificados.Primeros.Add(e);
            clasificados.GruposDeOrigen[e] = grupoB;
        }

        // 8 segundos desde grupoA → segundosRestantes, todos del mismo grupo
        // (fuerza BuscarRivalAlternativo a devolver null)
        for (int i = 1; i <= 8; i++)
        {
            var e = new Equipo { Nombre = $"Segundo{i}", RankingFIFA = 900 + i, Confederacion = Confederacion.CONMEBOL };
            clasificados.Segundos.Add(e);
            clasificados.GruposDeOrigen[e] = grupoA;
        }
        // 4 segundos adicionales desde grupoC (segundosDeMenorRendimiento)
        for (int i = 9; i <= 12; i++)
        {
            var e = new Equipo { Nombre = $"Segundo{i}", RankingFIFA = 900 + i, Confederacion = Confederacion.CONMEBOL };
            clasificados.Segundos.Add(e);
            clasificados.GruposDeOrigen[e] = grupoC;
        }

        // 8 terceros desde grupoA → cruces primeros vs terceros todos conflictan
        // (fuerza BuscarVisitanteAlternativo a devolver null)
        for (int i = 1; i <= 8; i++)
        {
            var e = new Equipo { Nombre = $"Tercero{i}", RankingFIFA = 800 + i, Confederacion = Confederacion.AFC };
            clasificados.MejoresTerceros.Add(e);
            clasificados.GruposDeOrigen[e] = grupoA;
        }

        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

        // Act — no debe lanzar excepción aunque no se puedan evitar cruces del mismo grupo
        var partidos = servicioCruces.GenerarPrimeraRondaEliminatoria(
            new ContextoSorteo { Clasificados = clasificados, Estadios = estadios, ConfiguracionTorneo = configuracionTorneo }
        );

        // Assert
        Assert.AreEqual(16, partidos.Count);
        Assert.IsTrue(partidos.All(p => p.Fase == FaseTorneo.PrimeraRondaEliminatoria));
    }

    private class RandomSiempreCero : Random
    {
        public override int Next(int maxValue) => 0;
    }

    [TestMethod]
    public void GenerarPrimeraRondaEliminatoria_CuandoHayConflictosResolubles_ReordenaCorrectamente()
    {
        // Arrange
        // Con RandomSiempreCero, Fisher-Yates rota los elementos una posición a la izquierda.
        // primerosRestantes=[P9(C),P10(D),P11(E),P12(F)] → [P10(D),P11(E),P12(F),P9(C)]
        // segundosDeMenorRendimiento=[SE1(C),SE2(D),SE3(E),SE4(F)] → [SE2(D),SE3(E),SE4(F),SE1(C)]
        // → pares (D,D),(E,E),(F,F),(C,C): todos conflictos resolubles → cubre líneas 176-180
        //
        // segundosRestantes=[SA1..SA4(A),SB1..SB4(B)] → [SA2,SA3,SA4,SB1,SB2,SB3,SB4,SA1]
        // → par (0,1)=(A,A): conflicto resoluble con SB2 en posición 4 → cubre líneas 229-233 y 264-265
        var servicioCruces = new ServicioCruces();
        var clasificados = new ClasificadosFaseGrupos();

        var grupoA = new Grupo("A");
        var grupoB = new Grupo("B");
        var grupoC = new Grupo("C");
        var grupoD = new Grupo("D");
        var grupoE = new Grupo("E");
        var grupoF = new Grupo("F");
        var grupoG = new Grupo("G"); // primeros[0-7] → sin conflicto con terceros(H)
        var grupoH = new Grupo("H"); // terceros[0-7]

        // Primeros[0-7] → todos en grupoG (distinto de grupoH de terceros → sin conflictos en ese cruce)
        for (int i = 0; i < 8; i++)
        {
            var e = new Equipo { Nombre = $"Primero{i + 1}", RankingFIFA = 1000 + i, Confederacion = Confederacion.UEFA };
            clasificados.Primeros.Add(e);
            clasificados.GruposDeOrigen[e] = grupoG;
        }
        // Primeros[8-11] → grupos C,D,E,F
        var gruposExtra = new[] { grupoC, grupoD, grupoE, grupoF };
        for (int i = 0; i < 4; i++)
        {
            var e = new Equipo { Nombre = $"Primero{i + 9}", RankingFIFA = 1100 + i, Confederacion = Confederacion.UEFA };
            clasificados.Primeros.Add(e);
            clasificados.GruposDeOrigen[e] = gruposExtra[i];
        }

        // Segundos[0-7] → 4 de grupoA, 4 de grupoB (para segundosRestantes con conflictos)
        for (int i = 0; i < 4; i++)
        {
            var e = new Equipo { Nombre = $"SegundoA{i + 1}", RankingFIFA = 900 + i, Confederacion = Confederacion.CONMEBOL };
            clasificados.Segundos.Add(e);
            clasificados.GruposDeOrigen[e] = grupoA;
        }
        for (int i = 0; i < 4; i++)
        {
            var e = new Equipo { Nombre = $"SegundoB{i + 1}", RankingFIFA = 850 + i, Confederacion = Confederacion.CONMEBOL };
            clasificados.Segundos.Add(e);
            clasificados.GruposDeOrigen[e] = grupoB;
        }
        // Segundos[8-11] → grupos C,D,E,F (para segundosDeMenorRendimiento con conflictos)
        for (int i = 0; i < 4; i++)
        {
            var e = new Equipo { Nombre = $"SegundoExtra{i + 1}", RankingFIFA = 800 + i, Confederacion = Confederacion.CONMEBOL };
            clasificados.Segundos.Add(e);
            clasificados.GruposDeOrigen[e] = gruposExtra[i];
        }

        // MejoresTerceros[0-7] → todos en grupoH (distinto de grupoG → sin conflictos al cruzar con primeros)
        for (int i = 0; i < 8; i++)
        {
            var e = new Equipo { Nombre = $"Tercero{i + 1}", RankingFIFA = 700 + i, Confederacion = Confederacion.AFC };
            clasificados.MejoresTerceros.Add(e);
            clasificados.GruposDeOrigen[e] = grupoH;
        }

        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

        // Act — RandomSiempreCero produce shuffles predecibles que garantizan conflictos resolubles
        var partidos = servicioCruces.GenerarPrimeraRondaEliminatoria(
            new ContextoSorteo { Clasificados = clasificados, Estadios = estadios, ConfiguracionTorneo = configuracionTorneo },
            new RandomSiempreCero());

        // Assert
        Assert.AreEqual(16, partidos.Count);
        Assert.IsTrue(partidos.All(p => p.Fase == FaseTorneo.PrimeraRondaEliminatoria));
        Assert.IsTrue(partidos.All(p => p.EquipoLocal != p.EquipoVisitante));
    }

    [TestMethod]
    public void GenerarFaseEliminatoria_CompletaDesdePrimeraRonda_DebeGenerarHastaFinal()
    {
        // Arrange
        var servicioCruces = new ServicioCruces();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo
        {
            FechaInicioTorneo = new DateTime(2026, 6, 1),
            SeparacionEntreFechas = 3
        };

        var primeraRonda = CrearPartidosPrimeraRondaJugados(16);

        // Act
        var octavos = servicioCruces.GenerarOctavos(
            primeraRonda,
            estadios,
            configuracionTorneo
        );

        MarcarPartidosComoJugados(octavos);

        var cuartos = servicioCruces.GenerarCuartos(
            octavos,
            estadios,
            configuracionTorneo
        );

        MarcarPartidosComoJugados(cuartos);

        var semifinales = servicioCruces.GenerarSemifinales(
            cuartos,
            estadios,
            configuracionTorneo
        );

        MarcarPartidosComoJugados(semifinales);

        var tercerPuesto = servicioCruces.GenerarTercerPuesto(
            semifinales,
            estadios,
            configuracionTorneo
        );

        var final = servicioCruces.GenerarFinal(
            semifinales,
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(8, octavos.Count);
        Assert.AreEqual(4, cuartos.Count);
        Assert.AreEqual(2, semifinales.Count);

        Assert.AreEqual(FaseTorneo.TercerPuesto, tercerPuesto.Fase);
        Assert.AreEqual(FaseTorneo.Final, final.Fase);

        Assert.AreEqual(103, tercerPuesto.Id);
        Assert.AreEqual(104, final.Id);
    }
}
