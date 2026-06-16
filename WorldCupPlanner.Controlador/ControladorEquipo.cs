using WorldCupPlanner.Dominio.Entidades;
using WorldCupPlanner.Dominio.DTO;
using WorldCupPlanner.Dominio.Enumeraciones;
using WorldCupPlanner.Dominio.Excepciones;
using WorldCupPlanner.Servicios;

namespace WorldCupPlanner.Controladores;

public class ControladorEquipo
{
    private readonly ServicioEquipo _servicioEquipo;
    private readonly ServicioUsuario? _servicioUsuario;

    public ControladorEquipo(ServicioEquipo servicioEquipo, ServicioUsuario? servicioUsuario = null)
    {
        _servicioEquipo = servicioEquipo;
        _servicioUsuario = servicioUsuario;
    }

    public List<Equipo> Listar() =>
        _servicioEquipo.Buscar(_ => true);

    public List<EquipoDTO> ListarDTO() =>
        Listar().Select(EquipoDTO.FromEntity).ToList();

    public List<Equipo> Buscar(Func<Equipo, bool> predicado) =>
        _servicioEquipo.Buscar(predicado);

    public Equipo ObtenerParaEdicion(string nombre) =>
        _servicioEquipo.ClonarParaEdicion(nombre);

    public EquipoDTO ObtenerParaEdicionDTO(string nombre) =>
        EquipoDTO.FromEntity(ObtenerParaEdicion(nombre));

    public Dictionary<string, string> ObtenerBanderasPorEquipo() =>
        Listar()
            .Where(e => !string.IsNullOrEmpty(e.Bandera))
            .ToDictionary(e => e.Nombre, e => e.Bandera, StringComparer.OrdinalIgnoreCase);

    public void Crear(NuevoEquipoDto equipo, UsuarioDTO? usuarioActual = null) =>
        _servicioEquipo.Crear(equipo, ResolverUsuario(usuarioActual));

    public void Crear(EquipoDTO equipo, UsuarioDTO? usuarioActual = null) =>
        _servicioEquipo.Crear(new NuevoEquipoDto
        {
            Nombre = equipo.Nombre,
            Confederacion = equipo.Confederacion,
            RankingFIFA = equipo.RankingFIFA,
            Bandera = equipo.Bandera
        }, ResolverUsuario(usuarioActual));

    public void Agregar(Equipo equipo, Usuario? usuarioActual = null) =>
        _servicioEquipo.Agregar(equipo, usuarioActual);

    public void Actualizar(Equipo equipo, string nombreOriginal, Usuario? usuarioActual = null) =>
        _servicioEquipo.EditarEquipo(equipo, nombreOriginal, usuarioActual);

    public void Actualizar(EquipoDTO equipo, string nombreOriginal, UsuarioDTO? usuarioActual = null) =>
        _servicioEquipo.EditarEquipo(equipo.ToEntity(), nombreOriginal, ResolverUsuario(usuarioActual));

    public void Eliminar(Equipo equipo, Usuario? usuarioActual = null) =>
        _servicioEquipo.Eliminar(equipo, usuarioActual);

    public void Eliminar(string nombre, UsuarioDTO? usuarioActual = null)
    {
        var equipo = _servicioEquipo.Buscar(e => string.Equals(e.Nombre, nombre, StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault() ?? throw new EntidadInexistenteExcepcion("El equipo no existe.");
        _servicioEquipo.Eliminar(equipo, ResolverUsuario(usuarioActual));
    }

    public List<Equipo> CompletarEquipos(int semilla, UsuarioDTO? usuarioActual = null) =>
        _servicioEquipo.CompletarEquipos(semilla, ResolverUsuario(usuarioActual));

    public List<EquipoDTO> CompletarEquiposDTO(int semilla, UsuarioDTO? usuarioActual = null) =>
        CompletarEquipos(semilla, usuarioActual).Select(EquipoDTO.FromEntity).ToList();

    public string ObtenerMensajeComplecion(int cantidadGenerados) =>
        _servicioEquipo.ObtenerMensajeComplecion(cantidadGenerados);

    public int ObtenerMaximoEquipos() =>
        _servicioEquipo.ObtenerMaximoEquipos();

    public IReadOnlyDictionary<Confederacion, int> ObtenerCuposMaximos() =>
        _servicioEquipo.ObtenerCuposMaximos();

    public Dictionary<Confederacion, (int registrados, int cupo)> ObtenerCuposConOcupacion(List<Equipo> equipos) =>
        _servicioEquipo.ObtenerCuposConOcupacion(equipos);

    public Dictionary<Confederacion, (int registrados, int cupo)> ObtenerCuposConOcupacionDTO(List<EquipoDTO> equipos) =>
        _servicioEquipo.ObtenerCuposConOcupacion(equipos.Select(e => e.ToEntity()).ToList());

    public int ObtenerCantidadFaltante(int cantidadInscritos) =>
        _servicioEquipo.ObtenerCantidadFaltante(cantidadInscritos);

    public bool FixtureYaFueGenerado =>
        _servicioEquipo.FixtureYaFueGenerado;

    public void RegistrarFixtureGenerado(Fixture fixture) =>
        _servicioEquipo.RegistrarFixtureGenerado(fixture);

    public void LimpiarFixtureGenerado() =>
        _servicioEquipo.LimpiarFixtureGenerado();

    private Usuario? ResolverUsuario(UsuarioDTO? usuarioActual)
    {
        if (usuarioActual == null)
            return null;

        if (_servicioUsuario == null)
            throw new InvalidOperationException("El servicio de usuario no fue configurado.");

        return _servicioUsuario.ObtenerPorEmail(usuarioActual.Email)
               ?? throw new EntidadInexistenteExcepcion($"No existe un usuario con el email '{usuarioActual.Email}'.");
    }
}
