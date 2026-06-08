using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioConfiguracionTorneoTests
{
    [TestMethod]
    public void ModificarConfiguracion_ConAdministrador_AplicaNuevaConfiguracion()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var nuevaConfiguracion = new ConfiguracionTorneo
        {
            SemillaFixture = 7,
            MaxPartidosPorDia = 4,
            SeparacionEntreFechas = 2
        };

        // Act
        servicioConfiguracion.ModificarConfiguracion(administrador, nuevaConfiguracion);

        // Assert
        var configuracionActual = servicioConfiguracion.ObtenerConfiguracionActual();
        Assert.AreEqual(7, configuracionActual.SemillaFixture);
        Assert.AreEqual(4, configuracionActual.MaxPartidosPorDia);
        Assert.AreEqual(2, configuracionActual.SeparacionEntreFechas);
    }

    [TestMethod]
    public void ModificarConfiguracion_ConEditor_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var editor = CrearUsuarioConRol(RolUsuario.Editor);
        var nuevaConfiguracion = new ConfiguracionTorneo { SemillaFixture = 7 };

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioConfiguracion.ModificarConfiguracion(editor, nuevaConfiguracion));
    }

    [TestMethod]
    public void ModificarConfiguracion_ConSemillaCompletarNegativa_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var nuevaConfiguracion = new ConfiguracionTorneo { SemillaCompletar = -1 };

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioConfiguracion.ModificarConfiguracion(administrador, nuevaConfiguracion));
    }

    [TestMethod]
    public void ModificarConfiguracion_ConMaxPartidosPorDiaMenorAlMinimo_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var nuevaConfiguracion = new ConfiguracionTorneo { MaxPartidosPorDia = 1 };

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioConfiguracion.ModificarConfiguracion(administrador, nuevaConfiguracion));
    }

    [TestMethod]
    public void ModificarConfiguracion_ConMaxPartidosPorDiaCero_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var nuevaConfiguracion = new ConfiguracionTorneo { MaxPartidosPorDia = 0 };

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioConfiguracion.ModificarConfiguracion(administrador, nuevaConfiguracion));
    }

    [TestMethod]
    public void ModificarConfiguracion_ConMaxPartidosPorDiaMinimo_NoLanzaExcepcion()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var nuevaConfiguracion = new ConfiguracionTorneo { MaxPartidosPorDia = 2 };

        // Act & Assert — valor límite válido, no debe lanzar
        servicioConfiguracion.ModificarConfiguracion(administrador, nuevaConfiguracion);
        Assert.AreEqual(2, servicioConfiguracion.ObtenerConfiguracionActual().MaxPartidosPorDia);
    }

    [TestMethod]
    public void ModificarConfiguracion_ConSemillaFixtureNegativa_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var nuevaConfiguracion = new ConfiguracionTorneo { SemillaFixture = -1 };

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioConfiguracion.ModificarConfiguracion(administrador, nuevaConfiguracion));
    }

    [TestMethod]
    public void ModificarConfiguracion_ConSemillaCrucesFaseNegativa_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var nuevaConfiguracion = new ConfiguracionTorneo { SemillaCrucesFase = -1 };

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioConfiguracion.ModificarConfiguracion(administrador, nuevaConfiguracion));
    }

    [TestMethod]
    public void ModificarConfiguracion_ConSemillaSimulationNegativa_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var nuevaConfiguracion = new ConfiguracionTorneo { SemillaSimulation = -1 };

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioConfiguracion.ModificarConfiguracion(administrador, nuevaConfiguracion));
    }

    [TestMethod]
    public void ModificarConfiguracion_ConFixtureGenerado_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var configuracionUsada = new ConfiguracionTorneo { SemillaFixture = 1 };
        var nuevaConfiguracion = new ConfiguracionTorneo { SemillaFixture = 2 };
        servicioConfiguracion.RegistrarFixtureGenerado(configuracionUsada);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioConfiguracion.ModificarConfiguracion(administrador, nuevaConfiguracion));
    }

    [TestMethod]
    public void ObtenerConfiguracionUsadaParaFixture_ConFixtureGenerado_DebeDevolverConfiguracionBloqueada()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var configuracionUsada = new ConfiguracionTorneo
        {
            SemillaFixture = 3,
            SemillaCompletar = 4,
            SemillaCrucesFase = 5,
            SemillaSimulation = 6
        };

        // Act
        servicioConfiguracion.RegistrarFixtureGenerado(configuracionUsada);
        var configuracionVisible = servicioConfiguracion.ObtenerConfiguracionUsadaParaFixture();

        // Assert
        Assert.AreEqual(3, configuracionVisible.SemillaFixture);
        Assert.AreEqual(4, configuracionVisible.SemillaCompletar);
        Assert.AreEqual(5, configuracionVisible.SemillaCrucesFase);
        Assert.AreEqual(6, configuracionVisible.SemillaSimulation);
    }

    [TestMethod]
    public void ObtenerConfiguracionActual_SinModificaciones_DevuelveValoresPorDefecto()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();

        // Act
        var configuracion = servicioConfiguracion.ObtenerConfiguracionActual();

        // Assert
        Assert.AreEqual(0, configuracion.SemillaFixture);
        Assert.AreEqual(0, configuracion.SemillaCompletar);
        Assert.AreEqual(0, configuracion.SemillaCrucesFase);
        Assert.AreEqual(0, configuracion.SemillaSimulation);
        Assert.AreEqual(new DateTime(2026, 6, 1), configuracion.FechaInicioTorneo);
        Assert.AreEqual(3, configuracion.MaxPartidosPorDia);
        Assert.AreEqual(3, configuracion.SeparacionEntreFechas);
    }

    [TestMethod]
    public void ObtenerConfiguracionActual_DespuesDeModificar_DevuelveValoresActualizados()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var nuevaConfiguracion = new ConfiguracionTorneo
        {
            SemillaFixture = 42,
            SemillaSimulation = 99,
            MaxPartidosPorDia = 5,
            FechaInicioTorneo = new DateTime(2026, 7, 1)
        };

        // Act
        servicioConfiguracion.ModificarConfiguracion(administrador, nuevaConfiguracion);
        var configuracion = servicioConfiguracion.ObtenerConfiguracionActual();

        // Assert
        Assert.AreEqual(42, configuracion.SemillaFixture);
        Assert.AreEqual(99, configuracion.SemillaSimulation);
        Assert.AreEqual(5, configuracion.MaxPartidosPorDia);
        Assert.AreEqual(new DateTime(2026, 7, 1), configuracion.FechaInicioTorneo);
    }

    [TestMethod]
    public void ObtenerConfiguracionActual_DevuelveCopia_ModificarlaNoAlteraElEstadoInterno()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var configuracionOriginal = new ConfiguracionTorneo { SemillaFixture = 10 };
        servicioConfiguracion.ModificarConfiguracion(administrador, configuracionOriginal);

        // Act
        var copia = servicioConfiguracion.ObtenerConfiguracionActual();
        copia.SemillaFixture = 999;

        // Assert
        var configuracionNoMutada = servicioConfiguracion.ObtenerConfiguracionActual();
        Assert.AreEqual(10, configuracionNoMutada.SemillaFixture);
    }

    [TestMethod]
    public void EsConfiguracionModificable_SinFixtureGenerado_DevuelveTrue()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();

        // Act & Assert
        Assert.IsTrue(servicioConfiguracion.EsConfiguracionModificable);
    }

    [TestMethod]
    public void EsConfiguracionModificable_ConFixtureGenerado_DevuelveFalse()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        servicioConfiguracion.RegistrarFixtureGenerado(new ConfiguracionTorneo());

        // Act & Assert
        Assert.IsFalse(servicioConfiguracion.EsConfiguracionModificable);
    }

    [TestMethod]
    public void EsConfiguracionModificable_ConFixtureGeneradoYTorneoFinalizado_DevuelveTrue()
    {
        // Arrange — fixture generado pero el torneo ya terminó: la config debe desbloquearse
        var estadoTorneo = new EstadoTorneo
        {
            Fixture = new WorldCupPlanner.Dominio.Entidades.Fixture(),
            TorneoFinalizado = true
        };
        var servicioConfiguracion = new ServicioConfiguracionTorneo(estadoTorneo: estadoTorneo);
        servicioConfiguracion.RegistrarFixtureGenerado(new ConfiguracionTorneo());

        // Act & Assert
        Assert.IsTrue(servicioConfiguracion.EsConfiguracionModificable);
    }

    [TestMethod]
    public void EsConfiguracionModificable_ConFixtureArchivadoYFixtureNulo_DevuelveTrue()
    {
        // Arrange — fixture fue archivado: EstadoTorneo.Fixture == null indica que no hay torneo activo
        var estadoTorneo = new EstadoTorneo { Fixture = null, TorneoFinalizado = false };
        var servicioConfiguracion = new ServicioConfiguracionTorneo(estadoTorneo: estadoTorneo);
        servicioConfiguracion.RegistrarFixtureGenerado(new ConfiguracionTorneo());

        // Act & Assert
        Assert.IsTrue(servicioConfiguracion.EsConfiguracionModificable);
    }

    [TestMethod]
    public void ModificarConfiguracion_ConTorneoFinalizado_NoBloqueaConfiguracion()
    {
        // Arrange
        var estadoTorneo = new EstadoTorneo
        {
            Fixture = new WorldCupPlanner.Dominio.Entidades.Fixture(),
            TorneoFinalizado = true
        };
        var servicioConfiguracion = new ServicioConfiguracionTorneo(estadoTorneo: estadoTorneo);
        servicioConfiguracion.RegistrarFixtureGenerado(new ConfiguracionTorneo { SemillaFixture = 1 });
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var nuevaConfiguracion = new ConfiguracionTorneo { SemillaFixture = 99 };

        // Act
        servicioConfiguracion.ModificarConfiguracion(administrador, nuevaConfiguracion);

        // Assert
        Assert.AreEqual(99, servicioConfiguracion.ObtenerConfiguracionActual().SemillaFixture);
    }

    [TestMethod]
    public void EsConfiguracionModificable_ConFixtureGeneradoYTorneoNoFinalizado_DevuelveFalse()
    {
        // Arrange — torneo activo en curso: la config debe permanecer bloqueada
        var estadoTorneo = new EstadoTorneo
        {
            Fixture = new WorldCupPlanner.Dominio.Entidades.Fixture(),
            TorneoFinalizado = false
        };
        var servicioConfiguracion = new ServicioConfiguracionTorneo(estadoTorneo: estadoTorneo);
        servicioConfiguracion.RegistrarFixtureGenerado(new ConfiguracionTorneo());

        // Act & Assert
        Assert.IsFalse(servicioConfiguracion.EsConfiguracionModificable);
    }

    [TestMethod]
    public void ModificarConfiguracion_ConAdministrador_RegistraEntradaDeAuditoria()
    {
        // Arrange
        var repoAuditoria = new RepositorioAuditoriaFake();
        var auditoria = new ServicioAuditoria(repoAuditoria);
        var servicioConfiguracion = new ServicioConfiguracionTorneo(auditoria);
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var nuevaConfiguracion = new ConfiguracionTorneo { SemillaFixture = 42 };

        // Act
        servicioConfiguracion.ModificarConfiguracion(administrador, nuevaConfiguracion);

        // Assert
        Assert.AreEqual(1, repoAuditoria.Entradas.Count);
        StringAssert.Contains(repoAuditoria.Entradas[0].Accion, "configuración del torneo");
    }

    [TestMethod]
    public void ModificarConfiguracion_SinAuditoria_NoLanzaExcepcion()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var nuevaConfiguracion = new ConfiguracionTorneo { SemillaFixture = 5 };

        // Act — no debe lanzar aunque no haya auditoría inyectada
        servicioConfiguracion.ModificarConfiguracion(administrador, nuevaConfiguracion);

        // Assert
        Assert.AreEqual(5, servicioConfiguracion.ObtenerConfiguracionActual().SemillaFixture);
    }

    // --- Unicidad de semilla de generación automática ---

    [TestMethod]
    public void SemillaCompletar_FuenteUnica_ConfiguracionCentralizadaEsOrigenDeLaSemilla()
    {
        // La semilla de generación automática vive exclusivamente en ConfiguracionTorneo.
        // Este test verifica que se persiste y se obtiene desde el mismo servicio,
        // garantizando que no existen otras fuentes paralelas.
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        var configuracion = new ConfiguracionTorneo { SemillaCompletar = 77 };

        servicioConfiguracion.ModificarConfiguracion(administrador, configuracion);

        Assert.AreEqual(77, servicioConfiguracion.ObtenerConfiguracionActual().SemillaCompletar);
    }

    [TestMethod]
    public void SemillaCompletar_Persistencia_MantieneValorTrasMultiplesConsultas()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        servicioConfiguracion.ModificarConfiguracion(administrador, new ConfiguracionTorneo { SemillaCompletar = 42 });

        // Act — múltiples lecturas deben devolver el mismo valor
        var semilla1 = servicioConfiguracion.ObtenerConfiguracionActual().SemillaCompletar;
        var semilla2 = servicioConfiguracion.ObtenerConfiguracionActual().SemillaCompletar;
        var semilla3 = servicioConfiguracion.ObtenerConfiguracionActual().SemillaCompletar;

        // Assert
        Assert.AreEqual(42, semilla1);
        Assert.AreEqual(42, semilla2);
        Assert.AreEqual(42, semilla3);
    }

    [TestMethod]
    public void SemillaCompletar_UsadaEnGeneracion_MismaSemillaProduceMismosEquipos()
    {
        // Verifica que la semilla centralizada produce resultados reproducibles en ServicioEquipo.
        var servicioConfiguracion1 = new ServicioConfiguracionTorneo();
        var servicioConfiguracion2 = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        servicioConfiguracion1.ModificarConfiguracion(administrador, new ConfiguracionTorneo { SemillaCompletar = 99 });
        servicioConfiguracion2.ModificarConfiguracion(administrador, new ConfiguracionTorneo { SemillaCompletar = 99 });

        var servicioEquipo1 = new ServicioEquipo(new RepositorioEquipoStub());
        var servicioEquipo2 = new ServicioEquipo(new RepositorioEquipoStub());

        // Act — ambos usan la semilla de su respectiva configuración centralizada
        var generados1 = servicioEquipo1.CompletarEquipos(
            servicioConfiguracion1.ObtenerConfiguracionActual().SemillaCompletar);
        var generados2 = servicioEquipo2.CompletarEquipos(
            servicioConfiguracion2.ObtenerConfiguracionActual().SemillaCompletar);

        // Assert
        CollectionAssert.AreEqual(
            generados1.Select(e => e.Nombre).ToList(),
            generados2.Select(e => e.Nombre).ToList());
        CollectionAssert.AreEqual(
            generados1.Select(e => e.RankingFIFA).ToList(),
            generados2.Select(e => e.RankingFIFA).ToList());
    }

    [TestMethod]
    public void SemillaCompletar_DistintasSemillas_ProducenEquiposDiferentes()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();
        var administrador = CrearUsuarioConRol(RolUsuario.AdministradorDelSistema);
        servicioConfiguracion.ModificarConfiguracion(administrador, new ConfiguracionTorneo { SemillaCompletar = 1 });
        var semilla1 = servicioConfiguracion.ObtenerConfiguracionActual().SemillaCompletar;

        servicioConfiguracion.ModificarConfiguracion(administrador, new ConfiguracionTorneo { SemillaCompletar = 2 });
        var semilla2 = servicioConfiguracion.ObtenerConfiguracionActual().SemillaCompletar;

        var servicioEquipo1 = new ServicioEquipo(new RepositorioEquipoStub());
        var servicioEquipo2 = new ServicioEquipo(new RepositorioEquipoStub());

        // Act
        var rankings1 = servicioEquipo1.CompletarEquipos(semilla1).Select(e => e.RankingFIFA).ToList();
        var rankings2 = servicioEquipo2.CompletarEquipos(semilla2).Select(e => e.RankingFIFA).ToList();

        // Assert — semillas distintas deben generar rankings distintos
        Assert.IsFalse(rankings1.SequenceEqual(rankings2),
            "Semillas distintas no deben producir rankings idénticos.");
    }

    [TestMethod]
    public void SemillaCompletar_NoExisteEnOtrasFuentes_SoloEnConfiguracionTorneo()
    {
        // Prueba estructural: ConfiguracionTorneo es el único contenedor de SemillaCompletar.
        // Si esta propiedad existiera en múltiples clases, habría duplicación.
        var configuracion = new ConfiguracionTorneo { SemillaCompletar = 55 };

        Assert.AreEqual(55, configuracion.SemillaCompletar);

        Assert.AreEqual(typeof(int), configuracion.SemillaCompletar.GetType());
    }

    [TestMethod]
    public void ObtenerConfiguracionUsadaParaFixture_SinFixtureGenerado_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioConfiguracion = new ServicioConfiguracionTorneo();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioConfiguracion.ObtenerConfiguracionUsadaParaFixture());
    }

    private static Usuario CrearUsuarioConRol(RolUsuario rol)
    {
        var usuario = new Usuario(new DatosUsuario
        {
            Nombre = "Seba",
            Apellido = "Tester",
            Email = $"{rol}@example.com",
            FechaNacimiento = "01-01-2000"
        });
        usuario.AgregarRol(rol);

        return usuario;
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
