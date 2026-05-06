using  WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioClasificacionTests
{
    [TestMethod]
    public void CalcularTabla_ConResultadosDeGrupo_CalculaPuntosCorrectamente()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();

        var equipo1 = CrearEquipo("Equipo1");
        var equipo2 = CrearEquipo("Equipo2");
        var equipo3 = CrearEquipo("Equipo3");
        var equipo4 = CrearEquipo("Equipo4");

        var grupo = new Grupo("A", new List<Equipo>
        {
            equipo1,
            equipo2,
            equipo3,
            equipo4
        });

        grupo.Partidos.Add(CrearPartido(equipo1, equipo4, 2, 0)); // Equipo1 gana
        grupo.Partidos.Add(CrearPartido(equipo2, equipo3, 1, 1)); // Empate
        grupo.Partidos.Add(CrearPartido(equipo1, equipo3, 0, 0)); // Empate
        grupo.Partidos.Add(CrearPartido(equipo2, equipo4, 3, 1)); // Equipo2 gana
        grupo.Partidos.Add(CrearPartido(equipo1, equipo2, 1, 0)); // Equipo1 gana
        grupo.Partidos.Add(CrearPartido(equipo3, equipo4, 2, 1)); // Equipo3 gana

        // Act
        var tabla = servicioClasificacion.CalcularTabla(grupo);

        // Assert
        Assert.AreEqual(7, ObtenerPuntos(tabla, equipo1));
        Assert.AreEqual(4, ObtenerPuntos(tabla, equipo2));
        Assert.AreEqual(5, ObtenerPuntos(tabla, equipo3));
        Assert.AreEqual(0, ObtenerPuntos(tabla, equipo4));
    }

    private Equipo CrearEquipo(string nombre)
    {
        return CrearEquipo(nombre, Confederacion.UEFA);
    }

    private Equipo CrearEquipo(string nombre, Confederacion confederacion)
    {
        return new Equipo
        {
            Nombre = nombre,
            RankingFIFA = 1000,
            Confederacion = confederacion
        };
    }

    private Partido CrearPartido(Equipo local, Equipo visitante, int golesLocal, int golesVisitante)
    {
        return new Partido
        {
            EquipoLocal = local,
            EquipoVisitante = visitante,
            GolesLocal = golesLocal,
            GolesVisitante = golesVisitante,
            Fase = FaseTorneo.FaseDeGrupos,
            Estadio = new Estadio
            {
                Nombre = "Estadio",
                Ciudad = "Ciudad",
                Capacidad = 30000
            },
            Fecha = DateTime.Now
        };
    }

    private int ObtenerPuntos(List<PosicionEquipo> tabla, Equipo equipo)
    {
        return tabla.Single(p => p.Equipo == equipo).Puntos;
    }
    
    [TestMethod]
    public void CalcularTabla_ConResultadosDeGrupo_OrdenaPorPuntosDescendente()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();

        var equipo1 = CrearEquipo("Equipo1");
        var equipo2 = CrearEquipo("Equipo2");
        var equipo3 = CrearEquipo("Equipo3");
        var equipo4 = CrearEquipo("Equipo4");

        var grupo = new Grupo("A", new List<Equipo>
        {
            equipo1,
            equipo2,
            equipo3,
            equipo4
        });

        grupo.Partidos.Add(CrearPartido(equipo1, equipo4, 2, 0)); // Equipo1 gana: 3 pts
        grupo.Partidos.Add(CrearPartido(equipo2, equipo3, 1, 1)); // Equipo2 y Equipo3: 1 pt
        grupo.Partidos.Add(CrearPartido(equipo1, equipo3, 0, 0)); // Equipo1 y Equipo3: +1 pt
        grupo.Partidos.Add(CrearPartido(equipo2, equipo4, 3, 1)); // Equipo2 gana: +3 pts
        grupo.Partidos.Add(CrearPartido(equipo1, equipo2, 1, 0)); // Equipo1 gana: +3 pts
        grupo.Partidos.Add(CrearPartido(equipo3, equipo4, 2, 1)); // Equipo3 gana: +3 pts

        // Act
        var tabla = servicioClasificacion.CalcularTabla(grupo);

        // Assert
        var ordenEsperado = new List<Equipo>
        {
            equipo1, // 7 puntos
            equipo3, // 5 puntos
            equipo2, // 4 puntos
            equipo4  // 0 puntos
        };

        var ordenObtenido = tabla.Select(p => p.Equipo).ToList();

        CollectionAssert.AreEqual(ordenEsperado, ordenObtenido);
    }
    
    [TestMethod]
    public void CalcularTabla_ConMismosPuntos_OrdenaPorDiferenciaDeGolesDescendente()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();

        var equipo1 = CrearEquipo("Equipo1");
        var equipo2 = CrearEquipo("Equipo2");
        var equipo3 = CrearEquipo("Equipo3");
        var equipo4 = CrearEquipo("Equipo4");

        var grupo = new Grupo("A", new List<Equipo>
        {
            equipo1,
            equipo2,
            equipo3,
            equipo4
        });

        grupo.Partidos.Add(CrearPartido(equipo1, equipo2, 0, 3)); // Equipo2 gana por 3
        grupo.Partidos.Add(CrearPartido(equipo3, equipo4, 2, 0)); // Equipo3 gana por 2
        grupo.Partidos.Add(CrearPartido(equipo1, equipo3, 1, 0)); // Equipo1 gana por 1
        grupo.Partidos.Add(CrearPartido(equipo2, equipo4, 0, 1)); // Equipo4 gana por 1
        grupo.Partidos.Add(CrearPartido(equipo1, equipo4, 0, 0)); // Empate
        grupo.Partidos.Add(CrearPartido(equipo2, equipo3, 0, 0)); // Empate

        // Act
        var tabla = servicioClasificacion.CalcularTabla(grupo);

        // Assert
        var ordenEsperado = new List<Equipo>
        {
            equipo2, // 4 puntos, diferencia +2
            equipo3, // 4 puntos, diferencia +1
            equipo4, // 4 puntos, diferencia -1
            equipo1  // 4 puntos, diferencia -2
        };

        var ordenObtenido = tabla.Select(p => p.Equipo).ToList();

        CollectionAssert.AreEqual(ordenEsperado, ordenObtenido);
    }
    
    [TestMethod]
    public void CalcularTabla_ConMismosPuntosYMismaDiferencia_OrdenaPorGolesAFavorDescendente()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();

        var equipo1 = CrearEquipo("Equipo1");
        var equipo2 = CrearEquipo("Equipo2");
        var equipo3 = CrearEquipo("Equipo3");
        var equipo4 = CrearEquipo("Equipo4");

        var grupo = new Grupo("A", new List<Equipo>
        {
            equipo1,
            equipo2,
            equipo3,
            equipo4
        });

        grupo.Partidos.Add(CrearPartido(equipo1, equipo2, 4, 3)); // Equipo1 gana
        grupo.Partidos.Add(CrearPartido(equipo3, equipo4, 6, 7)); // Equipo4 gana
        grupo.Partidos.Add(CrearPartido(equipo1, equipo3, 2, 3)); // Equipo3 gana
        grupo.Partidos.Add(CrearPartido(equipo2, equipo4, 5, 4)); // Equipo2 gana
        grupo.Partidos.Add(CrearPartido(equipo1, equipo4, 0, 0)); // Empate
        grupo.Partidos.Add(CrearPartido(equipo2, equipo3, 1, 1)); // Empate

        // Act
        var tabla = servicioClasificacion.CalcularTabla(grupo);

        // Assert
        var ordenEsperado = new List<Equipo>
        {
            equipo4, // 4 puntos, diferencia 0, 11 goles a favor
            equipo3, // 4 puntos, diferencia 0, 10 goles a favor
            equipo2, // 4 puntos, diferencia 0, 9 goles a favor
            equipo1  // 4 puntos, diferencia 0, 6 goles a favor
        };

        var ordenObtenido = tabla.Select(p => p.Equipo).ToList();

        CollectionAssert.AreEqual(ordenEsperado, ordenObtenido);
    }
    
    [TestMethod]
