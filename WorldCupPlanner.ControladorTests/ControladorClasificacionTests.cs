using WorldCupPlanner.Controladores;
using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ControladorTests;

[TestClass]
public class ControladorClasificacionTests
{
    private int _proximoEquipoId = 1;

    [TestMethod]
    public void CalcularTabla_SinConfiguracion_DelegaEnServicioClasificacion()
    {
        var servicio = new ServicioClasificacion();
        var controlador = new ControladorClasificacion(servicio);
        var grupo = CrearGrupoConResultados("A");

        var tablaEsperada = servicio.CalcularTabla(grupo);
        var tablaObtenida = controlador.CalcularTabla(grupo);

        AssertTablasEquivalentes(tablaEsperada, tablaObtenida);
    }

    [TestMethod]
    public void CalcularTabla_ConConfiguracion_DelegaRespetandoSemillaDeDesempate()
    {
        var servicio = new ServicioClasificacion();
        var controlador = new ControladorClasificacion(servicio);
        var grupo = CrearGrupoConEmpateTotal("B");
        var configuracion = new ConfiguracionTorneo { SemillaCrucesFase = 9 };

        var tablaEsperada = servicio.CalcularTabla(grupo, configuracion);
        var tablaObtenida = controlador.CalcularTabla(grupo, configuracion);

        AssertTablasEquivalentes(tablaEsperada, tablaObtenida);
    }

    [TestMethod]
    public void CalcularTablaDTO_DevuelveDatosDeTablaParaUI()
    {
        var controlador = new ControladorClasificacion(new ServicioClasificacion());
        var grupo = CrearGrupoConResultados("A");

        var tabla = controlador.CalcularTablaDTO(grupo).ToList();

        Assert.AreEqual(4, tabla.Count);
        Assert.IsInstanceOfType(tabla[0], typeof(PosicionEquipoDTO));
        Assert.AreEqual(1, tabla[0].Posicion);
        Assert.AreEqual(tabla[0].Equipo?.Nombre, tabla[0].EquipoNombre);
        Assert.AreEqual("A", tabla[0].GrupoEtiqueta);
        Assert.IsTrue(tabla[0].Clasifica);
        Assert.IsFalse(tabla[0].EsTercero);
    }

    [TestMethod]
    public void CalcularTablaDTO_RespetaOrdenYDatosDelServicio()
    {
        var servicio = new ServicioClasificacion();
        var controlador = new ControladorClasificacion(servicio);
        var grupo = CrearGrupoConEmpateTotal("B");
        var configuracion = new ConfiguracionTorneo { SemillaCrucesFase = 9 };

        var esperada = servicio.CalcularTabla(grupo, configuracion);
        var obtenida = controlador.CalcularTablaDTO(grupo, configuracion).ToList();

        CollectionAssert.AreEqual(
            esperada.Select(p => p.Equipo.Nombre).ToList(),
            obtenida.Select(p => p.EquipoNombre).ToList());
        CollectionAssert.AreEqual(
            esperada.Select(p => p.Posicion).ToList(),
            obtenida.Select(p => p.Posicion).ToList());
    }

    [TestMethod]
    public void ObtenerClasificadosAFaseEliminatoria_DelegaEnServicioClasificacion()
    {
        var servicio = new ServicioClasificacion();
        var controlador = new ControladorClasificacion(servicio);
        var fixture = CrearFixtureConDoceGruposResueltos();
        var configuracion = new ConfiguracionTorneo { SemillaCrucesFase = 3 };

        var esperados = servicio.ObtenerClasificadosAFaseEliminatoria(fixture, configuracion);
        var obtenidos = controlador.ObtenerClasificadosAFaseEliminatoria(fixture, configuracion);

        CollectionAssert.AreEqual(esperados.Primeros.Select(e => e.Nombre).ToList(), obtenidos.Primeros.Select(e => e.Nombre).ToList());
        CollectionAssert.AreEqual(esperados.Segundos.Select(e => e.Nombre).ToList(), obtenidos.Segundos.Select(e => e.Nombre).ToList());
        CollectionAssert.AreEqual(esperados.MejoresTerceros.Select(e => e.Nombre).ToList(), obtenidos.MejoresTerceros.Select(e => e.Nombre).ToList());
        Assert.AreEqual(32, obtenidos.Todos.Count);
        Assert.AreSame(
            esperados.ObtenerGrupoDeOrigen(esperados.Primeros[0]),
            obtenidos.ObtenerGrupoDeOrigen(obtenidos.Primeros[0]));
    }

