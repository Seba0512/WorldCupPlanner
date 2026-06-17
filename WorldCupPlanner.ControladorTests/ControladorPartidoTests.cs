using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Controladores;
using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ControladorTests;

[TestClass]
public class ControladorPartidoTests
{
    [TestMethod]
    public void BuscarPartidoPorId_DevuelvePartidoDelServicio()
    {
        var repositorio = new RepositorioPartidoControladorFake();
        var partido = CrearPartido(1);
        repositorio.Agregar(partido);
        var controlador = CrearControlador(repositorio);

        var encontrado = controlador.BuscarPartidoPorId(1);

        Assert.AreSame(partido, encontrado);
    }

    [TestMethod]
    public void ObtenerPartidos_DevuelveTodosLosPartidos()
    {
        var repositorio = new RepositorioPartidoControladorFake();
        repositorio.Agregar(CrearPartido(1));
        repositorio.Agregar(CrearPartido(2));
        var controlador = CrearControlador(repositorio);

        var partidos = controlador.ObtenerPartidos();

        Assert.AreEqual(2, partidos.Count);
    }

    [TestMethod]
    public void BuscarPartidoPorIdDTO_DevuelvePartidoDTO()
    {
        var repositorio = new RepositorioPartidoControladorFake();
        var partido = CrearPartido(1, grupo: "A");
        partido.Incidencias.Add(new TarjetaRoja
        {
            Id = 10,
            PartidoId = partido.Id,
            Equipo = partido.EquipoLocal,
            MinutoDelPartido = 77
        });
        repositorio.Agregar(partido);
        var controlador = CrearControlador(repositorio);

        var encontrado = controlador.BuscarPartidoPorIdDTO(1);

        Assert.IsNotNull(encontrado);
        Assert.AreEqual(partido.Id, encontrado.Id);
        Assert.AreEqual(partido.EquipoLocal!.Nombre, encontrado.EquipoLocal!.Nombre);
        Assert.AreEqual("A", encontrado.GrupoEtiqueta);
        Assert.AreEqual(1, encontrado.Incidencias.Count);
        Assert.AreEqual(TipoTarjeta.Roja, encontrado.Incidencias[0].Tipo);
        Assert.AreEqual(partido.EquipoLocal.Nombre, encontrado.Incidencias[0].EquipoNombre);
    }

    [TestMethod]
    public void ObtenerPartidosDTO_DevuelvePartidosDTO()
    {
        var repositorio = new RepositorioPartidoControladorFake();
        repositorio.Agregar(CrearPartido(1));
        repositorio.Agregar(CrearPartido(2));
        var controlador = CrearControlador(repositorio);

        var partidos = controlador.ObtenerPartidosDTO();

        Assert.AreEqual(2, partidos.Count);
        Assert.IsInstanceOfType(partidos[0], typeof(PartidoDTO));
    }

    [TestMethod]
    public void EditarFechaYEstadio_DelegaEdicionEnServicio()
    {
        var repositorio = new RepositorioPartidoControladorFake();
        var partido = CrearPartido(1);
        repositorio.Agregar(partido);
        var controlador = CrearControlador(repositorio);
        var nuevaFecha = new DateTime(2026, 6, 10, 18, 0, 0);
        var nuevoEstadio = CrearEstadio("Estadio Nuevo");

        controlador.EditarFechaYEstadio(partido.Id, nuevaFecha, nuevoEstadio);

        Assert.AreEqual(nuevaFecha, partido.Fecha);
        Assert.AreSame(nuevoEstadio, partido.Estadio);
    }

    [TestMethod]
    public void GuardarCambiosPartido_DelegaEnOrquestadorPartido()
    {
        var repositorioPartido = new RepositorioPartidoControladorFake();
        var partido = CrearPartido(1);
        repositorioPartido.Agregar(partido);
        var repositorioEstadio = new RepositorioGenericoControladorPartidoFake<Estadio>();
        repositorioEstadio.Agregar(CrearEstadio("Estadio Nuevo"));
        var servicioPartido = new ServicioPartido(new ProveedorPartidosRepositorio(repositorioPartido));
        var orquestador = new ServicioOrquestadorPartido(
            servicioPartido,
            new ServicioConfiguracionTorneo(),
            new ServicioNotificacion(new RepositorioNotificacionControladorPartidoFake()),
            CrearServicioUsuario(),
            new ServicioRankingDinamico(),
            new ServicioEstadio(repositorioEstadio));
        var controlador = new ControladorPartido(servicioPartido, orquestador);
        var estado = new EstadoTorneo();
        estado.AgregarPartidosEliminatorios(new[] { partido });
        var nuevaFecha = new DateTime(2026, 6, 20, 19, 0, 0);

        controlador.GuardarCambiosPartido(
            partido.Id,
            nuevaFecha,
            "Estadio Nuevo",
            null,
            null,
            null,
            estado);

        Assert.AreEqual(nuevaFecha, partido.Fecha);
        Assert.AreEqual("Estadio Nuevo", partido.Estadio!.Nombre);
    }

