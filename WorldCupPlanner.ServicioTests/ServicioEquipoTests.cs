using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

internal class RepositorioAuditoriaFake : IRepositorioConId<Auditoria>
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
        _servicio = new ServicioEquipo(new RepositorioEquipoStub());
    }

    [TestMethod]
    public void Agregar_EquipoValido_SeAgregaCorrectamente()
    {
        // Arrange
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);

        // Act
        _servicio.Agregar(equipo);

        // Assert
        Assert.AreEqual(1, _servicio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Agregar_NombreDuplicado_LanzaExcepcion()
    {
        // Arrange
        _servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));

        // Act & Assert
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() =>
            _servicio.Agregar(new Equipo { Nombre = "Uruguay", RankingFIFA = 800, Confederacion = Confederacion.CONMEBOL }));
    }

    [TestMethod]
    public void Agregar_NombreDuplicadoDistintaMayuscula_LanzaExcepcion()
    {
        // Arrange
        _servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));

        // Act & Assert
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() =>
            _servicio.Agregar(CrearEquipo("URUGUAY", Confederacion.CONMEBOL)));
    }

    [TestMethod]
    public void EditarEquipo_NombreDuplicadoDistintaMayuscula_LanzaExcepcion()
    {
        // Arrange
        _servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));
        _servicio.Agregar(CrearEquipo("Argentina", Confederacion.CONMEBOL));

        // Act & Assert
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() =>
            _servicio.EditarEquipo(CrearEquipo("uruguay", Confederacion.CONMEBOL), "Argentina"));
    }

    [TestMethod]
    public void Agregar_EquipoInvalido_LanzaExcepcion()
    {
        // Arrange
        var equipo = new Equipo { Nombre = "", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL };

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.Agregar(equipo));
    }

    [TestMethod]
    public void Agregar_CupoLlenoOFC_LanzaExcepcion()
    {
        // Arrange
        _servicio.Agregar(CrearEquipo("Nueva Zelanda", Confederacion.OFC));

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            _servicio.Agregar(CrearEquipo("Australia", Confederacion.OFC)));
    }

    [TestMethod]
    public void Agregar_UltimoEquipoDentroDelCupo_SeAgregaCorrectamente()
    {
        // Arrange & Act
        for (var i = 1; i <= 7; i++)
            _servicio.Agregar(CrearEquipo($"Equipo CONMEBOL {i}", Confederacion.CONMEBOL));

        // Assert
        Assert.AreEqual(7, _servicio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Agregar_CupoLlenoCONMEBOL_LanzaExcepcion()
    {
        // Arrange
        for (var i = 1; i <= 7; i++)
            _servicio.Agregar(CrearEquipo($"Equipo CONMEBOL {i}", Confederacion.CONMEBOL));

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            _servicio.Agregar(CrearEquipo("Equipo CONMEBOL 8", Confederacion.CONMEBOL)));
    }

    [TestMethod]
    public void Agregar_CupoLlenoEnUnaConfederacion_NoAfectaOtraConfederacion()
    {
        // Arrange
        _servicio.Agregar(CrearEquipo("Nueva Zelanda", Confederacion.OFC));

        // Act
        _servicio.Agregar(CrearEquipo("España", Confederacion.UEFA));

        // Assert
        Assert.AreEqual(2, _servicio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Agregar_CupoLlenoUEFA_LanzaExcepcion()
    {
        // Arrange
        for (var i = 1; i <= 16; i++)
            _servicio.Agregar(CrearEquipo($"Equipo UEFA {i}", Confederacion.UEFA));

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            _servicio.Agregar(CrearEquipo("Equipo UEFA 17", Confederacion.UEFA)));
    }

    [TestMethod]
    public void EditarEquipo_EquipoInvalido_LanzaExcepcion()
    {
        // Arrange
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        _servicio.Agregar(equipo);
        equipo.Nombre = "";

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.EditarEquipo(equipo, "Uruguay"));
    }

    [TestMethod]
    public void EditarEquipo_EquipoNoExistente_LanzaExcepcion()
    {
        // Arrange
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => _servicio.EditarEquipo(equipo, "Uruguay"));
    }

    [TestMethod]
    public void EditarEquipo_NombreDuplicado_LanzaExcepcion()
    {
        // Arrange
        var equipo1 = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        var equipo2 = CrearEquipo("Argentina", Confederacion.CONMEBOL);
        _servicio.Agregar(equipo1);
        _servicio.Agregar(equipo2);
        equipo2.Nombre = "Uruguay";

        // Act & Assert
        Assert.ThrowsException<EntidadYaExistenteExcepcion>(() => _servicio.EditarEquipo(equipo2, "Argentina"));
    }

    [TestMethod]
    public void Actualizar_EquipoExistente_PersisteCambios()
    {
        // Arrange
        var equipo = CrearEquipo("España", Confederacion.UEFA);
        _servicio.Agregar(equipo);
        equipo.RankingFIFA = 500;

        // Act
        _servicio.Actualizar(equipo);

        // Assert
        Assert.AreEqual(500, _servicio.Buscar(e => e.Nombre == "España").First().RankingFIFA);
    }

    [TestMethod]
    public void Actualizar_EquipoNoExistente_LanzaExcepcion()
    {
        // Arrange
        var equipo = CrearEquipo("España", Confederacion.UEFA);

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() => _servicio.Actualizar(equipo));
    }

    [TestMethod]
    public void Actualizar_EquipoNulo_LanzaExcepcion()
    {
        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() => _servicio.Actualizar(null!));
    }

    [TestMethod]
    public void Eliminar_EquipoExistente_SeEliminaCorrectamente()
    {
        // Arrange
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        _servicio.Agregar(equipo);

        // Act
        _servicio.Eliminar(equipo);

        // Assert
        Assert.AreEqual(0, _servicio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Eliminar_EquipoUsadoEnFixture_DebeLanzarExcepcion()
    {
        // Arrange
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        var fixture = CrearFixtureConEquipo(equipo);
        _servicio.Agregar(equipo);
        _servicio.RegistrarFixtureGenerado(fixture);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            _servicio.Eliminar(equipo));
    }

    [TestMethod]
    public void Eliminar_DespuesDeLimpiarFixtureGenerado_SeEliminaCorrectamente()
    {
        // Arrange
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        var fixture = CrearFixtureConEquipo(equipo);
        _servicio.Agregar(equipo);
        _servicio.RegistrarFixtureGenerado(fixture);

        // Act
        _servicio.LimpiarFixtureGenerado();
        _servicio.Eliminar(equipo);

        // Assert
        Assert.AreEqual(0, _servicio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Eliminar_CuandoFixtureGenerado_EquipoNoEnFixture_DebeLanzarExcepcion()
    {
        // Arrange
        var equipoEnFixture = CrearEquipo("Argentina", Confederacion.CONMEBOL);
        var equipoFueraDelFixture = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        var fixture = CrearFixtureConEquipo(equipoEnFixture);
        _servicio.Agregar(equipoFueraDelFixture);
        _servicio.RegistrarFixtureGenerado(fixture);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            _servicio.Eliminar(equipoFueraDelFixture));
    }

    [TestMethod]
    public void FixtureYaFueGenerado_SinFixtureRegistrado_DevuelveFalse()
    {
        // Act & Assert
        Assert.IsFalse(_servicio.FixtureYaFueGenerado);
    }

    [TestMethod]
    public void FixtureYaFueGenerado_ConFixtureRegistrado_DevuelveTrue()
    {
        // Arrange
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        var fixture = CrearFixtureConEquipo(equipo);

        // Act
        _servicio.RegistrarFixtureGenerado(fixture);

        // Assert
        Assert.IsTrue(_servicio.FixtureYaFueGenerado);
    }

    [TestMethod]
    public void EditarEquipo_CuandoFixtureGenerado_NoLanzaExcepcion()
    {
        // Arrange
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        _servicio.Agregar(equipo);
        var fixture = CrearFixtureConEquipo(equipo);
        _servicio.RegistrarFixtureGenerado(fixture);

        // Act & Assert - editar no debe verse bloqueado por el fixture
        _servicio.EditarEquipo(CrearEquipo("Uruguay", Confederacion.CONMEBOL), "Uruguay");
        Assert.AreEqual(1, _servicio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Buscar_CuandoFixtureGenerado_DevuelveEquiposCorrectamente()
    {
        // Arrange
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        _servicio.Agregar(equipo);
        var fixture = CrearFixtureConEquipo(equipo);
        _servicio.RegistrarFixtureGenerado(fixture);

        // Act
        var resultado = _servicio.Buscar(_ => true);

        // Assert
        Assert.AreEqual(1, resultado.Count);
    }

    [TestMethod]
    public void Buscar_ConPredicadoCoincidente_DevuelveEquiposCorrespondientes()
    {
        // Arrange
        _servicio.Agregar(CrearEquipo("España", Confederacion.UEFA));
        _servicio.Agregar(CrearEquipo("Francia", Confederacion.UEFA));
        _servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));

        // Act
        var resultado = _servicio.Buscar(e => e.Confederacion == Confederacion.UEFA);

        // Assert
        Assert.AreEqual(2, resultado.Count);
    }

    [TestMethod]
    public void Buscar_SinCoincidencias_DevuelveListaVacia()
    {
        // Arrange
        _servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));

        // Act
        var resultado = _servicio.Buscar(e => e.Confederacion == Confederacion.OFC);

        // Assert
        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void CompletarEquipos_RepositorioVacio_Genera48Equipos()
    {
        // Act
        var generados = _servicio.CompletarEquipos(42);

        // Assert
        Assert.AreEqual(48, _servicio.Buscar(_ => true).Count);
        Assert.AreEqual(48, generados.Count);
    }

    [TestMethod]
    public void CompletarEquipos_RepositorioLleno_RetornaListaVacia()
    {
        // Arrange
        _servicio.CompletarEquipos(42);

        // Act
        var generados = _servicio.CompletarEquipos(42);

        // Assert
        Assert.AreEqual(0, generados.Count);
    }

    [TestMethod]
    public void CompletarEquipos_ConEquiposExistentes_ComplementaHasta48()
    {
        // Arrange
        _servicio.Agregar(CrearEquipo("España", Confederacion.UEFA));
        _servicio.Agregar(CrearEquipo("Argentina", Confederacion.CONMEBOL));

        // Act
        _servicio.CompletarEquipos(42);

        // Assert
        Assert.AreEqual(48, _servicio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void CompletarEquipos_MismaSemilla_GeneraMismosEquipos()
    {
        // Arrange
        var servicio2 = new ServicioEquipo(new RepositorioEquipoStub());

        // Act
        var generados1 = _servicio.CompletarEquipos(42);
        var generados2 = servicio2.CompletarEquipos(42);

        // Assert
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
        // Arrange
        var servicio2 = new ServicioEquipo(new RepositorioEquipoStub());

        // Act
        var generados1 = _servicio.CompletarEquipos(1);
        var generados2 = servicio2.CompletarEquipos(2);

        // Assert
        var rankings1 = generados1.Select(e => e.RankingFIFA).ToList();
        var rankings2 = generados2.Select(e => e.RankingFIFA).ToList();

        Assert.IsFalse(rankings1.SequenceEqual(rankings2));
    }

    [TestMethod]
    public void CompletarEquipos_RespetaCuposPorConfederacion()
    {
        // Act
        _servicio.CompletarEquipos(42);

        // Assert
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
        // Act
        var generados = _servicio.CompletarEquipos(42);

        // Assert
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
        // Arrange
        _servicio.Agregar(CrearEquipo("UEFA_01", Confederacion.UEFA));

        // Act
        _servicio.CompletarEquipos(42);

        // Assert
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
        // Act
        var generados = _servicio.CompletarEquipos(42);

        // Assert
        var nombres = generados.Select(e => e.Nombre).ToList();
        var nombresUnicos = nombres.Distinct().ToList();

        Assert.AreEqual(nombres.Count, nombresUnicos.Count);
    }

    [TestMethod]
    public void CompletarEquipos_ConEquipoExistenteEnMinusculas_SaltaAlSiguienteNumero()
    {
        // Arrange — "uefa_01" coincide case-insensitivo con "UEFA_01" que GenerarNombre produciría
        _servicio.Agregar(CrearEquipo("uefa_01", Confederacion.UEFA));

        // Act — no debe lanzar excepción y debe saltear ese nombre
        var generados = _servicio.CompletarEquipos(42);

        // Assert — 16 UEFA en total (1 manual + 15 generados), sin colisión de case
        Assert.AreEqual(16, _servicio.Buscar(e => e.Confederacion == Confederacion.UEFA).Count);
        Assert.IsFalse(generados.Any(e => string.Equals(e.Nombre, "uefa_01", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void CompletarEquipos_TodosLosEquiposGeneradosSonValidos()
    {
        // Act
        var generados = _servicio.CompletarEquipos(42);

        // Assert
        Assert.IsTrue(generados.All(e => e.EsValido()));
    }

    [TestMethod]
    public void CompletarEquipos_RankingFIFA_SiempreDentroDelRangoValido()
    {
        // Arrange
        const int RankingMinimo = 300;
        const int RankingMaximo = 2500;

        // Act
        var generados = _servicio.CompletarEquipos(42);

        // Assert
        foreach (var equipo in generados)
        {
            Assert.IsTrue(
                equipo.RankingFIFA >= RankingMinimo && equipo.RankingFIFA <= RankingMaximo,
                $"RankingFIFA fuera de rango: {equipo.RankingFIFA} para equipo {equipo.Nombre}");
        }
    }

    [TestMethod]
    public void CompletarEquipos_RankingFIFA_NuncaMenorAlMinimo()
    {
        // Arrange & Act
        var generados = _servicio.CompletarEquipos(99);

        // Assert
        Assert.IsTrue(generados.All(e => e.RankingFIFA >= 300),
            "Existen equipos con RankingFIFA menor a 300.");
    }

    [TestMethod]
    public void CompletarEquipos_RankingFIFA_NuncaMayorAlMaximo()
    {
        // Arrange & Act
        var generados = _servicio.CompletarEquipos(77);

        // Assert
        Assert.IsTrue(generados.All(e => e.RankingFIFA <= 2500),
            "Existen equipos con RankingFIFA mayor a 2500.");
    }

    [TestMethod]
    public void CompletarEquipos_RankingFIFA_DistribucionNoConcentradaEnMinimo()
    {
        // Arrange & Act
        var generados = _servicio.CompletarEquipos(42);

        // Assert: el promedio debe estar por encima del 40% del rango (780), lo que indica distribución uniforme
        var promedio = generados.Average(e => e.RankingFIFA);
        Assert.IsTrue(promedio > 780,
            $"Distribución demasiado concentrada en valores bajos. Promedio: {promedio:F0}");
    }

    [TestMethod]
    public void CompletarEquipos_RankingFIFA_ExistenEquiposConRankingAlto()
    {
        // Arrange & Act
        var generados = _servicio.CompletarEquipos(42);

        // Assert: al menos algún equipo debe tener ranking superior a 1500
        Assert.IsTrue(generados.Any(e => e.RankingFIFA > 1500),
            "Ningún equipo generado tiene RankingFIFA mayor a 1500; distribución no es uniforme.");
    }

    [TestMethod]
    public void CompletarEquipos_RankingFIFA_Determinismo_MismaSemillaMismoRanking()
    {
        // Arrange
        var servicio2 = new ServicioEquipo(new RepositorioEquipoStub());

        // Act
        var generados1 = _servicio.CompletarEquipos(123);
        var generados2 = servicio2.CompletarEquipos(123);

        // Assert
        var rankings1 = generados1.Select(e => e.RankingFIFA).ToList();
        var rankings2 = generados2.Select(e => e.RankingFIFA).ToList();
        CollectionAssert.AreEqual(rankings1, rankings2);
    }

    // --- Auditoría ---

    private static (ServicioEquipo servicio, RepositorioAuditoriaFake repoAuditoria) CrearServicioConAuditoria()
    {
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var servicio = new ServicioEquipo(new RepositorioEquipoStub(), auditoria);
        return (servicio, repoAuditoria);
    }

    [TestMethod]
    public void Agregar_EquipoValido_RegistraEntradaDeAuditoria()
    {
        // Arrange
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();

        // Act
        servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));

        // Assert
        Assert.AreEqual(1, repoAuditoria.Entradas.Count);
        Assert.AreEqual("Alta", repoAuditoria.Entradas[0].Accion);
        StringAssert.Contains(repoAuditoria.Entradas[0].Detalle, "Uruguay");
    }

    [TestMethod]
    public void Agregar_EquipoInvalido_NoRegistraAuditoria()
    {
        // Arrange
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicio.Agregar(new Equipo { Nombre = "", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL }));

        Assert.AreEqual(0, repoAuditoria.Entradas.Count);
    }

    [TestMethod]
    public void EditarEquipo_EquipoValido_RegistraEntradaDeAuditoria()
    {
        // Arrange
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));
        repoAuditoria.Entradas.Clear();

        // Act
        servicio.EditarEquipo(CrearEquipo("Uruguay Editado", Confederacion.CONMEBOL), "Uruguay");

        // Assert
        Assert.AreEqual(1, repoAuditoria.Entradas.Count);
        Assert.AreEqual("Edicion", repoAuditoria.Entradas[0].Accion);
        StringAssert.Contains(repoAuditoria.Entradas[0].Detalle, "Uruguay");
        StringAssert.Contains(repoAuditoria.Entradas[0].Detalle, "Uruguay Editado");
    }

    [TestMethod]
    public void EditarEquipo_EquipoInvalido_NoRegistraAuditoria()
    {
        // Arrange
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));
        repoAuditoria.Entradas.Clear();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicio.EditarEquipo(new Equipo { Nombre = "", RankingFIFA = 1000, Confederacion = Confederacion.CONMEBOL }, "Uruguay"));

        Assert.AreEqual(0, repoAuditoria.Entradas.Count);
    }

    [TestMethod]
    public void Eliminar_EquipoExistente_RegistraEntradaDeAuditoria()
    {
        // Arrange
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        servicio.Agregar(equipo);
        repoAuditoria.Entradas.Clear();

        // Act
        servicio.Eliminar(equipo);

        // Assert
        Assert.AreEqual(1, repoAuditoria.Entradas.Count);
        Assert.AreEqual("Baja", repoAuditoria.Entradas[0].Accion);
        StringAssert.Contains(repoAuditoria.Entradas[0].Detalle, "Uruguay");
    }

    [TestMethod]
    public void Agregar_SinAuditoria_NoLanzaExcepcion()
    {
        // Arrange
        var servicio = new ServicioEquipo(new RepositorioEquipoStub());

        // Act
        servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));

        // Assert
        Assert.AreEqual(1, servicio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Auditoria_RegistraUsuarioSistema()
    {
        // Arrange
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();

        // Act
        servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));

        // Assert
        Assert.AreEqual("Sistema", repoAuditoria.Entradas[0].Usuario);
    }

    [TestMethod]
    public void CompletarEquipos_GeneraUnaEntradaAltaPorEquipoGenerado()
    {
        // Arrange
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();

        // Act
        var generados = servicio.CompletarEquipos(42);

        // Assert
        var entradasAlta = repoAuditoria.Entradas.Where(e => e.Accion == "Alta").ToList();
        Assert.AreEqual(generados.Count, entradasAlta.Count);
    }

    private static Equipo CrearEquipo(string nombre, Confederacion confederacion) => new()
    {
        Nombre = nombre,
        RankingFIFA = 1000,
        Confederacion = confederacion
    };

    private static Fixture CrearFixtureConEquipo(Equipo equipo)
    {
        return new Fixture
        {
            Grupos =
            {
                new Grupo("A", new List<Equipo>
                {
                    equipo,
                    CrearEquipo("Argentina", Confederacion.CONMEBOL),
                    CrearEquipo("Francia", Confederacion.UEFA),
                    CrearEquipo("Marruecos", Confederacion.CAF)
                })
            }
        };
    }

    // --- Autorización ---

    [TestMethod]
    public void Agregar_ConSolicitanteAdministrador_AgregaCorrectamente()
    {
        // Arrange
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);

        // Act
        _servicio.Agregar(equipo, solicitante: admin);

        // Assert
        Assert.AreEqual(1, _servicio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void Agregar_ConSolicitanteEditor_LanzaAccesoDenegado()
    {
        // Arrange
        var editor = CrearUsuarioConRol(RolUsuario.Editor);
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.Agregar(equipo, solicitante: editor));
    }

    [TestMethod]
    public void Agregar_ConSolicitanteSinRoles_LanzaAccesoDenegado()
    {
        // Arrange
        var sinRoles = CrearUsuarioSinRoles();
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.Agregar(equipo, solicitante: sinRoles));
    }

    [TestMethod]
    public void EditarEquipo_ConSolicitanteEditor_LanzaAccesoDenegado()
    {
        // Arrange
        var editor = CrearUsuarioConRol(RolUsuario.Editor);
        _servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.EditarEquipo(CrearEquipo("Uruguay", Confederacion.CONMEBOL), "Uruguay", solicitante: editor));
    }

    [TestMethod]
    public void EditarEquipo_ConSolicitanteAdministrador_EditaCorrectamente()
    {
        // Arrange
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        _servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));
        var editado = new Equipo { Nombre = "Uruguay", RankingFIFA = 999, Confederacion = Confederacion.CONMEBOL };

        // Act
        _servicio.EditarEquipo(editado, "Uruguay", solicitante: admin);

        // Assert
        Assert.AreEqual(999, _servicio.Buscar(e => e.Nombre == "Uruguay").First().RankingFIFA);
    }

    [TestMethod]
    public void Eliminar_ConSolicitanteEditor_LanzaAccesoDenegado()
    {
        // Arrange
        var editor = CrearUsuarioConRol(RolUsuario.Editor);
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        _servicio.Agregar(equipo);

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.Eliminar(equipo, solicitante: editor));
    }

    [TestMethod]
    public void Eliminar_ConSolicitanteAdministrador_EliminaCorrectamente()
    {
        // Arrange
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        _servicio.Agregar(equipo);

        // Act
        _servicio.Eliminar(equipo, solicitante: admin);

        // Assert
        Assert.AreEqual(0, _servicio.Buscar(_ => true).Count);
    }

    [TestMethod]
    public void CompletarEquipos_ConSolicitanteEditor_LanzaAccesoDenegado()
    {
        // Arrange
        var editor = CrearUsuarioConRol(RolUsuario.Editor);

        // Act & Assert
        Assert.ThrowsException<AccesoDenegadoExcepcion>(() =>
            _servicio.CompletarEquipos(42, solicitante: editor));
    }

    [TestMethod]
    public void CompletarEquipos_ConSolicitanteAdministrador_CompletaCorrectamente()
    {
        // Arrange
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);

        // Act
        var generados = _servicio.CompletarEquipos(42, solicitante: admin);

        // Assert
        Assert.AreEqual(48, generados.Count);
    }

    [TestMethod]
    public void CompletarEquipos_ConSemillaNegativa_LanzaExcepcion()
    {
        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            _servicio.CompletarEquipos(-1));
    }

    [TestMethod]
    public void EditarEquipo_CambiandoConfederacion_ValidaCupoNuevaConfederacion()
    {
        // Arrange
        _servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));
        var editado = new Equipo { Nombre = "Uruguay", RankingFIFA = 1000, Confederacion = Confederacion.UEFA };

        // Act
        _servicio.EditarEquipo(editado, "Uruguay");

        // Assert
        Assert.AreEqual(Confederacion.UEFA, _servicio.Buscar(e => e.Nombre == "Uruguay").First().Confederacion);
    }

    [TestMethod]
    public void Agregar_ConAuditoriaYSolicitante_RegistraUsuarioEnAuditoria()
    {
        // Arrange
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);

        // Act
        servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL), solicitante: admin);

        // Assert
        Assert.AreEqual("Test User", repoAuditoria.Entradas[0].Usuario);
    }

    [TestMethod]
    public void Eliminar_ConAuditoriaYSolicitante_RegistraUsuarioEnAuditoria()
    {
        // Arrange
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var equipo = CrearEquipo("Uruguay", Confederacion.CONMEBOL);
        servicio.Agregar(equipo);
        repoAuditoria.Entradas.Clear();

        // Act
        servicio.Eliminar(equipo, solicitante: admin);

        // Assert
        Assert.AreEqual("Test User", repoAuditoria.Entradas[0].Usuario);
    }

    [TestMethod]
    public void CompletarEquipos_ConAuditoriaYSolicitante_RegistraUsuarioEnAuditoria()
    {
        // Arrange
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);

        // Act
        servicio.CompletarEquipos(42, solicitante: admin);

        // Assert - la entrada inicial de generación registra al solicitante
        Assert.IsTrue(repoAuditoria.Entradas.Any(e => e.Usuario == "Test User"));
    }

    [TestMethod]
    public void EditarEquipo_ConAuditoriaYSolicitante_RegistraUsuarioEnAuditoria()
    {
        // Arrange
        var (servicio, repoAuditoria) = CrearServicioConAuditoria();
        var admin = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        servicio.Agregar(CrearEquipo("Uruguay", Confederacion.CONMEBOL));
        repoAuditoria.Entradas.Clear();

        // Act
        servicio.EditarEquipo(CrearEquipo("Uruguay Editado", Confederacion.CONMEBOL), "Uruguay", solicitante: admin);

        // Assert
        Assert.AreEqual("Test User", repoAuditoria.Entradas[0].Usuario);
    }

    private static Usuario CrearUsuarioConRol(RolUsuario rol)
    {
        var usuario = CrearUsuarioSinRoles();
        usuario.AgregarRol(rol);
        return usuario;
    }

    private static Usuario CrearUsuarioSinRoles()
    {
        return new Usuario(new DatosUsuario
        {
            Nombre = "Test",
            Apellido = "User",
            Email = "test@example.com",
            FechaNacimiento = "01-01-1990"
        });
    }
}

file sealed class RepositorioEquipoStub : IRepositorio<Equipo>
{
    private readonly List<Equipo> _equipos = new();

    public void Agregar(Equipo equipo) => _equipos.Add(equipo);

    public void Actualizar(Equipo equipo)
    {
        var indice = _equipos.FindIndex(e => ReferenceEquals(e, equipo));
        if (indice == -1)
            throw new EntidadInexistenteExcepcion("El equipo no existe.");
        _equipos[indice] = equipo;
    }

    public void Eliminar(Equipo equipo) => _equipos.Remove(equipo);

    public List<Equipo> Buscar(Func<Equipo, bool> predicado)
        => _equipos.Where(predicado).ToList();
}
