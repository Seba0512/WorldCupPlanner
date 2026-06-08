using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

// --- Notificaciones al cargar resultado ---

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioPartidoTests
{
    [TestMethod]
    public void EditarFecha_ConPartidoExistente_ActualizaFecha()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);
        var nuevaFecha = new DateTime(2026, 6, 15, 18, 0, 0);

        // Act
        servicioPartido.EditarFecha(partido.Id, nuevaFecha);

        // Assert
        Assert.AreEqual(nuevaFecha, partido.Fecha);
    }

    [TestMethod]
    public void EditarEstadio_ConPartidoExistente_ActualizaEstadio()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);
        var nuevoEstadio = CrearEstadio("Estadio Nuevo");

        // Act
        servicioPartido.EditarEstadio(partido.Id, nuevoEstadio);

        // Assert
        Assert.AreEqual(nuevoEstadio, partido.Estadio);
    }

    [TestMethod]
    public void CargarResultado_ConPartidoExistente_ActualizaGoles()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);

        // Act
        servicioPartido.CargarResultado(partido.Id, 2, 1);

        // Assert
        Assert.AreEqual(2, partido.GolesLocal);
        Assert.AreEqual(1, partido.GolesVisitante);
    }

    [TestMethod]
    public void EditarFecha_ConPartidoInexistente_DebeLanzarExcepcion()
    {
        // Arrange
        var servicioPartido = new ServicioPartido(new RepositorioPartidoStub());

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() =>
            servicioPartido.EditarFecha(99, new DateTime(2026, 6, 15)));
    }

    [TestMethod]
    public void EditarEstadio_ConEstadioInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);
        var estadioInvalido = CrearEstadio("");

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioPartido.EditarEstadio(partido.Id, estadioInvalido));
    }

    [TestMethod]
    public void EditarEstadio_ConEstadioNull_LanzaEntidadInvalidaExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioPartido.EditarEstadio(partido.Id, null!));
    }

    [TestMethod]
    public void CargarResultado_ConGolesNegativos_DebeLanzarExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioPartido.CargarResultado(partido.Id, -1, 0));
    }

    [TestMethod]
    public void EditarPartidoDeFaseDeGrupos_ConCrucesGenerados_DebeLanzarExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);
        servicioPartido.BloquearEdicionPartidosDeFaseDeGrupos();

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioPartido.CargarResultado(partido.Id, 2, 1));
    }

    [TestMethod]
    public void EditarPartidoDePrimeraRonda_ConOctavosGenerados_DebeLanzarExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido(FaseTorneo.PrimeraRondaEliminatoria);
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);
        servicioPartido.RegistrarFaseGenerada(FaseTorneo.Octavos);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioPartido.CargarResultado(partido.Id, 2, 1));
    }

    [TestMethod]
    public void EditarPartidoDeOctavos_ConCuartosGenerados_DebeLanzarExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido(FaseTorneo.Octavos);
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);
        servicioPartido.RegistrarFaseGenerada(FaseTorneo.Cuartos);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioPartido.EditarFecha(partido.Id, new DateTime(2026, 7, 1)));
    }

    [TestMethod]
    public void EditarPartidoDeCuartos_ConSemifinalesGeneradas_DebeLanzarExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido(FaseTorneo.Cuartos);
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);
        servicioPartido.RegistrarFaseGenerada(FaseTorneo.Semifinal);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioPartido.EditarEstadio(partido.Id, CrearEstadio("Estadio Nuevo")));
    }

    [TestMethod]
    public void EditarPartidoDeSemifinal_ConFinalGenerada_DebeLanzarExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido(FaseTorneo.Semifinal);
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);
        servicioPartido.RegistrarFaseGenerada(FaseTorneo.Final);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioPartido.CargarResultado(partido.Id, 2, 1));
    }

    [TestMethod]
    public void EsFaseBloqueada_SinFasesBloqueadas_RetornaFalso()
    {
        // Arrange
        var servicioPartido = new ServicioPartido(new RepositorioPartidoStub());

        // Act & Assert
        Assert.IsFalse(servicioPartido.EsFaseBloqueada(FaseTorneo.FaseDeGrupos));
    }

    [TestMethod]
    public void EsFaseBloqueada_ConFaseDeGruposBloqueadaExplicitamente_RetornaVerdadero()
    {
        // Arrange
        var servicioPartido = new ServicioPartido(new RepositorioPartidoStub());
        servicioPartido.BloquearEdicionPartidosDeFaseDeGrupos();

        // Act & Assert
        Assert.IsTrue(servicioPartido.EsFaseBloqueada(FaseTorneo.FaseDeGrupos));
    }

    [TestMethod]
    public void BuscarPartidoPorId_ConIdInexistente_RetornaNull()
    {
        // Arrange
        var servicioPartido = new ServicioPartido(new RepositorioPartidoStub());

        // Act
        var resultado = servicioPartido.BuscarPartidoPorId(99);

        // Assert
        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void BuscarPartidoPorId_ConPartidoExistente_RetornaPartido()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);

        // Act
        var resultado = servicioPartido.BuscarPartidoPorId(partido.Id);

        // Assert
        Assert.AreEqual(partido, resultado);
    }

    [TestMethod]
    public void CargarResultado_CuandoResultadoValido_GeneraNotificacionesParaPeriodistas()
    {
        // Arrange
        var repositorioPartido = new RepositorioPartidoStub();
        var repositorioNotificacion = new RepositorioNotificacionStub();
        var servicioNotificacion = new ServicioNotificacion(repositorioNotificacion);
        var periodista = new Usuario(new DatosUsuario { Nombre = "Sebita", Apellido = "Luciardi", Email = "periodista@test.com", FechaNacimiento = "01-01-1990" });
        periodista.AgregarRol(RolUsuario.Periodista);
        var periodistas = new List<Usuario> { periodista };
        var partido = CrearPartido();
        repositorioPartido.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorioPartido, servicioNotificacion, periodistas);

        // Act
        servicioPartido.CargarResultado(partido.Id, 2, 1);

        // Assert
        Assert.AreEqual(1, repositorioNotificacion.ObtenerPorUsuario(periodista.Id).Count);
    }

    private static Partido CrearPartido()
    {
        return CrearPartido(FaseTorneo.FaseDeGrupos);
    }

    private static Partido CrearPartido(FaseTorneo fase)
    {
        return new Partido
        {
            Id = 1,
            Fecha = new DateTime(2026, 6, 1, 14, 0, 0),
            Estadio = CrearEstadio("Estadio Inicial"),
            EquipoLocal = CrearEquipo("Equipo Local"),
            EquipoVisitante = CrearEquipo("Equipo Visitante"),
            Fase = fase
        };
    }

    private static Equipo CrearEquipo(string nombre)
    {
        return new Equipo
        {
            Nombre = nombre,
            RankingFIFA = 1000,
            Confederacion = Confederacion.UEFA
        };
    }

    private static Estadio CrearEstadio(string nombre)
    {
        return new Estadio
        {
            Nombre = nombre,
            Ciudad = "Ciudad",
            Capacidad = 30000
        };
    }

    [TestMethod]
    public void CargarResultado_SiempreResetea_GanadorPorPenales()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido(FaseTorneo.Octavos);
        partido.GanadorPorPenales = partido.EquipoLocal;
        repositorio.Agregar(partido);
        var servicio = new ServicioPartido(repositorio);

        // Act — cargamos resultado no empate, GanadorPorPenales debe quedar null
        servicio.CargarResultado(partido.Id, 2, 1);

        // Assert
        Assert.IsNull(partido.GanadorPorPenales);
    }

    [TestMethod]
    public void ResolverEmpateConPenales_EnFaseEliminatoriaConEmpate_AsignaGanadorPorPenales()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido(FaseTorneo.Octavos);
        partido.GolesLocal = 1;
        partido.GolesVisitante = 1;
        repositorio.Agregar(partido);
        var servicio = new ServicioPartido(repositorio);

        // Act
        servicio.ResolverEmpateConPenales(partido.Id, semilla: 42);

        // Assert
        Assert.IsNotNull(partido.GanadorPorPenales);
        Assert.IsTrue(
            ReferenceEquals(partido.GanadorPorPenales, partido.EquipoLocal) ||
            ReferenceEquals(partido.GanadorPorPenales, partido.EquipoVisitante));
    }

    [TestMethod]
    public void ResolverEmpateConPenales_EnFaseDeGruposConEmpate_NoAsignaGanadorPorPenales()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido(FaseTorneo.FaseDeGrupos);
        partido.GolesLocal = 1;
        partido.GolesVisitante = 1;
        repositorio.Agregar(partido);
        var servicio = new ServicioPartido(repositorio);

        // Act
        servicio.ResolverEmpateConPenales(partido.Id, semilla: 42);

        // Assert
        Assert.IsNull(partido.GanadorPorPenales);
    }

    [TestMethod]
    public void ResolverEmpateConPenales_CuandoNoEsEmpate_NoAsignaGanadorPorPenales()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido(FaseTorneo.Cuartos);
        partido.GolesLocal = 2;
        partido.GolesVisitante = 1;
        repositorio.Agregar(partido);
        var servicio = new ServicioPartido(repositorio);

        // Act
        servicio.ResolverEmpateConPenales(partido.Id, semilla: 42);

        // Assert
        Assert.IsNull(partido.GanadorPorPenales);
    }

    [TestMethod]
    public void ResolverEmpateConPenales_ConMismaSemilla_EsDeterministico()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido(FaseTorneo.Semifinal);
        partido.GolesLocal = 0;
        partido.GolesVisitante = 0;
        repositorio.Agregar(partido);
        var servicio = new ServicioPartido(repositorio);

        // Act
        servicio.ResolverEmpateConPenales(partido.Id, semilla: 7);
        var ganador1 = partido.GanadorPorPenales;
        partido.GanadorPorPenales = null;
        servicio.ResolverEmpateConPenales(partido.Id, semilla: 7);
        var ganador2 = partido.GanadorPorPenales;

        // Assert
        Assert.AreEqual(ganador1, ganador2);
    }

    // --- Constructor con EstadoTorneo ---

    [TestMethod]
    public void BuscarPartidoPorId_ViaEstadoTorneo_EnFixture_RetornaPartido()
    {
        // Arrange
        var partido = CrearPartido();
        var grupo = new Grupo("A");
        grupo.Partidos.Add(partido);
        var fixture = new Fixture();
        fixture.Grupos.Add(grupo);
        var estadoTorneo = new EstadoTorneo { Fixture = fixture };
        var servicio = new ServicioPartido(estadoTorneo);

        // Act
        var resultado = servicio.BuscarPartidoPorId(partido.Id);

        // Assert
        Assert.AreEqual(partido, resultado);
    }

    [TestMethod]
    public void BuscarPartidoPorId_ViaEstadoTorneo_EnEliminatorios_RetornaPartido()
    {
        // Arrange
        var partido = CrearPartido(FaseTorneo.PrimeraRondaEliminatoria);
        var estadoTorneo = new EstadoTorneo();
        estadoTorneo.PartidosEliminatorios.Add(partido);
        var servicio = new ServicioPartido(estadoTorneo);

        // Act
        var resultado = servicio.BuscarPartidoPorId(partido.Id);

        // Assert
        Assert.AreEqual(partido, resultado);
    }

    [TestMethod]
    public void BuscarPartidoPorId_ViaEstadoTorneo_ConFixtureSinCoincidenciaYEliminatorioExistente_RetornaEliminatorio()
    {
        // Arrange
        var partidoFixture = CrearPartido(FaseTorneo.FaseDeGrupos);
        var grupo = new Grupo("A");
        grupo.Partidos.Add(partidoFixture);
        var fixture = new Fixture();
        fixture.Grupos.Add(grupo);

        var partidoEliminatorio = CrearPartido(FaseTorneo.Octavos);
        partidoEliminatorio.Id = 2;

        var estadoTorneo = new EstadoTorneo { Fixture = fixture };
        estadoTorneo.PartidosEliminatorios.Add(partidoEliminatorio);
        var servicio = new ServicioPartido(estadoTorneo);

        // Act
        var resultado = servicio.BuscarPartidoPorId(partidoEliminatorio.Id);

        // Assert
        Assert.AreEqual(partidoEliminatorio, resultado);
    }

    [TestMethod]
    public void BuscarPartidoPorId_ConEstadoTorneoNulo_RetornaNull()
    {
        // Arrange
        var servicio = new ServicioPartido((EstadoTorneo)null!);

        // Act
        var resultado = servicio.BuscarPartidoPorId(99);

        // Assert
        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void BuscarPartidoPorId_ViaEstadoTorneo_SinPartido_RetornaNull()
    {
        // Arrange
        var estadoTorneo = new EstadoTorneo();
        var servicio = new ServicioPartido(estadoTorneo);

        // Act
        var resultado = servicio.BuscarPartidoPorId(99);

        // Assert
        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void ObtenerPartidos_ConRepositorio_DevuelvePartidos()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicio = new ServicioPartido(repositorio);

        // Act
        var partidos = servicio.ObtenerPartidos();

        // Assert
        Assert.AreEqual(1, partidos.Count);
        Assert.AreEqual(partido, partidos[0]);
    }

    [TestMethod]
    public void ObtenerPartidos_ViaEstadoTorneo_DevuelveFixtureYEliminatorios()
    {
        // Arrange
        var partidoGrupo = CrearPartido(FaseTorneo.FaseDeGrupos);
        var partidoElim = CrearPartido(FaseTorneo.PrimeraRondaEliminatoria);
        partidoElim.Id = 2;
        var grupo = new Grupo("A");
        grupo.Partidos.Add(partidoGrupo);
        var fixture = new Fixture();
        fixture.Grupos.Add(grupo);
        var estadoTorneo = new EstadoTorneo { Fixture = fixture };
        estadoTorneo.PartidosEliminatorios.Add(partidoElim);
        var servicio = new ServicioPartido(estadoTorneo);

        // Act
        var partidos = servicio.ObtenerPartidos();

        // Assert
        Assert.AreEqual(2, partidos.Count);
    }

    [TestMethod]
    public void ObtenerPartidos_ViaEstadoTorneo_SinFixtureNiEliminatorios_DevuelveListaVacia()
    {
        // Arrange
        var estadoTorneo = new EstadoTorneo();
        var servicio = new ServicioPartido(estadoTorneo);

        // Act
        var partidos = servicio.ObtenerPartidos();

        // Assert
        Assert.AreEqual(0, partidos.Count);
    }

    [TestMethod]
    public void ObtenerPartidos_ConEstadoTorneoNulo_DevuelveListaVacia()
    {
        // Arrange
        var servicio = new ServicioPartido((EstadoTorneo)null!);

        // Act
        var partidos = servicio.ObtenerPartidos();

        // Assert
        Assert.AreEqual(0, partidos.Count);
    }

    [TestMethod]
    public void EsFaseBloqueada_ViaEstadoTorneo_ConEliminatorios_FaseDeGruposBloqueada()
    {
        // Arrange
        var estadoTorneo = new EstadoTorneo();
        estadoTorneo.PartidosEliminatorios.Add(CrearPartido(FaseTorneo.PrimeraRondaEliminatoria));
        var servicio = new ServicioPartido(estadoTorneo);

        // Act & Assert
        Assert.IsTrue(servicio.EsFaseBloqueada(FaseTorneo.FaseDeGrupos));
    }

    [TestMethod]
    public void EsFaseBloqueada_ViaEstadoTorneo_SinEliminatorios_FaseDeGruposNoEsBloqueada()
    {
        // Arrange
        var estadoTorneo = new EstadoTorneo();
        var servicio = new ServicioPartido(estadoTorneo);

        // Act & Assert
        Assert.IsFalse(servicio.EsFaseBloqueada(FaseTorneo.FaseDeGrupos));
    }

    [TestMethod]
    public void EditarFecha_ConFechaPorDefecto_DebeLanzarExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicio = new ServicioPartido(repositorio);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicio.EditarFecha(partido.Id, default));
    }

    [TestMethod]
    public void CargarResultado_ConGolesVisitanteNegativos_DebeLanzarExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicio = new ServicioPartido(repositorio);

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicio.CargarResultado(partido.Id, 0, -1));
    }

    [TestMethod]
    public void RegistrarFaseGenerada_TercerPuesto_BloqueaSemifinal()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido(FaseTorneo.Semifinal);
        repositorio.Agregar(partido);
        var servicio = new ServicioPartido(repositorio);

        // Act
        servicio.RegistrarFaseGenerada(FaseTorneo.TercerPuesto);

        // Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicio.CargarResultado(partido.Id, 1, 0));
    }

    [TestMethod]
    public void RegistrarFaseGenerada_FaseInvalida_DebeLanzarExcepcion()
    {
        // Arrange
        var servicio = new ServicioPartido(new RepositorioPartidoStub());

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicio.RegistrarFaseGenerada(FaseTorneo.FaseDeGrupos));
    }

    [TestMethod]
    public void BloquearEdicionFase_Directamente_BloqueaFaseIndicada()
    {
        // Arrange
        var repositorio = new RepositorioPartidoStub();
        var partido = CrearPartido(FaseTorneo.Octavos);
        repositorio.Agregar(partido);
        var servicio = new ServicioPartido(repositorio);

        // Act
        servicio.BloquearEdicionFase(FaseTorneo.Octavos);

        // Assert
        Assert.IsTrue(servicio.EsFaseBloqueada(FaseTorneo.Octavos));
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicio.EditarFecha(partido.Id, new DateTime(2026, 9, 1)));
    }

    [TestMethod]
    public void EsFaseBloqueada_FaseDistintaAGrupos_SinBloqueoExplicito_RetornaFalso()
    {
        // Arrange
        var estadoTorneo = new EstadoTorneo();
        estadoTorneo.PartidosEliminatorios.Add(CrearPartido(FaseTorneo.PrimeraRondaEliminatoria));
        var servicio = new ServicioPartido(estadoTorneo);

        // EsFaseBloqueada para Octavos sin fase bloqueada explícita → debe ser false
        Assert.IsFalse(servicio.EsFaseBloqueada(FaseTorneo.Octavos));
    }

    [TestMethod]
    public void EditarFecha_ViaEstadoTorneo_ActualizaFecha()
    {
        // Arrange
        var partido = CrearPartido();
        var grupo = new Grupo("A");
        grupo.Partidos.Add(partido);
        var fixture = new Fixture();
        fixture.Grupos.Add(grupo);
        var estadoTorneo = new EstadoTorneo { Fixture = fixture };
        var servicio = new ServicioPartido(estadoTorneo);
        var nuevaFecha = new DateTime(2026, 8, 1, 12, 0, 0);

        // Act
        servicio.EditarFecha(partido.Id, nuevaFecha);

        // Assert
        Assert.AreEqual(nuevaFecha, partido.Fecha);
    }

    [TestMethod]
    public void CargarResultado_ViaEstadoTorneo_ActualizaGoles()
    {
        // Arrange
        var partido = CrearPartido(FaseTorneo.PrimeraRondaEliminatoria);
        var estadoTorneo = new EstadoTorneo();
        estadoTorneo.PartidosEliminatorios.Add(partido);
        var servicio = new ServicioPartido(estadoTorneo);

        // Act
        servicio.CargarResultado(partido.Id, 3, 1);

        // Assert
        Assert.AreEqual(3, partido.GolesLocal);
        Assert.AreEqual(1, partido.GolesVisitante);
    }

    // --- ObtenerFaseAnterior ---

    [TestMethod]
    public void RegistrarFaseGenerada_PrimeraRondaEliminatoria_BloqueaFaseDeGrupos()
    {
        // Arrange
        var servicio = new ServicioPartido(new RepositorioPartidoStub());

        // Act
        servicio.RegistrarFaseGenerada(FaseTorneo.PrimeraRondaEliminatoria);

        // Assert
        Assert.IsTrue(servicio.EsFaseBloqueada(FaseTorneo.FaseDeGrupos));
    }

    [TestMethod]
    public void RegistrarFaseGenerada_Octavos_BloquePrimeraRonda()
    {
        // Arrange
        var servicio = new ServicioPartido(new RepositorioPartidoStub());

        // Act
        servicio.RegistrarFaseGenerada(FaseTorneo.Octavos);

        // Assert
        Assert.IsTrue(servicio.EsFaseBloqueada(FaseTorneo.PrimeraRondaEliminatoria));
    }

    // --- EditarEstadio via EstadoTorneo ---

    [TestMethod]
    public void EditarEstadio_ViaEstadoTorneo_ActualizaEstadio()
    {
        // Arrange
        var partido = CrearPartido();
        var grupo = new Grupo("A");
        grupo.Partidos.Add(partido);
        var fixture = new Fixture();
        fixture.Grupos.Add(grupo);
        var estadoTorneo = new EstadoTorneo { Fixture = fixture };
        var servicio = new ServicioPartido(estadoTorneo);
        var nuevoEstadio = CrearEstadio("Nuevo Estadio");

        // Act
        servicio.EditarEstadio(partido.Id, nuevoEstadio);

        // Assert
        Assert.AreEqual("Nuevo Estadio", partido.Estadio?.Nombre);
    }

    // --- ObtenerPartidos con solo eliminatorios ---

    [TestMethod]
    public void ObtenerPartidos_ViaEstadoTorneo_SoloEliminatorios_DevuelveEliminatorios()
    {
        // Arrange
        var partidoElim = CrearPartido(FaseTorneo.PrimeraRondaEliminatoria);
        var estadoTorneo = new EstadoTorneo();
        estadoTorneo.PartidosEliminatorios.Add(partidoElim);
        var servicio = new ServicioPartido(estadoTorneo);

        // Act
        var partidos = servicio.ObtenerPartidos();

        // Assert
        Assert.AreEqual(1, partidos.Count);
        Assert.AreEqual(partidoElim, partidos[0]);
    }

    // --- EstadoTorneo propiedades ---

    [TestMethod]
    public void EstadoTorneo_PropiedadesAdicionales_SeEstablecenCorrectamente()
    {
        // Arrange
        var fixture = new Fixture();
        var fixtureAnterior = new Fixture();
        var partidoAnterior = CrearPartido(FaseTorneo.Semifinal);

        // Act
        var estado = new EstadoTorneo
        {
            Fixture = fixture,
            TorneoFinalizado = true,
            FixtureAnterior = fixtureAnterior
        };
        estado.PartidosEliminatoriosAnteriores.Add(partidoAnterior);

        // Assert
        Assert.AreEqual(fixture, estado.Fixture);
        Assert.IsTrue(estado.TorneoFinalizado);
        Assert.AreEqual(fixtureAnterior, estado.FixtureAnterior);
        Assert.AreEqual(1, estado.PartidosEliminatoriosAnteriores.Count);
    }
}

