using WorldCupPlanner.AccesoADatos.Interfaces;
using WorldCupPlanner.Controladores;
using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.ControladorTests;

[TestClass]
public class ControladorIncidenciaTests
{
    [TestMethod]
    public void AgregarTarjeta_DelegaEnServicio()
    {
        var repositorio = new RepositorioIncidenciaControladorFake();
        var controlador = CrearControlador(repositorio);

        controlador.AgregarTarjeta(10, CrearEquipo("Uruguay"), new ComandoTarjeta(TipoTarjeta.Amarilla, 25));

        var incidencias = controlador.ObtenerIncidencias(10);
        Assert.AreEqual(1, incidencias.Count);
        Assert.IsInstanceOfType(incidencias[0], typeof(TarjetaAmarilla));
        Assert.AreEqual(25, incidencias[0].MinutoDelPartido);
        Assert.AreEqual("Uruguay", incidencias[0].Equipo!.Nombre);
    }

    [TestMethod]
    public void AgregarTarjetaAPartido_DelegaPersistenciaYServicioIncidencia()
    {
        var repositorioIncidencia = new RepositorioIncidenciaControladorFake();
        var repositorioPartido = new RepositorioGenericoControladorIncidenciaFake<Partido>();
        var persistencia = new ServicioPersistenciaFixture(
            new RepositorioGenericoControladorIncidenciaFake<Grupo>(),
            repositorioPartido,
            new RepositorioTorneoArchivadoControladorIncidenciaFake());
        var controlador = new ControladorIncidencia(
            new ServicioIncidencia(repositorioIncidencia),
            persistencia);
        var partido = new Partido
        {
            Id = 20,
            EquipoLocal = CrearEquipo("Local"),
            EquipoVisitante = CrearEquipo("Visitante"),
            Fecha = new DateTime(2026, 6, 10),
            Fase = FaseTorneo.FaseDeGrupos
        };

        controlador.AgregarTarjetaAPartido(partido, "Local", new ComandoTarjeta(TipoTarjeta.Roja, 44));

        Assert.AreEqual(1, repositorioPartido.Buscar(_ => true).Count);
        var incidencias = repositorioIncidencia.ObtenerPorPartido(partido.Id);
        Assert.AreEqual(1, incidencias.Count);
        Assert.IsInstanceOfType(incidencias[0], typeof(TarjetaRoja));
        Assert.AreEqual("Local", incidencias[0].Equipo!.Nombre);
    }

    [TestMethod]
    public void AgregarTarjeta_ConMinutoInvalido_MantieneValidacionDelServicio()
    {
        var controlador = CrearControlador(new RepositorioIncidenciaControladorFake());

        Assert.ThrowsException<EntidadInvalidaExcepcion>(() =>
            controlador.AgregarTarjeta(10, CrearEquipo("Brasil"), new ComandoTarjeta(TipoTarjeta.Roja, 91)));
    }

    [TestMethod]
    public void ObtenerIncidencias_DevuelveSoloLasDelPartido()
    {
        var repositorio = new RepositorioIncidenciaControladorFake();
        var controlador = CrearControlador(repositorio);
        controlador.AgregarTarjeta(1, CrearEquipo("A"), new ComandoTarjeta(TipoTarjeta.Amarilla, 20));
        controlador.AgregarTarjeta(1, CrearEquipo("B"), new ComandoTarjeta(TipoTarjeta.Roja, 70));
        controlador.AgregarTarjeta(2, CrearEquipo("C"), new ComandoTarjeta(TipoTarjeta.Amarilla, 30));

        var incidencias = controlador.ObtenerIncidencias(1);

        Assert.AreEqual(2, incidencias.Count);
        Assert.IsTrue(incidencias.All(i => i.PartidoId == 1));
    }

    [TestMethod]
    public void ObtenerIncidenciasDTO_DevuelveDatosParaUI()
    {
        var repositorio = new RepositorioIncidenciaControladorFake();
        var controlador = CrearControlador(repositorio);
        controlador.AgregarTarjeta(1, CrearEquipo("A"), new ComandoTarjeta(TipoTarjeta.Amarilla, 20));
        controlador.AgregarTarjeta(1, CrearEquipo("B"), new ComandoTarjeta(TipoTarjeta.Roja, 70));

        var incidencias = controlador.ObtenerIncidenciasDTO(1);

        Assert.AreEqual(2, incidencias.Count);
        Assert.AreEqual(TipoTarjeta.Amarilla, incidencias[0].Tipo);
        Assert.AreEqual(20, incidencias[0].Minuto);
        Assert.AreEqual("A", incidencias[0].EquipoNombre);
        Assert.AreEqual(TipoTarjeta.Roja, incidencias[1].Tipo);
        Assert.AreEqual("B", incidencias[1].Equipo!.Nombre);
    }

