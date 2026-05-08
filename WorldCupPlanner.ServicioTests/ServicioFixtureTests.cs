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
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1,
            SemillaFixture = 1
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(
            equipos,
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
        Assert.IsTrue(todosLosEquipos.Any(e => e.Nombre.StartsWith("Equipo Ficticio")));
    }

    [TestMethod]
    public void CompletarEquipos_ConMenosDe48Equipos_DebeAgregarEquiposFicticiosHasta48()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            equipos,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(48, equiposCompletos.Count);
    }

    [TestMethod]
    public void CompletarEquipos_ConMasDe48Equipos_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();

        equipos.Add(new Equipo
        {
            Nombre = "EquipoExtra",
            RankingFIFA = 1000,
            Confederacion = Confederacion.CONMEBOL
        });

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioFixture.CompletarEquipos(
                equipos,
                configuracionTorneo
            )
        );
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
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo)
        );
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

    [TestMethod]
    public void GenerarFixture_DebeDistribuirLosEquiposOrdenadosPorRankingEnRoundRobin()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

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
    public void GenerarFixture_ConEmpatesEnRanking_YDistintaSemilla_DebeGenerarDistribucionDistinta()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        // Cambiar algunos rankings para tener empates, permitiendo diferentes mezclas
        for (int i = 0; i < 12; i++)
        {
            equipos[i].RankingFIFA = 1000;
        }
        
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo1 = new ConfiguracionTorneo { SemillaFixture = 1 };
        var configuracionTorneo2 = new ConfiguracionTorneo { SemillaFixture = 2 };

        // Act
        var fixture1 = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo1);
        var fixture2 = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo2);

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
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        
        for (int i = 0; i < 12; i++)
        {
            equipos[i].RankingFIFA = 1000;
        }
        
        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo1 = new ConfiguracionTorneo { SemillaFixture = 7 };
        var configuracionTorneo2 = new ConfiguracionTorneo { SemillaFixture = 7 };

        // Act
        var fixture1 = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo1);
        var fixture2 = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo2);

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
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposQueRequierenAjustePorConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        Assert.IsTrue(fixture.Grupos.All(g => g.EsValido()));
    }
    
    [TestMethod]
    public void GenerarFixture_DebeCrearSeisPartidosPorGrupo()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        Assert.IsTrue(fixture.Grupos.All(g => g.Partidos.Count == 6));
    }
    
    [TestMethod]
    public void GenerarFixture_DebeCrearPartidosDelGrupoSegunPatronDeJornadas()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

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
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
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
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

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
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidosDesordenadosParaRotacion();
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

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
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
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
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

        // Assert
        var cantidadPartidosPorDia = fixture.Grupos
            .SelectMany(g => g.Partidos)
            .GroupBy(p => p.Fecha.Date)
            .Select(g => g.Count())
            .ToList();

        Assert.IsTrue(cantidadPartidosPorDia.All(cantidad => cantidad <= configuracionTorneo.MaxPartidosPorDia));
    }

    [TestMethod]
    public void GenerarFixture_DebeAsignarIdsIncrementalesALosPartidos()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

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
    public void GenerarFixture_ConMasDe48Equipos_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidos(49);
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo)
        );
    }

    [TestMethod]
    public void GenerarFixture_ConEquipoInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidos(47);
        // Agregar un equipo invalido (sin confederacion)
        equipos.Add(new Equipo
        {
            Nombre = "EquipoInvalido",
            RankingFIFA = 500,
            Confederacion = null
        });
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo)
        );
    }

    [TestMethod]
    public void GenerarFixture_ConEstadioInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidos(48);
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
            servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo)
        );
    }

    [TestMethod]
    public void GenerarFixture_DebeAsignarCadaEquipoUnaSolaVez()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var equiposSet = new HashSet<string>(equipos.Select(e => e.Nombre));
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo();

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

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
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

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
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

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
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

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
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

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
        var servicioFixture = new ServicioFixture();
        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();
        var estadios = CrearEstadiosValidos(4);
        var configuracionTorneo = new ConfiguracionTorneo { SemillaFixture = 1 };

        // Act
        var fixture = servicioFixture.GenerarFixture(equipos, estadios, configuracionTorneo);

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
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            equipos,
            configuracionTorneo
        );

        // Assert
        var equiposFicticios = equiposCompletos
            .Where(e => e.Nombre.StartsWith("Equipo Ficticio"))
            .ToList();

        Assert.AreEqual(2, equiposFicticios.Count);
        Assert.IsTrue(equiposFicticios.All(e => e.EsValido()));
    }
    
    [TestMethod]
    public void CompletarEquipos_ConDistintasSemillas_DebePermitirEquiposFicticiosDistintos()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equiposBase1 = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var equiposBase2 = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var configuracionTorneo1 = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        var configuracionTorneo2 = new ConfiguracionTorneo
        {
            SemillaCompletar = 2
        };

        // Act
        var equiposCompletos1 = servicioFixture.CompletarEquipos(
            equiposBase1,
            configuracionTorneo1
        );

        var equiposCompletos2 = servicioFixture.CompletarEquipos(
            equiposBase2,
            configuracionTorneo2
        );

        // Assert
        var rankingsFicticios1 = equiposCompletos1
            .Where(e => e.Nombre.StartsWith("Equipo Ficticio"))
            .Select(e => e.RankingFIFA)
            .ToList();

        var rankingsFicticios2 = equiposCompletos2
            .Where(e => e.Nombre.StartsWith("Equipo Ficticio"))
            .Select(e => e.RankingFIFA)
            .ToList();

        CollectionAssert.AreNotEqual(rankingsFicticios1, rankingsFicticios2);
    }
    
    [TestMethod]
    public void CompletarEquipos_ConMismaSemilla_DebeGenerarMismosEquiposFicticios()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equiposBase1 = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var equiposBase2 = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var configuracionTorneo1 = new ConfiguracionTorneo
        {
            SemillaCompletar = 7
        };

        var configuracionTorneo2 = new ConfiguracionTorneo
        {
            SemillaCompletar = 7
        };

        // Act
        var equiposCompletos1 = servicioFixture.CompletarEquipos(
            equiposBase1,
            configuracionTorneo1
        );

        var equiposCompletos2 = servicioFixture.CompletarEquipos(
            equiposBase2,
            configuracionTorneo2
        );

        // Assert
        var rankingsFicticios1 = equiposCompletos1
            .Where(e => e.Nombre.StartsWith("Equipo Ficticio"))
            .Select(e => e.RankingFIFA)
            .ToList();

        var rankingsFicticios2 = equiposCompletos2
            .Where(e => e.Nombre.StartsWith("Equipo Ficticio"))
            .Select(e => e.RankingFIFA)
            .ToList();

        CollectionAssert.AreEqual(rankingsFicticios1, rankingsFicticios2);
    }
    
    [TestMethod]
    public void CompletarEquipos_NoDebeModificarListaOriginal()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(24)
            .ToList();

        var cantidadOriginal = equipos.Count;

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            equipos,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(24, cantidadOriginal);
        Assert.AreEqual(24, equipos.Count);
        Assert.AreEqual(48, equiposCompletos.Count);
    }
    
    [TestMethod]
    public void CompletarEquipos_Con48Equipos_NoDebeAgregarEquiposFicticios()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion();

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            equipos,
            configuracionTorneo
        );

        // Assert
        Assert.AreEqual(48, equiposCompletos.Count);
        Assert.IsFalse(equiposCompletos.Any(e => e.Nombre.StartsWith("Equipo Ficticio")));
    }
    
    [TestMethod]
    public void CompletarEquipos_ConEquipoOriginalInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(47)
            .ToList();

        equipos[0].Nombre = "";

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioFixture.CompletarEquipos(
                equipos,
                configuracionTorneo
            )
        );
    }
    
    [TestMethod]
    public void GenerarFixture_ConMenosDe48Equipos_DebeIncluirEquiposFicticiosEnGrupos()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1,
            SemillaFixture = 1
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(
            equipos,
            estadios,
            configuracionTorneo
        );

        // Assert
        var equiposDelFixture = fixture.Grupos
            .SelectMany(g => g.Equipos)
            .ToList();

        var equiposFicticios = equiposDelFixture
            .Where(e => e.Nombre.StartsWith("Equipo Ficticio"))
            .ToList();

        Assert.AreEqual(2, equiposFicticios.Count);
        Assert.IsTrue(equiposFicticios.All(e => e.EsValido()));
    }
    
    [TestMethod]
    public void GenerarFixture_ConMenosDe48Equipos_DebeGenerarGruposValidos()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1,
            SemillaFixture = 1
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(
            equipos,
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
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(24)
            .ToList();

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            equipos,
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
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(24)
            .ToList();

        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1,
            SemillaFixture = 1
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(
            equipos,
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
        Assert.AreEqual(24, equiposDelFixture.Count(e => e.Nombre.StartsWith("Equipo Ficticio")));
    }

    [TestMethod]
    public void CompletarEquipos_NoDebeGenerarNombresRepetidosParaEquiposFicticios()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        equipos[0].Nombre = "Equipo Ficticio 47";

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1
        };

        // Act
        var equiposCompletos = servicioFixture.CompletarEquipos(
            equipos,
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
    public void GenerarFixture_ConMenosDe48Equipos_NoDebeDuplicarNombresEnGrupos()
    {
        // Arrange
        var servicioFixture = new ServicioFixture();

        var equipos = CrearEquiposValidosSinConflictosDeConfederacion()
            .Take(46)
            .ToList();

        equipos[0].Nombre = "Equipo Ficticio 47";

        var estadios = CrearEstadiosValidos(4);

        var configuracionTorneo = new ConfiguracionTorneo
        {
            SemillaCompletar = 1,
            SemillaFixture = 1
        };

        // Act
        var fixture = servicioFixture.GenerarFixture(
            equipos,
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
}


