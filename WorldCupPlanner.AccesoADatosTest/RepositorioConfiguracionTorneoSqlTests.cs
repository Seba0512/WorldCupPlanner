using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;

namespace WorldCupPlanner.AccesoADatosTest;

[TestClass]
public class RepositorioConfiguracionTorneoSqlTests
{
    [TestMethod]
    public void ObtenerConfiguracionActual_SinDatosEnBd_DevuelveValoresPorDefecto()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(ObtenerConfiguracionActual_SinDatosEnBd_DevuelveValoresPorDefecto));
        var repositorio = new RepositorioConfiguracionTorneoSql(contexto);

        // Act
        var configuracion = repositorio.ObtenerConfiguracionActual();

        // Assert
        var porDefecto = new ConfiguracionTorneo();
        Assert.IsNotNull(configuracion);
        Assert.AreEqual(porDefecto.SemillaFixture, configuracion.SemillaFixture);
        Assert.AreEqual(porDefecto.MaxPartidosPorDia, configuracion.MaxPartidosPorDia);
        Assert.AreEqual(porDefecto.FechaInicioTorneo, configuracion.FechaInicioTorneo);
    }

    [TestMethod]
    public void GuardarConfiguracionActual_PrimeraVez_PersisteLaConfiguracion()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(GuardarConfiguracionActual_PrimeraVez_PersisteLaConfiguracion));
        var repositorio = new RepositorioConfiguracionTorneoSql(contexto);
        var configuracion = new ConfiguracionTorneo { SemillaFixture = 42, MaxPartidosPorDia = 5 };

        // Act
        repositorio.GuardarConfiguracionActual(configuracion);

        // Assert
        using var contextoVerificacion = SqlContextFactory.Crear(nameof(GuardarConfiguracionActual_PrimeraVez_PersisteLaConfiguracion));
        var repositorioVerificacion = new RepositorioConfiguracionTorneoSql(contextoVerificacion);
        var recuperada = repositorioVerificacion.ObtenerConfiguracionActual();
        Assert.AreEqual(42, recuperada.SemillaFixture);
        Assert.AreEqual(5, recuperada.MaxPartidosPorDia);
    }

    [TestMethod]
    public void GuardarConfiguracionActual_Actualizar_SobreescribeValores()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(GuardarConfiguracionActual_Actualizar_SobreescribeValores));
        var repositorio = new RepositorioConfiguracionTorneoSql(contexto);
        repositorio.GuardarConfiguracionActual(new ConfiguracionTorneo { SemillaFixture = 10 });

        // Act
        repositorio.GuardarConfiguracionActual(new ConfiguracionTorneo { SemillaFixture = 99 });

        // Assert
        var recuperada = repositorio.ObtenerConfiguracionActual();
        Assert.AreEqual(99, recuperada.SemillaFixture);
        Assert.AreEqual(1, contexto.ConfiguracionesTorneo.Count());
    }

    [TestMethod]
    public void ObtenerConfiguracionUsadaParaFixture_SinDatos_DevuelveNull()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(ObtenerConfiguracionUsadaParaFixture_SinDatos_DevuelveNull));
        var repositorio = new RepositorioConfiguracionTorneoSql(contexto);

        // Act
        var resultado = repositorio.ObtenerConfiguracionUsadaParaFixture();

        // Assert
        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void GuardarConfiguracionUsadaParaFixture_PersisteSemillaUsada()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(GuardarConfiguracionUsadaParaFixture_PersisteSemillaUsada));
        var repositorio = new RepositorioConfiguracionTorneoSql(contexto);
        var configuracionDeFixture = new ConfiguracionTorneo
        {
            SemillaFixture = 7,
            SemillaCompletar = 3,
            MaxPartidosPorDia = 4
        };

        // Act
        repositorio.GuardarConfiguracionUsadaParaFixture(configuracionDeFixture);

        // Assert
        using var contextoVerificacion = SqlContextFactory.Crear(nameof(GuardarConfiguracionUsadaParaFixture_PersisteSemillaUsada));
        var repositorioVerificacion = new RepositorioConfiguracionTorneoSql(contextoVerificacion);
        var recuperada = repositorioVerificacion.ObtenerConfiguracionUsadaParaFixture();
        Assert.IsNotNull(recuperada);
        Assert.AreEqual(7, recuperada.SemillaFixture);
        Assert.AreEqual(3, recuperada.SemillaCompletar);
        Assert.AreEqual(4, recuperada.MaxPartidosPorDia);
    }

    [TestMethod]
    public void ConfiguracionActualYDeFixture_SonRegistrosSeparados()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(ConfiguracionActualYDeFixture_SonRegistrosSeparados));
        var repositorio = new RepositorioConfiguracionTorneoSql(contexto);

        // Act
        repositorio.GuardarConfiguracionActual(new ConfiguracionTorneo { SemillaFixture = 1 });
        repositorio.GuardarConfiguracionUsadaParaFixture(new ConfiguracionTorneo { SemillaFixture = 2 });

        // Assert
        Assert.AreEqual(2, contexto.ConfiguracionesTorneo.Count());
        Assert.AreEqual(1, repositorio.ObtenerConfiguracionActual().SemillaFixture);
        Assert.AreEqual(2, repositorio.ObtenerConfiguracionUsadaParaFixture()!.SemillaFixture);
    }

    [TestMethod]
    public void GuardarConfiguracionActual_PersisteTodosLosParametros()
    {
        // Arrange
        using var contexto = SqlContextFactory.Crear(nameof(GuardarConfiguracionActual_PersisteTodosLosParametros));
        var repositorio = new RepositorioConfiguracionTorneoSql(contexto);
        var horaInicio = new TimeSpan(15, 30, 0);
        var configuracion = new ConfiguracionTorneo
        {
            SemillaFixture = 11,
            SemillaCompletar = 22,
            SemillaCrucesFase = 33,
            SemillaSimulation = 44,
            FechaInicioTorneo = new DateTime(2026, 7, 1),
            MaxPartidosPorDia = 5,
            SeparacionEntreFechas = 4,
            HoraInicioPartidos = horaInicio,
            SeparacionHorasEntrePartidos = 3
        };

        // Act
        repositorio.GuardarConfiguracionActual(configuracion);

        // Assert
        var recuperada = repositorio.ObtenerConfiguracionActual();
        Assert.AreEqual(11, recuperada.SemillaFixture);
        Assert.AreEqual(22, recuperada.SemillaCompletar);
        Assert.AreEqual(33, recuperada.SemillaCrucesFase);
        Assert.AreEqual(44, recuperada.SemillaSimulation);
        Assert.AreEqual(new DateTime(2026, 7, 1), recuperada.FechaInicioTorneo);
        Assert.AreEqual(5, recuperada.MaxPartidosPorDia);
        Assert.AreEqual(4, recuperada.SeparacionEntreFechas);
        Assert.AreEqual(horaInicio, recuperada.HoraInicioPartidos);
        Assert.AreEqual(3, recuperada.SeparacionHorasEntrePartidos);
    }
}