public void CalcularTabla_ConEmpateTotalYMismaSemilla_DebeGenerarMismoDesempate()
{
    // Arrange
    var servicioClasificacion = new ServicioClasificacion();
    var grupo = CrearGrupoConEmpateTotal();

    var configuracionTorneo1 = new ConfiguracionTorneo { SemillaCrucesFase = 7 };
    var configuracionTorneo2 = new ConfiguracionTorneo { SemillaCrucesFase = 7 };

    // Act
    var tabla1 = servicioClasificacion.CalcularTabla(grupo, configuracionTorneo1);
    var tabla2 = servicioClasificacion.CalcularTabla(grupo, configuracionTorneo2);

    // Assert
    var orden1 = tabla1.Select(p => p.Equipo.Nombre).ToList();
    var orden2 = tabla2.Select(p => p.Equipo.Nombre).ToList();

    CollectionAssert.AreEqual(orden1, orden2);
}

[TestMethod]
public void CalcularTabla_ConEmpateTotalYDistintasSemillas_DebePermitirDesempateDistinto()
{
    // Arrange
    var servicioClasificacion = new ServicioClasificacion();
    var grupo = CrearGrupoConEmpateTotal();

    var configuracionReferencia = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

    // Act
    var tablaReferencia = servicioClasificacion.CalcularTabla(grupo, configuracionReferencia);
    var ordenReferencia = tablaReferencia.Select(p => p.Equipo.Nombre).ToList();

    var existeOrdenDistinto = false;

    for (int semilla = 2; semilla <= 20; semilla++)
    {
        var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = semilla };
        var tabla = servicioClasificacion.CalcularTabla(grupo, configuracionTorneo);
        var orden = tabla.Select(p => p.Equipo.Nombre).ToList();

        if (!orden.SequenceEqual(ordenReferencia))
        {
            existeOrdenDistinto = true;
            break;
        }
    }

    // Assert
    Assert.IsTrue(existeOrdenDistinto);
}

