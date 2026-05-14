using WorldCupPlanner.AccesoADatos.Repositorios;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ServicioTests;

[TestClass]
public class ServicioPartidoTests
{
    [TestMethod]
    public void EditarFecha_ConPartidoExistente_ActualizaFecha()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
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
        var repositorio = new RepositorioPartidoEnMemoria();
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
        var repositorio = new RepositorioPartidoEnMemoria();
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
        var servicioPartido = new ServicioPartido(new RepositorioPartidoEnMemoria());

        // Act & Assert
        Assert.ThrowsException<EntidadInexistenteExcepcion>(() =>
            servicioPartido.EditarFecha(99, new DateTime(2026, 6, 15)));
    }

    [TestMethod]
    public void EditarEstadio_ConEstadioInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);
        var estadioInvalido = CrearEstadio("");

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicioPartido.EditarEstadio(partido.Id, estadioInvalido));
    }

    [TestMethod]
    public void CargarResultado_ConGolesNegativos_DebeLanzarExcepcion()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
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
        var repositorio = new RepositorioPartidoEnMemoria();
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
        var repositorio = new RepositorioPartidoEnMemoria();
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
        var repositorio = new RepositorioPartidoEnMemoria();
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
        var repositorio = new RepositorioPartidoEnMemoria();
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
        var repositorio = new RepositorioPartidoEnMemoria();
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
        var servicioPartido = new ServicioPartido(new RepositorioPartidoEnMemoria());

        // Act & Assert
        Assert.IsFalse(servicioPartido.EsFaseBloqueada(FaseTorneo.FaseDeGrupos));
    }

    [TestMethod]
    public void EsFaseBloqueada_ConFaseDeGruposBloqueadaExplicitamente_RetornaVerdadero()
    {
        // Arrange
        var servicioPartido = new ServicioPartido(new RepositorioPartidoEnMemoria());
        servicioPartido.BloquearEdicionPartidosDeFaseDeGrupos();

        // Act & Assert
        Assert.IsTrue(servicioPartido.EsFaseBloqueada(FaseTorneo.FaseDeGrupos));
    }

    [TestMethod]
    public void BuscarPartidoPorId_ConIdInexistente_RetornaNull()
    {
        // Arrange
        var servicioPartido = new ServicioPartido(new RepositorioPartidoEnMemoria());

        // Act
        var resultado = servicioPartido.BuscarPartidoPorId(99);

        // Assert
        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void BuscarPartidoPorId_ConPartidoExistente_RetornaPartido()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
        var partido = CrearPartido();
        repositorio.Agregar(partido);
        var servicioPartido = new ServicioPartido(repositorio);

        // Act
        var resultado = servicioPartido.BuscarPartidoPorId(partido.Id);

        // Assert
        Assert.AreEqual(partido, resultado);
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
        var repositorio = new RepositorioPartidoEnMemoria();
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
        var repositorio = new RepositorioPartidoEnMemoria();
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
        var repositorio = new RepositorioPartidoEnMemoria();
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
        var repositorio = new RepositorioPartidoEnMemoria();
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
        var servicio = new ServicioPartido(new RepositorioPartidoEnMemoria());

        // Act & Assert
        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            servicio.RegistrarFaseGenerada(FaseTorneo.FaseDeGrupos));
    }

    [TestMethod]
    public void BloquearEdicionFase_Directamente_BloqueaFaseIndicada()
    {
        // Arrange
        var repositorio = new RepositorioPartidoEnMemoria();
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
        var servicio = new ServicioPartido(new RepositorioPartidoEnMemoria());

        // Act
        servicio.RegistrarFaseGenerada(FaseTorneo.PrimeraRondaEliminatoria);

        // Assert
        Assert.IsTrue(servicio.EsFaseBloqueada(FaseTorneo.FaseDeGrupos));
    }

    [TestMethod]
    public void RegistrarFaseGenerada_Octavos_BloquePrimeraRonda()
    {
        // Arrange
        var servicio = new ServicioPartido(new RepositorioPartidoEnMemoria());

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
