using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioEquipoTests
{
    private ServicioEquipo _servicio;

    [TestInitialize]
    public void Setup()
    {
        _servicio = new ServicioEquipo();
    }

    [TestMethod]
    public void Agregar_EquipoValido_SeAgregaCorrectamente()
    {
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);

        _servicio.Agregar(equipo);

        Assert.AreEqual(1, _servicio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Agregar_NombreDuplicado_LanzaExcepcion()
    {
        _servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));

        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() =>
            _servicio.Agregar(new Equipo { Nombre = "Uruguay", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL }));
    }

    [TestMethod]
    public void Agregar_EquipoInvalido_LanzaExcepcion()
    {
        var equipo = new Equipo { Nombre = "", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL };

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.Agregar(equipo));
    }

    [TestMethod]
    public void Agregar_CupoLlenoOFC_LanzaExcepcion()
    {
        _servicio.Agregar(CrearEquipo("Nueva Zelanda", Confederacion.OFC));

        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.Agregar(CrearEquipo("Australia", Confederacion.OFC)));
    }

    [TestMethod]
    public void Agregar_UltimoEquipoDentroDelCupo_SeAgregaCorrectamente()
    {
        for (var i = 1; i <= 7; i++)
            _servicio.Agregar(CrearEquipo($"Equipo CONMEBOL {i}", Confederacion.CONMEBOL));

        Assert.AreEqual(7, _servicio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Agregar_CupoLlenoCONMEBOL_LanzaExcepcion()
    {
        for (var i = 1; i <= 7; i++)
            _servicio.Agregar(CrearEquipo($"Equipo CONMEBOL {i}", Confederacion.CONMEBOL));

        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.Agregar(CrearEquipo("Equipo CONMEBOL 8", Confederacion.CONMEBOL)));
    }

    [TestMethod]
    public void Agregar_CupoLlenoEnUnaConfederacion_NoAfectaOtraConfederacion()
    {
        _servicio.Agregar(CrearEquipo("Nueva Zelanda", Confederacion.OFC));

        _servicio.Agregar(CrearEquipo("España", Confederacion.UEFA));

        Assert.AreEqual(2, _servicio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Agregar_CupoLlenoUEFA_LanzaExcepcion()
    {
        for (var i = 1; i <= 16; i++)
            _servicio.Agregar(CrearEquipo($"Equipo UEFA {i}", Confederacion.UEFA));

        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.Agregar(CrearEquipo("Equipo UEFA 17", Confederacion.UEFA)));
    }

    [TestMethod]
    public void EditarEquipo_EquipoInvalido_LanzaExcepcion()
    {
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        _servicio.Agregar(equipo);
        equipo.Nombre = "";

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.EditarEquipo(equipo, "Uruguay"));
    }

    [TestMethod]
    public void EditarEquipo_NombreDuplicado_LanzaExcepcion()
    {
        var equipo1 = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        var equipo2 = CrearEquipo("Argentina", Confederacion.CONMEBOL);
        _servicio.Agregar(equipo1);
        _servicio.Agregar(equipo2);
        equipo2.Nombre = "Uruguay";

        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() => _servicio.EditarEquipo(equipo2, "Argentina"));
    }

    [TestMethod]
    public void Actualizar_EquipoExistente_PersisteCambios()
    {
        var equipo = CrearEquipo("España", Confederacion.UEFA);
        _servicio.Agregar(equipo);
        equipo.RankingFIFA = 500;

        _servicio.Actualizar(equipo);

        Assert.AreEqual(500, _servicio.Buscar(e => e.Nombre == "España").First().RankingFIFA);
    }

    [TestMethod]
    public void Actualizar_EquipoNoExistente_LanzaExcepcion()
    {
        var equipo = CrearEquipo("España", Confederacion.UEFA);

        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => _servicio.Actualizar(equipo));
    }

    [TestMethod]
    public void Actualizar_EquipoNulo_LanzaExcepcion()
    {
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.Actualizar(null!));
    }

    [TestMethod]
    public void Eliminar_EquipoExistente_SeEliminaCorrectamente()
    {
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        _servicio.Agregar(equipo);

        _servicio.Eliminar(equipo);

        Assert.AreEqual(0, _servicio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Buscar_ConPredicadoCoincidente_DevuelveEquiposCorrespondientes()
    {
        _servicio.Agregar(CrearEquipo("España", Confederacion.UEFA));
        _servicio.Agregar(CrearEquipo("Francia", Confederacion.UEFA));
        _servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));

        var resultado = _servicio.Buscar(e => e.Confederacion == Confederacion.UEFA);

        Assert.AreEqual(2, resultado.Count);
    }

    [TestMethod]
    public void Buscar_SinCoincidencias_DevuelveListaVacia()
    {
        _servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));

        var resultado = _servicio.Buscar(e => e.Confederacion == Confederacion.OFC);

        Assert.AreEqual(0, resultado.Count);
    }

    private static Equipo CrearEquipo(string nombre, Confederacion confederacion) => new()
    {
        Nombre = nombre,
        RankingFIFA = 1000,
        Confederacion = confederacion
    };
}