private Grupo CrearGrupoConEmpateTotal()
{
    var equipo1 = CrearEquipo("Equipo1");
    var equipo2 = CrearEquipo("Equipo2");
    var equipo3 = CrearEquipo("Equipo3");
    var equipo4 = CrearEquipo("Equipo4");

    var grupo = new Grupo("A", new List<Equipo>
    {
        equipo1,
        equipo2,
        equipo3,
        equipo4
    });

    grupo.Partidos.Add(CrearPartido(equipo1, equipo2, 0, 0));
    grupo.Partidos.Add(CrearPartido(equipo3, equipo4, 0, 0));
    grupo.Partidos.Add(CrearPartido(equipo1, equipo3, 0, 0));
    grupo.Partidos.Add(CrearPartido(equipo2, equipo4, 0, 0));
    grupo.Partidos.Add(CrearPartido(equipo1, equipo4, 0, 0));
    grupo.Partidos.Add(CrearPartido(equipo2, equipo3, 0, 0));

    return grupo;
}

private Fixture CrearFixtureConDoceGruposResueltos()
{
    var fixture = new Fixture();

    for (char etiqueta = 'A'; etiqueta <= 'L'; etiqueta++)
    {
        fixture.Grupos.Add(CrearGrupoResuelto(etiqueta.ToString()));
    }

    return fixture;
}