    [TestMethod]
    public void CargarResultado_ConGolesNegativos_MantieneValidacionDelServicio()
    {
        var repositorio = new RepositorioPartidoControladorFake();
        var partido = CrearPartido(1);
        repositorio.Agregar(partido);
        var controlador = CrearControlador(repositorio);

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            controlador.CargarResultado(partido.Id, -1, 0));
    }

    [TestMethod]
    public void EsFaseBloqueada_CuandoHayFasePosterior_DelegadaAlServicio()
    {
        var repositorio = new RepositorioPartidoControladorFake();
        repositorio.Agregar(CrearPartido(1, FaseTorneo.FaseDeGrupos));
        repositorio.Agregar(CrearPartido(2, FaseTorneo.PrimeraRondaEliminatoria));
        var controlador = CrearControlador(repositorio);

        Assert.IsTrue(controlador.EsFaseBloqueada(FaseTorneo.FaseDeGrupos));
    }

    [TestMethod]
    public void FiltrarPartidos_DevuelveCoincidenciasDelFiltro()
    {
        var controlador = CrearControlador(new RepositorioPartidoControladorFake());
        var partidos = new List<Partido>
        {
            CrearPartido(1, estadio: "Estadio A", grupo: "A"),
            CrearPartido(2, estadio: "Estadio B", grupo: "B")
        };

        var filtrados = controlador.FiltrarPartidos(partidos, new FiltroPartidos { Grupo = "A" }).ToList();

        Assert.AreEqual(1, filtrados.Count);
        Assert.AreEqual("A", filtrados[0].Grupo!.Etiqueta);
    }

    [TestMethod]
    public void FiltrarPartidosDTO_MantieneFiltros()
    {
        var controlador = CrearControlador(new RepositorioPartidoControladorFake());
        var partidos = new List<PartidoDTO>
        {
            PartidoDTO.FromEntity(CrearPartido(1, estadio: "Estadio A", grupo: "A")),
            PartidoDTO.FromEntity(CrearPartido(2, estadio: "Estadio B", grupo: "B"))
        };

        var filtrados = controlador.FiltrarPartidosDTO(partidos, new FiltroPartidos { Grupo = "A" }).ToList();

        Assert.AreEqual(1, filtrados.Count);
        Assert.AreEqual("A", filtrados[0].GrupoEtiqueta);
    }

    [TestMethod]
    public void GuardarCambiosPartido_ConPartidoDTO_DelegaEnOrquestadorPartido()
    {
        var repositorioPartido = new RepositorioPartidoControladorFake();
        var partido = CrearPartido(1);
        repositorioPartido.Agregar(partido);
        var repositorioEstadio = new RepositorioGenericoControladorPartidoFake<Estadio>();
        repositorioEstadio.Agregar(CrearEstadio("Estadio DTO"));
        var servicioPartido = new ServicioPartido(new ProveedorPartidosRepositorio(repositorioPartido));
        var orquestador = new ServicioOrquestadorPartido(
            servicioPartido,
            new ServicioConfiguracionTorneo(),
            new ServicioNotificacion(new RepositorioNotificacionControladorPartidoFake()),
            CrearServicioUsuario(),
            new ServicioRankingDinamico(),
            new ServicioEstadio(repositorioEstadio));
        var controlador = new ControladorPartido(servicioPartido, orquestador);
        var estado = new EstadoTorneo();
        estado.AgregarPartidosEliminatorios(new[] { partido });
        var partidoDto = PartidoDTO.FromEntity(partido);
        partidoDto.Fecha = new DateTime(2026, 6, 25, 20, 0, 0);
        partidoDto.Estadio = EstadioDTO.FromEntity(CrearEstadio("Estadio DTO"));

        controlador.GuardarCambiosPartido(partidoDto, null, estado);

        Assert.AreEqual(partidoDto.Fecha, partido.Fecha);
        Assert.AreEqual("Estadio DTO", partido.Estadio!.Nombre);
    }

    [TestMethod]
    public void HelpersDisponibles_DeleganListadosDelServicio()
    {
        var controlador = CrearControlador(new RepositorioPartidoControladorFake());
        var partidos = new List<Partido>
        {
            CrearPartido(1, estadio: "Estadio B", grupo: "B"),
            CrearPartido(2, estadio: "Estadio A", grupo: "A", fase: FaseTorneo.Octavos)
        };

        CollectionAssert.AreEqual(
            new[] { "Estadio A", "Estadio B" },
            controlador.ObtenerEstadiosDisponibles(partidos).ToArray());
        CollectionAssert.AreEqual(
            new[] { "A", "B" },
            controlador.ObtenerGruposDisponibles(partidos).ToArray());
        CollectionAssert.AreEqual(
            new[] { FaseTorneo.FaseDeGrupos, FaseTorneo.Octavos },
            controlador.ObtenerFasesDisponibles(partidos).ToArray());
    }

    private static ControladorPartido CrearControlador(IRepositorio<Partido> repositorio)
    {
        var servicio = new ServicioPartido(new ProveedorPartidosRepositorio(repositorio));
        return new ControladorPartido(servicio);
    }

    private static ServicioUsuario CrearServicioUsuario()
    {
        var auditoria = new ServicioAuditoria(new RepositorioAuditoriaControladorPartidoFake());
        return new ServicioUsuario(new RepositorioUsuarioControladorPartidoFake(), auditoria);
    }

    private static Partido CrearPartido(
        int id,
        FaseTorneo fase = FaseTorneo.FaseDeGrupos,
        string estadio = "Estadio Inicial",
        string? grupo = null)
    {
        return new Partido
        {
            Id = id,
            Fecha = new DateTime(2026, 6, id, 14, 0, 0),
            Estadio = CrearEstadio(estadio),
            EquipoLocal = CrearEquipo($"Local {id}"),
            EquipoVisitante = CrearEquipo($"Visitante {id}"),
            Grupo = grupo == null ? null : new Grupo(grupo),
            Fase = fase
        };
    }

    private static Equipo CrearEquipo(string nombre) => new()
    {
        Nombre = nombre,
        Confederacion = Confederacion.UEFA,
        RankingFIFA = 1200
    };

    private static Estadio CrearEstadio(string nombre) => new()
    {
        Nombre = nombre,
        Ciudad = "Ciudad",
        Capacidad = 30000
    };
}