    [TestMethod]
    public void ObtenerClasificadosAFaseEliminatoria_ConPartidoPendiente_PropagaExcepcionDelServicio()
    {
        var controlador = new ControladorClasificacion(new ServicioClasificacion());
        var fixture = CrearFixtureConDoceGruposResueltos();
        var configuracion = new ConfiguracionTorneo { SemillaCrucesFase = 1 };
        fixture.Grupos[0].Partidos[0].GolesLocal = null;
        fixture.Grupos[0].Partidos[0].GolesVisitante = null;

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            controlador.ObtenerClasificadosAFaseEliminatoria(fixture, configuracion));
    }

    [TestMethod]
    public void ObtenerClasificadosAFaseEliminatoriaDTO_DevuelveClasificadosSinExponerDominio()
    {
        var controlador = new ControladorClasificacion(new ServicioClasificacion());
        var fixture = CrearFixtureConDoceGruposResueltos();
        var configuracion = new ConfiguracionTorneo { SemillaCrucesFase = 3 };

        var clasificados = controlador.ObtenerClasificadosAFaseEliminatoriaDTO(fixture, configuracion);

        Assert.IsInstanceOfType(clasificados, typeof(ClasificadosFaseGruposDTO));
        Assert.AreEqual(12, clasificados.Primeros.Count);
        Assert.AreEqual(12, clasificados.Segundos.Count);
        Assert.AreEqual(8, clasificados.MejoresTerceros.Count);
        Assert.AreEqual(32, clasificados.Todos.Count);
        Assert.AreEqual("A", clasificados.GruposDeOrigen.First().Grupo.Etiqueta);
    }

    [TestMethod]
    public void ObtenerCampeon_DelegaEnServicioClasificacion()
    {
        var servicio = new ServicioClasificacion();
        var controlador = new ControladorClasificacion(servicio);
        var local = CrearEquipo("Finalista1");
        var visitante = CrearEquipo("Finalista2");
        var final = CrearPartido(local, visitante, 2, 1);

        var campeonEsperado = servicio.ObtenerCampeon(final);
        var campeonObtenido = controlador.ObtenerCampeon(final);

        Assert.AreSame(campeonEsperado, campeonObtenido);
        Assert.IsNull(controlador.ObtenerCampeon(null));
    }

    [TestMethod]
    public void ObtenerCampeonDTO_DevuelveEquipoDTO()
    {
        var controlador = new ControladorClasificacion(new ServicioClasificacion());
        var local = CrearEquipo("Finalista1");
        var visitante = CrearEquipo("Finalista2");
        var final = CrearPartido(local, visitante, 2, 1);

        var campeon = controlador.ObtenerCampeonDTO(final);

        Assert.IsNotNull(campeon);
        Assert.IsInstanceOfType(campeon, typeof(EquipoDTO));
        Assert.AreEqual(local.Nombre, campeon.Nombre);
        Assert.IsNull(controlador.ObtenerCampeonDTO(null));
    }

    private Grupo CrearGrupoConResultados(string etiqueta)
    {
        var equipo1 = CrearEquipo($"{etiqueta}1", Confederacion.UEFA);
        var equipo2 = CrearEquipo($"{etiqueta}2", Confederacion.CONMEBOL);
        var equipo3 = CrearEquipo($"{etiqueta}3", Confederacion.AFC);
        var equipo4 = CrearEquipo($"{etiqueta}4", Confederacion.CAF);

        var grupo = new Grupo(etiqueta, new List<Equipo> { equipo1, equipo2, equipo3, equipo4 });
        grupo.Partidos.Add(CrearPartido(equipo1, equipo2, 3, 1));
        grupo.Partidos.Add(CrearPartido(equipo1, equipo3, 2, 2));
        grupo.Partidos.Add(CrearPartido(equipo1, equipo4, 0, 1));
        grupo.Partidos.Add(CrearPartido(equipo2, equipo3, 2, 0));
        grupo.Partidos.Add(CrearPartido(equipo2, equipo4, 1, 1));
        grupo.Partidos.Add(CrearPartido(equipo3, equipo4, 4, 2));

        return grupo;
    }

    private Grupo CrearGrupoConEmpateTotal(string etiqueta)
    {
        var equipo1 = CrearEquipo($"{etiqueta}1");
        var equipo2 = CrearEquipo($"{etiqueta}2");
        var equipo3 = CrearEquipo($"{etiqueta}3");
        var equipo4 = CrearEquipo($"{etiqueta}4");

        var grupo = new Grupo(etiqueta, new List<Equipo> { equipo1, equipo2, equipo3, equipo4 });
        grupo.Partidos.Add(CrearPartido(equipo1, equipo2, 0, 0));
        grupo.Partidos.Add(CrearPartido(equipo1, equipo3, 0, 0));
        grupo.Partidos.Add(CrearPartido(equipo1, equipo4, 0, 0));
        grupo.Partidos.Add(CrearPartido(equipo2, equipo3, 0, 0));
        grupo.Partidos.Add(CrearPartido(equipo2, equipo4, 0, 0));
        grupo.Partidos.Add(CrearPartido(equipo3, equipo4, 0, 0));

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

        var grupo = new Grupo(etiqueta, new List<Equipo> { equipo1, equipo2, equipo3, equipo4 });
        grupo.Partidos.Add(CrearPartido(equipo1, equipo2, 3, 0));
        grupo.Partidos.Add(CrearPartido(equipo1, equipo3, 3, 0));
        grupo.Partidos.Add(CrearPartido(equipo1, equipo4, 3, 0));
        grupo.Partidos.Add(CrearPartido(equipo2, equipo3, 2, 0));
        grupo.Partidos.Add(CrearPartido(equipo2, equipo4, 2, 0));
        grupo.Partidos.Add(CrearPartido(equipo3, equipo4, 1, 0));

        return grupo;
    }

    private Equipo CrearEquipo(string nombre, Confederacion confederacion = Confederacion.UEFA) => new()
    {
        Id = _proximoEquipoId++,
        Nombre = nombre,
        Confederacion = confederacion,
        RankingFIFA = 1200
    };

    private static Partido CrearPartido(Equipo local, Equipo visitante, int golesLocal, int golesVisitante) => new()
    {
        EquipoLocal = local,
        EquipoVisitante = visitante,
        GolesLocal = golesLocal,
        GolesVisitante = golesVisitante,
        Fase = FaseTorneo.FaseDeGrupos,
        Estadio = new Estadio { Nombre = "Estadio", Ciudad = "Ciudad", Capacidad = 30000 },
        Fecha = new DateTime(2026, 6, 1, 14, 0, 0)
    };

    private static void AssertTablasEquivalentes(List<PosicionEquipo> esperada, List<PosicionEquipo> obtenida)
    {
        Assert.AreEqual(esperada.Count, obtenida.Count);

        for (int i = 0; i < esperada.Count; i++)
        {
            Assert.AreSame(esperada[i].Equipo, obtenida[i].Equipo);
            Assert.AreSame(esperada[i].Grupo, obtenida[i].Grupo);
            Assert.AreEqual(esperada[i].Posicion, obtenida[i].Posicion);
            Assert.AreEqual(esperada[i].PartidosJugados, obtenida[i].PartidosJugados);
            Assert.AreEqual(esperada[i].PartidosGanados, obtenida[i].PartidosGanados);
            Assert.AreEqual(esperada[i].PartidosEmpatados, obtenida[i].PartidosEmpatados);
            Assert.AreEqual(esperada[i].PartidosPerdidos, obtenida[i].PartidosPerdidos);
            Assert.AreEqual(esperada[i].GolesAFavor, obtenida[i].GolesAFavor);
            Assert.AreEqual(esperada[i].GolesEnContra, obtenida[i].GolesEnContra);
            Assert.AreEqual(esperada[i].DiferenciaGoles, obtenida[i].DiferenciaGoles);
            Assert.AreEqual(esperada[i].Puntos, obtenida[i].Puntos);
        }
    }
}

