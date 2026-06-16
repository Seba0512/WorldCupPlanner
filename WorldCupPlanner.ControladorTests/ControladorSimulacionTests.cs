using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Controladores;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ControladorTests;

[TestClass]
public class ControladorSimulacionTests
{
    [TestMethod]
    public void ObtenerMotoresDisponibles_DelegaEnServicioSimulacion()
    {
        var servicio = new ServicioSimulacion();
        var controlador = CrearControlador(servicio);

        var esperados = servicio.ObtenerMotoresDisponibles().ToList();
        var obtenidos = controlador.ObtenerMotoresDisponibles().ToList();

        Assert.AreEqual(esperados.Count, obtenidos.Count);
        CollectionAssert.AreEqual(
            esperados.Select(m => m.Tipo).ToArray(),
            obtenidos.Select(m => m.Tipo).ToArray());
        CollectionAssert.AreEqual(
            esperados.Select(m => m.Nombre).ToArray(),
            obtenidos.Select(m => m.Nombre).ToArray());
    }

    [TestMethod]
    public void ObtenerNombreMotor_DelegaEnServicioSimulacion()
    {
        var servicio = new ServicioSimulacion();
        var controlador = CrearControlador(servicio);

        foreach (MotorSimulacion motor in Enum.GetValues<MotorSimulacion>())
        {
            Assert.AreEqual(servicio.ObtenerNombreMotor(motor), controlador.ObtenerNombreMotor(motor));
        }
    }

    [TestMethod]
    public void SimularYPersistirPartidos_DelegaEnOrquestadorYGuardaResultado()
    {
        var contexto = CrearContextoSimulacion();
        var controlador = CrearControlador(repositorioPartido: contexto.RepositorioPartidos);

        controlador.SimularYPersistirPartidos(
            new List<Partido> { contexto.Partido },
            CrearConfiguracion(),
            usuarioActual: null,
            contexto.EstadoTorneo);

        Assert.IsTrue(contexto.Partido.EstaJugado());
        Assert.AreEqual(1, contexto.RepositorioPartidos.Buscar(_ => true).Count);
        Assert.AreSame(contexto.Partido, contexto.RepositorioPartidos.Buscar(_ => true)[0]);
    }