    [TestMethod]
    public void AgregarTarjetaAPartido_ConPartidoIdYRol_DelegaCorrectamente()
    {
        var repositorioIncidencia = new RepositorioIncidenciaControladorFake();
        var repositorioPartido = new RepositorioGenericoControladorIncidenciaFake<Partido>();
        var persistencia = new ServicioPersistenciaFixture(
            new RepositorioGenericoControladorIncidenciaFake<Grupo>(),
            repositorioPartido,
            new RepositorioTorneoArchivadoControladorIncidenciaFake());
        var partido = new Partido
        {
            Id = 30,
            EquipoLocal = CrearEquipo("Local"),
            EquipoVisitante = CrearEquipo("Visitante"),
            Fecha = new DateTime(2026, 6, 10),
            Fase = FaseTorneo.FaseDeGrupos
        };
        repositorioPartido.Agregar(partido);
        var servicioPartido = new ServicioPartido(new ProveedorPartidosRepositorio(repositorioPartido));
        var controlador = new ControladorIncidencia(
            new ServicioIncidencia(repositorioIncidencia),
            persistencia,
            servicioPartido);

        controlador.AgregarTarjetaAPartido(partido.Id, "Visitante", new ComandoTarjeta(TipoTarjeta.Roja, 44));

        var incidencias = controlador.ObtenerIncidenciasDTO(partido.Id);
        Assert.AreEqual(1, incidencias.Count);
        Assert.AreEqual(TipoTarjeta.Roja, incidencias[0].Tipo);
        Assert.AreEqual("Visitante", incidencias[0].EquipoNombre);
    }

    [TestMethod]
    public void EliminarIncidencia_DelegaEnServicio()
    {
        var repositorio = new RepositorioIncidenciaControladorFake();
        var controlador = CrearControlador(repositorio);
        controlador.AgregarTarjeta(1, CrearEquipo("A"), new ComandoTarjeta(TipoTarjeta.Amarilla, 20));
        var incidenciaId = controlador.ObtenerIncidencias(1).Single().Id;

        controlador.EliminarIncidencia(incidenciaId);

        Assert.AreEqual(0, controlador.ObtenerIncidencias(1).Count);
    }

    [TestMethod]
    public void ResolverEquipoPorRol_DelegaResolucionDelServicio()
    {
        var controlador = CrearControlador(new RepositorioIncidenciaControladorFake());
        var local = CrearEquipo("Local");
        var visitante = CrearEquipo("Visitante");
        var partido = new Partido { EquipoLocal = local, EquipoVisitante = visitante };

        Assert.AreSame(local, controlador.ResolverEquipoPorRol(partido, "Local"));
        Assert.AreSame(visitante, controlador.ResolverEquipoPorRol(partido, "Visitante"));
    }

    private static ControladorIncidencia CrearControlador(IRepositorioIncidencia repositorio)
    {
        var servicio = new ServicioIncidencia(repositorio);
        return new ControladorIncidencia(servicio);
    }

    private static Equipo CrearEquipo(string nombre) => new()
    {
        Nombre = nombre,
        Confederacion = Confederacion.CONMEBOL,
        RankingFIFA = 1200
    };
}

file sealed class RepositorioIncidenciaControladorFake : IRepositorioIncidencia
{
    private readonly List<Incidencia> _incidencias = new();
    private int _siguienteId = 1;

    public void Agregar(Incidencia entidad)
    {
        entidad.Id = _siguienteId++;
        _incidencias.Add(entidad);
    }

    public void Actualizar(Incidencia entidad)
    {
        var indice = _incidencias.FindIndex(i => i.Id == entidad.Id);
        if (indice == -1)
            throw new EntidadInexistenteExcepcion("La incidencia no existe.");
        _incidencias[indice] = entidad;
    }

    public void Eliminar(Incidencia entidad)
    {
        var existente = _incidencias.FirstOrDefault(i => i.Id == entidad.Id)
            ?? throw new EntidadInexistenteExcepcion("La incidencia no existe.");
        _incidencias.Remove(existente);
    }

    public List<Incidencia> Buscar(Func<Incidencia, bool> predicado) =>
        _incidencias.Where(predicado).ToList();

    public List<Incidencia> ObtenerPorPartido(int partidoId) =>
        _incidencias.Where(i => i.PartidoId == partidoId).ToList();
}

file sealed class RepositorioGenericoControladorIncidenciaFake<T> : IRepositorio<T>
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

file sealed class RepositorioTorneoArchivadoControladorIncidenciaFake : IRepositorioTorneoArchivado
{
    private TorneoArchivado? _torneoArchivado;

    public void Guardar(TorneoArchivado torneoArchivado) => _torneoArchivado = torneoArchivado;

    public TorneoArchivado? Obtener() => _torneoArchivado;
}