private Grupo CrearGrupoResuelto(string etiqueta)
{
    var equipo1 = CrearEquipo($"{etiqueta}1", Confederacion.UEFA);
    var equipo2 = CrearEquipo($"{etiqueta}2", Confederacion.CONMEBOL);
    var equipo3 = CrearEquipo($"{etiqueta}3", Confederacion.AFC);
    var equipo4 = CrearEquipo($"{etiqueta}4", Confederacion.CAF);

    var grupo = new Grupo(etiqueta, new List<Equipo>
    {
        equipo1,
        equipo2,
        equipo3,
        equipo4
    });

    grupo.Partidos.Add(CrearPartido(equipo1, equipo2, 3, 0));
    grupo.Partidos.Add(CrearPartido(equipo1, equipo3, 3, 0));
    grupo.Partidos.Add(CrearPartido(equipo1, equipo4, 3, 0));
    grupo.Partidos.Add(CrearPartido(equipo2, equipo3, 2, 0));
    grupo.Partidos.Add(CrearPartido(equipo2, equipo4, 2, 0));
    grupo.Partidos.Add(CrearPartido(equipo3, equipo4, 1, 0));

    return grupo;
}

[TestMethod]
public void ObtenerClasificadosAFaseEliminatoria_DebeTomarPrimerosSegundosYMejoresTerceros()
{
    // Arrange
    var servicioClasificacion = new ServicioClasificacion();
    var fixture = CrearFixtureConDoceGruposResueltos();
    var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

    // Act
    var clasificados = servicioClasificacion.ObtenerClasificadosAFaseEliminatoria(
        fixture,
        configuracionTorneo
    );

    // Assert
    Assert.AreEqual(12, clasificados.Primeros.Count);
    Assert.AreEqual(12, clasificados.Segundos.Count);
    Assert.AreEqual(8, clasificados.MejoresTerceros.Count);
    Assert.AreEqual(32, clasificados.Todos.Count);

    Assert.IsTrue(clasificados.Primeros.All(e => e.Nombre.EndsWith("1")));
    Assert.IsTrue(clasificados.Segundos.All(e => e.Nombre.EndsWith("2")));
}