    [TestMethod]
    public void SimularYPersistirPartidos_SinPendientes_PropagaValidacionDelOrquestador()
    {
        var controlador = CrearControlador();

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            controlador.SimularYPersistirPartidos(
                new List<Partido>(),
                CrearConfiguracion(),
                usuarioActual: null,
                new EstadoTorneo()));
    }

    [TestMethod]
    public void SimularPartido_ConPartidoId_ResuelvePartidoYDelega()
    {
        var contexto = CrearContextoSimulacion();
        var controlador = CrearControlador(
            repositorioPartido: contexto.RepositorioPartidos,
            servicioConfiguracionTorneo: CrearServicioConfiguracion());

        controlador.SimularPartido(
            contexto.Partido.Id,
            contexto.EstadoTorneo,
            usuarioActual: null);

        Assert.IsTrue(contexto.Partido.EstaJugado());
        Assert.AreEqual(1, contexto.RepositorioPartidos.Buscar(_ => true).Count);
        Assert.AreSame(contexto.Partido, contexto.RepositorioPartidos.Buscar(_ => true)[0]);
    }

    [TestMethod]
    public void SimularPartido_ConPartidoInexistente_LanzaExcepcion()
    {
        var controlador = CrearControlador(
            servicioConfiguracionTorneo: CrearServicioConfiguracion());

        Assert.ThrowsException<EntidadInexistenteExcepcion>(() =>
            controlador.SimularPartido(
                999,
                new EstadoTorneo(),
                usuarioActual: null));
    }

    [TestMethod]
    public void EsPartidoSimulable_ConPartidoId_UsaReglaDeFixture()
    {
        var contexto = CrearContextoSimulacion();
        var controlador = CrearControlador(servicioFixture: new ServicioFixture());

        Assert.IsTrue(controlador.EsPartidoSimulable(
            contexto.Partido.Id,
            contexto.EstadoTorneo,
            FaseTorneo.FaseDeGrupos));
    }

    [TestMethod]
    public void SimularJornadaOFecha_ConJornada_ResuelvePartidosYDelega()
    {
        var contexto = CrearContextoSimulacion();
        var controlador = CrearControlador(
            repositorioPartido: contexto.RepositorioPartidos,
            servicioConfiguracionTorneo: CrearServicioConfiguracion(),
            servicioFixture: new ServicioFixture());

        controlador.SimularJornadaOFecha(
            FaseTorneo.FaseDeGrupos,
            ServicioFixture.PrimeraJornada,
            contexto.EstadoTorneo,
            usuarioActual: null);

        Assert.IsTrue(contexto.Partido.EstaJugado());
        Assert.AreEqual(1, contexto.RepositorioPartidos.Buscar(_ => true).Count);
        Assert.AreSame(contexto.Partido, contexto.RepositorioPartidos.Buscar(_ => true)[0]);
    }

    [TestMethod]
    public void SimularJornadaOFecha_SinPartidosPendientes_PropagaValidacionDelOrquestador()
    {
        var contexto = CrearContextoSimulacion();
        var controlador = CrearControlador(
            servicioConfiguracionTorneo: CrearServicioConfiguracion(),
            servicioFixture: new ServicioFixture());

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            controlador.SimularJornadaOFecha(
                FaseTorneo.FaseDeGrupos,
                ServicioFixture.SegundaJornada,
                contexto.EstadoTorneo,
                usuarioActual: null));

        Assert.IsFalse(contexto.Partido.EstaJugado());
    }

    [TestMethod]
    public void HayPartidosJornadaOFechaSimulables_UsaReglaDeFixture()
    {
        var contexto = CrearContextoSimulacion();
        var controlador = CrearControlador(servicioFixture: new ServicioFixture());

        Assert.IsTrue(controlador.HayPartidosJornadaOFechaSimulables(
            FaseTorneo.FaseDeGrupos,
            ServicioFixture.PrimeraJornada,
            contexto.EstadoTorneo));

        Assert.IsFalse(controlador.HayPartidosJornadaOFechaSimulables(
            FaseTorneo.FaseDeGrupos,
            ServicioFixture.SegundaJornada,
            contexto.EstadoTorneo));
    }

    [TestMethod]
    public void SimularFaseActiva_ConFase_ResuelvePartidosYDelega()
    {
        var contexto = CrearContextoSimulacion();
        var controlador = CrearControlador(
            repositorioPartido: contexto.RepositorioPartidos,
            servicioConfiguracionTorneo: CrearServicioConfiguracion(),
            servicioFixture: new ServicioFixture());

        controlador.SimularFaseActiva(
            FaseTorneo.FaseDeGrupos,
            contexto.EstadoTorneo,
            usuarioActual: null);

        Assert.IsTrue(contexto.Partido.EstaJugado());
        Assert.AreEqual(1, contexto.RepositorioPartidos.Buscar(_ => true).Count);
        Assert.AreSame(contexto.Partido, contexto.RepositorioPartidos.Buscar(_ => true)[0]);
    }

    [TestMethod]
    public void SimularFaseActiva_SinPartidosPendientes_PropagaValidacionDelOrquestador()
    {
        var contexto = CrearContextoSimulacion();
        contexto.Partido.GolesLocal = 1;
        contexto.Partido.GolesVisitante = 0;
        var controlador = CrearControlador(
            servicioConfiguracionTorneo: CrearServicioConfiguracion(),
            servicioFixture: new ServicioFixture());

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            controlador.SimularFaseActiva(
                FaseTorneo.FaseDeGrupos,
                contexto.EstadoTorneo,
                usuarioActual: null));
    }

    [TestMethod]
    public void HayPartidosFaseActivaSimulables_UsaReglaDeFixture()
    {
        var contexto = CrearContextoSimulacion();
        var controlador = CrearControlador(servicioFixture: new ServicioFixture());

        Assert.IsTrue(controlador.HayPartidosFaseActivaSimulables(
            FaseTorneo.FaseDeGrupos,
            contexto.EstadoTorneo));

        contexto.Partido.GolesLocal = 1;
        contexto.Partido.GolesVisitante = 0;

        Assert.IsFalse(controlador.HayPartidosFaseActivaSimulables(
            FaseTorneo.FaseDeGrupos,
            contexto.EstadoTorneo));
    }

    private static ControladorSimulacion CrearControlador(
        ServicioSimulacion? servicioSimulacion = null,
        RepositorioGenericoControladorSimulacionFake<Partido>? repositorioPartido = null,
        ServicioConfiguracionTorneo? servicioConfiguracionTorneo = null,
        ServicioFixture? servicioFixture = null)
    {
        servicioSimulacion ??= new ServicioSimulacion();
        repositorioPartido ??= new RepositorioGenericoControladorSimulacionFake<Partido>();

        var auditoria = new ServicioAuditoria(new RepositorioAuditoriaControladorSimulacionFake());
        var persistencia = new ServicioPersistenciaFixture(
            new RepositorioGenericoControladorSimulacionFake<Grupo>(),
            repositorioPartido,
            new RepositorioTorneoArchivadoControladorSimulacionFake());
        var incidencia = new ServicioIncidencia(new RepositorioIncidenciaControladorSimulacionFake());
        var usuario = new ServicioUsuario(new RepositorioUsuarioControladorSimulacionFake(), auditoria);
        var notificacion = new ServicioNotificacion(new RepositorioNotificacionControladorSimulacionFake());
        var consultas = new ServicioConsultasTorneo();
        var ranking = new ServicioRankingDinamico();
        var orquestador = new ServicioOrquestadorSimulacion(
            servicioSimulacion,
            persistencia,
            incidencia,
            ranking,
            usuario,
            notificacion,
            consultas);

        return new ControladorSimulacion(
            servicioSimulacion,
            orquestador,
            servicioConfiguracionTorneo: servicioConfiguracionTorneo,
            servicioFixture: servicioFixture);
    }

    private static ContextoSimulacionControlador CrearContextoSimulacion()
    {
        var local = CrearEquipo(1, "Local");
        var visitante = CrearEquipo(2, "Visitante");
        var partido = new Partido
        {
            Id = 1,
            Fecha = new DateTime(2026, 6, 1, 14, 0, 0),
            EquipoLocal = local,
            EquipoVisitante = visitante,
            Estadio = new Estadio { Nombre = "Estadio", Ciudad = "Montevideo", Capacidad = 60000 },
            Fase = FaseTorneo.FaseDeGrupos
        };
        var grupo = new Grupo("A", new List<Equipo> { local, visitante });
        grupo.Partidos.Add(partido);
        var estado = new EstadoTorneo();
        estado.EstablecerFixture(new Fixture { Grupos = { grupo } });

        return new ContextoSimulacionControlador(
            partido,
            estado,
            new RepositorioGenericoControladorSimulacionFake<Partido>());
    }

    private static ConfiguracionTorneo CrearConfiguracion() => new()
    {
        SemillaSimulation = 7,
        MotorSimulacion = MotorSimulacion.RankingActualYSemilla
    };

    private static ServicioConfiguracionTorneo CrearServicioConfiguracion()
    {
        var servicio = new ServicioConfiguracionTorneo();
        servicio.RegistrarFixtureGenerado(CrearConfiguracion());
        return servicio;
    }

    private static Equipo CrearEquipo(int id, string nombre) => new()
    {
        Id = id,
        Nombre = nombre,
        Confederacion = Confederacion.UEFA,
        RankingFIFA = 1500,
        RankingInicial = 1500
    };
}

