using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

file class RepositorioAuditoriaFake : IRepositorioConId<Auditoria>
{
    private int _nextId = 1;
    public List<Auditoria> Entradas { get; } = [];

    public int SiguienteId() => _nextId++;
    public void Agregar(Auditoria entidad) => Entradas.Add(entidad);
    public void Actualizar(Auditoria entidad) { }
    public void Eliminar(Auditoria entidad) => Entradas.Remove(entidad);
    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado) => Entradas.Where(predicado).ToList();
}

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

    [TestMethod]
    public void CompletarEquipos_RepositorioVacio_Genera48Equipos()
    {
        var generados = _servicio.CompletarEquipos(42);

        Assert.AreEqual(48, _servicio.Buscar(_ => true).Count);
        Assert.AreEqual(48, generados.Count);
    }

    [TestMethod]
    public void CompletarEquipos_RepositorioLleno_RetornaListaVacia()
    {
        _servicio.CompletarEquipos(42);

        var generados = _servicio.CompletarEquipos(42);

        Assert.AreEqual(0, generados.Count);
    }

    [TestMethod]
    public void CompletarEquipos_ConEquiposExistentes_ComplementaHasta48()
    {
        _servicio.Agregar(CrearEquipo("España", Confederacion.UEFA));
        _servicio.Agregar(CrearEquipo("Argentina", Confederacion.CONMEBOL));

        _servicio.CompletarEquipos(42);

        Assert.AreEqual(48, _servicio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void CompletarEquipos_MismaSemilla_GeneraMismosEquipos()
    {
        var servicio2 = new ServicioEquipo();

        var generados1 = _servicio.CompletarEquipos(42);
        var generados2 = servicio2.CompletarEquipos(42);

        var nombres1 = generados1.Select(e => e.Nombre).ToList();
        var nombres2 = generados2.Select(e => e.Nombre).ToList();
        var rankings1 = generados1.Select(e => e.RankingFIFA).ToList();
        var rankings2 = generados2.Select(e => e.RankingFIFA).ToList();

        CollectionAssert.AreEqual(nombres1, nombres2);
        CollectionAssert.AreEqual(rankings1, rankings2);
    }

    [TestMethod]
    public void CompletarEquipos_DistintaSemilla_GeneraDistintosRankings()
    {
        var servicio2 = new ServicioEquipo();

        var generados1 = _servicio.CompletarEquipos(1);
        var generados2 = servicio2.CompletarEquipos(2);

        var rankings1 = generados1.Select(e => e.RankingFIFA).ToList();
        var rankings2 = generados2.Select(e => e.RankingFIFA).ToList();

        Assert.IsFalse(rankings1.SequenceEqual(rankings2));
    }

    [TestMethod]
    public void CompletarEquipos_RespetaCuposPorConfederacion()
    {
        _servicio.CompletarEquipos(42);

        Assert.AreEqual(16, _servicio.Buscar(e => e.Confederacion == Confederacion.UEFA).Count);
        Assert.AreEqual(7,  _servicio.Buscar(e => e.Confederacion == Confederacion.CONMEBOL).Count);
        Assert.AreEqual(7,  _servicio.Buscar(e => e.Confederacion == Confederacion.CONCACAF).Count);
        Assert.AreEqual(9,  _servicio.Buscar(e => e.Confederacion == Confederacion.CAF).Count);
        Assert.AreEqual(8,  _servicio.Buscar(e => e.Confederacion == Confederacion.AFC).Count);
        Assert.AreEqual(1,  _servicio.Buscar(e => e.Confederacion == Confederacion.OFC).Count);
    }

    [TestMethod]
    public void CompletarEquipos_NombresEnFormatoCorrecto()
    {
        var generados = _servicio.CompletarEquipos(42);

        foreach (var equipo in generados)
        {
            var partes = equipo.Nombre.Split('_');
            Assert.AreEqual(2, partes.Length, $"Formato inválido: {equipo.Nombre}");
            Assert.IsTrue(int.TryParse(partes[1], out var numero), $"Número no válido en: {equipo.Nombre}");
            Assert.IsTrue(numero >= 1, $"Número menor a 1 en: {equipo.Nombre}");
        }
    }

    [TestMethod]
    public void CompletarEquipos_ContinuaNumeracionExistente()
    {
        _servicio.Agregar(CrearEquipo("UEFA_01", Confederacion.UEFA));

        _servicio.CompletarEquipos(42);

        var nombresUefa = _servicio.Buscar(e => e.Confederacion == Confederacion.UEFA)
            .Select(e => e.Nombre)
            .ToList();

        Assert.AreEqual(16, nombresUefa.Count);
        Assert.IsTrue(nombresUefa.Contains("UEFA_01"));
        Assert.IsTrue(nombresUefa.Contains("UEFA_02"));
    }

    [TestMethod]
    public void CompletarEquipos_EvitaNombresDuplicados()
    {
        var generados = _servicio.CompletarEquipos(42);

        var nombres = generados.Select(e => e.Nombre).ToList();
        var nombresUnicos = nombres.Distinct().ToList();

        Assert.AreEqual(nombres.Count, nombresUnicos.Count);
    }

    [TestMethod]
    public void CompletarEquipos_TodosLosEquiposGeneradosSonValidos()
    {
        var generados = _servicio.CompletarEquipos(42);

        Assert.IsTrue(generados.All(e => e.EsValido()));
    }

    // --- Auditoría ---

    private static (ServicioEquipo servicio, RepositorioAuditoriaFake repoAuditoria) CrearServicioConAuditoria()
    {
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var servicio = new ServicioEquipo(new RepositorioEquipoEnMemoria(), auditoria);
        return (servicio, repoAuditoria);
    }

    [TestMethod]
    public void Agregar_EquipoValido_RegistraEntradaDeAuditoria()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();

        servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));

        Assert.AreEqual(1, repoAuditoria.Entradas.Count);
        Assert.AreEqual("Alta", repoAuditoria.Entradas[0].Accion);
        StringAssert.Contains(repoAuditoria.Entradas[0].Detalle, "Uruguay");
    }

    [TestMethod]
    public void Agregar_EquipoInvalido_NoRegistraAuditoria()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicio.Agregar(new Equipo { Nombre = "", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL }));

        Assert.AreEqual(0, repoAuditoria.Entradas.Count);
    }

    [TestMethod]
    public void EditarEquipo_EquipoValido_RegistraEntradaDeAuditoria()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));
        repoAuditoria.Entradas.Clear();

        servicio.EditarEquipo(CrearEquipo("Uruguay Editado", Confederacion.CONMEBOL), "Uruguay");

        Assert.AreEqual(1, repoAuditoria.Entradas.Count);
        Assert.AreEqual("Edicion", repoAuditoria.Entradas[0].Accion);
        StringAssert.Contains(repoAuditoria.Entradas[0].Detalle, "Uruguay");
        StringAssert.Contains(repoAuditoria.Entradas[0].Detalle, "Uruguay Editado");
    }

    [TestMethod]
    public void EditarEquipo_EquipoInvalido_NoRegistraAuditoria()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));
        repoAuditoria.Entradas.Clear();

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicio.EditarEquipo(new Equipo { Nombre = "", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL }, "Uruguay"));

        Assert.AreEqual(0, repoAuditoria.Entradas.Count);
    }

    [TestMethod]
    public void Eliminar_EquipoExistente_RegistraEntradaDeAuditoria()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        servicio.Agregar(equipo);
        repoAuditoria.Entradas.Clear();

        servicio.Eliminar(equipo);

        Assert.AreEqual(1, repoAuditoria.Entradas.Count);
        Assert.AreEqual("Baja", repoAuditoria.Entradas[0].Accion);
        StringAssert.Contains(repoAuditoria.Entradas[0].Detalle, "Uruguay");
    }

    [TestMethod]
    public void Agregar_SinAuditoria_NoLanzaExcepcion()
    {
        var servicio = new ServicioEquipo(new RepositorioEquipoEnMemoria());

        servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));

        Assert.AreEqual(1, servicio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Auditoria_RegistraUsuarioSistema()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();

        servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));

        Assert.AreEqual("Sistema", repoAuditoria.Entradas[0].Usuario);
    }

    [TestMethod]
    public void CompletarEquipos_GeneraUnaEntradaPorEquipoGenerado()
    {
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();

        var generados = servicio.CompletarEquipos(42);

        Assert.AreEqual(generados.Count, repoAuditoria.Entradas.Count);
        Assert.IsTrue(repoAuditoria.Entradas.All(e => e.Accion == "Alta"));
    }

    private static Equipo CrearEquipo(string nombre, Confederacion confederacion) => new()
    {
        Nombre = nombre,
        RankingFIFA = 1000,
        Confederacion = confederacion
    };
}