file sealed class RepositorioPartidoStub : IRepositorio<Partido>
{
    private readonly List<Partido> _partidos = new();

    public void Agregar(Partido partido) => _partidos.Add(partido);

    public void Actualizar(Partido partido)
    {
        var indice = _partidos.FindIndex(p => ReferenceEquals(p, partido));
        if (indice == -1)
            throw new EntidadInexistenteExcepcion("El partido no existe.");
        _partidos[indice] = partido;
    }

    public void Eliminar(Partido partido) => _partidos.Remove(partido);

    public List<Partido> Buscar(Func<Partido, bool> predicado)
        => _partidos.Where(predicado).ToList();
}

file sealed class RepositorioNotificacionStub : IRepositorioNotificacion
{
    private readonly List<Notificacion> _notificaciones = new();
    private int _siguienteId = 1;

    public void Agregar(Notificacion notificacion)
    {
        notificacion.Id = _siguienteId++;
        _notificaciones.Add(notificacion);
    }

    public void Actualizar(Notificacion notificacion)
    {
        var indice = _notificaciones.FindIndex(n => n.Id == notificacion.Id);
        if (indice == -1)
            throw new EntidadInexistenteExcepcion("La notificación no existe.");
        _notificaciones[indice] = notificacion;
    }

    public void Eliminar(Notificacion notificacion) => _notificaciones.Remove(notificacion);

    public List<Notificacion> Buscar(Func<Notificacion, bool> predicado)
        => _notificaciones.Where(predicado).ToList();

    public List<Notificacion> ObtenerPorUsuario(int usuarioId)
        => _notificaciones.Where(n => n.UsuarioDestinatarioId == usuarioId).ToList();
}