internal sealed record ContextoSimulacionControlador(
    Partido Partido,
    EstadoTorneo EstadoTorneo,
    RepositorioGenericoControladorSimulacionFake<Partido> RepositorioPartidos);

internal sealed class RepositorioGenericoControladorSimulacionFake<T> : IRepositorio<T>
{
    private readonly List<T> _entidades = new();

    public void Agregar(T entidad) => _entidades.Add(entidad);

    public void Actualizar(T entidad)
    {
    }

    public void Eliminar(T entidad) => _entidades.Remove(entidad);

    public List<T> Buscar(Func<T, bool> predicado) =>
        _entidades.Where(predicado).ToList();
}

file sealed class RepositorioAuditoriaControladorSimulacionFake : IRepositorioConId<Auditoria>
{
    private readonly List<Auditoria> _auditorias = new();
    private int _siguienteId = 1;

    public int SiguienteId() => _siguienteId++;

    public void Agregar(Auditoria entidad) => _auditorias.Add(entidad);

    public void Actualizar(Auditoria entidad)
    {
    }

    public void Eliminar(Auditoria entidad) => _auditorias.Remove(entidad);

    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado) =>
        _auditorias.Where(predicado).ToList();
}

file sealed class RepositorioTorneoArchivadoControladorSimulacionFake : IRepositorioTorneoArchivado
{
    private TorneoArchivado? _torneoArchivado;

    public void Guardar(TorneoArchivado torneoArchivado) => _torneoArchivado = torneoArchivado;

    public TorneoArchivado? Obtener() => _torneoArchivado;
}

file sealed class RepositorioIncidenciaControladorSimulacionFake : IRepositorioIncidencia
{
    private readonly List<Incidencia> _incidencias = new();

    public List<Incidencia> ObtenerPorPartido(int partidoId) =>
        Buscar(i => i.PartidoId == partidoId);

    public void Agregar(Incidencia entidad) => _incidencias.Add(entidad);

    public void Actualizar(Incidencia entidad)
    {
    }

    public void Eliminar(Incidencia entidad) => _incidencias.Remove(entidad);

    public List<Incidencia> Buscar(Func<Incidencia, bool> predicado) =>
        _incidencias.Where(predicado).ToList();
}

file sealed class RepositorioUsuarioControladorSimulacionFake : IRepositorioUsuario
{
    private readonly List<Usuario> _usuarios = new();

    public Usuario? ObtenerPorEmail(string email) =>
        _usuarios.FirstOrDefault(u => u.Email == email);

    public void Agregar(Usuario entidad) => _usuarios.Add(entidad);

    public void Actualizar(Usuario entidad)
    {
    }

    public void Eliminar(Usuario entidad) => _usuarios.Remove(entidad);

    public List<Usuario> Buscar(Func<Usuario, bool> predicado) =>
        _usuarios.Where(predicado).ToList();
}

file sealed class RepositorioNotificacionControladorSimulacionFake : IRepositorioNotificacion
{
    private readonly List<Notificacion> _notificaciones = new();

    public List<Notificacion> ObtenerPorUsuario(int usuarioId) =>
        Buscar(n => n.UsuarioDestinatarioId == usuarioId);

    public void Agregar(Notificacion entidad) => _notificaciones.Add(entidad);

    public void Actualizar(Notificacion entidad)
    {
    }

    public void Eliminar(Notificacion entidad) => _notificaciones.Remove(entidad);

    public List<Notificacion> Buscar(Func<Notificacion, bool> predicado) =>
        _notificaciones.Where(predicado).ToList();
}