file sealed class RepositorioPartidoControladorFake : IRepositorio<Partido>
{
    private readonly List<Partido> _partidos = new();

    public void Agregar(Partido partido) => _partidos.Add(partido);

    public void Actualizar(Partido partido)
    {
        var indice = _partidos.FindIndex(p => p.Id == partido.Id);
        if (indice == -1)
            throw new EntidadInexistenteExcepcion("El partido no existe.");
        _partidos[indice] = partido;
    }

    public void Eliminar(Partido partido) => _partidos.Remove(partido);

    public List<Partido> Buscar(Func<Partido, bool> predicado) =>
        _partidos.Where(predicado).ToList();
}

file sealed class RepositorioGenericoControladorPartidoFake<T> : IRepositorio<T>
{
    private readonly List<T> _entidades = new();

    public void Agregar(T entidad) => _entidades.Add(entidad);

    public void Actualizar(T entidad)
    {
        var indice = _entidades.FindIndex(e => ReferenceEquals(e, entidad));
        if (indice == -1)
            _entidades.Add(entidad);
        else
            _entidades[indice] = entidad;
    }

    public void Eliminar(T entidad) => _entidades.Remove(entidad);

    public List<T> Buscar(Func<T, bool> predicado) => _entidades.Where(predicado).ToList();
}

file sealed class RepositorioNotificacionControladorPartidoFake : IRepositorioNotificacion
{
    private readonly List<Notificacion> _notificaciones = new();

    public void Agregar(Notificacion entidad) => _notificaciones.Add(entidad);

    public void Actualizar(Notificacion entidad)
    {
        var indice = _notificaciones.FindIndex(n => n.Id == entidad.Id);
        if (indice == -1)
            _notificaciones.Add(entidad);
        else
            _notificaciones[indice] = entidad;
    }

    public void Eliminar(Notificacion entidad) => _notificaciones.Remove(entidad);

    public List<Notificacion> Buscar(Func<Notificacion, bool> predicado) =>
        _notificaciones.Where(predicado).ToList();

    public List<Notificacion> ObtenerPorUsuario(int usuarioId) =>
        _notificaciones.Where(n => n.UsuarioDestinatarioId == usuarioId).ToList();
}

file sealed class RepositorioUsuarioControladorPartidoFake : IRepositorioUsuario
{
    private readonly List<Usuario> _usuarios = new();

    public void Agregar(Usuario entidad) => _usuarios.Add(entidad);

    public void Actualizar(Usuario entidad)
    {
        var indice = _usuarios.FindIndex(u => u.Id == entidad.Id || u.Email == entidad.Email);
        if (indice == -1)
            _usuarios.Add(entidad);
        else
            _usuarios[indice] = entidad;
    }

    public void Eliminar(Usuario entidad) => _usuarios.Remove(entidad);

    public List<Usuario> Buscar(Func<Usuario, bool> predicado) => _usuarios.Where(predicado).ToList();

    public Usuario? ObtenerPorEmail(string email) =>
        _usuarios.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
}

file sealed class RepositorioAuditoriaControladorPartidoFake : IRepositorioConId<Auditoria>
{
    private readonly List<Auditoria> _auditorias = new();
    private int _siguienteId = 1;

    public void Agregar(Auditoria entidad)
    {
        SiguienteId();
        _auditorias.Add(entidad);
    }

    public void Actualizar(Auditoria entidad)
    {
        var indice = _auditorias.FindIndex(a => a.Id == entidad.Id);
        if (indice == -1)
            _auditorias.Add(entidad);
        else
            _auditorias[indice] = entidad;
    }

    public void Eliminar(Auditoria entidad) => _auditorias.Remove(entidad);

    public List<Auditoria> Buscar(Func<Auditoria, bool> predicado) => _auditorias.Where(predicado).ToList();

    public int SiguienteId() => _siguienteId++;
}