[TestMethod]
public void ObtenerClasificadosAFaseEliminatoria_ConTercerosTotalmenteEmpatadosYDistintasSemillas_DebePermitirMejoresTercerosDistintos()
{
    // Arrange
    var servicioClasificacion = new ServicioClasificacion();
    var fixture = CrearFixtureConDoceGruposResueltos();

    var configuracionReferencia = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

    // Act
    var clasificadosReferencia = servicioClasificacion.ObtenerClasificadosAFaseEliminatoria(
        fixture,
        configuracionReferencia
    );

    var mejoresTercerosReferencia = clasificadosReferencia.MejoresTerceros
        .Select(e => e.Nombre)
        .ToList();

    var existeSeleccionDistinta = false;

    for (int semilla = 2; semilla <= 20; semilla++)
    {
        var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = semilla };

        var clasificados = servicioClasificacion.ObtenerClasificadosAFaseEliminatoria(
            fixture,
            configuracionTorneo
        );

        var mejoresTerceros = clasificados.MejoresTerceros
            .Select(e => e.Nombre)
            .ToList();

        if (!mejoresTerceros.SequenceEqual(mejoresTercerosReferencia))
        {
            existeSeleccionDistinta = true;
            break;
        }
    }

    // Assert
    Assert.IsTrue(existeSeleccionDistinta);
}

    [TestMethod]
    public void ObtenerClasificadosAFaseEliminatoria_DebeOrdenarPrimerosPorRendimientoGlobal()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();
        var fixture = CrearFixtureConPrimerGrupoDeMenorRendimiento();
        var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

        var primeroGrupoA = fixture.Grupos.Single(g => g.Etiqueta == "A").Equipos[0];
        var primeroGrupoB = fixture.Grupos.Single(g => g.Etiqueta == "B").Equipos[0];

        // Act
        var clasificados = servicioClasificacion.ObtenerClasificadosAFaseEliminatoria(
            fixture,
            configuracionTorneo
        );

        // Assert
        Assert.IsTrue(
            clasificados.Primeros.IndexOf(primeroGrupoB) < clasificados.Primeros.IndexOf(primeroGrupoA)
        );
    }

    [TestMethod]
    public void ObtenerClasificadosAFaseEliminatoria_DebeOrdenarSegundosPorRendimientoGlobal()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();
        var fixture = CrearFixtureConPrimerGrupoDeMenorRendimiento();
        var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

        var segundoGrupoA = fixture.Grupos.Single(g => g.Etiqueta == "A").Equipos[1];
        var segundoGrupoB = fixture.Grupos.Single(g => g.Etiqueta == "B").Equipos[1];

        // Act
        var clasificados = servicioClasificacion.ObtenerClasificadosAFaseEliminatoria(
            fixture,
            configuracionTorneo
        );

        // Assert
        Assert.IsTrue(
            clasificados.Segundos.IndexOf(segundoGrupoB) < clasificados.Segundos.IndexOf(segundoGrupoA)
        );
    }

    [TestMethod]
    public void ObtenerClasificadosAFaseEliminatoria_DebeGuardarGrupoDeOrigenDeCadaClasificado()
    {
        // Arrange
        var servicioClasificacion = new ServicioClasificacion();
        var fixture = CrearFixtureConDoceGruposResueltos();
        var configuracionTorneo = new ConfiguracionTorneo { SemillaCrucesFase = 1 };

        var grupoA = fixture.Grupos.Single(g => g.Etiqueta == "A");
        var primerClasificadoGrupoA = grupoA.Equipos[0];
        var segundoClasificadoGrupoA = grupoA.Equipos[1];

        // Act
        var clasificados = servicioClasificacion.ObtenerClasificadosAFaseEliminatoria(
            fixture,
            configuracionTorneo
        );

        // Assert
        Assert.AreSame(grupoA, clasificados.ObtenerGrupoDeOrigen(primerClasificadoGrupoA));
        Assert.AreSame(grupoA, clasificados.ObtenerGrupoDeOrigen(segundoClasificadoGrupoA));
    }

    private Fixture CrearFixtureConPrimerGrupoDeMenorRendimiento()
    {
        var fixture = new Fixture();
        fixture.Grupos.Add(CrearGrupoConPrimerYSegundoDeMenorRendimiento("A"));

        for (char etiqueta = 'B'; etiqueta <= 'L'; etiqueta++)
        {
            fixture.Grupos.Add(CrearGrupoResuelto(etiqueta.ToString()));
        }

        return fixture;
    }

    private Grupo CrearGrupoConPrimerYSegundoDeMenorRendimiento(string etiqueta)
    {
        var equipo1 = CrearEquipo($"{etiqueta}1", Confederacion.UEFA);
        var equipo2 = CrearEquipo($"{etiqueta}2", Confederacion.CONMEBOL);
        var equipo3 = CrearEquipo($"{etiqueta}3", Confederacion.AFC);
        var equipo4 = CrearEquipo($"{etiqueta}4", Confederacion.CAF);

        var grupo = new Grupo(etiqueta, new List<Equipo>
        {
            equipo1,
            equipo2,
            equipo3,
            equipo4
        });

        grupo.Partidos.Add(CrearPartido(equipo1, equipo2, 1, 1));
        grupo.Partidos.Add(CrearPartido(equipo1, equipo3, 2, 0));
        grupo.Partidos.Add(CrearPartido(equipo1, equipo4, 2, 0));
        grupo.Partidos.Add(CrearPartido(equipo2, equipo3, 1, 0));
        grupo.Partidos.Add(CrearPartido(equipo2, equipo4, 0, 0));
        grupo.Partidos.Add(CrearPartido(equipo3, equipo4, 1, 0));

        return grupo;
    }

}
