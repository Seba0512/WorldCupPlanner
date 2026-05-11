using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioFixtureTests
{
    [TestMethod]
    public void GenerarFixture_ConMenosDe48Equipos_DebeCompletarEquiposYGenerarFixture()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1,
            SemillaFixture = 1
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(12, fixture.Grupos.Count);
        Assert.IsTrue(fixture.Grupos.All(g => g.Equipos.Count == 4));

        var todosLosEquipos = fixture.Grupos
            .SelectMany(g => g.Equipos)
            .ToList();

        Assert.AreEqual(48, todosLosEquipos.Count);

        var nombresBase = equipos.Select(e => e.Nombre).ToHashSet();
        Assert.IsTrue(todosLosEquipos.Any(e => !nombresBase.Contains(e.Nombre)));
    }

    [TestMethod]
    public void CompletarEquipos_ConMenosDe48Equipos_DebeAgregarEquiposFicticiosHasta48()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(48, equiposCompletos.Count);
    }

    [TestMethod]
    public void GenerarFixture_ConMenosDe4Estadios_DebeLanzarExcepcion()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(3);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioFixture.GenerarFixture(estadios, configuracionTorneo)
        );
    }

    [TestMethod]
    public void GenerarFixture_Con48EquiposY4Estadios_DebeGenerar12Grupos()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var fixture = servicioFixture.GenerarFixture(estadios, configuracionTorneo);

        // Assert
        Assert.AreEqual(12, fixture.Grupos.Count);
    }

    [TestMethod]
    public void GenerarFixture_DebeCrearGruposConEtiquetasDeAToL()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var fixture = servicioFixture.GenerarFixture(estadios, configuracionTorneo);

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
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var fixture = servicioFixture.GenerarFixture(estadios, configuracionTorneo);

        // Assert
        Assert.IsTrue(fixture.Grupos.All(g => g.Equipos.Count == 4));
    }

    [TestMethod]
    public void GenerarFixture_DebeDistribuirLosEquiposOrdenadosPorRankingEnRoundRobin()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var fixture = servicioFixture.GenerarFixture(estadios, configuracionTorneo);

        // Assert
        var nombresGrupoA = fixture.Grupos[0].Equipos.Select(e => e.Nombre).ToList();
        var nombresGrupoB = fixture.Grupos[1].Equipos.Select(e => e.Nombre).ToList();

        CollectionAssert.AreEqual(
            new List<string> { "Equipo48", "Equipo36", "Equipo24", "Equipo12" },
            nombresGrupoA
        );

        CollectionAssert.AreEqual(
            new List<string> { "Equipo47", "Equipo35", "Equipo23", "Equipo11" },
            nombresGrupoB
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

    private ServicioEquipo CrearServicioEquipoConEquipos(IEnumerable<Equipo> equipos)
    {
        var servicio = new ServicioEquipo();
        foreach (var e in equipos)
            servicio.Agregar(e);
        return servicio;
    }

    [TestMethod]
    public void GenerarFixture_ConEmpatesEnRanking_YDistintaSemilla_DebeGenerarDistribucionDistinta()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        // Cambiar algunos rankings para tener empates, permitiendo diferentes mezclas
        for (int i = 0; i < 12; i++)
        {
            equipos[i].RankingFIFA = 1000;
        }

        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo1 = new ConfiguracionTorneo { SemillaFixture = 1 };
        var configuracionTorneo2 = new ConfiguracionTorneo { SemillaFixture = 2 };

        // Act
        var fixture1 = servicioFixture.GenerarFixture(estadios, configuracionTorneo1);
        var fixture2 = servicioFixture.GenerarFixture(estadios, configuracionTorneo2);

        // Assert
        var ordenFixture1 = fixture1.Grupos
            .SelectMany(g => g.Equipos)
            .Select(e => e.Nombre)
            .ToList();

        var ordenFixture2 = fixture2.Grupos
            .SelectMany(g => g.Equipos)
            .Select(e => e.Nombre)
            .ToList();

        CollectionAssert.AreNotEqual(ordenFixture1, ordenFixture2);
    }

    [TestMethod]
    public void GenerarFixture_ConMismaSemilla_YEmpatesEnRanking_DebeGenerarMismaDistribucion()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();

        for (int i = 0; i < 12; i++)
        {
            equipos[i].RankingFIFA = 1000;
        }

        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo1 = new ConfiguracionTorneo { SemillaFixture = 7 };
        var configuracionTorneo2 = new ConfiguracionTorneo { SemillaFixture = 7 };

        // Act
        var fixture1 = servicioFixture.GenerarFixture(estadios, configuracionTorneo1);
        var fixture2 = servicioFixture.GenerarFixture(estadios, configuracionTorneo2);

        // Assert
        var ordenFixture1 = fixture1.Grupos
            .SelectMany(g => g.Equipos)
            .Select(e => e.Nombre)
            .ToList();

        var ordenFixture2 = fixture2.Grupos
            .SelectMany(g => g.Equipos)
            .Select(e => e.Nombre)
            .ToList();

        CollectionAssert.AreEqual(ordenFixture1, ordenFixture2);
    }

    private List<Equipo> CrearEquiposValidosSinConflictosDeConfederacion()
    {
        var confederacionesPorOrden = new[]
        {
            Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA,
            Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA,
            Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA,

            Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA,
            Confederacion.CONMEBOL, Confederacion.CONMEBOL, Confederacion.CONMEBOL,
            Confederacion.CONMEBOL, Confederacion.CONMEBOL, Confederacion.CONMEBOL,
            Confederacion.CONMEBOL, Confederacion.OFC,

            Confederacion.CONCACAF, Confederacion.CONCACAF, Confederacion.CONCACAF,
            Confederacion.CONCACAF, Confederacion.CONCACAF, Confederacion.CONCACAF,
            Confederacion.CONCACAF,

            Confederacion.CAF, Confederacion.CAF, Confederacion.CAF, Confederacion.CAF,
            Confederacion.CAF, Confederacion.CAF, Confederacion.CAF, Confederacion.CAF,
            Confederacion.CAF,

            Confederacion.AFC, Confederacion.AFC, Confederacion.AFC, Confederacion.AFC,
            Confederacion.AFC, Confederacion.AFC, Confederacion.AFC, Confederacion.AFC
        };

        var equipos = new List<Equipo>();

        for (int posicion = 0; posicion < confederacionesPorOrden.Length; posicion++)
        {
            int numeroEquipo = 48 - posicion;

            equipos.Add(new Equipo
            {
                Nombre = $"Equipo{numeroEquipo}",
                RankingFIFA = 500 + numeroEquipo,
                Confederacion = confederacionesPorOrden[posicion]
            });
        }

        return equipos;
    }

    private List<Equipo> CrearEquiposQueRequierenAjustePorConfederacion()
    {
        var confederacionesPorOrden = new List<Confederacion>
        {
            Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA,
            Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA,
            Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA,

            Confederacion.CONMEBOL,

            Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA, Confederacion.UEFA,

            Confederacion.CONMEBOL, Confederacion.CONMEBOL, Confederacion.CONMEBOL,
            Confederacion.CONMEBOL, Confederacion.CONMEBOL,

            Confederacion.OFC,
            Confederacion.CONCACAF,

            Confederacion.CONMEBOL,

            Confederacion.CONCACAF, Confederacion.CONCACAF, Confederacion.CONCACAF,
            Confederacion.CONCACAF, Confederacion.CONCACAF, Confederacion.CONCACAF,

            Confederacion.CAF, Confederacion.CAF, Confederacion.CAF, Confederacion.CAF,
            Confederacion.CAF, Confederacion.CAF, Confederacion.CAF, Confederacion.CAF,
            Confederacion.CAF,

            Confederacion.AFC, Confederacion.AFC, Confederacion.AFC, Confederacion.AFC,
            Confederacion.AFC, Confederacion.AFC, Confederacion.AFC, Confederacion.AFC
        };

        var equipos = new List<Equipo>();

        for (int posicion = 0; posicion < confederacionesPorOrden.Count; posicion++)
        {
            int numeroEquipo = 48 - posicion;

            equipos.Add(new Equipo
            {
                Nombre = $"Equipo{numeroEquipo}",
                RankingFIFA = 500 + numeroEquipo,
                Confederacion = confederacionesPorOrden[posicion]
            });
        }

        return equipos;
    }

    [TestMethod]
    public void GenerarFixture_DebeCrearGruposValidosRespetandoRestriccionesDeConfederacion()
    {
        // Arrange
        var equipos = CrearEquiposQueRequierenAjustePorConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(estadios, configuracionTorneo);

        // Assert
        Assert.IsTrue(fixture.Grupos.All(g => g.EsValido()));
    }

    [TestMethod]
    public void GenerarFixture_DebeCrearSeisPartidosPorGrupo()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(estadios, configuracionTorneo);

        // Assert
        Assert.IsTrue(fixture.Grupos.All(g => g.Partidos.Count == 6));
    }

    [TestMethod]
    public void GenerarFixture_DebeCrearPartidosDelGrupoSegunPatronDeJornadas()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(estadios, configuracionTorneo);

        // Assert
        var grupo = fixture.Grupos[0];
        var equiposDelGrupo = grupo.Equipos;
        var partidos = grupo.Partidos;

        Assert.AreEqual(equiposDelGrupo[0], partidos[0].EquipoLocal);
        Assert.AreEqual(equiposDelGrupo[3], partidos[0].EquipoVisitante);

        Assert.AreEqual(equiposDelGrupo[1], partidos[1].EquipoLocal);
        Assert.AreEqual(equiposDelGrupo[2], partidos[1].EquipoVisitante);

        Assert.AreEqual(equiposDelGrupo[0], partidos[2].EquipoLocal);
        Assert.AreEqual(equiposDelGrupo[2], partidos[2].EquipoVisitante);

        Assert.AreEqual(equiposDelGrupo[1], partidos[3].EquipoLocal);
        Assert.AreEqual(equiposDelGrupo[3], partidos[3].EquipoVisitante);

        Assert.AreEqual(equiposDelGrupo[0], partidos[4].EquipoLocal);
        Assert.AreEqual(equiposDelGrupo[1], partidos[4].EquipoVisitante);

        Assert.AreEqual(equiposDelGrupo[2], partidos[5].EquipoLocal);
        Assert.AreEqual(equiposDelGrupo[3], partidos[5].EquipoVisitante);
    }

    [TestMethod]
    public void GenerarFixture_DebeAsignarHorariosSegunJornadasDelGrupo()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaFixture = 1,
            FechaInicioTorneo = new DateTime(2026, 6, 1),
            SeparacionEntreFechas = 3,
            HoraInicioPartidos = new TimeSpan(14, 0, 0),
            SeparacionHorasEntrePartidos = 4
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(estadios, configuracionTorneo);

        // Assert
        var partidos = fixture.Grupos[0].Partidos;

        Assert.AreEqual(new DateTime(2026, 6, 1, 14, 0, 0), partidos[0].Fecha);
        Assert.AreEqual(new DateTime(2026, 6, 1, 18, 0, 0), partidos[1].Fecha);

        Assert.AreEqual(new DateTime(2026, 6, 4, 14, 0, 0), partidos[2].Fecha);
        Assert.AreEqual(new DateTime(2026, 6, 4, 18, 0, 0), partidos[3].Fecha);

        Assert.AreEqual(new DateTime(2026, 6, 7, 14, 0, 0), partidos[4].Fecha);
        Assert.AreEqual(new DateTime(2026, 6, 7, 14, 0, 0), partidos[5].Fecha);
    }

    private List<Estadio> CrearEstadiosValidosDesordenadosParaRotacion()
    {
        return new List<Estadio>
        {
            new Estadio { Nombre = "Estadio3", Ciudad = "Ciudad3", Capacidad = 30000 },
            new Estadio { Nombre = "Estadio1", Ciudad = "Ciudad1", Capacidad = 30000 },
            new Estadio { Nombre = "Estadio4", Ciudad = "Ciudad4", Capacidad = 30000 },
            new Estadio { Nombre = "Estadio2", Ciudad = "Ciudad2", Capacidad = 30000 }
        };
    }

    [TestMethod]
    public void GenerarFixture_DebeAsignarEstadiosPorRotacionSegunNombreNormalizado()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidosDesordenadosParaRotacion();
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(estadios, configuracionTorneo);

        // Assert
        var partidos = fixture.Grupos.SelectMany(g => g.Partidos).ToList();

        Assert.AreEqual("Estadio1", partidos[0].Estadio.Nombre);
        Assert.AreEqual("Estadio2", partidos[1].Estadio.Nombre);
        Assert.AreEqual("Estadio3", partidos[2].Estadio.Nombre);
        Assert.AreEqual("Estadio4", partidos[3].Estadio.Nombre);
        Assert.AreEqual("Estadio1", partidos[4].Estadio.Nombre);
    }

    [TestMethod]
    public void GenerarFixture_NoDebeSuperarMaximoPartidosPorDia()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaFixture = 1,
            FechaInicioTorneo = new DateTime(2026, 6, 1),
            SeparacionEntreFechas = 3,
            HoraInicioPartidos = new TimeSpan(14, 0, 0),
            SeparacionHorasEntrePartidos = 4,
            MaxPartidosPorDia = 3
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(estadios, configuracionTorneo);

        // Assert
        var cantidadPartidosPorDia = fixture.Grupos
            .SelectMany(g => g.Partidos)
            .GroupBy(p => p.Fecha.Date)
            .Select(g => g.Count())
            .ToList();

        Assert.IsTrue(cantidadPartidosPorDia.All(cantidad => cantidad <= configuracionTorneo.MaxPartidosPorDia));
    }

    [TestMethod]
    public void GenerarFixture_ConMaxPartidosPorDiaMayor_DebeUsarMasCuposDelDia()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaFixture = 1,
            FechaInicioTorneo = new DateTime(2026, 6, 1),
            SeparacionEntreFechas = 3,
            HoraInicioPartidos = new TimeSpan(14, 0, 0),
            SeparacionHorasEntrePartidos = 4,
            MaxPartidosPorDia = 4
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(estadios, configuracionTorneo);

        // Assert
        var cantidadPartidosPorDia = fixture.Grupos
            .SelectMany(g => g.Partidos)
            .GroupBy(p => p.Fecha.Date)
            .Select(g => g.Count())
            .ToList();

        Assert.IsTrue(cantidadPartidosPorDia.Any(cantidad => cantidad == configuracionTorneo.MaxPartidosPorDia));
        Assert.IsTrue(cantidadPartidosPorDia.All(cantidad => cantidad <= configuracionTorneo.MaxPartidosPorDia));
    }

    [TestMethod]
    public void GenerarFixture_ConMaxPartidosPorDiaMenorAUnaJornada_DebeLanzarExcepcion()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaFixture = 1,
            MaxPartidosPorDia = 1
        };

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioFixture.GenerarFixture(estadios, configuracionTorneo)
        );
    }

    [TestMethod]
    public void RegenerarFixture_ConFixtureSinResultados_DebeGenerarNuevoFixture()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);
        var configuracionOriginal = new ConfiguracionTorneo { SemillaFixture = 1 };
        var configuracionNueva = new ConfiguracionTorneo { SemillaFixture = 2 };
        var fixtureActual = servicioFixture.GenerarFixture(estadios, configuracionOriginal);

        // Act
        var fixtureRegenerado = servicioFixture.RegenerarFixture(
            fixtureActual,
            estadios,
            configuracionNueva);

        // Assert
        Assert.AreEqual(12, fixtureRegenerado.Grupos.Count);
        Assert.AreEqual(72, fixtureRegenerado.Partidos.Count);
    }

    [TestMethod]
    public void RegenerarFixture_ConFixtureConResultados_DebeLanzarExcepcion()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };
        var fixtureActual = servicioFixture.GenerarFixture(estadios, configuracionTorneo);
        fixtureActual.Partidos[0].GolesLocal = 1;
        fixtureActual.Partidos[0].GolesVisitante = 0;

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioFixture.RegenerarFixture(
                fixtureActual,
                estadios,
                configuracionTorneo));
    }

    [TestMethod]
    public void GenerarFixture_DebeAsignarIdsIncrementalesALosPartidos()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(estadios, configuracionTorneo);

        // Assert
        var partidos = fixture.Grupos
            .SelectMany(g => g.Partidos)
            .ToList();

        var idsEsperados = Enumerable.Range(1, 72).ToList();
        var idsObtenidos = partidos.Select(p => p.Id).ToList();

        Assert.AreEqual(72, partidos.Count);
        CollectionAssert.AreEqual(idsEsperados, idsObtenidos);
    }

    [TestMethod]
    public void GenerarFixture_ConEstadioInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(3);
        // Agregar un estadio invalido (capacidad menor a 20000)
        estadios.Add(new Estadio
        {
            Nombre = "EstadioInvalido",
            Ciudad = "Ciudad",
            Capacidad = 10000
        });
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioFixture.GenerarFixture(estadios, configuracionTorneo)
        );
    }

    [TestMethod]
    public void GenerarFixture_DebeAsignarCadaEquipoUnaSolaVez()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var equiposSet = new HashSet<string>(equipos.Select(e => e.Nombre));
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var fixture = servicioFixture.GenerarFixture(estadios, configuracionTorneo);

        // Assert - Total de 48 equipos asignados
        var todosLosEquiposEnFixture = fixture.Grupos.SelectMany(g => g.Equipos).ToList();
        Assert.AreEqual(48, todosLosEquiposEnFixture.Count);

        // Assert - No hay repetidos
        var equiposUnicos = new HashSet<string>(todosLosEquiposEnFixture.Select(e => e.Nombre));
        Assert.AreEqual(48, equiposUnicos.Count);

        // Assert - Todos los originales estan en el fixture
        var equiposEnFixtureSet = new HashSet<string>(todosLosEquiposEnFixture.Select(e => e.Nombre));
        CollectionAssert.AreEquivalent(equiposSet.ToList(), equiposEnFixtureSet.ToList());
    }

    [TestMethod]
    public void GenerarFixture_CadaEquipoDebeJugarTresPartidosEnSuGrupo()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(estadios, configuracionTorneo);

        // Assert
        foreach (var grupo in fixture.Grupos)
        {
            foreach (var equipo in grupo.Equipos)
            {
                var partidosDelEquipo = grupo.Partidos
                    .Where(p => p.EquipoLocal == equipo || p.EquipoVisitante == equipo)
                    .ToList();

                Assert.AreEqual(3, partidosDelEquipo.Count,
                    $"El equipo {equipo.Nombre} en el grupo {grupo.Etiqueta} debe jugar exactamente 3 partidos");
            }
        }
    }

    [TestMethod]
    public void GenerarFixture_NoDebeRepetirEnfrentamientosDentroDelGrupo()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(estadios, configuracionTorneo);

        // Assert
        foreach (var grupo in fixture.Grupos)
        {
            var partidosUnicos = new HashSet<string>();

            foreach (var partido in grupo.Partidos)
            {
                // Crear clave normalizada del enfrentamiento (sin importar orden)
                var clave = string.Concat(
                    new[] { partido.EquipoLocal.Nombre, partido.EquipoVisitante.Nombre }.OrderBy(x => x)
                );

                Assert.IsTrue(
                    partidosUnicos.Add(clave),
                    $"El enfrentamiento entre {partido.EquipoLocal.Nombre} y {partido.EquipoVisitante.Nombre} " +
                    $"aparece repetido en el grupo {grupo.Etiqueta}"
                );
            }
        }
    }

    [TestMethod]
    public void GenerarFixture_TodosLosPartidosGeneradosDebenSerDeFaseDeGrupos()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(estadios, configuracionTorneo);

        // Assert
        foreach (var grupo in fixture.Grupos)
        {
            foreach (var partido in grupo.Partidos)
            {
                Assert.AreEqual(FaseTorneo.FaseDeGrupos, partido.Fase,
                    $"El partido entre {partido.EquipoLocal.Nombre} y {partido.EquipoVisitante.Nombre} " +
                    $"debe tener fase FaseDeGrupos");
            }
        }
    }

    [TestMethod]
    public void GenerarFixture_TodosLosPartidosDebenTenerGrupoAsignado()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(estadios, configuracionTorneo);

        // Assert
        foreach (var grupo in fixture.Grupos)
        {
            foreach (var partido in grupo.Partidos)
            {
                Assert.IsNotNull(partido.Grupo,
                    $"El partido entre {partido.EquipoLocal.Nombre} y {partido.EquipoVisitante.Nombre} debe tener grupo asignado");

                Assert.AreEqual(grupo, partido.Grupo,
                    $"El partido debe pertenencer al grupo {grupo.Etiqueta}");

                var etiqueta = grupo.Etiqueta;
                Assert.IsTrue(etiqueta[0] >= 'A' && etiqueta[0] <= 'L',
                    $"La etiqueta del grupo '{etiqueta}' debe estar entre A y L");
            }
        }
    }

    [TestMethod]
    public void GenerarFixture_TodosLosPartidosDebenTenerEstadioAsignado()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(estadios, configuracionTorneo);

        // Assert
        foreach (var grupo in fixture.Grupos)
        {
            foreach (var partido in grupo.Partidos)
            {
                Assert.IsNotNull(partido.Estadio,
                    $"El partido entre {partido.EquipoLocal.Nombre} y {partido.EquipoVisitante.Nombre} debe tener estadio asignado");

                Assert.IsTrue(estadios.Contains(partido.Estadio),
                    $"El estadio {partido.Estadio.Nombre} debe ser uno de los estadios disponibles");

                Assert.IsTrue(partido.Estadio.EsValido(),
                    $"El estadio {partido.Estadio.Nombre} asignado debe ser valido");
            }
        }
    }

    [TestMethod]
    public void CompletarEquipos_DebeCrearEquiposFicticiosValidos()
    {
        // Arrange
        var equiposBase = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var servicioEquipo = CrearServicioEquipoConEquipos(equiposBase);
        var servicioFixture = new ServicioFixture(servicioEquipo);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            configuracionTorneo
        );

        // Assert
        var nombresBase = equiposBase.Select(e => e.Nombre).ToHashSet();
        var equiposFicticios = equiposCompletos
            .Where(e => !nombresBase.Contains(e.Nombre))
            .ToList();

        Assert.AreEqual(2, equiposFicticios.Count);
        Assert.IsTrue(equiposFicticios.All(e => e.EsValido()));
    }

    [TestMethod]
    public void CompletarEquipos_ConDistintasSemillas_DebePermitirEquiposFicticiosDistintos()
    {
        // Arrange
        var equiposBase1 = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var equiposBase2 = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var servicioEquipo1 = CrearServicioEquipoConEquipos(equiposBase1);
        var servicioEquipo2 = CrearServicioEquipoConEquipos(equiposBase2);
        var servicioFixture1 = new ServicioFixture(servicioEquipo1);
        var servicioFixture2 = new ServicioFixture(servicioEquipo2);

        var configuracionTorneo1 = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        var configuracionTorneo2 = new ConfiguracionTorneo
        {
            SemillaCompletar = 2
        };

        // Act
        var equiposCompletos1 = servicioFixture1.CompletarEquipos(configuracionTorneo1);
        var equiposCompletos2 = servicioFixture2.CompletarEquipos(configuracionTorneo2);

        // Assert
        var nombresBase1 = equiposBase1.Select(e => e.Nombre).ToHashSet();
        var nombresBase2 = equiposBase2.Select(e => e.Nombre).ToHashSet();

        var rankingsFicticios1 = equiposCompletos1
            .Where(e => !nombresBase1.Contains(e.Nombre))
            .Select(e => e.RankingFIFA)
            .ToList();

        var rankingsFicticios2 = equiposCompletos2
            .Where(e => !nombresBase2.Contains(e.Nombre))
            .Select(e => e.RankingFIFA)
            .ToList();

        CollectionAssert.AreNotEqual(rankingsFicticios1, rankingsFicticios2);
    }

    [TestMethod]
    public void CompletarEquipos_ConMismaSemilla_DebeGenerarMismosEquiposFicticios()
    {
        // Arrange
        var equiposBase1 = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var equiposBase2 = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var servicioEquipo1 = CrearServicioEquipoConEquipos(equiposBase1);
        var servicioEquipo2 = CrearServicioEquipoConEquipos(equiposBase2);
        var servicioFixture1 = new ServicioFixture(servicioEquipo1);
        var servicioFixture2 = new ServicioFixture(servicioEquipo2);

        var configuracionTorneo1 = new ConfiguracionTorneo
        {
            SemillaCompletar = 7
        };

        var configuracionTorneo2 = new ConfiguracionTorneo
        {
            SemillaCompletar = 7
        };

        // Act
        var equiposCompletos1 = servicioFixture1.CompletarEquipos(configuracionTorneo1);
        var equiposCompletos2 = servicioFixture2.CompletarEquipos(configuracionTorneo2);

        // Assert
        var nombresBase1 = equiposBase1.Select(e => e.Nombre).ToHashSet();
        var nombresBase2 = equiposBase2.Select(e => e.Nombre).ToHashSet();

        var rankingsFicticios1 = equiposCompletos1
            .Where(e => !nombresBase1.Contains(e.Nombre))
            .Select(e => e.RankingFIFA)
            .ToList();

        var rankingsFicticios2 = equiposCompletos2
            .Where(e => !nombresBase2.Contains(e.Nombre))
            .Select(e => e.RankingFIFA)
            .ToList();

        CollectionAssert.AreEqual(rankingsFicticios1, rankingsFicticios2);
    }

    [TestMethod]
    public void CompletarEquipos_Con48Equipos_NoDebeAgregarEquiposFicticios()
    {
        // Arrange
        var equiposBase = CrearEquiposValidosSinConflictosDeConfederacion();

        var servicioEquipo = CrearServicioEquipoConEquipos(equiposBase);
        var servicioFixture = new ServicioFixture(servicioEquipo);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(48, equiposCompletos.Count);
        var nombresBase = equiposBase.Select(e => e.Nombre).ToHashSet();
        Assert.IsFalse(equiposCompletos.Any(e => !nombresBase.Contains(e.Nombre)));
    }

    [TestMethod]
    public void GenerarFixture_ConMenosDe48Equipos_DebeIncluirEquiposFicticiosEnGrupos()
    {
        // Arrange
        var equiposBase = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var servicioEquipo = CrearServicioEquipoConEquipos(equiposBase);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1,
            SemillaFixture = 1
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(
            estadios,
            configuracionTorneo
        );

        // Assert
        var equiposDelFixture = fixture.Grupos
            .SelectMany(g => g.Equipos)
            .ToList();

        var nombresBase = equiposBase.Select(e => e.Nombre).ToHashSet();
        var equiposFicticios = equiposDelFixture
            .Where(e => !nombresBase.Contains(e.Nombre))
            .ToList();

        Assert.AreEqual(2, equiposFicticios.Count);
        Assert.IsTrue(equiposFicticios.All(e => e.EsValido()));
    }

    [TestMethod]
    public void GenerarFixture_ConMenosDe48Equipos_DebeGenerarGruposValidos()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1,
            SemillaFixture = 1
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.IsTrue(fixture.Grupos.All(g => g.EsValido()));
    }

    [TestMethod]
    public void CompletarEquipos_ConMuchosEquiposFaltantes_DebeRespetarCuposPorConfederacion()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(24)
            .ToList();

        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(48, equiposCompletos.Count);

        Assert.IsTrue(equiposCompletos.Count(e => e.Confederacion == Confederacion.UEFA) <= 16);
        Assert.IsTrue(equiposCompletos.Count(e => e.Confederacion == Confederacion.CONMEBOL) <= 7);
        Assert.IsTrue(equiposCompletos.Count(e => e.Confederacion == Confederacion.CONCACAF) <= 7);
        Assert.IsTrue(equiposCompletos.Count(e => e.Confederacion == Confederacion.CAF) <= 9);
        Assert.IsTrue(equiposCompletos.Count(e => e.Confederacion == Confederacion.AFC) <= 8);
        Assert.IsTrue(equiposCompletos.Count(e => e.Confederacion == Confederacion.OFC) <= 1);
    }

    [TestMethod]
    public void GenerarFixture_ConMuchosEquiposFaltantes_DebeCompletarYGenerarGruposValidos()
    {
        // Arrange
        var equiposBase = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(24)
            .ToList();

        var servicioEquipo = CrearServicioEquipoConEquipos(equiposBase);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1,
            SemillaFixture = 1
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(
            estadios,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(12, fixture.Grupos.Count);
        Assert.IsTrue(fixture.Grupos.All(g => g.Equipos.Count == 4));
        Assert.IsTrue(fixture.Grupos.All(g => g.EsValido()));

        var equiposDelFixture = fixture.Grupos
            .SelectMany(g => g.Equipos)
            .ToList();

        Assert.AreEqual(48, equiposDelFixture.Count);
        var nombresBase = equiposBase.Select(e => e.Nombre).ToHashSet();
        Assert.AreEqual(24, equiposDelFixture.Count(e => !nombresBase.Contains(e.Nombre)));
    }

    [TestMethod]
    public void CompletarEquipos_NoDebeGenerarNombresRepetidosParaEquiposFicticios()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        // Rename before adding to the repo
        equipos[0].Nombre = "AFC_01";

        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            configuracionTorneo
        );

        // Assert
        var nombres = equiposCompletos
            .Select(e => e.Nombre)
            .ToList();

        Assert.AreEqual(48, nombres.Count);
        Assert.AreEqual(48, nombres.Distinct().Count());
    }

    [TestMethod]
    public void CompletarEquipos_DebeGenerarNombresDeterministicosPorConfederacion()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(24)
            .ToList();

        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            configuracionTorneo
        );

        // Assert
        var nombresBase = equipos.Select(e => e.Nombre).ToHashSet();
        var equiposAgregados = equiposCompletos
            .Where(e => !nombresBase.Contains(e.Nombre))
            .ToList();

        Assert.AreEqual(24, equiposAgregados.Count);

        CollectionAssert.AreEqual(
            new List<string>
            {
                "CONCACAF_01", "CONCACAF_02", "CONCACAF_03", "CONCACAF_04",
                "CONCACAF_05", "CONCACAF_06", "CONCACAF_07"
            },
            ObtenerNombresPorConfederacion(equiposAgregados, Confederacion.CONCACAF)
        );

        CollectionAssert.AreEqual(
            new List<string>
            {
                "CAF_01", "CAF_02", "CAF_03", "CAF_04", "CAF_05",
                "CAF_06", "CAF_07", "CAF_08", "CAF_09"
            },
            ObtenerNombresPorConfederacion(equiposAgregados, Confederacion.CAF)
        );

        CollectionAssert.AreEqual(
            new List<string>
            {
                "AFC_01", "AFC_02", "AFC_03", "AFC_04",
                "AFC_05", "AFC_06", "AFC_07", "AFC_08"
            },
            ObtenerNombresPorConfederacion(equiposAgregados, Confederacion.AFC)
        );
    }

    [TestMethod]
    public void GenerarFixture_ConMenosDe48Equipos_NoDebeDuplicarNombresEnGrupos()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        // Rename before adding to the repo
        equipos[0].Nombre = "AFC_01";

        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1,
            SemillaFixture = 1
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(
            estadios,
            configuracionTorneo
        );

        // Assert
        var nombresEquipos = fixture.Grupos
            .SelectMany(g => g.Equipos)
            .Select(e => e.Nombre)
            .ToList();

        Assert.AreEqual(48, nombresEquipos.Count);
        Assert.AreEqual(48, nombresEquipos.Distinct().Count());
    }

    private List<string> ObtenerNombresPorConfederacion(
        List<Equipo> equipos,
        Confederacion confederacion)
    {
        return equipos
            .Where(e => e.Confederacion == confederacion)
            .Select(e => e.Nombre)
            .OrderBy(nombre => nombre)
            .ToList();
    }

    [TestMethod]
    public void GenerarFixture_ConMenosDe48Equipos_DebeIncluirFicticiosConNombresDeterministicosPorConfederacion()
    {
        // Arrange
        var equiposBase = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(24)
            .ToList();

        var servicioEquipo = CrearServicioEquipoConEquipos(equiposBase);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1,
            SemillaFixture = 1
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(
            estadios,
            configuracionTorneo
        );

        // Assert
        var equiposDelFixture = fixture.Grupos
            .SelectMany(g => g.Equipos)
            .ToList();

        var nombresBase = equiposBase.Select(e => e.Nombre).ToHashSet();
        var equiposAgregados = equiposDelFixture
            .Where(e => !nombresBase.Contains(e.Nombre))
            .ToList();

        Assert.AreEqual(24, equiposAgregados.Count);

        Assert.IsTrue(equiposAgregados.Any(e => e.Nombre == "CONCACAF_01"));
        Assert.IsTrue(equiposAgregados.Any(e => e.Nombre == "CAF_01"));
        Assert.IsTrue(equiposAgregados.Any(e => e.Nombre == "AFC_01"));

        Assert.AreEqual(
            equiposDelFixture.Count,
            equiposDelFixture.Select(e => e.Nombre).Distinct().Count()
        );
    }

    [TestMethod]
    public void CompletarEquipos_CuandoSoloFaltaOfc_DebeAgregarEquipoOfc()
    {
        // Arrange — 47 equipos con todas las confederaciones al máximo excepto OFC (0/1).
        var equipos = new List<Equipo>();
        for (int i = 0; i < 16; i++)
            equipos.Add(new Equipo { Nombre = $"UEFA{i + 1}", RankingFIFA = 1000 + i, Confederacion = Confederacion.UEFA });
        for (int i = 0; i < 7; i++)
            equipos.Add(new Equipo { Nombre = $"CON{i + 1}", RankingFIFA = 900 + i, Confederacion = Confederacion.CONMEBOL });
        for (int i = 0; i < 7; i++)
            equipos.Add(new Equipo { Nombre = $"COCA{i + 1}", RankingFIFA = 800 + i, Confederacion = Confederacion.CONCACAF });
        for (int i = 0; i < 9; i++)
            equipos.Add(new Equipo { Nombre = $"CAF{i + 1}", RankingFIFA = 700 + i, Confederacion = Confederacion.CAF });
        for (int i = 0; i < 8; i++)
            equipos.Add(new Equipo { Nombre = $"AFC{i + 1}", RankingFIFA = 600 + i, Confederacion = Confederacion.AFC });

        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaCompletar = 1 };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(configuracionTorneo);

        // Assert
        Assert.AreEqual(48, equiposCompletos.Count);
        Assert.AreEqual(1, equiposCompletos.Count(e => e.Confederacion == Confederacion.OFC));
    }

    [TestMethod]
    public void CompletarEquipos_NoDebeDuplicarNombreCuandoYaExisteNombreDeterministico()
    {
        // Arrange
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(24)
            .ToList();

        // Rename before adding to the repo
        equipos[0].Nombre = "CONCACAF_01";

        var servicioEquipo = CrearServicioEquipoConEquipos(equipos);
        var servicioFixture = new ServicioFixture(servicioEquipo);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            configuracionTorneo
        );

        // Assert
        var nombres = equiposCompletos
            .Select(e => e.Nombre)
            .ToList();

        Assert.AreEqual(48, nombres.Count);
        Assert.AreEqual(48, nombres.Distinct().Count());

        Assert.IsTrue(nombres.Contains("CONCACAF_01"));
        Assert.IsTrue(nombres.Contains("CONCACAF_02"));
    }
}


